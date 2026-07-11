using System;
using System.Runtime.InteropServices;

namespace Utools.内存;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct IMAGE_NT_HEADER32
{
	public int Signature;

	public IMAGE_FILE_HEADER FileHeader;

	public IMAGE_OPTIONAL_HEADER32 OptionalHeader;
}
