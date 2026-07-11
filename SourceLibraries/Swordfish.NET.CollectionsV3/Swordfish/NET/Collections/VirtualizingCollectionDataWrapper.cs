using System.ComponentModel;

namespace Swordfish.NET.Collections;

public class VirtualizingCollectionDataWrapper<T> : INotifyPropertyChanged where T : class
{
	private int index;

	private T data;

	public int Index => index;

	public int ItemNumber => index + 1;

	public bool IsLoading => Data == null;

	public T Data
	{
		get
		{
			return data;
		}
		internal set
		{
			data = value;
			OnPropertyChanged("Data");
			OnPropertyChanged("IsLoading");
		}
	}

	public bool IsInUse => this.PropertyChanged != null;

	public event PropertyChangedEventHandler PropertyChanged;

	public VirtualizingCollectionDataWrapper(int index)
	{
		this.index = index;
	}

	private void OnPropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
