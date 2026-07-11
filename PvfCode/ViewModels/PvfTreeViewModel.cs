using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using GMTool.Dot;
using GMTool.Dot.QueryModel;
using GMTool.Dot.QueryModel.Enums;
using GMTool.Dot.taiwan_cain_2nd;
using GMTool.Services;
using GMTool.SqlModel.Enums;
using Microsoft.WindowsAPICodePack.Dialogs;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using PvfCode.LoggerBase;
using PvfCode.MVVMServices;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Models.Pvf.ImportModels;
using PvfCode.Models.Pvf.PvfFileDiffModels.Enums;
using PvfCode.Services;
using PvfCode.Services.ExtractFilesModel;
using PvfCode.ViewModels.BookMarkEdit;
using PvfCode.ViewModels.Diff;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.ViewModels.PvfDiffTool;
using PvfCode.ViewModels.TreeFolder;
using PvfCode.ViewModels.TreeFolder.Enums;
using PvfCode.Views.BatchOperation;
using PvfCode.Views.BookMark;
using PvfCode.Views.ImportViews;
using PvfCode.Views.LstTools;
using PvfCode.Views.PvfTreeFolder;
using Utools;
using dRvgUYFXgiUlumD56M2;
using pvfUtility.WebApi.Dto.Res;

namespace PvfCode.ViewModels;

public class PvfTreeViewModel : ViewModelBase
{
	public delegate void DelegateDropFile(IEnumerable<string> files);

	public delegate void SelectedRowChangedDelegate(KeyValuePair<string, PvfTreeFileBase> selectedRow);

	public delegate void NodeDoubleClickDelegate(KeyValuePair<string, PvfTreeFileBase> row);

	public delegate void DelegateOpenDocument(PvfTreeFileBase treeFile);

	public delegate void RemoveSelectedItemsDelegate(PooledSet<string> fileList);

