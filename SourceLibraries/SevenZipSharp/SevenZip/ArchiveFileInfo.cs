using System;
using System.Globalization;

namespace SevenZip;

public struct ArchiveFileInfo
{
	[CLSCompliant(false)]
	public int Index { get; set; }

	public string FileName { get; set; }

	public DateTime LastWriteTime { get; set; }

	public DateTime CreationTime { get; set; }

	public DateTime LastAccessTime { get; set; }

	[CLSCompliant(false)]
	public ulong Size { get; set; }

	[CLSCompliant(false)]
	public uint Crc { get; set; }

	[CLSCompliant(false)]
	public uint Attributes { get; set; }

	public bool IsDirectory { get; set; }

	public bool Encrypted { get; set; }

	public string Comment { get; set; }

	public string Method { get; set; }

	public override bool Equals(object obj)
	{
		if (obj is ArchiveFileInfo afi)
		{
			return Equals(afi);
		}
		return false;
	}

	public bool Equals(ArchiveFileInfo afi)
	{
		if (afi.Index == Index)
		{
			return afi.FileName == FileName;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return FileName.GetHashCode() ^ Index;
	}

	public override string ToString()
	{
		return "[" + Index.ToString(CultureInfo.CurrentCulture) + "] " + FileName;
	}

	public static bool operator ==(ArchiveFileInfo afi1, ArchiveFileInfo afi2)
	{
		return afi1.Equals(afi2);
	}

	public static bool operator !=(ArchiveFileInfo afi1, ArchiveFileInfo afi2)
	{
		return !afi1.Equals(afi2);
	}
}
