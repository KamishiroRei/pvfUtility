using System.Globalization;
using System.Text;
using System.Text.Json;
using Pvf110.Core;

namespace Pvf110.Cli;

/// <summary>
/// pvfUtility CLI（AI 工作版）。这是唯一允许自动化代理使用的 PVF 入口；
/// pvfUtility.exe 是用户 GUI，不由 AI 启动。统一打开 Pvf110(sk.dat) 与
/// NKPI/ProtectedNKPI(90CN)，提供读取、解编译、搜索、提取、校验与重建能力。
/// </summary>
internal static class Program
{
    private const string AiCliContractVersion = "pvfUtility-ai-cli-20260831";
    private static string PvfPath => RequiredPath("PVF_PATH");
    private static string? SkdatPath
        => Environment.GetEnvironmentVariable("PVF_SKDAT") is { Length: > 0 } s ? Path.GetFullPath(s) : null;

    /// <summary>客户端主程序路径（外层包装密钥来源）。未设置时按 PVF 同目录自动探测。</summary>
    private static string? ClientExePath
        => Environment.GetEnvironmentVariable("PVF_CLIENT_EXE") is { Length: > 0 } s ? Path.GetFullPath(s) : null;

    /// <summary>
    /// 非 type-1（原始文本块，如 <c>string\*.str</c>）解码用编码。
    /// 未设置时保持历史行为（UTF-16LE 截断预览）。
    /// 支持 .NET 名称（<c>gb18030</c>/<c>utf-8</c>），也支持代码页号形式（<c>949</c>、<c>cp949</c>、<c>936</c>）。
    /// </summary>
    private static Encoding? TextEncoding
    {
        get
        {
            string? name = Environment.GetEnvironmentVariable("PVF_TEXT_ENCODING");
            if (string.IsNullOrWhiteSpace(name)) return null;
            string trimmed = name.Trim();
            string numeric = trimmed.StartsWith("cp", StringComparison.OrdinalIgnoreCase) ? trimmed[2..] : trimmed;
            try
            {
                return int.TryParse(numeric, out int page)
                    ? Encoding.GetEncoding(page)
                    : Encoding.GetEncoding(trimmed);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(
                    $"PVF_TEXT_ENCODING 无法解析: {name} ({ex.Message})；代码页形式请写 949 / cp949 / 936，名称形式请写 gb18030 / utf-8");
            }
        }
    }

    /// <summary>非 type-1 文本块输出上限（字符）。未设置时 300；显式 <c>0</c> 表示输出全部。</summary>
    private static int TextPreviewChars
    {
        get
        {
            string? raw = Environment.GetEnvironmentVariable("PVF_TEXT_CHARS");
            if (string.IsNullOrWhiteSpace(raw)) return 300;
            return int.TryParse(raw, out int n) ? Math.Max(0, n) : 300;
        }
    }
    private static string OutputRoot => Environment.GetEnvironmentVariable("PVF_OUTPUT_DIR")
        ?? Path.Combine(Environment.CurrentDirectory, "out");

    private static int Main(string[] args)
    {
        if (args.Length == 0) { Usage(); return 1; }
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        try { Console.OutputEncoding = Encoding.UTF8; } catch (IOException) { }
        try
        {
            switch (args[0])
            {
                case "keyset-probe": return KeySetProbe(args.Length > 1 && !args[1].StartsWith("--", StringComparison.Ordinal) ? args[1] : null,
                    args.Contains("--emit"),
                    args.SkipWhile(a => a != "--write").Skip(1).FirstOrDefault());
                case "version": return Version();
                case "info": return Info();
                case "list": return List(args.Length > 1 ? args[1] : "");
                case "file": return ReadFile(args[1]);
                case "decompile": return Decompile(args[1]);
                case "write": return WriteFile(args[1], args[2], args.Length > 3 ? args[3] : Path.Combine(OutputRoot, "write"));
                case "add": return Add(args[1], args[2], args.Length > 3 ? args[3] : Path.Combine(OutputRoot, "add"));
                case "mainline-epicdiff": return MainlineEpicDiff(args[1], args[2], args.Length > 3 ? args[3] : Path.Combine(OutputRoot, "mainline-epicdiff"));
                case "tune-monster-base": return TuneMonsterBase(args.Length > 1 ? args[1] : Path.Combine(OutputRoot, "tune-monster-base"));
                case "search": return Search(args[1], args.Length > 2 ? args[2] : "", args.Length > 3 ? args[3] : "type1");
                case "extract": return Extract(args.Length > 1 ? args[1] : Path.Combine(OutputRoot, "extract"), args.Length > 2 ? args[2] : "");
                case "compiled-roundtrip": return CompiledRoundtrip(args.Length > 1 ? args[1] : "stackable/10000001/10000039.stk");
                case "find-empty": return FindEmpty(args.Length > 1 ? int.Parse(args[1]) : 20000);
                case "scan-all": return ScanAll(args.Length > 1 ? int.Parse(args[1]) : 1);
                case "trace": return Trace(args[1]);
                case "rebuild-test": return RebuildTest(args.Length > 1 ? args[1] : Path.Combine(OutputRoot, "rebuild-test"));
                case "roundtrip": return Roundtrip();
                case "repack": return Repack(args.Length > 1 ? args[1] : Path.Combine(OutputRoot, "repack"));
                case "validate": return Validate();
                case "hash-test": return HashTest();
                case "batch-decompile": return BatchDecompile(args.Length > 1 ? args[1] : "");
                case "batch-decompile-script": return BatchDecompileScript(args[1], args[2]);
                case "batch-write": return BatchWrite(args[1], args.Length > 2 ? args[2] : Path.Combine(OutputRoot, "batch-write"));
                case "tags": return Tags(args.Length > 1 ? args[1] : "--sample", args.Length > 2 ? int.Parse(args[2]) : 300);
                case "nkpi-incremental-test": return NkpiIncrementalTest(args.Length > 1 ? args[1] : Path.Combine(OutputRoot, "nkpi-incremental-test"));
                case "pvf110-incremental-test": return Pvf110IncrementalSyntheticTest(args.Length > 1 ? args[1] : Path.Combine(OutputRoot, "pvf110-incremental-test"));
                default: Usage(); return 1;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ERROR: {ex.Message}");
            if (Environment.GetEnvironmentVariable("PVF_CLI_DEBUG") == "1")
                Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    // ─── 统一归档抽象：Pvf110 与 NKPI/ProtectedNKPI ──────────────────────

    private sealed class Archive
    {
        public required string FormatName { get; init; }
        public required int Count { get; init; }
        public required Func<int, string> FilePath { get; init; }
        public required Func<int, int> DataType { get; init; }
        public required Func<int, byte[]> ReadEntry { get; init; }
        /// <summary>条目名称池 magic（HASH 段 pairs 的 name 侧）。</summary>
        public required Func<int, int> NameMagic { get; init; }
        /// <summary>条目路径池 magic（HASH 段 pairs 的 path 侧，根目录为 0xFFFFFFFF）。</summary>
        public required Func<int, uint> PathMagic { get; init; }
        public required Pvf110Compiled Compiled { get; init; }
        public required Pvf110Reader? Pvf110 { get; init; }
        public required NkpiReader? Nkpi { get; init; }
    }

    private static Archive Open()
    {
        string pvfPath = PvfPath;

        // 1. NKPI / ProtectedNKPI（90CN）优先：无需 sk.dat；流式打开只读结构区，body 各组按需加载
        try
        {
            var r = NkpiReader.OpenFile(pvfPath);
            return new Archive
            {
                FormatName = r.Format == NkpiFormatKind.Protected ? "ProtectedNKPI" : "StandardNKPI",
                Count = r.Entries.Count,
                FilePath = i => r.FilePath(r.Entries[i]),
                DataType = i => r.Entries[i].DataType,
                ReadEntry = i => r.ReadEntry(r.Entries[i]),
                NameMagic = i => r.Entries[i].NameOffset,
                PathMagic = i => r.Entries[i].PathOffset < 0 ? HashNullPath : unchecked((uint)r.Entries[i].PathOffset),
                Compiled = new Pvf110Compiled(r),
                Pvf110 = null,
                Nkpi = r,
            };
        }
        catch (InvalidDataException)
        {
            // NKPI 探测失败，继续按独立 Pvf110 归档处理
        }

        // 2. Pvf110（Builder 外层包装；sk.dat 三级回退：PVF_SKDAT → 同目录/上溯 → 内置 sk.dat）
        var p = Pvf110Reader.OpenPvf(pvfPath, ResolveSkDatPath(pvfPath), ClientExePath);
        return new Archive
        {
            FormatName = "Pvf110",
            Count = p.Entries.Count,
            FilePath = i => p.FilePath(p.Entries[i]),
            DataType = i => p.Entries[i].DataType,
            ReadEntry = i => p.ReadEntry(p.Entries[i]),
            NameMagic = i => p.Entries[i].NameOffset,
            PathMagic = i => p.Entries[i].PathOffset < 0 ? HashNullPath : unchecked((uint)p.Entries[i].PathOffset),
            Compiled = new Pvf110Compiled(p),
            Pvf110 = p,
            Nkpi = null,
        };
    }

    /// <summary>HASH 段中表示"无路径"（根目录文件）的 sentinel，与 NkpiHashTable.NullPath 一致。</summary>
    private const uint HashNullPath = 0xFFFFFFFFu;

    /// <summary>
    /// sk.dat 文件解析：显式 <c>PVF_SKDAT</c> 优先；否则用 <see cref="Pvf110Support.FindSkDat"/>
    /// （PVF 同目录及其上溯层）。返回 null 表示没有文件 —— 打开流程随后使用**内置 sk.dat**
    /// （见 <see cref="Pvf110Reader.OpenPvf"/>），因此 115 固定版单机客户端无需任何配套文件。
    /// </summary>
    private static string? ResolveSkDatPath(string pvfPath)
    {
        string? explicitPath = SkdatPath;
        if (explicitPath != null) return explicitPath;
        return Pvf110Support.FindSkDat(pvfPath);
    }

    private static uint EntryNameMagic(Archive a, int index) => unchecked((uint)a.NameMagic(index));

    private static uint EntryPathMagic(Archive a, int index) => a.PathMagic(index);

    private static Archive RequirePvf110(Archive a)
        => a.Pvf110 != null ? a : throw new InvalidOperationException("this command requires a Pvf110(sk.dat) archive, current is " + a.FormatName);

    /// <summary>
    /// 把客户端 EXE 现场派生的密钥集注册给 <see cref="Pvf110Crypto"/>。
    /// 客户端 EXE 由 <see cref="Pvf110ClientKeys.FindClientExe"/> 自动定位（PVF 同目录或上溯数层），
    /// <c>PVF_CLIENT_EXE</c> 只作显式覆盖；同一 EXE 同进程内只扫描一次。
    /// </summary>
    private static void RegisterClientKeySets(string pvfPath)
        => Pvf110ClientKeys.EnsureRegistered(pvfPath, ClientExePath);

    /// <summary>
    /// 诊断（固定版单机客户端）：从客户端 EXE 现场派生候选密钥集，用当前 PVF 验证出实际生效的那一组，
    /// 并把 AES 密钥与 RSA 私钥 PEM 原样打印；带 <c>--emit</c> 时同时打印 sk.dat 的 base64。
    /// 用途：把固定版客户端的密钥材料内置进源码，之后打开 115 归档不再需要外部 sk.dat 与客户端 EXE。
    /// </summary>
    /// <summary>
    /// 密钥材料探测/落库：从客户端 EXE 现场派生候选密钥集，用真实归档验证出实际生效的那一组。
    /// `--emit` 打印可直接粘贴进材料文件的 JSON 条目；`--write &lt;文件&gt;` 直接写/更新
    /// <c>Pvf110KeyMaterial.json</c>（外部材料文件，工具与 GUI 打开时自动发现）。
    /// 因此新增/更换客户端版本是**写数据**，不需要改代码或重新编译。
    /// </summary>
    private static int KeySetProbe(string? clientExeArg, bool emit, string? writePath)
    {
        string pvfPath = PvfPath;
        string? exe = clientExeArg ?? ClientExePath ?? Pvf110ClientKeys.FindClientExe(pvfPath);
        string? skdat = ResolveSkDatPath(pvfPath);
        Console.WriteLine($"pvf={pvfPath}");
        Console.WriteLine($"clientExe={exe ?? "(none)"}");
        Console.WriteLine($"skdat={skdat ?? "(none)"}");
        int registered = 0;
        if (exe != null && File.Exists(exe))
        {
            foreach (var (aesKeyHex, pem) in Pvf110ClientKeys.ExtractCandidates(exe))
            {
                Pvf110Crypto.AddRuntimeKeySet(aesKeyHex, pem);
                registered++;
            }
        }
        Console.WriteLine($"candidatesRegistered={registered}");

        // sk.dat 允许来自文件或密钥材料注册表（外部材料文件 → 内置条目）
        byte[] skDatBytes;
        string skDatSource;
        if (skdat != null)
        {
            skDatBytes = File.ReadAllBytes(skdat);
            skDatSource = "file:" + skdat;
        }
        else if (Pvf110KeyMaterial.FindSkDat(pvfPath) is (byte[] materialBytes, string materialSource))
        {
            skDatBytes = materialBytes;
            skDatSource = materialSource;
        }
        else
        {
            Console.Error.WriteLine("keyset-probe needs a sk.dat (PVF_SKDAT / beside the PVF / key material registry)");
            return 1;
        }
        Console.WriteLine($"skdatSource={skDatSource}");

        var reader = Pvf110Reader.Open(skDatBytes, File.ReadAllBytes(pvfPath));
        Console.WriteLine($"entries={reader.Entries.Count} groups={reader.Groups.Count} chunks={reader.ChunkKeys.Length}");
        Pvf110Crypto.Pvf110KeySet active = Pvf110Crypto.LastOpenedKeySet ?? Pvf110Crypto.ActiveKeySet;
        Console.WriteLine($"resolvedByClientExe={Pvf110Crypto.LastOpenedKeySet != null}");
        Console.WriteLine("AES=" + active.AesKeyHex);
        Console.WriteLine(active.BuilderPrivateKeyPem.TrimEnd());

        string sha256 = exe != null && File.Exists(exe)
            ? Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(exe)))[..16].ToLowerInvariant()
            : "unknown";
        var entry = new Pvf110KeyMaterialEntry
        {
            Label = $"{Path.GetFileNameWithoutExtension(exe ?? "client")} ({sha256})",
            AesKeyHex = active.AesKeyHex,
            PrivateKeyPem = active.BuilderPrivateKeyPem.TrimEnd(),
            SkDatBase64 = Convert.ToBase64String(skDatBytes),
        };
        if (emit || writePath != null)
        {
            if (writePath != null)
            {
                Pvf110KeyMaterial.AppendToFile(writePath, entry);
                Console.WriteLine($"material written: {Path.GetFullPath(writePath)}");
            }
            if (emit)
            {
                string json = JsonSerializer.Serialize(new { schemaVersion = 1, entries = new[] { entry } },
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    });
                Console.WriteLine("--- material entry (paste under entries in " + Pvf110KeyMaterial.MaterialFileName + ") ---");
                Console.WriteLine(json);
            }
        }
        return 0;
    }

