using System;

namespace Utools.内存;

[Serializable]
public struct IMAGE_RESOURCE_DATA_ENTRY
{
	public uint OffsetToData;

	public uint Size;

	public uint CodePage;

	public uint Reserved;
}
