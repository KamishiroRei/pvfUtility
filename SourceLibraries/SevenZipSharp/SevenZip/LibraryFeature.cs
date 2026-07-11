using System;

namespace SevenZip;

[Flags]
[CLSCompliant(false)]
public enum LibraryFeature : uint
{
	None = 0u,
	Extract7z = 1u,
	Extract7zLZMA2 = 2u,
	Extract7zAll = 7u,
	ExtractZip = 8u,
	ExtractRar = 0x10u,
	ExtractGzip = 0x20u,
	ExtractBzip2 = 0x40u,
	ExtractTar = 0x80u,
	ExtractXz = 0x100u,
	ExtractAll = 0x1FFu,
	Compress7z = 0x200u,
	Compress7zLZMA2 = 0x400u,
	Compress7zAll = 0xE00u,
	CompressTar = 0x1000u,
	CompressGzip = 0x2000u,
	CompressBzip2 = 0x4000u,
	CompressXz = 0x8000u,
	CompressZip = 0x10000u,
	CompressAll = 0x1FE00u,
	Modify = 0x20000u
}
