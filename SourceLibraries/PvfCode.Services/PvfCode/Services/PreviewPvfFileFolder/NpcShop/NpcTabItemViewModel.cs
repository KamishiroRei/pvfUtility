using DevExpress.Mvvm;
using Swordfish.NET.Collections;

namespace PvfCode.Services.PreviewPvfFileFolder.NpcShop;

public class NpcTabItemViewModel : ViewModelBase
{
	public string Title { get; set; }

	public ConcurrentObservableCollection<NpcShopPreviewItem> Items { get; set; }

	public NpcShopPreviewItem CurrentItem { get; set; }

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