    private static int FindEntryIndex(Archive archive, string path)
    {
        string target = NormalizeArchivePath(path);
        for (int i = 0; i < archive.Count; i++)
            if (string.Equals(archive.FilePath(i), target, StringComparison.OrdinalIgnoreCase))
                return i;
        return -1;
    }

    // ─── 命令实现 ────────────────────────────────────────────────────────

    private static int Version()
    {
        Console.WriteLine("tool=pvfUtility");
        Console.WriteLine("interface=ai-cli");
        Console.WriteLine($"contract={AiCliContractVersion}");
        Console.WriteLine("gui=pvfUtility.exe (user-only; AI must not launch)");
        Console.WriteLine("formats=StandardNKPI,ProtectedNKPI,Pvf110");
        Console.WriteLine("pvf110-keysets=key-material registry (external Pvf110KeyMaterial.json, else built-in) + client-EXE derived + legacy built-ins");
        Console.WriteLine("pvf110-skdat=PVF_SKDAT, else sk.dat beside the PVF or up to 3 levels above, else key-material registry (built-in entry)");
        Console.WriteLine("capabilities.read=info,list,file,decompile,batch-decompile,batch-decompile-script,search,extract,validate,scan-all,hash-test");
        Console.WriteLine("capabilities.write=write,batch-write,add,repack,roundtrip (NKPI/ProtectedNKPI + Pvf110)");
        Console.WriteLine("capabilities.project=mainline-epicdiff,tune-monster-base (NKPI/ProtectedNKPI)");
        Console.WriteLine("pvf110-write=incremental group rebuild; hash/name sections preserved; client sk.dat reused when key slots suffice");
        return 0;
    }

    private static int Info()
    {
        var a = Open();
        Console.WriteLine($"format={a.FormatName}");
        Console.WriteLine($"requiresSkDat={a.Pvf110 != null}");
        Console.WriteLine($"entries={a.Count}");
        var nk = a.Nkpi;
        if (nk != null)
        {
            Console.WriteLine($"fileCount={nk.Header.FileCount} groupCount={nk.Header.GroupCount}");
            Console.WriteLine($"hashTableSize={nk.Header.HashTableSize} nameTableSize={nk.Header.NameTableSize} bodySize={nk.Header.BodySize}");
            Console.WriteLine($"utf8Pool={nk.Utf8Pool.Length} utf16Pool={nk.Utf16Pool.Length}");
            Console.WriteLine($"last group cumulative={nk.Groups[^1].cumulative} == bodySize {nk.Groups[^1].cumulative == nk.Header.BodySize}");
        }
        else
        {
            var r = a.Pvf110!;
            Console.WriteLine($"entries={r.Header.EntryCount} groups={r.Header.GroupCount}");
            Console.WriteLine($"hashTableSize={r.Header.HashTableSize} nameTableSize={r.Header.NameTableSize} bodySize={r.Header.BodySize}");
            Console.WriteLine($"utf8Pool={r.Utf8Pool.Length} utf16Pool={r.Utf16Pool.Length}");
            Console.WriteLine($"last group cumulative={r.Groups[^1].cumulative} == bodySize {r.Groups[^1].cumulative == r.Header.BodySize}");
            int chunks = (r.Stream.Length + Pvf110Crypto.ChunkStride - 1) / Pvf110Crypto.ChunkStride;
            Console.WriteLine($"skdatSource={r.SkDatSource} skdatPath={r.SkDatPath ?? "(built-in)"}");
            Console.WriteLine($"clientExe={Pvf110ClientKeys.LastClientExe ?? "(none)"}");
            Console.WriteLine($"keySet=runtime-derived={Pvf110Crypto.RuntimeKeySetCount} resolved={Pvf110Crypto.LastOpenedKeySet != null} aes={Pvf110Crypto.ActiveKeySet.AesKeyHex[..12]}…");
            Console.WriteLine($"chunkKeys={r.ChunkKeys.Length} chunksUsed={chunks} skdatSlotsEnough={r.ChunkKeys.Length >= chunks} skdatBytes={r.SkDatBytes.Length}");
        }
        for (int i = 0; i < Math.Min(5, a.Count); i++)
            Console.WriteLine($"  [{i}] {a.FilePath(i)}");
        return 0;
    }

    private static int List(string prefix)
    {
        var a = Open();
        string filter = prefix.Replace('\\', '/').Trim('/');
        int shown = 0;
        for (int i = 0; i < a.Count; i++)
        {
            string p = a.FilePath(i);
            if (filter.Length == 0 || p.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(p);
                shown++;
            }
        }
        Console.Error.WriteLine($"# total listed: {shown}/{a.Count}");
        return 0;
    }

    private static int ReadFile(string path)
    {
        var a = Open();
        string target = NormalizeArchivePath(path);
        int index = FindEntryIndex(a, target);
        if (index >= 0)
        {
            byte[] d = a.ReadEntry(index);
            Console.WriteLine($"size={d.Length} type={a.DataType(index)}");
            Console.WriteLine(Convert.ToHexString(d.AsSpan(0, Math.Min(64, d.Length))));
            return 0;
        }
        Console.Error.WriteLine($"path not found: {target}");
        return 1;
    }

    private static int Decompile(string path)
    {
        var a = Open();
        string target = NormalizeArchivePath(path);
        int index = FindEntryIndex(a, target);
        if (index >= 0)
        {
            byte[] d = a.ReadEntry(index);
            Console.WriteLine($"=== {a.FilePath(index)} ({d.Length}B type={a.DataType(index)}) ===");
            if (a.DataType(index) == 1)
                Console.Write(a.Compiled.ToScriptText(d));
            else
                Console.Write(DecodeTextBlock(d));
            return 0;
        }
        Console.Error.WriteLine($"path not found: {target}");
        return 1;
    }

    /// <summary>
    /// 非 type-1 文本块输出。设置 <c>PVF_TEXT_ENCODING</c> 时按该编码解码，否则按历史行为用 UTF-16LE；
    /// 两种情况都按 <c>PVF_TEXT_CHARS</c>（缺省 300 字符，0 = 全部）截断。
    /// </summary>
    private static string DecodeTextBlock(byte[] data)
    {
        string text = (TextEncoding ?? Encoding.Unicode).GetString(data);
        int limit = TextPreviewChars;
        if (limit == 0 || text.Length <= limit) return text + Environment.NewLine;
        return text[..limit] + Environment.NewLine
            + $"[... truncated: {text.Length} chars total; PVF_TEXT_CHARS=0 输出全部]" + Environment.NewLine;
    }

    private static int BatchDecompile(string pathListFile)
    {
        if (string.IsNullOrEmpty(pathListFile) || !File.Exists(pathListFile))
        {
            Console.Error.WriteLine("batch-decompile requires a file path containing one PVF path per line");
            return 1;
        }
        var a = Open();
        string[] targets = File.ReadAllLines(pathListFile)
            .Select(l => l.Trim().Replace('\\', '/').Trim('/'))
            .Where(l => l.Length > 0 && !l.StartsWith('#'))
            .ToArray();
        
        // Build lookup: path → index
        var lookup = new Dictionary<string, int>(a.Count, StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < a.Count; i++)
            lookup.TryAdd(a.FilePath(i), i);
        
        int ok = 0, fail = 0;
        foreach (string target in targets)
        {
            if (lookup.TryGetValue(target, out int idx))
            {
                byte[] d = a.ReadEntry(idx);
                if (a.DataType(idx) == 1)
                {
                    string text = a.Compiled.ToText(d);
                    Console.WriteLine($"=== {target} ({d.Length}B type={a.DataType(idx)}) ===");
                    Console.Write(text);
                }
                else
                {
                    Console.WriteLine($"=== {target} ({d.Length}B type={a.DataType(idx)}) ===");
                    Console.Write(DecodeTextBlock(d));
                }
                ok++;
            }
            else
            {
                Console.Error.WriteLine($"path not found: {target}");
                fail++;
            }
        }
        if (fail > 0)
            Console.Error.WriteLine($"# batch-decompile: ok={ok} fail={fail}");
        return fail == 0 ? 0 : 1;
    }

