#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using HL.Interfaces;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.LoggerBase;
using PvfCode.Services;
using PvfCode.Views.Dialogs;
using PvfCode.Views.NpcShopEditor;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels;

public class LoggerViewModel : ViewModelBase, Ilogger
{
	private CancellationTokenSource cancellationTokenSource;

	private TextDocument document;

	private IHighlightingDefinition highlighting;

	private ConcurrentObservableCollection<ErrorItem> errorItems;

	public CancellationTokenSource TaskCancellationTokenSource
	{
		get
		{
			if (cancellationTokenSource == null)
			{
				cancellationTokenSource = new CancellationTokenSource();
			}
			return cancellationTokenSource;
		}
		set
		{
			cancellationTokenSource = value;
		}
	}

	public bool TaskIsWork { get; set; }

	public DelegateCommand ClearOutPutCommand { get; set; }

	public TextDocument Document
	{
		get
		{
			return document;
		}
		set
		{
			document = value;
			RaisePropertyChanged("_Document");
		}
	}

	public IHighlightingDefinition Highlighting
	{
		get
		{
			return highlighting;
		}
		set
		{
			highlighting = value;
			RaisePropertyChanged("Highlighting");
		}
	}

	public ConcurrentObservableCollection<ErrorItem> ErrorItems
	{
		get
		{
			return errorItems;
		}
		set
		{
			errorItems = value;
			RaisePropertyChanged("ErrorItems");
		}
	}

	public void TaskTokenStart()
	{
		TaskIsWork = true;
		TaskCancellationTokenSource = new CancellationTokenSource();
	}

	public void TaskTokenStop()
	{
		TaskCancellationTokenSource.Cancel();
		TaskIsWork = false;
	}

	public LoggerViewModel()
	{
		ErrorItems = new ConcurrentObservableCollection<ErrorItem>();
		Document = new TextDocument();
		Document.Text = "PvfUtility：\r\n";
		ClearOutPutCommand = new DelegateCommand(ClearMessage);
		ThemeSwitcher.Instance.PvfCodeThemeChangedEvent += OnThemeChanged;
	}

	public void Loaded(object obj)
	{
		UpdateHighlighting();
	}

	private void OnThemeChanged()
	{
		UpdateHighlighting();
	}

	private void UpdateHighlighting()
	{
		IThemedHighlightingManager service = AppSetting.Instance.GetService<IThemedHighlightingManager>();
		Highlighting = service.GetDefinition("LOG");
	}

	public void Error(List<ErrorItem> errs)
	{
		((DispatcherObject)Application.Current).Dispatcher.Invoke(() => ErrorItems.AddRange(errs));
	}

	public void Error(string msg)
	{
		WriteMessage("Error：" + msg);
		Trace.WriteLine(msg);
	}

	[Command]
	public void ClearErrorList()
	{
		ErrorItems?.Clear();
		ErrorItems = null;
		ErrorItems = new ConcurrentObservableCollection<ErrorItem>();
	}

	[Command]
	public void OnCopyErrorSelectedString(ErrorItem errorItem)
	{
		if (errorItem != null)
		{
			AppCore.CopyString(errorItem.ToString());
		}
	}

	public MessageResult ShowDialog(string msg, string? caption = null)
	{
		return AppCore.ViewModelBase.ShowDialogResult(msg, caption);
	}

	public MessageResult ShowDialogResult(IMessageBoxService service, string msg, string? caption = null)
	{
		return AppCore.ViewModelBase.ShowDialogResult(service, msg, caption);
	}

	public void ShowMsg(string msg, bool isError = false, string? caption = null, bool loggerError = false)
	{
		AppCore.ViewModelBase.ShowMsg(msg, isError, caption);
		if (loggerError)
		{
			Error(msg);
		}
	}

	public void Debug(string msg)
	{
		WriteMessage(msg);
	}

	public void Debug(object obj)
	{
		Debug(obj?.ToString());
	}

	private void WriteMessage(string message)
	{
		try
		{
			Trace.WriteLine(message);
			((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)(() => Document.Insert(Document.TextLength, DateTime.Now.ToString("HH:mm:ss") + " " + message + "\r\n")), Array.Empty<object>());
		}
		catch (Exception)
		{
		}
	}

