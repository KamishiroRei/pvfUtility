using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Grid;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.WindowsAPICodePack.Dialogs;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.MVVMServices;
using PvfCode.ViewModels.TreeFolder;
using PvfCode.Views.BookMark;
using Utools;

namespace PvfCode.ViewModels.BookMarkEdit;

public class BookMarkEditViewViewModel : ViewModelBase
{
	private bool isUpdate;

	private Action Close { get; set; }

	public ObservableConcurrentDictionaryEx<string, BookMarkDto> _Trees;

	private PVfTreeChildrenSelector childNodesSelector;

	public ITreeListService Service => GetService<ITreeListService>();

	public IHighlightingDefinition Highlighting { get; set; }

	public TextDocument DetailedInstructionsDocument { get; set; }

	public bool IsTreeList { get; set; }

	public string Title { get; set; }

	public bool IsPreview { get; set; }

	public bool IsUpdate
	{
		get
		{
			return isUpdate;
		}
		set
		{
			isUpdate = value;
			Group.UpdateTime = DateTime.Now;
		}
	}

	public BookMarkGroupDto Group
	{
		get
		{
			return GetProperty(() => Group);
		}
		set
		{
			SetProperty<BookMarkGroupDto>(() => Group, value);
		}
	}

	public ObservableConcurrentDictionaryEx<string, BookMarkDto> Trees
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

	public KeyValuePair<string, BookMarkDto>? SelectedNodeBindgBase
	{
		get
		{
			return GetProperty(() => SelectedNodeBindgBase);
		}
		set
		{
			SetProperty<KeyValuePair<string, BookMarkDto>?>(() => SelectedNodeBindgBase, value);
		}
	}

	public BookMarkDto FocuRow
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

	public ObservableCollection<KeyValuePair<string, BookMarkDto>> SelectedNodesBindBase
	{
		get
		{
			return GetProperty(() => SelectedNodesBindBase);
		}
		set
		{
			SetProperty<ObservableCollection<KeyValuePair<string, BookMarkDto>>>(() => SelectedNodesBindBase, value);
		}
	}

	public IDictionary<string, BookMarkDto> TreeGetSource { get; set; }

	public bool PasteIsEnabled
	{
		get
		{
			return GetProperty(() => PasteIsEnabled);
		}
		set
		{
			SetProperty(() => PasteIsEnabled, value);
		}
	}

	private IEnumerable GetChildren(object node)
	{
		if (node == null)
		{
			return null;
		}
		KeyValuePair<string, BookMarkDto> keyValuePair = (KeyValuePair<string, BookMarkDto>)node;
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
		KeyValuePair<string, BookMarkDto> keyValuePair = (KeyValuePair<string, BookMarkDto>)node;
		if (keyValuePair.Value.IsFile || !keyValuePair.Value.HaveChildren())
		{
			return null;
		}
		return GetFolders(keyValuePair.Value.Children);
	}

	private Dictionary<string, BookMarkDto> GetFolders(IDictionary<string, BookMarkDto> nodes)
	{
		Dictionary<string, BookMarkDto> dictionary = new Dictionary<string, BookMarkDto>();
		foreach (KeyValuePair<string, BookMarkDto> item in nodes)
		{
			if (!item.Value.IsFile)
			{
				dictionary.Add(item.Key, item.Value);
			}
		}
		return dictionary;
	}

