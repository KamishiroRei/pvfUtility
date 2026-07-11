using System;
using System.Runtime.InteropServices;

namespace ICSharpCode.AvalonEdit.Editing;

[ComImport]
[Guid("aa80e801-2021-11d2-93e0-0060b067b86e")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ITfThreadMgr
{
	void Activate(out int clientId);

	void Deactivate();

	void CreateDocumentMgr(out IntPtr docMgr);

	void EnumDocumentMgrs(out IntPtr enumDocMgrs);

	void GetFocus(out IntPtr docMgr);

	void SetFocus(IntPtr docMgr);

	void AssociateFocus(IntPtr hwnd, IntPtr newDocMgr, out IntPtr prevDocMgr);

	void IsThreadFocus([MarshalAs(UnmanagedType.Bool)] out bool isFocus);

	void GetFunctionProvider(ref Guid classId, out IntPtr funcProvider);

	void EnumFunctionProviders(out IntPtr enumProviders);

	void GetGlobalCompartment(out IntPtr compartmentMgr);
}
