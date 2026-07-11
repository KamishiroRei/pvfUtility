using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.NPK.Utils.Models;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels;

public class WinFindNpkViewModel : ViewModelBase
{
	[CompilerGenerated]
	private ObservableCollection<NpkFindResult> LbhFbWBUWl;

	[CompilerGenerated]
	private ConcurrentObservableCollection<NpkFindResult> mGZFIEaUBQ;

	public string Keyword
	{
		get
		{
			return GetProperty(() => Keyword);
		}
		set
		{
			SetProperty<string>(() => Keyword, value);
		}
	}

	public bool IsLoading
	{
		get
		{
			return GetProperty(() => IsLoading);
		}
		set
		{
			SetProperty(() => IsLoading, value);
		}
	}

	public NpkFindResult SelectedItem
	{
		get
		{
			return GetProperty(() => SelectedItem);
		}
		set
		{
			SetProperty<NpkFindResult>(() => SelectedItem, value);
		}
	}

	public NpkFindResult SelectedItem2
	{
		get
		{
			return GetProperty(() => SelectedItem2);
		}
		set
		{
			SetProperty<NpkFindResult>(() => SelectedItem2, value);
		}
	}

	public ObservableCollection<NpkFindResult> SelectedItems
	{
		[CompilerGenerated]
		get
		{
			return LbhFbWBUWl;
		}
		[CompilerGenerated]
		set
		{
			LbhFbWBUWl = value;
		}
	}

	public ConcurrentObservableCollection<NpkFindResult> Items
	{
		[CompilerGenerated]
		get
		{
			return mGZFIEaUBQ;
		}
		[CompilerGenerated]
		set
		{
			mGZFIEaUBQ = value;
		}
	}

	public WinFindNpkViewModel()
	{
		SelectedItems = new ObservableCollection<NpkFindResult>();
		Items = new ConcurrentObservableCollection<NpkFindResult>();
	}

	[Command]
	public void OnFind()
	{
		if (string.IsNullOrEmpty(Keyword))
		{
			return;
		}
		IEnumerable<KeyValuePair<string, UtImgFile>> enumerable = ImagePack2Service.Instance.NpkImgDIC.Where<KeyValuePair<string, UtImgFile>>((KeyValuePair<string, UtImgFile> P_0) => P_0.Key.Contains(Keyword));
		if (enumerable == null)
		{
			return;
		}
		List<NpkFindResult> list = new List<NpkFindResult>();
		foreach (KeyValuePair<string, UtImgFile> item in enumerable)
		{
			list.Add(new NpkFindResult(item.Value, item.Key));
		}
		Items.AddRange(list);
	}

	[CompilerGenerated]
	private bool JL9FtCLr0V(KeyValuePair<string, UtImgFile> P_0)
	{
		return P_0.Key.Contains(Keyword);
	}
}
