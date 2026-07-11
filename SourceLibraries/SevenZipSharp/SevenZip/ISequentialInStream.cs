using System.Runtime.InteropServices;

namespace SevenZip;

[ComImport]
[Guid("23170F69-40C1-278A-0000-000300010000")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ISequentialInStream
{
	int Read([Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] data, uint size);
}
