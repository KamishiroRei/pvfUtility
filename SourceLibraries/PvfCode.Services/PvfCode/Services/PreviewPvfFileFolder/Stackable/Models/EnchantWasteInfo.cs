using System.Runtime.CompilerServices;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

public class EnchantWasteInfo
{
	[CompilerGenerated]
	private string? OGPjSr6ysN;

	[CompilerGenerated]
	private string? JrfjbWntYy;

	public string? EquTypes
	{
		[CompilerGenerated]
		get
		{
			return OGPjSr6ysN;
		}
		[CompilerGenerated]
		set
		{
			OGPjSr6ysN = value;
		}
	}

	public string? Text
	{
		[CompilerGenerated]
		get
		{
			return JrfjbWntYy;
		}
		[CompilerGenerated]
		set
		{
			JrfjbWntYy = value;
		}
	}

	public EnchantWasteInfo(PvfFile file, PvfGroup group)
	{
		if (file.GetSectionIntValue("[monster card id]", group, out var val))
		{
			string text = group.ListFileTable.ItemCodeConvertFilePath("stackable", val);
			if (!string.IsNullOrEmpty(text) && group.FileList.TryGetValue(text, out PvfFile value) && value != null && new ServiceStackable(group, value).GetEnchantCardInfo(out EnchantCardInfo card))
			{
				EquTypes = card.EquipPartsText;
				Text = card.EquBlueAttributes;
			}
		}
	}
}