    /// <summary>
    /// 批量解编译为脚本文本文件（ToScriptText 经典格式，即 write/FromText 兼容格式）。
    /// pathlist 每行一个 PVF 路径（# 开头为注释行）；输出 outDir/归档相对路径，UTF-8 无 BOM。
    /// </summary>
    private static int BatchDecompileScript(string pathListFile, string outDir)
    {
        if (string.IsNullOrEmpty(pathListFile) || !File.Exists(pathListFile))
        {
            Console.Error.WriteLine("batch-decompile-script requires a file containing one PVF path per line");
            return 1;
        }
        var a = Open();
        string[] targets = File.ReadAllLines(pathListFile)
            .Select(l => l.Trim().Replace('\\', '/').Trim('/'))
            .Where(l => l.Length > 0 && !l.StartsWith('#'))
            .ToArray();

        var lookup = new Dictionary<string, int>(a.Count, StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < a.Count; i++)
            lookup.TryAdd(a.FilePath(i), i);

        Directory.CreateDirectory(outDir);
        int ok = 0, fail = 0, skippedNonScript = 0;
        foreach (string target in targets)
        {
            if (!lookup.TryGetValue(target, out int idx))
            {
                Console.Error.WriteLine($"path not found: {target}");
                fail++;
                continue;
            }
            if (a.DataType(idx) != 1)
            {
                Console.Error.WriteLine($"not a type-1 script, skipped: {target}");
                skippedNonScript++;
                continue;
            }
            string dest = Path.Combine(outDir, target.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.WriteAllText(dest, a.Compiled.ToScriptText(a.ReadEntry(idx)), new UTF8Encoding(false));
            ok++;
        }
        Console.WriteLine($"batch-decompile-script: ok={ok} fail={fail} skippedNonScript={skippedNonScript} -> {outDir}");
        return fail == 0 ? 0 : 1;
    }

    private sealed class BatchProgressReporter : IProgress<double>
    {
        private int _lastReported;
        public void Report(double value)
        {
            int v = (int)Math.Round(value);
            if (v >= _lastReported + 10 || v >= 100)
            {
                _lastReported = v;
                Console.WriteLine($"  rebuild progress {v}%");
            }
        }
    }

    /// <summary>
    /// 批量写回：清单每行 `PVF路径[TAB]内容文件[TAB]模式`，模式可省（缺省 auto）。
    /// 统一覆盖 NKPI/ProtectedNKPI 与 Pvf110：一次打开归档，内存中应用全部条目补丁后单次落盘
    /// （未修改组复用原始密文，仅重压缩含修改条目的组；HASH/name 段原样保留）；随后重开输出
    /// 文件并逐条目校验内容一致性。原始 PVF 不被修改。
    ///
    /// 模式语义：
    ///   auto  —— type-1 条目按脚本文本编译；其他类型按逐字节替换
    ///   text  —— 强制按 ToScriptText 脚本文本编译（仅 type-1）
    ///   raw   —— 内容文件字节逐字节写入（任意类型，含 <c>.str</c> 字符串表）
    ///   block —— 非 type-1 原始文本块：按 <c>PVF_TEXT_ENCODING</c>（缺省 UTF-16LE）解码后回编码写入
    /// </summary>
    private static int BatchWrite(string manifestPath, string outDir)
    {
        if (string.IsNullOrEmpty(manifestPath) || !File.Exists(manifestPath))
        {
            Console.Error.WriteLine("batch-write requires a manifest file with `archivePath<TAB>file[<TAB>mode]` per line");
            return 1;
        }
        var a = Open();

        var rows = new List<(string ArchivePath, string File, string Mode)>();
        foreach (string line in File.ReadLines(manifestPath))
        {
            string t = line.Trim().TrimStart('\ufeff');
            if (t.Length == 0 || t.StartsWith('#')) continue;
            string[] parts = t.Split('\t');
            if (parts.Length is < 2 or > 3 || parts[0].Trim().Length == 0 || parts[1].Trim().Length == 0)
            {
                Console.Error.WriteLine($"bad manifest line (expect archivePath<TAB>file[<TAB>mode]): {t}");
                return 1;
            }
            string mode = parts.Length == 3 ? parts[2].Trim().ToLowerInvariant() : "auto";
            if (mode is not ("auto" or "text" or "raw" or "block"))
            {
                Console.Error.WriteLine($"unknown mode '{mode}' in line: {t} (expect auto|text|raw|block)");
                return 1;
            }
            rows.Add((NormalizeArchivePath(parts[0]), parts[1].Trim(), mode));
        }
        if (rows.Count == 0) { Console.Error.WriteLine("manifest has no rows"); return 1; }

        var lookup = new Dictionary<string, int>(a.Count, StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < a.Count; i++)
            lookup.TryAdd(a.FilePath(i), i);

        var edits = new Dictionary<int, byte[]>();
        var modes = new Dictionary<int, string>();
        foreach ((string archivePath, string file, string mode) in rows)
        {
            if (!lookup.TryGetValue(archivePath, out int idx))
            { Console.Error.WriteLine($"path not found in archive: {archivePath}"); return 1; }
            if (!File.Exists(file))
            { Console.Error.WriteLine($"content file missing: {file}"); return 1; }
            int dataType = a.DataType(idx);
            string effective = mode;
            if (effective == "auto") effective = dataType == 1 ? "text" : "raw";
            if (effective == "text" && dataType != 1)
            { Console.Error.WriteLine($"mode text requires a type-1 entry: {archivePath} (type={dataType})"); return 1; }
            if (effective == "block" && dataType == 1)
            { Console.Error.WriteLine($"mode block is for non-type-1 entries; use text for type-1: {archivePath}"); return 1; }

            byte[] content = effective switch
            {
                "text" => a.Compiled.FromText(File.ReadAllText(file)),
                "raw" => File.ReadAllBytes(file),
                _ => (TextEncoding ?? Encoding.Unicode).GetBytes(
                        (TextEncoding ?? Encoding.Unicode).GetString(File.ReadAllBytes(file))),
            };
            if (!edits.TryAdd(idx, content))
            { Console.Error.WriteLine($"duplicate archive path in manifest: {archivePath}"); return 1; }
            modes[idx] = effective;
        }
        var modeSummary = edits.Keys.GroupBy(i => modes[i]).OrderBy(g => g.Key)
            .Select(g => $"{g.Key}={g.Count()}");
        Console.WriteLine($"batch-write: entries={edits.Count} (manifest rows={rows.Count}) modes: {string.Join(" ", modeSummary)}");

        Directory.CreateDirectory(outDir);
        string outPath = Path.Combine(outDir, "Script.pvf");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        if (a.Nkpi != null)
        {
            NkpiRepacker.RebuildIncrementalToFile(a.Nkpi, edits.ContainsKey, i => edits[i], outPath, new BatchProgressReporter());
        }
        else
        {
            var r = a.Pvf110!;
            byte[] sk;
            using (FileStream rebuildStream = new FileStream(outPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 1 << 20))
            {
                (_, sk) = Pvf110Rebuilder.RebuildToStream(
                    r,
                    rebuildStream,
                    e => edits.TryGetValue(e.Index, out byte[]? c) ? c : r.ReadEntry(e),
                    new BatchProgressReporter(),
                    e => edits.ContainsKey(e.Index));
            }
            string skPath = Path.Combine(outDir, "sk.dat");
            File.WriteAllBytes(skPath, sk);
            Console.WriteLine($"sk.dat written: {skPath} ({sk.Length:N0}B, unchanged={sk.AsSpan().SequenceEqual(r.SkDatBytes)})");
        }
        sw.Stop();
        Console.WriteLine($"rebuild done in {sw.Elapsed.TotalSeconds:F1}s -> {outPath}");

        if (a.Nkpi != null)
        {
            using NkpiReader verify = NkpiReader.OpenFile(outPath);
            if (verify.Entries.Count != a.Nkpi.Entries.Count || verify.Groups.Count != a.Nkpi.Groups.Count)
                throw new InvalidDataException("batch-write verify: entry/group count mismatch");
            if (verify.Header.HashTableSize != a.Nkpi.Header.HashTableSize
                || verify.Header.NameTableSize != a.Nkpi.Header.NameTableSize)
                throw new InvalidDataException("batch-write verify: hash/name table size changed");
            int verified = 0;
            foreach ((int idx, byte[] content) in edits)
            {
                byte[] back = verify.ReadEntry(verify.Entries[idx]);
                if (!back.AsSpan().SequenceEqual(content))
                    throw new InvalidDataException($"batch-write verify: content mismatch entry {idx} ({a.FilePath(idx)})");
                verified++;
                if ((verified % 1000) == 0)
                    Console.WriteLine($"  verified {verified}/{edits.Count}");
            }
            Console.WriteLine($"verify ok: {verified} entries");
        }
        else
        {
            var r = a.Pvf110!;
            var verify = Pvf110Reader.Open(File.ReadAllBytes(Path.Combine(outDir, "sk.dat")), File.ReadAllBytes(outPath));
            if (verify.Entries.Count != r.Entries.Count || verify.Groups.Count != r.Groups.Count)
                throw new InvalidDataException("batch-write verify: entry/group count mismatch");
            if (verify.Header.HashTableSize != r.Header.HashTableSize
                || verify.Header.NameTableSize != r.Header.NameTableSize)
                throw new InvalidDataException("batch-write verify: hash/name table size changed");
            int verified = 0;
            foreach ((int idx, byte[] content) in edits)
            {
                byte[] back = verify.ReadEntry(verify.Entries[idx]);
                if (!back.AsSpan().SequenceEqual(content))
                    throw new InvalidDataException($"batch-write verify: content mismatch entry {idx} ({a.FilePath(idx)})");
                verified++;
            }
            int sampled = 0, mismatched = 0;
            var rnd = new Random(20260916);
            for (int k = 0; k < 500 && verify.Entries.Count > edits.Count; k++)
            {
                int idx = rnd.Next(verify.Entries.Count);
                if (edits.ContainsKey(idx)) continue;
                sampled++;
                if (!r.ReadEntry(r.Entries[idx]).AsSpan().SequenceEqual(verify.ReadEntry(verify.Entries[idx]))) mismatched++;
            }
            Console.WriteLine($"verify ok: {verified} edited entries match; {sampled} sampled unedited entries byte-identical (mismatches={mismatched})");
            if (mismatched != 0)
                throw new InvalidDataException("batch-write verify: unedited entries changed");
        }
        long outLen = new FileInfo(outPath).Length;
        Console.WriteLine($"batch-write done: {outPath} ({outLen:N0}B) entries={edits.Count}");
        return 0;
    }

    private static int WriteFile(string path, string textFile, string outDir)
    {
        var a = Open();
        string target = NormalizeArchivePath(path);
        int targetIndex = FindEntryIndex(a, target);
        if (targetIndex < 0) { Console.Error.WriteLine($"path not found: {target}"); return 1; }

        string newText = File.ReadAllText(textFile);
        byte[] newContent = a.Compiled.FromText(newText);

        Console.WriteLine($"modifying {target} ({newContent.Length}B, type={a.DataType(targetIndex)})");

        // 按格式选择重建器
        if (a.Nkpi != null)
        {
            Directory.CreateDirectory(outDir);
            string outPath = Path.Combine(outDir, "Script.pvf");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            // 增量写回：只重压缩目标条目所在组；HASH/名称段和其他组字节原样复制
            NkpiRepacker.PatchEntry(a.Nkpi, targetIndex, newContent, outPath);
            sw.Stop();
            int patchGroup = a.Nkpi.Entries[targetIndex].ChunkIndex;
            using (NkpiReader verify = NkpiReader.OpenFile(outPath))
            {
                if (verify.Entries.Count != a.Nkpi.Entries.Count || verify.Groups.Count != a.Nkpi.Groups.Count)
                    throw new InvalidDataException("incremental patch verify: entry/group count mismatch");
                if (verify.Header.HashTableSize != a.Nkpi.Header.HashTableSize
                    || verify.Header.NameTableSize != a.Nkpi.Header.NameTableSize)
                    throw new InvalidDataException("incremental patch verify: hash/name table size changed");
                byte[] back = verify.ReadEntry(verify.Entries[targetIndex]);
                if (!back.AsSpan().SequenceEqual(newContent))
                    throw new InvalidDataException("incremental patch verify: target entry content mismatch");
            }
            long outLen = new FileInfo(outPath).Length;
            Console.WriteLine($"NKPI incremental patch done: {outPath} ({outLen:N0}B) in {sw.Elapsed.TotalSeconds:F1}s");
            Console.WriteLine($"recompressed group={patchGroup} only; hash/name sections and other groups copied verbatim");
            return 0;
        }

        if (a.Pvf110 != null)
        {
            var r = a.Pvf110;
            byte[] GetContent(Pvf110Entry e)
            {
                if (e.Index == targetIndex) return newContent;
                return r.ReadEntry(e);
            }
            Directory.CreateDirectory(outDir);
            string pvfOut = Path.Combine(outDir, "Script.pvf");
            string skOut = Path.Combine(outDir, "sk.dat");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            byte[] sk;
            using (FileStream rebuildStream = new FileStream(pvfOut, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 1 << 20))
            {
                (_, sk) = Pvf110Rebuilder.RebuildToStream(r, rebuildStream, GetContent, null, e => e.Index == targetIndex);
            }
            File.WriteAllBytes(skOut, sk);
            sw.Stop();
            bool skUnchanged = sk.AsSpan().SequenceEqual(r.SkDatBytes);
            Console.WriteLine($"Pvf110 rebuild done: {pvfOut} ({new FileInfo(pvfOut).Length:N0}B) sk.dat ({sk.Length:N0}B, unchanged={skUnchanged}) in {sw.Elapsed.TotalSeconds:F1}s");
            Console.WriteLine($"recompressed group={r.Entries[targetIndex].ChunkIndex} only; other groups and hash/name sections copied verbatim");

            var verify = Pvf110Reader.Open(sk, File.ReadAllBytes(pvfOut));
            if (verify.Entries.Count != r.Entries.Count || verify.Groups.Count != r.Groups.Count)
                throw new InvalidDataException("Pvf110 write verify: entry/group count mismatch");
            if (verify.Header.HashTableSize != r.Header.HashTableSize
                || verify.Header.NameTableSize != r.Header.NameTableSize)
                throw new InvalidDataException("Pvf110 write verify: hash/name table size changed");
            byte[] back = verify.ReadEntry(verify.Entries[targetIndex]);
            if (!back.AsSpan().SequenceEqual(newContent))
                throw new InvalidDataException("Pvf110 write verify: target entry content mismatch");
            int sampled = 0, mismatched = 0;
            var rnd = new Random(20260916);
            for (int k = 0; k < 200; k++)
            {
                int idx = rnd.Next(r.Entries.Count);
                if (idx == targetIndex) continue;
                sampled++;
                if (!r.ReadEntry(r.Entries[idx]).AsSpan().SequenceEqual(verify.ReadEntry(verify.Entries[idx]))) mismatched++;
            }
            if (mismatched != 0)
                throw new InvalidDataException($"Pvf110 write verify: {mismatched}/{sampled} unmodified entries changed");
            Console.WriteLine($"verify ok: target content matches; {sampled} sampled unmodified entries all byte-identical");
            return 0;
        }

        Console.Error.WriteLine("unsupported format for write");
        return 1;
    }

    private static int Add(string sourcePath, string archivePath, string outDir)
    {
        var archive = Open();

        string source = Path.GetFullPath(sourcePath);
        if (!File.Exists(source) && !Directory.Exists(source))
            throw new FileNotFoundException("add source does not exist", source);

        string prefix = archivePath.Replace('\\', '/').Trim('/');
        var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < archive.Count; i++)
        {
            existing.Add(archive.FilePath(i));
            existing.Add(NormalizeArchivePath(archive.FilePath(i)));
        }

        var sources = new List<(string File, string Target)>();
        if (File.Exists(source))
        {
            if (prefix.Length == 0)
                throw new ArgumentException("archive path is required when adding one file", nameof(archivePath));
            sources.Add((source, prefix));
        }
        else
        {
            string[] files = Directory.GetFiles(source, "*", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            foreach (string file in files)
            {
                string relative = Path.GetRelativePath(source, file).Replace('\\', '/');
                string target = prefix.Length == 0 ? relative : prefix + "/" + relative;
                sources.Add((file, target));
            }
        }
        if (sources.Count == 0)
            throw new InvalidOperationException("add source contains no files");
        foreach ((string _, string target) in sources)
        {
            if (existing.Contains(target) || existing.Contains(NormalizeArchivePath(target)))
                throw new InvalidOperationException("archive path already exists; add only new paths: " + target);
        }

        if (archive.Nkpi != null)
        {
            var pool = new NkpiNamePoolBuilder(archive.Nkpi);
            var additions = sources
                .Select(s => CreateNewFile(s.File, s.Target, archive.Compiled, pool.GetOrAdd))
                .ToList();

            Directory.CreateDirectory(outDir);
            byte[] rebuilt = NkpiRepacker.RebuildWithAdditions(
                archive.Nkpi,
                archive.ReadEntry,
                additions,
                pool);
            string outputPath = Path.Combine(outDir, "Script.pvf");
            File.WriteAllBytes(outputPath, rebuilt);

            var reopened = NkpiReader.Open(rebuilt);
            int verified = 0;
            foreach (NkpiNewFile addition in additions)
            {
                NkpiEntry? entry = reopened.Entries.FirstOrDefault(e => reopened.FilePath(e) == addition.FilePath);
                if (entry == null)
                    throw new InvalidDataException("new file missing after NKPI rebuild: " + addition.FilePath);
                byte[] actual = reopened.ReadEntry(entry);
                if (!actual.AsSpan().SequenceEqual(addition.Content))
                    throw new InvalidDataException("new file content mismatch after NKPI rebuild: " + addition.FilePath);
                verified++;
            }
            Console.WriteLine($"NKPI add done: files={additions.Count} verified={verified} output={outputPath} ({rebuilt.Length:N0}B)");
            return 0;
        }

        // Pvf110：名称池追加 + HASH 段增量 + 末尾新组；原 sk.dat 槽位够用时原样回写
        var r = archive.Pvf110!;
        var pvfPool = Pvf110NamePool.FromReader(r);
        var pvfAdditions = new List<Pvf110Addition>();
        foreach ((string file, string target) in sources)
        {
            NkpiNewFile nf = CreateNewFile(file, target, archive.Compiled, pvfPool.GetOrAdd);
            pvfAdditions.Add(new Pvf110Addition(nf.FilePath, nf.Content, nf.DataType));
        }

        Directory.CreateDirectory(outDir);
        string pvfOut = Path.Combine(outDir, "Script.pvf");
        string skOut = Path.Combine(outDir, "sk.dat");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        byte[] sk;
        using (FileStream rebuildStream = new FileStream(pvfOut, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 1 << 20))
        {
            (_, sk) = Pvf110Rebuilder.RebuildToStream(
                r,
                rebuildStream,
                e => r.ReadEntry(e),
                new BatchProgressReporter(),
                _ => false,
                pvfAdditions,
                Pvf110RebuildOptions.Default,
                pvfPool);
        }
        File.WriteAllBytes(skOut, sk);
        sw.Stop();

        var verify = Pvf110Reader.Open(sk, File.ReadAllBytes(pvfOut));
        if (verify.Entries.Count != archive.Count + pvfAdditions.Count)
            throw new InvalidDataException($"Pvf110 add verify: entry count {verify.Entries.Count} != {archive.Count + pvfAdditions.Count}");
        if (verify.Header.GroupCount != r.Header.GroupCount + 1)
            throw new InvalidDataException("Pvf110 add verify: group count did not grow by exactly one");
        foreach (Pvf110Addition addition in pvfAdditions)
        {
            int idx = verify.Entries.FindIndex(e =>
                string.Equals(verify.FilePath(e), addition.Path, StringComparison.OrdinalIgnoreCase));
            if (idx < 0)
                throw new InvalidDataException("Pvf110 add verify: new path missing: " + addition.Path);
            byte[] actual = verify.ReadEntry(verify.Entries[idx]);
            if (!actual.AsSpan().SequenceEqual(addition.Content))
                throw new InvalidDataException("Pvf110 add verify: new file content mismatch: " + addition.Path);
            if (verify.Entries[idx].DataType != addition.DataType)
                throw new InvalidDataException("Pvf110 add verify: data type mismatch: " + addition.Path);
            string expected = NormalizeArchivePath(addition.Path);
            if (NormalizeArchivePath(verify.FilePath(verify.Entries[idx])) != expected)
                throw new InvalidDataException($"Pvf110 add verify: path roundtrip mismatch: {verify.FilePath(verify.Entries[idx])} != {expected}");
        }
        int preChecked = 0;
        for (int i = 0; i < archive.Count; i++)
        {
            if (!string.Equals(verify.FilePath(verify.Entries[i]), archive.FilePath(i), StringComparison.Ordinal))
                throw new InvalidDataException($"Pvf110 add verify: existing path {i} changed");
            byte[] before = r.ReadEntry(r.Entries[i]);
            byte[] after = verify.ReadEntry(verify.Entries[i]);
            if (!before.AsSpan().SequenceEqual(after))
                throw new InvalidDataException($"Pvf110 add verify: existing entry {i} content changed ({archive.FilePath(i)})");
            preChecked++;
        }
        Console.WriteLine($"Pvf110 add done: files={pvfAdditions.Count} verified={pvfAdditions.Count} existing_preserved={preChecked} in {sw.Elapsed.TotalSeconds:F1}s");
        Console.WriteLine($"output={pvfOut} ({new FileInfo(pvfOut).Length:N0}B) sk.dat={skOut} ({sk.Length:N0}B, unchanged={sk.AsSpan().SequenceEqual(r.SkDatBytes)})");
        Console.WriteLine("hash segment: original pairs/tail bytes preserved, new pairs appended; name pool: existing magic reused, new strings appended to utf16 tail");
        return 0;
    }

    private sealed class MainlineManifest
    {
        public List<MainlineDungeon> Dungeons { get; set; } = new();
    }

    private sealed class MainlineDungeon
    {
        public string Path { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
    }

    /// <summary>
    /// 为主线副本生成按等级段隔离的 Monster/APC 难度表，并只替换主线 DGN
    /// 的 [monsterapc diff table] 引用。整个操作仍由源码 CLI 完成，不启动 GUI。
    /// </summary>
    private static int MainlineEpicDiff(string manifestPath, string templatePath, string outDir)
    {
        var archive = Open();
        if (archive.Nkpi == null)
            throw new InvalidOperationException("mainline-epicdiff currently requires NKPI/ProtectedNKPI; current format is " + archive.FormatName);

        string manifestFile = Path.GetFullPath(manifestPath);
        if (!File.Exists(manifestFile))
            throw new FileNotFoundException("mainline manifest does not exist", manifestFile);
        MainlineManifest manifest = JsonSerializer.Deserialize<MainlineManifest>(File.ReadAllText(manifestFile),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidDataException("mainline manifest is empty");
        if (manifest.Dungeons.Count == 0)
            throw new InvalidDataException("mainline manifest contains no dungeons");

        string template = NormalizeArchivePath(templatePath);
        var lookup = new Dictionary<string, int>(archive.Count, StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < archive.Count; i++)
            lookup.TryAdd(archive.FilePath(i), i);
        if (!lookup.TryGetValue(template, out int templateIndex))
            throw new InvalidDataException("difficulty template is not in current PVF: " + template);

        var selected = new List<(string path, int index, int upperLevel)>();
        var selectedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (MainlineDungeon dungeon in manifest.Dungeons)
        {
            string path = NormalizeArchivePath(dungeon.Path);
            if (!selectedPaths.Add(path))
                continue;
            if (!lookup.TryGetValue(path, out int index))
                throw new InvalidDataException("mainline DGN is not in current PVF: " + path);
            int upperLevel = ParseBatchUpperLevel(dungeon.Batch);
            selected.Add((path, index, upperLevel));
        }

        int[] batchLevels = selected.Select(item => item.upperLevel).Distinct().OrderBy(level => level).ToArray();
        string templateText = archive.Compiled.ToText(archive.ReadEntry(templateIndex));
        var namePool = new NkpiNamePoolBuilder(archive.Nkpi);
        var additions = new List<NkpiNewFile>(batchLevels.Length);
        var updates = new Dictionary<int, byte[]>(selected.Count);
        var expectedDgnRefs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        Directory.CreateDirectory(outDir);
        string sourceOutDir = Path.Combine(outDir, "epicdiff");
        Directory.CreateDirectory(sourceOutDir);
        for (int i = 0; i < batchLevels.Length; i++)
        {
            int upperLevel = batchLevels[i];
            string targetPath = $"dungeon/epicdiff/epic_diff_lv{upperLevel}.tbl";
            string tableText = BuildEpicDiffTableText(templateText, upperLevel);
            byte[] tableContent = archive.Compiled.FromText(tableText, namePool.GetOrAdd);
            int existingTableIndex = FindEntryIndex(archive, targetPath);
            if (existingTableIndex >= 0)
                updates[existingTableIndex] = tableContent;
            else
                additions.Add(new NkpiNewFile(targetPath, tableContent, 1));
            File.WriteAllText(Path.Combine(sourceOutDir, $"epic_diff_lv{upperLevel}.tbl"),
                "#PVF_File\n\n" + tableText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }

        foreach ((string path, int index, int upperLevel) in selected)
        {
            string targetPath = $"dungeon/epicdiff/epic_diff_lv{upperLevel}.tbl";
            string originalText = archive.Compiled.ToText(archive.ReadEntry(index));
            string patchedText = ReplaceMonsterApcDiffTable(originalText, targetPath);
            updates[index] = archive.Compiled.FromText(patchedText, namePool.GetOrAdd);
            expectedDgnRefs[path] = NormalizeArchivePath(targetPath);
        }

        byte[] rebuilt = additions.Count > 0
            ? NkpiRepacker.RebuildWithAdditions(
                archive.Nkpi,
                index => updates.TryGetValue(index, out byte[]? content) ? content : archive.ReadEntry(index),
                additions,
                namePool)
            : NkpiRepacker.Rebuild(
                archive.Nkpi,
                index => updates.TryGetValue(index, out byte[]? content) ? content : archive.ReadEntry(index));
        string outputPath = Path.Combine(outDir, "Script.pvf");
        File.WriteAllBytes(outputPath, rebuilt);

        NkpiReader reopened = NkpiReader.Open(rebuilt);
        VerifyMainlineEpicDiff(archive.Nkpi, reopened, additions, updates, expectedDgnRefs);
        Console.WriteLine($"mainline-epicdiff done: dungeons={selected.Count} batches={batchLevels.Length} additions={additions.Count} updated={updates.Count}");
        Console.WriteLine($"output={outputPath} ({rebuilt.Length:N0}B)");
        Console.WriteLine($"source_tables={sourceOutDir}");
        Console.WriteLine("profile=warlike 50/75/100/120/150; attack damage & defense 1/1.5/2/2.5/3; other groups unchanged");
        return 0;
    }

    /// <summary>
    /// 对当前版本真正生效的普通/精英共用 common 表与 BOSS 基础表使用同一等级倍率，
    /// 只调整每等级攻击四字段，保留各类基础表各自的 HP、防御和其他差异。
    /// namedmonsterbaseparameter 仅作只读对照；教程 11828 尚未证明它是当前精英运行入口。
    /// 曲线最高约 5 倍，按用户给出的 10/20/30/40/60/80 级范围做平滑过渡。
    /// </summary>
    private static int TuneMonsterBase(string outDir)
    {
        var archive = Open();
        if (archive.Nkpi == null)
            throw new InvalidOperationException("tune-monster-base currently requires NKPI/ProtectedNKPI; current format is " + archive.FormatName);

        string[] targetPaths =
        {
            "monster/commonmonsterbaseparameter.tbl",
            "monster/bossmonsterbaseparameter.tbl"
        };
        Directory.CreateDirectory(outDir);
        string sourceOutDir = Path.Combine(outDir, "monsterbase");
        Directory.CreateDirectory(sourceOutDir);

        var updates = new Dictionary<int, byte[]>();
        var originalTokens = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);
        var patchedTokens = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);
        foreach (string targetPath in targetPaths)
        {
            int targetIndex = FindEntryIndex(archive, targetPath);
            if (targetIndex < 0)
                throw new InvalidDataException("monster base table is not in current PVF: " + targetPath);

            string originalText = archive.Compiled.ToText(archive.ReadEntry(targetIndex));
            string patchedText = BuildSharedMonsterBaseTableText(originalText, out int[] original, out int[] patched, out int changedFields);
            updates[targetIndex] = archive.Compiled.FromText(patchedText);
            originalTokens[targetPath] = original;
            patchedTokens[targetPath] = patched;
            File.WriteAllText(
                Path.Combine(sourceOutDir, Path.GetFileName(targetPath)),
                "#PVF_File\n\n" + patchedText,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            Console.WriteLine($"{targetPath}: tokens={original.Length} attack_fields_changed={changedFields}");
        }

        byte[] rebuilt = NkpiRepacker.Rebuild(
            archive.Nkpi,
            index => updates.TryGetValue(index, out byte[]? content) ? content : archive.ReadEntry(index));
        string outputPath = Path.Combine(outDir, "Script.pvf");
        File.WriteAllBytes(outputPath, rebuilt);

        using NkpiReader reopened = NkpiReader.OpenFile(outputPath);
        var reopenedCompiled = new Pvf110Compiled(reopened);
        if (reopened.Entries.Count != archive.Nkpi.Entries.Count || reopened.Groups.Count != archive.Nkpi.Groups.Count)
            throw new InvalidDataException("tune-monster-base verify: entry/group count mismatch");
        if (reopened.Header.HashTableSize != archive.Nkpi.Header.HashTableSize
            || reopened.Header.NameTableSize != archive.Nkpi.Header.NameTableSize)
            throw new InvalidDataException("tune-monster-base verify: hash/name table size changed");

        foreach (string targetPath in targetPaths)
        {
            int targetIndex = reopened.Entries.FindIndex(entry =>
                string.Equals(reopened.FilePath(entry), targetPath, StringComparison.OrdinalIgnoreCase));
            if (targetIndex < 0)
                throw new InvalidDataException("tune-monster-base verify: target disappeared: " + targetPath);
            byte[] back = reopened.ReadEntry(reopened.Entries[targetIndex]);
            int[] backTokens = ParseMonsterBaseTokens(reopenedCompiled.ToText(back), targetPath);
            int[] expected = patchedTokens[targetPath];
            if (!backTokens.SequenceEqual(expected))
                throw new InvalidDataException("tune-monster-base verify: target content mismatch: " + targetPath);
            int[] before = originalTokens[targetPath];
            for (int i = 0; i < before.Length; i++)
            {
                if (i % 24 is >= 1 and <= 4)
                    continue;
                if (before[i] != backTokens[i])
                    throw new InvalidDataException($"tune-monster-base verify: non-attack token changed at {targetPath} index {i}");
            }
        }

        Console.WriteLine($"tune-monster-base done: effective_tables={targetPaths.Length} output={outputPath} ({rebuilt.Length:N0}B)");
        Console.WriteLine("profile=common(non-boss ordinary+elite) and boss share smooth attack multiplier; max 5.0; anchors 10=5.0,20=4.0,30=3.0,40=2.0,60=1.5,80+=1.0");
        Console.WriteLine($"source_tables={sourceOutDir}");
        return 0;
    }

    private static string BuildSharedMonsterBaseTableText(
        string text,
        out int[] original,
        out int[] patched,
        out int changedFields)
    {
        original = ParseMonsterBaseTokens(text, "monster base table");
        if (original.Length == 0 || original.Length % 24 != 0)
            throw new InvalidDataException("monster base table token count is not a multiple of 24");

        patched = (int[])original.Clone();
        changedFields = 0;
        int levelCount = original.Length / 24;
        for (int row = 0; row < levelCount; row++)
        {
            int offset = row * 24;
            int level = original[offset];
            if (level != row + 1)
                throw new InvalidDataException($"monster base table level token mismatch at row {row}: {level}");
            double multiplier = SharedMonsterAttackMultiplier(level);
            for (int field = 1; field <= 4; field++)
            {
                int value = (int)Math.Round(original[offset + field] * multiplier, MidpointRounding.AwayFromZero);
                if (patched[offset + field] != value)
                {
                    patched[offset + field] = value;
                    changedFields++;
                }
            }
        }

        var builder = new StringBuilder(patched.Length * 4);
        builder.Append('\t');
        for (int i = 0; i < patched.Length; i++)
        {
            if (i > 0)
                builder.Append(i % 24 == 0 ? "\n\t" : "\t");
            builder.Append(patched[i].ToString(CultureInfo.InvariantCulture));
        }
        builder.Append('\n');
        return builder.ToString();
    }

    private static int[] ParseMonsterBaseTokens(string text, string label)
    {
        string normalized = text.Replace("\r\n", "\n").Replace('\r', '\n');
        var tokens = new List<int>();
        foreach (string line in normalized.Split('\n'))
        {
            string trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.Equals("#PVF_File", StringComparison.OrdinalIgnoreCase))
                continue;
            foreach (string token in trimmed.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                    throw new InvalidDataException($"{label} contains non-integer token: {token}");
                tokens.Add(value);
            }
        }
        return tokens.ToArray();
    }

    private static double SharedMonsterAttackMultiplier(int level)
    {
        int[] levels = { 1, 10, 20, 30, 40, 60, 80 };
        double[] values = { 5.0, 5.0, 4.0, 3.0, 2.0, 1.5, 1.0 };
        if (level <= levels[0])
            return values[0];
        if (level >= levels[^1])
            return values[^1];

        for (int i = 1; i < levels.Length; i++)
        {
            if (level > levels[i])
                continue;
            double t = (level - levels[i - 1]) / (double)(levels[i] - levels[i - 1]);
            t = t * t * (3.0 - 2.0 * t);
            return values[i - 1] + (values[i] - values[i - 1]) * t;
        }
        return values[^1];
    }

    private static int ParseBatchUpperLevel(string batch)
    {
        string value = (batch ?? string.Empty).Trim();
        int separator = value.LastIndexOf('-');
        string upper = separator >= 0 ? value[(separator + 1)..] : value;
        if (!int.TryParse(upper, NumberStyles.Integer, CultureInfo.InvariantCulture, out int level) || level <= 0)
            throw new InvalidDataException("invalid mainline batch upper level: " + batch);
        return level;
    }

    private static string NormalizeArchivePath(string path)
        => (path ?? string.Empty).Replace('\\', '/').Trim('/').ToLowerInvariant();

    /// <summary>
    /// 按当前锁定的难度设计改写模板表(2026-08-31,用户锁定):
    /// 好战性(1-5)固定 50/75/100/120/150;攻击伤害(41-45)与防御(46-50)
    /// 固定 1/1.5/2/2.5/3;其余属性组保持模板原值。
    /// 位置语义来源:通用知识区/技能机制/怪物属性与防御计算手册(教程 11827 已验证映射)。
    /// </summary>
    private static string BuildEpicDiffTableText(string templateText, int upperLevel)
    {
        string[] lines = templateText.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        int numericIndex = 0;
        bool foundBoundary = false;
        if (upperLevel < 5 || upperLevel % 5 != 0)
            throw new InvalidDataException("EpicDiff batch upper level must be a positive multiple of 5: " + upperLevel);
        double[] warlike = { 50, 75, 100, 120, 150 };
        double[] attack = { 1, 1.5, 2, 2.5, 3 };
        double[] defense = { 1, 1.5, 2, 2.5, 3 };
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim().Equals("[dungeon party balance]", StringComparison.OrdinalIgnoreCase))
            {
                foundBoundary = true;
                break;
            }
            if (!double.TryParse(lines[i].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out _))
                continue;

            if (numericIndex < 5)
            {
                lines[i] = "\t" + FormatPvfFloat(warlike[numericIndex]);
            }
            else if (numericIndex is >= 40 and < 45)
            {
                lines[i] = "\t" + FormatPvfFloat(attack[numericIndex - 40]);
            }
            else if (numericIndex is >= 45 and < 50)
            {
                lines[i] = "\t" + FormatPvfFloat(defense[numericIndex - 45]);
            }
            numericIndex++;
        }

        if (!foundBoundary || numericIndex < 50)
            throw new InvalidDataException("difficulty template has an unexpected leading numeric layout");
        return string.Join('\n', lines).TrimEnd('\n') + "\n";
    }

