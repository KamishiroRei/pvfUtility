using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pvf110.Core;

/// <summary>
/// 一条密钥材料（一个客户端版本一行）：外层包装 AES 密钥 + Builder RSA 私钥 PEM，
/// 可选携带与该客户端配套的 <c>sk.dat</c> 字节（base64）。
/// </summary>
public sealed record Pvf110KeyMaterialEntry
{
    public required string Label { get; init; }
    public required string AesKeyHex { get; init; }
    public required string PrivateKeyPem { get; init; }

    /// <summary>与该客户端配套的 sk.dat（base64）。为空表示只提供密钥集、不提供 sk.dat 回退。</summary>
    public string? SkDatBase64 { get; init; }

    /// <summary>来源：内置（编译进工具）或外部材料文件路径；仅用于诊断。</summary>
    public string? Source { get; init; }

    private byte[]? _skDat;

    /// <summary>惰性解码的 sk.dat 字节；未提供时返回 null。调用方不得就地修改。</summary>
    [JsonIgnore]
    public byte[]? SkDatBytes => string.IsNullOrWhiteSpace(SkDatBase64)
        ? null
        : _skDat ??= Convert.FromBase64String(SkDatBase64);

    [JsonIgnore]
    public bool FromFile => !string.IsNullOrEmpty(Source) && Source != EmbeddedSource;

    internal const string EmbeddedSource = "<embedded>";
}

/// <summary>
/// Pvf110 密钥材料注册表（**通用机制，按数据驱动，不按版本分叉代码**）。
///
/// 查找顺序（先文件后内置，逐条用归档 header magic 验证择优）：
/// <list type="number">
/// <item><c>PVF_KEY_MATERIAL</c> 指定的材料文件；</item>
/// <item>PVF 同目录及其上溯 3 层内的 <c>Pvf110KeyMaterial.json</c>；</item>
/// <item>工具自身目录（GUI/CLI 可执行文件所在目录）的 <c>Pvf110KeyMaterial.json</c>；</item>
/// <item>编译进工具的内置条目（见 <see cref="EmbeddedEntries"/>，随工具发布，零外部文件即可工作）。</item>
/// </list>
///
/// 新增客户端版本**不需要改代码**：用
/// <c>Pvf110.Cli.dll keyset-probe &lt;新客户端EXE&gt; --emit</c>（或 <c>--write &lt;材料文件&gt;</c>）
/// 取出该客户端的 AES/PEM/sk.dat base64，写入材料文件即可；内置条目保留兜底。
/// 材料文件是数据、不是路径依赖：不存在时工具照常工作。
/// </summary>
public static class Pvf110KeyMaterial
{
    /// <summary>材料文件名（与 PVF 同目录、PVF 上溯层或工具目录内自动发现）。</summary>
    public const string MaterialFileName = "Pvf110KeyMaterial.json";

    /// <summary>PVF 目录与其上溯层数（与其他配套件探测保持一致）。</summary>
    private const int MaxParentLevels = 3;

    /// <summary>显式材料文件路径（环境变量 <c>PVF_KEY_MATERIAL</c>）。</summary>
    public static string? ExplicitMaterialPath =>
        Environment.GetEnvironmentVariable("PVF_KEY_MATERIAL") is { Length: > 0 } s ? Path.GetFullPath(s) : null;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    /// <summary>
    /// 内置条目：随工具编译发布，保证"零外部文件"可用。
    /// 数据行可以增删，代码不含任何版本分支；外部材料文件的条目排在前面、优先验证。
    /// </summary>
    public static readonly IReadOnlyList<Pvf110KeyMaterialEntry> EmbeddedEntries = new[]
    {
        new Pvf110KeyMaterialEntry
        {
            Label = "115 client (DFO.exe 1d3948784e5e0f77)",
            AesKeyHex = B115AesKeyHex,
            PrivateKeyPem = B115PrivateKeyPem,
            SkDatBase64 = B115SkDatBase64,
            Source = Pvf110KeyMaterialEntry.EmbeddedSource,
        },
    };