	public void ClearMessage()
	{
		Document.Text = "";
		WriteMessage(AppCore.ViewModelBase.AppName + ":");
	}

	public void Warning(string msg)
	{
		WriteMessage("Warning：" + msg);
	}

	public void Success(string msg)
	{
		WriteMessage("Success：" + msg);
	}

	public async Task ShowNotification(NotificationViewModel vm)
	{
		await AppCore.ViewModelBase.ShowNotification(vm);
	}

	public async Task ShowNotification<TIcommandParm>(NotificationViewModel<TIcommandParm> vm)
	{
		await AppCore.ViewModelBase.ShowNotification(vm);
	}

	public void ProgressUpdate(double value)
	{
		if (value != AppCore.ViewModelBase.Progress)
		{
			AppCore.ViewModelBase.Progress = value;
		}
	}

	public async Task TreeListAddFiles(PooledList<string> fileList)
	{
		Task.Run(delegate
		{
			AppCore.ViewModelBase.PVF.Init();
		});
		await AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.CreateTrees(fileList);
	}

	public Window CreateLoadingWindow(string title, Window? owner = null)
	{
		if (owner == null)
		{
			((DispatcherObject)Application.Current).Dispatcher.Invoke(() => owner = Application.Current.MainWindow);
		}
		return AppCore.CreateLoading(title, owner);
	}

	public void ShowLoadingWindow(Window win)
	{
		((DispatcherObject)Application.Current).Dispatcher.Invoke(() => win.Show());
	}

	public void CloseLoadingWindow(Window win)
	{
		if (win != null)
		{
			((DispatcherObject)Application.Current).Dispatcher.Invoke(() => win.Close());
		}
	}

	public void ErrorUploadDialog(Exception e, string caption)
	{
		App.ShowAppError(e, caption);
	}

	public void ShowSavePvfPackOptionsDialog()
	{
		((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
		{
			DialogPvfPackSaveOptions dialogPvfPackSaveOptions = new DialogPvfPackSaveOptions();
			dialogPvfPackSaveOptions.Owner = Application.Current.MainWindow;
			dialogPvfPackSaveOptions.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			dialogPvfPackSaveOptions.ShowDialog();
		});
	}

	public void AddFileListToSearchPanel(IEnumerable<string> fileList)
	{
		AppCore.ViewModelBase.SearchResultViewModel.AddSearchResult(new PooledList<string>(fileList), "导入结果");
	}

	public void GoToTreeListNode(string filePath)
	{
		AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(filePath);
	}

	public string GetStr(string key)
	{
		return ((string)Application.Current.TryFindResource(key)).Replace("\\r\\n", "\r\n").Replace("\\t", "\t");
	}

	public string GetStrNoReplace(string key)
	{
		return Application.Current.TryFindResource(key).ToString();
	}

	public void OpenPvfFileDocument(string filePath, bool goToNode = false)
	{
		AppCore.ViewModelBase.RootDocument.AddDocument(filePath, goToNode);
	}

	public void SetDocumentFocused(string filePath, bool goToNode = false)
	{
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)(() => AppCore.ViewModelBase.RootDocument.AddDocument(filePath, goToNode)), Array.Empty<object>());
	}

	public async Task AddFileListToCurrentSearchPanel(IEnumerable<string> filleList, bool expandAllNodes = true)
	{
		await AppCore.ViewModelBase.SearchResultViewModel.AddSearchToCurrent(new PooledList<string>(filleList));
		if (expandAllNodes)
		{
			AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.Service.ExpandAllNodes();
		}
	}

	public void AddFileListToNewSearchPanel(IEnumerable<string> filleList, string title)
	{
		AppCore.ViewModelBase.SearchResultViewModel.AddSearchResult(new PooledList<string>(filleList), title);
	}

	public Dispatcher GetAppDispatcher()
	{
		return ((DispatcherObject)Application.Current).Dispatcher;
	}

	public void OpenNpcShopEditor(string filePath)
	{
		if (AppCore.ViewModelBase.PVF.PvfIsOpen && AppCore.ViewModelBase.PVF.GetFile(filePath) != null)
		{
			WinNpcShopEditor winNpcShopEditor = new WinNpcShopEditor(filePath);
			winNpcShopEditor.Owner = Application.Current.MainWindow;
			winNpcShopEditor.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			winNpcShopEditor.Show();
		}
	}
}
