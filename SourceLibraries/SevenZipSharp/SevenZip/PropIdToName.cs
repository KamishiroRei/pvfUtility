using System.Collections.Generic;

namespace SevenZip;

internal static class PropIdToName
{
	public static readonly Dictionary<ItemPropId, string> PropIdNames = new Dictionary<ItemPropId, string>(46)
	{
		{
			ItemPropId.Path,
			"Path"
		},
		{
			ItemPropId.Name,
			"Name"
		},
		{
			ItemPropId.IsDirectory,
			"Folder"
		},
		{
			ItemPropId.Size,
			"Size"
		},
		{
			ItemPropId.PackedSize,
			"Packed Size"
		},
		{
			ItemPropId.Attributes,
			"Attributes"
		},
		{
			ItemPropId.CreationTime,
			"Created"
		},
		{
			ItemPropId.LastAccessTime,
			"Accessed"
		},
		{
			ItemPropId.LastWriteTime,
			"Modified"
		},
		{
			ItemPropId.Solid,
			"Solid"
		},
		{
			ItemPropId.Commented,
			"Commented"
		},
		{
			ItemPropId.Encrypted,
			"Encrypted"
		},
		{
			ItemPropId.SplitBefore,
			"Split Before"
		},
		{
			ItemPropId.SplitAfter,
			"Split After"
		},
		{
			ItemPropId.DictionarySize,
			"Dictionary Size"
		},
		{
			ItemPropId.Crc,
			"CRC"
		},
		{
			ItemPropId.Type,
			"Type"
		},
		{
			ItemPropId.IsAnti,
			"Anti"
		},
		{
			ItemPropId.Method,
			"Method"
		},
		{
			ItemPropId.HostOS,
			"Host OS"
		},
		{
			ItemPropId.FileSystem,
			"File System"
		},
		{
			ItemPropId.User,
			"User"
		},
		{
			ItemPropId.Group,
			"Group"
		},
		{
			ItemPropId.Block,
			"Block"
		},
		{
			ItemPropId.Comment,
			"Comment"
		},
		{
			ItemPropId.Position,
			"Position"
		},
		{
			ItemPropId.Prefix,
			"Prefix"
		},
		{
			ItemPropId.NumSubDirs,
			"Number of subdirectories"
		},
		{
			ItemPropId.NumSubFiles,
			"Number of subfiles"
		},
		{
			ItemPropId.UnpackVersion,
			"Unpacker version"
		},
		{
			ItemPropId.VolumeIndex,
			"VolumeIndex"
		},
		{
			ItemPropId.Volume,
			"Volume"
		},
		{
			ItemPropId.IsVolume,
			"IsVolume"
		},
		{
			ItemPropId.Offset,
			"Offset"
		},
		{
			ItemPropId.Links,
			"Links"
		},
		{
			ItemPropId.NumBlocks,
			"Number of blocks"
		},
		{
			ItemPropId.NumVolumes,
			"Number of volumes"
		},
		{
			ItemPropId.TimeType,
			"Time type"
		},
		{
			ItemPropId.Bit64,
			"64-bit"
		},
		{
			ItemPropId.BigEndian,
			"Big endian"
		},
		{
			ItemPropId.Cpu,
			"CPU"
		},
		{
			ItemPropId.PhysicalSize,
			"Physical Size"
		},
		{
			ItemPropId.HeadersSize,
			"Headers Size"
		},
		{
			ItemPropId.Checksum,
			"Checksum"
		},
		{
			ItemPropId.FreeSpace,
			"Free Space"
		},
		{
			ItemPropId.ClusterSize,
			"Cluster Size"
		}
	};
}