	public BookMarkEditViewViewModel(Action close, bool isTreeList, bool isPreview = false, BookMarkGroupDto previewSource = null)
	{
		DetailedInstructionsDocument = new TextDocument();
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
		IsTreeList = isTreeList;
		IsPreview = isPreview;
		if (isPreview)
		{
			Group = previewSource;
			Title = AppSetting.Instance.GetIlogger()?.GetStr("BookMarkWindow_PreviewTitle");
		}
		else if (isTreeList)
		{
			Group = AppSetting.Instance.BookMarkGroup;
			Title = AppSetting.Instance.GetIlogger()?.GetStr("BookMarkWindow_SelectSaveDirectoryTitle");
		}
		else
		{
			string json = AppSetting.Instance.BookMarkGroup.ToJson();
			Group = json.JsonToObject<BookMarkGroupDto>();
			Title = AppSetting.Instance.GetIlogger()?.GetStr("BookMarkWindow_Title");
		}
		Close = close;
		DetailedInstructionsDocument.Text = Group.DetailedInstructions;
		SelectedNodesBindBase = new ObservableCollection<KeyValuePair<string, BookMarkDto>>();
		if (IsTreeList)
		{
			ChildNodesSelector = new PVfTreeChildrenSelector(GetFolderChildren);
		}
		else
		{
			ChildNodesSelector = new PVfTreeChildrenSelector(GetChildren);
		}
	}

	public void Loaded()
	{
		KeyValuePair<string, BookMarkDto> keyValuePair = Trees.ToList().Find((KeyValuePair<string, BookMarkDto> it) => it.Key == "我的书签");
		if (keyValuePair.Value.HaveChildren())
		{
			GoToNode(keyValuePair.Value.Children.ToList()[0]);
		}
	}

	[Command]
	public void NodeDoubleClick(NodeClickArgs nodeClickArgs)
	{
		if (!((KeyValuePair<string, BookMarkDto>)nodeClickArgs.Item).Value.IsFile)
		{
			TreeListNode treeListNode = Service.ContentToNode(nodeClickArgs.Item);
			if (treeListNode != null)
			{
				treeListNode.IsExpanded = !treeListNode.IsExpanded;
			}
		}
	}

