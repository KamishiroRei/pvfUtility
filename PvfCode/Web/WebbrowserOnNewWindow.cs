using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Forms;

namespace PvfCode.Web;

public class WebbrowserOnNewWindow
{
	public delegate void OnNewWindow(WebBrowserUrl2 webBrowserUrl, WebBrowserEvent webBrowserEvent);

	private static readonly BindingFlags BrowserBindingFlags;

	private System.Windows.Controls.WebBrowser webBrowser;

	public event OnNewWindow BeforeNewWidnow;

	public WebbrowserOnNewWindow(System.Windows.Controls.WebBrowser webBrowser)
	{
		this.webBrowser = webBrowser ?? throw new Exception();
		AttachBrowserEventSink();
	}

	private void AttachBrowserEventSink()
	{
		object? value = ((object)webBrowser).GetType().GetProperty("AxIWebBrowser2", BrowserBindingFlags).GetValue(webBrowser, null);
		NewWebBrowserEvent sink = new NewWebBrowserEvent(this);
		new AxHost.ConnectionPointCookie(value, sink, typeof(INewDWWebBrowserEvent));
	}

	public void OnBeforeNewWindow(string url, out bool cancel)
	{
		WebBrowserUrl2 webBrowserUrl = new WebBrowserUrl2(url, null);
		WebBrowserEvent webBrowserEvent = new WebBrowserEvent();
		BeforeNewWidnow?.Invoke(webBrowserUrl, webBrowserEvent);
		cancel = webBrowserEvent.cancel;
	}

	static WebbrowserOnNewWindow()
	{
		BrowserBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.CreateInstance;
	}
}
