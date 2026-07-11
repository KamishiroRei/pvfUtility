using System.Runtime.InteropServices;

namespace PvfCode.Web;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
[Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
[TypeLibType(TypeLibTypeFlags.FHidden)]
public interface INewDWWebBrowserEvent
{
	[DispId(250)]
	void BeforeNavigate2(object pDisp, ref object urlObject, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel);

	[DispId(251)]
	void NewWindow2(ref object ppDisp, ref bool cancel);

	[DispId(273)]
	void NewWindow3(object pDisp, ref bool cancel, ref object flags, ref object URLContext, ref object URL);
}
