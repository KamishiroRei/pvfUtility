using System;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;

namespace PvfCode.ViewModels.DocumentFolder.VerticalScrollBarHighlighted;

public class VerticalScrollBarHighlightedMagager : ViewModelBase, IDisposable
{
	private ObservableCollection<VerticalScrollBarHighlightedData> items;

	public ObservableCollection<VerticalScrollBarHighlightedData> Items
	{
		get
		{
			return items;
		}
		set
		{
			items = value;
			RaisePropertyChanged("Items");
		}
	}

	public VerticalScrollBarHighlightedMagager()
	{
		Items = new ObservableCollection<VerticalScrollBarHighlightedData>();
	}

	public void RefreshSelectedRow(double position)
	{
		VerticalScrollBarHighlightedData verticalScrollBarHighlightedData = Items?.FirstOrDefault((VerticalScrollBarHighlightedData it) => it.Type == VerticalScrollBarHighlightedType.当前选中行);
		if (verticalScrollBarHighlightedData != null)
		{
			Items.Remove(verticalScrollBarHighlightedData);
		}
		Items?.Add(new VerticalScrollBarHighlightedData(position, 3, VerticalScrollBarHighlightedType.当前选中行));
	}

	public void Remove(VerticalScrollBarHighlightedType key)
	{
		VerticalScrollBarHighlightedData[] matches = Items.Where(item => item.Type == key).ToArray();
		foreach (VerticalScrollBarHighlightedData item in matches)
		{
			Items.Remove(item);
		}
	}

	public void Dispose()
	{
		Items = null;
	}
}
