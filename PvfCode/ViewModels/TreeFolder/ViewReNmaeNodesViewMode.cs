using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
	private PooledList<string> fileList;

	private readonly char[] pathSeparators;

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

	public PVfTreeChildrenSelector ChildNodesSelector { get; set; }

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

	public ObservableCollection<KeyValuePair<string, PvfTreeFileBase>> SelectedItems { get; set; }

	private PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	public ViewReNmaeNodesViewMode(PooledList<string> fileList)
	{
		this.fileList = new PooledList<string>();
		pathSeparators = new char[2] { '\\', '/' };
		SelectedItems = new ObservableCollection<KeyValuePair<string, PvfTreeFileBase>>();
		this.fileList = fileList;
		ChildNodesSelector = new PVfTreeChildrenSelector(GetChildren);
	}

	private IEnumerable GetChildren(object node)
	{
		if (node == null)
		{
			return null;
		}
		KeyValuePair<string, PvfTreeFileBase>? keyValuePair = (KeyValuePair<string, PvfTreeFileBase>)node;
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
		Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		IsLoaded = true;
		await Task.Run(CreateTreeAsync);
		Trees.NotifyObserversOfChange();
		IsLoaded = false;
		fileList.Dispose();
		((DispatcherObject)tree).Dispatcher.BeginInvoke((Delegate)(Action)(() => tree.View.ExpandAllNodes()), Array.Empty<object>());
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
		ResultData<IEnumerable<string>> result = await Task.Run(ApplyRenamesAsync);
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

	private Task<ResultData<IEnumerable<string>>> ApplyRenamesAsync()
	{
		ResultData<IEnumerable<string>> resultData = new ResultData<IEnumerable<string>>();
		Dictionary<string, PvfTreeFileRename> dictionary = GetChangedFiles();
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

	private Dictionary<string, PvfTreeFileRename> GetChangedFiles()
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
			Dictionary<string, PvfTreeFileRename> dictionary2 = GetChangedFiles(item.Value.Children);
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

	private Dictionary<string, PvfTreeFileRename> GetChangedFiles(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> trees)
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
			Dictionary<string, PvfTreeFileRename> dictionary2 = GetChangedFiles(tree.Value.Children);
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
		KeyValuePair<string, PvfTreeFileBase>? keyValuePair = GetNextNode(pvfTreeFileRename.FullPath);
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
		KeyValuePair<string, PvfTreeFileBase>? keyValuePair = GetPreviousNode(pvfTreeFileRename.FullPath);
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
		Clear();
		win.Close();
	}

	private void Clear()
	{
		Trees.Dispose();
		Trees = null;
	}

	private Task CreateTreeAsync()
	{
		if (!fileList.Any())
		{
			return Task.CompletedTask;
		}
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = 1
		};
		if (fileList.Count() >= 100)
		{
			parallelOptions.MaxDegreeOfParallelism = 100;
		}
		else
		{
			parallelOptions.MaxDegreeOfParallelism = 1;
		}
		Parallel.ForEach(fileList, parallelOptions, (fullPath, loopState) =>
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = Trees;
			string[] array = fullPath.Split(pathSeparators);
			short num = 0;
			int num2 = array.Length - 1;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				lock (this)
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
						PvfTreeFileRename pvfTreeFileRename = new PvfTreeFileRename(Pvf, stringBuilder.ToString(), text, num == num2, num);
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFileRename);
						observableConcurrentDictionaryEx = pvfTreeFileRename.Children;
					}
				}
				num++;
			}
		});
		return Task.CompletedTask;
	}

	private KeyValuePair<string, PvfTreeFileBase>? GetNextNode(string fullPath)
	{
		ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = Trees;
		string[] array = fullPath.Split(pathSeparators);
		for (int i = 0; i < array.Length - 1; i++)
		{
			if (observableConcurrentDictionaryEx.TryGetValue(array[i], out var value))
			{
				observableConcurrentDictionaryEx = value.Children;
			}
		}
		string fileName = array[^1];
		List<KeyValuePair<string, PvfTreeFileBase>> list = (from it in observableConcurrentDictionaryEx
			orderby it.Key
			orderby it.Value.IsFile
			select it).ToList();
		int num = list.IndexOf((KeyValuePair<string, PvfTreeFileBase> it) => it.Key == fileName);
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

	private KeyValuePair<string, PvfTreeFileBase>? GetPreviousNode(string fullPath)
	{
		ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = Trees;
		string[] array = fullPath.Split(pathSeparators);
		for (int i = 0; i < array.Length - 1; i++)
		{
			if (observableConcurrentDictionaryEx.TryGetValue(array[i], out var value))
			{
				observableConcurrentDictionaryEx = value.Children;
			}
		}
		string fileName = array[^1];
		List<KeyValuePair<string, PvfTreeFileBase>> list = (from it in observableConcurrentDictionaryEx
			orderby it.Key
			orderby it.Value.IsFile
			select it).ToList();
		int num = list.IndexOf((KeyValuePair<string, PvfTreeFileBase> it) => it.Key == fileName);
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
		Action<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>> action = trees =>
		{
			foreach (KeyValuePair<string, PvfTreeFileBase> item in selecteddic)
			{
				string fullPath = item.Value.FullPath;
				if (fullPath.IndexOf("/") < 0)
				{
					trees.RemoveTry(item.Key);
				}
				else
				{
					ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = trees;
					string[] array = fullPath.Split(pathSeparators, StringSplitOptions.RemoveEmptyEntries);
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
		((Action<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>>)(trees =>
		{
			if (trees != null)
			{
				string fullPath = node.Value.FullPath;
				if (fullPath.IndexOf("/") < 0)
				{
					trees.RemoveTry(node.Key);
				}
				else
				{
					ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = trees;
					string[] array = fullPath.Split(pathSeparators, StringSplitOptions.RemoveEmptyEntries);
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
		}))(Trees);
		Trees.NotifyObserversOfChange();
	}
}
