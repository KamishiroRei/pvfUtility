using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Collections.Pooled;
using PvfCode.Dot;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Models.Pvf.ImportModels;
using PvfCode.Models.Pvf.PvfFileDiffModels.Enums;
using Swordfish.NET.Collections.Auxiliary;
using Utools;

namespace PvfCode.ViewModels.TreeFolder;

public class TreeGroup : ModelBase
{
	private readonly TreeViewType TreeType;

	public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> _Trees;

	public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>? _SearchResultTrees;

	private PVfTreeChildrenSelector childNodesSelector;

	private bool loading;

	private string loadingTitle;

	private readonly char[] pathSeparators;

	public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> Trees
	{
		get
		{
			if (!ShowSearchResulTrees)
			{
				return _Trees;
			}
			return _SearchResultTrees;
		}
		set
		{
			_Trees = value;
			DoNotify("Trees");
			if (!ShowSearchResulTrees)
			{
				UpdateFileCount();
			}
		}
	}

	public SearchPanelOptions SearchPanelOptions { get; set; }

	public bool ShowSearchResulTrees { get; set; }

	public PVfTreeChildrenSelector ChildNodesSelector
	{
		get
		{
			return childNodesSelector;
		}
		set
		{
			childNodesSelector = value;
			DoNotify("ChildNodesSelector");
		}
	}

	public bool Loading
	{
		get
		{
			return loading;
		}
		set
		{
			loading = value;
			DoNotify("Loading");
		}
	}

	public string LoadingTitle
	{
		get
		{
			return loadingTitle;
		}
		set
		{
			loadingTitle = value;
			DoNotify(LoadingTitle);
		}
	}

	public int FileCount => CountFiles();

	public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> GetTrees()
	{
		return _Trees;
	}

	public void ClearSearchResult()
	{
		_SearchResultTrees = null;
		ShowSearchResulTrees = false;
		DoNotify("Trees");
	}

	public TreeGroup(TreeViewType treeType)
	{
		pathSeparators = new char[2] { '\\', '/' };
		SearchPanelOptions = new SearchPanelOptions();
		TreeType = treeType;
		Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		InitializeChildNodesSelector();
		if (TreeType == TreeViewType.SearchResult)
		{
			LoadingTitle = AppSetting.Instance.GetIlogger().GetStr("mess_Searching");
		}
		else
		{
			LoadingTitle = "Loading...";
		}
	}

	public void UpdateFileCount()
	{
		DoNotify("FileCount");
	}

	private IEnumerable GetChildNodes(object item)
	{
		if (item == null)
		{
			return null;
		}
		KeyValuePair<string, PvfTreeFileBase>? keyValuePair = (KeyValuePair<string, PvfTreeFileBase>)item;
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

	private IEnumerable GetFolderChildNodes(object item)
	{
		if (item == null)
		{
			return null;
		}
		KeyValuePair<string, PvfTreeFileBase> keyValuePair = (KeyValuePair<string, PvfTreeFileBase>)item;
		if (keyValuePair.Value.IsFileMethon())
		{
			return null;
		}
		return FilterFolderNodes(keyValuePair.Value.Children);
	}

	private Dictionary<string, PvfTreeFileBase> FilterFolderNodes(IDictionary<string, PvfTreeFileBase> nodes)
	{
		Dictionary<string, PvfTreeFileBase> dictionary = new Dictionary<string, PvfTreeFileBase>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in nodes)
		{
			if (!item.Value.IsFile)
			{
				dictionary.Add(item.Key, item.Value);
			}
		}
		return dictionary;
	}

	private void InitializeChildNodesSelector()
	{
		if (TreeType == TreeViewType.SelectFolder)
		{
			ChildNodesSelector = new PVfTreeChildrenSelector(GetFolderChildNodes);
		}
		else
		{
			ChildNodesSelector = new PVfTreeChildrenSelector(GetChildNodes);
		}
	}

