using System.IO;

namespace PvfCode.Models.Pvf.ImportModels;

public class ImportFileItem
{
	public readonly string DirPath;

	private readonly int _directoryPrefixLength;

	public readonly string FullPath;

	public int? IndexForm7zip { get; set; }

	public string FilePath => FullPath.Remove(0, _directoryPrefixLength);

	public string TreeFullPath { get; set; }

	public string Extension => Path.GetExtension(TreeFullPath);

	public ImportFileItem(string fullPath, string dirPath, int? indexForm7zip = null)
	{
		FullPath = fullPath;
		DirPath = dirPath;
		_directoryPrefixLength = DirPath.Length;
		IndexForm7zip = indexForm7zip;
	}
}
