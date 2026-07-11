using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using DevExpress.Mvvm;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.LoggerBase;
using PvfCode.Models.Macro;
using PvfCode.ViewModels;
using PvfCode.ViewModels.TreeFolder;
using PvfCode.Views;
using PvfCode.Views.BatchOperation;
using PvfCode.Views.Dialogs;
using PvfCode.Views.Macro;
using PvfCode.Views.PvfTreeFolder;
using PvfCode.Views.SearchPvf.SearchName;
using Utools;

namespace PvfCode;

public class AppCore
{
	public static MainWindowViewModel ViewModelBase;

	public static string Version => Application.ResourceAssembly?.GetName()?.Version?.ToString();

	public static string Root => AppDomain.CurrentDomain.BaseDirectory;

	public static bool IsSaveAllDocument { get; set; }

	public static LoggerViewModel Logger { get; set; }

	public static MainWindow MainWin => (MainWindow)Application.Current.MainWindow;

	public static string NickNameTemp { get; set; }

	public static ObservableCollection<string> EditorSearchKeywordLog { get; set; } = new ObservableCollection<string>();

	public static ObservableCollection<string> EditorReplaceKeywordLog { get; set; } = new ObservableCollection<string>();

	public static ICommand<(IEnumerable<string>, IEnumerable<string>)> ShowBatchOperationDetailsCommand => new DelegateCommand<(IEnumerable<string>, IEnumerable<string>)>(ShowBatchOperationDetails);

	public static void ShowMsg(string msg, bool isError = false, string? caption = null)
	{
		Logger.ShowMsg(msg, isError, caption);
	}

	public static void ShowMsg(IMessageBoxService messageBoxService, string msg, bool isError = false, string? caption = null)
	{
		caption = caption ?? ViewModelBase.AppName;
		((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)(() =>
		{
			messageBoxService.ShowMessage(msg, caption, MessageButton.OK, isError ? MessageIcon.Error : MessageIcon.Information);
		}));
	}

	public static void CopyString(string obj)
	{
		try
		{
			Clipboard.SetText(obj);
		}
		catch (Exception)
		{
		}
	}

