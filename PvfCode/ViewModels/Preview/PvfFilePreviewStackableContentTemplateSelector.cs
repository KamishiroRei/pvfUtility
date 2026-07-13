using System.Windows;
using System.Windows.Controls;
using PvfCode.Models.Pvf.Enums.Stackable;
using PvfCode.Services.PreviewPvfFileFolder.Stackable;

namespace PvfCode.ViewModels.Preview;

public class PvfFilePreviewStackableContentTemplateSelector : DataTemplateSelector
{
	public DataTemplate 礼包 { get; set; }

	public DataTemplate Stackable_附魔宝珠 { get; set; }

	public DataTemplate Stackable_时装礼包可选 { get; set; }

	public DataTemplate NullControl { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item == null)
		{
			return NullControl;
		}
		if (item is StackablePreviewBase { StackableType: var stackableType })
		{
			switch (stackableType)
			{
			case StackableType.消耗品_可选盒子_0:
			case StackableType.消耗品_礼包_0:
			case StackableType.消耗品_礼包_1:
			case StackableType.消耗品_点券盒子_0:
			case StackableType.消耗品_点券盒子_1:
				return 礼包;
			case StackableType.消耗品_附魔宝珠_0:
				return Stackable_附魔宝珠;
			case StackableType.消耗品_点卷礼包_0:
			case StackableType.消耗品_可选属性的时装礼包_0:
				return Stackable_时装礼包可选;
			default:
				return NullControl;
			}
		}
		return base.SelectTemplate(item, container);
	}

	public PvfFilePreviewStackableContentTemplateSelector()
	{
	}
}
