using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass84_0
	{
		public TreeListNode JCJI3LIHsF;

		public BookMarkEditViewViewModel RO2IRfobXd;

		public KeyValuePair<string, BookMarkDto>? cOMINirhhe;

		public _003C_003Ec__DisplayClass84_0()
		{
		}

		internal async void A2uIVrLR6O()
		{
			if (JCJI3LIHsF == null)
			{
				RO2IRfobXd.Lakx2Wp2DA(cOMINirhhe.Value);
			}
			RO2IRfobXd.Service.UnselectAll();
			RO2IRfobXd.SelectedNodeBindgBase = cOMINirhhe;
			RO2IRfobXd.SelectedNodesBindBase?.Clear();
			RO2IRfobXd.SelectedNodesBindBase.Add(cOMINirhhe.Value);
		}
	}

	[CompilerGenerated]
	private IHighlightingDefinition y8px4Tx8G2;

	[CompilerGenerated]
	private TextDocument ArhxYOrKw0;

	[CompilerGenerated]
	private bool lUpxykc4G2;

	[CompilerGenerated]
	private string forxiJoYMD;

	[CompilerGenerated]
	private bool gIXxu9d9kd;

	private bool KRUxGPkamo;

	[CompilerGenerated]
	private Action d0CxxPvITx;

	public ObservableConcurrentDictionaryEx<string, BookMarkDto> _Trees;

	private PVfTreeChildrenSelector bocxQEDgef;

	[CompilerGenerated]
	private IDictionary<string, BookMarkDto> UswxaqoEo2;

	public ITreeListService Service => GetService<ITreeListService>();

	public IHighlightingDefinition Highlighting
	{
		[CompilerGenerated]
		get
		{
			return y8px4Tx8G2;
		}
		[CompilerGenerated]
		set
		{
			y8px4Tx8G2 = value;
		}
	}

	public TextDocument DetailedInstructionsDocument
	{
		[CompilerGenerated]
		get
		{
			return ArhxYOrKw0;
		}
		[CompilerGenerated]
		set
		{
			ArhxYOrKw0 = value;
		}
	}

	public bool IsTreeList
	{
		[CompilerGenerated]
		get
		{
			return lUpxykc4G2;
		}
		[CompilerGenerated]
		set
		{
			lUpxykc4G2 = value;
		}
	}

	public string Title
	{
		[CompilerGenerated]
		get
		{
			return forxiJoYMD;
		}
		[CompilerGenerated]
		set
		{
			forxiJoYMD = value;
		}
	}

	public bool IsPreview
	{
		[CompilerGenerated]
		get
		{
			return gIXxu9d9kd;
		}
		[CompilerGenerated]
		set
		{
			gIXxu9d9kd = value;
		}
	}

	public bool IsUpdate
	{
		get
		{
			return KRUxGPkamo;
		}
		set
		{
			KRUxGPkamo = value;
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
			return bocxQEDgef;
		}
		set
		{
			bocxQEDgef = value;
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

	public IDictionary<string, BookMarkDto> TreeGetSource
	{
		[CompilerGenerated]
		get
		{
			return UswxaqoEo2;
		}
		[CompilerGenerated]
		set
		{
			UswxaqoEo2 = value;
		}
	}

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

	[SpecialName]
	[CompilerGenerated]
	private Action t6Dx56Pkfy()
	{
		return d0CxxPvITx;
	}

	[SpecialName]
	[CompilerGenerated]
	private void zV3xSAhXgc(Action P_0)
	{
		d0CxxPvITx = P_0;
	}

	private IEnumerable mHXxlXZmnR(object P_0)
	{
		if (P_0 == null)
		{
			return null;
		}
		KeyValuePair<string, BookMarkDto> keyValuePair = (KeyValuePair<string, BookMarkDto>)P_0;
		if (keyValuePair.Value.IsFile || !keyValuePair.Value.HaveChildren())
		{
			return null;
		}
		return keyValuePair.Value.Children;
	}

	private IEnumerable A7xxjLf12I(object P_0)
	{
		if (P_0 == null)
		{
			return null;
		}
		KeyValuePair<string, BookMarkDto> keyValuePair = (KeyValuePair<string, BookMarkDto>)P_0;
		if (keyValuePair.Value.IsFile || !keyValuePair.Value.HaveChildren())
		{
			return null;
		}
		return a3NxTQe2bu(keyValuePair.Value.Children);
	}

	private Dictionary<string, BookMarkDto> a3NxTQe2bu(IDictionary<string, BookMarkDto> P_0)
	{
		Dictionary<string, BookMarkDto> dictionary = new Dictionary<string, BookMarkDto>();
		foreach (KeyValuePair<string, BookMarkDto> item in P_0)
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
		zV3xSAhXgc(close);
		DetailedInstructionsDocument.Text = Group.DetailedInstructions;
		SelectedNodesBindBase = new ObservableCollection<KeyValuePair<string, BookMarkDto>>();
		if (IsTreeList)
		{
			ChildNodesSelector = new PVfTreeChildrenSelector(A7xxjLf12I);
		}
		else
		{
			ChildNodesSelector = new PVfTreeChildrenSelector(mHXxlXZmnR);
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
				PkkxC6lwx9();
			}
		}
	}

	private void PkkxC6lwx9()
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
				KeyValuePair<string, BookMarkDto> keyValuePair2 = ugkxfqxAya(parentNode.Content);
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
				KeyValuePair<string, BookMarkDto> keyValuePair = ugkxfqxAya(parentNode.Nodes[num - 1].Content);
				int sort = keyValuePair.Value.Sort;
				int sort2 = focuRow.Sort;
				focuRow.Sort = sort;
				keyValuePair.Value.Sort = sort2;
				Trees.NotifyObserversOfChange();
				IsUpdate = true;
			}
		}
	}

	private int WdlxHSWxrn(IEnumerable<BookMarkDto> P_0)
	{
		if (!P_0.Any())
		{
			return 0;
		}
		return P_0.Max((BookMarkDto it) => it.Sort) + 1;
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
				KeyValuePair<string, BookMarkDto> keyValuePair = ugkxfqxAya(parentNode.Nodes[num + 1].Content);
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
				int sort = WdlxHSWxrn(children.Values);
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
				t6Dx56Pkfy()();
				await AppSetting.Instance.SaveSetting();
			}
			return;
		}
		Group.DetailedInstructions = DetailedInstructionsDocument.Text;
		Group.IsShare = false;
		AppSetting.Instance.BookMarkGroup = Group;
		await AppSetting.Instance.SaveSetting();
		IsUpdate = false;
		t6Dx56Pkfy()();
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
		HnxxhfDJnP(Trees, null);
		foreach (KeyValuePair<string, BookMarkDto> item in SelectedNodesBindBase)
		{
			item.Value.CutStatus = true;
			if (!item.Value.IsFile && item.Value.HaveChildren())
			{
				HnxxhfDJnP(item.Value.Children, true);
			}
		}
		PasteIsEnabled = true;
	}

	private void HnxxhfDJnP(IDictionary<string, BookMarkDto> P_0, bool? P_1)
	{
		foreach (KeyValuePair<string, BookMarkDto> item in P_0)
		{
			if (item.Value.CutStatus != P_1)
			{
				item.Value.CutStatus = P_1;
			}
			if (!item.Value.IsFile && item.Value.HaveChildren())
			{
				HnxxhfDJnP(item.Value.Children, P_1);
			}
		}
	}

	[Command]
	public void OnPaste()
	{
		List<KeyValuePair<string, BookMarkDto>> list = new List<KeyValuePair<string, BookMarkDto>>();
		l4ZxBno48i(Trees, list);
		if (!list.Any())
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoPasteNode"));
			PasteIsEnabled = false;
			HnxxhfDJnP(Trees, null);
			return;
		}
		KeyValuePair<string, BookMarkDto> keyValuePair = SelectedNodeBindgBase.Value;
		if (keyValuePair.Value.IsFile)
		{
			keyValuePair = ugkxfqxAya(Service.ContentToNode(keyValuePair).ParentNode.Content);
		}
		foreach (KeyValuePair<string, BookMarkDto> item in list)
		{
			if (keyValuePair.Value.Children.ContainsKey(item.Key))
			{
				HnxxhfDJnP(Trees, null);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_CutFailed") + item.Key, isError: true);
				PasteIsEnabled = false;
				return;
			}
		}
		BqaxvtoHXH(Trees);
		int num = WdlxHSWxrn(keyValuePair.Value.Children.Values);
		foreach (KeyValuePair<string, BookMarkDto> item2 in list)
		{
			item2.Value.Sort = num;
			keyValuePair.Value.Children.Add(item2);
			num++;
		}
		Trees.NotifyObserversOfChange();
		PasteIsEnabled = false;
		HnxxhfDJnP(Trees, null);
		GoToNode(list[0]);
	}

	private void BqaxvtoHXH(IDictionary<string, BookMarkDto> P_0)
	{
		KeyValuePair<string, BookMarkDto>[] array = P_0.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<string, BookMarkDto> keyValuePair = array[i];
			if (keyValuePair.Value.CutStatus.HasValue)
			{
				P_0.Remove(keyValuePair.Key);
			}
			else if (!keyValuePair.Value.IsFile)
			{
				BqaxvtoHXH(keyValuePair.Value.Children);
			}
		}
	}

	private void l4ZxBno48i(IDictionary<string, BookMarkDto> P_0, List<KeyValuePair<string, BookMarkDto>> result)
	{
		foreach (KeyValuePair<string, BookMarkDto> item in P_0)
		{
			if (item.Value.CutStatus.HasValue)
			{
				result.Add(item);
			}
			else if (!item.Value.IsFile && item.Value.HaveChildren())
			{
				l4ZxBno48i(item.Value.Children, result);
			}
		}
	}

	public void AddNode(string title, BookMarkDto bookMarkData, IDictionary<string, BookMarkDto>? source)
	{
		KeyValuePair<string, BookMarkDto> newNode = Group.AddNode(title, bookMarkData, source);
		tfMxmkCOnF(newNode);
	}

	private async void tfMxmkCOnF(KeyValuePair<string, BookMarkDto> newNode)
	{
		Trees.NotifyObserversOfChange();
		await Task.Delay(10);
		GoToNode(newNode);
	}

	public void GoToNode(KeyValuePair<string, BookMarkDto>? row)
	{
		_003C_003Ec__DisplayClass84_0 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass84_0();
		CS_0024_003C_003E8__locals15.RO2IRfobXd = this;
		CS_0024_003C_003E8__locals15.cOMINirhhe = row;
		if (!CS_0024_003C_003E8__locals15.cOMINirhhe.HasValue)
		{
			return;
		}
		CS_0024_003C_003E8__locals15.JCJI3LIHsF = Service.ContentToNode(CS_0024_003C_003E8__locals15.cOMINirhhe);
		if (CS_0024_003C_003E8__locals15.JCJI3LIHsF == null)
		{
			return;
		}
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)async delegate
		{
			if (CS_0024_003C_003E8__locals15.JCJI3LIHsF == null)
			{
				CS_0024_003C_003E8__locals15.RO2IRfobXd.Lakx2Wp2DA(CS_0024_003C_003E8__locals15.cOMINirhhe.Value);
			}
			CS_0024_003C_003E8__locals15.RO2IRfobXd.Service.UnselectAll();
			CS_0024_003C_003E8__locals15.RO2IRfobXd.SelectedNodeBindgBase = CS_0024_003C_003E8__locals15.cOMINirhhe;
			CS_0024_003C_003E8__locals15.RO2IRfobXd.SelectedNodesBindBase?.Clear();
			CS_0024_003C_003E8__locals15.RO2IRfobXd.SelectedNodesBindBase.Add(CS_0024_003C_003E8__locals15.cOMINirhhe.Value);
		}, Array.Empty<object>());
	}

	private void Lakx2Wp2DA(KeyValuePair<string, BookMarkDto> bookmarkNode)
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

	private KeyValuePair<string, BookMarkDto> ugkxfqxAya(object P_0)
	{
		return (KeyValuePair<string, BookMarkDto>)P_0;
	}
}
