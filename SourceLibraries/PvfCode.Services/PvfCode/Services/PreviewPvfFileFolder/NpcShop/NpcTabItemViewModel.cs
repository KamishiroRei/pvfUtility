using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using Swordfish.NET.Collections;

namespace PvfCode.Services.PreviewPvfFileFolder.NpcShop;

public class NpcTabItemViewModel : ViewModelBase
{
	[CompilerGenerated]
	private string RPAMMmQ4DO;

	[CompilerGenerated]
	private ConcurrentObservableCollection<NpcShopPreviewItem> PNFMO4W69b;

	[CompilerGenerated]
	private NpcShopPreviewItem lj7M0qNBFk;

	public string Title
	{
		[CompilerGenerated]
		get
		{
			return RPAMMmQ4DO;
		}
		[CompilerGenerated]
		set
		{
			RPAMMmQ4DO = value;
		}
	}

	public ConcurrentObservableCollection<NpcShopPreviewItem> Items
	{
		[CompilerGenerated]
		get
		{
			return PNFMO4W69b;
		}
		[CompilerGenerated]
		set
		{
			PNFMO4W69b = value;
		}
	}

	public NpcShopPreviewItem CurrentItem
	{
		[CompilerGenerated]
		get
		{
			return lj7M0qNBFk;
		}
		[CompilerGenerated]
		set
		{
			lj7M0qNBFk = value;
		}
	}

	public bool IsSelected
	{
		get
		{
			return GetProperty(() => IsSelected);
		}
		set
		{
			SetProperty(() => IsSelected, value);
		}
	}

	public NpcTabItemViewModel(string title, ConcurrentObservableCollection<NpcShopPreviewItem> items)
	{
		Title = title;
		Items = items;
		if (Items == null)
		{
			Items = new ConcurrentObservableCollection<NpcShopPreviewItem>();
		}
	}
}
