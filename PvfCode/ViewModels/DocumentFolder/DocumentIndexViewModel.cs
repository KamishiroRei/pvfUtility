using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private bool rxafbLSwTc;

	private WebView2 OULfICuT2H;

	private bool IsLoaded;

	private bool fvSfEYBOAy;

	public bool InsertView2
	{
		[CompilerGenerated]
		get
		{
			return rxafbLSwTc;
		}
		[CompilerGenerated]
		set
		{
			rxafbLSwTc = value;
		}
	}

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
		if (!IsLoaded)
		{
			int errid = 0;
			try
			{
				OULfICuT2H = new WebView2();
				errid++;
				OULfICuT2H.Source = new Uri(Url);
				errid++;
				gridRoot.Children.Add(OULfICuT2H);
				errid++;
				await OULfICuT2H.EnsureCoreWebView2Async();
				IsLoaded = true;
				errid++;
				OULfICuT2H.CoreWebView2InitializationCompleted += h1AfLyc0gH;
				errid++;
				ThemeSwitcher.Instance.PvfCodeThemeChangedEvent += kT1fddRn5r;
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

	private void h1AfLyc0gH(object? sender, CoreWebView2InitializationCompletedEventArgs P_1)
	{
		try
		{
			OULfICuT2H.CoreWebView2.Settings.AreDevToolsEnabled = true;
			OULfICuT2H.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
			OULfICuT2H.CoreWebView2.NavigationCompleted += G4Wfn3c7pE;
		}
		catch (Exception)
		{
		}
	}

	private void G4Wfn3c7pE(object? sender, CoreWebView2NavigationCompletedEventArgs P_1)
	{
		try
		{
			Xb4fqhiplC(AppSetting.Instance.NowThemeType);
		}
		catch (Exception)
		{
		}
	}

	private async void Xb4fqhiplC(ThemeType P_0)
	{
		try
		{
			if (OULfICuT2H != null && OULfICuT2H.CoreWebView2 != null)
			{
				await OULfICuT2H.CoreWebView2.ExecuteScriptAsync("SetTheme(\"" + HttpUtility.JavaScriptStringEncode(P_0.ToString()) + "\")");
			}
		}
		catch (Exception)
		{
		}
	}

	private void kT1fddRn5r()
	{
		this?.Xb4fqhiplC(AppSetting.Instance.NowThemeType);
	}

	[Command]
	public void WebBrowser(WebBrowser webBrowser)
	{
		webBrowser.Source = new Uri(Url);
		SuppressScriptErrors(webBrowser, Hide: true);
		webBrowser.LoadCompleted += ntTfedjcJ1;
	}

	private void ntTfedjcJ1(object P_0, NavigationEventArgs P_1)
	{
		try
		{
			if (!fvSfEYBOAy)
			{
				new WebbrowserOnNewWindow((WebBrowser)P_0).BeforeNewWidnow += r8OfteF3xb;
				fvSfEYBOAy = true;
			}
		}
		catch (Exception)
		{
		}
	}

	private void r8OfteF3xb(WebBrowserUrl2 P_0, WebBrowserEvent P_1)
	{
		try
		{
			string url = P_0.Url;
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
			P_1.cancel = true;
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
			if (OULfICuT2H != null)
			{
				OULfICuT2H.CoreWebView2InitializationCompleted -= h1AfLyc0gH;
				OULfICuT2H.CoreWebView2.NavigationCompleted -= G4Wfn3c7pE;
				OULfICuT2H.Dispose();
			}
			ThemeSwitcher.Instance.PvfCodeThemeChangedEvent -= kT1fddRn5r;
			OULfICuT2H = null;
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
