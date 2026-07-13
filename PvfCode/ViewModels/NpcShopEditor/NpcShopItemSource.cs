namespace PvfCode.ViewModels.NpcShopEditor;

public class NpcShopItemSource : NpcShopItem
{
	public override int ItemCode
	{
		get
		{
			if (File.ItemCode.HasValue)
			{
				return File.ItemCode.Value;
			}
			return -1;
		}
	}

	public override PvfFile? File { get; }

	public NpcShopItemSource(PvfFile file)
	{
		File = file;
		InvalidatePurchaseData();
	}
}
