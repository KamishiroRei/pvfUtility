using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Grid;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.MVVMServices;
using PvfCode.Models.Macro;
using PvfCode.ViewModels.TreeFolder;
using Utools;

namespace PvfCode.Views.Macro;

public class WindowMacroToolViewModel : ViewModelBase, IDisposable
{
	private readonly Action Close;

	public Window Win;

	private PVfTreeChildrenSelector childNodesSelector;

	public ITreeListService Service => GetService<ITreeListService>();

	public string Title { get; set; }

	public bool IsUpdate { get; set; }

	public bool IsTreeList { get; set; }

	public MacroGroup Group { get; set; }

	public ObservableConcurrentDictionaryEx<string, MacroData> Trees
	{
		get
		{
			return Group.Trees;
		}
		set
		{
			Group.Trees = value;
		}
	}

	public PVfTreeChildrenSelector ChildNodesSelector
	{
		get
		{
			return childNodesSelector;
		}
		set
		{
			childNodesSelector = value;
			RaisePropertyChanged("ChildNodesSelector");
		}
	}

	public KeyValuePair<string, MacroData>? SelectedNodeBindgBase
	{
		get
		{
			return GetProperty(() => SelectedNodeBindgBase);
		}
		set
		{
			SetProperty<KeyValuePair<string, MacroData>?>(() => SelectedNodeBindgBase, value);
		}
	}

	public MacroData FocuRow
	{
		get
		{
			if (SelectedNodeBindgBase.HasValue)
			{
				return SelectedNodeBindgBase.Value.Value;
			}
			return null;
		}
	}

	public bool IsSelectedNodes
	{
		get
		{
			if (SelectedNodesBindBase != null)
			{
				return SelectedNodesBindBase.Count > 0;
			}
			return false;
		}
	}

	public ObservableCollection<KeyValuePair<string, MacroData>> SelectedNodesBindBase
	{
		get
		{
			return GetProperty(() => SelectedNodesBindBase);
		}
		set
		{
			SetProperty<ObservableCollection<KeyValuePair<string, MacroData>>>(() => SelectedNodesBindBase, value);
		}
	}

	public bool IsSelect { get; set; }

	public bool SaveLoading
	{
		get
		{
			return GetProperty(() => SaveLoading);
		}
		set
		{
			SetProperty(() => SaveLoading, value);
		}
	}