    private static IReadOnlyList<Pvf110KeyMaterialEntry>? _cached;
    private static string? _cachedFrom;
    private static readonly object Gate = new();

    /// <summary>本次进程内解析出的材料条目（外部文件条目在前，内置条目在后）。</summary>
    public static IReadOnlyList<Pvf110KeyMaterialEntry> Entries => EntriesFor(null);

    /// <summary>
    /// 解析材料条目：<paramref name="pvfPath"/> 用于发现与 PVF 同目录/上溯层的材料文件。
    /// 结果按发现路径缓存；文件缺失时退回内置条目。
    /// </summary>
    public static IReadOnlyList<Pvf110KeyMaterialEntry> EntriesFor(string? pvfPath)
    {
        string? file = ResolveMaterialFile(pvfPath);
        lock (Gate)
        {
            if (_cached != null && _cachedFrom == file) return _cached;
            List<Pvf110KeyMaterialEntry> list = new();
            if (file != null)
            {
                try
                {
                    foreach (Pvf110KeyMaterialEntry e in ReadFile(file)) list.Add(e);
                }
                catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
                {
                    Console.Error.WriteLine($"pvf110 key material: 材料文件解析失败（已忽略，继续用内置条目）: {file}: {ex.Message}");
                }
            }
            list.AddRange(EmbeddedEntries);
            _cached = list;
            _cachedFrom = file;
            return list;
        }
    }

    /// <summary>当前生效的材料文件路径（无外部文件时为 null）。</summary>
    public static string? ResolveMaterialFile(string? pvfPath)
    {
        string? explicitPath = ExplicitMaterialPath;
        if (explicitPath != null) return File.Exists(explicitPath) ? explicitPath : null;
        if (!string.IsNullOrEmpty(pvfPath))
        {
            string? dir = Path.GetDirectoryName(Path.GetFullPath(pvfPath));
            for (int level = 0; level <= MaxParentLevels && !string.IsNullOrEmpty(dir); level++)
            {
                string candidate = Path.Combine(dir, MaterialFileName);
                if (File.Exists(candidate)) return candidate;
                dir = Path.GetDirectoryName(dir);
            }
        }
        string toolDir = AppContext.BaseDirectory;
        string beside = Path.Combine(toolDir, MaterialFileName);
        return File.Exists(beside) ? beside : null;
    }

    /// <summary>按归档与材料条目取第一条提供 sk.dat 的条目；<paramref name="fromFile"/> 表示是否来自外部材料文件。</summary>
    public static (byte[] Bytes, string Source)? FindSkDat(string? pvfPath)
    {
        foreach (Pvf110KeyMaterialEntry e in EntriesFor(pvfPath))
        {
            byte[]? bytes = e.SkDatBytes;
            if (bytes is { Length: > 0 })
                return (bytes, e.FromFile ? "material" : "embedded");
        }
        return null;
    }

    /// <summary>解析材料文件（JSON）。</summary>
    public static IReadOnlyList<Pvf110KeyMaterialEntry> ReadFile(string path)
    {
        Pvf110KeyMaterialFile? parsed = JsonSerializer.Deserialize<Pvf110KeyMaterialFile>(File.ReadAllText(path), JsonOptions);
        if (parsed?.Entries == null) return Array.Empty<Pvf110KeyMaterialEntry>();
        List<Pvf110KeyMaterialEntry> result = new(parsed.Entries.Count);
        foreach (Pvf110KeyMaterialEntry e in parsed.Entries)
        {
            if (string.IsNullOrWhiteSpace(e.AesKeyHex) || string.IsNullOrWhiteSpace(e.PrivateKeyPem)) continue;
            result.Add(e with { Source = Path.GetFullPath(path) });
        }
        return result;
    }

