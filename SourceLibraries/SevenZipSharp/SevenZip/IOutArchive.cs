using System;
using System.Runtime.InteropServices;

namespace SevenZip;

[ComImport]
[Guid("23170F69-40C1-278A-0000-000600A00000")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IOutArchive
{
	[PreserveSig]
	int UpdateItems([MarshalAs(UnmanagedType.Interface)] ISequentialOutStream outStream, uint numItems, [MarshalAs(UnmanagedType.Interface)] IArchiveUpdateCallback updateCallback);

	void GetFileTimeType(IntPtr type);
}