	public WindowMacroToolViewModel(Action close, bool isTreeList, MacroType? macroType = null)
	{
		IsTreeList = isTreeList;
		Close = close;
		SelectedNodesBindBase = new ObservableCollection<KeyValuePair<string, MacroData>>();
		string json = AppSetting.Instance.MacroGroup.ToJson();
		Group = json.JsonToObject<MacroGroup>();
		if (IsTreeList)
		{
			ChildNodesSelector = new PVfTreeChildrenSelector(GetFolderChildren);
			Title = AppSetting.Instance.GetIlogger().GetStr("mess_SelectMacroSavePath");
			KeyValuePair<string, MacroData>[] array = Group.Trees.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				KeyValuePair<string, MacroData> keyValuePair = array[i];
				if (keyValuePair.Key != macroType.ToString())
				{
					Group.Trees.Remove(keyValuePair.Key);
				}
			}
		}
		else
		{
			ChildNodesSelector = new PVfTreeChildrenSelector(GetChildren);
			Title = AppSetting.Instance.GetIlogger().GetStr("WindowMacroTool_Title");
		}
	}

	private IEnumerable GetChildren(object node)
	{
		if (node == null)
		{
			return null;
		}
		KeyValuePair<string, MacroData> keyValuePair = (KeyValuePair<string, MacroData>)node;
		if (keyValuePair.Value.IsFile || !keyValuePair.Value.HaveChildren())
		{
			return null;
		}
		return keyValuePair.Value.Children;
	}

	private IEnumerable GetFolderChildren(object node)
	{
		if (node == null)
		{
			return null;
		}
		KeyValuePair<string, MacroData> keyValuePair = (KeyValuePair<string, MacroData>)node;
		if (keyValuePair.Value.IsFile || !keyValuePair.Value.HaveChildren())
		{
			return null;
		}
		return GetFolders(keyValuePair.Value.Children);
	}

	private Dictionary<string, MacroData> GetFolders(IDictionary<string, MacroData> nodes)
	{
		Dictionary<string, MacroData> dictionary = new Dictionary<string, MacroData>();
		foreach (KeyValuePair<string, MacroData> item in nodes)
		{
			if (!item.Value.IsFile)
			{
				dictionary.Add(item.Key, item.Value);
			}
		}
		return dictionary;
	}

	[Command]
	public void NodeDoubleClick(NodeClickArgs nodeClickArgs)
	{
		if (!((KeyValuePair<string, MacroData>)nodeClickArgs.Item).Value.IsFile)
		{
			TreeListNode treeListNode = Service.ContentToNode(nodeClickArgs.Item);
			if (treeListNode != null)
			{
				treeListNode.IsExpanded = !treeListNode.IsExpanded;
			}
		}
	}

	[Command]
	public void OnAdd()
	{
		if (FocuRow != null)
		{
			TreeListNode treeListNode = Service.ContentToNode(SelectedNodeBindgBase.Value);
			ObservableConcurrentDictionaryEx<string, MacroData> children = FocuRow.Children;
			if (FocuRow.IsFile)
			{
				children = ((KeyValuePair<string, MacroData>?)treeListNode.ParentNode.Content).Value.Value.Children;
			}
			string dirName = Group.CheckTitle(children, AppSetting.Instance.GetIlogger()?.GetStr("WindowMacroTool_DefaultDirectoryName"));
			WindowEditMarcoName windowEditMarcoName = new WindowEditMarcoName(AppSetting.Instance.GetIlogger()?.GetStr("WindowMacroTool_Button_NewDirectory"), dirName)
			{
				Owner = Application.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};
			if (windowEditMarcoName.ShowDialog().Value)
			{
				string value = windowEditMarcoName.Input.Value;
				MacroData bookMarkData = new MacroData
				{
					Sort = GetNextSort(children.Values),
					IsFile = false
				};
				KeyValuePair<string, MacroData> newNode = Group.AddNode(value, bookMarkData, children);
				IsUpdate = true;
				RefreshAndSelectNode(newNode);
			}
		}
	}

	public async Task Add(string title, MacroData newData)
	{
		if (FocuRow == null)
		{
			return;
		}
		TreeListNode treeListNode = Service.ContentToNode(SelectedNodeBindgBase.Value);
		ObservableConcurrentDictionaryEx<string, MacroData> children = FocuRow.Children;
		if (FocuRow.IsFile)
		{
			children = ((KeyValuePair<string, MacroData>?)treeListNode.ParentNode.Content).Value.Value.Children;
		}
		newData.Sort = GetNextSort(children.Values);
		newData.IsFile = true;
		Group.AddNode(title, newData, children);
		string key = Trees.ToList()[0].Key;
		KeyValuePair<string, MacroData>[] array = AppSetting.Instance.MacroGroup.Trees.ToArray();
		foreach (KeyValuePair<string, MacroData> keyValuePair in array)
		{
			if (keyValuePair.Key == key)
			{
				AppSetting.Instance.MacroGroup.Trees.Remove(key);
				AppSetting.Instance.MacroGroup.Trees.AddTry(key, Trees[key]);
				await AppSetting.Instance.SaveSetting();
				break;
			}
		}
	}

	[Command]
	public void OnEdit()
	{
		MacroData focuRow = FocuRow;
		if (focuRow == null)
		{
			return;
		}
		if (focuRow.IsRoot)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_RootFolderCannotEdit"));
			return;
		}
		ObservableConcurrentDictionaryEx<string, MacroData> children = ((KeyValuePair<string, MacroData>?)Service.ContentToNode(SelectedNodeBindgBase.Value).ParentNode.Content).Value.Value.Children;
		WindowEditMarcoName windowEditMarcoName = new WindowEditMarcoName(AppSetting.Instance.GetIlogger()?.GetStr("WindowMacroTool_EditName"), SelectedNodeBindgBase.Value.Key)
		{
			Owner = Application.Current.MainWindow,
			WindowStartupLocation = WindowStartupLocation.CenterOwner
		};
		if (windowEditMarcoName.ShowDialog().Value)
		{
			string value = windowEditMarcoName.Input.Value;
			MacroData bookMarkData = focuRow;
			if (children.ContainsKey(SelectedNodeBindgBase.Value.Key))
			{
				children.Remove(SelectedNodeBindgBase.Value.Key);
			}
			KeyValuePair<string, MacroData> newNode = Group.AddNode(value, bookMarkData, children);
			IsUpdate = true;
			RefreshAndSelectNode(newNode);
		}
	}

	[Command]
	public void OnDelete()
	{
		if (IsSelectedNodes && AppCore.Logger.ShowDialog(AppSetting.Instance.GetIlogger()?.GetStr("mess_ConfirmDeleteSelectedNode")) == MessageResult.Yes)
		{
			DeleteSelectedNodes();
		}
	}

	private void DeleteSelectedNodes()
	{
		bool flag = false;
		KeyValuePair<string, MacroData>[] array = SelectedNodesBindBase.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<string, MacroData> keyValuePair = array[i];
			if (keyValuePair.Value.IsRoot)
			{
				flag = true;
				continue;
			}
			TreeListNode treeListNode = Service.ContentToNode(keyValuePair);
			if (treeListNode == null)
			{
				continue;
			}
			TreeListNode parentNode = treeListNode.ParentNode;
			if (parentNode != null)
			{
				KeyValuePair<string, MacroData> keyValuePair2 = GetNodeData(parentNode.Content);
				if (keyValuePair2.Value.Children.ContainsKey(keyValuePair.Key))
				{
					keyValuePair2.Value.Children.Remove(keyValuePair.Key);
					IsUpdate = true;
				}
			}
		}
		Trees.NotifyObserversOfChange();
		if (flag)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_RootFolderCannotDelete"));
		}
	}

	[Command]
	public void OnUp()
	{
		MacroData focuRow = FocuRow;
		if (focuRow == null)
		{
			return;
		}
		TreeListNode treeListNode = Service.ContentToNode(SelectedNodeBindgBase.Value);
		TreeListNode parentNode = treeListNode.ParentNode;
		if (parentNode != null)
		{
			int num = parentNode.Nodes.IndexOf(treeListNode);
			if (num != 0)
			{
				KeyValuePair<string, MacroData> keyValuePair = GetNodeData(parentNode.Nodes[num - 1].Content);
				int sort = keyValuePair.Value.Sort;
				int sort2 = focuRow.Sort;
				focuRow.Sort = sort;
				keyValuePair.Value.Sort = sort2;
				Trees.NotifyObserversOfChange();
				IsUpdate = true;
			}
		}
	}

	private int GetNextSort(IEnumerable<MacroData> nodes)
	{
		if (!nodes.Any())
		{
			return 0;
		}
		return nodes.Max((MacroData it) => it.Sort) + 1;
	}

	[Command]
	public void OnDown()
	{
		MacroData focuRow = FocuRow;
		if (focuRow == null)
		{
			return;
		}
		TreeListNode treeListNode = Service.ContentToNode(SelectedNodeBindgBase.Value);
		TreeListNode parentNode = treeListNode.ParentNode;
		if (parentNode != null)
		{
			int num = parentNode.Nodes.IndexOf(treeListNode);
			if (num != parentNode.Nodes.Count - 1)
			{
				KeyValuePair<string, MacroData> keyValuePair = GetNodeData(parentNode.Nodes[num + 1].Content);
				int sort = keyValuePair.Value.Sort;
				int sort2 = focuRow.Sort;
				focuRow.Sort = sort;
				keyValuePair.Value.Sort = sort2;
				Trees.NotifyObserversOfChange();
				IsUpdate = true;
			}
		}
	}

	[Command]
	public async void OnSave()
	{
		IsUpdate = false;
		if (IsTreeList)
		{
			if (!IsSelectedNodes)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectPath"));
				return;
			}
			IsSelect = true;
			Close();
			return;
		}
		SaveLoading = true;
		WindowLoading loading = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_Saving"), Win);
		loading.Show();
		AppSetting.Instance.MacroGroup = Group;
		await AppSetting.Instance.SaveSetting();
		loading.Close();
		SaveLoading = false;
		Close();
	}

	[Command]
	public async void OnUpload(KeyValuePair<string, MacroData> val)
	{
		await AppCore.SaveMacroData(val.Value, val.Key, Win, showShareCheckBox: true, isShare: true);
	}

	[Command]
	public void OnCancel()
	{
		Close();
	}

	private async void RefreshAndSelectNode(KeyValuePair<string, MacroData> newNode)
	{
		Trees.NotifyObserversOfChange();
		await Task.Delay(20);
		GoToNode(newNode);
	}

	public void GoToNode(KeyValuePair<string, MacroData>? row)
	{
		if (!row.HasValue)
		{
			return;
		}
		TreeListNode treeListNode = Service.ContentToNode(row);
		if (treeListNode == null)
		{
			return;
		}
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			if (treeListNode == null)
			{
				ExpandNodePath(row.Value);
			}
			Service.UnselectAll();
			SelectedNodeBindgBase = row;
			SelectedNodesBindBase?.Clear();
			SelectedNodesBindBase.Add(row.Value);
		}, Array.Empty<object>());
	}

	private void ExpandNodePath(KeyValuePair<string, MacroData> bookmarkNode)
	{
		TreeListNode treeListNode = Service.ContentToNode(bookmarkNode);
		List<TreeListNode> list = new List<TreeListNode>();
		list.Add(treeListNode);
		TreeListNode treeListNode2 = treeListNode;
		while (treeListNode2.ParentNode != null)
		{
			list.Add(treeListNode2.ParentNode);
			treeListNode2 = treeListNode2.ParentNode;
		}
		list.Reverse();
		foreach (TreeListNode item in list)
		{
			if (item != null)
			{
				item.IsExpanded = true;
			}
		}
	}

	private KeyValuePair<string, MacroData> GetNodeData(object content)
	{
		return (KeyValuePair<string, MacroData>)content;
	}

	public string SelectNodeToPath()
	{
		KeyValuePair<string, MacroData> value = SelectedNodeBindgBase.Value;
		TreeListNode parentNode = Service.ContentToNode(value).ParentNode;
		List<string> list = new List<string> { value.Key };
		while (parentNode != null)
		{
			list.Add(GetNodeData(parentNode.Content).Key);
			parentNode = parentNode.ParentNode;
		}
		list.Reverse();
		return string.Join("/", list);
	}

	public void Dispose()
	{
		Win = null;
	}
}