    /// <summary>把一条条目写入/追加到材料文件（供 <c>keyset-probe --write</c> 使用）。</summary>
    public static void AppendToFile(string path, Pvf110KeyMaterialEntry entry)
    {
        List<Pvf110KeyMaterialEntry> existing = new();
        if (File.Exists(path))
        {
            foreach (Pvf110KeyMaterialEntry e in ReadFile(path)) existing.Add(e with { Source = null });
        }
        bool replaced = false;
        for (int i = 0; i < existing.Count; i++)
        {
            if (existing[i].AesKeyHex == entry.AesKeyHex)
            {
                existing[i] = entry with { Source = null };
                replaced = true;
            }
        }
        if (!replaced) existing.Add(entry with { Source = null });

        var payload = new Pvf110KeyMaterialFile { SchemaVersion = 1, Entries = existing };
        string? dir = Path.GetDirectoryName(Path.GetFullPath(path));
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(path, JsonSerializer.Serialize(payload, JsonOptions));
        lock (Gate)
        {
            _cached = null;
            _cachedFrom = null;
        }
    }

    private sealed class Pvf110KeyMaterialFile
    {
        public int SchemaVersion { get; set; } = 1;
        public List<Pvf110KeyMaterialEntry> Entries { get; set; } = new();
    }

    // ── 内置条目数据（115 美服固定版客户端；取自该客户端 DFO.exe，见 keyset-probe）──────
    //    用户明确要求：单机改版不做程序安全设计，密钥直接内置，打开归档不需要任何配套文件。

    private const string B115AesKeyHex =
        "BFCB117EC7208BF46384CC38B7D0AA798847213137C76AD1189B8D470AA50B4E";

    private const string B115PrivateKeyPem = """
-----BEGIN PRIVATE KEY-----
MIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAKx6SxdMAJ94JO5t
NZPVsHmAd4ImYr65VuDBwU8e0IfpE3KsaWsA8fTt3NxABLkK5R4IOacOprLCoXgE
O7SOBfOS1cPTkc12XPFI9WFPtuDQyIgOarC+wGU0Gqeh/dAKAFdfhz2+uRKopXcG
gmFeSsF4s5+Ak+dcUgizuJftv4tFAgMBAAECgYBomhpkXKGcFT9Aolb1+4j1hyXz
dtg4FytLT/aux19srbwvtcSRMpeLFZ72rJSwrJzbo91jJY2PqF5I7ThQfjm4yXsN
/ct4vFa93AtpZhd6wyNwDG2mJe25c8sqJnkgRjeEeBnLhk7Gi/glwtO6sB6Ikf27
ktq+bXt+VGP8JVe2mQJBAOFSQ0dC1PKdHblmwxqkkNJ/GgglnGz/ilbIweJUIyoR
km2iXR4rC+zLagmcfknJleEAhvCIoqAnXvS0/GGR+2sCQQDD9iC3A93P2HE3Tv+V
gwxpUgb1RI91jXx+3dh+4ofem3IG94urySlAkYcnEDuNoxwAkYDFzDd6ax/zRgXQ
WXAPAkEAvqGRqD3D2ovRNSXbFbR7jovYY2ImyRUeXrQ1TR4wLgx13WnL7JUw3qNu
0Djlo9n4g0el4uVG6cbFkLV6/bW7hwJAXalaYZ87eYheNK1Rg8irOfu2z6wBVZJW
mty2CY+EmWV6ztGqtGTljrMAAi/EByKa07q37dJ6Ac0J7GDfjoHxCQI/dhQ3/z11
oZdf85T29pbk97M+sNXkpPgYdXebEdmN1b17k3qJgRKp5jWIMcdMCr96At/CfUol
bREzhaj4Z6IG
-----END PRIVATE KEY-----
""";

    private const string B115SkDatBase64 =
        "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"; // 占位：由 tools 脚本替换为真实 base64（见下）
}
