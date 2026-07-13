using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass10_0
	{
		public MainWindowViewModel Ik2suvTPdy;

		public NotificationViewModel duOsGWErlU;

		public _003C_003Ec__DisplayClass10_0()
		{
		}

		internal async Task FBrsiSVorJ()
		{
			await Ik2suvTPdy.AppNotificationService.CreateCustomNotification(duOsGWErlU).ShowAsync();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass11_0<T> where T : notnull
	{
		public MainWindowViewModel lmesQsGc03;

		public NotificationViewModel<T> XV1sayFo8D;

		public _003C_003Ec__DisplayClass11_0()
		{
		}

		internal async Task L1usxpcPPd()
		{
			await lmesQsGc03.AppNotificationService.CreateCustomNotification(XV1sayFo8D).ShowAsync();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass14_0
	{
		public MainWindowViewModel w9es6LVkqJ;

		public string xQEs10hmGL;

		public string PEDswRtBGI;

		public bool K4isod4Vii;

		public _003C_003Ec__DisplayClass14_0()
		{
		}

		internal void Ssdsgu4l7n()
		{
			w9es6LVkqJ.MessageBoxService.ShowMessage(xQEs10hmGL, PEDswRtBGI, MessageButton.OK, K4isod4Vii ? MessageIcon.Error : MessageIcon.Information);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass15_0
	{
		public MessageResult c3lsL1lKIK;

		public MainWindowViewModel HYbsngLcvt;

		public string yUGsqIHvKY;

		public string MnusdhqBl4;

		public _003C_003Ec__DisplayClass15_0()
		{
		}

		internal void q0MssIP3eS()
		{
			c3lsL1lKIK = HYbsngLcvt.MessageBoxService.ShowMessage(yUGsqIHvKY, MnusdhqBl4, MessageButton.YesNo, MessageIcon.Question);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass16_0
	{
		public MessageResult a6YstkUgZA;

		public IMessageBoxService RItsbdu8ra;

		public string kWHsIIO1y9;

		public string rBasEIYn7o;

		public _003C_003Ec__DisplayClass16_0()
		{
		}

		internal void y3Hse0DGxC()
		{
			a6YstkUgZA = RItsbdu8ra.ShowMessage(kWHsIIO1y9, rBasEIYn7o, MessageButton.YesNo, MessageIcon.Question);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass28_0
	{
		public MainWindowViewModel K37sKBT6o3;

		public int uXKs96K7pt;

		public _003C_003Ec__DisplayClass28_0()
		{
		}

		internal void aTysON6ATP()
		{
			K37sKBT6o3.Progress = uXKs96K7pt / 100;
		}
	}

	[CompilerGenerated]
	private string W5UBIjfXNV;

	private TaskbarItemProgressState Hv1BEMnLAL;

	private double UvbBOu06Fn;

	[CompilerGenerated]
	private readonly Progress<double> R4VBKII8jm;

	[CompilerGenerated]
	private BarViewModel HGtB9cDRYo;

	[CompilerGenerated]
	private PvfTreeViewModel DWUBPGqWOK;

	[CompilerGenerated]
	private DocumentRoot RunBZ7jGnk;

	[CompilerGenerated]
	private SearchResultTreeViewModel AT9BJSZZJ6;

	[CompilerGenerated]
	private ImagePacks2ViewModel FL6B0wwsPg;

	[CompilerGenerated]
	private GameLoginViewModel A0OB7FJbeC;

	public Login.LoginViewModel LoginViewModel { get; } = new Login.LoginViewModel();

	private bool IsLoaded;

	public string AppName
	{
		[CompilerGenerated]
		get
		{
			return W5UBIjfXNV;
		}
		[CompilerGenerated]
		set
		{
			W5UBIjfXNV = value;
		}
	}

	public IDockLayoutManagerService DockLayoutManagerService => GetService<IDockLayoutManagerService>();

	public IDocumentGroupService DocumentGroupService => GetService<IDocumentGroupService>();

	[ServiceProperty(Key = "NotificationService")]
	protected virtual INotificationService AppNotificationService => GetService<INotificationService>(ServiceSearchMode.PreferParents);

	protected virtual IMessageBoxService MessageBoxService => GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);

	public TaskbarItemProgressState TaskbarItemProgressState
	{
		get
		{
			return Hv1BEMnLAL;
		}
		set
		{
			Hv1BEMnLAL = value;
			RaisePropertyChanged("TaskbarItemProgressState");
		}
	}

	public double Progress
	{
		get
		{
			return UvbBOu06Fn;
		}
		set
		{
			UvbBOu06Fn = value;
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

	public Progress<double> MainProgress
	{
		[CompilerGenerated]
		get
		{
			return R4VBKII8jm;
		}
	}

	public BarViewModel BarsVm
	{
		[CompilerGenerated]
		get
		{
			return HGtB9cDRYo;
		}
		[CompilerGenerated]
		set
		{
			HGtB9cDRYo = value;
		}
	}

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

	public PvfTreeViewModel PvfFileTreeViewModel
	{
		[CompilerGenerated]
		get
		{
			return DWUBPGqWOK;
		}
		[CompilerGenerated]
		set
		{
			DWUBPGqWOK = value;
		}
	}

	public DocumentRoot RootDocument
	{
		[CompilerGenerated]
		get
		{
			return RunBZ7jGnk;
		}
		[CompilerGenerated]
		set
		{
			RunBZ7jGnk = value;
		}
	}

	public SearchResultTreeViewModel SearchResultViewModel
	{
		[CompilerGenerated]
		get
		{
			return AT9BJSZZJ6;
		}
		[CompilerGenerated]
		set
		{
			AT9BJSZZJ6 = value;
		}
	}

	public ImagePacks2ViewModel ImagePacks2ViewModel
	{
		[CompilerGenerated]
		get
		{
			return FL6B0wwsPg;
		}
		[CompilerGenerated]
		set
		{
			FL6B0wwsPg = value;
		}
	}

	public GameLoginViewModel GameLoginViewModel
	{
		[CompilerGenerated]
		get
		{
			return A0OB7FJbeC;
		}
		[CompilerGenerated]
		set
		{
			A0OB7FJbeC = value;
		}
	}

	public async Task ShowNotification(NotificationViewModel vm)
	{
		_003C_003Ec__DisplayClass10_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass10_0();
		CS_0024_003C_003E8__locals4.Ik2suvTPdy = this;
		CS_0024_003C_003E8__locals4.duOsGWErlU = vm;
		await ((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Func<Task>)async delegate
		{
			await CS_0024_003C_003E8__locals4.Ik2suvTPdy.AppNotificationService.CreateCustomNotification(CS_0024_003C_003E8__locals4.duOsGWErlU).ShowAsync();
		}, Array.Empty<object>());
	}

	public async Task ShowNotification<T>(NotificationViewModel<T> vm)
	{
		_003C_003Ec__DisplayClass11_0<T> CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass11_0<T>();
		CS_0024_003C_003E8__locals4.lmesQsGc03 = this;
		CS_0024_003C_003E8__locals4.XV1sayFo8D = vm;
		await ((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Func<Task>)async delegate
		{
			await CS_0024_003C_003E8__locals4.lmesQsGc03.AppNotificationService.CreateCustomNotification(CS_0024_003C_003E8__locals4.XV1sayFo8D).ShowAsync();
		}, Array.Empty<object>());
	}

	public void ShowMsg(string msg, bool isError, string? caption = null)
	{
		_003C_003Ec__DisplayClass14_0 CS_0024_003C_003E8__locals10 = new _003C_003Ec__DisplayClass14_0();
		CS_0024_003C_003E8__locals10.w9es6LVkqJ = this;
		CS_0024_003C_003E8__locals10.xQEs10hmGL = msg;
		CS_0024_003C_003E8__locals10.PEDswRtBGI = caption;
		CS_0024_003C_003E8__locals10.K4isod4Vii = isError;
		CS_0024_003C_003E8__locals10.PEDswRtBGI = CS_0024_003C_003E8__locals10.PEDswRtBGI ?? AppName;
		((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
		{
			CS_0024_003C_003E8__locals10.w9es6LVkqJ.MessageBoxService.ShowMessage(CS_0024_003C_003E8__locals10.xQEs10hmGL, CS_0024_003C_003E8__locals10.PEDswRtBGI, MessageButton.OK, CS_0024_003C_003E8__locals10.K4isod4Vii ? MessageIcon.Error : MessageIcon.Information);
		});
	}

	public MessageResult ShowDialogResult(string msg, string? caption = null)
	{
		_003C_003Ec__DisplayClass15_0 CS_0024_003C_003E8__locals11 = new _003C_003Ec__DisplayClass15_0();
		CS_0024_003C_003E8__locals11.HYbsngLcvt = this;
		CS_0024_003C_003E8__locals11.yUGsqIHvKY = msg;
		CS_0024_003C_003E8__locals11.MnusdhqBl4 = caption;
		CS_0024_003C_003E8__locals11.MnusdhqBl4 = CS_0024_003C_003E8__locals11.MnusdhqBl4 ?? AppName;
		CS_0024_003C_003E8__locals11.c3lsL1lKIK = MessageResult.None;
		((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
		{
			CS_0024_003C_003E8__locals11.c3lsL1lKIK = CS_0024_003C_003E8__locals11.HYbsngLcvt.MessageBoxService.ShowMessage(CS_0024_003C_003E8__locals11.yUGsqIHvKY, CS_0024_003C_003E8__locals11.MnusdhqBl4, MessageButton.YesNo, MessageIcon.Question);
		});
		return CS_0024_003C_003E8__locals11.c3lsL1lKIK;
	}

	public MessageResult ShowDialogResult(IMessageBoxService service, string msg, string? caption = null)
	{
		_003C_003Ec__DisplayClass16_0 CS_0024_003C_003E8__locals11 = new _003C_003Ec__DisplayClass16_0();
		CS_0024_003C_003E8__locals11.RItsbdu8ra = service;
		CS_0024_003C_003E8__locals11.kWHsIIO1y9 = msg;
		CS_0024_003C_003E8__locals11.rBasEIYn7o = caption;
		CS_0024_003C_003E8__locals11.rBasEIYn7o = CS_0024_003C_003E8__locals11.rBasEIYn7o ?? AppName;
		CS_0024_003C_003E8__locals11.a6YstkUgZA = MessageResult.None;
		((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
		{
			CS_0024_003C_003E8__locals11.a6YstkUgZA = CS_0024_003C_003E8__locals11.RItsbdu8ra.ShowMessage(CS_0024_003C_003E8__locals11.kWHsIIO1y9, CS_0024_003C_003E8__locals11.rBasEIYn7o, MessageButton.YesNo, MessageIcon.Question);
		});
		return CS_0024_003C_003E8__locals11.a6YstkUgZA;
	}

	public void ProgressBarShow(int nValue, int nMaxValue)
	{
		_003C_003Ec__DisplayClass28_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass28_0();
		CS_0024_003C_003E8__locals5.K37sKBT6o3 = this;
		CS_0024_003C_003E8__locals5.uXKs96K7pt = nValue * 100 / nMaxValue;
		if ((double)CS_0024_003C_003E8__locals5.uXKs96K7pt != Progress)
		{
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				CS_0024_003C_003E8__locals5.K37sKBT6o3.Progress = CS_0024_003C_003E8__locals5.uXKs96K7pt / 100;
			});
		}
	}

	private void cIVBnPNvt9(double P_0)
	{
		if (P_0 != UvbBOu06Fn)
		{
			Progress = P_0;
		}
	}

	public MainWindowViewModel()
	{
		W5UBIjfXNV = "PvfUtility";
		try
		{
			AppCore.ViewModelBase = this;
			AppSetting.Instance.MacroGroup.Init();
			R4VBKII8jm = new Progress<double>(cIVBnPNvt9);
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
			await Task.Run((Func<Task?>)U4lBqPVLhl);
			RootDocument.AddControl(PvfFileDocumentType.起始页);
		}
		catch (Exception ex2)
		{
			AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ClosePvfError"), ex2.Message));
		}
		loading.Close();
	}

	private async Task U4lBqPVLhl()
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
			NFjBd0hUXY();
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

	private void NFjBd0hUXY()
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
		nQyBeGZsR0(resultKey);
	}

	private void nQyBeGZsR0(string P_0)
	{
		VeiwSearchPvf veiwSearchPvf = new VeiwSearchPvf();
		veiwSearchPvf.Owner = Application.Current.MainWindow;
		veiwSearchPvf.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		veiwSearchPvf.Show();
		SearchResultViewModel.TheSpecifiedFind(P_0);
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

	[CompilerGenerated]
	private async Task rLeBt04r8i()
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
	}

	[CompilerGenerated]
	private void nqNBbyurgr()
	{
		Task.Run((Func<Task<ResultData>?>)WebApiServer.Instance.Start);
		PraserInfoProviderConfiger.Init();
		_ = ServicePvfTabComment.Instance;
		AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.XmlToModel();
		RegPvfRegistered();
	}
}
