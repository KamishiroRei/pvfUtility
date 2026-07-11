using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SevenZip;

[ComImport]
[Guid("23170F69-40C1-278A-0000-000300040000")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IOutStream
{
	[PreserveSig]
	int Write([In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data, uint size, IntPtr processedSize);

	void Seek(long offset, SeekOrigin seekOrigin, IntPtr newPosition);

	[PreserveSig]
	int SetSize(long newSize);
}
