using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using PvfCode.Models.Enums;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.Views;
using PvfCode.Web;
using Utools;
using Utools.系统;

namespace PvfCode.ViewModels.DocumentFolder;

public class DocumentIndexViewModel : DocumentBase
{
	private WebView2 webView2;

	private bool isWebView2Loaded;

	private bool webBrowserNewWindowHandlerAttached;

	public bool InsertView2 { get; set; }

	public string Url
	{
		get
		{
			return "about:blank";
		}
	}

	public DocumentIndexViewModel()
		: base(AppSetting.Instance.GetIlogger()?.GetStr("DocumentName_StartPage"))
	{
		base.DocumentType = PvfFileDocumentType.起始页;
		InsertView2 = WindowsEx.CheckIsInsertMicrosoftEdgeRuntime();
	}

	[Command]
	public async void LoadedView2(Grid gridRoot)
	{
		if (!isWebView2Loaded)
		{
			int errid = 0;
			try
			{
				webView2 = new WebView2();
				errid++;
				webView2.Source = new Uri(Url);
				errid++;
				gridRoot.Children.Add(webView2);
				errid++;
				await webView2.EnsureCoreWebView2Async();
				isWebView2Loaded = true;
				errid++;
				webView2.CoreWebView2InitializationCompleted += OnCoreWebView2InitializationCompleted;
				errid++;
				ThemeSwitcher.Instance.PvfCodeThemeChangedEvent += OnThemeChanged;
				errid++;
			}
			catch (Exception e)
			{
				App.ShowAppError(e, "起始页加载失败：" + errid);
				WindowInsertMicrosoftEdgeRuntime windowInsertMicrosoftEdgeRuntime = new WindowInsertMicrosoftEdgeRuntime();
				windowInsertMicrosoftEdgeRuntime.WindowStartupLocation = WindowStartupLocation.CenterScreen;
				windowInsertMicrosoftEdgeRuntime.Topmost = true;
				windowInsertMicrosoftEdgeRuntime.ShowDialog();
			}
		}
	}

	private void OnCoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
	{
		try
		{
			webView2.CoreWebView2.Settings.AreDevToolsEnabled = true;
			webView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
			webView2.CoreWebView2.NavigationCompleted += OnCoreWebView2NavigationCompleted;
		}
		catch (Exception)
		{
		}
	}

	private void OnCoreWebView2NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
	{
		try
		{
			ApplyThemeToWebView(AppSetting.Instance.NowThemeType);
		}
		catch (Exception)
		{
		}
	}

	private async void ApplyThemeToWebView(ThemeType theme)
	{
		try
		{
			if (webView2 != null && webView2.CoreWebView2 != null)
			{
				await webView2.CoreWebView2.ExecuteScriptAsync("SetTheme(\"" + HttpUtility.JavaScriptStringEncode(theme.ToString()) + "\")");
			}
		}
		catch (Exception)
		{
		}
	}

	private void OnThemeChanged()
	{
		ApplyThemeToWebView(AppSetting.Instance.NowThemeType);
	}

	[Command]
	public void WebBrowser(WebBrowser webBrowser)
	{
		webBrowser.Source = new Uri(Url);
		SuppressScriptErrors(webBrowser, Hide: true);
		webBrowser.LoadCompleted += OnWebBrowserLoadCompleted;
	}

	private void OnWebBrowserLoadCompleted(object sender, NavigationEventArgs e)
	{
		try
		{
			if (!webBrowserNewWindowHandlerAttached)
			{
				new WebbrowserOnNewWindow((WebBrowser)sender).BeforeNewWidnow += OnWebBrowserBeforeNewWindow;
				webBrowserNewWindowHandlerAttached = true;
			}
		}
		catch (Exception)
		{
		}
	}

	private void OnWebBrowserBeforeNewWindow(WebBrowserUrl2 browserUrl, WebBrowserEvent browserEvent)
	{
		try
		{
			string url = browserUrl.Url;
			Process process = new Process();
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = false;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			process.StandardInput.WriteLine("start " + url + "&exit");
			process.StandardInput.AutoFlush = true;
			process.WaitForExit();
			process.Close();
			browserEvent.cancel = true;
		}
		catch (Exception)
		{
		}
	}

	public void SuppressScriptErrors(WebBrowser wb, bool Hide)
	{
		try
		{
			FieldInfo field = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
			if (!(field == null))
			{
				object value = field.GetValue(wb);
				value?.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, value, new object[1] { Hide });
			}
		}
		catch (Exception)
		{
		}
	}

	public override void Dispose()
	{
		try
		{
			if (webView2 != null)
			{
				webView2.CoreWebView2InitializationCompleted -= OnCoreWebView2InitializationCompleted;
				webView2.CoreWebView2.NavigationCompleted -= OnCoreWebView2NavigationCompleted;
				webView2.Dispose();
			}
			ThemeSwitcher.Instance.PvfCodeThemeChangedEvent -= OnThemeChanged;
			webView2 = null;
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg("起始页释放失败：" + ex.Message);
			WindowInsertMicrosoftEdgeRuntime windowInsertMicrosoftEdgeRuntime = new WindowInsertMicrosoftEdgeRuntime();
			windowInsertMicrosoftEdgeRuntime.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			windowInsertMicrosoftEdgeRuntime.Topmost = true;
			windowInsertMicrosoftEdgeRuntime.ShowDialog();
		}
	}
}
