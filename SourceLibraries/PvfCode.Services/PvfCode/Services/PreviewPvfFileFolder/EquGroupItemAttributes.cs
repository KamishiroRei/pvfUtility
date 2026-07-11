using System.Runtime.CompilerServices;
using PvfCode.Models.Pvf;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class EquGroupItemAttributes
{
	[CompilerGenerated]
	private int eALjcs0LjJ;

	[CompilerGenerated]
	private bool DREjsutY8Y;

	[CompilerGenerated]
	private string X1sj5k9u9F;

	[CompilerGenerated]
	private PvfFile OIJjGCZUnI;

	public int ItemCode
	{
		[CompilerGenerated]
		get
		{
			return eALjcs0LjJ;
		}
		[CompilerGenerated]
		set
		{
			eALjcs0LjJ = value;
		}
	}

	public bool IsRoot
	{
		[CompilerGenerated]
		get
		{
			return DREjsutY8Y;
		}
		[CompilerGenerated]
		set
		{
			DREjsutY8Y = value;
		}
	}

	public string Name
	{
		[CompilerGenerated]
		get
		{
			return X1sj5k9u9F;
		}
		[CompilerGenerated]
		set
		{
			X1sj5k9u9F = value;
		}
	}

	public PvfFile File
	{
		[CompilerGenerated]
		get
		{
			return OIJjGCZUnI;
		}
		[CompilerGenerated]
		set
		{
			OIJjGCZUnI = value;
		}
	}

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
