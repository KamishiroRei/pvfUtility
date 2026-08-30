namespace Pvf110.Core;

/// <summary>Pvf110 支持辅助：检测与打开。</summary>
public static class Pvf110Support
{
    /// <summary>返回 pvf 同目录下配套的 sk.dat 路径；不存在返回 null。</summary>
    public static string? FindSkDat(string pvfPath)
    {
        string? dir = Path.GetDirectoryName(pvfPath);
        if (string.IsNullOrEmpty(dir)) return null;
        string sk = Path.Combine(dir, "sk.dat");
        return File.Exists(sk) ? sk : null;
    }
}