    private static string FormatPvfFloat(double value)
    {
        string result = value.ToString("0.###", CultureInfo.InvariantCulture);
        return result.Contains('.') ? result : result + ".0";
    }

    private static string ReplaceMonsterApcDiffTable(string text, string targetPath)
    {
        string[] lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        bool replaced = false;
        for (int i = 0; i < lines.Length; i++)
        {
            if (!lines[i].Trim().Equals("[monsterapc diff table]", StringComparison.OrdinalIgnoreCase))
                continue;
            int valueIndex = i + 1;
            while (valueIndex < lines.Length && string.IsNullOrWhiteSpace(lines[valueIndex]))
                valueIndex++;
            if (valueIndex < lines.Length && !lines[valueIndex].TrimStart().StartsWith("[", StringComparison.Ordinal))
                lines[valueIndex] = "\t" + targetPath;
            else
            {
                List<string> expanded = lines.ToList();
                expanded.Insert(valueIndex, "\t" + targetPath);
                lines = expanded.ToArray();
            }
            replaced = true;
        }

        if (!replaced)
        {
            var expanded = lines.ToList();
            while (expanded.Count > 0 && expanded[^1].Length == 0)
                expanded.RemoveAt(expanded.Count - 1);
            expanded.Add("[monsterapc diff table]");
            expanded.Add("\t" + targetPath);
            lines = expanded.ToArray();
        }
        return string.Join('\n', lines).TrimEnd('\n') + "\n";
    }

