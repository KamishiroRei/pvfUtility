using System;

namespace Utools.内存;

[Serializable]
public struct IMAGE_DATA_DIRECTORY
{
	public uint VirtualAddress;

	public uint Size;
}
