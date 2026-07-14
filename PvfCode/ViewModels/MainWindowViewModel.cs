using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Localization;
using PvfCode.MVVMServices;
using PvfCode.Services.PvfParsingNew;
using PvfCode.ViewModels.Bars;
using PvfCode.ViewModels.DocumentFolder;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.ViewModels.Game;
using PvfCode.ViewModels.TreeFolder;
using PvfCode.Views.SearchPvf;
using ServiceLocator;
using SqlSugar.Extensions;
using Utools;
using dRvgUYFXgiUlumD56M2;

namespace PvfCode.ViewModels;

public class MainWindowViewModel : ViewModelBase, IDisposable
{
	private TaskbarItemProgressState taskbarItemProgressState;

	private double progress;

	private readonly Progress<double> mainProgress;

	public Login.LoginViewModel LoginViewModel { get; } = new Login.LoginViewModel();

	private bool IsLoaded;

	public string AppName { get; set; }

	public IDockLayoutManagerService DockLayoutManagerService => GetService<IDockLayoutManagerService>();

	public IDocumentGroupService DocumentGroupService => GetService<IDocumentGroupService>();

	[ServiceProperty(Key = "NotificationService")]
	protected virtual INotificationService AppNotificationService => GetService<INotificationService>(ServiceSearchMode.PreferParents);