	[Command]
	public void OnDeleteSelectedNodes()
	{
		if (IsSelectedNodes)
		{
			KeyValuePair<string, BookMarkDto> keyValuePair = SelectedNodesBindBase.FirstOrDefault();
			KeyValuePair<string, BookMarkDto> keyValuePair2 = Trees.FirstOrDefault();
			if (SelectedNodesBindBase.Count == 1 && keyValuePair.Value == keyValuePair2.Value)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotDeleteRootDir"));
			}
			else if (AppCore.Logger.ShowDialog(AppSetting.Instance.GetIlogger()?.GetStr("mess_DeleteBookmarkConfirm")) == MessageResult.Yes)
			{
				DeleteSelectedNodes();
			}
		}
	}

	private void DeleteSelectedNodes()
	{
		KeyValuePair<string, BookMarkDto>[] array = SelectedNodesBindBase.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<string, BookMarkDto> keyValuePair = array[i];
			TreeListNode treeListNode = Service.ContentToNode(keyValuePair);
			if (treeListNode == null)
			{
				continue;
			}
			TreeListNode parentNode = treeListNode.ParentNode;
			if (parentNode != null)
			{
				KeyValuePair<string, BookMarkDto> keyValuePair2 = GetNodeData(parentNode.Content);
				if (keyValuePair2.Value.Children.ContainsKey(keyValuePair.Key))
				{
					keyValuePair2.Value.Children.Remove(keyValuePair.Key);
				}
			}
		}
		Trees.NotifyObserversOfChange();
	}

	[Command]
	public void OnUp()
	{
		BookMarkDto focuRow = FocuRow;
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
				KeyValuePair<string, BookMarkDto> keyValuePair = GetNodeData(parentNode.Nodes[num - 1].Content);
				int sort = keyValuePair.Value.Sort;
				int sort2 = focuRow.Sort;
				focuRow.Sort = sort;
				keyValuePair.Value.Sort = sort2;
				Trees.NotifyObserversOfChange();
				IsUpdate = true;
			}
		}
	}

	private int GetNextSort(IEnumerable<BookMarkDto> nodes)
	{
		if (!nodes.Any())
		{
			return 0;
		}
		return nodes.Max((BookMarkDto it) => it.Sort) + 1;
	}

	[Command]
	public void OnDown()
	{
		BookMarkDto focuRow = FocuRow;
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
				KeyValuePair<string, BookMarkDto> keyValuePair = GetNodeData(parentNode.Nodes[num + 1].Content);
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
	public void OnAddBookMark(bool isFile)
	{
		if (FocuRow != null)
		{
			TreeListNode treeListNode = Service.ContentToNode(SelectedNodeBindgBase.Value);
			IDictionary<string, BookMarkDto> children = FocuRow.Children;
			if (FocuRow.IsFile)
			{
				children = ((KeyValuePair<string, BookMarkDto>?)treeListNode.ParentNode.Content).Value.Value.Children;
			}
			string key = Group.CheckTitle(children, (!isFile) ? AppSetting.Instance.GetIlogger()?.GetStr("NewBookMarkFolderName") : AppSetting.Instance.GetIlogger()?.GetStr("NewBookMarkFileName"));
			KeyValuePair<string, BookMarkDto> row = new KeyValuePair<string, BookMarkDto>(key, new BookMarkDto
			{
				IsFile = isFile
			});
			EditBookmarkView editBookmarkView = new EditBookmarkView(row, isAdd: true)
			{
				Owner = Application.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterScreen
			};
			if (editBookmarkView.ShowDialog().Value)
			{
				row = editBookmarkView.Row;
				int sort = GetNextSort(children.Values);
				row.Value.Sort = sort;
				AddNode(row.Key, row.Value, children);
				IsUpdate = true;
			}
		}
	}

	[Command]
	public void OnEditBookMark()
	{
		if (FocuRow == null)
		{
			return;
		}
		TreeListNode treeListNode = Service.ContentToNode(SelectedNodeBindgBase.Value);
		if (treeListNode.ParentNode == null)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotEditRootDir"));
			return;
		}
		_ = FocuRow.Children;
		IDictionary<string, BookMarkDto> children = ((KeyValuePair<string, BookMarkDto>?)treeListNode.ParentNode.Content).Value.Value.Children;
		EditBookmarkView editBookmarkView = new EditBookmarkView(SelectedNodeBindgBase.Value, isAdd: false)
		{
			Owner = Application.Current.MainWindow,
			WindowStartupLocation = WindowStartupLocation.CenterScreen
		};
		if (editBookmarkView.ShowDialog().Value)
		{
			children.Remove(SelectedNodeBindgBase.Value.Key);
			Trees.NotifyObserversOfChange();
			KeyValuePair<string, BookMarkDto> row = editBookmarkView.Row;
			AddNode(row.Key, row.Value, children);
			IsUpdate = true;
		}
	}

	[Command]
	public async void OnSaveCommand()
	{
		if (IsTreeList)
		{
			if (FocuRow != null)
			{
				TreeListNode treeListNode = Service.ContentToNode(SelectedNodeBindgBase.Value);
				IDictionary<string, BookMarkDto> children = FocuRow.Children;
				if (FocuRow.IsFile)
				{
					children = ((KeyValuePair<string, BookMarkDto>?)treeListNode.ParentNode.Content).Value.Value.Children;
				}
				TreeGetSource = children;
				Close();
				await AppSetting.Instance.SaveSetting();
			}
			return;
		}
		Group.DetailedInstructions = DetailedInstructionsDocument.Text;
		Group.IsShare = false;
		AppSetting.Instance.BookMarkGroup = Group;
		await AppSetting.Instance.SaveSetting();
		IsUpdate = false;
		Close();
	}

	[Command]
	public void OnCut()
	{
		PasteIsEnabled = false;
		if (!IsSelectedNodes)
		{
			return;
		}
		KeyValuePair<string, BookMarkDto> keyValuePair = SelectedNodesBindBase.FirstOrDefault();
		KeyValuePair<string, BookMarkDto> keyValuePair2 = Trees.FirstOrDefault();
		if (SelectedNodesBindBase.Count == 1 && keyValuePair.Value == keyValuePair2.Value)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotCutRootDir"));
			return;
		}
		SetCutStatus(Trees, null);
		foreach (KeyValuePair<string, BookMarkDto> item in SelectedNodesBindBase)
		{
			item.Value.CutStatus = true;
			if (!item.Value.IsFile && item.Value.HaveChildren())
			{
				SetCutStatus(item.Value.Children, true);
			}
		}
		PasteIsEnabled = true;
	}

	private void SetCutStatus(IDictionary<string, BookMarkDto> nodes, bool? status)
	{
		foreach (KeyValuePair<string, BookMarkDto> item in nodes)
		{
			if (item.Value.CutStatus != status)
			{
				item.Value.CutStatus = status;
			}
			if (!item.Value.IsFile && item.Value.HaveChildren())
			{
				SetCutStatus(item.Value.Children, status);
			}
		}
	}

	[Command]
	public void OnPaste()
	{
		List<KeyValuePair<string, BookMarkDto>> list = new List<KeyValuePair<string, BookMarkDto>>();
		CollectCutNodes(Trees, list);
		if (!list.Any())
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoPasteNode"));
			PasteIsEnabled = false;
			SetCutStatus(Trees, null);
			return;
		}
		KeyValuePair<string, BookMarkDto> keyValuePair = SelectedNodeBindgBase.Value;
		if (keyValuePair.Value.IsFile)
		{
			keyValuePair = GetNodeData(Service.ContentToNode(keyValuePair).ParentNode.Content);
		}
		foreach (KeyValuePair<string, BookMarkDto> item in list)
		{
			if (keyValuePair.Value.Children.ContainsKey(item.Key))
			{
				SetCutStatus(Trees, null);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_CutFailed") + item.Key, isError: true);
				PasteIsEnabled = false;
				return;
			}
		}
		RemoveCutNodes(Trees);
		int num = GetNextSort(keyValuePair.Value.Children.Values);
		foreach (KeyValuePair<string, BookMarkDto> item2 in list)
		{
			item2.Value.Sort = num;
			keyValuePair.Value.Children.Add(item2);
			num++;
		}
		Trees.NotifyObserversOfChange();
		PasteIsEnabled = false;
		SetCutStatus(Trees, null);
		GoToNode(list[0]);
	}

	private void RemoveCutNodes(IDictionary<string, BookMarkDto> nodes)
	{
		KeyValuePair<string, BookMarkDto>[] array = nodes.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<string, BookMarkDto> keyValuePair = array[i];
			if (keyValuePair.Value.CutStatus.HasValue)
			{
				nodes.Remove(keyValuePair.Key);
			}
			else if (!keyValuePair.Value.IsFile)
			{
				RemoveCutNodes(keyValuePair.Value.Children);
			}
		}
	}

	private void CollectCutNodes(IDictionary<string, BookMarkDto> nodes, List<KeyValuePair<string, BookMarkDto>> result)
	{
		foreach (KeyValuePair<string, BookMarkDto> item in nodes)
		{
			if (item.Value.CutStatus.HasValue)
			{
				result.Add(item);
			}
			else if (!item.Value.IsFile && item.Value.HaveChildren())
			{
				CollectCutNodes(item.Value.Children, result);
			}
		}
	}

	public void AddNode(string title, BookMarkDto bookMarkData, IDictionary<string, BookMarkDto>? source)
	{
		KeyValuePair<string, BookMarkDto> newNode = Group.AddNode(title, bookMarkData, source);
		RefreshAndSelectNode(newNode);
	}

	private async void RefreshAndSelectNode(KeyValuePair<string, BookMarkDto> newNode)
	{
		Trees.NotifyObserversOfChange();
		await Task.Delay(10);
		GoToNode(newNode);
	}

	public void GoToNode(KeyValuePair<string, BookMarkDto>? row)
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
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)async delegate
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

	private void ExpandNodePath(KeyValuePair<string, BookMarkDto> bookmarkNode)
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

	private KeyValuePair<string, BookMarkDto> GetNodeData(object content)
	{
		return (KeyValuePair<string, BookMarkDto>)content;
	}
}
