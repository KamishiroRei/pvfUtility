using System;
using System.Collections.Generic;
using System.IO;

namespace SevenZip;

public static class Formats
{
	internal static readonly Dictionary<InArchiveFormat, Guid> InFormatGuids;

	internal static readonly Dictionary<OutArchiveFormat, Guid> OutFormatGuids;

	internal static readonly Dictionary<CompressionMethod, string> MethodNames;

	internal static readonly Dictionary<OutArchiveFormat, InArchiveFormat> InForOutFormats;

	private static readonly Dictionary<string, InArchiveFormat> InExtensionFormats;

	internal static readonly Dictionary<string, InArchiveFormat> InSignatureFormats;

	internal static Dictionary<InArchiveFormat, string> InSignatureFormatsReversed;

	static Formats()
	{
		InFormatGuids = new Dictionary<InArchiveFormat, Guid>
		{
			{
				InArchiveFormat.SevenZip,
				new Guid("23170f69-40c1-278a-1000-000110070000")
			},
			{
				InArchiveFormat.Arj,
				new Guid("23170f69-40c1-278a-1000-000110040000")
			},
			{
				InArchiveFormat.BZip2,
				new Guid("23170f69-40c1-278a-1000-000110020000")
			},
			{
				InArchiveFormat.Cab,
				new Guid("23170f69-40c1-278a-1000-000110080000")
			},
			{
				InArchiveFormat.Chm,
				new Guid("23170f69-40c1-278a-1000-000110e90000")
			},
			{
				InArchiveFormat.Compound,
				new Guid("23170f69-40c1-278a-1000-000110e50000")
			},
			{
				InArchiveFormat.Cpio,
				new Guid("23170f69-40c1-278a-1000-000110ed0000")
			},
			{
				InArchiveFormat.Deb,
				new Guid("23170f69-40c1-278a-1000-000110ec0000")
			},
			{
				InArchiveFormat.GZip,
				new Guid("23170f69-40c1-278a-1000-000110ef0000")
			},
			{
				InArchiveFormat.Iso,
				new Guid("23170f69-40c1-278a-1000-000110e70000")
			},
			{
				InArchiveFormat.Lzh,
				new Guid("23170f69-40c1-278a-1000-000110060000")
			},
			{
				InArchiveFormat.Lzma,
				new Guid("23170f69-40c1-278a-1000-0001100a0000")
			},
			{
				InArchiveFormat.Nsis,
				new Guid("23170f69-40c1-278a-1000-000110090000")
			},
			{
				InArchiveFormat.Rar,
				new Guid("23170f69-40c1-278a-1000-000110CC0000")
			},
			{
				InArchiveFormat.Rar4,
				new Guid("23170f69-40c1-278a-1000-000110030000")
			},
			{
				InArchiveFormat.Rpm,
				new Guid("23170f69-40c1-278a-1000-000110eb0000")
			},
			{
				InArchiveFormat.Split,
				new Guid("23170f69-40c1-278a-1000-000110ea0000")
			},
			{
				InArchiveFormat.Tar,
				new Guid("23170f69-40c1-278a-1000-000110ee0000")
			},
			{
				InArchiveFormat.Wim,
				new Guid("23170f69-40c1-278a-1000-000110e60000")
			},
			{
				InArchiveFormat.Lzw,
				new Guid("23170f69-40c1-278a-1000-000110050000")
			},
			{
				InArchiveFormat.Zip,
				new Guid("23170f69-40c1-278a-1000-000110010000")
			},
			{
				InArchiveFormat.Udf,
				new Guid("23170f69-40c1-278a-1000-000110E00000")
			},
			{
				InArchiveFormat.Xar,
				new Guid("23170f69-40c1-278a-1000-000110E10000")
			},
			{
				InArchiveFormat.Mub,
				new Guid("23170f69-40c1-278a-1000-000110E20000")
			},
			{
				InArchiveFormat.Hfs,
				new Guid("23170f69-40c1-278a-1000-000110E30000")
			},
			{
				InArchiveFormat.Dmg,
				new Guid("23170f69-40c1-278a-1000-000110E40000")
			},
			{
				InArchiveFormat.XZ,
				new Guid("23170f69-40c1-278a-1000-0001100C0000")
			},
			{
				InArchiveFormat.Mslz,
				new Guid("23170f69-40c1-278a-1000-000110D50000")
			},
			{
				InArchiveFormat.PE,
				new Guid("23170f69-40c1-278a-1000-000110DD0000")
			},
			{
				InArchiveFormat.Elf,
				new Guid("23170f69-40c1-278a-1000-000110DE0000")
			},
			{
				InArchiveFormat.Swf,
				new Guid("23170f69-40c1-278a-1000-000110D70000")
			},
			{
				InArchiveFormat.Vhd,
				new Guid("23170f69-40c1-278a-1000-000110DC0000")
			},
			{
				InArchiveFormat.Flv,
				new Guid("23170f69-40c1-278a-1000-000110D60000")
			},
			{
				InArchiveFormat.SquashFS,
				new Guid("23170f69-40c1-278a-1000-000110D20000")
			},
			{
				InArchiveFormat.Lzma86,
				new Guid("23170f69-40c1-278a-1000-0001100B0000")
			},
			{
				InArchiveFormat.Ppmd,
				new Guid("23170f69-40c1-278a-1000-0001100D0000")
			},
			{
				InArchiveFormat.TE,
				new Guid("23170f69-40c1-278a-1000-000110CF0000")
			},
			{
				InArchiveFormat.UEFIc,
				new Guid("23170f69-40c1-278a-1000-000110D00000")
			},
			{
				InArchiveFormat.UEFIs,
				new Guid("23170f69-40c1-278a-1000-000110D10000")
			},
			{
				InArchiveFormat.CramFS,
				new Guid("23170f69-40c1-278a-1000-000110D30000")
			},
			{
				InArchiveFormat.APM,
				new Guid("23170f69-40c1-278a-1000-000110D40000")
			},
			{
				InArchiveFormat.Swfc,
				new Guid("23170f69-40c1-278a-1000-000110D80000")
			},
			{
				InArchiveFormat.Ntfs,
				new Guid("23170f69-40c1-278a-1000-000110D90000")
			},
			{
				InArchiveFormat.Fat,
				new Guid("23170f69-40c1-278a-1000-000110DA0000")
			},
			{
				InArchiveFormat.Mbr,
				new Guid("23170f69-40c1-278a-1000-000110DB0000")
			},
			{
				InArchiveFormat.MachO,
				new Guid("23170f69-40c1-278a-1000-000110DF0000")
			}
		};
		OutFormatGuids = new Dictionary<OutArchiveFormat, Guid>
		{
			{
				OutArchiveFormat.SevenZip,
				new Guid("23170f69-40c1-278a-1000-000110070000")
			},
			{
				OutArchiveFormat.Zip,
				new Guid("23170f69-40c1-278a-1000-000110010000")
			},
			{
				OutArchiveFormat.BZip2,
				new Guid("23170f69-40c1-278a-1000-000110020000")
			},
			{
				OutArchiveFormat.GZip,
				new Guid("23170f69-40c1-278a-1000-000110ef0000")
			},
			{
				OutArchiveFormat.Tar,
				new Guid("23170f69-40c1-278a-1000-000110ee0000")
			},
			{
				OutArchiveFormat.XZ,
				new Guid("23170f69-40c1-278a-1000-0001100C0000")
			}
		};
		MethodNames = new Dictionary<CompressionMethod, string>
		{
			{
				CompressionMethod.Copy,
				"Copy"
			},
			{
				CompressionMethod.Deflate,
				"Deflate"
			},
			{
				CompressionMethod.Deflate64,
				"Deflate64"
			},
			{
				CompressionMethod.Lzma,
				"LZMA"
			},
			{
				CompressionMethod.Lzma2,
				"LZMA2"
			},
			{
				CompressionMethod.Ppmd,
				"PPMd"
			},
			{
				CompressionMethod.BZip2,
				"BZip2"
			}
		};
		InForOutFormats = new Dictionary<OutArchiveFormat, InArchiveFormat>
		{
			{
				OutArchiveFormat.SevenZip,
				InArchiveFormat.SevenZip
			},
			{
				OutArchiveFormat.GZip,
				InArchiveFormat.GZip
			},
			{
				OutArchiveFormat.BZip2,
				InArchiveFormat.BZip2
			},
			{
				OutArchiveFormat.Tar,
				InArchiveFormat.Tar
			},
			{
				OutArchiveFormat.XZ,
				InArchiveFormat.XZ
			},
			{
				OutArchiveFormat.Zip,
				InArchiveFormat.Zip
			}
		};
		InExtensionFormats = new Dictionary<string, InArchiveFormat>
		{
			{
				"7z",
				InArchiveFormat.SevenZip
			},
			{
				"gz",
				InArchiveFormat.GZip
			},
			{
				"tar",
				InArchiveFormat.Tar
			},
			{
				"rar",
				InArchiveFormat.Rar
			},
			{
				"zip",
				InArchiveFormat.Zip
			},
			{
				"lzma",
				InArchiveFormat.Lzma
			},
			{
				"lzh",
				InArchiveFormat.Lzh
			},
			{
				"arj",
				InArchiveFormat.Arj
			},
			{
				"bz2",
				InArchiveFormat.BZip2
			},
			{
				"cab",
				InArchiveFormat.Cab
			},
			{
				"chm",
				InArchiveFormat.Chm
			},
			{
				"deb",
				InArchiveFormat.Deb
			},
			{
				"iso",
				InArchiveFormat.Iso
			},
			{
				"rpm",
				InArchiveFormat.Rpm
			},
			{
				"wim",
				InArchiveFormat.Wim
			},
			{
				"udf",
				InArchiveFormat.Udf
			},
			{
				"mub",
				InArchiveFormat.Mub
			},
			{
				"xar",
				InArchiveFormat.Xar
			},
			{
				"hfs",
				InArchiveFormat.Hfs
			},
			{
				"dmg",
				InArchiveFormat.Dmg
			},
			{
				"Z",
				InArchiveFormat.Lzw
			},
			{
				"xz",
				InArchiveFormat.XZ
			},
			{
				"flv",
				InArchiveFormat.Flv
			},
			{
				"swf",
				InArchiveFormat.Swf
			},
			{
				"exe",
				InArchiveFormat.PE
			},
			{
				"dll",
				InArchiveFormat.PE
			},
			{
				"vhd",
				InArchiveFormat.Vhd
			}
		};
		InSignatureFormats = new Dictionary<string, InArchiveFormat>
		{
			{
				"37-7A-BC-AF-27-1C",
				InArchiveFormat.SevenZip
			},
			{
				"1F-8B-08",
				InArchiveFormat.GZip
			},
			{
				"75-73-74-61-72",
				InArchiveFormat.Tar
			},
			{
				"52-61-72-21-1A-07-00",
				InArchiveFormat.Rar4
			},
			{
				"52-61-72-21-1A-07-01-00",
				InArchiveFormat.Rar
			},
			{
				"50-4B-03-04",
				InArchiveFormat.Zip
			},
			{
				"5D-00-00-40-00",
				InArchiveFormat.Lzma
			},
			{
				"2D-6C-68",
				InArchiveFormat.Lzh
			},
			{
				"1F-9D-90",
				InArchiveFormat.Lzw
			},
			{
				"60-EA",
				InArchiveFormat.Arj
			},
			{
				"42-5A-68",
				InArchiveFormat.BZip2
			},
			{
				"4D-53-43-46",
				InArchiveFormat.Cab
			},
			{
				"49-54-53-46",
				InArchiveFormat.Chm
			},
			{
				"21-3C-61-72-63-68-3E-0A-64-65-62-69-61-6E-2D-62-69-6E-61-72-79",
				InArchiveFormat.Deb
			},
			{
				"43-44-30-30-31",
				InArchiveFormat.Iso
			},
			{
				"ED-AB-EE-DB",
				InArchiveFormat.Rpm
			},
			{
				"4D-53-57-49-4D-00-00-00",
				InArchiveFormat.Wim
			},
			{
				"udf",
				InArchiveFormat.Udf
			},
			{
				"mub",
				InArchiveFormat.Mub
			},
			{
				"78-61-72-21",
				InArchiveFormat.Xar
			},
			{
				"48-2B",
				InArchiveFormat.Hfs
			},
			{
				"FD-37-7A-58-5A",
				InArchiveFormat.XZ
			},
			{
				"46-4C-56",
				InArchiveFormat.Flv
			},
			{
				"46-57-53",
				InArchiveFormat.Swf
			},
			{
				"4D-5A",
				InArchiveFormat.PE
			},
			{
				"7F-45-4C-46",
				InArchiveFormat.Elf
			},
			{
				"78",
				InArchiveFormat.Dmg
			},
			{
				"63-6F-6E-65-63-74-69-78",
				InArchiveFormat.Vhd
			}
		};
		InSignatureFormatsReversed = new Dictionary<InArchiveFormat, string>(InSignatureFormats.Count);
		foreach (KeyValuePair<string, InArchiveFormat> inSignatureFormat in InSignatureFormats)
		{
			InSignatureFormatsReversed.Add(inSignatureFormat.Value, inSignatureFormat.Key);
		}
	}

	public static InArchiveFormat FormatByFileName(string fileName, bool reportErrors)
	{
		if (string.IsNullOrEmpty(fileName) && reportErrors)
		{
			throw new ArgumentException("File name is null or empty string!");
		}
		string text = Path.GetExtension(fileName).Substring(1);
		if (!InExtensionFormats.ContainsKey(text) && reportErrors)
		{
			throw new ArgumentException("Extension \"" + text + "\" is not a supported archive file name extension.");
		}
		return InExtensionFormats[text];
	}
}
