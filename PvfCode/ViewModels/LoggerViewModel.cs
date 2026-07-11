#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass30_0
	{
		public LoggerViewModel IbGsCmPVb1;

		public List<ErrorItem> hqBsH9HNIJ;

		public _003C_003Ec__DisplayClass30_0()
		{
		}

		internal void QVCsTlB0BA()
		{
			IbGsCmPVb1.ErrorItems.AddRange(hqBsH9HNIJ);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass39_0
	{
		public LoggerViewModel tcNsvj0PpC;

		public string w6wsB4LqLn;

		public _003C_003Ec__DisplayClass39_0()
		{
		}

		internal void nH3sh5Hq7U()
		{
			tcNsvj0PpC.Document.Insert(tcNsvj0PpC.Document.TextLength, DateTime.Now.ToString("HH:mm:ss") + " " + w6wsB4LqLn + "\r\n");
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass48_0
	{
		public Window kW1srETLCC;

		public _003C_003Ec__DisplayClass48_0()
		{
		}

		internal void slcsFw75Ca()
		{
			kW1srETLCC = Application.Current.MainWindow;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass49_0
	{
		public Window win;

		public _003C_003Ec__DisplayClass49_0()
		{
		}

		internal void acIsWFQhol()
		{
			win.Show();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass50_0
	{
		public Window win;

		public _003C_003Ec__DisplayClass50_0()
		{
		}

		internal void iwosmMfaZE()
		{
			win.Close();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass58_0
	{
		public string hkBsfDjPD3;

		public bool JBNs5aybFS;

		public _003C_003Ec__DisplayClass58_0()
		{
		}

		internal void xg9s2JPBnp()
		{
			AppCore.ViewModelBase.RootDocument.AddDocument(hkBsfDjPD3, JBNs5aybFS);
		}
	}

	private CancellationTokenSource QXNBgpOZN8;

	[CompilerGenerated]
	private bool RDjB6wZrV4;

	[CompilerGenerated]
	private DelegateCommand xOqB1NKMBD;

	private TextDocument h5XBwI05d2;

	private IHighlightingDefinition E5XBosMDun;

	private ConcurrentObservableCollection<ErrorItem> iULBsyGNpy;

	private int OaoBLq9VIP;

	public CancellationTokenSource TaskCancellationTokenSource
	{
		get
		{
			if (QXNBgpOZN8 == null)
			{
				QXNBgpOZN8 = new CancellationTokenSource();
			}
			return QXNBgpOZN8;
		}
		set
		{
			QXNBgpOZN8 = value;
		}
	}

	public bool TaskIsWork
	{
		[CompilerGenerated]
		get
		{
			return RDjB6wZrV4;
		}
		[CompilerGenerated]
		set
		{
			RDjB6wZrV4 = value;
		}
	}

	public DelegateCommand ClearOutPutCommand
	{
		[CompilerGenerated]
		get
		{
			return xOqB1NKMBD;
		}
		[CompilerGenerated]
		set
		{
			xOqB1NKMBD = value;
		}
	}

	public TextDocument Document
	{
		get
		{
			return h5XBwI05d2;
		}
		set
		{
			h5XBwI05d2 = value;
			RaisePropertyChanged("_Document");
		}
	}

	public IHighlightingDefinition Highlighting
	{
		get
		{
			return E5XBosMDun;
		}
		set
		{
			E5XBosMDun = value;
			RaisePropertyChanged("Highlighting");
		}
	}

	public ConcurrentObservableCollection<ErrorItem> ErrorItems
	{
		get
		{
			return iULBsyGNpy;
		}
		set
		{
			iULBsyGNpy = value;
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
		ThemeSwitcher.Instance.PvfCodeThemeChangedEvent += RlLBxQxy20;
	}

	public void Loaded(object obj)
	{
		s8OBQ9A9pU();
	}

	private void RlLBxQxy20()
	{
		s8OBQ9A9pU();
	}

	private void s8OBQ9A9pU()
	{
		IThemedHighlightingManager service = AppSetting.Instance.GetService<IThemedHighlightingManager>();
		Highlighting = service.GetDefinition("LOG");
	}

	public void Error(List<ErrorItem> errs)
	{
		_003C_003Ec__DisplayClass30_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass30_0();
		CS_0024_003C_003E8__locals4.IbGsCmPVb1 = this;
		CS_0024_003C_003E8__locals4.hqBsH9HNIJ = errs;
		((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
		{
			CS_0024_003C_003E8__locals4.IbGsCmPVb1.ErrorItems.AddRange(CS_0024_003C_003E8__locals4.hqBsH9HNIJ);
		});
	}

	public void Error(string msg)
	{
		SBABaiIBAr("Error：" + msg);
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
		SBABaiIBAr(msg);
	}

	public void Debug(object obj)
	{
		Debug(obj?.ToString());
	}

	private void SBABaiIBAr(string P_0)
	{
		_003C_003Ec__DisplayClass39_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass39_0();
		CS_0024_003C_003E8__locals6.tcNsvj0PpC = this;
		CS_0024_003C_003E8__locals6.w6wsB4LqLn = P_0;
		try
		{
			Trace.WriteLine(CS_0024_003C_003E8__locals6.w6wsB4LqLn);
			((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
			{
				CS_0024_003C_003E8__locals6.tcNsvj0PpC.Document.Insert(CS_0024_003C_003E8__locals6.tcNsvj0PpC.Document.TextLength, DateTime.Now.ToString("HH:mm:ss") + " " + CS_0024_003C_003E8__locals6.w6wsB4LqLn + "\r\n");
			}, Array.Empty<object>());
		}
		catch (Exception)
		{
		}
	}

	public void ClearMessage()
	{
		Document.Text = "";
		SBABaiIBAr(AppCore.ViewModelBase.AppName + ":");
	}

	public void Warning(string msg)
	{
		SBABaiIBAr("Warning：" + msg);
	}

	public void Success(string msg)
	{
		SBABaiIBAr("Success：" + msg);
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
		_003C_003Ec__DisplayClass48_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass48_0();
		CS_0024_003C_003E8__locals4.kW1srETLCC = owner;
		if (CS_0024_003C_003E8__locals4.kW1srETLCC == null)
		{
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				CS_0024_003C_003E8__locals4.kW1srETLCC = Application.Current.MainWindow;
			});
		}
		return AppCore.CreateLoading(title, CS_0024_003C_003E8__locals4.kW1srETLCC);
	}

	public void ShowLoadingWindow(Window win)
	{
		_003C_003Ec__DisplayClass49_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass49_0();
		CS_0024_003C_003E8__locals2.win = win;
		((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
		{
			CS_0024_003C_003E8__locals2.win.Show();
		});
	}

	public void CloseLoadingWindow(Window win)
	{
		_003C_003Ec__DisplayClass50_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass50_0();
		CS_0024_003C_003E8__locals3.win = win;
		if (CS_0024_003C_003E8__locals3.win != null)
		{
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				CS_0024_003C_003E8__locals3.win.Close();
			});
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
		_003C_003Ec__DisplayClass58_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass58_0();
		CS_0024_003C_003E8__locals4.hkBsfDjPD3 = filePath;
		CS_0024_003C_003E8__locals4.JBNs5aybFS = goToNode;
		((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			AppCore.ViewModelBase.RootDocument.AddDocument(CS_0024_003C_003E8__locals4.hkBsfDjPD3, CS_0024_003C_003E8__locals4.JBNs5aybFS);
		}, Array.Empty<object>());
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
