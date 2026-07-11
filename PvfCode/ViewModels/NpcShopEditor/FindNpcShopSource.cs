using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PvfCode.ViewModels.NpcShopEditor;

public class FindNpcShopSource
{
	[CompilerGenerated]
	private PvfFile kGDmL3hSDp;

	public PvfFile File
	{
		[CompilerGenerated]
		get
		{
			return kGDmL3hSDp;
		}
		[CompilerGenerated]
		set
		{
			kGDmL3hSDp = value;
		}
	}

	public string? NpcName
	{
		get
		{
			PvfFile file = File;
			PvfGroup pVF = AppCore.ViewModelBase.PVF;
			if (file.GetNpcId(pVF, out var npcId))
			{
				PvfFile pvfFile = pVF.ListFileTable.ItemCodeConvertPvfFile(pVF, npcId, new string[1] { "npc" });
				if (pvfFile == null)
				{
					return string.Empty;
				}
				if (pvfFile == null || !pvfFile.GetNameText(AppCore.ViewModelBase.PVF, "[name]", out string name))
				{
					return null;
				}
				return name;
			}
			return string.Empty;
		}
	}

	public string? NpcId
	{
		get
		{
			PvfFile file = File;
			if (file == null || !file.GetNpcId(AppCore.ViewModelBase.PVF, out var npcId))
			{
				return null;
			}
			return npcId.ToString();
		}
	}

	public FindNpcShopSource(string filePath)
	{
		File = AppCore.ViewModelBase.PVF.GetFile(filePath);
	}

	public bool Find(string key)
	{
		if (File != null && File.FileName.Contains(key))
		{
			return true;
		}
		if (NpcName != null && NpcName.Contains(key))
		{
			return true;
		}
		if (File != null && File.ItemCode.HasValue && File.ItemCode.Value.ToString().Contains(key))
		{
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 3);
		defaultInterpolatedStringHandler.AppendLiteral("[");
		defaultInterpolatedStringHandler.AppendFormatted(NpcName);
		defaultInterpolatedStringHandler.AppendLiteral(",<");
		defaultInterpolatedStringHandler.AppendFormatted(File?.ItemCode);
		defaultInterpolatedStringHandler.AppendLiteral(">,");
		defaultInterpolatedStringHandler.AppendFormatted(File?.ShortName);
		defaultInterpolatedStringHandler.AppendLiteral("]");
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}

	public static List<FindNpcShopSource> Create()
	{
		IEnumerable<string> files = AppCore.ViewModelBase.PVF.GetFiles("itemshop", PvfFileType.shp);
		if (files == null || !files.Any())
		{
			return new List<FindNpcShopSource>();
		}
		List<FindNpcShopSource> list = new List<FindNpcShopSource>();
		foreach (string item in files)
		{
			list.Add(new FindNpcShopSource(item));
		}
		return list;
	}
}
