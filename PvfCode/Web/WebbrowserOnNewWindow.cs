using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;

namespace PvfCode.Web;

public class WebbrowserOnNewWindow
{
	public delegate void OnNewWindow(WebBrowserUrl2 webBrowserUrl, WebBrowserEvent webBrowserEvent);

	private static readonly BindingFlags ajGTXIBRuT;

	private System.Windows.Controls.WebBrowser gxSTpTF2SM;

	[CompilerGenerated]
	private OnNewWindow evaTUgqGwl;

	public event OnNewWindow BeforeNewWidnow
	{
		[CompilerGenerated]
		add
		{
			OnNewWindow onNewWindow = evaTUgqGwl;
			OnNewWindow onNewWindow2;
			do
			{
				onNewWindow2 = onNewWindow;
				OnNewWindow value2 = (OnNewWindow)Delegate.Combine(onNewWindow2, value);
				onNewWindow = Interlocked.CompareExchange(ref evaTUgqGwl, value2, onNewWindow2);
			}
			while ((object)onNewWindow != onNewWindow2);
		}
		[CompilerGenerated]
		remove
		{
			OnNewWindow onNewWindow = evaTUgqGwl;
			OnNewWindow onNewWindow2;
			do
			{
				onNewWindow2 = onNewWindow;
				OnNewWindow value2 = (OnNewWindow)Delegate.Remove(onNewWindow2, value);
				onNewWindow = Interlocked.CompareExchange(ref evaTUgqGwl, value2, onNewWindow2);
			}
			while ((object)onNewWindow != onNewWindow2);
		}
	}

	public WebbrowserOnNewWindow(System.Windows.Controls.WebBrowser webBrowser)
	{
		gxSTpTF2SM = webBrowser ?? throw new Exception();
		h1XT7mcRF2();
	}

	private void h1XT7mcRF2()
	{
		object? value = ((object)gxSTpTF2SM).GetType().GetProperty("AxIWebBrowser2", ajGTXIBRuT).GetValue(gxSTpTF2SM, null);
		NewWebBrowserEvent sink = new NewWebBrowserEvent(this);
		new AxHost.ConnectionPointCookie(value, sink, typeof(INewDWWebBrowserEvent));
	}

	public void OnBeforeNewWindow(string url, out bool cancel)
	{
		OnNewWindow onNewWindow = evaTUgqGwl;
		WebBrowserUrl2 webBrowserUrl = new WebBrowserUrl2(url, null);
		WebBrowserEvent webBrowserEvent = new WebBrowserEvent();
		onNewWindow?.Invoke(webBrowserUrl, webBrowserEvent);
		cancel = webBrowserEvent.cancel;
	}

	static WebbrowserOnNewWindow()
	{
		ajGTXIBRuT = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.CreateInstance;
	}
}
