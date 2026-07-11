using System.Runtime.InteropServices;

namespace SevenZip;

[ComImport]
[Guid("23170F69-40C1-278A-0000-000500110000")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ICryptoGetTextPassword2
{
	[PreserveSig]
	int CryptoGetTextPassword2(ref int passwordIsDefined, [MarshalAs(UnmanagedType.BStr)] out string password);
}
