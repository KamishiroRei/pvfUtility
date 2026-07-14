using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using Swordfish.NET.Collections;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder.KorElementGenerator;

public class StringQuoteViewModel : ViewModelBase
{
	private readonly int Index;

	private readonly SourceType SourceType;

	public string Key { get; set; }

	public string Text { get; set; }

	public int Count { get; set; }

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

	public ConcurrentObservableCollection<StringQuoteRoot> Root
	{
		get
		{
			return GetProperty(() => Root);
		}
		set
		{
			SetProperty<ConcurrentObservableCollection<StringQuoteRoot>>(() => Root, value);
		}
	}

	~StringQuoteViewModel()
	{
		Root = null;
	}

	public StringQuoteViewModel(int index, string key, SourceType sourceType)
	{
		SourceType = sourceType;
		Index = index;
		Key = key;
		if (SourceType == SourceType.StringView)
		{
			Count = AppCore.ViewModelBase.PVF.Strview.GetQuoteCount(index, key);
		}
		else
		{
			Count = AppCore.ViewModelBase.PVF.Strtable.GetQuoteCount(index);
		}
		Text = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("StringQuote_ButtonTitle"), Count);
		Root = new ConcurrentObservableCollection<StringQuoteRoot>();
	}

	[Command]
	public async void Opened()
	{
		IsLoading = true;
		if (SourceType != SourceType.StringView)
		{
			await Task.Run(LoadStringTableQuotesAsync);
		}
		else
		{
			await Task.Run(LoadStringViewQuotes);
		}
		IsLoading = false;
	}

	private Task LoadStringTableQuotesAsync()
	{
		lock (this)
		{
			Root = null;
			Root = new ConcurrentObservableCollection<StringQuoteRoot>();
			if (Count == 0)
			{
				return Task.CompletedTask;
			}
			IEnumerable<KeyValuePair<string, PvfFile>> enumerable = from it in AppCore.ViewModelBase.PVF.FileList
				where it.Value.IsScriptFile
				where it.Value.IsScriptFile
				select it;
			if (enumerable == null)
			{
				return Task.CompletedTask;
			}
			List<StringQuoteRoot> list = new List<StringQuoteRoot>();
			foreach (KeyValuePair<string, PvfFile> item in enumerable)
			{
				PooledList<int> stringDatas = item.Value.GetStringDatas();
				if (stringDatas == null || stringDatas.Count == 0)
				{
					stringDatas?.Dispose();
					continue;
				}
				int num = stringDatas.Count((int P_0) => P_0 == Index);
				if (num > 0)
				{
					list.Add(new StringQuoteRoot(item.Key, num));
				}
				stringDatas.Dispose();
			}
			Root.AddRange(list);
			return Task.CompletedTask;
		}
	}

	private void LoadStringViewQuotes()
	{
		Root = null;
		Root = new ConcurrentObservableCollection<StringQuoteRoot>();
		if (Count == 0)
		{
			return;
		}
		PvfGroup pvf = AppCore.ViewModelBase.PVF;
		string strFileName = pvf.Strview.Get_pvfstrlist()[Index].StrFileName;
		if (AppCore.ViewModelBase.PVF.GetFile(strFileName) == null)
		{
			return;
		}
		IEnumerable<KeyValuePair<string, PvfFile>> enumerable = pvf.FileList.Where((KeyValuePair<string, PvfFile> it) => it.Value.IsScriptFile);
		if (enumerable == null || !enumerable.Any())
		{
			return;
		}
		ConcurrentBag<StringQuoteRoot> results = new ConcurrentBag<StringQuoteRoot>();
		int stringTableId = pvf.Strtable.GetStringTableId(Key);
		Parallel.ForEach(enumerable, item =>
		{
			int num = item.Value.FindStringViewQuote(pvf.Strtable, Index, stringTableId);
			if (num > 0)
			{
				results.Add(new StringQuoteRoot(item.Key, num));
			}
		});
		Root.AddRange(results);
	}

	[Command]
	public void NodeDoubleClick(RowClickArgs nodeClickArgs)
	{
		if (nodeClickArgs.Item != null)
		{
			object item = nodeClickArgs.Item;
			if (item is StringQuoteRoot)
			{
				StringQuoteRoot stringQuoteRoot = (StringQuoteRoot)item;
				AppCore.ViewModelBase.RootDocument.AddDocument(stringQuoteRoot.FileName, gotoNode: true);
			}
		}
	}

	[Command]
	public void Unloaded()
	{
		Root = null;
		Text = null;
	}
}