	public delegate void SetFocuNode(TreeListNode node);

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass104_0
	{
		public PvfTreeViewModel WBks8XnqmV;

		public KeyValuePair<string, PvfTreeFileBase>? UYOsMBjXf8;

		public _003C_003Ec__DisplayClass104_0()
		{
		}

		internal async void gj7scjJM1m()
		{
			TreeListNode treeListNode = WBks8XnqmV.Service.ContentToNode(UYOsMBjXf8.Value);
			WBks8XnqmV.SelectedNodesBindBase = null;
			WBks8XnqmV.SelectedNodesBindBase = new ObservableCollection<KeyValuePair<string, PvfTreeFileBase>>();
			if (treeListNode == null)
			{
				WBks8XnqmV.YOUB3nq5qd(UYOsMBjXf8.Value.Value.FullPath);
				treeListNode = WBks8XnqmV.Service.ContentToNode(UYOsMBjXf8.Value);
			}
			WBks8XnqmV.Service.SetFocusableNode(treeListNode);
			WBks8XnqmV.SelectedNodeBindgBase = UYOsMBjXf8;
			WBks8XnqmV.SelectedNodesBindBase.Add(UYOsMBjXf8.Value);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass124_0
	{
		public PvfTreeViewModel XbNs3gj9Fn;

		public string ytUsRkM01r;

		public _003C_003Ec__DisplayClass124_0()
		{
		}

		internal Task<int>? AyHsVUdHA0()
		{
			return XbNs3gj9Fn.TreeGroupData.SearchFileList(ytUsRkM01r);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass127_0
	{
		public PvfTreeViewModel PyXszAmjjb;

		public string QX5LDqIyPJ;

		public _003C_003Ec__DisplayClass127_0()
		{
		}

		internal async Task fxbsNfRXrP()
		{
			await PyXszAmjjb.TreeGroupData.CreateTrees(new PooledList<string> { QX5LDqIyPJ });
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass128_0
	{
		public PvfTreeViewModel NBMLjBXN4r;

		public List<string> I2bLTVohwZ;

		public _003C_003Ec__DisplayClass128_0()
		{
		}

		internal async Task dbxLlhtXBX()
		{
			await NBMLjBXN4r.TreeGroupData.CreateTrees(new PooledList<string>(I2bLTVohwZ));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass137_0
	{
		public ResultData<List<PostalSendRes>> UYyLH5sARl;

		public _003C_003Ec__DisplayClass137_0()
		{
		}

		internal async Task? EMrLCSc4dA()
		{
			DnfSqlService dnfSqlService = new DnfSqlService(AppSetting.Instance.GameOptions.GameServerOptions.GetDb());
			List<CharacInfoDto> list = await dnfSqlService.FindCharac(new FindUserDto
			{
				FindUserType = FindUserType.OnLineCharac
			});
			if (list == null || list.Count == 0)
			{
				AppCore.ShowMsg("没有在线角色无法发送");
				return;
			}
			ResultData<string> resultData = await dnfSqlService.SendPostal(list, UYyLH5sARl.Data, AppSetting.Instance.GMToolOptions.PostalSendTitle, AppSetting.Instance.GMToolOptions.PostalSendText);
			AppCore.Logger.Warning("邮件发送回调：" + resultData.Data);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass82_0
	{
		public string hlsLvI1aeU;

		public _003C_003Ec__DisplayClass82_0()
		{
		}

		internal void E5oLhMt3RN(string it)
		{
			AppCore.ShowExtractLstWindow(hlsLvI1aeU);
		}
	}

	[CompilerGenerated]
	private DelegateDropFile TevFvjfYjL;

	[CompilerGenerated]
	private DataTemplate tdrFB7h60x;

	[CompilerGenerated]
	private Style SBoFFfPLBX;

	[CompilerGenerated]
	private TreeViewType eJmFrvf0Dg;

	[CompilerGenerated]
	private TreeGroup whgFWeE87q;

	[CompilerGenerated]
	private SelectedRowChangedDelegate j5ZFmrSlL2;

	[CompilerGenerated]
	private List<KeyValuePair<string, PvfTreeFileBase>> i5XF2iknTV;

	[CompilerGenerated]
	private NodeDoubleClickDelegate gNZFfonSBa;

	[CompilerGenerated]
	private DelegateOpenDocument ergF58gtUV;

	[CompilerGenerated]
	private RemoveSelectedItemsDelegate eDfFSZJKUi;

	[CompilerGenerated]
	private DiffEvents.DiffExtractSelectedDelegate uYYFAoMGLM;

	public ITreeListService Service => GetService<ITreeListService>();

	public string ExtractDefaultPath => AppSetting.Instance.PvfConfig.ExtractConfig.TargetPath;

	public string CurrentSelectedTreePath
	{
		get
		{
			return GetProperty(() => CurrentSelectedTreePath);
		}
		set
		{
			SetProperty<string>(() => CurrentSelectedTreePath, value);
		}
	}

	public List<string> SearchComboBoxItems => new List<string>();

	public DataTemplate TreeColumnDataTemplate
	{
		[CompilerGenerated]
		get
		{
			return tdrFB7h60x;
		}
		[CompilerGenerated]
		set
		{
			tdrFB7h60x = value;
		}
	}

	public Style FileListRowStyle
	{
		[CompilerGenerated]
		get
		{
			return SBoFFfPLBX;
		}
		[CompilerGenerated]
		set
		{
			SBoFFfPLBX = value;
		}
	}

	public TreeViewType TreeType
	{
		[CompilerGenerated]
		get
		{
			return eJmFrvf0Dg;
		}
		[CompilerGenerated]
		set
		{
			eJmFrvf0Dg = value;
		}
	}

	public TreeGroup TreeGroupData
	{
		[CompilerGenerated]
		get
		{
			return whgFWeE87q;
		}
		[CompilerGenerated]
		set
		{
			whgFWeE87q = value;
		}
	}

	public bool ShowSelectionRectangle
	{
		get
		{
			return GetProperty(() => ShowSelectionRectangle);
		}
		set
		{
			SetProperty(() => ShowSelectionRectangle, value);
		}
	}

	public List<KeyValuePair<string, PvfTreeFileBase>> VisibleItems
	{
		[CompilerGenerated]
		get
		{
			return i5XF2iknTV;
		}
		[CompilerGenerated]
		set
		{
			i5XF2iknTV = value;
		}
	}

	public KeyValuePair<string, PvfTreeFileBase>? SelectedNodeBindgBase
	{
		get
		{
			return GetProperty(() => SelectedNodeBindgBase);
		}
		set
		{
			SetProperty<KeyValuePair<string, PvfTreeFileBase>?>(() => SelectedNodeBindgBase, value, sUhBXsk0yX);
		}
	}

	public ObservableCollection<KeyValuePair<string, PvfTreeFileBase>> SelectedNodesBindBase
	{
		get
		{
			return GetProperty(() => SelectedNodesBindBase);
		}
		set
		{
			SetProperty<ObservableCollection<KeyValuePair<string, PvfTreeFileBase>>>(() => SelectedNodesBindBase, value);
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

	public bool ForbidVerticalScrollBarAnnotation
	{
		get
		{
			return GetProperty(() => ForbidVerticalScrollBarAnnotation);
		}
		set
		{
			SetProperty(() => ForbidVerticalScrollBarAnnotation, value);
		}
	}

	public string SearchKeyword
	{
		get
		{
			return GetProperty(() => SearchKeyword);
		}
		set
		{
			SetProperty<string>(() => SearchKeyword, value, gWABz80lcj);
		}
	}

	public bool SearchPanelVisibility
	{
		get
		{
			return GetProperty(() => SearchPanelVisibility);
		}
		set
		{
			SetProperty(() => SearchPanelVisibility, value, DTsBNrVHPe);
		}
	}

	public List<TreeFileListSearchType> SearchTreeTypes => EnumberHelper.EnumToEnumList<TreeFileListSearchType>();

	public event DelegateDropFile EventDropFile
	{
		[CompilerGenerated]
		add
		{
			DelegateDropFile delegateDropFile = TevFvjfYjL;
			DelegateDropFile delegateDropFile2;
			do
			{
				delegateDropFile2 = delegateDropFile;
				DelegateDropFile value2 = (DelegateDropFile)Delegate.Combine(delegateDropFile2, value);
				delegateDropFile = Interlocked.CompareExchange(ref TevFvjfYjL, value2, delegateDropFile2);
			}
			while ((object)delegateDropFile != delegateDropFile2);
		}
		[CompilerGenerated]
		remove
		{
			DelegateDropFile delegateDropFile = TevFvjfYjL;
			DelegateDropFile delegateDropFile2;
			do
			{
				delegateDropFile2 = delegateDropFile;
				DelegateDropFile value2 = (DelegateDropFile)Delegate.Remove(delegateDropFile2, value);
				delegateDropFile = Interlocked.CompareExchange(ref TevFvjfYjL, value2, delegateDropFile2);
			}
			while ((object)delegateDropFile != delegateDropFile2);
		}
	}

	public event SelectedRowChangedDelegate SelectedRowChangedEvent
	{
		[CompilerGenerated]
		add
		{
			SelectedRowChangedDelegate selectedRowChangedDelegate = j5ZFmrSlL2;
			SelectedRowChangedDelegate selectedRowChangedDelegate2;
			do
			{
				selectedRowChangedDelegate2 = selectedRowChangedDelegate;
				SelectedRowChangedDelegate value2 = (SelectedRowChangedDelegate)Delegate.Combine(selectedRowChangedDelegate2, value);
				selectedRowChangedDelegate = Interlocked.CompareExchange(ref j5ZFmrSlL2, value2, selectedRowChangedDelegate2);
			}
			while ((object)selectedRowChangedDelegate != selectedRowChangedDelegate2);
		}
		[CompilerGenerated]
		remove
		{
			SelectedRowChangedDelegate selectedRowChangedDelegate = j5ZFmrSlL2;
			SelectedRowChangedDelegate selectedRowChangedDelegate2;
			do
			{
				selectedRowChangedDelegate2 = selectedRowChangedDelegate;
				SelectedRowChangedDelegate value2 = (SelectedRowChangedDelegate)Delegate.Remove(selectedRowChangedDelegate2, value);
				selectedRowChangedDelegate = Interlocked.CompareExchange(ref j5ZFmrSlL2, value2, selectedRowChangedDelegate2);
			}
			while ((object)selectedRowChangedDelegate != selectedRowChangedDelegate2);
		}
	}

	public event NodeDoubleClickDelegate EventNodeDoubleClick
	{
		[CompilerGenerated]
		add
		{
			NodeDoubleClickDelegate nodeDoubleClickDelegate = gNZFfonSBa;
			NodeDoubleClickDelegate nodeDoubleClickDelegate2;
			do
			{
				nodeDoubleClickDelegate2 = nodeDoubleClickDelegate;
				NodeDoubleClickDelegate value2 = (NodeDoubleClickDelegate)Delegate.Combine(nodeDoubleClickDelegate2, value);
				nodeDoubleClickDelegate = Interlocked.CompareExchange(ref gNZFfonSBa, value2, nodeDoubleClickDelegate2);
			}
			while ((object)nodeDoubleClickDelegate != nodeDoubleClickDelegate2);
		}
		[CompilerGenerated]
		remove
		{
			NodeDoubleClickDelegate nodeDoubleClickDelegate = gNZFfonSBa;
			NodeDoubleClickDelegate nodeDoubleClickDelegate2;
			do
			{
				nodeDoubleClickDelegate2 = nodeDoubleClickDelegate;
				NodeDoubleClickDelegate value2 = (NodeDoubleClickDelegate)Delegate.Remove(nodeDoubleClickDelegate2, value);
				nodeDoubleClickDelegate = Interlocked.CompareExchange(ref gNZFfonSBa, value2, nodeDoubleClickDelegate2);
			}
			while ((object)nodeDoubleClickDelegate != nodeDoubleClickDelegate2);
		}
	}

	public event DelegateOpenDocument EventOpenDocumenting
	{
		[CompilerGenerated]
		add
		{
			DelegateOpenDocument delegateOpenDocument = ergF58gtUV;
			DelegateOpenDocument delegateOpenDocument2;
			do
			{
				delegateOpenDocument2 = delegateOpenDocument;
				DelegateOpenDocument value2 = (DelegateOpenDocument)Delegate.Combine(delegateOpenDocument2, value);
				delegateOpenDocument = Interlocked.CompareExchange(ref ergF58gtUV, value2, delegateOpenDocument2);
			}
			while ((object)delegateOpenDocument != delegateOpenDocument2);
		}
		[CompilerGenerated]
		remove
		{
			DelegateOpenDocument delegateOpenDocument = ergF58gtUV;
			DelegateOpenDocument delegateOpenDocument2;
			do
			{
				delegateOpenDocument2 = delegateOpenDocument;
				DelegateOpenDocument value2 = (DelegateOpenDocument)Delegate.Remove(delegateOpenDocument2, value);
				delegateOpenDocument = Interlocked.CompareExchange(ref ergF58gtUV, value2, delegateOpenDocument2);
			}
			while ((object)delegateOpenDocument != delegateOpenDocument2);
		}
	}

	public event RemoveSelectedItemsDelegate RemoveSelectedItemsEvent
	{
		[CompilerGenerated]
		add
		{
			RemoveSelectedItemsDelegate removeSelectedItemsDelegate = eDfFSZJKUi;
			RemoveSelectedItemsDelegate removeSelectedItemsDelegate2;
			do
			{
				removeSelectedItemsDelegate2 = removeSelectedItemsDelegate;
				RemoveSelectedItemsDelegate value2 = (RemoveSelectedItemsDelegate)Delegate.Combine(removeSelectedItemsDelegate2, value);
				removeSelectedItemsDelegate = Interlocked.CompareExchange(ref eDfFSZJKUi, value2, removeSelectedItemsDelegate2);
			}
			while ((object)removeSelectedItemsDelegate != removeSelectedItemsDelegate2);
		}
		[CompilerGenerated]
		remove
		{
			RemoveSelectedItemsDelegate removeSelectedItemsDelegate = eDfFSZJKUi;
			RemoveSelectedItemsDelegate removeSelectedItemsDelegate2;
			do
			{
				removeSelectedItemsDelegate2 = removeSelectedItemsDelegate;
				RemoveSelectedItemsDelegate value2 = (RemoveSelectedItemsDelegate)Delegate.Remove(removeSelectedItemsDelegate2, value);
				removeSelectedItemsDelegate = Interlocked.CompareExchange(ref eDfFSZJKUi, value2, removeSelectedItemsDelegate2);
			}
			while ((object)removeSelectedItemsDelegate != removeSelectedItemsDelegate2);
		}
	}

	public event DiffEvents.DiffExtractSelectedDelegate EventDiffExtractSelected
	{
		[CompilerGenerated]
		add
		{
			DiffEvents.DiffExtractSelectedDelegate diffExtractSelectedDelegate = uYYFAoMGLM;
			DiffEvents.DiffExtractSelectedDelegate diffExtractSelectedDelegate2;
			do
			{
				diffExtractSelectedDelegate2 = diffExtractSelectedDelegate;
				DiffEvents.DiffExtractSelectedDelegate value2 = (DiffEvents.DiffExtractSelectedDelegate)Delegate.Combine(diffExtractSelectedDelegate2, value);
				diffExtractSelectedDelegate = Interlocked.CompareExchange(ref uYYFAoMGLM, value2, diffExtractSelectedDelegate2);
			}
			while ((object)diffExtractSelectedDelegate != diffExtractSelectedDelegate2);
		}
		[CompilerGenerated]
		remove
		{
			DiffEvents.DiffExtractSelectedDelegate diffExtractSelectedDelegate = uYYFAoMGLM;
			DiffEvents.DiffExtractSelectedDelegate diffExtractSelectedDelegate2;
			do
			{
				diffExtractSelectedDelegate2 = diffExtractSelectedDelegate;
				DiffEvents.DiffExtractSelectedDelegate value2 = (DiffEvents.DiffExtractSelectedDelegate)Delegate.Remove(diffExtractSelectedDelegate2, value);
				diffExtractSelectedDelegate = Interlocked.CompareExchange(ref uYYFAoMGLM, value2, diffExtractSelectedDelegate2);
			}
			while ((object)diffExtractSelectedDelegate != diffExtractSelectedDelegate2);
		}
	}

	public PvfTreeViewModel(TreeViewType treeType)
	{
		ShowSelectionRectangle = true;
		SearchKeyword = string.Empty;
		TreeType = treeType;
		TreeGroupData = new TreeGroup(treeType);
		SelectedNodesBindBase = new ObservableCollection<KeyValuePair<string, PvfTreeFileBase>>();
		if (TreeType == TreeViewType.SearchResult)
		{
			TreeColumnDataTemplate = (DataTemplate)Application.Current.MainWindow.FindResource("SearchResultPvfFileTreeColumnTemplate");
			FileListRowStyle = (Style)Application.Current.MainWindow.FindResource("FileListTreeRowStyle");
		}
		else if (TreeType == TreeViewType.ImportFiles)
		{
			TreeColumnDataTemplate = (DataTemplate)Application.Current.MainWindow.FindResource("ImportPvfFileColumnTemplate");
			FileListRowStyle = (Style)Application.Current.MainWindow.FindResource("FileListTreeRowStyle");
		}
		else if (TreeType == TreeViewType.PvfDiffLeft || treeType == TreeViewType.PvfDiffRight)
		{
			TreeColumnDataTemplate = (DataTemplate)Application.Current.MainWindow.FindResource("PvfDiffTreeColumnTemplate");
			FileListRowStyle = (Style)Application.Current.MainWindow.FindResource("DiffTreeRowStyle");
		}
		else
		{
			TreeColumnDataTemplate = (DataTemplate)Application.Current.MainWindow.FindResource("PvfFileTreeColumnTemplate");
			FileListRowStyle = (Style)Application.Current.MainWindow.FindResource("FileListTreeRowStyle");
		}
		TreeViewType treeType2 = TreeType;
		if (treeType2 == TreeViewType.FileList || treeType2 == TreeViewType.FileListDescription)
		{
			SearchPanelVisibility = true;
		}
	}

	public void Clear()
	{
		if (SelectedNodesBindBase != null)
		{
			SelectedNodesBindBase.Clear();
		}
		TreeGroupData.Clear();
		CurrentSelectedTreePath = string.Empty;
	}

	[SpecialName]
	private PvfTreeFileBase UtFFHl2Drj()
	{
		if (SelectedNodeBindgBase.HasValue)
		{
			return SelectedNodeBindgBase.Value.Value;
		}
		return null;
	}

	private void sUhBXsk0yX()
	{
		if (SelectedNodeBindgBase.HasValue)
		{
			j5ZFmrSlL2?.Invoke(SelectedNodeBindgBase.Value);
			CurrentSelectedTreePath = SelectedNodeBindgBase.Value.Value.FullPath;
		}
		else
		{
			CurrentSelectedTreePath = string.Empty;
		}
	}

	public PooledSet<string> GetSelectedFilePaths(GetTreeType type)
	{
		if (!IsSelectedNodes)
		{
			return null;
		}
		return TreeGroupData.SelectedNodesToFilePaths(SelectedNodesBindBase, type);
	}

	[Command]
	public void NodeDoubleClick(NodeClickArgs nodeClickArgs)
	{
		KeyValuePair<string, PvfTreeFileBase> row = (KeyValuePair<string, PvfTreeFileBase>)nodeClickArgs.Item;
		PvfTreeFileBase value = row.Value;
		gNZFfonSBa?.Invoke(row);
		if (value.IsFile)
		{
			switch (TreeType)
			{
			case TreeViewType.ImportFiles:
				d4OBpQ84tu(value);
				return;
			case TreeViewType.PvfDiffLeft:
			case TreeViewType.PvfDiffRight:
				return;
			}
			if (AppCore.ViewModelBase.PVF.PvfIsOpen)
			{
				HV0BUUJvdK(value);
			}
		}
		else
		{
			TreeListNode treeListNode = Service.ContentToNode(nodeClickArgs.Item);
			if (treeListNode != null)
			{
				treeListNode.IsExpanded = !treeListNode.IsExpanded;
			}
		}
	}

	private void d4OBpQ84tu(PvfTreeFileBase P_0)
	{
		if (ergF58gtUV != null)
		{
			ergF58gtUV(P_0);
		}
	}

	private void HV0BUUJvdK(PvfTreeFileBase P_0)
	{
		PvfFile file = AppCore.ViewModelBase.PVF.GetFile(P_0.FullPath);
		if (file == null)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), P_0.FullPath), isError: true);
			return;
		}
		AppCore.ViewModelBase.RootDocument.AddDocument(file);
		if (TreeType == TreeViewType.SearchResult)
		{
			PvfTreeViewModel pvfFileTreeViewModel = AppCore.ViewModelBase.PvfFileTreeViewModel;
			pvfFileTreeViewModel.GoToNode(pvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(P_0.FullPath));
		}
	}

	[Command]
	public void OnCollapseAllNodes(bool iscollapse)
	{
		if (!iscollapse)
		{
			Service.ExpandAllNodes();
		}
	}

	[Command]
	public void OnDeleteSelectedNodes()
	{
		try
		{
			if (!IsSelectedNodes)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFile"), isError: true);
				return;
			}
			PvfGroup pVF = AppCore.ViewModelBase.PVF;
			PooledSet<string> pooledSet = TreeGroupData.SelectedNodesToFilePaths(SelectedNodesBindBase, GetTreeType.File);
			if (AppCore.Logger.ShowDialog(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ConfirmDeleteSelectedFile"), pooledSet.Count)) == MessageResult.Yes)
			{
				if (TreeType == TreeViewType.SearchResult)
				{
					AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.DeleteTreeNode(TreeGroupData.SelectedNodesToFilePaths(SelectedNodesBindBase, GetTreeType.File));
				}
				AppCore.ViewModelBase.RootDocument.RemoveDocuments(pooledSet);
				pVF.DeleteFiles(pooledSet);
				TreeGroupData.DeleteTreeNodes(SelectedNodesBindBase);
				TreeGroupData.UpdateFileCount();
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnDeleteSelectedNodes");
		}
	}

	[Command]
	public void OnRemoveSelectedNodes()
	{
		if (!IsSelectedNodes)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFile"), isError: true);
		}
		else if (AppCore.Logger.ShowDialog(AppSetting.Instance.GetIlogger()?.GetStr("mess_ConfirmRemoveSelectedNode")) == MessageResult.Yes)
		{
			RemoveSelectedNodes();
		}
	}

	public void RemoveSelectedNodes()
	{
		try
		{
			if (TreeType == TreeViewType.SearchResult && eDfFSZJKUi != null)
			{
				eDfFSZJKUi(GetSelectedFilePaths(GetTreeType.File));
			}
			TreeGroupData.DeleteTreeNodes(SelectedNodesBindBase);
			TreeGroupData.UpdateFileCount();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.RemoveSelectedNodes");
		}
	}

	private void lSCBc70DGM()
	{
		if (SelectedNodesBindBase.Count == 1)
		{
			Service.ContentToNode(SelectedNodeBindgBase.Value);
		}
	}

	[Command]
	public void OnRenameNode()
	{
		if (UtFFHl2Drj() != null)
		{
			ViewRenameNode viewRenameNode = new ViewRenameNode(SelectedNodeBindgBase.Value);
			viewRenameNode.Owner = Application.Current.MainWindow;
			viewRenameNode.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			viewRenameNode.Show();
		}
	}

	[Command]
	public void OnViewReNmaeNodes()
	{
		if (IsSelectedNodes)
		{
			ViewReNmaeNodes viewReNmaeNodes = new ViewReNmaeNodes(new PooledList<string>(TreeGroupData.SelectedNodesToFilePaths(SelectedNodesBindBase, GetTreeType.File)));
			viewReNmaeNodes.Owner = Application.Current.MainWindow;
			viewReNmaeNodes.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			viewReNmaeNodes.Show();
		}
	}

	[Command]
	public void OnEditComment()
	{
		if (UtFFHl2Drj() == null)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFileToEditComment"));
			return;
		}
		ViewEditTreeComment viewEditTreeComment = new ViewEditTreeComment(UtFFHl2Drj());
		viewEditTreeComment.Owner = Application.Current.MainWindow;
		viewEditTreeComment.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		viewEditTreeComment.Show();
	}

	[Command]
	public void OnGoToFile()
	{
		ViewOpenPvfFileDocument viewOpenPvfFileDocument = new ViewOpenPvfFileDocument();
		viewOpenPvfFileDocument.Owner = Application.Current.MainWindow;
		viewOpenPvfFileDocument.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		viewOpenPvfFileDocument.Show();
	}

	[Command]
	public async void OnExtractSelectedsFiles(int type)
	{
		try
		{
			if (!IsSelectedNodes)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFileToExtract"), isError: true);
				return;
			}
			PooledSet<string> files = GetSelectedFilePaths(GetTreeType.File);
			if (files == null || files.Count == 0)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileToExtract"), isError: true);
				return;
			}
			switch (type)
			{
			case 1:
			{
				ExtractConfig extractConfig = AppSetting.Instance.PvfConfig.ExtractConfig.CloneData();
				extractConfig.ExtractTo7zip = false;
				extractConfig.TargetPath = AppCore.ViewModelBase.PVF.PvfPackDefaultExtractDir;
				extractConfig.SourceFiles = files.ToHashSet();
				await AppCore.ViewModelBase.PVF.ExtractFiles(extractConfig);
				break;
			}
			case 2:
				AppSetting.Instance.PvfConfig.ExtractConfig.ExtractTo7zip = true;
				break;
			}
			if (type != 1)
			{
				if (TreeType == TreeViewType.PvfDiffLeft || TreeType == TreeViewType.PvfDiffRight)
				{
					uYYFAoMGLM?.Invoke(TreeType, files);
				}
				else
				{
					AppCore.ViewModelBase.BarsVm.OnExtractFiles(files);
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnExtractSelectedsFiles");
		}
	}

	[Command]
	public async void OnExtractToLstItems()
	{
		try
		{
			_003C_003Ec__DisplayClass82_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass82_0();
			if (!IsSelectedNodes)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFile"), isError: true);
				return;
			}
			PooledSet<string> selectedFilePaths = GetSelectedFilePaths(GetTreeType.File);
			if (selectedFilePaths == null || selectedFilePaths.Count == 0)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFile"), isError: true);
				return;
			}
			List<PvfFile> files = AppCore.ViewModelBase.PVF.GetFiles(selectedFilePaths);
			if (files == null || files.Count == 0)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileToExtractToLst"), isError: true);
				return;
			}
			CS_0024_003C_003E8__locals4.hlsLvI1aeU = ServiceItemCodeTable.FilesToLstItemsToString(AppCore.ViewModelBase.PVF, files, out var count);
			if (string.IsNullOrEmpty(CS_0024_003C_003E8__locals4.hlsLvI1aeU))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileToExtractToLst"), isError: true);
				return;
			}
			AppCore.CopyString(CS_0024_003C_003E8__locals4.hlsLvI1aeU);
			string text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExtractedLstCount"), count);
			AppCore.Logger.Success(text);
			await AppCore.Logger.ShowNotification(new NotificationViewModel<string>(AppSetting.Instance.AppName, text, Res.Instance.VisualStudioBlendLogo2015Pre_16x, AppSetting.Instance.GetIlogger()?.GetStr("mess_ViewDetails"), new DelegateCommand<string>(delegate
			{
				AppCore.ShowExtractLstWindow(CS_0024_003C_003E8__locals4.hlsLvI1aeU);
			})));
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnExtractSelectedsFiles");
		}
	}

	[Command]
	public void OnGoTreeNode()
	{
		if (UtFFHl2Drj() != null)
		{
			AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(UtFFHl2Drj().FullPath);
		}
	}

	[Command]
	public async void OnImportFiles(bool isSelectPath)
	{
		try
		{
			PvfTreeFileBase pvfTreeFileBase = UtFFHl2Drj();
			if (pvfTreeFileBase == null)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFile"));
				return;
			}
			string tarGetPath = ((!pvfTreeFileBase.IsFile) ? pvfTreeFileBase.FullPath : Path.GetDirectoryName(pvfTreeFileBase.FullPath)?.Replace('\\', '/'));
			if (!isSelectPath)
			{
				AppCore.ViewModelBase.BarsVm.ShowImportFiles(tarGetPath);
				return;
			}
			CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
			{
				Title = AppSetting.Instance.GetIlogger().GetStr("mess_SelectFileToImport"),
				IsFolderPicker = false,
				Multiselect = true,
				AllowPropertyEditing = true,
				EnsurePathExists = true,
				EnsureValidNames = true
			};
			if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok)
			{
				return;
			}
			IReadOnlyList<string> files = commonOpenFileDialog.FileNames;
			AppSetting.Instance.PvfConfig.ImportConfig.TargetPath = tarGetPath;
			await AppSetting.Instance.PvfConfig.ImportConfig.DiskFileListToImportItems(files.ToList());
			HashSet<ImportFileItem> sourceFiles = AppSetting.Instance.PvfConfig.ImportConfig.SourceFiles;
			if (sourceFiles == null || sourceFiles.Count == 0)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileToImport"), isError: true);
				return;
			}
			List<string> list = files.ToList();
			string arg = list[0].Remove(list[0].LastIndexOf('\\'));
			ImportFilesOptionsDialog importFilesOptionsDialog = new ImportFilesOptionsDialog(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportConfirm"), arg, sourceFiles.Count, tarGetPath))
			{
				WindowStartupLocation = WindowStartupLocation.CenterScreen
			};
			bool? flag = importFilesOptionsDialog.ShowDialog();
			if (flag.HasValue && flag.Value)
			{
				if (importFilesOptionsDialog.DVqhc7IK51)
				{
					AppCore.ViewModelBase.BarsVm.ShowImportFiles(tarGetPath, sourceFiles);
				}
				else
				{
					await AppCore.ViewModelBase.PVF.ImportFiles(AppSetting.Instance.PvfConfig.ImportConfig, is7z: false);
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnImportFiles");
		}
	}

	[Command]
	public void OnOpenDiffWindow(bool isLeft)
	{
		try
		{
			PvfTreeFileBase pvfTreeFileBase = UtFFHl2Drj();
			if (pvfTreeFileBase == null)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFile"));
				return;
			}
			if (!pvfTreeFileBase.IsFile)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFileNotFolder"));
				return;
			}
			DiffSource source = null;
			switch (TreeType)
			{
			case TreeViewType.FileList:
			case TreeViewType.SearchResult:
			case TreeViewType.ExtractFiles:
				source = new DiffSource(AppCore.ViewModelBase.PVF, pvfTreeFileBase.FullPath);
				break;
			case TreeViewType.ImportFiles:
			{
				PvfTreeFileImport pvfTreeFileImport = (PvfTreeFileImport)pvfTreeFileBase;
				if (pvfTreeFileImport.ImportItem.IndexForm7zip.HasValue)
				{
					if (AppCore.ViewModelBase.RootDocument.GetDocument(PvfFileDocumentType.导入文件.ToString()) is ViewImportFilesViewModel viewImportFilesViewModel)
					{
						string filePathFrom7zip = viewImportFilesViewModel.FilePathFrom7zip;
						source = new DiffSource(pvfTreeFileImport.ImportItem.FullPath, filePathFrom7zip);
					}
				}
				else
				{
					source = new DiffSource(pvfTreeFileImport.ImportItem.FullPath);
				}
				break;
			}
			}
			AppCore.ViewModelBase.BarsVm.OpenDiffWindow(isLeft, source);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnOpenDiffWindow");
		}
	}

	[Command]
	public async void OnGetSelectedFileItemCode()
	{
		try
		{
			if (!IsSelectedNodes)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFile"));
				return;
			}
			PooledSet<string> pooledSet = TreeGroupData.SelectedNodesToFilePaths(SelectedNodesBindBase, GetTreeType.File);
			if (pooledSet.Count == 0)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileToGetCode"));
				return;
			}
			List<int> list = AppCore.ViewModelBase.PVF.GetItemCodes(pooledSet).ToList();
			if (AppSetting.Instance.TreeSetting.GetItemCodeSort)
			{
				list.Sort();
			}
			AppCore.CopyString(string.Join('\t', list));
			string text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_GetCodeSuccess"), list.Count);
			AppCore.Logger.Success(text);
			await AppCore.Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, text, Res.Instance.VisualStudioBlendLogo2015Pre_16x));
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnGetSelectedFileItemCode");
		}
	}

	[Command]
	public async void OnGetSelectedFileItemNameAndItemCode()
	{
		try
		{
			if (!IsSelectedNodes)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFile"));
				return;
			}
			PooledSet<string> pooledSet = TreeGroupData.SelectedNodesToFilePaths(SelectedNodesBindBase, GetTreeType.File);
			if (pooledSet.Count == 0)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileToGetCode"));
				return;
			}
			int outCount = 0;
			string itemCodeAndItemNames = AppCore.ViewModelBase.PVF.GetItemCodeAndItemNames(pooledSet, out outCount);
			AppCore.CopyString(string.Join('\t', itemCodeAndItemNames));
			string text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_GetNameAndCodeSuccess"), outCount);
			AppCore.Logger.Success(text);
			await AppCore.Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, text, Res.Instance.VisualStudioBlendLogo2015Pre_16x));
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnGetSelectedFileItemNameAndItemCode");
		}
	}

	[Command]
	public async void OnGetSelectedFileFullPathndItemCode()
	{
		try
		{
			if (!IsSelectedNodes)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFile"));
				return;
			}
			PooledSet<string> pooledSet = TreeGroupData.SelectedNodesToFilePaths(SelectedNodesBindBase, GetTreeType.File);
			if (pooledSet.Count == 0)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileToGetCode"));
				return;
			}
			List<PvfFile> files = AppCore.ViewModelBase.PVF.GetFiles(pooledSet);
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (PvfFile item in files)
			{
				if (item.ItemCode.HasValue)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
					handler.AppendFormatted(item.FileName);
					handler.AppendLiteral("\t");
					handler.AppendFormatted(item.ItemCode);
					stringBuilder2.AppendLine(ref handler);
					num++;
				}
			}
			AppCore.CopyString(string.Join('\t', stringBuilder));
			string text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_GetPathAndNameSuccess"), num);
			AppCore.Logger.Success(text);
			await AppCore.Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, text, Res.Instance.VisualStudioBlendLogo2015Pre_16x));
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnGetSelectedFileFullPathndItemCode");
		}
	}

	[Command]
	public void OnAddBookMark()
	{
		try
		{
			if (UtFFHl2Drj() == null)
			{
				return;
			}
			string fullPath = UtFFHl2Drj().FullPath;
			string text = AppCore.ViewModelBase.PVF.GetItemName(fullPath);
			if (string.IsNullOrEmpty(text))
			{
				text = AppSetting.Instance.GetIlogger()?.GetStr("NewBookMarkFileName");
			}
			EditBookmarkView editBookmarkView = new EditBookmarkView(new KeyValuePair<string, BookMarkDto>(text, new BookMarkDto
			{
				IsFile = true,
				FilePath = fullPath
			}), isAdd: true)
			{
				Owner = Application.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterScreen
			};
			if (editBookmarkView.ShowDialog().Value)
			{
				KeyValuePair<string, BookMarkDto> row = editBookmarkView.Row;
				BookMarkEditView bookMarkEditView = new BookMarkEditView(isTreeList: true);
				bookMarkEditView.Owner = Application.Current.MainWindow;
				bookMarkEditView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				bookMarkEditView.ShowDialog();
				BookMarkEditViewViewModel bookMarkEditViewViewModel = (BookMarkEditViewViewModel)bookMarkEditView.DataContext;
				if (bookMarkEditViewViewModel.TreeGetSource != null)
				{
					AppSetting.Instance.BookMarkGroup.AddNode(row.Key, row.Value, bookMarkEditViewViewModel.TreeGetSource);
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnAddBookMark");
		}
	}

	[Command]
	public void OnBatchOperation()
	{
		try
		{
			PooledSet<string> selectedFilePaths = GetSelectedFilePaths(GetTreeType.File);
			if (selectedFilePaths == null || selectedFilePaths.Count == 0)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFile"));
				return;
			}
			BatchOperationView batchOperationView = new BatchOperationView();
			batchOperationView.Owner = (AppSetting.Instance.ChildWindowAttachmentMainWindow ? Application.Current.MainWindow : null);
			batchOperationView.WindowStartupLocation = ((!AppSetting.Instance.ChildWindowAttachmentMainWindow) ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner);
			batchOperationView.Show();
			batchOperationView.GetVm().AddFiles(selectedFilePaths);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnBatchOperation");
		}
	}

	[Command]
	public async void OnAddNewPvfFile()
	{
		try
		{
			if (UtFFHl2Drj() == null)
			{
				return;
			}
			string rootPath;
			if (UtFFHl2Drj().IsFile)
			{
				if (UtFFHl2Drj().Level == 0)
				{
					_ = TreeGroupData.Trees;
					rootPath = null;
				}
				else
				{
					KeyValuePair<string, PvfTreeFileBase>? parentNodeContent = GetParentNodeContent(SelectedNodeBindgBase.Value);
					if (!parentNodeContent.HasValue)
					{
						AppCore.Logger.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_CurrentNodeHasNoParent"));
						return;
					}
					_ = parentNodeContent.Value.Value.Children;
					rootPath = parentNodeContent.Value.Value.FullPath;
				}
			}
			else
			{
				_ = UtFFHl2Drj().Children;
				rootPath = UtFFHl2Drj().FullPath;
			}
			WinAddNewPvfFile winAddNewPvfFile = new WinAddNewPvfFile(rootPath)
			{
				Owner = Application.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterScreen
			};
			if (!winAddNewPvfFile.ShowDialog().Value)
			{
				return;
			}
			string text = winAddNewPvfFile.FullPpath.ToLower();
			PvfFile file = new PvfFile
			{
				FileName = text
			};
			if (AppCore.ViewModelBase.PVF.FileAny(file.FileName))
			{
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileAlreadyExists_2"), file.FileName));
				return;
			}
			bool newFileRegLstFile = winAddNewPvfFile.NewFileRegLstFile;
			AppCore.ViewModelBase.PVF.FileList.Add(file.FileName, file);
			if (newFileRegLstFile)
			{
				ResultData resultData = AppCore.ViewModelBase.PVF.RegLst(text);
				if (resultData.IsError)
				{
					AppCore.Logger.Error(resultData.Msg);
				}
			}
			await TreeGroupData.CreateTrees(new PooledList<string> { text });
			await Task.Delay(1);
			AppCore.ViewModelBase.RootDocument.AddDocument(file.FileName, gotoNode: true);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnAddNewPvfFile");
		}
	}

	[Command]
	public void OnAddNewFolder(KeyValuePair<string, PvfTreeFileBase> row)
	{
		KeyValuePair<string, PvfTreeFileBase>? keyValuePair = null;
		if (row.Value.IsFile)
		{
			if (row.Value.Level != 0)
			{
				TreeListNode treeListNode = Service.ContentToNode(row);
				keyValuePair = zETFl4aec5(treeListNode.ParentNode.Content);
			}
		}
		else
		{
			keyValuePair = row;
		}
		ViewAddNewFolder viewAddNewFolder = new ViewAddNewFolder(keyValuePair);
		viewAddNewFolder.Owner = Application.Current.MainWindow;
		viewAddNewFolder.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		viewAddNewFolder.Show();
	}

	[Command]
	public void OnCopyFiles(int type)
	{
		switch (type)
		{
		case 0:
			aU7B8iW8Zq();
			break;
		case 1:
			mAtBMgerKR();
			break;
		case 2:
			BwHBVswmDu();
			break;
		}
	}

	[Command]
	public async void OnSelectedItemsAddToSearchResultPanel()
	{
		try
		{
			if (IsSelectedNodes)
			{
				PooledSet<string> selectedFilePaths = GetSelectedFilePaths(GetTreeType.File);
				AppCore.ViewModelBase.SearchResultViewModel.SearchReusltData[AppCore.ViewModelBase.SearchResultViewModel.SelectedItem].FileList.AddRange(selectedFilePaths);
				await AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(selectedFilePaths));
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnSelectedItemsAddToSearchResultPanel");
		}
	}

	[Command]
	public void OnOpenLstTools()
	{
		try
		{
			if (UtFFHl2Drj() == null)
			{
				return;
			}
			string fullPath = UtFFHl2Drj().FullPath;
			KeyValuePair<string, string>? selectedItem = null;
			foreach (KeyValuePair<string, string> lstFilePath in AppCore.ViewModelBase.PVF.ListFileTable.LstFilePaths)
			{
				if (lstFilePath.Value == fullPath)
				{
					selectedItem = lstFilePath;
					break;
				}
			}
			WinLstTools winLstTools = new WinLstTools(selectedItem);
			winLstTools.Owner = (AppSetting.Instance.ChildWindowAttachmentMainWindow ? Application.Current.MainWindow : null);
			winLstTools.WindowStartupLocation = ((!AppSetting.Instance.ChildWindowAttachmentMainWindow) ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner);
			winLstTools.Show();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.OnOpenLstTools");
		}
	}

	[Command]
	public void OnOpenViewFileAttributes()
	{
		if (UtFFHl2Drj() != null)
		{
			WinPvfFileAttributes winPvfFileAttributes = new WinPvfFileAttributes(UtFFHl2Drj());
			winPvfFileAttributes.Owner = Application.Current.MainWindow;
			winPvfFileAttributes.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			winPvfFileAttributes.Show();
		}
	}

	private async void aU7B8iW8Zq()
	{
		if (!IsSelectedNodes)
		{
			return;
		}
		try
		{
			await qjqilnF7lAFbCxZ5lIf.Instance.D8FFUxEc3P();
			WindowLoading loading = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_Cutting"), Application.Current.MainWindow);
			loading.Show();
			await qjqilnF7lAFbCxZ5lIf.Instance.Pi8FpQMdM7(SelectedNodesBindBase, TreeFileCopyStatus.剪切, TreeType);
			loading.Close();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.ShearFiles");
		}
	}

	private async void mAtBMgerKR()
	{
		if (!IsSelectedNodes)
		{
			return;
		}
		try
		{
			await qjqilnF7lAFbCxZ5lIf.Instance.D8FFUxEc3P();
			WindowLoading loading = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_Copying"), Application.Current.MainWindow);
			loading.Show();
			await qjqilnF7lAFbCxZ5lIf.Instance.Pi8FpQMdM7(SelectedNodesBindBase, TreeFileCopyStatus.复制, TreeType);
			loading.Close();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.CopyFiles");
		}
	}

	private void BwHBVswmDu()
	{
		try
		{
			if (UtFFHl2Drj() == null)
			{
				return;
			}
			string text;
			if (UtFFHl2Drj().IsFile)
			{
				if (UtFFHl2Drj().Level == 0)
				{
					text = null;
				}
				else
				{
					KeyValuePair<string, PvfTreeFileBase>? parentNodeContent = GetParentNodeContent(SelectedNodeBindgBase.Value);
					if (!parentNodeContent.HasValue)
					{
						AppCore.Logger.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_CurrentNodeHasNoParent"));
						return;
					}
					text = parentNodeContent.Value.Value.FullPath;
				}
			}
			else
			{
				text = UtFFHl2Drj().FullPath;
			}
			qjqilnF7lAFbCxZ5lIf.Instance.ckWFcDK6M8(text);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.PastFilese");
		}
	}

	public void GoToFirstOrDefault()
	{
		if (TreeGroupData.Trees != null && TreeGroupData.Trees.Any())
		{
			TreeListNode treeListNode = Service.FirstOrDefaultNode();
			if (treeListNode != null)
			{
				GoToNode(zETFl4aec5(treeListNode.Content));
			}
		}
	}

	public void GoToNode(string filePath, bool showError = true)
	{
		try
		{
			if (filePath != null)
			{
				ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> source = (TreeGroupData.ShowSearchResulTrees ? TreeGroupData._SearchResultTrees : TreeGroupData._Trees);
				KeyValuePair<string, PvfTreeFileBase>? row = TreeGroupData.FilePathGetTreeNode(filePath, source);
				if (row.HasValue)
				{
					GoToNode(row);
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.GoToNode");
		}
	}

	public KeyValuePair<string, PvfTreeFileBase>? GetParentNodeContent(KeyValuePair<string, PvfTreeFileBase> nowRow)
	{
		TreeListNode parentNode = Service.ContentToNode(nowRow).ParentNode;
		if (parentNode == null)
		{
			return null;
		}
		return (KeyValuePair<string, PvfTreeFileBase>)parentNode.Content;
	}

	public void GoToNode(KeyValuePair<string, PvfTreeFileBase>? row)
	{
		_003C_003Ec__DisplayClass104_0 CS_0024_003C_003E8__locals16 = new _003C_003Ec__DisplayClass104_0();
		CS_0024_003C_003E8__locals16.WBks8XnqmV = this;
		CS_0024_003C_003E8__locals16.UYOsMBjXf8 = row;
		if (!CS_0024_003C_003E8__locals16.UYOsMBjXf8.HasValue)
		{
			return;
		}
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)async delegate
		{
			TreeListNode treeListNode = CS_0024_003C_003E8__locals16.WBks8XnqmV.Service.ContentToNode(CS_0024_003C_003E8__locals16.UYOsMBjXf8.Value);
			CS_0024_003C_003E8__locals16.WBks8XnqmV.SelectedNodesBindBase = null;
			CS_0024_003C_003E8__locals16.WBks8XnqmV.SelectedNodesBindBase = new ObservableCollection<KeyValuePair<string, PvfTreeFileBase>>();
			if (treeListNode == null)
			{
				CS_0024_003C_003E8__locals16.WBks8XnqmV.YOUB3nq5qd(CS_0024_003C_003E8__locals16.UYOsMBjXf8.Value.Value.FullPath);
				treeListNode = CS_0024_003C_003E8__locals16.WBks8XnqmV.Service.ContentToNode(CS_0024_003C_003E8__locals16.UYOsMBjXf8.Value);
			}
			CS_0024_003C_003E8__locals16.WBks8XnqmV.Service.SetFocusableNode(treeListNode);
			CS_0024_003C_003E8__locals16.WBks8XnqmV.SelectedNodeBindgBase = CS_0024_003C_003E8__locals16.UYOsMBjXf8;
			CS_0024_003C_003E8__locals16.WBks8XnqmV.SelectedNodesBindBase.Add(CS_0024_003C_003E8__locals16.UYOsMBjXf8.Value);
		}, Array.Empty<object>());
	}

	private void YOUB3nq5qd(string P_0)
	{
		List<KeyValuePair<string, PvfTreeFileBase>> list = new List<KeyValuePair<string, PvfTreeFileBase>>();
		string[] array = P_0.Split('/');
		string text = null;
		for (int i = 0; i < array.Length; i++)
		{
			text = ((i != 0) ? (text + "/" + array[i]) : array[i]);
			KeyValuePair<string, PvfTreeFileBase>? keyValuePair = TreeGroupData.FilePathGetTreeNode(text);
			if (keyValuePair.HasValue)
			{
				list.Add(keyValuePair.Value);
			}
		}
		foreach (KeyValuePair<string, PvfTreeFileBase> item in list)
		{
			if (!item.Value.IsFile)
			{
				TreeListNode treeListNode = Service.ContentToNode(item);
				if (treeListNode != null)
				{
					treeListNode.IsExpanded = true;
				}
			}
		}
	}

	public void OnEventDropFile(IEnumerable<string> files)
	{
		switch (TreeType)
		{
		case TreeViewType.FileList:
			if (files != null && Path.GetExtension(files.ToList()[0]).ToLower() == ".pvf")
			{
				AppCore.ViewModelBase.BarsVm.OnOpenPvfFile(files.ToList()[0]);
			}
			break;
		default:
			TevFvjfYjL?.Invoke(files);
			break;
		case TreeViewType.SearchResult:
			break;
		}
	}

	public void TreeListView_ScrollBarCustomRowAnnotation(object sender, ScrollBarCustomRowAnnotationEventArgs e)
	{
		if (!ForbidVerticalScrollBarAnnotation)
		{
			TreeViewType treeType = TreeType;
			if ((uint)(treeType - 8) <= 1u)
			{
				gtxBR5x09e(e);
			}
		}
	}

	private void gtxBR5x09e(ScrollBarCustomRowAnnotationEventArgs P_0)
	{
		try
		{
			PvfTreeFileDiff pvfTreeFileDiff = (PvfTreeFileDiff)((KeyValuePair<string, PvfTreeFileBase>)P_0.Row).Value;
			if (pvfTreeFileDiff.Diffs != null && pvfTreeFileDiff.Diffs.Count > 0)
			{
				ScrollBarAnnotationAlignment alignment = ScrollBarAnnotationAlignment.Left;
				SolidColorBrush brush;
				if (pvfTreeFileDiff.Diffs.Contains(PvfFileDiffType.FilePath))
				{
					brush = (SolidColorBrush)Application.Current.TryFindResource("TreeDiffScrollBarCustomRowAnnotationBrush");
					alignment = ScrollBarAnnotationAlignment.Right;
				}
				else
				{
					brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b9d7ac"));
				}
				P_0.ScrollBarAnnotationInfo = new ScrollBarAnnotationInfo
				{
					Alignment = alignment,
					Brush = brush,
					MinHeight = 0.5,
					Width = 6.0
				};
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.DiffScrollBarCustomRowAnnotation");
		}
	}

	public void NodeExpanded(object sender, TreeListNodeEventArgs e)
	{
	}

	private void DTsBNrVHPe()
	{
		if (!SearchPanelVisibility)
		{
			TreeGroupData.ClearSearchResult();
		}
	}

	private void gWABz80lcj()
	{
		if (TreeGroupData != null)
		{
			if (string.IsNullOrEmpty(SearchKeyword))
			{
				TreeGroupData.ClearSearchResult();
				return;
			}
			TreeGroupData._SearchResultTrees = null;
			TreeGroupData.ShowSearchResulTrees = false;
			TreeGroupData.DoNotify("Trees");
		}
	}

	[Command]
	public void OnSearchTreeFileList()
	{
		l0JFDclKyA();
	}

	private async void l0JFDclKyA()
	{
		_003C_003Ec__DisplayClass124_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass124_0();
		CS_0024_003C_003E8__locals5.XbNs3gj9Fn = this;
		CS_0024_003C_003E8__locals5.ytUsRkM01r = SearchKeyword;
		if (!string.IsNullOrEmpty(CS_0024_003C_003E8__locals5.ytUsRkM01r))
		{
			TreeGroupData.Loading = true;
			TreeGroupData._SearchResultTrees = null;
			TreeGroupData._SearchResultTrees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
			int num = await Task.Run(() => CS_0024_003C_003E8__locals5.XbNs3gj9Fn.TreeGroupData.SearchFileList(CS_0024_003C_003E8__locals5.ytUsRkM01r));
			TreeGroupData.Loading = false;
			if (num > 0)
			{
				GoToFirstOrDefault();
			}
		}
	}

	[Command]
	public void ClearSearchKeyword()
	{
		SearchKeyword = string.Empty;
	}

	[Command]
	public void OnShowSearchPanel(bool vis)
	{
		SearchPanelVisibility = !SearchPanelVisibility;
		SearchPanelVisibility = vis;
	}

	public async Task<bool> WebApiImportFile(Stream stream, string filePath)
	{
		_003C_003Ec__DisplayClass127_0 CS_0024_003C_003E8__locals11 = new _003C_003Ec__DisplayClass127_0();
		CS_0024_003C_003E8__locals11.PyXszAmjjb = this;
		CS_0024_003C_003E8__locals11.QX5LDqIyPJ = filePath;
		CS_0024_003C_003E8__locals11.QX5LDqIyPJ = CS_0024_003C_003E8__locals11.QX5LDqIyPJ.ToLower();
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		if (pVF.FileAny(CS_0024_003C_003E8__locals11.QX5LDqIyPJ))
		{
			return pVF.ImportUpdateFile(pVF.GetFile(CS_0024_003C_003E8__locals11.QX5LDqIyPJ), stream, CS_0024_003C_003E8__locals11.QX5LDqIyPJ, compileScript: true, compileBinaryAni: true, convertChinese: false);
		}
		if (pVF.ImportNewFile(CS_0024_003C_003E8__locals11.QX5LDqIyPJ, stream, compileScript: true, compileBinaryAni: true, convertChinese: false))
		{
			await ((DispatcherObject)Application.Current).Dispatcher.Invoke<Task>((Func<Task>)async delegate
			{
				await CS_0024_003C_003E8__locals11.PyXszAmjjb.TreeGroupData.CreateTrees(new PooledList<string> { CS_0024_003C_003E8__locals11.QX5LDqIyPJ });
			});
			return true;
		}
		return false;
	}

	public async Task<ResultData<IEnumerable<string>>> WebApiImportFiles(IEnumerable<ImportFileRes> fileDataList)
	{
		_003C_003Ec__DisplayClass128_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass128_0();
		CS_0024_003C_003E8__locals6.NBMLjBXN4r = this;
		ResultData<IEnumerable<string>> result = new ResultData<IEnumerable<string>>();
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		try
		{
			List<string> list = new List<string>();
			if (!pVF.PvfIsOpen)
			{
				result.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_PleaseLoadPvfPackFirst");
				return result;
			}
			if (fileDataList == null || !fileDataList.Any())
			{
				result.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_ImportListCannotBeEmpty");
				return result;
			}
			CS_0024_003C_003E8__locals6.I2bLTVohwZ = new List<string>();
			foreach (ImportFileRes fileData in fileDataList)
			{
				Stream stream = BytesHelper.StringToStream(fileData.FileContent);
				if (pVF.FileAny(fileData.FilePath))
				{
					if (!pVF.ImportUpdateFile(pVF.GetFile(fileData.FilePath), stream, fileData.FilePath, compileScript: true, compileBinaryAni: true, convertChinese: false))
					{
						list.Add(fileData.FilePath);
					}
				}
				else if (pVF.ImportNewFile(fileData.FilePath, stream, compileScript: true, compileBinaryAni: true, convertChinese: false))
				{
					CS_0024_003C_003E8__locals6.I2bLTVohwZ.Add(fileData.FilePath);
				}
				else
				{
					list.Add(fileData.FilePath);
				}
			}
			result.Data = list;
			if (CS_0024_003C_003E8__locals6.I2bLTVohwZ.Count > 0)
			{
				await ((DispatcherObject)Application.Current).Dispatcher.Invoke<Task>((Func<Task>)async delegate
				{
					await CS_0024_003C_003E8__locals6.NBMLjBXN4r.TreeGroupData.CreateTrees(new PooledList<string>(CS_0024_003C_003E8__locals6.I2bLTVohwZ));
				});
			}
		}
		catch (Exception ex)
		{
			result.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BatchImportError"), ex.Message);
		}
		return result;
	}

	private KeyValuePair<string, PvfTreeFileBase>? zETFl4aec5(object P_0)
	{
		if (P_0 == null)
		{
			return null;
		}
		return (KeyValuePair<string, PvfTreeFileBase>)P_0;
	}

	public async void TreeList_Drop(object sender, DragEventArgs e)
	{
		if (e.Data.GetData(DataFormats.FileDrop) != null)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			OnEventDropFile(files);
			return;
		}
		string text = (string)e.Data.GetData(typeof(string));
		if (text != null && text == "TreeListDropGroup：15427586-86B6-5410-5D88-7F139C0C1E9E" && TreeListDropGroup.Instance.Source != TreeType)
		{
			TreeViewType treeType = TreeType;
			if ((treeType == TreeViewType.SearchResult || treeType == TreeViewType.ExtractFiles || treeType == TreeViewType.BatchOperation) && TreeListDropGroup.Instance.Source != TreeType)
			{
				PooledSet<string> fileList = TreeListDropGroup.Instance.GetFilePaths();
				TreeListDropGroup.Instance.Success = true;
				await TreeGroupData.CreateTrees(fileList.ToPooledList());
				fileList.Dispose();
			}
		}
	}

	public void TreeListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		TreeListView dragSource = (TreeListView)sender;
		if (((int)Keyboard.Modifiers & 2) == 2)
		{
			TreeListDropGroup.Instance.SourceViewModel = this;
			TreeListDropGroup.Instance.Source = TreeType;
			TreeListDropGroup.Instance.Items = SelectedNodesBindBase;
			DragDrop.DoDragDrop((DependencyObject)(object)dragSource, "TreeListDropGroup：15427586-86B6-5410-5D88-7F139C0C1E9E", DragDropEffects.Copy);
			if (!TreeListDropGroup.Instance.Success && UtFFHl2Drj() != null && Service.ContentToNode(SelectedNodeBindgBase) != null)
			{
				if (!SelectedNodesBindBase.Contains(SelectedNodeBindgBase.Value))
				{
					SelectedNodesBindBase.Add(SelectedNodeBindgBase.Value);
				}
				else
				{
					SelectedNodesBindBase.Remove(SelectedNodeBindgBase.Value);
				}
			}
			TreeListDropGroup.Clear();
		}
		else
		{
			_ = (int)Keyboard.Modifiers & 1;
		}
	}

	public void TreeListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
	}

	[Command]
	public void OnCreateShopPackage()
	{
		PooledSet<string> selectedFilePaths = GetSelectedFilePaths(GetTreeType.File);
		if (selectedFilePaths != null && selectedFilePaths.Any())
		{
			CreateShopItem createShopItem = new CreateShopItem(selectedFilePaths.ToList());
			createShopItem.Owner = Application.Current.MainWindow;
			createShopItem.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			createShopItem.Show();
		}
	}

	[Command]
	public void OnGoToDocument(string filePath)
	{
		if (AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			AppCore.ViewModelBase.RootDocument.AddDocument(filePath, gotoNode: true);
		}
		else
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseLoadPvfPackFirst"));
		}
	}

	[Command]
	public void CustomColumnSort(NodeSortArgs args)
	{
	}

	[Command]
	public async void OnSendPostal()
	{
		_003C_003Ec__DisplayClass137_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass137_0();
		if (!IsSelectedNodes)
		{
			AppCore.ShowMsg("请先选中要发送的物品");
			return;
		}
		CS_0024_003C_003E8__locals4.UYyLH5sARl = abrFTjlTIu();
		if (CS_0024_003C_003E8__locals4.UYyLH5sARl.IsError)
		{
			AppCore.ShowMsg(CS_0024_003C_003E8__locals4.UYyLH5sARl.Msg);
			return;
		}
		await Task.Run(async delegate
		{
			DnfSqlService dnfSqlService = new DnfSqlService(AppSetting.Instance.GameOptions.GameServerOptions.GetDb());
			List<CharacInfoDto> list = await dnfSqlService.FindCharac(new FindUserDto
			{
				FindUserType = FindUserType.OnLineCharac
			});
			if (list == null || list.Count == 0)
			{
				AppCore.ShowMsg("没有在线角色无法发送");
			}
			else
			{
				ResultData<string> resultData = await dnfSqlService.SendPostal(list, CS_0024_003C_003E8__locals4.UYyLH5sARl.Data, AppSetting.Instance.GMToolOptions.PostalSendTitle, AppSetting.Instance.GMToolOptions.PostalSendText);
				AppCore.Logger.Warning("邮件发送回调：" + resultData.Data);
			}
		});
	}

	private ResultData<List<PostalSendRes>> abrFTjlTIu()
	{
		ResultData<List<PostalSendRes>> resultData = new ResultData<List<PostalSendRes>>();
		IEnumerable<PvfFile> enumerable = from it in SelectedNodesBindBase
			where it.Value.File != null
			where it.Value.File.ItemCode.HasValue
			where it.Value.File.FileType == PvfFileType.equ || it.Value.File.FileType == PvfFileType.stk
			select it.Value.File;
		if (enumerable == null)
		{
			resultData.Msg = "没有可以发送到游戏邮件的物品";
			return resultData;
		}
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		List<PostalSendRes> list = new List<PostalSendRes>();
		foreach (PvfFile item in enumerable)
		{
			PostalSendRes postalSendRes = new PostalSendRes
			{
				item_id = item.ItemCode.Value,
				IsEqu = (item.FileType == PvfFileType.equ),
				ItemName = pVF.GetItemName(item),
				add_info = 1
			};
			EDWFCojmVG(postalSendRes, item, pVF);
			list.Add(postalSendRes);
		}
		if (list.Count == 0)
		{
			resultData.Msg = "没有可以发送到游戏邮件的物品";
			return resultData;
		}
		resultData.Data = list;
		return resultData;
	}

	private void EDWFCojmVG(PostalSendRes P_0, PvfFile P_1, PvfGroup P_2)
	{
		if (P_0.IsEqu.HasValue && P_0.IsEqu.Value)
		{
			switch (P_1.GetEquType(P_2))
			{
			case EquTypeDefault.Default:
				P_0.PostalType = PostalType.普通邮件;
				break;
			case EquTypeDefault.Avatar:
				P_0.PostalType = PostalType.时装邮件;
				break;
			case EquTypeDefault.Pet:
			case EquTypeDefault.PetEqu:
				P_0.PostalType = PostalType.宠物;
				break;
			case EquTypeDefault.PetEgg:
				P_0.PostalType = PostalType.宠物蛋;
				break;
			}
		}
		P_0.Init();
	}
}
