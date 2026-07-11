using System;
using System.Runtime.InteropServices;

namespace Utools.内存;

[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct IMAGE_RESOURCE_DIRECTORY_ENTRY
{
	[FieldOffset(0)]
	public uint NameRva;

	[FieldOffset(0)]
	public uint IntegerId;

	[FieldOffset(4)]
	public uint DataEntryRva;

	[FieldOffset(4)]
	public uint SubdirectoryRva;
}
