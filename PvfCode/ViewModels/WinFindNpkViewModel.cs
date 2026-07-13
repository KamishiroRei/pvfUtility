using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.NPK.Utils.Models;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels;

public class WinFindNpkViewModel : ViewModelBase
{
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

	public ObservableCollection<NpkFindResult> SelectedItems { get; set; }

	public ConcurrentObservableCollection<NpkFindResult> Items { get; set; }

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
		IEnumerable<KeyValuePair<string, UtImgFile>> matches = ImagePack2Service.Instance.NpkImgDIC.Where((KeyValuePair<string, UtImgFile> item) => item.Key.Contains(Keyword));
		if (matches == null)
		{
			return;
		}
		List<NpkFindResult> results = new List<NpkFindResult>();
		foreach (KeyValuePair<string, UtImgFile> item in matches)
		{
			results.Add(new NpkFindResult(item.Value, item.Key));
		}
		Items.AddRange(results);
	}
}
