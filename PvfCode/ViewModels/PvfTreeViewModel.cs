using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

	public DataTemplate TreeColumnDataTemplate { get; set; }

	public Style FileListRowStyle { get; set; }

	public TreeViewType TreeType { get; set; }

	public TreeGroup TreeGroupData { get; set; }

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

	public List<KeyValuePair<string, PvfTreeFileBase>> VisibleItems { get; set; }

	public KeyValuePair<string, PvfTreeFileBase>? SelectedNodeBindgBase
	{
		get
		{
			return GetProperty(() => SelectedNodeBindgBase);
		}
		set
		{
			SetProperty<KeyValuePair<string, PvfTreeFileBase>?>(() => SelectedNodeBindgBase, value, OnSelectedNodeChanged);
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
			SetProperty<string>(() => SearchKeyword, value, OnSearchKeywordChanged);
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
			SetProperty(() => SearchPanelVisibility, value, OnSearchPanelVisibilityChanged);
		}
	}

	public List<TreeFileListSearchType> SearchTreeTypes => EnumberHelper.EnumToEnumList<TreeFileListSearchType>();

	public event DelegateDropFile EventDropFile;

	public event SelectedRowChangedDelegate SelectedRowChangedEvent;

	public event NodeDoubleClickDelegate EventNodeDoubleClick;

	public event DelegateOpenDocument EventOpenDocumenting;

	public event RemoveSelectedItemsDelegate RemoveSelectedItemsEvent;

	public event DiffEvents.DiffExtractSelectedDelegate EventDiffExtractSelected;

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

	private PvfTreeFileBase GetSelectedTreeFile()
	{
		if (SelectedNodeBindgBase.HasValue)
		{
			return SelectedNodeBindgBase.Value.Value;
		}
		return null;
	}

	private void OnSelectedNodeChanged()
	{
		if (SelectedNodeBindgBase.HasValue)
		{
			SelectedRowChangedEvent?.Invoke(SelectedNodeBindgBase.Value);
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
		EventNodeDoubleClick?.Invoke(row);
		if (value.IsFile)
		{
			switch (TreeType)
			{
			case TreeViewType.ImportFiles:
				OpenImportDocument(value);
				return;
			case TreeViewType.PvfDiffLeft:
			case TreeViewType.PvfDiffRight:
				return;
			}
			if (AppCore.ViewModelBase.PVF.PvfIsOpen)
			{
				OpenPvfDocument(value);
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

	private void OpenImportDocument(PvfTreeFileBase treeFile)
	{
		EventOpenDocumenting?.Invoke(treeFile);
	}

	private void OpenPvfDocument(PvfTreeFileBase treeFile)
	{
		PvfFile file = AppCore.ViewModelBase.PVF.GetFile(treeFile.FullPath);
		if (file == null)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), treeFile.FullPath), isError: true);
			return;
		}
		AppCore.ViewModelBase.RootDocument.AddDocument(file);
		if (TreeType == TreeViewType.SearchResult)
		{
			PvfTreeViewModel pvfFileTreeViewModel = AppCore.ViewModelBase.PvfFileTreeViewModel;
			pvfFileTreeViewModel.GoToNode(pvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(treeFile.FullPath));
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
			if (TreeType == TreeViewType.SearchResult && RemoveSelectedItemsEvent != null)
			{
				RemoveSelectedItemsEvent(GetSelectedFilePaths(GetTreeType.File));
			}
			TreeGroupData.DeleteTreeNodes(SelectedNodesBindBase);
			TreeGroupData.UpdateFileCount();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.RemoveSelectedNodes");
		}
	}

	private void RefreshSingleSelection()
	{
		if (SelectedNodesBindBase.Count == 1)
		{
			Service.ContentToNode(SelectedNodeBindgBase.Value);
		}
	}

	[Command]
	public void OnRenameNode()
	{
		if (GetSelectedTreeFile() != null)
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
		if (GetSelectedTreeFile() == null)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFileToEditComment"));
			return;
		}
		ViewEditTreeComment viewEditTreeComment = new ViewEditTreeComment(GetSelectedTreeFile());
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
					EventDiffExtractSelected?.Invoke(TreeType, files);
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
			string lstItems = ServiceItemCodeTable.FilesToLstItemsToString(AppCore.ViewModelBase.PVF, files, out var count);
			if (string.IsNullOrEmpty(lstItems))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileToExtractToLst"), isError: true);
				return;
			}
			AppCore.CopyString(lstItems);
			string text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExtractedLstCount"), count);
			AppCore.Logger.Success(text);
			await AppCore.Logger.ShowNotification(new NotificationViewModel<string>(AppSetting.Instance.AppName, text, Res.Instance.VisualStudioBlendLogo2015Pre_16x, AppSetting.Instance.GetIlogger()?.GetStr("mess_ViewDetails"), new DelegateCommand<string>(delegate
			{
				AppCore.ShowExtractLstWindow(lstItems);
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
		if (GetSelectedTreeFile() != null)
		{
			AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(GetSelectedTreeFile().FullPath);
		}
	}

	[Command]
	public async void OnImportFiles(bool isSelectPath)
	{
		try
		{
			PvfTreeFileBase pvfTreeFileBase = GetSelectedTreeFile();
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
			PvfTreeFileBase pvfTreeFileBase = GetSelectedTreeFile();
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
			if (GetSelectedTreeFile() == null)
			{
				return;
			}
			string fullPath = GetSelectedTreeFile().FullPath;
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
			if (GetSelectedTreeFile() == null)
			{
				return;
			}
			string rootPath;
			if (GetSelectedTreeFile().IsFile)
			{
				if (GetSelectedTreeFile().Level == 0)
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
				_ = GetSelectedTreeFile().Children;
				rootPath = GetSelectedTreeFile().FullPath;
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
				keyValuePair = GetNodeData(treeListNode.ParentNode.Content);
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
			CutFiles();
			break;
		case 1:
			CopyFiles();
			break;
		case 2:
			PasteFiles();
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
			if (GetSelectedTreeFile() == null)
			{
				return;
			}
			string fullPath = GetSelectedTreeFile().FullPath;
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
		if (GetSelectedTreeFile() != null)
		{
			WinPvfFileAttributes winPvfFileAttributes = new WinPvfFileAttributes(GetSelectedTreeFile());
			winPvfFileAttributes.Owner = Application.Current.MainWindow;
			winPvfFileAttributes.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			winPvfFileAttributes.Show();
		}
	}

	private async void CutFiles()
	{
		if (!IsSelectedNodes)
		{
			return;
		}
		try
		{
			await qjqilnF7lAFbCxZ5lIf.Instance.ClearCutStatusAsync();
			WindowLoading loading = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_Cutting"), Application.Current.MainWindow);
			loading.Show();
			await qjqilnF7lAFbCxZ5lIf.Instance.SetClipboardAsync(SelectedNodesBindBase, TreeFileCopyStatus.剪切, TreeType);
			loading.Close();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.ShearFiles");
		}
	}

	private async void CopyFiles()
	{
		if (!IsSelectedNodes)
		{
			return;
		}
		try
		{
			await qjqilnF7lAFbCxZ5lIf.Instance.ClearCutStatusAsync();
			WindowLoading loading = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_Copying"), Application.Current.MainWindow);
			loading.Show();
			await qjqilnF7lAFbCxZ5lIf.Instance.SetClipboardAsync(SelectedNodesBindBase, TreeFileCopyStatus.复制, TreeType);
			loading.Close();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "PvfTreeViewModel.CopyFiles");
		}
	}

	private void PasteFiles()
	{
		try
		{
			if (GetSelectedTreeFile() == null)
			{
				return;
			}
			string text;
			if (GetSelectedTreeFile().IsFile)
			{
				if (GetSelectedTreeFile().Level == 0)
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
				text = GetSelectedTreeFile().FullPath;
			}
			qjqilnF7lAFbCxZ5lIf.Instance.PasteFiles(text);
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
				GoToNode(GetNodeData(treeListNode.Content));
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
		if (!row.HasValue)
		{
			return;
		}
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)async delegate
		{
			TreeListNode treeListNode = Service.ContentToNode(row.Value);
			SelectedNodesBindBase = null;
			SelectedNodesBindBase = new ObservableCollection<KeyValuePair<string, PvfTreeFileBase>>();
			if (treeListNode == null)
			{
				ExpandNodePath(row.Value.Value.FullPath);
				treeListNode = Service.ContentToNode(row.Value);
			}
			Service.SetFocusableNode(treeListNode);
			SelectedNodeBindgBase = row;
			SelectedNodesBindBase.Add(row.Value);
		}, Array.Empty<object>());
	}

	private void ExpandNodePath(string filePath)
	{
		List<KeyValuePair<string, PvfTreeFileBase>> list = new List<KeyValuePair<string, PvfTreeFileBase>>();
		string[] array = filePath.Split('/');
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
			EventDropFile?.Invoke(files);
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
				SetDiffScrollBarAnnotation(e);
			}
		}
	}

	private void SetDiffScrollBarAnnotation(ScrollBarCustomRowAnnotationEventArgs args)
	{
		try
		{
			PvfTreeFileDiff pvfTreeFileDiff = (PvfTreeFileDiff)((KeyValuePair<string, PvfTreeFileBase>)args.Row).Value;
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
				args.ScrollBarAnnotationInfo = new ScrollBarAnnotationInfo
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

	private void OnSearchPanelVisibilityChanged()
	{
		if (!SearchPanelVisibility)
		{
			TreeGroupData.ClearSearchResult();
		}
	}

	private void OnSearchKeywordChanged()
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
		SearchTreeAsync();
	}

	private async void SearchTreeAsync()
	{
		string keyword = SearchKeyword;
		if (!string.IsNullOrEmpty(keyword))
		{
			TreeGroupData.Loading = true;
			TreeGroupData._SearchResultTrees = null;
			TreeGroupData._SearchResultTrees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
			int num = await Task.Run(() => TreeGroupData.SearchFileList(keyword));
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
		filePath = filePath.ToLower();
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		if (pVF.FileAny(filePath))
		{
			return pVF.ImportUpdateFile(pVF.GetFile(filePath), stream, filePath, compileScript: true, compileBinaryAni: true, convertChinese: false);
		}
		if (pVF.ImportNewFile(filePath, stream, compileScript: true, compileBinaryAni: true, convertChinese: false))
		{
			await ((DispatcherObject)Application.Current).Dispatcher.Invoke<Task>((Func<Task>)async delegate
			{
				await TreeGroupData.CreateTrees(new PooledList<string> { filePath });
			});
			return true;
		}
		return false;
	}

	public async Task<ResultData<IEnumerable<string>>> WebApiImportFiles(IEnumerable<ImportFileRes> fileDataList)
	{
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
			List<string> newFilePaths = new List<string>();
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
					newFilePaths.Add(fileData.FilePath);
				}
				else
				{
					list.Add(fileData.FilePath);
				}
			}
			result.Data = list;
			if (newFilePaths.Count > 0)
			{
				await ((DispatcherObject)Application.Current).Dispatcher.Invoke<Task>((Func<Task>)async delegate
				{
					await TreeGroupData.CreateTrees(new PooledList<string>(newFilePaths));
				});
			}
		}
		catch (Exception ex)
		{
			result.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BatchImportError"), ex.Message);
		}
		return result;
	}

	private KeyValuePair<string, PvfTreeFileBase>? GetNodeData(object content)
	{
		if (content == null)
		{
			return null;
		}
		return (KeyValuePair<string, PvfTreeFileBase>)content;
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
			if (!TreeListDropGroup.Instance.Success && GetSelectedTreeFile() != null && Service.ContentToNode(SelectedNodeBindgBase) != null)
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
		if (!IsSelectedNodes)
		{
			AppCore.ShowMsg("请先选中要发送的物品");
			return;
		}
		ResultData<List<PostalSendRes>> postalItems = CreatePostalItems();
		if (postalItems.IsError)
		{
			AppCore.ShowMsg(postalItems.Msg);
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
				ResultData<string> resultData = await dnfSqlService.SendPostal(list, postalItems.Data, AppSetting.Instance.GMToolOptions.PostalSendTitle, AppSetting.Instance.GMToolOptions.PostalSendText);
				AppCore.Logger.Warning("邮件发送回调：" + resultData.Data);
			}
		});
	}

	private ResultData<List<PostalSendRes>> CreatePostalItems()
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
			InitializePostalItem(postalSendRes, item, pVF);
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

	private void InitializePostalItem(PostalSendRes postalItem, PvfFile file, PvfGroup pvf)
	{
		if (postalItem.IsEqu.HasValue && postalItem.IsEqu.Value)
		{
			switch (file.GetEquType(pvf))
			{
			case EquTypeDefault.Default:
				postalItem.PostalType = PostalType.普通邮件;
				break;
			case EquTypeDefault.Avatar:
				postalItem.PostalType = PostalType.时装邮件;
				break;
			case EquTypeDefault.Pet:
			case EquTypeDefault.PetEqu:
				postalItem.PostalType = PostalType.宠物;
				break;
			case EquTypeDefault.PetEgg:
				postalItem.PostalType = PostalType.宠物蛋;
				break;
			}
		}
		postalItem.Init();
	}
}
