using PvfCode.Models.Pvf;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class EquGroupItemAttributes
{
	public int ItemCode { get; set; }

	public bool IsRoot { get; set; }

	public string Name { get; set; }

	public PvfFile File { get; set; }

	public EquGroupItemAttributes(PvfFile file, bool isRoot, PvfPack pvf, int itemCode)
	{
		ItemCode = itemCode;
		File = file;
		IsRoot = isRoot;
		if (file == null)
		{
			Name = "源文件不存在";
			return;
		}
		Name = pvf.GetItemName(file);
		if (string.IsNullOrEmpty(Name))
		{
			Name = "未设定 [name]";
		}
	}
}
