namespace Pvf110.Core;

/// <summary>Pvf110 支持辅助：检测与打开。</summary>
public static class Pvf110Support
{
    /// <summary>PVF 目录与其上溯层数（配套件与 PVF 不总在同一层）。与 <see cref="Pvf110ClientKeys.FindClientExe"/> 保持一致。</summary>
    private const int MaxParentLevels = 3;

    /// <summary>
    /// 返回配套的 sk.dat 路径：先查 PVF 同目录，再逐级上溯至 <see cref="MaxParentLevels"/> 层；
    /// 不存在返回 null。sk.dat 是 Pvf110 容器专有的外层密钥文件，出现在 PVF 的任一祖先目录
    /// 都视为同一套容器配套件；不依赖固定盘符。
    /// </summary>
    public static string? FindSkDat(string pvfPath) => FindInAncestors(pvfPath, "sk.dat");

    /// <summary>在 PVF 同目录及其上溯层内查找指定文件名。</summary>
    public static string? FindInAncestors(string pvfPath, string fileName)
    {
        string? dir = Path.GetDirectoryName(Path.GetFullPath(pvfPath));
        for (int level = 0; level <= MaxParentLevels && !string.IsNullOrEmpty(dir); level++)
        {
            string candidate = Path.Combine(dir, fileName);
            if (File.Exists(candidate)) return candidate;
            dir = Path.GetDirectoryName(dir);
        }
        return null;
    }

    /// <summary>
    /// 取 sk.dat 字节，通用回退链：
    /// 显式路径（<c>PVF_SKDAT</c>）→ 文件探测（PVF 同目录/上溯 3 层）→ 密钥材料注册表
    /// （外部材料文件条目 → 内置条目，见 <see cref="Pvf110KeyMaterial"/>）。
    /// 因此固定版单机客户端打开 <c>Script.pvf</c> 既不需要 sk.dat 文件，也不需要客户端 EXE；
    /// 换客户端版本时把新 sk.dat 落到同目录，或把新条目写进材料文件即可，无需改代码。
    /// </summary>
    /// <returns>字节、来源标签（<c>explicit</c>/<c>file</c>/<c>material</c>/<c>embedded</c>）与来源文件路径（无文件时为 null）。</returns>
    public static (byte[] Bytes, string Source, string? Path) LoadSkDat(string pvfPath, string? explicitSkDatPath = null)
    {
        if (!string.IsNullOrWhiteSpace(explicitSkDatPath))
        {
            if (!File.Exists(explicitSkDatPath))
                throw new FileNotFoundException("PVF_SKDAT 指向的文件不存在", explicitSkDatPath);
            return (File.ReadAllBytes(explicitSkDatPath), "explicit", Path.GetFullPath(explicitSkDatPath));
        }
        string? found = FindSkDat(pvfPath);
        if (found != null) return (File.ReadAllBytes(found), "file", found);
        if (Pvf110KeyMaterial.FindSkDat(pvfPath) is (byte[] bytes, string source))
            return (bytes, source, Pvf110KeyMaterial.ResolveMaterialFile(pvfPath));
        throw new InvalidDataException(
            "既没有 sk.dat 文件（PVF_SKDAT / 同目录 / 上溯 3 层），密钥材料注册表里也没有携带 sk.dat 的条目。" +
            $"换客户端版本请把 sk.dat 放到 PVF 同目录，或用 keyset-probe --write {Pvf110KeyMaterial.MaterialFileName} 写入新材料条目。");
    }
}
