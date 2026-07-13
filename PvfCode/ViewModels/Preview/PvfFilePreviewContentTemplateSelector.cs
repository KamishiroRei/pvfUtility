using System.Windows;
using System.Windows.Controls;
using PvfCode.Models.Pvf.Enums.Stackable;
using PvfCode.Services.PreviewPvfFileFolder;
using PvfCode.Services.PreviewPvfFileFolder.NpcShop;
using PvfCode.Services.PreviewPvfFileFolder.Stackable;

namespace PvfCode.ViewModels.Preview;

public class PvfFilePreviewContentTemplateSelector : DataTemplateSelector
{
	public DataTemplate Equipment { get; set; }

	public DataTemplate StackableDefaultTemplate { get; set; }

	public DataTemplate Stkable_设计图 { get; set; }

	public DataTemplate Stackable_附魔卡片 { get; set; }

	public DataTemplate NpcShop { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item == null)
		{
			return null;
		}
		if (item is FilePreviewData_Equ)
		{
			return Equipment;
		}
		if (item is StackablePreviewBase { StackableType: var stackableType })
		{
			switch (stackableType)
			{
			case StackableType.设计图_杂七杂八_0:
			case StackableType.设计图_装备类_1:
			case StackableType.设计图_药剂类_2:
			case StackableType.设计图_装备类_5:
				return Stkable_设计图;
			case StackableType.材料_附魔卡片_1:
				return Stackable_附魔卡片;
			default:
				return StackableDefaultTemplate;
			}
		}
		if (item is NpcShopPreviewViewModel)
		{
			return NpcShop;
		}
		return base.SelectTemplate(item, container);
	}

	public PvfFilePreviewContentTemplateSelector()
	{
	}
}
