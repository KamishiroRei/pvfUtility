using System.Text;

namespace Pvf110.Core;

/// <summary>
/// 从客户端 EXE 现场派生 Pvf110 / Builder 外层包装所需的密钥材料：
/// RSA 私钥（PEM 字面量）与 metadata AES-256 密钥（64 位十六进制字面量）。
///
/// 设计约束：
/// - 不在源码、配置或文档中保存任何客户端密钥；本类只做运行时扫描与返回。
/// - 不依赖固定文件偏移或固定盘符：给定 EXE 路径后按数据区内容特征扫描，
///   候选组合交由 <see cref="Pvf110Crypto.OpenLogicalStream"/> 逐组验证（header magic "nkpi"）。
/// - 同一客户端的不同版本（不同密钥）可用同一实现处理，新增客户端无需改源码。
/// </summary>
public static class Pvf110ClientKeys
{
    private static readonly byte[] BeginMarker = Encoding.ASCII.GetBytes("-----BEGIN ");
    private static readonly byte[] EndMarker = Encoding.ASCII.GetBytes("-----END ");

    /// <summary>候选组合上限，避免异常 EXE 造成组合爆炸。</summary>
    private const int MaxCandidates = 256;

    /// <summary>优先把"十六进制字面量紧跟私钥之后"的组合排在前面（同一密钥表相邻存储）。</summary>
    private const int SameTableWindow = 0x100000;

    /// <summary>PVF 同目录下的客户端主程序名（按优先级）。</summary>
    private static readonly string[] ClientExeNames = { "DFO.exe", "DNF.exe" };

    /// <summary>PVF 目录与其上溯层数（客户端主程序与 PVF 不总在同一层）。</summary>
    private const int MaxParentLevels = 3;

    /// <summary>已注册过的客户端 EXE（路径 → 长度@最后写入时间），避免同一进程内重复扫描 200 MB 级 EXE。</summary>
    private static readonly Dictionary<string, string> RegisteredExeSignature = new(StringComparer.OrdinalIgnoreCase);
    private static readonly object RegistrationGate = new();

    /// <summary>最近一次成功解析出的客户端 EXE 路径（诊断用；未找到为 null）。</summary>
    public static string? LastClientExe { get; private set; }

    /// <summary>
    /// 返回与 PVF 配套的客户端主程序路径：先查 PVF 同目录，再逐级上溯至 <see cref="MaxParentLevels"/> 层；
    /// 都不存在时返回 null。不依赖固定盘符或绝对路径。
    /// </summary>
    public static string? FindClientExe(string pvfPath)
    {
        string? dir = Path.GetDirectoryName(Path.GetFullPath(pvfPath));
        for (int level = 0; level <= MaxParentLevels && !string.IsNullOrEmpty(dir); level++)
        {
            foreach (string name in ClientExeNames)
            {
                string candidate = Path.Combine(dir, name);
                if (File.Exists(candidate)) return candidate;
            }
            dir = Path.GetDirectoryName(dir);
        }
        return null;
    }

    /// <summary>
    /// 幂等地把 PVF 客户端派生的密钥集注册给 <see cref="Pvf110Crypto"/>。
    ///
    /// 调用方只需给出 PVF 路径：客户端 EXE 由 <see cref="FindClientExe"/> 自动定位（可用
    /// <paramref name="clientExePath"/> 覆盖），密钥在运行时从该 EXE 现场派生并全部注册，
    /// 由打开流程按 header magic 择优。同一 EXE 在同一进程内只扫描一次。
    ///
    /// 版本固定与否不影响本机制：客户端换了密钥也能自动适配，且源码与配置中始终不保存密钥。
    /// </summary>
    /// <returns>实际使用的客户端 EXE 路径；未找到时返回 null（此时仅剩内置密钥集）。</returns>
    public static string? EnsureRegistered(string pvfPath, string? clientExePath = null)
    {
        string? exe = string.IsNullOrWhiteSpace(clientExePath) ? FindClientExe(pvfPath) : clientExePath;
        if (exe == null || !File.Exists(exe)) return null;
        string full = Path.GetFullPath(exe);
        LastClientExe = full;

        var info = new FileInfo(full);
        string signature = $"{info.Length}@{info.LastWriteTimeUtc.Ticks}";
        lock (RegistrationGate)
        {
            if (RegisteredExeSignature.TryGetValue(full, out string? known) && known == signature)
                return full;
            foreach (var (aesKeyHex, pem) in ExtractCandidates(full))
                Pvf110Crypto.AddRuntimeKeySet(aesKeyHex, pem);
            RegisteredExeSignature[full] = signature;
        }
        return full;
    }

