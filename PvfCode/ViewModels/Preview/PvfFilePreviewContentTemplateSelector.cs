using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PvfCode.Models.Pvf.Enums.Stackable;
using PvfCode.Services.PreviewPvfFileFolder;
using PvfCode.Services.PreviewPvfFileFolder.NpcShop;
using PvfCode.Services.PreviewPvfFileFolder.Stackable;

namespace PvfCode.ViewModels.Preview;

public class PvfFilePreviewContentTemplateSelector : DataTemplateSelector
{
	[CompilerGenerated]
	private DataTemplate aPgmfndjOt;

	[CompilerGenerated]
	private DataTemplate n0Tm5K5cUt;

	[CompilerGenerated]
	private DataTemplate Pa5mS7g9dF;

	[CompilerGenerated]
	private DataTemplate AEHmAipYaR;

	[CompilerGenerated]
	private DataTemplate AACm4DOp0u;

	public DataTemplate Equipment
	{
		[CompilerGenerated]
		get
		{
			return aPgmfndjOt;
		}
		[CompilerGenerated]
		set
		{
			aPgmfndjOt = value;
		}
	}

	public DataTemplate StackableDefaultTemplate
	{
		[CompilerGenerated]
		get
		{
			return n0Tm5K5cUt;
		}
		[CompilerGenerated]
		set
		{
			n0Tm5K5cUt = value;
		}
	}

	public DataTemplate Stkable_设计图
	{
		[CompilerGenerated]
		get
		{
			return Pa5mS7g9dF;
		}
		[CompilerGenerated]
		set
		{
			Pa5mS7g9dF = value;
		}
	}

	public DataTemplate Stackable_附魔卡片
	{
		[CompilerGenerated]
		get
		{
			return AEHmAipYaR;
		}
		[CompilerGenerated]
		set
		{
			AEHmAipYaR = value;
		}
	}

	public DataTemplate NpcShop
	{
		[CompilerGenerated]
		get
		{
			return AACm4DOp0u;
		}
		[CompilerGenerated]
		set
		{
			AACm4DOp0u = value;
		}
	}

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