    private static void VerifyMainlineEpicDiff(
        NkpiReader original,
        NkpiReader rebuilt,
        IReadOnlyList<NkpiNewFile> additions,
        IReadOnlyDictionary<int, byte[]> updates,
        IReadOnlyDictionary<string, string> expectedDgnRefs)
    {
        var rebuiltLookup = new Dictionary<string, NkpiEntry>(rebuilt.Entries.Count, StringComparer.OrdinalIgnoreCase);
        foreach (NkpiEntry entry in rebuilt.Entries)
            rebuiltLookup[rebuilt.FilePath(entry)] = entry;

        foreach (NkpiNewFile addition in additions)
        {
            if (!rebuiltLookup.TryGetValue(addition.FilePath, out NkpiEntry? entry) || entry == null)
                throw new InvalidDataException("generated EpicDiff table missing after rebuild: " + addition.FilePath);
            if (!rebuilt.ReadEntry(entry).AsSpan().SequenceEqual(addition.Content))
                throw new InvalidDataException("generated EpicDiff table content mismatch: " + addition.FilePath);
        }

        int preserved = 0;
        foreach (NkpiEntry originalEntry in original.Entries)
        {
            string path = original.FilePath(originalEntry);
            if (!rebuiltLookup.TryGetValue(path, out NkpiEntry? rebuiltEntry) || rebuiltEntry == null)
                throw new InvalidDataException("existing PVF entry missing after mainline rebuild: " + path);
            byte[] expected;
            if (updates.TryGetValue(originalEntry.Index, out byte[]? update) && update != null)
                expected = update;
            else
                expected = original.ReadEntry(originalEntry);
            if (!rebuilt.ReadEntry(rebuiltEntry).AsSpan().SequenceEqual(expected))
                throw new InvalidDataException("existing PVF entry content mismatch after mainline rebuild: " + path);
            preserved++;
        }

        foreach ((string path, string expectedRef) in expectedDgnRefs)
        {
            if (!rebuiltLookup.TryGetValue(path, out NkpiEntry? entry) || entry == null)
                throw new InvalidDataException("updated mainline DGN missing after rebuild: " + path);
            string text = new Pvf110Compiled(rebuilt).ToText(rebuilt.ReadEntry(entry));
            if (!text.Contains(expectedRef, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("updated mainline DGN does not reference generated EpicDiff table: " + path);
        }
        Console.WriteLine($"verified additions={additions.Count} preserved_entries={preserved} updated_dungeons={expectedDgnRefs.Count}");
    }

    private static NkpiNewFile CreateNewFile(string sourcePath, string targetPath, Pvf110Compiled compiled, Func<string, int>? missingStringResolver)
    {
        byte[] raw = File.ReadAllBytes(sourcePath);
        int dataType = 2;
        byte[] content = raw;
        string text = Encoding.UTF8.GetString(raw);
        if (text.StartsWith("#PVF_File", StringComparison.OrdinalIgnoreCase))
        {
            string body = text.Replace("\r\n", "\n");
            int firstNewline = body.IndexOf('\n');
            body = firstNewline >= 0 ? body[(firstNewline + 1)..] : string.Empty;
            if (body.StartsWith("\n", StringComparison.Ordinal))
                body = body[1..];
            content = compiled.FromText(body, missingStringResolver);
            dataType = 1;
        }
        return new NkpiNewFile(targetPath, content, dataType);
    }

    /// <summary>
    /// 内容检索。<c>mode=type1</c>（缺省）只检索 type-1 脚本条目；
    /// <c>mode=all</c> 同时检索非 type-1 原始文本块（如 115 的 <c>string\*.str</c> 字符串表），
    /// 按 <c>PVF_TEXT_ENCODING</c>（缺省 UTF-16LE）解码后匹配。检索仍不索引名称池。
    /// </summary>
    private static int Search(string text, string pathPrefix, string mode = "type1")
    {
        var a = Open();
        bool allTypes = mode.Equals("all", StringComparison.OrdinalIgnoreCase);
        if (!allTypes && !mode.Equals("type1", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"search mode must be type1 or all, got '{mode}'");
        string prefix = pathPrefix.Replace('\\', '/').Trim('/');
        int hits = 0;
        for (int i = 0; i < a.Count; i++)
        {
            int type = a.DataType(i);
            if (type != 1 && !allTypes) continue;
            string p = a.FilePath(i);
            if (prefix.Length > 0 && !p.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
            byte[] d = a.ReadEntry(i);
            string txt = type == 1 ? a.Compiled.ToText(d) : (TextEncoding ?? Encoding.Unicode).GetString(d);
            if (txt.Contains(text, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"{p}\ttype={type}");
                hits++;
            }
        }
        Console.Error.WriteLine($"# matches: {hits} (mode={mode})");
        return 0;
    }

    private static int Extract(string outDir, string pathPrefix)
    {
        var a = Open();
        Directory.CreateDirectory(outDir);
        string prefix = pathPrefix.Replace('\\', '/').Trim('/');
        int extracted = 0;
        for (int i = 0; i < a.Count; i++)
        {
            string p = a.FilePath(i);
            if (prefix.Length > 0 && !p.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
            byte[] d = a.ReadEntry(i);
            string dest = Path.Combine(outDir, p.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.WriteAllBytes(dest, d);
            extracted++;
        }
        Console.WriteLine($"extracted {extracted} files -> {outDir}");
        return 0;
    }

    /// <summary>
    /// 往返自证：以"全部组未修改"的方式走完整重建链（流式写出），结果必须与源 Script.pvf 逐字节一致。
    /// 覆盖外层包装、段加密、chunk 前缀 AES 与增量复用逻辑；不再依赖外部 sk.dat 文件（用已打开的密钥）。
    /// </summary>
    private static int Roundtrip()
    {
        var a = RequirePvf110(Open());
        var r = a.Pvf110!;
        byte[] rebuilt;
        using (var ms = new MemoryStream(checked((int)r.Stream.Length)))
        {
            Pvf110Rebuilder.RebuildToStream(r, ms,
                _ => throw new InvalidOperationException("roundtrip must not recompile any entry"),
                null, _ => false);
            rebuilt = ms.ToArray();
        }
        byte[] original = File.ReadAllBytes(PvfPath);
        bool same = rebuilt.AsSpan().SequenceEqual(original);
        Console.WriteLine($"roundtrip == original Script.pvf: {same} (rebuilt={rebuilt.Length} orig={original.Length})");
        return same ? 0 : 1;
    }

    /// <summary>解析本次会话实际使用的 sk.dat（显式 PVF_SKDAT 优先，其次 PVF 同目录探测）。</summary>
    private static string RequireSkDatPath()
    {
        string pvf = PvfPath;
        string? sk = ResolveSkDatPath(pvf);
        if (sk == null || !File.Exists(sk))
            throw new InvalidOperationException("sk.dat not found: set PVF_SKDAT or place sk.dat beside the Pvf110 Script.pvf");
        return sk;
    }

    private static int Repack(string outDir)
    {
        var a = RequirePvf110(Open());
        var r = a.Pvf110!;
        Directory.CreateDirectory(outDir);
        byte[] plain = (byte[])r.Stream.Clone();
        byte[] hashPlain = Pvf110Crypto.LcgDecryptKey(Slice(plain, r.Layout.HashOffset, r.Layout.NameOffset - r.Layout.HashOffset), "hash");
        Array.Copy(hashPlain, 0, plain, r.Layout.HashOffset, hashPlain.Length);
        byte[] groupPlain = Pvf110Crypto.LcgDecryptKey(Slice(plain, r.Layout.GroupOffset, r.Layout.BodyOffset - r.Layout.GroupOffset), "group");
        Array.Copy(groupPlain, 0, plain, r.Layout.GroupOffset, groupPlain.Length);
        for (int i = 0; i < r.Groups.Count; i++)
        {
            int prev = i > 0 ? r.Groups[i - 1].cumulative : 0;
            int aa = r.Layout.BodyOffset + prev;
            int bb = r.Layout.BodyOffset + r.Groups[i].cumulative;
            byte[] seg = Pvf110Crypto.LcgDecryptKey(Slice(plain, aa, bb - aa), "body");
            Array.Copy(seg, 0, plain, aa, seg.Length);
        }
        int nChunks = (plain.Length + Pvf110Crypto.ChunkStride - 1) / Pvf110Crypto.ChunkStride;
        byte[][] keys = new byte[nChunks][];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        for (int i = 0; i < nChunks; i++) { keys[i] = new byte[0x20]; rng.GetBytes(keys[i]); }
        byte[] enc = Pvf110Repack.PackPvf(plain, r.Header, keys, r.Groups, hashPreserved: true);
        byte[] sk = Pvf110Repack.BuildSkDat(keys);
        File.WriteAllBytes(Path.Combine(outDir, "Script.pvf"), enc);
        File.WriteAllBytes(Path.Combine(outDir, "sk.dat"), sk);
        Console.WriteLine($"repack done: Script.pvf {enc.Length}B sk.dat {sk.Length}B -> {outDir}");
        Console.WriteLine($"repack keys: freshly generated {keys.Length}; sk.dat rebuilt with {(ReferenceEquals(Pvf110Crypto.LastOpenedKeySet, null) ? "built-in" : "client-derived")} key set");
        var r2 = Pvf110Reader.Open(sk, enc);
        int mism = 0;
        var rnd = new Random(42);
        for (int i = 0; i < 200; i++)
        {
            var e = r2.Entries[rnd.Next(r2.Entries.Count)];
            byte[] aa2 = r.ReadEntry(r.Entries[e.Index]);
            byte[] bb2 = r2.ReadEntry(e);
            if (!aa2.AsSpan().SequenceEqual(bb2)) mism++;
        }
        Console.WriteLine($"reopen sample 200 entries mismatch: {mism}");
        return 0;
    }

    private static int ScanAll(int stride)
    {
        var a = Open();
        int type1 = 0, ok = 0, fail = 0, type3 = 0;
        var fails = new List<string>();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < a.Count; i++)
        {
            if (a.DataType(i) != 1) { type3++; continue; }
            if ((i % stride) != 0) continue;
            type1++;
            try
            {
                byte[] d = a.ReadEntry(i);
                byte[] back = a.Compiled.FromText(a.Compiled.ToText(d));
                if (SemanticEqual(a.Compiled, d, back)) ok++;
                else { fail++; if (fails.Count < 30) fails.Add(a.FilePath(i)); }
            }
            catch (Exception ex)
            {
                fail++;
                if (fails.Count < 30) fails.Add($"{a.FilePath(i)} :: {ex.Message}");
            }
            if ((type1 % 20000) == 0)
                Console.WriteLine($"  [{sw.Elapsed.TotalSeconds:F0}s] checked={type1} ok={ok} fail={fail}");
        }
        Console.WriteLine($"scan-all done: type1_checked={type1} ok={ok} fail={fail} type3={type3} time={sw.Elapsed.TotalSeconds:F0}s");
        foreach (var f in fails) Console.WriteLine("  FAIL " + f);
        return fail == 0 ? 0 : 1;
    }

    private static bool SemanticEqual(Pvf110Compiled c, byte[] orig, byte[] back)
    {
        var to = c.DecodeTokens(orig);
        var tb = c.DecodeTokens(back);
        if (to.Count != tb.Count) return false;
        for (int i = 0; i < to.Count; i++)
        {
            var (ta, va, ra) = to[i];
            var (tbn, vb, rb) = tb[i];
            if (ta != tbn) return false;
            if (ta is Pvf110Compiled.TagS03 or Pvf110Compiled.TagS06 or Pvf110Compiled.TagS08)
            {
                if ((string)va != (string)vb) return false;
            }
            else if (ta == Pvf110Compiled.TagI32 || ta == Pvf110Compiled.TagF32)
            {
                if (!va.Equals(vb)) return false;
            }
            else
            {
                if (ra != rb) return false;
            }
        }
        return true;
    }

    private static int Trace(string path)
    {
        var a = Open();
        string target = NormalizeArchivePath(path);
        int index = FindEntryIndex(a, target);
        if (index >= 0)
        {
            byte[] d = a.ReadEntry(index);
            byte[] back = a.Compiled.FromText(a.Compiled.ToText(d));
            Console.WriteLine($"{a.FilePath(index)}: orig={d.Length}B back={back.Length}B");
            var to = a.Compiled.DecodeTokens(d);
            var tb = a.Compiled.DecodeTokens(back);
            int n = Math.Max(to.Count, tb.Count);
            for (int k = 0; k < n; k++)
            {
                string os = k < to.Count ? Fmt(to[k]) : "(missing)";
                string bs = k < tb.Count ? Fmt(tb[k]) : "(missing)";
                string mark = os == bs ? "" : "  <<<";
                Console.WriteLine($"[{k}] {os}  |  {bs}{mark}");
            }
            return 0;
        }
        Console.Error.WriteLine("not found: " + target);
        return 1;
    }

    private static string Fmt((byte tag, object value, string raw) t)
        => t.tag switch
        {
            Pvf110Compiled.TagS03 => $"S03 {t.value}",
            Pvf110Compiled.TagS06 => $"S06 {t.value}",
            Pvf110Compiled.TagS08 => $"S08 {t.value}",
            Pvf110Compiled.TagI32 => $"I32 {t.value}",
            Pvf110Compiled.TagF32 => $"F32 {t.value}",
            _ => $"RAW {t.raw}",
        };

    private static int FindEmpty(int limit)
    {
        var a = Open();
        int checkedFiles = 0;
        int emptyStrFiles = 0;
        var examples = new List<string>();
        for (int i = 0; i < a.Count; i++)
        {
            if (a.DataType(i) != 1) continue;
            checkedFiles++;
            byte[] d = a.ReadEntry(i);
            bool hasEmpty = false;
            foreach (var (tag, value, _) in a.Compiled.DecodeTokens(d))
                if ((tag is Pvf110Compiled.TagS03 or Pvf110Compiled.TagS06 or Pvf110Compiled.TagS08) && value is string s && s.Length == 0)
                { hasEmpty = true; break; }
            if (hasEmpty)
            {
                emptyStrFiles++;
                if (examples.Count < 10) examples.Add(a.FilePath(i));
            }
            if (checkedFiles >= limit) break;
        }
        Console.WriteLine($"checked={checkedFiles} files-with-empty-string={emptyStrFiles}");
        foreach (var x in examples) Console.WriteLine("  " + x);
        return 0;
    }

    /// <summary>
    /// 诊断命令：统计 type-1 条目的 token 标签分布与各标签解析形态。
    /// 用于确认 90CN token 标签与经典 ScriptType(2..10) 的对应关系。
    /// `tags --sample [maxFiles]` 抽样全包；`tags <pvf路径>` 针对单文件；
    /// `tags --all` 全量普查（确认稀有标签 0x01/0x04/0x05/0x07/0x08/0x09 的真实分布）。
    /// </summary>
    private static int Tags(string path, int maxFiles)
    {
        var a = Open();
        var stats = new Dictionary<byte, (int total, int str, int bracket, int smallInt, int raw)>();
        var samples = new Dictionary<byte, List<string>>();
        var rareFiles = new Dictionary<byte, List<string>>();
        List<int> targets = new();
        bool fullCensus = path == "--all";
        if (fullCensus || path == "--sample")
        {
            int step = fullCensus ? 1 : Math.Max(1, a.Count / Math.Max(1, maxFiles));
            for (int i = 0; i < a.Count; i += step)
                if (a.DataType(i) == 1) targets.Add(i);
        }
        else
        {
            int idx = FindEntryIndex(a, path);
            if (idx < 0) { Console.Error.WriteLine($"path not found: {path}"); return 1; }
            targets.Add(idx);
        }

        void Add(byte tag, string desc, bool isStr, bool isBracket, bool isSmallInt, string filePath)
        {
            var cur = stats.TryGetValue(tag, out var v) ? v : (total: 0, str: 0, bracket: 0, smallInt: 0, raw: 0);
            stats[tag] = (cur.total + 1, cur.str + (isStr ? 1 : 0), cur.bracket + (isBracket ? 1 : 0),
                cur.smallInt + (isSmallInt ? 1 : 0), cur.raw + (isStr ? 0 : 1));
            if (!samples.TryGetValue(tag, out var list)) { list = new List<string>(); samples[tag] = list; }
            if (list.Count < 4 && desc.Length > 0) list.Add(desc);
            if (tag is not (0x00 or 0x02 or 0x03 or 0x06))
            {
                if (!rareFiles.TryGetValue(tag, out var fl)) { fl = new List<string>(); rareFiles[tag] = fl; }
                if (fl.Count < 8 && !fl.Contains(filePath)) fl.Add(filePath);
            }
        }

        foreach (int i in targets)
        {
            byte[] d = a.ReadEntry(i);
            // 独立原始 token 扫描：对每个标签都尝试用名称池解析，验证标签语义假设
            for (int off = 0; off + 5 <= d.Length; off += 5)
            {
                byte tag = d[off];
                int v = BitConverter.ToInt32(d, off + 1);
                bool resolved = false;
                string text = "";
                try
                {
                    string s = a.Compiled.Resolve(v);
                    if (s.Length > 0 && s.All(c => !char.IsControl(c)))
                    {
                        text = s.Length > 40 ? s[..40] : s;
                        resolved = true;
                    }
                }
                catch { }
                bool isStr = resolved;
                bool bracket = resolved && text.StartsWith('[');
                bool smallInt = !resolved && v is > -100000 and < 100000;
                Add(tag, text, isStr, bracket, smallInt, a.FilePath(i));
            }
        }

        Console.WriteLine($"# scanned entries: {targets.Count} / format={a.FormatName}");
        Console.WriteLine($"# tag  total      str        [label]    smallInt   unresolved   samples");
        foreach (var (tag, s) in stats.OrderBy(kv => kv.Key))
        {
            string samp = samples.TryGetValue(tag, out var list) ? string.Join(" | ", list.Take(3).Select(x => x.Replace("\n", "\\n").Replace("\r", ""))) : "";
            Console.WriteLine($"  {tag:X2}  {s.total,8}  {s.str,8}  {s.bracket,8}  {s.smallInt,8}  {s.raw,8}   {samp}");
        }
        foreach (var (tag, files) in rareFiles.OrderBy(kv => kv.Key))
        {
            Console.WriteLine($"# rare tag {tag:X2} appears in: {string.Join(" | ", files.Take(6))}");
        }
        return 0;
    }

    private static bool IsHex(string s)
        => s.All(c => Uri.IsHexDigit(c));

    private static int CompiledRoundtrip(string path)
    {
        var a = Open();
        string target = NormalizeArchivePath(path);
        int ok = 0, fail = 0;
        int index = FindEntryIndex(a, target);
        if (index >= 0)
        {
            byte[] d = a.ReadEntry(index);
            if (a.DataType(index) == 1)
            {
                string text = a.Compiled.ToText(d);
                byte[] back = a.Compiled.FromText(text);
                bool same = back.AsSpan().SequenceEqual(d);
                Console.WriteLine($"compiled-roundtrip {target}: {(same ? "OK" : "FAIL")} ({d.Length}B -> text {text.Length}B -> {back.Length}B)");
                if (!same)
                {
                    for (int k = 0; k < Math.Min(back.Length, d.Length); k++)
                        if (back[k] != d[k])
                        {
                            Console.WriteLine($"  first diff at {k}: back={back[k]:X2} orig={d[k]:X2}");
                            Console.WriteLine($"  back[{k - 4}..]={Convert.ToHexString(back.AsSpan(Math.Max(0, k - 4), 12))}");
                            Console.WriteLine($"  orig[{k - 4}..]={Convert.ToHexString(d.AsSpan(Math.Max(0, k - 4), 12))}");
                            break;
                        }
                    if (back.Length != d.Length)
                        Console.WriteLine($"  length mismatch: back={back.Length} orig={d.Length}");
                }
                if (same) ok++; else fail++;
            }
            else
            {
                string t3 = Encoding.Unicode.GetString(d, 0, Math.Min(200, d.Length));
                Console.WriteLine($"type-3 {target}: {t3.Replace("\n", "\\n")}");
                ok++;
            }
        }
        else
        {
            Console.Error.WriteLine("not found: " + target);
            return 1;
        }
        Console.WriteLine($"result ok={ok} fail={fail}");
        return fail == 0 ? 0 : 1;
    }

    /// <summary>
    /// NKPI 增量重建验证（RebuildIncrementalToFile）：
    /// 阶段1 全部未修改 → 纯组复用路径；阶段2 修改单个条目 → 单组重建、其余复用。
    /// 输出重开后逐条目内容比对；阶段2 还验证目标条目文本包含修改。
    /// </summary>
    private static int NkpiIncrementalTest(string outDir)
    {
        var a = Open();
        var nk = a.Nkpi ?? throw new InvalidOperationException("nkpi-incremental-test requires NKPI/ProtectedNKPI, current is " + a.FormatName);
        Directory.CreateDirectory(outDir);

        // 阶段1：全部未修改 → 所有组复用原始密文
        string phase1 = Path.Combine(outDir, "nochange.pvf");
        NkpiRepacker.RebuildIncrementalToFile(nk,
            _ => false,
            i => throw new InvalidOperationException($"getContent must not be called when nothing modified (entry {i})"),
            phase1);
        var r1 = NkpiReader.OpenFile(phase1);
        int mism1 = CompareAllEntries(a, nk, r1);
        Console.WriteLine($"phase1 no-change rebuild: entries={r1.Entries.Count} mismatches={mism1}");
        if (mism1 != 0 || r1.Entries.Count != nk.Entries.Count) return 1;

        // 阶段2：修改一个条目 → 其所在组重建，其余组复用
        string target = "stackable/10000001/10000039.stk";
        int targetIndex = FindEntryIndex(a, target);
        if (targetIndex < 0) { Console.Error.WriteLine("target not found"); return 1; }
        byte[] origToken = nk.ReadEntry(nk.Entries[targetIndex]);
        string text = a.Compiled.ToText(origToken);
        string modified = text.Replace("[grade]\n\t1\n", "[grade]\n\t99\n");
        if (modified == text) { Console.Error.WriteLine("modify pattern not found"); return 1; }
        byte[] newToken = a.Compiled.FromText(modified);

        string phase2 = Path.Combine(outDir, "onechange.pvf");
        NkpiRepacker.RebuildIncrementalToFile(nk,
            i => i == targetIndex,
            i => i == targetIndex ? newToken : throw new InvalidOperationException($"getContent called for unmodified entry {i}"),
            phase2);
        var r2 = NkpiReader.OpenFile(phase2);
        var e2 = r2.Entries[targetIndex];
        string text2 = new Pvf110Compiled(r2).ToText(r2.ReadEntry(e2));
        bool gradeChanged = text2.Contains("[grade]\n\t99\n");
        int mism2 = CompareAllEntries(a, nk, r2, skip: targetIndex);
        Console.WriteLine($"phase2 one-change rebuild: grade==99: {gradeChanged}, mismatches(excl target)={mism2}");
        if (!gradeChanged || mism2 != 0) return 1;

        // 阶段3：脚本文本（ToScriptText，GUI 显示格式）→ FromText 往返语义一致
        string[] samples =
        {
            "equipment/character/common/amulet/100300001.equ",
            "character/character.lst",
            "monster/monsterrapcdifficultybonus.tbl",
        };
        int rtFail = 0;
        foreach (string sample in samples)
        {
            int idx = FindEntryIndex(a, sample);
            if (idx < 0) { Console.WriteLine($"phase3 scripttext roundtrip: skip (not found) {sample}"); continue; }
            byte[] orig = nk.ReadEntry(nk.Entries[idx]);
            if (nk.Entries[idx].DataType != 1) { Console.WriteLine($"phase3 scripttext roundtrip: skip (type != 1) {sample}"); continue; }
            string scriptText = a.Compiled.ToScriptText(orig);
            byte[] back = a.Compiled.FromText(scriptText);
            bool ok = SemanticEqual(a.Compiled, orig, back);
            Console.WriteLine($"phase3 scripttext roundtrip: {sample} -> {(ok ? "PASS" : "FAIL")} ({orig.Length}B -> {scriptText.Length} chars -> {back.Length}B)");
            if (!ok)
            {
                rtFail++;
                if (rtFail == 1)
                {
                    Console.WriteLine("  orig tokens: " + string.Join(" | ", a.Compiled.DecodeTokens(orig).Take(8).Select(t => $"{t.tag}:{t.value}")));
                    Console.WriteLine("  back tokens: " + string.Join(" | ", a.Compiled.DecodeTokens(back).Take(8).Select(t => $"{t.tag}:{t.value}")));
                }
            }
        }
        return rtFail == 0 ? 0 : 1;
    }

    private static int CompareAllEntries(Archive a, NkpiReader original, NkpiReader rebuilt, int? skip = null)
    {
        int mism = 0;
        for (int i = 0; i < original.Entries.Count; i++)
        {
            if (skip.HasValue && i == skip.Value) continue;
            byte[] aa = original.ReadEntry(original.Entries[i]);
            byte[] bb = rebuilt.ReadEntry(rebuilt.Entries[i]);
            if (!aa.AsSpan().SequenceEqual(bb))
            {
                mism++;
                if (mism <= 20) Console.WriteLine("  MISMATCH " + a.FilePath(i));
            }
        }
        return mism;
    }

    /// <summary>
    /// Pvf110 增量重建合成验证：本机构造小型 Pvf110(sk.dat) 归档，
    /// 阶段1 全部未修改（整组复用路径），阶段2 修改单条目（单组重建+同组兄弟复用）。
    /// 两条路径都重开归档逐条目内容比对，覆盖精确缓冲组装、chunk-AES、sk.dat 回读。
    /// </summary>
    private static int Pvf110IncrementalSyntheticTest(string outDir)
    {
        Directory.CreateDirectory(outDir);
        const uint NameUtf8Xor = 0xE7ADF7EA;
        const uint NameCountA = 0xB8DEA7AC;

        // 1. 名称池（utf8 + 空 utf16）
        string folder = "stackable";
        string[] names = { "a.stk", "b.stk", "c.stk" };
        var poolPos = new Dictionary<string, int>();
        MemoryStream BuildPool()
        {
            var ms = new MemoryStream();
            void Add(string s)
            {
                if (poolPos.ContainsKey(s)) return;
                poolPos[s] = (int)ms.Position;
                byte[] b = Encoding.UTF8.GetBytes(s);
                ms.Write(b, 0, b.Length);
                ms.WriteByte(0);
            }
            Add(folder);
            foreach (string s in names) Add(s);
            return ms;
        }
        byte[] utf8Pool = BuildPool().ToArray();
        int Magic(string s) => poolPos[s] << 1;

        // 2. 条目内容（5 字节 token 流：S03 字符串引用 / I32 整数）
        byte[] Token(int a, int b, int c)
        {
            byte[] t = new byte[15];
            t[0] = 0x00; BitConverter.GetBytes(a).CopyTo(t, 1);
            t[5] = 0x00; BitConverter.GetBytes(b).CopyTo(t, 6);
            t[10] = 0x00; BitConverter.GetBytes(c).CopyTo(t, 11);
            return t;
        }
        byte[] c0 = Token(1, 11, 111);
        byte[] c1 = Token(2, 22, 222);
        byte[] c2 = Token(3, 33, 333);

        // 3. name 表（ParseNameTable 布局）
        byte[] cs1Raw = Pvf110Crypto.LcgDecryptName(Compress(utf8Pool), "utf8");
        byte[] utf16Empty = Pvf110Crypto.LcgDecryptName(Compress(Array.Empty<byte>()), "utf16");
        using var ntMs = new MemoryStream();
        ntMs.Write(new byte[8]);
        ntMs.Write(BitConverter.GetBytes(NameUtf8Xor ^ (uint)cs1Raw.Length));
        ntMs.Write(BitConverter.GetBytes((uint)utf8Pool.Length ^ (uint)cs1Raw.Length));
        ntMs.Write(cs1Raw);
        ntMs.Write(BitConverter.GetBytes(NameCountA ^ (uint)utf16Empty.Length));
        ntMs.Write(BitConverter.GetBytes((uint)0 ^ (uint)utf16Empty.Length));
        ntMs.Write(utf16Empty);
        byte[] nameTable = ntMs.ToArray();

        // 4. 组与 body（组0 = 条目0,1；组1 = 条目2），先组表后 body，交给 PackPvf 加密
        var contents = new[] { c0, c1, c2 };
        int[] chunkOf = { 0, 0, 1 };
        var groupMembers = new Dictionary<int, List<int>> { { 0, new List<int> { 0, 1 } }, { 1, new List<int> { 2 } } };

        byte[] Compress(byte[] data)
        {
            using var ms = new MemoryStream();
            using (var zs = new System.IO.Compression.ZLibStream(ms, System.IO.Compression.CompressionLevel.SmallestSize, leaveOpen: true))
                zs.Write(data, 0, data.Length);
            return ms.ToArray();
        }

        // 每组明文 = 组内条目顺序拼接；body 存压缩后的字节
        var groupComp = new Dictionary<int, byte[]>();
        var groupPlainLen = new Dictionary<int, int>();
        var groupOffsets = new int[3];
        var groupSizes = new int[3];
        foreach (var kv in groupMembers)
        {
            using var ms = new MemoryStream();
            int pos = 0;
            foreach (int i in kv.Value)
            {
                groupOffsets[i] = pos;
                groupSizes[i] = contents[i].Length;
                ms.Write(contents[i], 0, contents[i].Length);
                pos += contents[i].Length;
            }
            byte[] plain = ms.ToArray();
            groupPlainLen[kv.Key] = plain.Length;
            groupComp[kv.Key] = Compress(plain);
        }
        int cumulative = 0;
        var groupTable = new List<(int cumulative, int original)>();
        foreach (int g in new[] { 0, 1 })
        {
            cumulative += groupComp[g].Length;
            groupTable.Add((cumulative, groupPlainLen[g]));
        }

        using var logical = new MemoryStream();
        byte[] header = new byte[Pvf110Crypto.HeaderSize];
        // HASH 段：按真实布局构建并 LCG 加密（合成包也保持与真机同构，
        // 这样新增条目的 HASH 增量路径才能被真实覆盖）
        var hashPairs = new List<(uint name, uint path)>();
        for (int i = 0; i < 3; i++)
            hashPairs.Add(((uint)Magic(names[i]), (uint)Magic(folder)));
        byte[] hashSegment = Pvf110HashTable.Encrypt(
            Pvf110HashTable.BuildPlainText(hashPairs, utf8Pool, Array.Empty<byte>()));
        BitConverter.GetBytes(0x69706B6Eu).CopyTo(header, 0);
        BitConverter.GetBytes(3).CopyTo(header, 0x18);          // EntryCount
        BitConverter.GetBytes(0).CopyTo(header, 0x1C);          // Padding
        BitConverter.GetBytes(groupTable[^1].cumulative).CopyTo(header, 0x20); // BodySize
        BitConverter.GetBytes(2).CopyTo(header, 0x24);          // GroupCount
        BitConverter.GetBytes(hashSegment.Length).CopyTo(header, 0x28);
        BitConverter.GetBytes(nameTable.Length).CopyTo(header, 0x2C);
        logical.Write(header);
        for (int i = 0; i < 3; i++)
        {
            byte[] row = new byte[Pvf110Crypto.FileEntrySize];
            BitConverter.GetBytes(Magic(names[i])).CopyTo(row, 0);
            BitConverter.GetBytes(Magic(folder)).CopyTo(row, 4);
            BitConverter.GetBytes(chunkOf[i]).CopyTo(row, 8);
            BitConverter.GetBytes(groupOffsets[i]).CopyTo(row, 12);
            BitConverter.GetBytes(groupSizes[i]).CopyTo(row, 16);
            BitConverter.GetBytes(1).CopyTo(row, 20);           // DataType = 1
            logical.Write(row);
        }
        logical.Write(hashSegment);                              // HASH 段（已加密，hashPreserved 直通）
        logical.Write(nameTable);
        foreach (var (cum, orig) in groupTable)
        {
            logical.Write(BitConverter.GetBytes(cum));
            logical.Write(BitConverter.GetBytes(orig));
        }
        foreach (int g in new[] { 0, 1 })
            logical.Write(groupComp[g]);
        byte[] plainStream = logical.ToArray();

        int nChunks = (plainStream.Length + Pvf110Crypto.ChunkStride - 1) / Pvf110Crypto.ChunkStride;
        byte[][] keys = new byte[nChunks][];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        for (int i = 0; i < nChunks; i++) { keys[i] = new byte[0x20]; rng.GetBytes(keys[i]); }
        var h = new Pvf110Header
        {
            Magic = 0x69706B6Eu,
            Guid = new byte[0x14],
            EntryCount = 3,
            Padding = 0,
            BodySize = groupTable[^1].cumulative,
            GroupCount = 2,
            HashTableSize = hashSegment.Length,
            NameTableSize = nameTable.Length,
        };
        byte[] enc0 = Pvf110Repack.PackPvf(plainStream, h, keys, groupTable, hashPreserved: true);
        byte[] sk0 = Pvf110Repack.BuildSkDat(keys);
        var reader = Pvf110Reader.Open(sk0, enc0);
        if (reader.Entries.Count != 3) { Console.Error.WriteLine("synthetic open failed: entries != 3"); return 1; }
        for (int i = 0; i < 3; i++)
        {
            string expected = folder + "/" + names[i];
            if (reader.FilePath(reader.Entries[i]) != expected) { Console.Error.WriteLine($"synthetic path mismatch: {reader.FilePath(reader.Entries[i])} != {expected}"); return 1; }
        }
        Console.WriteLine("synthetic archive open ok: 3 entries, paths resolved");
        if (Environment.GetEnvironmentVariable("PVF_CLI_DEBUG") == "1")
        {
            Console.WriteLine($"layout: hash={reader.Layout.HashOffset} name={reader.Layout.NameOffset} group={reader.Layout.GroupOffset} body={reader.Layout.BodyOffset} end={reader.Layout.EndOffset}");
            Console.WriteLine($"groups: {string.Join(", ", reader.Groups)}");
            Console.WriteLine($"body[0..8]: {Convert.ToHexString(reader.Stream.AsSpan(reader.Layout.BodyOffset, Math.Min(8, reader.Stream.Length - reader.Layout.BodyOffset)))}");
            byte[] bodyDec = Pvf110Crypto.LcgDecryptKey(reader.Stream.AsSpan(reader.Layout.BodyOffset, 30).ToArray(), "body");
            Console.WriteLine($"body[0..8] after LCG(body): {Convert.ToHexString(bodyDec.AsSpan(0, Math.Min(8, bodyDec.Length)))}");
            Console.WriteLine($"expected deflate[0..8]: {Convert.ToHexString(groupComp[0].AsSpan(0, Math.Min(8, groupComp[0].Length)))}");
            Console.WriteLine($"group table region hex: {Convert.ToHexString(reader.Stream.AsSpan(reader.Layout.GroupOffset, 16))}");
            Console.WriteLine($"entry0: off={reader.Entries[0].DataOffset} size={reader.Entries[0].DataSize} chunk={reader.Entries[0].ChunkIndex}");
            byte[] probe = reader.ReadEntry(reader.Entries[0]);
            Console.WriteLine($"entry0 read ok: {probe.Length}B");
        }

        // 5. 阶段1：全部未修改 → 全组复用
        var (encA, skA) = Pvf110Rebuilder.Rebuild(reader,
            _ => throw new InvalidOperationException("getContent must not be called when nothing modified"),
            null, _ => false);
        var rA = Pvf110Reader.Open(skA, encA);
        int mismA = 0;
        for (int i = 0; i < 3; i++)
            if (!reader.ReadEntry(reader.Entries[i]).AsSpan().SequenceEqual(rA.ReadEntry(rA.Entries[i]))) mismA++;
        bool skReusedA = skA.AsSpan().SequenceEqual(sk0);
        Console.WriteLine($"phase1 no-change rebuild: mismatches={mismA} skdat_reused={skReusedA}");
        if (mismA != 0 || !skReusedA) return 1;

        // 6. 阶段2：修改条目1（同组兄弟条目0 复用，组1 复用）
        byte[] newC1 = Token(99, 22, 222);
        var (encB, skB) = Pvf110Rebuilder.Rebuild(reader,
            e => e.Index == 1 ? newC1 : throw new InvalidOperationException("getContent called for unmodified entry"),
            null, e => e.Index == 1);
        var rB = Pvf110Reader.Open(skB, encB);
        bool modOk = rB.ReadEntry(rB.Entries[1]).AsSpan().SequenceEqual(newC1);
        int mismB = 0;
        for (int i = 0; i < 3; i++)
        {
            if (i == 1) continue;
            if (!reader.ReadEntry(reader.Entries[i]).AsSpan().SequenceEqual(rB.ReadEntry(rB.Entries[i]))) mismB++;
        }
        bool skReusedB = skB.AsSpan().SequenceEqual(sk0);
        Console.WriteLine($"phase2 one-change rebuild: modified entry ok: {modOk}, mismatches(excl target)={mismB}, skdat_reused={skReusedB}");
        if (!modOk || mismB != 0 || !skReusedB) return 1;

        // 7. 阶段3：新增条目（名称池追加 + HASH 段增量 + 末尾新组）
        var addPool = Pvf110NamePool.FromReader(reader);
        byte[] c3 = Token(4, 44, 444);
        var additions = new[] { new Pvf110Addition("stackable/d.stk", c3, 1) };
        foreach (Pvf110Addition ad in additions)
        {
            string norm = ad.Path.Replace('\\', '/');
            int slash = norm.LastIndexOf('/');
            addPool.GetOrAdd(norm[(slash + 1)..]);
            addPool.GetOrAdd(norm[..slash]);
        }
        var (encC, skC) = Pvf110Rebuilder.Rebuild(reader,
            _ => throw new InvalidOperationException("getContent must not be called for a pure add"),
            null, _ => false, additions, Pvf110RebuildOptions.Default, addPool);
        var rC = Pvf110Reader.Open(skC, encC);
        bool addFound = false, addContentOk = false;
        for (int i = 0; i < rC.Entries.Count; i++)
        {
            if (rC.FilePath(rC.Entries[i]) != "stackable/d.stk") continue;
            addFound = true;
            addContentOk = rC.ReadEntry(rC.Entries[i]).AsSpan().SequenceEqual(c3);
        }
        int mismC = 0;
        for (int i = 0; i < 3; i++)
            if (!reader.ReadEntry(reader.Entries[i]).AsSpan().SequenceEqual(rC.ReadEntry(rC.Entries[i]))) mismC++;
        bool skReusedC = skC.AsSpan().SequenceEqual(sk0);
        bool hashGrew = rC.Header.HashTableSize > reader.Header.HashTableSize;
        bool nameGrew = rC.Header.NameTableSize > reader.Header.NameTableSize;
        Console.WriteLine($"phase3 add: entries={rC.Entries.Count} found={addFound} contentOk={addContentOk} " +
                          $"groups={rC.Header.GroupCount} mismatches(existing)={mismC} skdat_reused={skReusedC} hashGrew={hashGrew} nameGrew={nameGrew}");
        if (rC.Entries.Count != 4 || !addFound || !addContentOk || mismC != 0 || !skReusedC || !hashGrew || !nameGrew) return 1;

        // 8. 阶段4：新增条目 + 既有条目同批修改（write 与 add 混合路径）
        byte[] c0b = Token(7, 11, 111);
        var (encD, skD) = Pvf110Rebuilder.Rebuild(reader,
            e => e.Index == 0 ? c0b : throw new InvalidOperationException("getContent called for unmodified entry"),
            null, e => e.Index == 0, additions, Pvf110RebuildOptions.Default, addPool);
        var rD = Pvf110Reader.Open(skD, encD);
        bool modOkD = rD.ReadEntry(rD.Entries[0]).AsSpan().SequenceEqual(c0b);
        int mismD = 0;
        for (int i = 1; i < 3; i++)
            if (!reader.ReadEntry(reader.Entries[i]).AsSpan().SequenceEqual(rD.ReadEntry(rD.Entries[i]))) mismD++;
        bool addOkD = false;
        for (int i = 0; i < rD.Entries.Count; i++)
            if (rD.FilePath(rD.Entries[i]) == "stackable/d.stk")
                addOkD = rD.ReadEntry(rD.Entries[i]).AsSpan().SequenceEqual(c3);
        Console.WriteLine($"phase4 modify+add rebuild: entries={rD.Entries.Count} modified={modOkD} addOk={addOkD} mismatches(other existing)={mismD} skdat_reused={skD.AsSpan().SequenceEqual(sk0)}");
        if (!modOkD || !addOkD || mismD != 0) return 1;

        // 9. 阶段5：仅名称池新增（无新增条目，模拟 GUI 编辑引入池外新串）→ 只重建 name 表段，HASH 段不动
        var pool5 = Pvf110NamePool.FromReader(reader);
        int newMagic = pool5.GetOrAdd("brand_new_string_xyz");
        var (encE, skE) = Pvf110Rebuilder.Rebuild(reader,
            _ => throw new InvalidOperationException("getContent must not be called for a name-pool-only rebuild"),
            null, _ => false, null, Pvf110RebuildOptions.Default, pool5);
        var rE = Pvf110Reader.Open(skE, encE);
        bool resolved = rE.ResolveName(newMagic) == "brand_new_string_xyz";
        bool nameGrewE = rE.Header.NameTableSize != reader.Header.NameTableSize;
        bool hashSameE = rE.Header.HashTableSize == reader.Header.HashTableSize;
        int mismE = 0;
        for (int i = 0; i < 3; i++)
            if (!reader.ReadEntry(reader.Entries[i]).AsSpan().SequenceEqual(rE.ReadEntry(rE.Entries[i]))) mismE++;
        Console.WriteLine($"phase5 name-pool-only rebuild: newStringResolved={resolved} nameTableGrew={nameGrewE} hashUnchanged={hashSameE} mismatches={mismE} skdat_reused={skE.AsSpan().SequenceEqual(sk0)}");
        return (resolved && nameGrewE && hashSameE && mismE == 0 && skE.AsSpan().SequenceEqual(sk0)) ? 0 : 1;
    }

    private static int RebuildTest(string outDir)
    {        var a = RequirePvf110(Open());
        var r = a.Pvf110!;
        var compiled = a.Compiled;
        string target = "stackable/10000001/10000039.stk";
        int targetIndex = -1;
        for (int i = 0; i < r.Entries.Count; i++)
            if (r.FilePath(r.Entries[i]) == target) { targetIndex = i; break; }
        if (targetIndex < 0) { Console.Error.WriteLine("target not found"); return 1; }

        var targetEntry = r.Entries[targetIndex];
        byte[] origToken = r.ReadEntry(targetEntry);
        string text = compiled.ToText(origToken);
        string modified = text.Replace("[grade]\n\t1\n", "[grade]\n\t99\n");
        if (modified == text) { Console.Error.WriteLine("modify pattern not found"); return 1; }
        byte[] newToken = compiled.FromText(modified);
        Console.WriteLine($"modified {target}: {origToken.Length}B token -> {newToken.Length}B token");

        byte[] GetContent(Pvf110Entry e)
        {
            if (e.Index == targetIndex) return newToken;
            return r.ReadEntry(e);
        }

        Directory.CreateDirectory(outDir);
        string repackPath = Path.Combine(outDir, "Script.pvf");
        long enc;
        byte[] sk;
        using (FileStream rebuildStream = new FileStream(repackPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 1 << 20))
        {
            (enc, sk) = Pvf110Rebuilder.RebuildToStream(r, rebuildStream, GetContent, null, e => e.Index == targetIndex);
        }
        File.WriteAllBytes(Path.Combine(outDir, "sk.dat"), sk);
        Console.WriteLine($"rebuild done: Script.pvf {enc:N0}B sk.dat {sk.Length}B (sk.dat unchanged={sk.AsSpan().SequenceEqual(r.SkDatBytes)})");

        var r2 = Pvf110Reader.Open(sk, File.ReadAllBytes(repackPath));
        var e2 = r2.Entries[targetIndex];
        byte[] content2 = r2.ReadEntry(e2);
        string text2 = new Pvf110Compiled(r2).ToText(content2);
        bool gradeChanged = text2.Contains("[grade]\n\t99\n");
        Console.WriteLine($"modified file grade==99: {gradeChanged}");
        int mism = 0;
        var rnd = new Random(7);
        for (int i = 0; i < 300; i++)
        {
            int idx = rnd.Next(r.Entries.Count);
            if (idx == targetIndex) continue;
            byte[] aa = r.ReadEntry(r.Entries[idx]);
            byte[] bb = r2.ReadEntry(r2.Entries[idx]);
            if (!aa.AsSpan().SequenceEqual(bb)) mism++;
        }
        Console.WriteLine($"sample 300 unmodified entries mismatch: {mism}");
        return (gradeChanged && mism == 0) ? 0 : 1;
    }

    /// <summary>
    /// HASH 段结构自证:解密 → 按 NkpiHashTable 逆向确认的布局重新构建 → 逐字节对比。
    /// mismatch=0 表示算法完全复现真实文件;同时报告 pairs 与文件表的集合差异。
    /// NKPI/ProtectedNKPI 与 Pvf110 共用同一布局,仅段密钥不同（前者 Protected 用 "hash"，
    /// 后者 Pvf110 用 "HSrm"）；两种格式都走同一条自证链。
    /// </summary>
    private static int HashTest()
    {
        var a = Open();
        NkpiReader? nk = a.Nkpi;
        string keyLabelVm;
        byte[] enc, plain;
        byte[] utf8Pool, utf16Pool;
        int entryCount;
        if (nk != null)
        {
            enc = nk.Stream.AsSpan(nk.Layout.HashOffset, nk.Header.HashTableSize).ToArray();
            plain = NkpiHashTable.Decrypt(enc);
            utf8Pool = nk.Utf8Pool;
            utf16Pool = nk.Utf16Pool;
            entryCount = nk.Header.FileCount;
            keyLabelVm = "hash(utf16,SegInc)";
        }
        else
        {
            var r = a.Pvf110!;
            enc = r.HashEncrypted;
            plain = Pvf110HashTable.Decrypt(enc);
            utf8Pool = r.Utf8Pool;
            utf16Pool = r.Utf16Pool;
            entryCount = r.Header.EntryCount;
            keyLabelVm = "HSrm(utf16,LcgInc)";
        }
        Console.WriteLine($"format={a.FormatName} hashKey={keyLabelVm}");
        var (pairCount, uniqueCount, mismatch, firstBad) = NkpiHashTable.SelfTest(plain, utf8Pool, utf16Pool);
        Console.WriteLine($"hash decrypt: plainSize={plain.Length:N0} expected={enc.Length:N0} sizeOk={plain.Length == enc.Length}");
        Console.WriteLine($"hash layout: pairCount={pairCount:N0} uniqueCount={uniqueCount:N0} (entryCount={entryCount:N0})");
        Console.WriteLine($"SELFTEST rebuild-vs-real: mismatchBytes={mismatch:N0} firstMismatch={firstBad}  {(mismatch == 0 ? "PASS (algorithm fully reproduces real file)" : "DEVIATION (see tail analysis below)")}");

        // 尾部唯一集合必须与 pairs 推导出的集合等势；不等势才是真失败，纯排序偏差属源归档固有
        int tailStart0 = 4 + pairCount * 8 + 4;
        var realTail = new List<uint>(uniqueCount);
        for (int i = 0; i < uniqueCount; i++)
            realTail.Add(BitConverter.ToUInt32(plain, tailStart0 + i * 4));
        var derived = new HashSet<uint>();
        for (int i = 0; i < pairCount; i++)
        {
            uint n2 = BitConverter.ToUInt32(plain, 4 + i * 8);
            uint p2 = BitConverter.ToUInt32(plain, 4 + i * 8 + 4);
            if (n2 != NkpiHashTable.NullPath) derived.Add(n2);
            if (p2 != NkpiHashTable.NullPath) derived.Add(p2);
        }
        var realSet = new HashSet<uint>(realTail);
        int setOnlyInDerived = derived.Count(m => !realSet.Contains(m));
        int setOnlyInReal = realSet.Count(m => !derived.Contains(m));
        Console.WriteLine($"tail set check: derived={derived.Count:N0} real(unique)={realSet.Count:N0} onlyInDerived={setOnlyInDerived} onlyInReal={setOnlyInReal}");

        int tailStart = tailStart0;
        int orderViol = 0;
        var violIndices = new List<int>();
        byte[] prev = Array.Empty<byte>();
        for (int i = 0; i < uniqueCount; i++)
        {
            uint m = BitConverter.ToUInt32(plain, tailStart + i * 4);
            byte[] seg = NkpiHashTable.PoolSegment(utf8Pool, utf16Pool, m);
            if (prev.Length > 0 && NkpiHashTable.CompareSegment(prev, seg) > 0)
            {
                orderViol++;
                if (violIndices.Count < 5) violIndices.Add(i);
            }
            prev = seg;
        }
        Console.WriteLine($"real-tail ordering check: ordinal-ascending violations={orderViol:N0} / {uniqueCount:N0}"
            + (violIndices.Count > 0 ? $" at indices {string.Join(",", violIndices)}" : ""));
        if (orderViol > 0 && firstBad >= tailStart)
        {
            int pos = (int)((firstBad - tailStart) / 4);
            for (int i = Math.Max(0, pos - 2); i < Math.Min(uniqueCount, pos + 4); i++)
            {
                uint m = realTail[i];
                string s = a.Compiled.Resolve(unchecked((int)m));
                string cmp = i + 1 < realTail.Count && i + 1 < uniqueCount
                    ? NkpiHashTable.CompareSegment(
                        NkpiHashTable.PoolSegment(utf8Pool, utf16Pool, m),
                        NkpiHashTable.PoolSegment(utf8Pool, utf16Pool, realTail[i + 1])).ToString()
                    : "-";
                Console.WriteLine($"  tail[{i:N0}] magic={m:X8} cmpNext={cmp} str={s}");
            }
            Console.WriteLine("  => 源归档自身的尾部排序存在少量偏差；客户端按字符串定位，排序仅供定位加速。");
            Console.WriteLine("  => 写回路径对无新增条目的编辑原样保留 HASH 段密文，不放大该偏差；新增条目走 AppendPairs 增量插入。");
        }

        // pairs 与文件表的集合差异(历史幽灵/缺失)
        var ft = new System.Collections.Generic.HashSet<ulong>();
        for (int i = 0; i < a.Count; i++)
        {
            uint n2 = EntryNameMagic(a, i);
            uint p2 = EntryPathMagic(a, i);
            ft.Add(((ulong)n2 << 32) | p2);
        }
        int ghosts = 0, missing = 0;
        var ghostList = new List<string>();
        for (int i = 0; i < pairCount; i++)
        {
            uint n2 = BitConverter.ToUInt32(plain, 4 + i * 8);
            uint p2 = BitConverter.ToUInt32(plain, 4 + i * 8 + 4);
            ulong k = ((ulong)n2 << 32) | p2;
            if (ft.Contains(k)) { ft.Remove(k); }
            else { ghosts++; if (ghostList.Count < 10) ghostList.Add($"{a.Compiled.Resolve(unchecked((int)n2))} @ {a.Compiled.Resolve(unchecked((int)p2))}"); }
        }
        missing = ft.Count;
        Console.WriteLine($"pairs-vs-filetable: ghosts(hash-only)={ghosts:N0} missing(filetable-only)={missing:N0}");
        foreach (string g in ghostList) Console.WriteLine($"  ghost: {g}");

        bool layoutOk = pairCount == entryCount && ghosts == 0 && missing == 0
                        && setOnlyInDerived == 0 && setOnlyInReal == 0 && orderViol == 0;
        Console.WriteLine(layoutOk
            ? "RESULT PASS: pairs 与文件表完全一致、尾部集合等势且严格升序"
            : (setOnlyInDerived == 0 && setOnlyInReal == 0 && ghosts == 0 && missing == 0
                ? "RESULT PASS(layout): pairs 与文件表完全一致、尾部集合等势；仅存在源归档固有的排序偏差（见上）"
                : "RESULT FAIL: HASH 布局或集合与本工具模型不符"));
        return (setOnlyInDerived == 0 && setOnlyInReal == 0 && ghosts == 0 && missing == 0 && pairCount == entryCount) ? 0 : 1;
    }

    private static int Validate()
    {
        var a = Open();
        int bad = 0;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < a.Count; i++)
        {
            try
            {
                byte[] d = a.ReadEntry(i);
                if (a.DataType(i) == 1) a.Compiled.ToText(d);
            }
            catch (Exception ex)
            {
                bad++;
                if (bad <= 30) Console.Error.WriteLine($"BAD {a.FilePath(i)} :: {ex.Message}");
            }
        }
        Console.WriteLine($"validate done: entries={a.Count} bad={bad} time={sw.Elapsed.TotalSeconds:F0}s");
        return bad == 0 ? 0 : 1;
    }

    private static string RequiredPath(string variable)
    {
        string? value = Environment.GetEnvironmentVariable(variable);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"set environment variable {variable} before running this command");
        return Path.GetFullPath(value);
    }

    private static byte[] Slice(byte[] data, int offset, int length)
    {
        byte[] result = new byte[length];
        Array.Copy(data, offset, result, 0, length);
        return result;
    }

    private static void Usage()
    {
        Console.WriteLine("Pvf110.Cli <cmd> [args]  (pvfUtility CLI / AI 工作版)");
        Console.WriteLine($"  contract={AiCliContractVersion}; GUI pvfUtility.exe 仅供用户使用，AI 不得启动 GUI");
        Console.WriteLine("  required environment: PVF_PATH=<Script.pvf>");
        Console.WriteLine("  optional environment: PVF_SKDAT=<sk.dat> (Pvf110 only; 缺省探测 PVF 同目录 sk.dat), PVF_OUTPUT_DIR=<out>");
        Console.WriteLine("  optional environment: PVF_CLIENT_EXE=<client exe>  派生 Pvf110 外层包装密钥");
        Console.WriteLine("                        （缺省探测 PVF 同目录的 DFO.exe / DNF.exe；源码内不保存密钥）");
        Console.WriteLine("  optional environment: PVF_TEXT_ENCODING=<name>  非 type-1 文本块解码（cp949/gb18030/utf-8…）");
        Console.WriteLine("                        PVF_TEXT_CHARS=<n>  非 type-1 文本块输出字符数上限（缺省 300；0 不限）");
        Console.WriteLine("  自动检测 NKPI/ProtectedNKPI(90CN) 与 Pvf110(115) 格式；读写命令两种格式通用");
        Console.WriteLine("  version                     输出 AI CLI 合同版本、格式与能力边界");
        Console.WriteLine("  info                        打开并打印格式/header/布局/密钥来源/前几项");
        Console.WriteLine("  list [prefix]               列出全部文件路径（可按前缀过滤）");
        Console.WriteLine("  file <path>                 读取指定文件内容(hex 头)");
        Console.WriteLine("  decompile <path>            解编译 type-1 文件为文本（非 type-1 按 PVF_TEXT_ENCODING 输出）");
        Console.WriteLine("  batch-decompile <pathlist>  批量解编译（从文件逐行读路径，高效复用会话）");
        Console.WriteLine("  batch-decompile-script <pathlist> <outdir>  批量解编译为 ToScriptText 文本文件（write 兼容格式）");
        Console.WriteLine("  batch-write <manifest> [outdir]  批量写回（清单：archivePath<TAB>file[<TAB>auto|text|raw|block]；单次落盘+逐条校验）");
        Console.WriteLine("  tags <path|--sample> [n]    诊断：token 标签分布统计（确认标签语义）");
        Console.WriteLine("  write <path> <file> [outdir]  修改指定条目内容并按增量路径写回（两格式均只重压缩所在组，附回读校验）");
        Console.WriteLine("  add <file-or-dir> <archive-path-or-prefix> [outdir]  定向新增文件/文件夹（NKPI 与 Pvf110 均可）");
        Console.WriteLine("  mainline-epicdiff <manifest> <template> [outdir]  生成主线五级难度表并替换 DGN 引用");
        Console.WriteLine("  tune-monster-base [outdir]  三类怪物基础表共享平滑攻击倍率并生成候选");
        Console.WriteLine("  search <text> [prefix] [type1|all]  内容检索（all 含非 type-1 文本块，需配 PVF_TEXT_ENCODING）");
        Console.WriteLine("  extract [outdir] [prefix]   按路径提取文件到目录");
        Console.WriteLine("  compiled-roundtrip [path]   解编译->重编译->与原二进制对比");
        Console.WriteLine("  find-empty [limit]          扫描含空字符串 token 的 type-1 文件");
        Console.WriteLine("  scan-all [stride]           全量往返扫描（stride=每 N 个取 1）");
        Console.WriteLine("  rebuild-test [outdir]       (Pvf110) 修改内容->重建->重开验证");
        Console.WriteLine("  roundtrip                   (Pvf110) 重打包往返一致性校验");
        Console.WriteLine("  repack [outdir]             (Pvf110) 重打包并重新打开抽样验证");
        Console.WriteLine("  nkpi-incremental-test [outdir]  (NKPI) 增量重建三阶段验证");
        Console.WriteLine("  pvf110-incremental-test [outdir]  (Pvf110) 增量重建四阶段验证（含新增条目与 sk.dat 复用）");
        Console.WriteLine("  validate                    全量读取+解编译校验");
        Console.WriteLine("  hash-test                   HASH 段结构自证(解密→重建→逐字节对比)，两格式通用");
        Console.WriteLine("  keyset-probe [exe] [--emit|--write <material.json>]  验证客户端密钥集并输出/落库材料条目（换版本无需改代码）");
    }
}
