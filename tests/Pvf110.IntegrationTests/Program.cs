using System.IO;
using System.Text;
using PvfCode;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;
using ServiceLocator;

namespace Pvf110.IntegrationTests;

/// <summary>
/// 统一管线端到端回归（90CN NKPI/ProtectedNKPI，headless）：
/// 打开 PVF（原文件只读）→ 虚拟串表 → 经典视图/富格式反编译 → 名称扫描/LST 表 →
/// 编辑保存到临时目录 → 重开逐字节/语义比对。输入取环境变量 PVF_PATH，
/// 输出固定写到 PVF_OUTPUT_DIR（不触碰原 PVF）。
/// </summary>
internal static class Program
{
    private static string PvfPath =>
        Environment.GetEnvironmentVariable("PVF_PATH")
        ?? throw new InvalidOperationException("set PVF_PATH");
    private static string OutDir =>
        Environment.GetEnvironmentVariable("PVF_OUTPUT_DIR")
        ?? Path.Combine(Path.GetTempPath(), "pvf110_it_out");

    private static int _failures;

    private static void Check(string name, bool ok, string? detail = null)
    {
        Console.WriteLine($"  [{(ok ? "PASS" : "FAIL")}] {name}{(string.IsNullOrEmpty(detail) ? "" : " :: " + detail)}");
        if (!ok) _failures++;
    }

    private static int Main()
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ServiceContainer.Instance.AddService<Ilogger>(new MockLogger());
            _ = AppSetting.Instance;
            Directory.CreateDirectory(OutDir);

            // 1. 打开（统一管线：虚拟串表 + 经典视图）
            var group = new PvfGroup();
            bool opened = group.OpenPvfPack(PvfPath, new Progress<double>()).GetAwaiter().GetResult();
            Console.WriteLine($"open={opened} IsPvf110={group.IsPvf110} files={group.FileList?.Count}");
            Check("open", opened);
            if (!opened) return 1;

            if (Environment.GetEnvironmentVariable("MEASURE_READER") == "1")
            {
                long before = GC.GetTotalMemory(true);
                var reader = Pvf110.Core.Pvf110Reader.OpenPvf(PvfPath);
                long afterReader = GC.GetTotalMemory(true);
                Console.WriteLine($"reader-only heap: {before / 1024.0 / 1024.0:F0} MB -> {afterReader / 1024.0 / 1024.0:F0} MB " +
                                  $"(entries={reader.Entries.Count}, groups={reader.Groups.Count}, utf16Pool={reader.Utf16Pool.Length / 1024.0 / 1024.0:F0} MB, stream={reader.Stream.Length / 1024.0 / 1024.0:F0} MB)");
            }

            string? sweep = Environment.GetEnvironmentVariable("SWEEP");
            if (!string.IsNullOrEmpty(sweep))
            {
                RunDecompileSweep(group, int.Parse(sweep));
            }

