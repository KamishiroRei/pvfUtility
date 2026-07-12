using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PvfCode.Models.Pvf.Enums.Stackable;
using PvfCode.Services.PreviewPvfFileFolder.Stackable;

namespace PvfCode.ViewModels.Preview;

public class PvfFilePreviewStackableContentTemplateSelector : DataTemplateSelector
{
	[CompilerGenerated]
	private DataTemplate YP7mY8MjUx;

	[CompilerGenerated]
	private DataTemplate HTfmyWWvG8;

	[CompilerGenerated]
	private DataTemplate ajYmiXU1Za;

	[CompilerGenerated]
	private DataTemplate lrkmuVq7jo;

	public DataTemplate 礼包
	{
		[CompilerGenerated]
		get
		{
			return YP7mY8MjUx;
		}
		[CompilerGenerated]
		set
		{
			YP7mY8MjUx = value;
		}
	}

	public DataTemplate Stackable_附魔宝珠
	{
		[CompilerGenerated]
		get
		{
			return HTfmyWWvG8;
		}
		[CompilerGenerated]
		set
		{
			HTfmyWWvG8 = value;
		}
	}

	public DataTemplate Stackable_时装礼包可选
	{
		[CompilerGenerated]
		get
		{
			return ajYmiXU1Za;
		}
		[CompilerGenerated]
		set
		{
			ajYmiXU1Za = value;
		}
	}

	public DataTemplate NullControl
	{
		[CompilerGenerated]
		get
		{
			return lrkmuVq7jo;
		}
		[CompilerGenerated]
		set
		{
			lrkmuVq7jo = value;
		}
	}

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
