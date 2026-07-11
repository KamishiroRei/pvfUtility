using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Grid;
using PvfCode.Dot;
using Utools;

namespace PvfCode.ViewModels.TreeFolder;

public class ViewReNmaeNodesViewMode : ViewModelBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass29_0
	{
		public TreeListControl tree;

		public _003C_003Ec__DisplayClass29_0()
		{
		}

		internal void O3Hnza4d5v()
		{
			tree.View.ExpandAllNodes();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass40_0
	{
		public ViewReNmaeNodesViewMode a52qlpNwhM;

		public char[] lMcqjD8Tth;

		public _003C_003Ec__DisplayClass40_0()
		{
		}

		internal void vvSqDB77eb(string fullPath, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = a52qlpNwhM.Trees;
			string[] array = fullPath.Split(lMcqjD8Tth);
			short num = 0;
			int num2 = array.Length - 1;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				lock (a52qlpNwhM)
				{
					if (num == 0)
					{
						stringBuilder.Append(text);
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text);
					}
					if (observableConcurrentDictionaryEx.TryGetValue(text, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						PvfTreeFileRename pvfTreeFileRename = new PvfTreeFileRename(a52qlpNwhM.Pvf, stringBuilder.ToString(), text, num == num2, num);
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFileRename);
						observableConcurrentDictionaryEx = pvfTreeFileRename.Children;
					}
				}
				num++;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass41_0
	{
		public string CyLqCaP9Dt;

		public _003C_003Ec__DisplayClass41_0()
		{
		}

		internal bool XwZqT7tukI(KeyValuePair<string, PvfTreeFileBase> it)
		{
			return it.Key == CyLqCaP9Dt;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass42_0
	{
		public string FvIqh0uhaL;

		public _003C_003Ec__DisplayClass42_0()
		{
		}

		internal bool HdRqHitxCr(KeyValuePair<string, PvfTreeFileBase> it)
		{
			return it.Key == FvIqh0uhaL;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass43_0
	{
		public IEnumerable<KeyValuePair<string, PvfTreeFileBase>> FQ5qB7W0sg;

		public ViewReNmaeNodesViewMode sD3qFll5rA;

		public _003C_003Ec__DisplayClass43_0()
		{
		}

		internal void JAIqvHRBQ6(ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> trees)
		{
			foreach (KeyValuePair<string, PvfTreeFileBase> item in FQ5qB7W0sg)
			{
				string fullPath = item.Value.FullPath;
				if (fullPath.IndexOf("/") < 0)
				{
					trees.RemoveTry(item.Key);
					continue;
				}
				ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = trees;
				string[] array = fullPath.Split(sD3qFll5rA.QbnrN34bwm, StringSplitOptions.RemoveEmptyEntries);
				PvfTreeFileBase value = null;
				for (int i = 0; i < array.Length - 1; i++)
				{
					string key = array[i];
					if (observableConcurrentDictionaryEx.TryGetValue(key, out value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
				}
				observableConcurrentDictionaryEx.RemoveTry(array[^1]);
			}
			trees.NotifyObserversOfChange();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass44_0
	{
		public KeyValuePair<string, PvfTreeFileBase> F18qWPKU24;

		public ViewReNmaeNodesViewMode IBeqmfPbBu;

		public _003C_003Ec__DisplayClass44_0()
		{
		}

		internal void w10qrZPSYQ(ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> trees)
		{
			if (trees == null)
			{
				return;
			}
			string fullPath = F18qWPKU24.Value.FullPath;
			if (fullPath.IndexOf("/") < 0)
			{
				trees.RemoveTry(F18qWPKU24.Key);
				return;
			}
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = trees;
			string[] array = fullPath.Split(IBeqmfPbBu.QbnrN34bwm, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length - 1; i++)
			{
				string key = array[i];
				if (observableConcurrentDictionaryEx.TryGetValue(key, out var value))
				{
					observableConcurrentDictionaryEx = value.Children;
				}
			}
			observableConcurrentDictionaryEx.RemoveTry(array[^1]);
		}
	}

	[CompilerGenerated]
	private PVfTreeChildrenSelector S6krVJ71X5;

	[CompilerGenerated]
	private ObservableCollection<KeyValuePair<string, PvfTreeFileBase>> FZAr3pn25F;

	private PooledList<string> El4rRcvveP;

	private readonly char[] QbnrN34bwm;

	public bool IsLoaded
	{
		get
		{
			return GetProperty(() => IsLoaded);
		}
		set
		{
			SetProperty(() => IsLoaded, value);
		}
	}

	public PVfTreeChildrenSelector ChildNodesSelector
	{
		[CompilerGenerated]
		get
		{
			return S6krVJ71X5;
		}
		[CompilerGenerated]
		set
		{
			S6krVJ71X5 = value;
		}
	}

	public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> Trees
	{
		get
		{
			return GetProperty(() => Trees);
		}
		set
		{
			SetProperty<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>>(() => Trees, value);
		}
	}

	public KeyValuePair<string, PvfTreeFileBase>? CurrentItem
	{
		get
		{
			return GetProperty(() => CurrentItem);
		}
		set
		{
			SetProperty<KeyValuePair<string, PvfTreeFileBase>?>(() => CurrentItem, value);
		}
	}

	public KeyValuePair<string, PvfTreeFileBase>? FocusedRow
	{
		get
		{
			return GetProperty(() => FocusedRow);
		}
		set
		{
			SetProperty<KeyValuePair<string, PvfTreeFileBase>?>(() => FocusedRow, value);
		}
	}

	public bool ReNameApplyTolstFile
	{
		get
		{
			return GetProperty(() => ReNameApplyTolstFile);
		}
		set
		{
			SetProperty(() => ReNameApplyTolstFile, value);
		}
	}

	public ObservableCollection<KeyValuePair<string, PvfTreeFileBase>> SelectedItems
	{
		[CompilerGenerated]
		get
		{
			return FZAr3pn25F;
		}
		[CompilerGenerated]
		set
		{
			FZAr3pn25F = value;
		}
	}

	private PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	public ViewReNmaeNodesViewMode(PooledList<string> fileList)
	{
		El4rRcvveP = new PooledList<string>();
		QbnrN34bwm = new char[2] { '\\', '/' };
		SelectedItems = new ObservableCollection<KeyValuePair<string, PvfTreeFileBase>>();
		El4rRcvveP = fileList;
		ChildNodesSelector = new PVfTreeChildrenSelector(AE2rk65Ju4);
	}

	private IEnumerable AE2rk65Ju4(object P_0)
	{
		if (P_0 == null)
		{
			return null;
		}
		KeyValuePair<string, PvfTreeFileBase>? keyValuePair = (KeyValuePair<string, PvfTreeFileBase>)P_0;
		if (!keyValuePair.HasValue)
		{
			return null;
		}
		if (keyValuePair.Value.Value.IsFileMethon())
		{
			return null;
		}
		return keyValuePair.Value.Value.Children;
	}

	[Command]
	public async void Loaded(TreeListControl tree)
	{
		_003C_003Ec__DisplayClass29_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass29_0();
		CS_0024_003C_003E8__locals3.tree = tree;
		Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		IsLoaded = true;
		await Task.Run((Func<Task?>)VZkrUMAMh9);
		Trees.NotifyObserversOfChange();
		IsLoaded = false;
		El4rRcvveP.Dispose();
		((DispatcherObject)CS_0024_003C_003E8__locals3.tree).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			CS_0024_003C_003E8__locals3.tree.View.ExpandAllNodes();
		}, Array.Empty<object>());
	}

	[Command]
	public void NodeDoubleClick(NodeClickArgs nodeClickArgs)
	{
		_ = ((KeyValuePair<string, PvfTreeFileBase>)nodeClickArgs.Item).Value.IsFile;
	}

	[Command]
	public void OnDeleteSelectedNodes(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> items)
	{
		if (items != null)
		{
			_ = SelectedItems;
			DeleteTreeNodes(items);
		}
	}

	[Command]
	public async void OnSave(Window win)
	{
		IsLoaded = true;
		ResultData<IEnumerable<string>> result = await Task.Run((Func<Task<ResultData<IEnumerable<string>>>?>)iQcr0IpN9E);
		if (result.IsError)
		{
			IsLoaded = false;
			AppCore.ShowMsg(result.Msg, isError: true);
			return;
		}
		if (result.Data != null && result.Data.Any())
		{
			await AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(result.Data));
			string filePath = ((CurrentItem.HasValue && ((PvfTreeFileRename)CurrentItem.Value.Value).Changed) ? ((PvfTreeFileRename)CurrentItem.Value.Value).GetNewFullPath() : result.Data.FirstOrDefault());
			AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(filePath);
		}
		IsLoaded = false;
		win.Close();
	}

	private Task<ResultData<IEnumerable<string>>> iQcr0IpN9E()
	{
		ResultData<IEnumerable<string>> resultData = new ResultData<IEnumerable<string>>();
		Dictionary<string, PvfTreeFileRename> dictionary = jXJr7H8nmY();
		if (dictionary.Count == 0)
		{
			return Task.FromResult(resultData);
		}
		foreach (KeyValuePair<string, PvfTreeFileRename> item in dictionary)
		{
			if (Pvf.FileAny(item.Value.GetNewFullPath()))
			{
				resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNameDuplicate"), item.Key);
				return Task.FromResult(resultData);
			}
		}
		AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.DeleteTreeNode(dictionary.Keys);
		foreach (KeyValuePair<string, PvfTreeFileRename> item2 in dictionary)
		{
			Pvf.RenameFile(item2.Value.FullPath, item2.Value.GetNewFullPath());
		}
		resultData.Data = dictionary.Values.Select((PvfTreeFileRename it) => it.GetNewFullPath());
		return Task.FromResult(resultData);
	}

	private Dictionary<string, PvfTreeFileRename> jXJr7H8nmY()
	{
		Dictionary<string, PvfTreeFileRename> dictionary = new Dictionary<string, PvfTreeFileRename>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in (IEnumerable<KeyValuePair<string, PvfTreeFileBase>>)Trees)
		{
			if (item.Value.IsFile)
			{
				PvfTreeFileRename pvfTreeFileRename = (PvfTreeFileRename)item.Value;
				if (pvfTreeFileRename.Changed && !dictionary.ContainsKey(pvfTreeFileRename.FullPath))
				{
					dictionary.Add(pvfTreeFileRename.FullPath, pvfTreeFileRename);
				}
				continue;
			}
			Dictionary<string, PvfTreeFileRename> dictionary2 = lgZrX7TbvB(item.Value.Children);
			if (dictionary2.Count <= 0)
			{
				continue;
			}
			foreach (KeyValuePair<string, PvfTreeFileRename> item2 in dictionary2)
			{
				if (!dictionary.ContainsKey(item2.Key))
				{
					dictionary.Add(item2.Key, item2.Value);
				}
			}
		}
		return dictionary;
	}

	private Dictionary<string, PvfTreeFileRename> lgZrX7TbvB(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> trees)
	{
		Dictionary<string, PvfTreeFileRename> dictionary = new Dictionary<string, PvfTreeFileRename>();
		foreach (KeyValuePair<string, PvfTreeFileBase> tree in trees)
		{
			if (tree.Value.IsFile)
			{
				PvfTreeFileRename pvfTreeFileRename = (PvfTreeFileRename)tree.Value;
				if (pvfTreeFileRename.Changed && !dictionary.ContainsKey(pvfTreeFileRename.FullPath))
				{
					dictionary.Add(pvfTreeFileRename.FullPath, pvfTreeFileRename);
				}
				continue;
			}
			Dictionary<string, PvfTreeFileRename> dictionary2 = lgZrX7TbvB(tree.Value.Children);
			if (dictionary2.Count <= 0)
			{
				continue;
			}
			foreach (KeyValuePair<string, PvfTreeFileRename> item in dictionary2)
			{
				if (!dictionary.ContainsKey(item.Key))
				{
					dictionary.Add(item.Key, item.Value);
				}
			}
		}
		return dictionary;
	}

	[Command]
	public void OnIncreasingDownward(KeyValuePair<string, PvfTreeFileBase>? row)
	{
		if (!row.HasValue)
		{
			return;
		}
		PvfTreeFileRename pvfTreeFileRename = (PvfTreeFileRename)row.Value.Value;
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pvfTreeFileRename.NewFileName);
		string text = "";
		if (!long.TryParse(fileNameWithoutExtension, out var result))
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text2 = fileNameWithoutExtension;
			for (int i = 0; i < text2.Length; i++)
			{
				if (int.TryParse(text2[i].ToString(), out var result2))
				{
					stringBuilder.Append(result2.ToString());
				}
				else
				{
					stringBuilder.Clear();
				}
			}
			if (stringBuilder.Length > 0)
			{
				if (!long.TryParse(stringBuilder.ToString(), out result))
				{
					AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_ValueTooBig"));
					return;
				}
				text = $"{fileNameWithoutExtension.Substring(0, fileNameWithoutExtension.Length - stringBuilder.Length)}{result + 1}";
			}
		}
		else
		{
			text = (result + 1).ToString();
		}
		KeyValuePair<string, PvfTreeFileBase>? keyValuePair = TVircB26By(pvfTreeFileRename.FullPath);
		if (!keyValuePair.HasValue)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_End"), isError: true);
			return;
		}
		if (!keyValuePair.Value.Value.IsFile)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NotSupportFolder"), isError: true);
			return;
		}
		((PvfTreeFileRename)keyValuePair.Value.Value).NewFileName = text + Path.GetExtension(keyValuePair.Value.Value.FileName);
		CurrentItem = keyValuePair;
		FocusedRow = keyValuePair;
	}

	[Command]
	public void OnIncreasingUpward(KeyValuePair<string, PvfTreeFileBase>? row)
	{
		if (!row.HasValue)
		{
			return;
		}
		PvfTreeFileRename pvfTreeFileRename = (PvfTreeFileRename)row.Value.Value;
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pvfTreeFileRename.NewFileName);
		string text = "";
		if (!long.TryParse(fileNameWithoutExtension, out var result))
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text2 = fileNameWithoutExtension;
			for (int i = 0; i < text2.Length; i++)
			{
				if (int.TryParse(text2[i].ToString(), out var result2))
				{
					stringBuilder.Append(result2.ToString());
				}
				else
				{
					stringBuilder.Clear();
				}
			}
			if (stringBuilder.Length > 0)
			{
				if (!long.TryParse(stringBuilder.ToString(), out result))
				{
					AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_ValueTooBig"));
					return;
				}
				text = $"{fileNameWithoutExtension.Substring(0, fileNameWithoutExtension.Length - stringBuilder.Length)}{result - 1}";
			}
		}
		else
		{
			text = (result - 1).ToString();
		}
		KeyValuePair<string, PvfTreeFileBase>? keyValuePair = Oxwr8EVegX(pvfTreeFileRename.FullPath);
		if (!keyValuePair.HasValue)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_End"), isError: true);
			return;
		}
		if (!keyValuePair.Value.Value.IsFile)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NotSupportFolder"), isError: true);
			return;
		}
		((PvfTreeFileRename)keyValuePair.Value.Value).NewFileName = text + Path.GetExtension(keyValuePair.Value.Value.FileName);
		CurrentItem = keyValuePair;
		FocusedRow = keyValuePair;
	}

	[Command]
	public void OnCancel(Window win)
	{
		BVFrpD14vs();
		win.Close();
	}

	private void BVFrpD14vs()
	{
		Trees.Dispose();
		Trees = null;
	}

	private Task VZkrUMAMh9()
	{
		_003C_003Ec__DisplayClass40_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass40_0();
		CS_0024_003C_003E8__locals6.a52qlpNwhM = this;
		if (!El4rRcvveP.Any())
		{
			return Task.CompletedTask;
		}
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = 1
		};
		if (El4rRcvveP.Count() >= 100)
		{
			parallelOptions.MaxDegreeOfParallelism = 100;
		}
		else
		{
			parallelOptions.MaxDegreeOfParallelism = 1;
		}
		CS_0024_003C_003E8__locals6.lMcqjD8Tth = QbnrN34bwm;
		Parallel.ForEach(El4rRcvveP, parallelOptions, delegate(string fullPath, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = CS_0024_003C_003E8__locals6.a52qlpNwhM.Trees;
			string[] array = fullPath.Split(CS_0024_003C_003E8__locals6.lMcqjD8Tth);
			short num = 0;
			int num2 = array.Length - 1;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				lock (CS_0024_003C_003E8__locals6.a52qlpNwhM)
				{
					if (num == 0)
					{
						stringBuilder.Append(text);
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text);
					}
					if (observableConcurrentDictionaryEx.TryGetValue(text, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						PvfTreeFileRename pvfTreeFileRename = new PvfTreeFileRename(CS_0024_003C_003E8__locals6.a52qlpNwhM.Pvf, stringBuilder.ToString(), text, num == num2, num);
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFileRename);
						observableConcurrentDictionaryEx = pvfTreeFileRename.Children;
					}
				}
				num++;
			}
		});
		return Task.CompletedTask;
	}

	private KeyValuePair<string, PvfTreeFileBase>? TVircB26By(string P_0)
	{
		_003C_003Ec__DisplayClass41_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass41_0();
		ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = Trees;
		string[] array = P_0.Split(QbnrN34bwm);
		for (int i = 0; i < array.Length - 1; i++)
		{
			if (observableConcurrentDictionaryEx.TryGetValue(array[i], out var value))
			{
				observableConcurrentDictionaryEx = value.Children;
			}
		}
		CS_0024_003C_003E8__locals2.CyLqCaP9Dt = array[^1];
		List<KeyValuePair<string, PvfTreeFileBase>> list = (from it in observableConcurrentDictionaryEx
			orderby it.Key
			orderby it.Value.IsFile
			select it).ToList();
		int num = list.IndexOf((KeyValuePair<string, PvfTreeFileBase> it) => it.Key == CS_0024_003C_003E8__locals2.CyLqCaP9Dt);
		if (num == -1)
		{
			return null;
		}
		if (num < list.Count - 1)
		{
			return list[num + 1];
		}
		return null;
	}

	private KeyValuePair<string, PvfTreeFileBase>? Oxwr8EVegX(string P_0)
	{
		_003C_003Ec__DisplayClass42_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass42_0();
		ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = Trees;
		string[] array = P_0.Split(QbnrN34bwm);
		for (int i = 0; i < array.Length - 1; i++)
		{
			if (observableConcurrentDictionaryEx.TryGetValue(array[i], out var value))
			{
				observableConcurrentDictionaryEx = value.Children;
			}
		}
		CS_0024_003C_003E8__locals2.FvIqh0uhaL = array[^1];
		List<KeyValuePair<string, PvfTreeFileBase>> list = (from it in observableConcurrentDictionaryEx
			orderby it.Key
			orderby it.Value.IsFile
			select it).ToList();
		int num = list.IndexOf((KeyValuePair<string, PvfTreeFileBase> it) => it.Key == CS_0024_003C_003E8__locals2.FvIqh0uhaL);
		if (num == -1)
		{
			return null;
		}
		if (num - 1 < 0 || num > list.Count - 1)
		{
			return null;
		}
		return list[num - 1];
	}

	public void DeleteTreeNodes(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> selecteddic)
	{
		_003C_003Ec__DisplayClass43_0 obj = new _003C_003Ec__DisplayClass43_0();
		obj.FQ5qB7W0sg = selecteddic;
		obj.sD3qFll5rA = this;
		Action<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>> action = delegate(ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> trees)
		{
			foreach (KeyValuePair<string, PvfTreeFileBase> item in obj.FQ5qB7W0sg)
			{
				string fullPath = item.Value.FullPath;
				if (fullPath.IndexOf("/") < 0)
				{
					trees.RemoveTry(item.Key);
				}
				else
				{
					ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = trees;
					string[] array = fullPath.Split(obj.sD3qFll5rA.QbnrN34bwm, StringSplitOptions.RemoveEmptyEntries);
					PvfTreeFileBase value = null;
					for (int i = 0; i < array.Length - 1; i++)
					{
						string key = array[i];
						if (observableConcurrentDictionaryEx.TryGetValue(key, out value))
						{
							observableConcurrentDictionaryEx = value.Children;
						}
					}
					observableConcurrentDictionaryEx.RemoveTry(array[^1]);
				}
			}
			trees.NotifyObserversOfChange();
		};
		if (Trees != null && Trees.Any())
		{
			action(Trees);
		}
	}

	public void DeleteTreeNode(KeyValuePair<string, PvfTreeFileBase> node)
	{
		_003C_003Ec__DisplayClass44_0 obj = new _003C_003Ec__DisplayClass44_0();
		obj.F18qWPKU24 = node;
		obj.IBeqmfPbBu = this;
		((Action<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>>)delegate(ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> trees)
		{
			if (trees != null)
			{
				string fullPath = obj.F18qWPKU24.Value.FullPath;
				if (fullPath.IndexOf("/") < 0)
				{
					trees.RemoveTry(obj.F18qWPKU24.Key);
				}
				else
				{
					ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = trees;
					string[] array = fullPath.Split(obj.IBeqmfPbBu.QbnrN34bwm, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length - 1; i++)
					{
						string key = array[i];
						if (observableConcurrentDictionaryEx.TryGetValue(key, out var value))
						{
							observableConcurrentDictionaryEx = value.Children;
						}
					}
					observableConcurrentDictionaryEx.RemoveTry(array[^1]);
				}
			}
		})(Trees);
		Trees.NotifyObserversOfChange();
	}
}
