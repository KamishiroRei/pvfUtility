using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SevenZip;

[ComImport]
[Guid("23170F69-40C1-278A-0000-000300030000")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IInStream
{
	int Read([Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data, uint size);

	void Seek(long offset, SeekOrigin seekOrigin, IntPtr newPosition);
}
