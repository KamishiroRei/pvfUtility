using System;
using System.Runtime.InteropServices;

namespace Utools.内存;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct IMAGE_BASE_RELOCATION
{
	public uint VirtualAddress;

	public uint SizeOfBlock;
}
