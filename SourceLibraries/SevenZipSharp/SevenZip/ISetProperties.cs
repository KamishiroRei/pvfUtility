using System;
using System.Runtime.InteropServices;

namespace SevenZip;

[ComImport]
[Guid("23170F69-40C1-278A-0000-000600030000")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ISetProperties
{
	int SetProperties(IntPtr names, IntPtr values, int numProperties);
}
