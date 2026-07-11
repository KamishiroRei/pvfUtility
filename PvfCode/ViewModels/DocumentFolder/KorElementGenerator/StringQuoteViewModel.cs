using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass24_0
	{
		public PvfGroup pvf;

		public StringQuoteViewModel lGatlo8OYg;

		public int ljatjw5Pwy;

		public ConcurrentBag<StringQuoteRoot> y4wtTVD0gr;

		public _003C_003Ec__DisplayClass24_0()
		{
		}

		internal void gictDBXAOP(KeyValuePair<string, PvfFile> item)
		{
			int num = item.Value.FindStringViewQuote(pvf.Strtable, lGatlo8OYg.Index, ljatjw5Pwy);
			if (num > 0)
			{
				y4wtTVD0gr.Add(new StringQuoteRoot(item.Key, num));
			}
		}
	}

	private readonly int Index;

	[CompilerGenerated]
	private string nw44pZ6kYQ;

	[CompilerGenerated]
	private string tgp4U6xZnX;

	[CompilerGenerated]
	private int gWS4cq9VJI;

	private readonly SourceType SourceType;

	public string Key
	{
		[CompilerGenerated]
		get
		{
			return nw44pZ6kYQ;
		}
		[CompilerGenerated]
		set
		{
			nw44pZ6kYQ = value;
		}
	}

	public string Text
	{
		[CompilerGenerated]
		get
		{
			return tgp4U6xZnX;
		}
		[CompilerGenerated]
		set
		{
			tgp4U6xZnX = value;
		}
	}

	public int Count
	{
		[CompilerGenerated]
		get
		{
			return gWS4cq9VJI;
		}
		[CompilerGenerated]
		set
		{
			gWS4cq9VJI = value;
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
			await Task.Run((Func<Task?>)bbh40wInlT);
		}
		else
		{
			await Task.Run((Action)gaB47m1cSw);
		}
		IsLoading = false;
	}

	private Task bbh40wInlT()
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

	private void gaB47m1cSw()
	{
		_003C_003Ec__DisplayClass24_0 CS_0024_003C_003E8__locals12 = new _003C_003Ec__DisplayClass24_0();
		CS_0024_003C_003E8__locals12.lGatlo8OYg = this;
		Root = null;
		Root = new ConcurrentObservableCollection<StringQuoteRoot>();
		if (Count == 0)
		{
			return;
		}
		CS_0024_003C_003E8__locals12.pvf = AppCore.ViewModelBase.PVF;
		string strFileName = CS_0024_003C_003E8__locals12.pvf.Strview.Get_pvfstrlist()[Index].StrFileName;
		if (AppCore.ViewModelBase.PVF.GetFile(strFileName) == null)
		{
			return;
		}
		IEnumerable<KeyValuePair<string, PvfFile>> enumerable = CS_0024_003C_003E8__locals12.pvf.FileList.Where<KeyValuePair<string, PvfFile>>((KeyValuePair<string, PvfFile> it) => it.Value.IsScriptFile);
		if (enumerable == null || !enumerable.Any())
		{
			return;
		}
		CS_0024_003C_003E8__locals12.y4wtTVD0gr = new ConcurrentBag<StringQuoteRoot>();
		CS_0024_003C_003E8__locals12.ljatjw5Pwy = CS_0024_003C_003E8__locals12.pvf.Strtable.GetStringTableId(Key);
		Parallel.ForEach(enumerable, delegate(KeyValuePair<string, PvfFile> item)
		{
			int num = item.Value.FindStringViewQuote(CS_0024_003C_003E8__locals12.pvf.Strtable, CS_0024_003C_003E8__locals12.lGatlo8OYg.Index, CS_0024_003C_003E8__locals12.ljatjw5Pwy);
			if (num > 0)
			{
				CS_0024_003C_003E8__locals12.y4wtTVD0gr.Add(new StringQuoteRoot(item.Key, num));
			}
		});
		Root.AddRange(CS_0024_003C_003E8__locals12.y4wtTVD0gr);
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

	[CompilerGenerated]
	private bool JRB4X4e6Vi(int P_0)
	{
		return P_0 == Index;
	}
}
