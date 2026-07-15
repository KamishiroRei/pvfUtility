namespace PvfCode.Models.Pvf;

public class LstItem
{
	public readonly int ItemCode;

	public readonly string ItemPath;

	public readonly string Header;

	public string FullPath
	{
		get
		{
			string text = ItemPath;
			if (!string.IsNullOrEmpty(text) && text[0] == '/')
			{
				text = text.Remove(0, 1);
			}
			return (Header + "/" + text).ToLower().Replace("\\", "/");
		}
	}

	public string GetFullPath(PvfPack pvf, PvfFile lstFile)
	{
		pvf.GetLstFullPath(lstFile, ItemPath, out string filePath);
		return filePath;
	}

	public string ToLstRow()
	{
		return $"{ItemCode}\t`{ItemPath}`";
	}

	public LstItem(string header, string itemPath, int itemCode)
	{
		ItemCode = itemCode;
		ItemPath = itemPath;
		Header = header;
	}
}