    /// <summary>
    /// 扫描客户端 EXE，返回 (AES 密钥十六进制, RSA 私钥 PEM) 候选组合。
    /// 调用方应全部注册给 <see cref="Pvf110Crypto.AddRuntimeKeySet"/>，
    /// 由打开流程按 header magic 验证后择优。
    /// </summary>
    public static IReadOnlyList<(string AesKeyHex, string Pem)> ExtractCandidates(string exePath)
    {
        byte[] image = File.ReadAllBytes(exePath);
        List<(int offset, string pem)> pems = ScanPemBlocks(image);
        List<(int offset, string hex)> hexes = ScanHex64Literals(image);
        if (pems.Count == 0 || hexes.Count == 0) return Array.Empty<(string, string)>();

        var ordered = new List<(string AesKeyHex, string Pem)>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var hex in hexes)
        {
            foreach (var pem in pems)
            {
                if (Math.Abs(hex.offset - pem.offset) > SameTableWindow) continue;
                if (seen.Add(pem.pem + "\n" + hex.hex))
                    ordered.Add((hex.hex, pem.pem));
            }
        }
        foreach (var pem in pems)
        {
            foreach (var hex in hexes)
            {
                if (seen.Add(pem.pem + "\n" + hex.hex))
                    ordered.Add((hex.hex, pem.pem));
            }
        }
        if (ordered.Count > MaxCandidates) ordered.RemoveRange(MaxCandidates, ordered.Count - MaxCandidates);
        return ordered;
    }

    private static List<(int offset, string pem)> ScanPemBlocks(byte[] image)
    {
        var result = new List<(int, string)>();
        ReadOnlySpan<byte> span = image;
        int search = 0;
        while (search < span.Length)
        {
            int begin = span[search..].IndexOf(BeginMarker);
            if (begin < 0) break;
            begin += search;
            int lineEnd = span[begin..].IndexOf((byte)'\n');
            if (lineEnd < 0) break;
            string header = Encoding.ASCII.GetString(span.Slice(begin, lineEnd)).Trim();
            string endTag = header.Replace("-----BEGIN ", "-----END ");
            int endMarkerAt = span[(begin + lineEnd)..].IndexOf(Encoding.ASCII.GetBytes("-----END "));
            if (endMarkerAt < 0) break;
            endMarkerAt += begin + lineEnd;
            int endLineEnd = span[endMarkerAt..].IndexOf((byte)'\n');
            int stop = endLineEnd < 0 ? span.Length : endMarkerAt + endLineEnd + 1;
            string body = Encoding.ASCII.GetString(span[begin..stop]);
            if (body.Contains(endTag, StringComparison.Ordinal))
                result.Add((begin, body.Trim() + "\n"));
            search = stop;
        }
        return result;
    }

    private static List<(int offset, string hex)> ScanHex64Literals(byte[] image)
    {
        var result = new List<(int, string)>();
        int runStart = -1;
        for (int i = 0; i <= image.Length; i++)
        {
            bool isHex = i < image.Length && IsHexDigit(image[i]);
            if (isHex)
            {
                if (runStart < 0) runStart = i;
                continue;
            }
            if (runStart >= 0)
            {
                int length = i - runStart;
                if (length == 64)
                    result.Add((runStart, Encoding.ASCII.GetString(image, runStart, length)));
                runStart = -1;
            }
        }
        return result;
    }

    private static bool IsHexDigit(byte b)
        => (b >= (byte)'0' && b <= (byte)'9')
           || (b >= (byte)'A' && b <= (byte)'F')
           || (b >= (byte)'a' && b <= (byte)'f');
}
