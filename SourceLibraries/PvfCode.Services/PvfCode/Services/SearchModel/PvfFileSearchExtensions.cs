using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PvfCode;
using PvfCode.Models.Pvf;

namespace PvfCode.Services.SearchModel;

internal static class PvfFileSearchExtensions
{
	public static bool ContainsStringTableReference(this PvfFile file, PvfGroup group, HashSet<int> stringTableIndexes, string? keyword = null, bool isStartMatch = false)
	{
		// 统一管线：110/NKPI 的 GetBinaryForScan 返回经典视图，虚拟串表 ID 与经典格式同语义
		byte[] data = group.GetBinaryForScan(file);
		if (data.Length < 7 || BitConverter.ToUInt16(data, 0) != 53424)
		{
			return false;
		}
		int dataLen = data.Length;
		for (int i = 2; i < dataLen - 4; i += 5)
		{
			if ((data[i] == 5 || data[i] == 7 || data[i] == 10) && stringTableIndexes.Contains(BitConverter.ToInt32(data, i + 1)))
			{
				return true;
			}
			if (i > 4 && data[i] == 10 && data[i - 5] == 9)
			{
				int compositeIndex = data[i - 4] * 16777216 + BitConverter.ToInt32(data, i + 1);
				if (stringTableIndexes.Contains(compositeIndex))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool ContainsIntegerValue(this PvfFile file, PvfGroup group, int value)
	{
		byte[] data = group.GetBinaryForScan(file);
		if (data.Length < 7 || BitConverter.ToUInt16(data, 0) != 53424)
		{
			return false;
		}
		int dataLen = data.Length;
		for (int i = 2; i < dataLen - 4; i += 5)
		{
			if ((data[i] == 2 || data[i] == 4) && BitConverter.ToInt32(data, i + 1) == value)
			{
				return true;
			}
		}
		return false;
	}

	public static bool ContainsBinarySequence(this PvfFile file, PvfGroup group, byte[] sequence)
	{
		int sequenceLength = sequence.Length;
		byte[] data = group.GetBinaryForScan(file);
		if (data.Length < 7 || BitConverter.ToUInt16(data, 0) != 53424)
		{
			return false;
		}
		int dataLen = data.Length;
		if (dataLen < sequenceLength)
		{
			return false;
		}
		for (int i = 2; i < dataLen; i += 5)
		{
			if (data[i] == sequence[0] && BitConverter.ToInt32(data, i + 1) == BitConverter.ToInt32(sequence, 1))
			{
				if (sequence.Length == 5)
				{
					return true;
				}
				if (i + sequenceLength > dataLen)
				{
					return false;
				}
				byte[] candidate = new byte[sequenceLength];
				Buffer.BlockCopy(data, i, candidate, 0, sequenceLength);
				if (DataHelper.BytesEquals(candidate, sequence))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool MatchesItemName(this PvfFile file, PvfGroup group, bool startsWith, string searchText, bool useWildcard, Regex regex)
	{
		string itemName = group.GetItemName(file);
		if (itemName == null)
		{
			return false;
		}
		if (regex != null && regex.IsMatch(itemName))
		{
			return true;
		}
		if (startsWith)
		{
			if (itemName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			return false;
		}
		if (useWildcard)
		{
			if (LikeOperator.LikeString(itemName, searchText, CompareMethod.Binary))
			{
				return true;
			}
			return itemName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
		}
		return searchText == itemName;
	}

	public static bool HasEquivalentContent(this PvfFile sourceFile, PvfGroup sourceGroup, PvfFile targetFile, PvfPack targetPack)
	{
		if (sourceFile.IsScriptFile != targetFile?.IsScriptFile)
		{
			return false;
		}
		if (!sourceFile.IsScriptFile && !targetFile.IsScriptFile)
		{
			return DataHelper.BytesEquals(targetFile.Data, sourceFile.Data);
		}
		if (targetFile.DataLen != sourceFile.DataLen)
		{
			return false;
		}
		for (int i = 2; i < sourceFile.DataLen - 4; i += 5)
		{
			byte itemType = sourceFile.Data[i];
			if ((itemType == 5 || itemType == 6 || itemType == 7 || itemType == 8 || itemType == 10) && sourceGroup.Strtable.GetStringItem(BitConverter.ToInt32(sourceFile.Data, i + 1)) != targetPack.Strtable.GetStringItem(BitConverter.ToInt32(targetFile.Data, i + 1)))
			{
				return false;
			}
			if ((itemType == 9 || itemType == 4 || itemType == 2) && BitConverter.ToInt32(sourceFile.Data, i + 1) != BitConverter.ToInt32(targetFile.Data, i + 1))
			{
				return false;
			}
		}
		return true;
	}
}
