using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using Nito.AsyncEx;
using PvfCode.Compatibility;
using PvfCode.Localization;
using PvfCode.LoggerBase;
using PvfCode.Models.Images;
using PvfCode.Services;
using PvfCode.Services.TimeServices;
using PvfCode.SplashScreen;
using PvfCode.ViewModels;
using PvfCode.Views;
using ServiceLocator;
using SevenZip;
using ThemedDemo;

namespace PvfCode;

public class App : Application
{
	private static bool IsRecoveredXamlSelfTest =>
		string.Equals(
			Environment.GetEnvironmentVariable("PVFUTILITY_RECOVERED_XAML_SELF_TEST"),
			"1",
			StringComparison.Ordinal);

	private static readonly AsyncLock UnhandledExceptionLock = new AsyncLock();

	private static readonly AsyncLock AppErrorLock = new AsyncLock();

	private CancellationToken backgroundServiceCancellationToken;

	private CleanMemoryTimeService cleanMemoryService;

	private PvfAutoBackupService autoBackupService;

	private GameProcessTimeService gameProcessTimeService;

	private bool backgroundServicesStarted;

	private bool contentLoaded;

	static App()
	{
		RecoveredAssemblyResolver.Register();
		DevExpressNet10Compatibility.Apply();
		if (IsRecoveredXamlSelfTest)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			ServiceInjector.InjectServices();
			RegisterRecoveredServices();
			return;
		}

		try
		{
			string path = Path.Combine(AppContext.BaseDirectory, "7z64.dll");
			if (!File.Exists(path))
			{
				File.WriteAllBytes(path, Resource1._7z64);
			}
			path = Path.Combine(AppContext.BaseDirectory, "e_sqlite3.dll");
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			path = Path.Combine(AppContext.BaseDirectory, "WebView2Loader.dll");
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
		catch (Exception)
		{
		}
		SplashScreenManager.Create(() => new PvfCodeSplashScreenWindow()).ShowOnStartup();
		ServiceInjector.InjectServices();
		Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		RegisterRecoveredServices();
	}

	private static void RegisterRecoveredServices()
	{
		AppCore.Logger = new LoggerViewModel();
		ServiceContainer.Instance.AddService((Ilogger)AppCore.Logger);
		ServiceContainer.Instance.AddService((IRes)Res.Instance);
	}

	private async void OnStartup(object sender, StartupEventArgs eventArgs)
	{
		RegisterGlobalExceptionHandlers();
		try
		{
			ThemeSwitcher.Instance.SwitchTheme(AppSetting.Instance.NowThemeType);
			await LanguageResourceManager.ApplyLanguageAsync(AppSetting.Instance.CurrentLang);
			MainWindow mainWindow = new MainWindow();
			Application.Current.MainWindow = mainWindow;
			base.MainWindow = mainWindow;
			mainWindow.Show();
			StartBackgroundServices();
		}
		catch (Exception ex)
		{
			ShowErrorMessage(ex.ToString());
		}
	}

	public static void OnExit()
	{
		Environment.Exit(0);
	}

	private static async void ShowErrorMessage(string message)
	{
		try
		{
			if (message.Contains("OpenClipboard 失败 (0x800401D0 (CLIPBRD_E_CANT_OPEN))"))
			{
				return;
			}
			await ((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)(() =>
			{
				WinErrorPanel winErrorPanel = new WinErrorPanel(message);
				winErrorPanel.WindowStartupLocation = WindowStartupLocation.CenterScreen;
				winErrorPanel.Topmost = true;
				winErrorPanel.ShowDialog();
			}), Array.Empty<object>());
		}
		catch (Exception)
		{
		}
	}

	private void RegisterGlobalExceptionHandlers()
	{
		TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
		base.DispatcherUnhandledException += OnDispatcherUnhandledException;
		AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
	}

	private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs eventArgs)
	{
		try
		{
			Exception exception = eventArgs.Exception;
			if (exception != null)
			{
				ReportUnhandledException(exception);
			}
		}
		catch (Exception ex)
		{
			ReportUnhandledException(ex);
		}
		finally
		{
			eventArgs.SetObserved();
		}
	}

	private static void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
	{
		try
		{
			if (eventArgs.ExceptionObject is Exception ex)
			{
				ReportUnhandledException(ex);
			}
		}
		catch (Exception ex2)
		{
			ReportUnhandledException(ex2);
		}
	}

	private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs eventArgs)
	{
		try
		{
			ReportUnhandledException(eventArgs.Exception);
		}
		catch (Exception ex)
		{
			ReportUnhandledException(ex);
		}
		finally
		{
			eventArgs.Handled = true;
		}
	}

	private static async void ReportUnhandledException(Exception exception)
	{
		using (await UnhandledExceptionLock.LockAsync())
		{
			ShowErrorMessage(await BuildExceptionMessageAsync(exception));
		}
	}

	public static async void ShowAppError(Exception e, string caption)
	{
		using (await AppErrorLock.LockAsync())
		{
			ShowErrorMessage(await BuildExceptionMessageAsync(e, caption));
		}
	}

	private static async Task<string> BuildExceptionMessageAsync(Exception exception, string? caption = null)
	{
		string message = ((caption == null) ? string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_Exception_2"), "QQ：812143836", exception.ToStringDemystified()) : string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_Exception"), "QQ：812143836", caption, exception.ToStringDemystified()));
		return message;
	}

	private async void StartBackgroundServices()
	{
		if (!backgroundServicesStarted)
		{
			backgroundServicesStarted = true;
			backgroundServiceCancellationToken = default(CancellationToken);
			cleanMemoryService = new CleanMemoryTimeService();
			await cleanMemoryService.StartAsync(backgroundServiceCancellationToken);
			autoBackupService = new PvfAutoBackupService();
			await autoBackupService.StartAsync(backgroundServiceCancellationToken);
			gameProcessTimeService = new GameProcessTimeService();
			await gameProcessTimeService.StartAsync(backgroundServiceCancellationToken);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!contentLoaded)
		{
			contentLoaded = true;
			base.Startup += OnStartup;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/app.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[STAThread]
	public static int Main()
	{
		App app = new App();
		app.InitializeComponent();
		if (IsRecoveredXamlSelfTest)
		{
			return RecoveredXamlSelfTest.Run(app);
		}
		app.Run();
		return 0;
	}
}
