namespace PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

public class EnchantWasteInfo
{
	public string? EquTypes { get; set; }

	public string? Text { get; set; }

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
