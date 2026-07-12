using System.Runtime.CompilerServices;

namespace PvfCode.ViewModels.NpcShopEditor;

public class NpcShopItemSource : NpcShopItem
{
	[CompilerGenerated]
	private readonly PvfFile? HXZmnBr19e;

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

	public override PvfFile? File
	{
		[CompilerGenerated]
		get
		{
			return HXZmnBr19e;
		}
	}

	public NpcShopItemSource(PvfFile file)
	{
		HXZmnBr19e = file;
		InvalidatePurchaseData();
	}
}