	public static IEnumerable<string> SelectPvfFileList(TreeViewType sourceType, string? title = null)
	{
		ViewSelectTreeFiles viewSelectTreeFiles = new ViewSelectTreeFiles(sourceType)
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen
		};
		if (!string.IsNullOrEmpty(title))
		{
			viewSelectTreeFiles.Title = title;
		}
		viewSelectTreeFiles.ShowDialog();
		ViewSelectTreeFilesViewModel jj1HLm82nA = viewSelectTreeFiles.jj1HLm82nA;
		if (jj1HLm82nA.IsOk && jj1HLm82nA.TreeViewModel.IsSelectedNodes)
		{
			return jj1HLm82nA.TreeViewModel.GetSelectedFilePaths(GetTreeType.File);
		}
		return null;
	}

	public static void ShowDefaultScriptEditorWindow(ViewScriptEditorViewModel vm, Window? owner = null)
	{
		if (owner == null)
		{
			owner = Application.Current.MainWindow;
		}
		ViewScriptEditor viewScriptEditor = new ViewScriptEditor(vm);
		viewScriptEditor.Owner = owner;
		viewScriptEditor.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		viewScriptEditor.Show();
	}

	public static string SelectPvfFolderPath(Window? owner = null)
	{
		if (owner == null)
		{
			owner = Application.Current.MainWindow;
		}
		ViewSelectFolder viewSelectFolder = new ViewSelectFolder();
		viewSelectFolder.Owner = owner;
		viewSelectFolder.ShowDialog();
		ViewSelectFolderViewModel viewSelectFolderViewModel = (ViewSelectFolderViewModel)viewSelectFolder.DataContext;
		if (viewSelectFolderViewModel.SelectedItem.HasValue)
		{
			return viewSelectFolderViewModel.SelectedItem.Value.Value.FullPath;
		}
		return null;
	}

	public static void Collect()
	{
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
	}

	public static IntPtr GetWindowHandle(Window win)
	{
		return new WindowInteropHelper(win).Handle;
	}

	private static void ShowBatchOperationDetails((IEnumerable<string>, IEnumerable<string>) data)
	{
		WindowBatchOperationDetails windowBatchOperationDetails = new WindowBatchOperationDetails(data.Item1, data.Item2);
		windowBatchOperationDetails.Owner = Application.Current.MainWindow;
		windowBatchOperationDetails.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		windowBatchOperationDetails.Show();
	}

	public static WindowMacroToolViewModel SelectMacroSavePath(MacroType macroType, string Name, MacroData data)
	{
		WindowMacroTool windowMacroTool = new WindowMacroTool(isTreeList: true, macroType);
		windowMacroTool.Owner = Application.Current.MainWindow;
		windowMacroTool.WindowStartupLocation = WindowStartupLocation.CenterScreen;
		windowMacroTool.ShowDialog();
		WindowMacroToolViewModel vM = windowMacroTool.VM;
		if (vM.IsSelect)
		{
			return vM;
		}
		return null;
	}

	public static async Task<bool> SaveMacroData(MacroData newData, string title, Window owner, bool showShareCheckBox = true, bool isShare = false)
	{
		WIndowSaveMacroData wIndowSaveMacroData = new WIndowSaveMacroData(newData, title, showShareCheckBox, isShare)
		{
			Owner = owner,
			WindowStartupLocation = WindowStartupLocation.CenterScreen
		};
		bool re = wIndowSaveMacroData.ShowDialog().Value;
		if (re)
		{
			AppSetting.Instance.MacroGroup.DoNotifyTrees();
			await Logger.ShowNotification(new NotificationViewModel(AppSetting.Instance.AppName, "宏保存成功", Res.Instance.VisualStudioBlendLogo2015Pre_16x));
		}
		return re;
	}

	public static WindowLoading CreateLoading(string title, Window owner)
	{
		WindowLoading window = null;
		((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)(() =>
		{
			window = new WindowLoading(title)
			{
				Owner = owner,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};
		}));
		return window;
	}

	public static void ShowExtractLstWindow(string codes)
	{
		ShowDefaultScriptEditorWindow(new ViewScriptEditorViewModel(AppSetting.Instance.GetIlogger()?.GetStr("mess_LstExtractResult"), codes)
		{
			IsReadOnly = false
		});
	}

	public static DialogStringListViewModelResult ShowDialogStringListViewModelResult(DialogStringListViewModel vm)
	{
		DialogStringList dialogStringList = new DialogStringList(vm);
		dialogStringList.Owner = Application.Current.MainWindow;
		dialogStringList.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		dialogStringList.ShowDialog();
		return dialogStringList.Result;
	}

	public static int? ShowItemCodeSelectView(string title, Window? owner = null, IEnumerable<string>? pathNames = null, bool initItems = false)
	{
		if (owner == null)
		{
			owner = Application.Current.MainWindow;
		}
		WindowSearchItemName windowSearchItemName = new WindowSearchItemName(title, showGroupPanel: false, pathNames, initItems)
		{
			Owner = owner,
			WindowStartupLocation = WindowStartupLocation.CenterScreen
		};
		if (!windowSearchItemName.ShowDialog().Value)
		{
			return null;
		}
		return windowSearchItemName.ItemCode;
	}

	public static IEnumerable<int> ShowItemCodeListSelectView(string title, Window? owner = null, IEnumerable<string>? pathNames = null, bool initItems = false)
	{
		if (owner == null)
		{
			owner = Application.Current.MainWindow;
		}
		WindowSearchItemName windowSearchItemName = new WindowSearchItemName(title, showGroupPanel: true, pathNames, initItems)
		{
			Owner = owner,
			WindowStartupLocation = WindowStartupLocation.CenterScreen
		};
		if (!windowSearchItemName.ShowDialog().Value)
		{
			return null;
		}
		return windowSearchItemName.ItemCodes();
	}

	public static async Task ClearMemory()
	{
		await Task.Run(delegate
		{
			WindowsEx.ClearMemorySilent(Process.GetCurrentProcess());
			GC.Collect();
			GC.Collect();
		});
	}

	public AppCore()
	{
	}

}
