using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Nito.AsyncEx;
using PvfCode.Dot;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.MVVMServices;
using PvfCode.Models.Enums;
using PvfCode.Models.Pvf.ImportModels;
using PvfCode.Services.PvfFileModel.etc;
using PvfCode.ViewModels.Diff;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.Views;
using PvfCode.Views.AniDesigner;
using PvfCode.Views.BatchOperation;
using PvfCode.Views.BookMark;
using PvfCode.Views.DescriptionViews;
using PvfCode.Views.Dialogs;
using PvfCode.Views.Diff;
using PvfCode.Views.GMTool;
using PvfCode.Views.GameImagesExtract;
using PvfCode.Views.ImportViews;
using PvfCode.Views.LstTools;
using PvfCode.Views.Macro;
using PvfCode.Views.NpcShopEditor;
using PvfCode.Views.SearchPvf.SearchName;
using PvfCode.Views.Tools;
using PvfCode.Views.Tools.ShopManager;
using PvfCode.Views.independent_drop;
using Utools;
using Views.Tools.ConvertChinaPvfFiles;

namespace PvfCode.ViewModels.Bars;

public class BarViewModel : ViewModelBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass58_0
	{
		public string A9TETD5UQ5;

		public _003C_003Ec__DisplayClass58_0()
		{
		}

		internal Task<bool>? UtyEj3XIQs()
		{
			return AppCore.ViewModelBase.PVF.OpenPvfPack(A9TETD5UQ5, AppCore.ViewModelBase.MainProgress);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass63_0
	{
		public string aXZEHemSwu;

		public _003C_003Ec__DisplayClass63_0()
		{
		}

		internal Task<ResultData>? Xi1ECSQlY6()
		{
			return AppCore.ViewModelBase.PVF.SavePvfPack(aXZEHemSwu, isFastMode: false, AppCore.ViewModelBase.MainProgress, notButtonClick: false);
		}
	}

	[CompilerGenerated]
	private WinNewDiffEditor zWTxXTQLIC;

	[CompilerGenerated]
	private bool FctxpshNB5;

	[CompilerGenerated]
	private WinNewDiffViewModel DkExUxsJVI;

	private readonly AsyncLock uJ3xcpPIRg;

	public ImageSource ThemeDarkImageSource => (ImageSource)new ThemePaletteGlyphConverter().Convert(ThemeType.VS2019Dark.ToString(), null, null, CultureInfo.CurrentCulture);

	public ImageSource ThemeLightImageSource => (ImageSource)new ThemePaletteGlyphConverter().Convert(ThemeType.VS2019Light.ToString(), null, null, CultureInfo.CurrentCulture);

	public ImageSource ThemeBlueImageSource => (ImageSource)new ThemePaletteGlyphConverter().Convert(ThemeType.VS2019Blue.ToString(), null, null, CultureInfo.CurrentCulture);

	public ThemeType SelectedTheme
	{
		get
		{
			return GetProperty(() => SelectedTheme);
		}
		set
		{
			SetProperty(() => SelectedTheme, value);
			OnThemeChanged(value);
		}
	}

	public bool DiffIsOpen
	{
		[CompilerGenerated]
		get
		{
			return FctxpshNB5;
		}
		[CompilerGenerated]
		set
		{
			FctxpshNB5 = value;
		}
	}

	public WinNewDiffViewModel WindowDiffViewModel
	{
		[CompilerGenerated]
		get
		{
			return DkExUxsJVI;
		}
		[CompilerGenerated]
		set
		{
			DkExUxsJVI = value;
		}
	}

	public BarViewModel()
	{
		uJ3xcpPIRg = new AsyncLock();
		SelectedTheme = AppSetting.Instance.NowThemeType;
	}

	[Command]
	public void OnOpenGameImagesExtractTool()
	{
		WindowGameImageExtract windowGameImageExtract = new WindowGameImageExtract();
		windowGameImageExtract.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		windowGameImageExtract.Show();
	}

	[Command]
	public void OnOpenNpcShopEditor()
	{
		WinNpcShopEditor winNpcShopEditor = new WinNpcShopEditor();
		winNpcShopEditor.Owner = Application.Current.MainWindow;
		winNpcShopEditor.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		winNpcShopEditor.Show();
	}

	[Command]
	public void OnOpenWindowAniDesigner()
	{
		WindowAniDesigner windowAniDesigner = new WindowAniDesigner();
		windowAniDesigner.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		windowAniDesigner.Show();
	}

	[Command]
	public void OnOpenGMTool()
	{
		new GMToolMain().Show();
	}

	[Command]
	public void OnConvertChinaPlusPvf()
	{
		ViewConvertChinaPvfFiles viewConvertChinaPvfFiles = new ViewConvertChinaPvfFiles();
		viewConvertChinaPvfFiles.Owner = Application.Current.MainWindow;
		viewConvertChinaPvfFiles.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		viewConvertChinaPvfFiles.Show();
	}

	[Command]
	public void OnOpenPvfUtilityRootDir()
	{
		FileHelper.OpenFolderAndSelectFile(AppSetting.AppBasePath);
	}

	[Command]
	public void OnOpenViewIndependent_drop()
	{
		ViewIndependent_drop viewIndependent_drop = new ViewIndependent_drop();
		viewIndependent_drop.Owner = (AppSetting.Instance.ChildWindowAttachmentMainWindow ? Application.Current.MainWindow : null);
		viewIndependent_drop.WindowStartupLocation = ((!AppSetting.Instance.ChildWindowAttachmentMainWindow) ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner);
		viewIndependent_drop.Show();
	}

	[Command]
	public void OnOpenViewFileListDescription()
	{
		ViewFileListDescription viewFileListDescription = new ViewFileListDescription();
		viewFileListDescription.Owner = Application.Current.MainWindow;
		viewFileListDescription.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		viewFileListDescription.Show();
	}

	[Command]
	public void OnOpenViewTabComment()
	{
		ViewTabComment viewTabComment = new ViewTabComment();
		viewTabComment.Owner = Application.Current.MainWindow;
		viewTabComment.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		viewTabComment.Show();
	}

	public async void OnThemeChanged(ThemeType themeType)
	{
		ApplicationThemeHelper.ApplicationThemeName = themeType.ToString();
		AppSetting.Instance.NowThemeType = themeType;
		ThemeSwitcher.Instance.SwitchTheme(themeType);
		await AppSetting.Instance.SaveSetting();
	}

	[Command]
	public void OnOpenWindowItemNameSearch()
	{
		WindowSearchItemName windowSearchItemName = new WindowSearchItemName();
		windowSearchItemName.Owner = Application.Current.MainWindow;
		windowSearchItemName.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		windowSearchItemName.Show();
	}

	[Command]
	public void OnOpenDocumentIndex()
	{
		AppCore.ViewModelBase.RootDocument.AddControl(PvfFileDocumentType.起始页);
	}

	[Command]
	public void OnOpenPvfDiffTool()
	{
		AppCore.ViewModelBase.RootDocument.AddControl(PvfFileDocumentType.PVF差异比较器);
	}

	[Command]
	public void OnOpenLstTools(string filePath)
	{
		WinLstTools winLstTools = new WinLstTools();
		winLstTools.Owner = (AppSetting.Instance.ChildWindowAttachmentMainWindow ? Application.Current.MainWindow : null);
		winLstTools.WindowStartupLocation = ((!AppSetting.Instance.ChildWindowAttachmentMainWindow) ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner);
		winLstTools.Show();
	}

	[Command]
	public void OnOpenWindowItemCodeSearch()
	{
		if (AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			WindowItemCodeSearch windowItemCodeSearch = new WindowItemCodeSearch();
			windowItemCodeSearch.Owner = (AppSetting.Instance.ChildWindowAttachmentMainWindow ? Application.Current.MainWindow : null);
			windowItemCodeSearch.WindowStartupLocation = ((!AppSetting.Instance.ChildWindowAttachmentMainWindow) ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner);
			windowItemCodeSearch.Show();
		}
		else
		{
			AppCore.ShowMsg(AppCore.Logger.GetStr("mess_PleaseLoadPvfPackFirst"), isError: true);
		}
	}

	[Command]
	public void OnOpenNpkSearchPanel()
	{
		WinFindNpk winFindNpk = new WinFindNpk();
		winFindNpk.Owner = Application.Current.MainWindow;
		winFindNpk.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		winFindNpk.ShowInTaskbar = false;
		winFindNpk.Show();
	}

	[Command]
	public void OnOpenMacroTool()
	{
		WindowMacroTool windowMacroTool = new WindowMacroTool(isTreeList: false);
		windowMacroTool.Owner = Application.Current.MainWindow;
		windowMacroTool.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		windowMacroTool.Show();
	}

	[SpecialName]
	[CompilerGenerated]
	private WinNewDiffEditor USjxkDhoAk()
	{
		return zWTxXTQLIC;
	}

	[SpecialName]
	[CompilerGenerated]
	private void R8Tx0hphas(WinNewDiffEditor P_0)
	{
		zWTxXTQLIC = P_0;
	}

	[Command]
	public void OnOpenDiffWindow()
	{
		OpenDiffWindow(isLeft: true, null);
	}

	public void OpenDiffWindow(bool isLeft, DiffSource source)
	{
		if (!WindowsEx.CheckIsInsertMicrosoftEdgeRuntime())
		{
			cAkx9QPmBC();
			return;
		}
		if (DiffIsOpen)
		{
			USjxkDhoAk().Activate();
		}
		else
		{
			WindowDiffViewModel = new WinNewDiffViewModel();
			R8Tx0hphas(new WinNewDiffEditor(WindowDiffViewModel)
			{
				Owner = Application.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			});
			USjxkDhoAk().Show();
			DiffIsOpen = true;
		}
		if (source != null)
		{
			if (isLeft)
			{
				WindowDiffViewModel.LeftSource = source;
			}
			else
			{
				WindowDiffViewModel.RightSource = source;
			}
			WindowDiffViewModel.RefEditor();
		}
	}

	public void OpenDiffWindow(DiffSource leftSource, DiffSource rightSource)
	{
		if (!WindowsEx.CheckIsInsertMicrosoftEdgeRuntime())
		{
			cAkx9QPmBC();
			return;
		}
		if (DiffIsOpen)
		{
			USjxkDhoAk().Activate();
		}
		else
		{
			WindowDiffViewModel = new WinNewDiffViewModel();
			R8Tx0hphas(new WinNewDiffEditor(WindowDiffViewModel)
			{
				Owner = Application.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			});
			USjxkDhoAk().Show();
			DiffIsOpen = true;
		}
		WindowDiffViewModel.LeftSource = leftSource;
		WindowDiffViewModel.RightSource = rightSource;
		WindowDiffViewModel.RefEditor();
	}

	private void cAkx9QPmBC()
	{
		WindowInsertMicrosoftEdgeRuntime windowInsertMicrosoftEdgeRuntime = new WindowInsertMicrosoftEdgeRuntime();
		windowInsertMicrosoftEdgeRuntime.Owner = Application.Current.MainWindow;
		windowInsertMicrosoftEdgeRuntime.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		windowInsertMicrosoftEdgeRuntime.ShowDialog();
	}

	[Command]
	public void OnEditPvfHeaderInfo()
	{
		WindowPvfFileHeaderEdit windowPvfFileHeaderEdit = new WindowPvfFileHeaderEdit();
		windowPvfFileHeaderEdit.Owner = Application.Current.MainWindow;
		windowPvfFileHeaderEdit.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		windowPvfFileHeaderEdit.Show();
	}

	[Command]
	public void OnOpenPublicSettingWindow()
	{
		WindowPublicSetting windowPublicSetting = new WindowPublicSetting();
		windowPublicSetting.Owner = Application.Current.MainWindow;
		windowPublicSetting.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		windowPublicSetting.Show();
	}

	[Command]
	public void OnOpenBookMarkEdit()
	{
		BookMarkEditView bookMarkEditView = new BookMarkEditView(isTreeList: false);
		bookMarkEditView.Owner = Application.Current.MainWindow;
		bookMarkEditView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		bookMarkEditView.Show();
	}

	[Command]
	public void OnAssociate_pvfPack()
	{
	}

	[Command]
	public void OnPvfRelease()
	{
		AppCore.ViewModelBase.RootDocument.AddControl(PvfFileDocumentType.发布);
	}

	[Command]
	public async void OnAddNewEmptyPvfFile()
	{
		string filter = AppSetting.Instance.GetIlogger()?.GetStr("SavePvfPackFileDialogFilterName") + " (*.pvf)|*.pvf";
		SaveFileDialog openFileDialog = new SaveFileDialog
		{
			Filter = filter,
			FileName = "NewScript.pvf",
			Title = AppSetting.Instance.GetIlogger()?.GetStr("SaveNewPvfPackFileDialogSelectDirTitle")
		};
		bool? flag = openFileDialog.ShowDialog(Application.Current.MainWindow);
		if (flag.HasValue && flag.Value)
		{
			PvfGroup pvf = AppCore.ViewModelBase.PVF;
			pvf.AddNewEmptyPvfFile();
			await AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(pvf.FileList.Keys));
			AppCore.Logger.Success(AppSetting.Instance.GetIlogger()?.GetStr("mess_NewPvfPackSuccess"));
			pvf.PvfPackFilePath = openFileDialog.FileName;
			NFbxZBiaRO(openFileDialog.FileName);
		}
	}

	[Command]
	public void BatchOperation()
	{
		BatchOperationView batchOperationView = new BatchOperationView();
		batchOperationView.Owner = (AppSetting.Instance.ChildWindowAttachmentMainWindow ? Application.Current.MainWindow : null);
		batchOperationView.WindowStartupLocation = ((!AppSetting.Instance.ChildWindowAttachmentMainWindow) ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner);
		batchOperationView.Show();
	}

	[Command]
	public async void OnRefTreeList()
	{
		AppCore.ViewModelBase.PvfFileTreeViewModel.Clear();
		await kOlxPaPXEo();
	}

	public void ApiRefTree()
	{
		OnRefTreeList();
	}

	[Command]
	public async void OnClosePvfFile()
	{
		string fileName = Path.GetFileName(AppCore.ViewModelBase.PVF.PvfPackFilePath);
		if (AppCore.Logger.ShowDialog(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_CloseCurrentPvfPackDialog"), fileName)) == MessageResult.Yes)
		{
			await AppCore.ViewModelBase.Clear();
		}
	}

	[Command]
	public async void OnOpenPvfFile(string filePath)
	{
		_003C_003Ec__DisplayClass58_0 CS_0024_003C_003E8__locals11 = new _003C_003Ec__DisplayClass58_0();
		CS_0024_003C_003E8__locals11.A9TETD5UQ5 = filePath;
		if (string.IsNullOrEmpty(CS_0024_003C_003E8__locals11.A9TETD5UQ5))
		{
			string initialDirectory = "";
			int count = AppSetting.Instance.PathConfig.PvfOpenLog.Count;
			if (count > 0)
			{
				initialDirectory = Path.GetDirectoryName(AppSetting.Instance.PathConfig.PvfOpenLog.Keys[count - 1]);
			}
			CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
			{
				Title = AppSetting.Instance.GetIlogger()?.GetStr("OpenPvfPackFileDialogFilterName"),
				IsFolderPicker = false,
				Multiselect = false,
				AllowPropertyEditing = true,
				EnsurePathExists = true,
				EnsureValidNames = true,
				InitialDirectory = initialDirectory
			};
			commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter(AppSetting.Instance.GetIlogger()?.GetStr("OpenPvfPackFileDialogFilterName"), ".pvf"));
			if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok)
			{
				return;
			}
			CS_0024_003C_003E8__locals11.A9TETD5UQ5 = commonOpenFileDialog.FileName;
		}
		else if (AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			if (AppCore.Logger.ShowDialog(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_LoadPvfPackDialog"), CS_0024_003C_003E8__locals11.A9TETD5UQ5)) != MessageResult.Yes)
			{
				return;
			}
			await AppCore.ViewModelBase.Clear();
		}
		if (!File.Exists(CS_0024_003C_003E8__locals11.A9TETD5UQ5))
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), CS_0024_003C_003E8__locals11.A9TETD5UQ5), isError: true);
			if (AppSetting.Instance.PathConfig.PvfOpenLog.ContainsKey(CS_0024_003C_003E8__locals11.A9TETD5UQ5))
			{
				AppSetting.Instance.PathConfig.PvfOpenLog.Remove(CS_0024_003C_003E8__locals11.A9TETD5UQ5);
			}
			return;
		}
		AppCore.Logger.Warning(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_PvfPackLoading"), CS_0024_003C_003E8__locals11.A9TETD5UQ5));
		bool isOpen = await Task.Run(() => AppCore.ViewModelBase.PVF.OpenPvfPack(CS_0024_003C_003E8__locals11.A9TETD5UQ5, AppCore.ViewModelBase.MainProgress));
		if (!isOpen)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotOpenPvfPack"), isError: true);
			return;
		}
		AppCore.ViewModelBase.PVF.PvfIsOpen = true;
		await kOlxPaPXEo();
		AppSetting.Instance.PathConfig.AddPvfOpenLog(CS_0024_003C_003E8__locals11.A9TETD5UQ5);
		if (isOpen)
		{
			if (AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.AutoTheBackupPvfIsOpen)
			{
				await Task.Run(() => AppCore.ViewModelBase.PVF.SavePvfPack(AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.CreateFilePath(AppCore.ViewModelBase.PVF), isFastMode: false, null));
			}
			await AppSetting.Instance.SaveSetting();
		}
		if (!string.IsNullOrEmpty(AppSetting.Instance.ImagePacks2Options.ImagePacks2Path) && ImagePack2Service.Instance.Count == 0)
		{
			await Task.Run((Func<Task?>)AppCore.ViewModelBase.ImagePacks2ViewModel.LoadImagePacks2);
		}
		AppCore.ClearMemory();
	}

	private async Task kOlxPaPXEo()
	{
		using (await uJ3xcpPIRg.LockAsync())
		{
			await AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(AppCore.ViewModelBase.PVF.FileList.Keys));
		}
	}

	[Command]
	public void OnSavePvfFile(string filePath)
	{
		if (AppSetting.Instance.PvfConfig.SavePvfPackShowDialog)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				filePath = AppCore.ViewModelBase.PVF.PvfPackFilePath;
			}
			if (AppCore.Logger.ShowDialog(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SavePvfPackShowDialog"), filePath)) != MessageResult.Yes)
			{
				return;
			}
		}
		NFbxZBiaRO(filePath);
	}

	[Command]
	public void OnSavePvfFileAs()
	{
		string filter = AppSetting.Instance.GetIlogger()?.GetStr("SavePvfPackFileDialogFilterName") + " (*.pvf)|*.pvf";
		SaveFileDialog saveFileDialog = new SaveFileDialog
		{
			Filter = filter,
			FileName = Path.GetFileName(AppCore.ViewModelBase.PVF.PvfPackFilePath),
			CheckFileExists = false
		};
		bool? flag = saveFileDialog.ShowDialog();
		if (flag.HasValue && flag.Value)
		{
			NFbxZBiaRO(saveFileDialog.FileName);
		}
	}

	private async void NFbxZBiaRO(string P_0)
	{
		_003C_003Ec__DisplayClass63_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass63_0();
		CS_0024_003C_003E8__locals5.aXZEHemSwu = P_0;
		if (string.IsNullOrEmpty(CS_0024_003C_003E8__locals5.aXZEHemSwu))
		{
			CS_0024_003C_003E8__locals5.aXZEHemSwu = AppCore.ViewModelBase.PVF.PvfPackFilePath;
		}
		if (AppCore.ViewModelBase.RootDocument.CheckNotSavedDocumentIsAny())
		{
			DialogStringListViewModelResult num = AppCore.ShowDialogStringListViewModelResult(new DialogStringListViewModel(AppCore.ViewModelBase.RootDocument.GetNotSaveFiles(), AppSetting.Instance.GetIlogger()?.GetStr("mess_ExistUnsavedFileDialogTitle"), "是", "否", "取消"));
			if (num == DialogStringListViewModelResult.Yes)
			{
				AppCore.ViewModelBase.RootDocument.SaveAllDocument();
			}
			if (num == DialogStringListViewModelResult.Cancel)
			{
				return;
			}
		}
		ResultData resultData = await Task.Run(() => AppCore.ViewModelBase.PVF.SavePvfPack(CS_0024_003C_003E8__locals5.aXZEHemSwu, isFastMode: false, AppCore.ViewModelBase.MainProgress, notButtonClick: false));
		if (!resultData.IsError)
		{
			AppCore.Logger.Debug(AppSetting.Instance.GetIlogger()?.GetStr("mess_SavePvfPackSuccess") + CS_0024_003C_003E8__locals5.aXZEHemSwu);
			return;
		}
		AppCore.ShowMsg(resultData.Msg, isError: true);
		AppCore.Logger.Error(resultData.Msg);
	}

	public async void ShowImportFiles(string tarGetPath = "", HashSet<ImportFileItem> importFiles = null)
	{
		ViewImportFilesViewModel viewImportFilesViewModel = new ViewImportFilesViewModel();
		ViewImportFilesPanel viewImportFilesPanel = new ViewImportFilesPanel();
		viewImportFilesPanel.DataContext = viewImportFilesViewModel;
		IDockLayoutManagerService dockLayoutManagerService = AppCore.ViewModelBase.DockLayoutManagerService;
		FloatGroup floatGroup = dockLayoutManagerService.AddFloatPanel(viewImportFilesPanel, AppCore.Logger.GetStr("WinImportFile_Title"));
		floatGroup.MinWidth = 700.0;
		dockLayoutManagerService.SetFloatPanelAutoHeight(floatGroup, SizeToContent.Height);
		dockLayoutManagerService.SetFloatPanelCenter(floatGroup);
		AppSetting.Instance.PvfConfig.ImportConfig.TargetPath = tarGetPath;
		if (importFiles != null)
		{
			await viewImportFilesViewModel.TreeViewModel.TreeGroupData.ImportFilesCreateTrees(importFiles, AppSetting.Instance.PvfConfig.ImportConfig.TargetPath);
		}
	}

	[Command]
	public void OnImportFiles()
	{
		ShowImportFiles();
	}

	[Command]
	public void OnExtractFiles(IEnumerable<string> targetFiles = null)
	{
		ViewExtractFiles viewExtractFiles = new ViewExtractFiles(targetFiles);
		viewExtractFiles.Owner = (AppSetting.Instance.ChildWindowAttachmentMainWindow ? Application.Current.MainWindow : null);
		viewExtractFiles.WindowStartupLocation = ((!AppSetting.Instance.ChildWindowAttachmentMainWindow) ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner);
		viewExtractFiles.Show();
	}

	[Command]
	public void OnExit()
	{
		App.OnExit();
	}

	[Command]
	public void OnNavigation(bool isLeft)
	{
		AppCore.Logger.Success(isLeft.ToString());
	}

	[Command]
	public void OnDiskOpenPvfFile()
	{
		if (AppCore.ViewModelBase.PVF.PvfIsOpen && ((int)Keyboard.Modifiers & 2) == 2)
		{
			try
			{
				FileHelper.OpenFolderAndSelectFile(AppCore.ViewModelBase.PVF.PvfPackFilePath);
			}
			catch (Exception ex)
			{
				AppCore.ShowMsg(ex.Message, isError: true);
			}
		}
	}

	[Command]
	public void OnDiskOpenPvfFileMouseLeftButtonDown()
	{
		Application.Current.MainWindow?.DragMove();
	}

	[Command]
	public void OnAbout()
	{
		WindowAbout windowAbout = new WindowAbout();
		windowAbout.Owner = Application.Current.MainWindow;
		windowAbout.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		windowAbout.ShowDialog();
	}

	[Command]
	public void OnOpenPublicSetting(string key)
	{
		WindowPublicSetting windowPublicSetting = new WindowPublicSetting(key);
		windowPublicSetting.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		windowPublicSetting.Owner = Application.Current.MainWindow;
		windowPublicSetting.Show();
	}

	[Command]
	public void OnOpenWorlddropTool()
	{
		WorlddropTool worlddropTool = new WorlddropTool();
		worlddropTool.Owner = Application.Current.MainWindow;
		worlddropTool.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		worlddropTool.Show();
	}

	[Command]
	public void OnOpenWinShopManager()
	{
		WinShopManager winShopManager = new WinShopManager();
		winShopManager.Owner = Application.Current.MainWindow;
		winShopManager.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		winShopManager.Show();
	}

	[Command]
	public void OnExtractTitleBookQstCodeList()
	{
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		List<int> list = new TitleBook(pVF.GetFile("etc/titlebook.etc") ?? throw new Exception("称号簿文件不存在"), pVF).ExtractQstCodeList();
		AppCore.CopyString(string.Join(",", list));
		AppCore.Logger.Success("已复制到剪贴板 Count：" + list.Count);
	}

	[Command]
	public void OnExtractTitleBookTitleEquCoode()
	{
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		List<int> list = new TitleBook(pVF.GetFile("etc/titlebook.etc") ?? throw new Exception("称号簿文件不存在"), pVF).ExtractTitleEquCodeList();
		AppCore.CopyString(string.Join(",", list));
		AppCore.Logger.Success("已复制到剪贴板 Count：" + list.Count);
	}
}
