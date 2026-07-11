using System.Runtime.InteropServices;

namespace PvfCode.Web;

public class NewWebBrowserEvent : StandardOleMarshalObject, INewDWWebBrowserEvent
{
	private WebbrowserOnNewWindow BV5TcmS5Nw;

	public NewWebBrowserEvent(WebbrowserOnNewWindow webbrowserOnNewWindow)
	{
		BV5TcmS5Nw = webbrowserOnNewWindow;
	}

	public void BeforeNavigate2(object pDisp, ref object urlObject, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
	{
	}

	public void NewWindow2(ref object ppDisp, ref bool cancel)
	{
	}

	public void NewWindow3(object pDisp, ref bool cancel, ref object flags, ref object URLContext, ref object URL)
	{
		BV5TcmS5Nw.OnBeforeNewWindow((string)URL, out cancel);
	}
}