            string? sweepPaths = Environment.GetEnvironmentVariable("SWEEP_PATHS");
            if (!string.IsNullOrEmpty(sweepPaths))
            {
                foreach (string path in sweepPaths.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        var f = group.GetFile(path.Trim());
                        if (f == null) { Console.WriteLine($"    path-probe MISSING {path}"); continue; }
                        group.EnsureFileData(f.FileName);
                        byte[] before = (byte[])f.Data.Clone();
                        string richText = group.GetFileText(f);
                        bool same = group.SaveFileAsScript(f, richText) && before.AsSpan().SequenceEqual(f.Data);
                        Console.WriteLine($"    path-probe {(same ? "OK" : "ROUNDTRIP-DIFF")} {path} " +
                                          $"(classic {before.Length}->{f.Data.Length}, text {richText.Length} chars)");
                        if (!same)
                        {
                            string tag = Path.GetFileName(path).Replace('.', '_');
                            File.WriteAllBytes(Path.Combine(OutDir, tag + ".before.bin"), before);
                            File.WriteAllBytes(Path.Combine(OutDir, tag + ".after.bin"), f.Data);
                            Console.WriteLine($"      dump: {tag}.before.bin / .after.bin; {DescribeTokenDiff(before, f.Data)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"    path-probe FAIL {path}: {ex.GetType().Name}: {ex.Message}");
                        Console.WriteLine(ex.StackTrace);
                    }
                }
            }

            // 探针：type-3（非 type-1 块）在 GUI 文本层的 解码 → 保存 往返保真性。
            // 环境变量 PROBE_TEXT_PATHS = 以 ';' 分隔的 PVF 路径。
            string? probePaths = Environment.GetEnvironmentVariable("PROBE_TEXT_PATHS");
            if (!string.IsNullOrEmpty(probePaths)) return RunTextEncodingProbe(group, probePaths);

            // 扫描：抽样统计“经典视图往返是否无损”，按扩展名汇总（GUI 编辑保存的正确性上限）。
            string? sweepRt = Environment.GetEnvironmentVariable("SWEEP_RT");
            if (!string.IsNullOrEmpty(sweepRt)) return RunClassicRoundTripSweep(group, int.Parse(sweepRt));

            // Pvf110（115 美服 Builder 包装 + sk.dat）走专项回归：打开与保存链路与 NKPI 不同
            if (group.IsPvf110) return RunPvf110Regression(group);

            // 2. 虚拟串表：IsAny + 双向解析 + 名称标签注册
            Check("strtable.IsAny", group.Strtable.IsAny());
            int nameId = group.Strtable.GetStringTableId("[name]");
            Check("strtable has [name]", nameId != -1, $"id={nameId}");
            if (nameId != -1)
            {
                Check("strtable roundtrip [name]", group.Strtable.GetStringItem(nameId) == "[name]");
            }

            // 3. 打开后不置 IsUpdated（GUI 树不再整包显示 Up）
            Check("no file IsUpdated after open", !group.FileList.Values.Any(f => f.IsUpdated));

            // 4. 经典视图：type-1 懒加载后 Data 带经典魔数（IsScriptFile 成立）
            var stk = group.GetFile("stackable/10000001/10000039.stk");
            Check("stk exists", stk != null);
            if (stk == null) return 1;
            Check("EnsureFileData", group.EnsureFileData(stk.FileName));
            Check("classic magic 0xD0B0", stk.IsScriptFile, $"DataLen={stk.DataLen}");

            // 5. 富格式反编译（旧版管线接管，非扁平文本）；全文落盘备查
            string decompiled = group.GetFileText(stk);
            File.WriteAllText(Path.Combine(OutDir, "decompiled.stk.txt"), decompiled, new UTF8Encoding(false));
            Check("decompile has #PVF_File", decompiled.StartsWith("#PVF_File", StringComparison.Ordinal));
            Check("decompile rich format (no RAW lines)", decompiled.Contains('[') && !decompiled.Contains("RAW\t"));

            // 6. 编辑 → 保存（经典编译器 → 经典视图；包保存适配回 110）
            //    富格式反编译排版（ScriptFileParserNew）：[grade]\r\n\t1（值行 TAB 前缀、无后缀）
            string modified = decompiled.Replace("[grade]\r\n\t1\r\n", "[grade]\r\n\t99\r\n");
            Check("modify pattern found", modified != decompiled);
            File.WriteAllText(Path.Combine(OutDir, "modified.stk.txt"), modified, new UTF8Encoding(false));
            bool savedFile = group.SaveFileAsScript(stk, modified);
            Check("SaveFileAsScript", savedFile);

            // 7. 名称扫描（经典 GetItemName 分支 + LST 代码表）
            WaitForPostOpenInit(group);
            int itemCodes = 0;
            for (int retry = 0; retry < 20; retry++)
            {
                try
                {
                    itemCodes = 0;
                    foreach (var kv in group.ListFileTable.CodeDic) itemCodes += kv.Value.Count;
                    break;
                }
                catch (InvalidOperationException) { Thread.Sleep(500); }
            }
            Check("LST code table loaded", itemCodes > 0, $"entries={itemCodes}");

            // 7a. 有名物品：从 equipment 代码表取第一个可寻址文件验证名称解析
            string? itemName = null;
            string? namedFile = null;
            if (group.ListFileTable.CodeDic.TryGetValue("equipment", out var equDic))
            {
                // 快照枚举：后台 LST 加载线程仍可能向表内增量写入，冲突则等待重试
                List<LstItem> snapshot = new();
                for (int retry = 0; retry < 20; retry++)
                {
                    try { snapshot = equDic.Values.Take(50).ToList(); break; }
                    catch (InvalidOperationException) { Thread.Sleep(500); }
                }
                foreach (var item in snapshot)
                {
                    var f = group.GetFile("equipment/" + item.ItemPath.Replace('\\', '/').ToLower());
                    if (f == null) continue;
                    itemName = group.GetItemName(f);
                    if (!string.IsNullOrEmpty(itemName)) { namedFile = f.FileName; break; }
                }
            }
            Check("GetItemName resolves item name", !string.IsNullOrEmpty(itemName),
                $"{namedFile} -> {itemName}");

            // 7b. 套装表（etc/equipmentpartset.etc 专项解析：90CN"套装名"版式自适应，不得报错且必须加载）
            for (int retry = 0; retry < 60 && group.EquipmentPartSetTable.Items.Count == 0; retry++)
                Thread.Sleep(500);
            Check("equipment part set table loaded", group.EquipmentPartSetTable.Items.Count > 0,
                $"sets={group.EquipmentPartSetTable.Items.Count}");

            // 8. 保存到临时目录（不触碰原 PVF）并重开验证
            string outPvf = Path.Combine(OutDir, "Script.pvf");
            var result = group.SavePvfPack(outPvf, isFastMode: true, new Progress<double>(), notButtonClick: true)
                .GetAwaiter().GetResult();
            Check("SavePvfPack", !result.IsError, result.Msg);
            if (result.IsError || !File.Exists(outPvf)) return 1;

            using (var verify = Pvf110.Core.NkpiReader.OpenFile(outPvf))
            using (var original = Pvf110.Core.NkpiReader.OpenFile(PvfPath))
            {
                Check("entry count preserved", verify.Entries.Count == original.Entries.Count,
                    $"{verify.Entries.Count} vs {original.Entries.Count}");
                Check("verify format matches", verify.Format == original.Format);

                // 8a. 修改条目语义等价（grade==99）
                int targetIdx = FindIndex(verify, stk.FileName);
                Check("modified entry present", targetIdx >= 0);
                if (targetIdx >= 0)
                {
                    string textBack = new Pvf110.Core.Pvf110Compiled(verify).ToText(verify.ReadEntry(verify.Entries[targetIdx]));
                    Check("modified grade==99", textBack.Contains("[grade]\n\t99\n"));
                }

                // 8b. 未修改条目字节保真（确定性抽样 300）
                var rnd = new Random(20260901);
                int modifiedIdx = FindIndex(original, stk.FileName);
                int mism = 0, checkedCount = 0;
                for (int i = 0; i < 300; i++)
                {
                    int idx = rnd.Next(original.Entries.Count);
                    if (idx == modifiedIdx) continue;
                    if (!original.ReadEntry(original.Entries[idx]).AsSpan()
                        .SequenceEqual(verify.ReadEntry(verify.Entries[idx]))) mism++;
                    checkedCount++;
                }
                Check("unmodified entries byte-identical (sample 300)", mism == 0,
                    $"checked={checkedCount} mismatch={mism}");
            }

            Console.WriteLine($"\nRESULT: {(_failures == 0 ? "PASS" : "FAIL")}");

            // ── 专项诊断：equipmentpartset.etc 报错段的 children 实际序列 ──
            if (Environment.GetEnvironmentVariable("EPSET_PROBE") == "1")
            {
                var etcFile = group.GetFile("etc/equipmentpartset.etc");
                group.EnsureFileData(etcFile!.FileName);
                var parser = new PvfCode.Services.PvfParsingNew.ScriptFileParserNew(etcFile, group);
                parser.PraseStructureMain();
                int dumped = 0;
                foreach (var sec in parser.Sections.OfType<PvfCode.Services.PvfParsingNew.PvfSection>())
                {
                    var texts = sec.Children.Select(c =>
                    {
                        if (c is PvfCode.Services.PvfParsingNew.PvfSection ps) return "<SEC:" + ps.GetSectionName() + ">";
                        return c.Item == null ? "<null>" : $"{c.Item.Type}:{c.Item.GetItemText(group)}";
                    }).ToList();
                    bool isProbe = texts.Count > 1 && (texts[1].Contains("12386") || texts[1].Contains("12387"));
                    if (!isProbe || dumped >= 2) continue;
                    dumped++;
                    Console.WriteLine($"\n--- section dump (children={sec.Children.Count}) ---");
                    for (int i = 0; i < sec.Children.Count; i++)
                        Console.WriteLine($"  [{i}] {texts[i]}");
                }
            }
            return _failures == 0 ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"FATAL: {ex}");
            return 1;
        }
    }

    /// <summary>
    /// 富格式反编译抽样扫描（GUI 打开文件走的就是这条 GetFileText 路径）：
    /// 均匀抽样 target 个条目，只对 type-1 脚本调用，统计异常并打印前若干例。
    /// </summary>
    private static void RunDecompileSweep(PvfGroup group, int target)
    {
        List<PvfFile> files = group.FileList!.Values.ToList();
        int stride = Math.Max(1, files.Count / Math.Max(1, target));
        int scanned = 0, scripts = 0, failures = 0, shown = 0;
        for (int i = 0; i < files.Count && scanned < target; i += stride)
        {
            PvfFile f = files[i];
            scanned++;
            try
            {
                if (!group.EnsureFileData(f.FileName)) continue;
                if (!f.IsScriptFile) continue;
                scripts++;
                _ = group.GetFileText(f);
            }
            catch (Exception ex)
            {
                failures++;
                if (shown < 10)
                {
                    shown++;
                    Console.WriteLine($"    sweep FAIL {f.FileName}: {ex.GetType().Name}: {ex.Message}");
                }
            }
        }
        Check("rich decompile sweep", failures == 0, $"scanned={scanned} scripts={scripts} failures={failures}");
    }

    /// <summary>
    /// type-3 文本编码往返探针：对每个路径执行 GUI 的真实动作序列——
    /// EnsureFileData（懒加载原始条目字节）→ GetFileText（编辑器显示文本，按 DefaultEncoding 解码）
    /// → SaveFileText（用户按保存文档）→ 比对 file.Data 与原条目字节，再把整包保存到输出目录并用
    /// Pvf110Reader 直接回读该条目，判定写进归档的内容是否正确。
    /// 判据：**用户未做任何修改**时，条目字节必须逐字节不变（“打开→保存”不得改内容）。
    /// </summary>
    private static int RunTextEncodingProbe(PvfGroup group, string paths)
    {
        Console.WriteLine($"default encoding = {AppSetting.Instance.PvfConfig.DefaultEncoding}");
        Console.WriteLine($"IsPvf110={group.IsPvf110} Is110Format={group.Is110Format}");
        string outDir = Path.Combine(OutDir, "probe");
        Directory.CreateDirectory(outDir);
        string editedPvf = Path.Combine(outDir, "Script.pvf");

        // 真值来源：直接从源归档取 110 原始 token 流，作为“用户没改任何东西”的对照基线。
        var source = Pvf110.Core.Pvf110Reader.OpenPvf(PvfPath);
        var sourceBytes = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < source.Entries.Count; i++)
        {
            Pvf110.Core.Pvf110Entry e = source.Entries[i];
            sourceBytes[source.FilePath(e)] = source.ReadEntry(e);
        }

        var targets = new List<(string Path, byte[] Original)>();
        foreach (string raw in paths.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            string path = raw.Trim();
            var file = group.GetFile(path);
            if (file == null) { Console.WriteLine($"  [SKIP] not found: {path}"); continue; }
            bool loaded = false;
            try { loaded = group.EnsureFileData(file.FileName); }
            catch (Exception ex) { Console.WriteLine($"  [EXC ] EnsureFileData {path}: {ex.GetType().Name}: {ex.Message}"); }
            bool known = sourceBytes.TryGetValue(path, out byte[]? original);
            Console.WriteLine($"  type={file.Pvf110DataType} fileType={file.FileType} " +
                              $"archiveDataLen={(known ? original!.Length : -1)} loadedDataLen={file.DataLen} " +
                              $"loaded={loaded} readOnly={file.IsRawReadOnly}");
            if (!known) { Console.WriteLine($"  [SKIP] not in source reader: {path}"); continue; }

            string text;
            try { text = group.GetFileText(file); }
            catch (Exception ex) { text = string.Empty; Console.WriteLine($"  [EXC ] GetFileText: {ex.Message}"); }
            Console.WriteLine($"  editor text {text.Length} chars: " +
                              $"{Truncate(text.Replace("\r\r\n", "\\r\\n").Replace("\r\n", "\\n"), 88)}");

            bool saved;
            try { saved = group.SaveFileText(file, text); }
            catch (Exception ex) { saved = false; Console.WriteLine($"  [EXC ] SaveFileText: {ex.Message}"); }
            if (file.IsRawReadOnly)
            {
                // 只读保护：必须拒绝改写（保存时按原始字节写回）
                Check($"SaveFileText refused by read-only guard ({path})", !saved);
            }
            else
            {
                Check($"SaveFileText({path})", saved);
            }
            // 可选：真实编辑（PROBE_EDIT_OLD / PROBE_EDIT_NEW 指定一处替换），验证"改完后保存"的正确性
            string? editOld = Environment.GetEnvironmentVariable("PROBE_EDIT_OLD");
            string? editNew = Environment.GetEnvironmentVariable("PROBE_EDIT_NEW");
            if (saved && !file.IsRawReadOnly && !string.IsNullOrEmpty(editOld) && text.Contains(editOld, StringComparison.Ordinal))
            {
                string edited = text.Replace(editOld, editNew ?? string.Empty);
                bool editedSaved = group.SaveFileText(file, edited);
                Check($"edited SaveFileText({path})", editedSaved);
                byte[] editedBytes = file.Data ?? Array.Empty<byte>();
                byte[] oldUtf16 = Encoding.Unicode.GetBytes(editOld);
                byte[] newUtf16 = Encoding.Unicode.GetBytes(editNew ?? string.Empty);
                int expectedLen = original!.Length - oldUtf16.Length + newUtf16.Length;
                Check($"edited entry length = original - old + new ({path})", editedBytes.Length == expectedLen,
                    $"orig={original.Length} edited={editedBytes.Length} expected={expectedLen}");
                byte[] expected = new byte[expectedLen];
                int head = IndexOf(original, oldUtf16);
                Array.Copy(original, 0, expected, 0, head);
                Array.Copy(newUtf16, 0, expected, head, newUtf16.Length);
                Array.Copy(original, head + oldUtf16.Length, expected, head + newUtf16.Length,
                    original.Length - head - oldUtf16.Length);
                bool minimal = editedBytes.AsSpan().SequenceEqual(expected);
                Check($"edited entry is a single exact substitution ({path})", minimal,
                    minimal ? $"'{editOld}' -> '{editNew}'" : DescribeEntryDiff(expected, editedBytes));
                targets.Add((path, expected));
                continue; // 该条目按编辑后的期望值验收
            }
            targets.Add((path, original!));
        }

        if (targets.Count > 0)
        {
            var result = group.SavePvfPack(editedPvf, isFastMode: true, progress: null!, notButtonClick: true)
                .GetAwaiter().GetResult();
            Check("SavePvfPack", !result.IsError, result.Msg);
            if (!result.IsError && File.Exists(editedPvf))
            {
                var verify = Pvf110.Core.Pvf110Reader.Open(
                    File.ReadAllBytes(Path.Combine(outDir, "sk.dat")), File.ReadAllBytes(editedPvf));
                var outBytes = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < verify.Entries.Count; i++)
                {
                    Pvf110.Core.Pvf110Entry e = verify.Entries[i];
                    outBytes[verify.FilePath(e)] = verify.ReadEntry(e);
                }
                foreach ((string path, byte[] original) in targets)
                {
                    if (!outBytes.TryGetValue(path, out byte[]? back))
                    { Check($"archive entry present ({path})", false); continue; }
                    bool same = back.AsSpan().SequenceEqual(original);
                    Check($"archive 110-token bytes unchanged after no-op save ({path})", same,
                        same ? $"{original.Length}B" : DescribeEntryDiff(original, back));
                }
            }
        }

        Console.WriteLine($"\nRESULT: {(_failures == 0 ? "PASS" : "FAIL")}");
        return _failures == 0 ? 0 : 1;
    }

    /// <summary>比较 110 token 流：按 5 字节记录报告长度、差异记录数与首几处 tag/payload。</summary>
    private static string DescribeEntryDiff(byte[] a, byte[] b)
    {
        int n = Math.Min(a.Length, b.Length);
        int diffBytes = 0, diffRecords = 0, shown = 0;
        var sb = new StringBuilder();
        for (int i = 0; i + 5 <= n; i += 5)
        {
            if (a.AsSpan(i, 5).SequenceEqual(b.AsSpan(i, 5))) continue;
            diffRecords++;
            if (shown < 4)
            {
                sb.Append($" rec#{i / 5} {a[i]:X2}:{BitConverter.ToInt32(a, i + 1)}->{b[i]:X2}:{BitConverter.ToInt32(b, i + 1)};");
                shown++;
            }
        }
        for (int i = 0; i < n; i++) if (a[i] != b[i]) diffBytes++;
        return $"len {a.Length}->{b.Length}, tokenDiffBytes={diffBytes}, records={diffRecords};{sb}";
    }

    private static string Truncate(string s, int n)
        => s.Length <= n ? s : s[..n] + "…";

    /// <summary>字节子串首次出现位置（-1 = 不存在）。</summary>
    private static int IndexOf(byte[] haystack, byte[] needle)
    {
        if (needle.Length == 0 || haystack.Length < needle.Length) return -1;
        for (int i = 0; i + needle.Length <= haystack.Length; i++)
        {
            bool hit = true;
            for (int k = 0; k < needle.Length; k++)
            {
                if (haystack[i + k] != needle[k]) { hit = false; break; }
            }
            if (hit) return i;
        }
        return -1;
    }

    /// <summary>
    /// 经典视图往返扫描：均匀抽样 n 个 type-1 条目，对每条执行「打开（110→经典视图）→
    /// 生成编辑器文本 → 原样回写（经典编译器）」并比对经典视图 token 流。
    /// 判据：编辑器文本未做任何编辑时，回写后的 token 流必须与原流一致（仅容忍尾部 0 填充）。
    /// 该项决定「GUI 保存会不会改内容」的正确性上限，按扩展名汇总失败率。
    /// </summary>
    private static int RunClassicRoundTripSweep(PvfGroup group, int target)
    {
        List<PvfFile> files = group.FileList!.Values.ToList();
        int stride = Math.Max(1, files.Count / Math.Max(1, target));
        var stats = new Dictionary<string, (int total, int lossy, int failed, int readonlyFiles)>(StringComparer.OrdinalIgnoreCase);
        var samples = new List<string>();
        int scanned = 0;

        for (int i = 0; i < files.Count && scanned < target; i += stride)
        {
            PvfFile f = files[i];
            if (f.Pvf110DataType != 1) continue;
            scanned++;
            string ext = Path.GetExtension(f.FileName).TrimStart('.').ToLowerInvariant();
            var cur = stats.TryGetValue(ext, out var v) ? v : (total: 0, lossy: 0, failed: 0, readonlyFiles: 0);
            cur.total++;

            try
            {
                if (!group.EnsureFileData(f.FileName))
                {
                    cur.failed++;
                    if (samples.Count < 20) samples.Add($"{f.FileName}: 打开失败（经典视图不可生成）");
                }
                else if (f.IsRawReadOnly)
                {
                    // 设计内的只读保护：保存按原始字节写回，不再统计为丢失
                    cur.readonlyFiles++;
                }
                else
                {
                    byte[] classic = (byte[])f.Data.Clone();
                    string text = group.GetFileText(f);
                    bool ok = group.SaveFileAsScript(f, text);
                    byte[] after = f.Data;
                    int n = Math.Min(classic.Length, after.Length);
                    bool prefixSame = true;
                    for (int k = 0; k < n; k++)
                        if (classic[k] != after[k]) { prefixSame = false; break; }
                    bool tailZero = after.Length >= classic.Length;
                    for (int k = classic.Length; k < after.Length && tailZero; k++)
                        if (after[k] != 0) tailZero = false;
                    bool lossless = ok && prefixSame && tailZero;
                    if (!lossless)
                    {
                        cur.lossy++;
                        if (samples.Count < 20)
                            samples.Add($"{f.FileName}: {DescribeEntryDiff(classic, after)}");
                    }
                    f.SetLoadedContent(classic); // 复原，避免影响后续判定
                }
            }
            catch (Exception ex)
            {
                cur.failed++;
                if (samples.Count < 20) samples.Add($"{f.FileName}: {ex.GetType().Name}: {ex.Message}");
            }
            stats[ext] = cur;
        }

        Console.WriteLine($"\n# 经典视图往返扫描：抽样 {scanned} 个 type-1 条目（stride={stride}）");
        Console.WriteLine($"# {"ext",-10} {"total",7} {"lossy",7} {"failed",7} {"readonly",9} {"lossy%",8}");
        foreach (var kv in stats.OrderByDescending(k => k.Value.total))
        {
            double pct = kv.Value.total == 0 ? 0 : 100.0 * kv.Value.lossy / kv.Value.total;
            Console.WriteLine($"# {kv.Key,-10} {kv.Value.total,7} {kv.Value.lossy,7} {kv.Value.failed,7} {kv.Value.readonlyFiles,9} {pct,7:F2}%");
        }
        Console.WriteLine($"# 只读保护合计 = {stats.Values.Sum(v => v.readonlyFiles)}");
        Console.WriteLine("\n# 样例（最多 20 条）");
        foreach (string s in samples) Console.WriteLine("  " + s);
        Console.WriteLine($"\nRESULT: {(stats.Values.Sum(v => v.lossy + v.failed) == 0 ? "PASS" : "FAIL")}");
        return 0;
    }

    /// <summary>
    /// Pvf110（115 美服 Builder 包装）服务层回归：与 GUI 完全相同的打开/保存链路。
    /// 重点复现"自动备份"调用形态（progress = null）——该路径曾因保存尾部未加保护的
    /// 进度回调抛 NullReferenceException，进而触发另存为 Script(1).pvf 的回退。
    /// </summary>
    private static int RunPvf110Regression(PvfGroup group)
    {
        Check("entries loaded", group.FileList != null && group.FileList.Count > 1000,
            $"files={group.FileList?.Count}");
        ReportHeap("after-open");

        var setFile = group.GetFile("etc/equipmentpartset.etc");
        Check("partset table file present", setFile != null);
        if (setFile != null)
        {
            Check("EnsureFileData(partset)", group.EnsureFileData(setFile.FileName));
            string setText = group.GetFileText(setFile);
            Check("partset decompile classic view", setText.StartsWith("#PVF_File", StringComparison.Ordinal));
        }

        for (int retry = 0; retry < 120 && group.EquipmentPartSetTable.Items.Count == 0; retry++)
            Thread.Sleep(500);
        Check("equipment part set table loaded", group.EquipmentPartSetTable.Items.Count > 0,
            $"sets={group.EquipmentPartSetTable.Items.Count}");

        string outPvf = Path.Combine(OutDir, "Script.pvf");
        string fallbackPvf = Path.Combine(OutDir, "Script(1).pvf");
        string skOut = Path.Combine(OutDir, "sk.dat");
        foreach (string path in new[] { outPvf, fallbackPvf, skOut })
        {
            if (File.Exists(path)) File.Delete(path);
        }

        // 与自动备份调用完全同形：progress = null（SavePvfPack 内部必须容忍）
        var result = group.SavePvfPack(outPvf, isFastMode: true, progress: null!, notButtonClick: true)
            .GetAwaiter().GetResult();
        Check("SavePvfPack(progress=null) no error", !result.IsError, result.Msg);
        Check("no fallback duplicate Script(1).pvf", !File.Exists(fallbackPvf));
        Check("output written", File.Exists(outPvf) && new FileInfo(outPvf).Length > 100L * 1024 * 1024,
            File.Exists(outPvf) ? $"{new FileInfo(outPvf).Length:N0}B" : "missing");
        Check("sk.dat written beside output", File.Exists(skOut));

        // 未修改保存必须与源归档逐字节一致（增量重建复用整组密文的强校验）
        if (File.Exists(outPvf))
        {
            Check("unmodified save is byte-identical to source", HashFile(outPvf) == HashFile(PvfPath));
        ReportHeap("after-save1");
        }

        // 编辑 → 保存（GUI 编辑器提交路径）也必须走通
        var stk = group.GetFile("stackable/10000001/10000039.stk");
        Check("stk exists", stk != null);
        if (stk != null)
        {
            Check("EnsureFileData(stk)", group.EnsureFileData(stk.FileName));
            string text = group.GetFileText(stk);

            // 富格式文本往返保真：原样回写后经典视图 token 流必须逐字节一致。
            // 这一项专门覆盖 115 独有的 1 token 链接（StringLinkIndex 无伴生项）与行版式。
            byte[] classicBefore = (byte[])stk.Data.Clone();
            Check("rich text starts with #PVF_File", text.StartsWith("#PVF_File", StringComparison.Ordinal));
            Check("SaveFileAsScript(round-trip)", group.SaveFileAsScript(stk, text));
            byte[] classicAfter = stk.Data;
            bool classicSame = classicBefore.AsSpan().SequenceEqual(classicAfter);
            // 编译器会在经典视图尾部补若干 0 填充字节；FromClassicView 只按完整 5 字节记录读取，
            // 故判定为：原长度内逐字节一致，且新增字节全为 0（尾填充）。
            bool prefixSame = classicAfter.Length >= classicBefore.Length
                && classicBefore.AsSpan().SequenceEqual(classicAfter.AsSpan(0, classicBefore.Length));
            bool tailPaddingOnly = true;
            for (int i = classicBefore.Length; i < classicAfter.Length; i++)
            {
                if (classicAfter[i] != 0) { tailPaddingOnly = false; break; }
            }
            Check("classic-view token round-trip lossless", classicSame || (prefixSame && tailPaddingOnly),
                classicSame
                    ? $"{classicBefore.Length}B"
                    : $"len {classicBefore.Length}->{classicAfter.Length}; prefix-identical={prefixSame}; " +
                      $"tail-zero-padding={tailPaddingOnly}; {DescribeTokenDiff(classicBefore, classicAfter)}");
            if (!classicSame)
            {
                File.WriteAllBytes(Path.Combine(OutDir, "classic-before.bin"), classicBefore);
                File.WriteAllBytes(Path.Combine(OutDir, "classic-after.bin"), classicAfter);
            }

            string edited = text.Replace("[grade]\r\n\t1\r\n", "[grade]\r\n\t99\r\n");
            if (edited == text) edited = text.Replace("[grade]\n\t1\n", "[grade]\n\t99\n");
            Check("edit pattern found", edited != text);

            string editedPvf = Path.Combine(OutDir, "ScriptEdited.pvf");
            string editedFallback = Path.Combine(OutDir, "ScriptEdited(1).pvf");
            foreach (string path in new[] { editedPvf, editedFallback })
            {
                if (File.Exists(path)) File.Delete(path);
            }
            Check("SaveFileAsScript", group.SaveFileAsScript(stk, edited));
            var editedResult = group.SavePvfPack(editedPvf, isFastMode: true, progress: null!, notButtonClick: true)
                .GetAwaiter().GetResult();
            Check("edited SavePvfPack(progress=null) no error", !editedResult.IsError, editedResult.Msg);
            Check("no fallback duplicate ScriptEdited(1).pvf", !File.Exists(editedFallback));
            Check("edited output written", File.Exists(editedPvf) && new FileInfo(editedPvf).Length > 100L * 1024 * 1024,
                File.Exists(editedPvf) ? $"{new FileInfo(editedPvf).Length:N0}B" : "missing");
            ReportHeap("after-save2");
        }

        PrintResourceUsage();
        Console.WriteLine($"\nRESULT: {(_failures == 0 ? "PASS" : "FAIL")}");
        return _failures == 0 ? 0 : 1;
    }

    /// <summary>分段堆占用（强制回收后测量）：定位常驻内存集中在哪一步。</summary>
    private static void ReportHeap(string stage)
    {
        if (Environment.GetEnvironmentVariable("MEASURE") != "1") return;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine($"heap[{stage}] = {GC.GetTotalMemory(true) / 1024.0 / 1024.0:F0} MB");
    }

    /// <summary>本次运行的内存水位（对比重建路径的内存占用）。</summary>
    private static void PrintResourceUsage()
    {
        using var p = System.Diagnostics.Process.GetCurrentProcess();
        p.Refresh();
        Console.WriteLine($"resource: peakWorkingSet={p.PeakWorkingSet64 / 1024.0 / 1024.0:F0} MB, " +
                          $"peakCommit={p.PeakPagedMemorySize64 / 1024.0 / 1024.0:F0} MB, " +
                          $"gcHeap={GC.GetTotalMemory(false) / 1024.0 / 1024.0:F0} MB");
    }

    private static string HashFile(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream));
    }

    /// <summary>按 5 字节经典 token 记录报告前几处差异（标签 + payload）。</summary>
    private static string DescribeTokenDiff(byte[] before, byte[] after)
    {
        var sb = new StringBuilder();
        sb.Append($"len {before.Length}->{after.Length};");
        int shown = 0;
        for (int i = 2; i + 5 <= Math.Min(before.Length, after.Length) && shown < 6; i += 5)
        {
            if (!before.AsSpan(i, 5).SequenceEqual(after.AsSpan(i, 5)))
            {
                uint pb = BitConverter.ToUInt32(before, i + 1);
                uint pa = BitConverter.ToUInt32(after, i + 1);
                sb.Append($" rec#{i / 5} tag {before[i]:X2}->{after[i]:X2} payload {pb}->{pa};");
                shown++;
            }
        }
        if (before.Length != after.Length)
        {
            sb.Append(" tail-bytes differ;");
        }
        else if (shown == 0)
        {
            sb.Append(" (diff outside token records?)");
        }
        return sb.ToString();
    }

    private static int FindIndex(Pvf110.Core.NkpiReader reader, string path)
    {
        for (int i = 0; i < reader.Entries.Count; i++)
            if (string.Equals(reader.FilePath(reader.Entries[i]), path, StringComparison.OrdinalIgnoreCase))
                return i;
        return -1;
    }

    /// <summary>等待打开后的后台初始化（套装表/LST 代码表）完成：
    /// 要求 "equipment" 表存在且全表条目数连续多次采样稳定（后台逐 lst 增量写入）。</summary>
    private static void WaitForPostOpenInit(PvfGroup group)
    {
        int stable = 0, last = -1;
        for (int i = 0; i < 240; i++)
        {
            int count = 0;
            lock (group.ListFileTable.CodeDic)
            {
                foreach (var kv in group.ListFileTable.CodeDic) count += kv.Value.Count;
            }
            bool hasEquipment = group.ListFileTable.CodeDic.ContainsKey("equipment");
            if (hasEquipment && count > 0 && count == last)
            {
                if (++stable >= 3) return;
            }
            else
            {
                stable = 0;
            }
            last = count;
            Thread.Sleep(500);
        }
    }
}