	public void Clear()
	{
		_Trees = null;
		_SearchResultTrees = null;
		Trees = null;
		ClearSearchResult();
	}

	public async Task<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>> CreateTrees(PooledList<string> fileList, PvfGroup? pvf = null, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> source = null)
	{
		if (Trees == null)
		{
			Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		}
		if (source == null)
		{
			source = _Trees;
		}
		if (fileList == null || fileList.Count() == 0)
		{
			return Trees;
		}
		Loading = true;
		if (pvf == null)
		{
			pvf = AppCore.ViewModelBase.PVF;
		}
		await Task.Run(() => BuildTrees(fileList, pvf, source));
		UpdateFileCount();
		if (Trees != null)
		{
			source.NotifyObserversOfChange();
		}
		Loading = false;
		return Trees;
	}

	private Task BuildTrees(IEnumerable<string> fileList, PvfGroup pvf, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> source)
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
		Parallel.ForEach(fileList.Where((string it) => it != null), parallelOptions, delegate(string fullPath, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = source;
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
					PvfTreeFile pvfTreeFile = new PvfTreeFile(pvf, stringBuilder.ToString(), text, num == num2, num);
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFile);
						observableConcurrentDictionaryEx = pvfTreeFile.Children;
					}
				}
				num++;
			}
		});
		return Task.CompletedTask;
	}

	public async Task CreateTreesFolder(PooledList<string> fileList, PvfGroup? pvf = null, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> source = null)
	{
		if (Trees == null)
		{
			Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		}
		if (source == null)
		{
			source = _Trees;
		}
		if (fileList != null && fileList.Count() != 0)
		{
			Loading = true;
			if (pvf == null)
			{
				pvf = AppCore.ViewModelBase.PVF;
			}
			await Task.Run(() => BuildFolderTrees(fileList, pvf, source));
			UpdateFileCount();
			if (Trees != null)
			{
				source.NotifyObserversOfChange();
			}
			Loading = false;
		}
	}

	private Task BuildFolderTrees(IEnumerable<string> fileList, PvfGroup pvf, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> source)
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
		Parallel.ForEach(fileList.Where((string it) => it != null), parallelOptions, delegate(string fullPath, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = source;
			string[] array = fullPath.Split(pathSeparators);
			short num = 0;
			_ = array.Length;
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
					PvfTreeFile pvfTreeFile = new PvfTreeFile(pvf, stringBuilder.ToString(), text, isFile: false, num);
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFile);
						observableConcurrentDictionaryEx = pvfTreeFile.Children;
					}
				}
				num++;
			}
		});
		return Task.CompletedTask;
	}

	public async Task<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>> CreateFileListDescriptionTrees(IEnumerable<string> fileList, Dictionary<string, TreelistCommentRes> treelistCommentSource, PvfGroup? pvf = null, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> source = null)
	{
		if (Trees == null)
		{
			Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		}
		if (source == null)
		{
			source = _Trees;
		}
		if (fileList == null || fileList.Count() == 0)
		{
			return Trees;
		}
		Loading = true;
		if (pvf == null)
		{
			pvf = AppCore.ViewModelBase.PVF;
		}
		await Task.Run(() => BuildFileListDescriptionTrees(fileList, pvf, treelistCommentSource, source));
		if (Trees != null)
		{
			DoNotify("Trees");
			source.NotifyObserversOfChange();
		}
		UpdateFileCount();
		Loading = false;
		return Trees;
	}

	private Task BuildFileListDescriptionTrees(IEnumerable<string> fileList, PvfGroup pvf, Dictionary<string, TreelistCommentRes> treelistCommentSource, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> source)
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
		Parallel.ForEach(fileList.Where((string it) => it != null), parallelOptions, delegate(string fullPath, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = source;
			string[] array = fullPath.Split(pathSeparators);
			short num = 0;
			int num2 = array.Length - 1;
			string text = null;
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				text = ((num != 0) ? (text + "/" + text2) : text2);
				lock (this)
				{
					if (observableConcurrentDictionaryEx.TryGetValue(text2, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
					PvfTreeFileFileListDescription pvfTreeFileFileListDescription = new PvfTreeFileFileListDescription(pvf, text, text2, num == num2, num, treelistCommentSource);
						observableConcurrentDictionaryEx.AddTry(text2, pvfTreeFileFileListDescription);
						observableConcurrentDictionaryEx = pvfTreeFileFileListDescription.Children;
					}
				}
				num++;
			}
		});
		return Task.CompletedTask;
	}

	public async Task DiffCreateTrees(IDictionary<string, List<PvfFileDiffType>?> fileList, TreeViewType treeViewType)
	{
		if (Trees == null)
		{
			Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		}
		if (fileList != null && fileList.Count() != 0)
		{
			Loading = true;
			await Task.Run(() => DiffCreateTreesTask(fileList, treeViewType));
			if (Trees != null)
			{
				DoNotify("Trees");
				Trees.NotifyObserversOfChange();
			}
			UpdateFileCount();
			Loading = false;
		}
	}

	public Task DiffCreateTreesTask(IDictionary<string, List<PvfFileDiffType>?> fileList, TreeViewType treeViewType)
	{
		if (!fileList.Any())
		{
			return Task.CompletedTask;
		}
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = 100
		};
		if (fileList.Count() >= 100)
		{
			parallelOptions.MaxDegreeOfParallelism = 100;
		}
		else
		{
			parallelOptions.MaxDegreeOfParallelism = 1;
		}
		Parallel.ForEach(from it in fileList.ToArray()
			where it.Key != null
			select it, parallelOptions, delegate(KeyValuePair<string, List<PvfFileDiffType>> row, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = Trees;
			string[] array = row.Key.Split(pathSeparators);
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
						bool flag = num == num2;
					PvfTreeFileDiff pvfTreeFileDiff = new PvfTreeFileDiff(AppCore.ViewModelBase.PVF, stringBuilder.ToString(), text, flag, num, flag ? row.Value : null, treeViewType);
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFileDiff);
						observableConcurrentDictionaryEx = pvfTreeFileDiff.Children;
					}
				}
				num++;
			}
		});
		return Task.CompletedTask;
	}

	public async Task ImportFilesCreateTrees(IEnumerable<ImportFileItem> fileList, string targetPath, bool is7zip = false)
	{
		if (Trees == null)
		{
			Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		}
		if (fileList != null)
		{
			await Task.Run(() => BuildImportFileTrees(fileList.ToArray(), targetPath, is7zip));
			if (Trees != null)
			{
				Trees.NotifyObserversOfChange();
			}
			UpdateFileCount();
		}
	}

	private async Task BuildImportFileTrees(IEnumerable<ImportFileItem> fileList, string targetPath, bool is7zip)
	{
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
		if (is7zip && !string.IsNullOrEmpty(targetPath))
		{
			targetPath += "/";
		}
		bool hasNoTargetPath = targetPath == null;
		await Parallel.ForEachAsync(fileList, parallelOptions, async delegate(ImportFileItem importItem, CancellationToken ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = Trees;
			string text = ((!hasNoTargetPath) ? (targetPath + importItem.FilePath) : importItem.FilePath);
			string[] array = text.ToLower().Split(pathSeparators, StringSplitOptions.RemoveEmptyEntries);
			short num = 0;
			int num2 = array.Length - 1;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				lock (this)
				{
					if (num == 0)
					{
						stringBuilder.Append(text2);
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text2);
					}
					if (observableConcurrentDictionaryEx.TryGetValue(text2, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						bool flag = num == num2;
						PvfTreeFileImport pvfTreeFileImport = new PvfTreeFileImport(AppCore.ViewModelBase.PVF, stringBuilder.ToString(), text2, flag, num, flag ? importItem : null);
						observableConcurrentDictionaryEx.AddTry(text2, pvfTreeFileImport);
						observableConcurrentDictionaryEx = pvfTreeFileImport.Children;
					}
				}
				num++;
			}
		});
	}

	public void AddNode(string key, PvfTreeFileBase tree, IDictionary<string, PvfTreeFileBase>? source = null)
	{
		if (source == null)
		{
			source = _Trees;
			source.Add(key, tree);
		}
		else
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = _Trees;
			string[] array = tree.FullPath.Split(pathSeparators, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				if (observableConcurrentDictionaryEx.ContainsKey(array[i]))
				{
					observableConcurrentDictionaryEx = observableConcurrentDictionaryEx[array[i]].Children;
				}
			}
			observableConcurrentDictionaryEx.Add(key, tree);
		}
		Trees.NotifyObserversOfChange();
	}

	public void AddFolder(string newFilePath, int level)
	{
	}

	public PooledSet<string> SelectedNodesToFilePaths(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> selecteddic, GetTreeType type)
	{
		PooledSet<string> pooledSet = new PooledSet<string>();
		if (selecteddic == null || selecteddic.Count() == 0)
		{
			return pooledSet;
		}
		foreach (KeyValuePair<string, PvfTreeFileBase> item in from it in selecteddic
			orderby it.Key
			orderby it.Value.IsFile descending
			select it)
		{
			switch (type)
			{
			case GetTreeType.File:
				if (item.Value.IsFile)
				{
					pooledSet.Add(item.Value.FullPath);
				}
				break;
			case GetTreeType.Folder:
				if (!item.Value.IsFile)
				{
					pooledSet.Add(item.Value.FullPath);
				}
				break;
			case GetTreeType.All:
				pooledSet.Add(item.Value.FullPath);
				break;
			}
			if (item.Value.HaveChildren())
			{
				PooledSet<string> pooledSet2 = SelectedNodesToFilePaths(item.Value.Children, type);
				if (pooledSet2.Count > 0)
				{
					pooledSet.AddRange(pooledSet2.ToArray());
				}
			}
		}
		return pooledSet;
	}

	public PooledSet<string> SelectedNodesToFilePaths(IDictionary<string, PvfTreeFileBase> dic, GetTreeType type)
	{
		PooledSet<string> pooledSet = new PooledSet<string>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in from it in dic
			orderby it.Key
			orderby it.Value.IsFile descending
			select it)
		{
			switch (type)
			{
			case GetTreeType.File:
				if (item.Value.IsFile)
				{
					pooledSet.Add(item.Value.FullPath);
				}
				break;
			case GetTreeType.Folder:
				if (!item.Value.IsFile)
				{
					pooledSet.Add(item.Value.FullPath);
				}
				break;
			case GetTreeType.All:
				pooledSet.Add(item.Value.FullPath);
				break;
			}
			if (item.Value.HaveChildren())
			{
				PooledSet<string> pooledSet2 = SelectedNodesToFilePaths(item.Value.Children, type);
				if (pooledSet2.Count > 0)
				{
					pooledSet.AddRange(pooledSet2.ToArray());
				}
			}
		}
		return pooledSet;
	}

	public void DeleteTreeNodes(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> selecteddic)
	{
		Action<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>> action = delegate(ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> trees)
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
		if (_Trees != null && _Trees.Any())
		{
			action(_Trees);
		}
		if (_SearchResultTrees != null && _SearchResultTrees.Any())
		{
			action(_SearchResultTrees);
		}
	}

	public void DeleteTreeNode(KeyValuePair<string, PvfTreeFileBase> node)
	{
		Action<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>> action = delegate(ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> trees)
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
		};
		if (ShowSearchResulTrees)
		{
			action(_SearchResultTrees);
		}
		action(_Trees);
		Trees.NotifyObserversOfChange();
	}

	public void DeleteTreeNode(IEnumerable<string> fileList)
	{
		if (fileList == null)
		{
			return;
		}
		HashSet<KeyValuePair<string, PvfTreeFileBase>> hashSet = new HashSet<KeyValuePair<string, PvfTreeFileBase>>();
		foreach (string file in fileList)
		{
			KeyValuePair<string, PvfTreeFileBase>? keyValuePair = FilePathGetTreeNode(file);
			if (keyValuePair.HasValue)
			{
				hashSet.Add(keyValuePair.Value);
			}
		}
		DeleteTreeNodes(hashSet);
	}

	public List<KeyValuePair<string, PvfTreeFileBase>> FilePathGetTreeNode(IEnumerable<string> fileList)
	{
		if (fileList == null)
		{
			return null;
		}
		List<KeyValuePair<string, PvfTreeFileBase>> list = new List<KeyValuePair<string, PvfTreeFileBase>>();
		foreach (string file in fileList)
		{
			KeyValuePair<string, PvfTreeFileBase>? keyValuePair = FilePathGetTreeNode(file);
			if (keyValuePair.HasValue)
			{
				list.Add(keyValuePair.Value);
			}
		}
		return list;
	}

	public KeyValuePair<string, PvfTreeFileBase>? FilePathGetTreeNode(string filePath, IDictionary<string, PvfTreeFileBase>? source = null)
	{
		if (source == null)
		{
			source = Trees;
		}
		if (source == null)
		{
			return null;
		}
		if (filePath.IndexOf("/") < 0)
		{
			if (source.ContainsKey(filePath))
			{
				return FindNodeByKey(Trees, filePath);
			}
			return null;
		}
		IDictionary<string, PvfTreeFileBase> dictionary = source;
		string[] array = filePath.Split(pathSeparators, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length - 1; i++)
		{
			string key = array[i];
			if (dictionary.TryGetValue(key, out var value))
			{
				dictionary = value.Children;
			}
		}
		return FindNodeByKey(dictionary, array[^1]);
	}

	private KeyValuePair<string, PvfTreeFileBase>? FindNodeByKey(IDictionary<string, PvfTreeFileBase> nodes, string key)
	{
		foreach (KeyValuePair<string, PvfTreeFileBase> item in nodes)
		{
			if (item.Key == key)
			{
				return item;
			}
		}
		return null;
	}

	public List<string> GetFolderChildren(PvfTreeFileBase pvfTreeFile)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in (IEnumerable<KeyValuePair<string, PvfTreeFileBase>>)pvfTreeFile.Children)
		{
			if (item.Value.IsFile)
			{
				list.Add(item.Value.FullPath);
			}
			else if (item.Value.HaveChildren())
			{
				List<string> folderChildren = GetFolderChildren(item.Value);
				if (folderChildren.Count > 0)
				{
					list.AddRange(folderChildren);
				}
			}
		}
		return list;
	}

	public int GetFolderChildrenFolderCount(PvfTreeFileBase pvfTreeFile)
	{
		int num = 0;
		foreach (KeyValuePair<string, PvfTreeFileBase> item in (IEnumerable<KeyValuePair<string, PvfTreeFileBase>>)pvfTreeFile.Children)
		{
			if (!item.Value.IsFile)
			{
				if (item.Value.HaveChildren())
				{
					int folderChildrenFolderCount = GetFolderChildrenFolderCount(item.Value);
					num += folderChildrenFolderCount;
				}
				num++;
			}
		}
		return num;
	}

	public IList<string> GetFolderPaths()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in (IEnumerable<KeyValuePair<string, PvfTreeFileBase>>)Trees)
		{
			if (item.Value.IsFile)
			{
				continue;
			}
			list.Add(item.Key);
			if (item.Value.HaveChildren())
			{
				List<string> folderPaths = GetFolderPaths(item.Value);
				if (folderPaths.Any())
				{
					list.AddRange(folderPaths);
				}
			}
		}
		return list;
	}

	public List<string> GetFolderPaths(PvfTreeFileBase treeFile)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in (IEnumerable<KeyValuePair<string, PvfTreeFileBase>>)treeFile.Children)
		{
			if (item.Value.IsFile)
			{
				continue;
			}
			list.Add(item.Value.FullPath);
			if (item.Value.HaveChildren())
			{
				List<string> folderPaths = GetFolderPaths(item.Value);
				if (folderPaths.Any())
				{
					list.AddRange(folderPaths);
				}
			}
		}
		return list;
	}

	private int CountFiles()
	{
		if (Trees == null)
		{
			return 0;
		}
		int location = 0;
		KeyValuePair<string, PvfTreeFileBase>[] array = Trees.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<string, PvfTreeFileBase> keyValuePair = array[i];
			if (keyValuePair.Value.IsFile)
			{
				Interlocked.Increment(ref location);
			}
			else if (keyValuePair.Value.HaveChildren())
			{
				location += CountFiles(keyValuePair.Value.Children.Values);
			}
		}
		return location;
	}

	private int CountFiles(IEnumerable<PvfTreeFileBase> files)
	{
		int location = 0;
		PvfTreeFileBase[] array = files.ToArray();
		foreach (PvfTreeFileBase pvfTreeFileBase in array)
		{
			if (pvfTreeFileBase.IsFile)
			{
				Interlocked.Increment(ref location);
			}
			else if (pvfTreeFileBase.HaveChildren())
			{
				location += CountFiles(pvfTreeFileBase.Children.Values);
			}
		}
		return location;
	}

	public List<string> GetAllFilePaths()
	{
		List<string> list = new List<string>();
		if (Trees == null)
		{
			return list;
		}
		foreach (PvfTreeFileBase value in Trees.Values)
		{
			if (value.IsFile)
			{
				list.Add(value.FullPath);
			}
			else if (value.HaveChildren())
			{
				List<string> filePaths = GetFilePaths(value.Children.Values);
				if (filePaths.Count > 0)
				{
					list.AddRange(filePaths);
				}
			}
		}
		return list;
	}

	public List<string> GetFilePaths(IEnumerable<PvfTreeFileBase> values)
	{
		List<string> list = new List<string>();
		foreach (PvfTreeFileBase value in values)
		{
			if (value.IsFile)
			{
				list.Add(value.FullPath);
			}
			else if (value.HaveChildren())
			{
				List<string> filePaths = GetFilePaths(value.Children.Values.ToList());
				if (filePaths.Count > 0)
				{
					list.AddRange(filePaths);
				}
			}
		}
		return list;
	}

	public List<ImportFileItem> GetAllImportItems()
	{
		if (Trees == null)
		{
			return null;
		}
		List<ImportFileItem> list = new List<ImportFileItem>();
		foreach (PvfTreeFileImport value in Trees.Values)
		{
			if (value.IsFile)
			{
				list.Add(value.ImportItem);
			}
			else if (value.HaveChildren())
			{
				List<ImportFileItem> importItems = GetImportItems(value.Children.Values);
				if (importItems.Count > 0)
				{
					list.AddRange(importItems);
				}
			}
		}
		return list;
	}

	public List<ImportFileItem> GetImportItems(IEnumerable<PvfTreeFileBase> values)
	{
		List<ImportFileItem> list = new List<ImportFileItem>();
		foreach (PvfTreeFileImport value in values)
		{
			if (value.IsFile)
			{
				list.Add(value.ImportItem);
			}
			else if (value.HaveChildren())
			{
				List<ImportFileItem> importItems = GetImportItems(value.Children.Values);
				if (importItems.Count > 0)
				{
					list.AddRange(importItems);
				}
			}
		}
		return list;
	}

	public Task SetFilesCutStatus(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> treeFiles)
	{
		if (treeFiles == null)
		{
			return Task.CompletedTask;
		}
		Parallel.ForEach<KeyValuePair<string, PvfTreeFileBase>>(treeFiles, delegate(KeyValuePair<string, PvfTreeFileBase> row)
		{
			if (!row.Value.IsShearStatus.HasValue)
			{
				row.Value.IsShearStatus = true;
				row.Value.DoNotifyStatus();
			}
		});
		return Task.CompletedTask;
	}

	public Task ClearFileCopyStatus(IEnumerable<string> fileList)
	{
		if (Trees == null || !Trees.Any())
		{
			return Task.CompletedTask;
		}
		Parallel.ForEach(fileList, delegate(string P_0)
		{
			KeyValuePair<string, PvfTreeFileBase>? keyValuePair = FilePathGetTreeNode(P_0);
			if (keyValuePair.HasValue)
			{
				keyValuePair.Value.Value.IsShearStatus = null;
				keyValuePair.Value.Value.DoNotifyStatus();
			}
		});
		return Task.CompletedTask;
	}

	public bool Any(string filePath)
	{
		if (Trees == null || !Trees.Any())
		{
			return false;
		}
		if (filePath.IndexOf('/') < 0)
		{
			return Trees.ContainsKey(filePath);
		}
		ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = Trees;
		string[] array = filePath.Split(pathSeparators, StringSplitOptions.RemoveEmptyEntries);
		foreach (string key in array)
		{
			if (observableConcurrentDictionaryEx.TryGetValue(key, out var value))
			{
				observableConcurrentDictionaryEx = value.Children;
				continue;
			}
			return false;
		}
		return true;
	}

	public Task<int> SearchFileList(string keyword)
	{
		if (AppSetting.Instance.PvfConfig.DefaultEncoding == EncodingType.TW && SearchPanelOptions.ConvertTw && !AppSetting.Instance.TreeSetting.TwConvertSimplified)
		{
			keyword = ChineseHelper.ToTraditional(keyword);
		}
		ConcurrentBag<KeyValuePair<string, PvfTreeFileBase>> searchResults = new ConcurrentBag<KeyValuePair<string, PvfTreeFileBase>>();
		if (_Trees != null && _Trees.Any())
		{
			Action<KeyValuePair<string, PvfTreeFileBase>> addSearchResult = delegate(KeyValuePair<string, PvfTreeFileBase> row)
			{
				lock (this)
				{
					PvfTreeFileBase value = row.Value;
					if (SearchPanelOptions.FilePath && value.FullPath.ToLower().Contains(keyword))
					{
						searchResults.Add(row);
					}
					else if (SearchPanelOptions.ItemName && value.ItemName != null && value.ItemName.Contains(keyword))
					{
						searchResults.Add(row);
					}
					else if (SearchPanelOptions.Comment && value.Comment != null && value.Comment.Contains(keyword))
					{
						searchResults.Add(row);
					}
					else if (SearchPanelOptions.ItemCode && value.ItemCodeStr != null && value.ItemCodeStr.Contains(keyword))
					{
						searchResults.Add(row);
					}
				}
			};
			Parallel.ForEach<KeyValuePair<string, PvfTreeFileBase>>(_Trees, async delegate(KeyValuePair<string, PvfTreeFileBase> item)
			{
				if (item.Value.IsFile)
				{
					addSearchResult(item);
				}
				else
				{
					addSearchResult(item);
					if (item.Value.HaveChildren())
					{
						await TraverseTrees(item.Value.Children, addSearchResult);
					}
				}
			});
		}
		_SearchResultTrees.AddRange(searchResults);
		ShowSearchResulTrees = true;
		DoNotify("Trees");
		return Task.FromResult(searchResults.Count);
	}

	private Task TraverseTrees(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> trees, Action<KeyValuePair<string, PvfTreeFileBase>> treeAction)
	{
		Parallel.ForEach<KeyValuePair<string, PvfTreeFileBase>>(trees, async delegate(KeyValuePair<string, PvfTreeFileBase> item)
		{
			if (item.Value.IsFile)
			{
				treeAction(item);
			}
			else
			{
				treeAction(item);
				if (item.Value.HaveChildren())
				{
					await TraverseTrees(item.Value.Children, treeAction);
				}
			}
		});
		return Task.CompletedTask;
	}

}