	protected virtual IMessageBoxService MessageBoxService => GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);

	public TaskbarItemProgressState TaskbarItemProgressState
	{
		get
		{
			return taskbarItemProgressState;
		}
		set
		{
			taskbarItemProgressState = value;
			RaisePropertyChanged("TaskbarItemProgressState");
		}
	}

	public double Progress
	{
		get
		{
			return progress;
		}
		set
		{
			progress = value;
			RaisePropertyChanged("Progress");
			if (value >= 1.0)
			{
				if (TaskbarItemProgressState != TaskbarItemProgressState.None)
				{
					TaskbarItemProgressState = TaskbarItemProgressState.None;
				}
			}
			else if (TaskbarItemProgressState != TaskbarItemProgressState.Normal)
			{
				TaskbarItemProgressState = TaskbarItemProgressState.Normal;
			}
		}
	}

	public Progress<double> MainProgress => mainProgress;

	public BarViewModel BarsVm { get; set; }

	public PvfGroup PVF
	{
		get
		{
			return GetProperty(() => PVF);
		}
		set
		{
			SetProperty<PvfGroup>(() => PVF, value);
		}
	}

	public PvfTreeViewModel PvfFileTreeViewModel { get; set; }

	public DocumentRoot RootDocument { get; set; }

	public SearchResultTreeViewModel SearchResultViewModel { get; set; }

	public ImagePacks2ViewModel ImagePacks2ViewModel { get; set; }

	public GameLoginViewModel GameLoginViewModel { get; set; }

	public async Task ShowNotification(NotificationViewModel vm)
	{
		await ((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Func<Task>)(async () => await AppNotificationService.CreateCustomNotification(vm).ShowAsync()), Array.Empty<object>());
	}

	public async Task ShowNotification<T>(NotificationViewModel<T> vm)
	{
		await ((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Func<Task>)(async () => await AppNotificationService.CreateCustomNotification(vm).ShowAsync()), Array.Empty<object>());
	}

	public void ShowMsg(string msg, bool isError, string? caption = null)
	{
		caption ??= AppName;
		((DispatcherObject)Application.Current).Dispatcher.Invoke(() => MessageBoxService.ShowMessage(msg, caption, MessageButton.OK, isError ? MessageIcon.Error : MessageIcon.Information));
	}

	public MessageResult ShowDialogResult(string msg, string? caption = null)
	{
		caption ??= AppName;
		MessageResult result = MessageResult.None;
		((DispatcherObject)Application.Current).Dispatcher.Invoke(() => result = MessageBoxService.ShowMessage(msg, caption, MessageButton.YesNo, MessageIcon.Question));
		return result;
	}

	public MessageResult ShowDialogResult(IMessageBoxService service, string msg, string? caption = null)
	{
		caption ??= AppName;
		MessageResult result = MessageResult.None;
		((DispatcherObject)Application.Current).Dispatcher.Invoke(() => result = service.ShowMessage(msg, caption, MessageButton.YesNo, MessageIcon.Question));
		return result;
	}

	public void ProgressBarShow(int nValue, int nMaxValue)
	{
		int progressPercent = nValue * 100 / nMaxValue;
		if ((double)progressPercent != Progress)
		{
			((DispatcherObject)Application.Current).Dispatcher.Invoke(() => Progress = progressPercent / 100);
		}
	}

	private void UpdateProgress(double value)
	{
		if (value != progress)
		{
			Progress = value;
		}
	}

	public MainWindowViewModel()
	{
		AppName = "PvfUtility";
		try
		{
			AppCore.ViewModelBase = this;
			AppSetting.Instance.MacroGroup.Init();
			mainProgress = new Progress<double>(UpdateProgress);
			ServiceLocator.ServiceContainer.Instance.AddService((IProgress<double>)MainProgress);
			PVF = new PvfGroup();
			Progress = 100.0;
			BarsVm = new BarViewModel();
			SearchResultViewModel = new SearchResultTreeViewModel();
			PvfFileTreeViewModel = new PvfTreeViewModel(TreeViewType.FileList);
			RootDocument = new DocumentRoot();
			ImagePacks2ViewModel = new ImagePacks2ViewModel();
			GameLoginViewModel = new GameLoginViewModel();
		}
		catch (Exception e)
		{
			App.ShowAppError(e, "mainView");
		}
	}

	public async Task Clear()
	{
		WindowLoading loading = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_ClosingPvfAndClearGarbage"), Application.Current.MainWindow);
		loading.Show();
		try
		{
			try
			{
				foreach (Window window in Application.Current.Windows)
				{
					if (window != Application.Current.MainWindow)
					{
						window.Close();
					}
				}
			}
			catch (Exception)
			{
			}
			await Task.Run(ClearCoreAsync);
			RootDocument.AddControl(PvfFileDocumentType.起始页);
		}
		catch (Exception ex2)
		{
			AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ClosePvfError"), ex2.Message));
		}
		loading.Close();
	}

	private async Task ClearCoreAsync()
	{
		await ((DispatcherObject)Application.Current).Dispatcher.InvokeAsync<Task>((Func<Task>)async delegate
		{
			try
			{
				RootDocument.Clear();
				SearchResultViewModel.Clear();
				PvfFileTreeViewModel.Clear();
				qjqilnF7lAFbCxZ5lIf.Instance.Clear();
				AppCore.EditorReplaceKeywordLog.Clear();
				AppCore.EditorSearchKeywordLog.Clear();
				PVF.Clear();
				PVF = null;
				PVF = new PvfGroup();
				AppCore.Logger.ClearErrorList();
				AppCore.Logger.ClearMessage();
				ImagePack2Service.Instance.Clear();
				await Task.Run(delegate
				{
					WindowsEx.ClearMemorySilent(Process.GetCurrentProcess());
				});
				GC.Collect();
				GC.WaitForPendingFinalizers();
				GC.Collect();
			}
			catch (Exception ex)
			{
				try
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();
					WindowsEx.ClearMemorySilent(Process.GetCurrentProcess());
				}
				catch (Exception)
				{
				}
				AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ClosePvfError2"), ex.Message, ex.Source, ex.StackTrace));
				PVF.Clear();
			}
		});
	}

	public async void Load(object sender)
	{
		if (IsLoaded)
		{
			return;
		}
		try
		{
			OpenPvfFromCommandLine();
			await Task.Run(delegate
			{
				Task.Run((Func<Task<ResultData>?>)WebApiServer.Instance.Start);
				PraserInfoProviderConfiger.Init();
				_ = ServicePvfTabComment.Instance;
				AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.XmlToModel();
				RegPvfRegistered();
			});
			if (!WindowsEx.CheckIsInsertMicrosoftEdgeRuntime())
			{
				AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInstallEdgeRuntime"), "https://go.microsoft.com/fwlink/p/?LinkId=2124703"));
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show("MainViewModelErr:" + ex.Message);
		}
	}

	private void OpenPvfFromCommandLine()
	{
		try
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs == null)
			{
				return;
			}
			string[] array = commandLineArgs;
			foreach (string text in array)
			{
				string extension = Path.GetExtension(text);
				if (!string.IsNullOrEmpty(extension) && extension.ToLower() == ".pvf" && File.Exists(text))
				{
					BarsVm.OnOpenPvfFile(text);
				}
			}
		}
		catch (Exception ex)
		{
			AppCore.Logger.Error("LoadPvfCmdError：" + ex.Message);
		}
	}

	[Command]
	public void RegPvfRegistered(bool showMess = false)
	{
		try
		{
			string text = Path.Combine(AppContext.BaseDirectory, "pvfUtility.exe");
			FileTypeRegister.RegisterFileType(new FileTypeRegInfo(".pvf")
			{
				Description = AppSetting.Instance.GetIlogger().GetStr("mess_PvfPack"),
				ExePath = text,
				ExtendName = ".pvf",
				IconPath = text
			});
			FileTypeRegister.SHChangeNotify(134217728, 8192, IntPtr.Zero, IntPtr.Zero);
			if (showMess)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_RemoveSuccessRestartOrLogout"));
			}
		}
		catch (Exception)
		{
			if (showMess)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_Associate_pvfPack_Error"));
			}
		}
	}

	[Command]
	public void OnSearchPvf(string resultKey)
	{
		OpenSearchPvf(resultKey);
	}

	private void OpenSearchPvf(string resultKey)
	{
		VeiwSearchPvf veiwSearchPvf = new VeiwSearchPvf();
		veiwSearchPvf.Owner = Application.Current.MainWindow;
		veiwSearchPvf.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		veiwSearchPvf.Show();
		SearchResultViewModel.TheSpecifiedFind(resultKey);
	}

	[Command]
	public async void OnChangedWebApiPort()
	{
		try
		{
			ResultData resultData = await Task.Run((Func<Task<ResultData>?>)WebApiServer.Instance.Start);
			if (resultData.IsError)
			{
				AppCore.ShowMsg(resultData.Msg);
			}
			else
			{
				await AppSetting.Instance.SaveSetting();
			}
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_HttpServiceStartFailed"), ex.Message), isError: true);
		}
	}

	public void Dispose()
	{
	}

	[Command]
	public async void ClearMemory()
	{
		await Task.Run(delegate
		{
			WindowsEx.ClearMemorySilent(Process.GetCurrentProcess());
		});
		await Task.Run((Action)GC.Collect);
	}

	[Command]
	public void Test()
	{
		AppCore.ShowMsg("该功能当前不可用。");
	}
}
