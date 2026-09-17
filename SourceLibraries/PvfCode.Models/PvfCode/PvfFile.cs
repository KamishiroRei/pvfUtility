using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Collections.Pooled;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Attributes;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Models.Pvf.Enums.Stackable;
using PvfCode.Models.Pvf.ScriptEnums;

namespace PvfCode;

public class PvfFile : ModelBase, ICloneable
{
	private bool isUpdated;

	private bool isNewFile;

	/// <summary>用户实际修改过内容（保存/导入/引用重建等变更路径置位）。
	/// 懒加载 SetLoadedContent 不置位，用于保存时跳过未修改文件的重编译与重压缩。</summary>
	public bool IsContentModified { get; private set; }

	/// <summary>
	/// 该条目的文本编辑/保存走 **110 原生文本编解码**（`Pvf110Compiled.ToScriptText` / `FromText`，
	/// 与 AI CLI 同一套、逐 token 无损），而不是经典富文本。
	/// 触发条件：经典视图不存在（出现经典无对应标签的 token，如 110 tag `0x0A`），
	/// 或经典富文本往返会丢 token（池字符串形如经典语法时，如 `etc/equipmentpartset.etc`、
	/// `clientonly/*.co`）。与只读的区别：这些条目**照旧可编辑**，只是文本形态为 110 原生式。
	/// </summary>
	public bool UsesNativeTokenText { get; private set; }

	/// <summary>二进制块（非 type-1，且按容器编码做「解码→回编码」不可逆，如 `.ctp`/`.skel`/`.db`）：
	/// 不做文本编辑，保存按原始字节写回。</summary>
	public bool IsBinaryBlock { get; private set; }

	/// <summary>置位文本形态时的原始条目字节（110/NKPI 原始 token 流或 type-3 原始块）。</summary>
	public byte[]? OriginalRawContent { get; private set; }

	/// <summary>
	/// type-3（Pvf110 文本块）解码时剥掉的尾部 NUL 字符个数；编码写回时按同数补回，
	/// 使「打开→保存」在字节层可逆（115 的 `.str` 块尾部实为两个 NUL 码元）。
	/// </summary>
	public int TextBlockTrailingNulCount { get; set; }

	public int? ItemCode { get; set; }

	/// <summary>Pvf110 文件 dataType（1=编译二进制/解编译文本，3=UTF-16 文本）。标准格式打开时为 0。</summary>
	public int Pvf110DataType { get; set; }

	public byte[] FileNameBytes { get; set; }

	public int FileNameLen => FileNameBytes.Length;

	/// <summary>文件名编码。根据归档格式选择；Pvf110 路径使用 UTF-8。</summary>
	public System.Text.Encoding FileNameEncoding { get; set; } = System.Text.Encoding.GetEncoding(949);

	public byte[] Data { get; private set; }

	public int DataLen { get; set; }

	public long Offset { get; set; }

	public uint Checksum { get; set; }

	public uint FileNameBytesChecksum { get; set; }

	public bool IsUpdated
	{
		get
		{
			return isUpdated;
		}
		set
		{
			isUpdated = value;
			DoNotify("IsUpdated");
		}
	}

	public string FileName
	{
		get
		{
			return FileNameEncoding.GetString(FileNameBytes).TrimEnd(new char[1]);
		}
		set
		{
			FileNameBytes = FileNameEncoding.GetBytes(value.Replace('\\', '/').ToLower());
			FileNameBytesChecksum = DataHelper.GetFileNameHashCode(FileNameBytes);
			if (DataLen > 0)
			{
				Checksum = PvfAlgorithmHelper.CreateBuffKey(Data, GetBlockLength(), FileNameBytesChecksum);
			}
			IsUpdated = true;
		}
	}

	/// <summary>打开归档时装载文件名：与 FileName setter 相同的规范化，但不置 IsUpdated
	/// （归档既有条目不是用户修改，避免文件树把整包显示为已更新）。</summary>
	public void SetFileNameInitial(string name)
	{
		FileNameBytes = FileNameEncoding.GetBytes(name.Replace('\\', '/').ToLower());
		FileNameBytesChecksum = DataHelper.GetFileNameHashCode(FileNameBytes);
	}

	public string ShortName => Path.GetFileName(FileName);

	public string DirectoryName
	{
		get
		{
			string fileName = FileName;
			if (fileName.LastIndexOf('/') <= 0)
			{
				return string.Empty;
			}
			return fileName.Substring(0, fileName.LastIndexOf('/'));
		}
	}

	public string FilePathHeader
	{
		get
		{
			string fileName = FileName;
			int num = fileName.IndexOf('/');
			if (num <= 0)
			{
				return string.Empty;
			}
			return fileName.Substring(0, num);
		}
	}

	public string? SkillLstItemPath { get; set; }

	public bool IsScriptFile
	{
		get
		{
			byte[] data = Data;
			if (data != null && data.Length >= 2)
			{
				return BitConverter.ToUInt16(Data, 0) == 53424;
			}
			return false;
		}
	}

	public bool IsBinaryAniFile
	{
		get
		{
			if (!IsScriptFile)
			{
				return FileName.EndsWith(".ani");
			}
			return false;
		}
	}

	public string Extension => Path.GetExtension(FileName);

	public bool IsNewFile
	{
		get
		{
			return isNewFile;
		}
		set
		{
			isNewFile = value;
			DoNotify("IsNewFile");
		}
	}

	public PvfFileType FileType
	{
		get
		{
			string extension = Path.GetExtension(FileName);
			return AppSetting.Instance.PvfConfig.GetPvfFileType(extension);
		}
	}

	public PvfFile()
	{
		IsNewFile = true;
	}

	public PvfFile(string fileName)
	{
		FileName = fileName;
		FileNameBytes = Encoding.GetEncoding(949).GetBytes(FileName);
		FileNameBytesChecksum = DataHelper.GetFileNameHashCode(FileNameBytes);
		Checksum = FileNameBytesChecksum;
		IsNewFile = true;
		IsUpdated = true;
	}

	public PvfFile(uint fileNameChecksum, byte[] fileNameBytes, int dataLen, uint checksum, int offset)
	{
		FileNameBytesChecksum = fileNameChecksum;
		FileNameBytes = fileNameBytes;
		DataLen = dataLen;
		Checksum = checksum;
		Offset = offset;
	}

	public int GetBlockLength()
	{
		return (DataLen + 3) & -4;
	}

	public void WriteFileData(byte[] fileData)
	{
		DataLen = fileData.Length;
		if (DataLen > 0)
		{
			Data = new byte[GetBlockLength()];
			Buffer.BlockCopy(fileData, 0, Data, 0, DataLen);
			Checksum = PvfAlgorithmHelper.CreateBuffKey(Data, GetBlockLength(), FileNameBytesChecksum);
			IsUpdated = true;
			IsContentModified = true;
		}
	}

	/// <summary>原样写入 Data（不做 4 字节对齐，不计算旧式 checksum）。Pvf110 模式使用，避免尾部填充污染明文。</summary>
	public void WriteRawData(byte[] fileData)
	{
		DataLen = fileData.Length;
		Data = fileData;
		IsUpdated = true;
		IsContentModified = true;
	}

	/// <summary>懒加载/打开时装载内容：不标记为用户修改（保存时可按原始内容跳过重编译）。</summary>
	public void SetLoadedContent(byte[] content)
	{
		Data = content;
		DataLen = content.Length;
	}

	/// <summary>
	/// 置为该条目的文本形态为 110 原生文本：保留原始字节（供展示与保存时兜底），
	/// 经典视图可用时保留在 <see cref="Data"/>（供套装表等只读解析器读取），否则 Data 为空。
	/// </summary>
	public void SetUsesNativeTokenText(byte[] rawContent, byte[]? classicView = null)
	{
		OriginalRawContent = rawContent;
		UsesNativeTokenText = true;
		IsContentModified = false;
		Data = classicView ?? Array.Empty<byte>();
		DataLen = Data.Length;
	}

	/// <summary>置为二进制块：原始字节保留在 Data（导出/预览走原始字节），禁文本编辑。</summary>
	public void SetBinaryBlock(byte[] rawContent)
	{
		OriginalRawContent = rawContent;
		IsBinaryBlock = true;
		IsContentModified = false;
		Data = rawContent;
		DataLen = rawContent.Length;
	}

	/// <summary>释放懒加载内容，回收常驻内存；用户已修改的文件不释放，避免丢失未保存编辑。</summary>
	public void ReleaseLoadedContent()
	{
		if (IsContentModified) return;
		Data = Array.Empty<byte>();
		DataLen = 0;
	}

	public void InitFile(byte[] bytes)
	{
		Data = bytes;
		Data = PvfAlgorithmHelper.DecryptionPvf(Data, GetBlockLength(), Checksum);
		for (int i = 0; i < GetBlockLength() - DataLen; i++)
		{
			Data[DataLen + i] = 0;
		}
	}

	public void InitNewCopyFile(byte[] bytes, string name)
	{
		Rename(name);
		if (bytes != null)
		{
			WriteFileData(bytes);
		}
	}

	public void Rename(string newFileName)
	{
		FileName = newFileName;
	}

	public override string ToString()
	{
		return FileName;
	}

	public string GetFilePathHeader(string filePath)
	{
		int num = filePath.IndexOf('/');
		if (num <= 0)
		{
			return null;
		}
		return filePath.Substring(0, num);
	}

	public string LstItemPathToFilePath(string itemPath)
	{
		return FilePathHeader + "/" + itemPath;
	}

	public bool IsSkillLst()
	{
		if (FileType == PvfFileType.lst)
		{
			return FilePathHeader == "skill";
		}
		return false;
	}

	public string GetLstPathHeader()
	{
		if (FileType == PvfFileType.skl)
		{
			string fileName = FileName;
			int num = fileName.IndexOf('/');
			if (num != -1)
			{
				num = fileName.IndexOf('/', num + 1);
				if (num != -1)
				{
					return fileName.Substring(0, num);
				}
			}
			return FilePathHeader;
		}
		return FilePathHeader;
	}

	public KeyValuePair<int, string>? ToLstItem()
	{
		if (!ItemCode.HasValue)
		{
			return null;
		}
		string fileName = FileName;
		string text = ((FileType == PvfFileType.skl) ? SkillLstItemPath : fileName.Remove(0, (GetFilePathHeader(fileName) + "/").Length));
		if (text == null)
		{
			return null;
		}
		return new KeyValuePair<int, string>(ItemCode.Value, text);
	}

	public void GetStringViewQuote(ConcurrentBag<KeyValuePair<int, int>> list, Stringtable stringtable)
	{
		if (Data == null || DataLen < 7)
		{
			return;
		}
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			if (Data[i] == 10)
			{
				int startIndex = i + 1;
				list.Add(new KeyValuePair<int, int>(BitConverter.ToInt32(Data, i - 4), BitConverter.ToInt32(Data, startIndex)));
			}
		}
	}

	public int FindStringViewQuote(Stringtable stringtable, int viewId, int strTabId)
	{
		int num = 0;
		if (Data != null && DataLen >= 7)
		{
			for (int i = 2; i < DataLen - 4; i += 5)
			{
				if (Data[i] != 10)
				{
					continue;
				}
				int startIndex = i + 1;
				if (BitConverter.ToInt32(Data, i - 4) == viewId && BitConverter.ToInt32(Data, startIndex) == strTabId)
				{
					lock (this)
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	public PooledList<int> GetStringDatas()
	{
		PooledList<int> pooledList = new PooledList<int>();
		if (Data != null && DataLen >= 7)
		{
			for (int i = 2; i < DataLen - 4; i += 5)
			{
				byte b = Data[i];
				int item = BitConverter.ToInt32(Data, i + 1);
				if ((uint)(b - 5) <= 3u || b == 10)
				{
					pooledList.Add(item);
				}
			}
		}
		return pooledList;
	}

	public void GetStringDatas(ConcurrentBag<int> list)
	{
		if (Data == null || DataLen < 7)
		{
			return;
		}
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			byte b = Data[i];
			int item = BitConverter.ToInt32(Data, i + 1);
			if ((uint)(b - 5) <= 3u || b == 10)
			{
				list.Add(item);
			}
		}
	}

	public string ToLstItemString()
	{
		if (!ItemCode.HasValue)
		{
			return null;
		}
		string fileName = FileName;
		string text = ((FileType == PvfFileType.skl) ? SkillLstItemPath : fileName.Remove(0, (GetFilePathHeader(fileName) + "/").Length));
		if (text == null)
		{
			return null;
		}
		if (text == null)
		{
			return null;
		}
		return ItemCode + "\t" + text;
	}

	public object Clone()
	{
		return MemberwiseClone();
	}

	public PvfFile CloneData()
	{
		return (PvfFile)Clone();
	}

	public void ReconstructNameReferenceData(Dictionary<int, int> dic)
	{
		if (Data == null || DataLen < 7)
		{
			return;
		}
		byte[] value = Data.ToArray();
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			byte b = Data[i];
			if ((uint)(b - 5) > 3u && b != 10)
			{
				continue;
			}
			int num = i + 1;
			if (dic.TryGetValue(BitConverter.ToInt32(value, num), out var value2))
			{
				byte[] bytes = BitConverter.GetBytes((uint)value2);
				Buffer.BlockCopy(bytes, 0, Data, num, bytes.Length);
				IsUpdated = true;
				IsContentModified = true;
			}
		}
	}

	public bool GetRarity(PvfPack pvf, out int rarity)
	{
		rarity = 0;
		if (!IsScriptFile)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId("[rarity]");
		if (stringTableId == -1)
		{
			return false;
		}
		if (Data != null && DataLen >= 7)
		{
			for (int i = 2; i < DataLen - 4; i += 5)
			{
				if (Data[i] == 5)
				{
					int num = i + 1;
					if (BitConverter.ToInt32(Data, num) == stringTableId)
					{
						rarity = BitConverter.ToInt32(Data, num + 5);
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool GetRarity(PvfPack pvf, out RarityType? rarityType)
	{
		if (GetRarity(pvf, out int rarity))
		{
			switch (rarity)
			{
			case 0:
				rarityType = RarityType.普通;
				break;
			case 1:
				rarityType = RarityType.高级;
				break;
			case 2:
				rarityType = RarityType.稀有;
				break;
			case 3:
				rarityType = RarityType.神器;
				break;
			case 4:
				rarityType = RarityType.史诗;
				break;
			case 5:
				rarityType = RarityType.勇者;
				break;
			default:
				rarityType = RarityType.未知品级;
				break;
			}
			return true;
		}
		rarityType = null;
		return false;
	}

	public bool GetMonsterType(PvfPack pvf, out MonsterCategoryType? monsterCategoryType)
	{
		monsterCategoryType = null;
		if (GetSectionStringValue("[category]", pvf, out string val))
		{
			monsterCategoryType = PvfFileHelper.StrConvertMonsterCategoryEnum(val);
		}
		return monsterCategoryType.HasValue;
	}

	public bool GetIcon(PvfPack pvf, out KeyValuePair<string, int>? icon)
	{
		icon = null;
		if (!IsScriptFile)
		{
			return false;
		}
		if (FileType == PvfFileType.shp)
		{
			return GetShopNpcIcon(pvf, out icon);
		}
		int stringTableId;
		switch (FileType)
		{
		case PvfFileType.mob:
			stringTableId = pvf.Strtable.GetStringTableId("[face image]");
			break;
		case PvfFileType.npc:
			stringTableId = pvf.Strtable.GetStringTableId("[small face]");
			break;
		case PvfFileType.equ:
		case PvfFileType.stk:
		case PvfFileType.skl:
			stringTableId = pvf.Strtable.GetStringTableId("[icon]");
			break;
		default:
			return false;
		}
		if (stringTableId == -1)
		{
			return false;
		}
		if (Data != null && DataLen >= 7)
		{
			for (int i = 2; i < DataLen - 4; i += 5)
			{
				if (Data[i] != 5)
				{
					continue;
				}
				int startIndex = i + 1;
				if (BitConverter.ToInt32(Data, startIndex) == stringTableId)
				{
					if (i + 10 >= DataLen)
					{
						return false;
					}
					if (i + 14 >= DataLen)
					{
						return false;
					}
					icon = new KeyValuePair<string, int>(pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, i + 6)), BitConverter.ToInt32(Data, i + 11));
					return true;
				}
			}
		}
		return false;
	}

	private bool GetShopNpcIcon(PvfPack pvf, out KeyValuePair<string, int>? icon)
	{
		if (GetNpcId(pvf, out var npcId) && pvf.ListFileTable.CodeDic.TryGetValue("npc", out Dictionary<int, LstItem> value) && value.TryGetValue(npcId, out var value2) && pvf.FileList.TryGetValue(value2.FullPath, out PvfFile value3) && value3.GetIcon(pvf, out icon))
		{
			return true;
		}
		icon = null;
		return false;
	}

	public bool GetNpcId(PvfPack pvf, out int npcId)
	{
		return GetNpcId(pvf, Data, out npcId);
	}

	public bool GetNpcId(PvfPack pvf, byte[]? scanData, out int npcId)
	{
		npcId = -1;
		if (scanData == null || scanData.Length < 2 || BitConverter.ToUInt16(scanData, 0) != 53424)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId("[NPC]");
		if (stringTableId == -1)
		{
			return false;
		}
		int dataLen = scanData.Length;
		if (dataLen >= 7)
		{
			for (int i = 2; i < dataLen - 4; i += 5)
			{
				if (scanData[i] == 5)
				{
					int num = i + 1;
					if (BitConverter.ToInt32(scanData, num) == stringTableId)
					{
						npcId = BitConverter.ToInt32(scanData, num + 5);
						return true;
					}
				}
			}
		}
		return false;
	}

	public QuestType? GetQuestType(PvfPack pvf)
	{
		if (Data == null || DataLen < 7)
		{
			return null;
		}
		int stringTableId = pvf.Strtable.GetStringTableId("[job change quest]");
		int stringTableId2 = pvf.Strtable.GetStringTableId("[grade]");
		int stringTableId3 = pvf.Strtable.GetStringTableId("[event]");
		int num = -1;
		string text = null;
		int num2 = -1;
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			if (Data[i] == 5)
			{
				int startIndex = i + 1;
				int num3 = BitConverter.ToInt32(Data, startIndex);
				if (stringTableId != -1 && num3 == stringTableId)
				{
					num = BitConverter.ToInt32(Data, i + 6);
				}
				else if (stringTableId2 != -1 && num3 == stringTableId2)
				{
					text = pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, i + 6));
				}
				else if (stringTableId3 != -1 && num3 == stringTableId3)
				{
					num2 = BitConverter.ToInt32(Data, i + 6);
				}
			}
			if (num != -1 && text != null && num2 != -1)
			{
				break;
			}
		}
		switch (num)
		{
		case 0:
			return QuestType.Common_unique;
		case 1:
			return QuestType.Charac_ConvertJob;
		case 2:
			return QuestType.Charac_Awakening;
		case 10:
			return QuestType.Evolution;
		case 20:
			return QuestType.SecondaryOccupation;
		default:
			if (num2 == 1)
			{
				return QuestType.Activities;
			}
			if (!string.IsNullOrEmpty(text) && text != null)
			{
				switch (text.Length)
				{
				case 10:
					switch (text[1])
					{
					case 't':
						if (!(text == "[training]"))
						{
							break;
						}
						return QuestType.Training;
					case 's':
						if (!(text == "[schedule]"))
						{
							break;
						}
						return QuestType.Schedule;
					}
					break;
				case 6:
					if (!(text == "[epic]"))
					{
						break;
					}
					return QuestType.Epic;
				case 15:
					if (!(text == "[common unique]"))
					{
						break;
					}
					return QuestType.Common_unique;
				case 13:
					if (!(text == "[achievement]"))
					{
						break;
					}
					return QuestType.Achievement;
				case 7:
					if (!(text == "[daily]"))
					{
						break;
					}
					return QuestType.Daily;
				case 16:
					if (!(text == "[normaly repeat]"))
					{
						break;
					}
					return QuestType.Normaly_repeat;
				case 8:
					if (!(text == "[urgent]"))
					{
						break;
					}
					return QuestType.Urgent;
				}
			}
			return null;
		}
	}

	public bool GetEmoAniPath(PvfPack pvf, out string? aniFilePath)
	{
		aniFilePath = null;
		if (Data == null || DataLen < 7)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId("[ani]");
		if (stringTableId == -1)
		{
			return false;
		}
		if (Data != null && DataLen >= 7)
		{
			for (int i = 2; i < DataLen - 4; i += 5)
			{
				if (Data[i] == 5)
				{
					int num = i + 1;
					if (BitConverter.ToInt32(Data, num) == stringTableId && i + 5 < DataLen)
					{
						string stringItem = pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, num + 5));
						aniFilePath = ((stringItem == null) ? string.Empty : stringItem.ToLower());
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool AllowPreview()
	{
		_ = FileType;
		PvfFileType fileType = FileType;
		if (fileType == PvfFileType.equ || fileType == PvfFileType.stk)
		{
			return true;
		}
		return false;
	}

	public EquTypeDefault GetEquType(PvfPack pvf)
	{
		if (Data == null || DataLen < 7)
		{
			return EquTypeDefault.Default;
		}
		int stringTableId = pvf.Strtable.GetStringTableId("[equipment type]");
		int stringTableId2 = pvf.Strtable.GetStringTableId("[sub type]");
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			if (Data[i] != 5)
			{
				continue;
			}
			int num = i + 1;
			if (BitConverter.ToInt32(Data, num) != stringTableId || i + 5 >= DataLen)
			{
				continue;
			}
			string stringItem = pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, num + 5));
			if (stringItem == "[creature]")
			{
				if (stringTableId2 != -1)
				{
					for (int j = 2; j < DataLen - 4; j += 5)
					{
						if (Data[j] == 5)
						{
							int num2 = j + 1;
							if (BitConverter.ToInt32(Data, num2) == stringTableId2 && j + 5 < DataLen && BitConverter.ToInt32(Data, num2 + 5) == 1)
							{
								return EquTypeDefault.PetEgg;
							}
						}
					}
				}
				return EquTypeDefault.Pet;
			}
			if (stringItem == "[artifact red]" || stringItem == "[artifact blue]" || stringItem == "[artifact green]")
			{
				return EquTypeDefault.PetEqu;
			}
			if (AppSetting.Instance.PvfConfig.AvatarParts.Contains(stringItem))
			{
				return EquTypeDefault.Avatar;
			}
			return EquTypeDefault.Default;
		}
		return EquTypeDefault.Default;
	}

	public bool GetName(PvfPack pvf, Name_Type name_Type, out string? name)
	{
		return GetNameText(pvf, GetNameSection(name_Type), out name);
	}

	private string GetNameSection(Name_Type nameType)
	{
		return nameType switch
		{
			Name_Type.name => "[name]", 
			Name_Type.name2 => "[name2]", 
			Name_Type.basic_explain => "[basic explain]", 
			Name_Type.flavor_text => "[flavor text]", 
			Name_Type.Detail_Explain => "[detail explain]", 
			Name_Type.explain => "[explain]", 
			_ => string.Empty, 
		};
	}

	public bool GetEquipmentType(PvfPack pvf, out EquipmentType? re)
	{
		re = null;
		if (Data == null || DataLen < 7)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId("[equipment type]");
		if (stringTableId == -1)
		{
			return false;
		}
		if (!FindSectionIndex(stringTableId, out var indexOut))
		{
			return false;
		}
		if (indexOut + 5 >= DataLen)
		{
			return false;
		}
		string stringItem = pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, indexOut + 6));
		if (stringItem != null)
		{
			switch (stringItem.Length)
			{
			case 12:
				switch (stringItem[1])
				{
				case 't':
					if (stringItem == "[title name]")
					{
						re = EquipmentType.称号;
					}
					break;
				case 'h':
					if (stringItem == "[hat avatar]")
					{
						re = EquipmentType.帽子装扮;
					}
					break;
				}
				break;
			case 8:
				switch (stringItem[1])
				{
				case 'w':
					if (stringItem == "[weapon]")
					{
						re = EquipmentType.武器;
					}
					break;
				case 'a':
					if (stringItem == "[amulet]")
					{
						re = EquipmentType.项链;
					}
					break;
				}
				break;
			case 6:
				switch (stringItem[1])
				{
				case 'c':
					if (stringItem == "[coat]")
					{
						re = EquipmentType.上衣;
					}
					break;
				case 'r':
					if (stringItem == "[ring]")
					{
						re = EquipmentType.戒指;
					}
					break;
				}
				break;
			case 7:
				switch (stringItem[1])
				{
				case 'p':
					if (stringItem == "[pants]")
					{
						re = EquipmentType.下衣;
					}
					break;
				case 'w':
					if (!(stringItem == "[waist]"))
					{
						if (stringItem == "[wrist]")
						{
							re = EquipmentType.手镯;
						}
					}
					else
					{
						re = EquipmentType.腰带;
					}
					break;
				case 's':
					if (stringItem == "[shoes]")
					{
						re = EquipmentType.鞋;
					}
					break;
				}
				break;
			case 10:
				switch (stringItem[1])
				{
				case 's':
					if (stringItem == "[shoulder]")
					{
						re = EquipmentType.护肩;
					}
					break;
				case 'c':
				{
					if (!(stringItem == "[creature]"))
					{
						break;
					}
					int stringTableId2 = pvf.Strtable.GetStringTableId("[sub type]");
					if (FindSectionIndex(stringTableId2, out var indexOut2) && indexOut2 + 5 < DataLen)
					{
						if (BitConverter.ToInt32(Data, indexOut2 + 5) == 1)
						{
							re = EquipmentType.宠物蛋;
						}
						else
						{
							re = EquipmentType.宠物;
						}
					}
					else
					{
						re = EquipmentType.宠物;
					}
					break;
				}
				}
				break;
			case 13:
				switch (stringItem[1])
				{
				case 'm':
					if (stringItem == "[magic stone]")
					{
						re = EquipmentType.魔法石;
					}
					break;
				case 's':
					if (stringItem == "[skin avatar]")
					{
						re = EquipmentType.皮肤装扮;
					}
					break;
				case 'c':
					if (stringItem == "[coat avatar]")
					{
						re = EquipmentType.上衣装扮;
					}
					break;
				case 'f':
					if (stringItem == "[face avatar]")
					{
						re = EquipmentType.脸部装扮;
					}
					break;
				case 'h':
					if (stringItem == "[hair avatar]")
					{
						re = EquipmentType.头部装扮;
					}
					break;
				}
				break;
			case 14:
				switch (stringItem[1])
				{
				case 'a':
					if (stringItem == "[artifact red]")
					{
						re = EquipmentType.宠物装备_红;
					}
					break;
				case 'w':
					if (stringItem == "[waist avatar]")
					{
						re = EquipmentType.腰部装扮;
					}
					break;
				case 'p':
					if (stringItem == "[pants avatar]")
					{
						re = EquipmentType.下装装扮;
					}
					break;
				case 's':
					if (stringItem == "[shoes avatar]")
					{
						re = EquipmentType.鞋装扮;
					}
					break;
				}
				break;
			case 15:
				switch (stringItem[3])
				{
				case 't':
					if (stringItem == "[artifact blue]")
					{
						re = EquipmentType.宠物装备_蓝;
					}
					break;
				case 'r':
					if (stringItem == "[aurora avatar]")
					{
						re = EquipmentType.光环装扮;
					}
					break;
				case 'e':
					if (stringItem == "[breast avatar]")
					{
						re = EquipmentType.胸部装扮;
					}
					break;
				}
				break;
			case 9:
				if (stringItem == "[support]")
				{
					re = EquipmentType.辅助装备;
				}
				break;
			case 16:
				if (stringItem == "[artifact green]")
				{
					re = EquipmentType.宠物装备_绿;
				}
				break;
			}
		}
		return re.HasValue;
	}

	public bool GetEquArmorType(PvfPack pvf, out EquArmorType? equArmorType)
	{
		equArmorType = null;
		if (Data == null || DataLen < 7)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId("[sub type]");
		if (stringTableId == -1)
		{
			return false;
		}
		if (!FindSectionIndex(stringTableId, out var indexOut))
		{
			return false;
		}
		if (indexOut + 5 < DataLen)
		{
			switch (BitConverter.ToInt32(Data, indexOut + 6))
			{
			case 0:
				equArmorType = EquArmorType.布甲;
				return true;
			case 1:
				equArmorType = EquArmorType.皮甲;
				return true;
			case 2:
				equArmorType = EquArmorType.轻甲;
				return true;
			case 3:
				equArmorType = EquArmorType.重甲;
				return true;
			case 4:
				equArmorType = EquArmorType.板甲;
				return true;
			}
		}
		return false;
	}

	public bool GetItemWeight(PvfPack pvf, out string? weight)
	{
		if (!GetSectionValue("[weight]", pvf, out weight))
		{
			return false;
		}
		return true;
	}

	public bool GetMiniNumLevel(PvfPack pvf, out string? val)
	{
		if (!GetSectionValue("[minimum level]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetGrade(PvfPack pvf, out string? val)
	{
		if (!GetSectionValue("[grade]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetMagicalAttack(PvfPack pvf, out string? val)
	{
		if (!GetSectionValue("[magical attack]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetMagicalDefense(PvfPack pvf, out string? val)
	{
		if (!GetSectionValue("[magical defense]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetPhysicalAttack(PvfPack pvf, out string? val)
	{
		if (!GetSectionValue("[physical attack]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetPhysicalDefense(PvfPack pvf, out string? val)
	{
		if (!GetSectionValue("[physical defense]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetMPRegenSpeed(PvfPack pvf, out string? val)
	{
		if (!GetSectionValue("[MP regen speed]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetHPRegenSpeed(PvfPack pvf, out string? val)
	{
		if (!GetSectionValue("[HP regen speed]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetPrice(PvfPack pvf, out string? val)
	{
		if (!GetSectionValue("[price]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetRepairPrice(PvfPack pvf, out string? val)
	{
		if (!GetSectionValue("[repair price]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetValuePrice(PvfPack pvf, out string? val)
	{
		if (!GetSectionValue("[value]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetEquipmentMagicalDefense(PvfPack pvf, out KeyValuePair<string?, string?>? val)
	{
		if (!GetSectionIntValue2("[equipment magical defense]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetEquipmentPhysicalDefense(PvfPack pvf, out KeyValuePair<string?, string?>? val)
	{
		if (!GetSectionIntValue2("[equipment physical defense]", pvf, out val))
		{
			return false;
		}
		return true;
	}

	public bool GetDurability(PvfPack pvf, out string? durability)
	{
		if (!GetSectionValue("[durability]", pvf, out durability))
		{
			return false;
		}
		return true;
	}

	public bool GetAttachType(PvfPack pvf, out AttachType? attachType)
	{
		if (GetSectionStringValue("[attach type]", pvf, out string val))
		{
			if (val == "[trade]")
			{
				attachType = AttachType.trade;
				return true;
			}
			if (val == "[sealing]")
			{
				attachType = AttachType.sealing;
				return true;
			}
			if (val == "[sealing trade]")
			{
				attachType = AttachType.sealing_trade;
				return true;
			}
			if (val == "[account]")
			{
				attachType = AttachType.account;
				return true;
			}
			if (val == "[free]")
			{
				attachType = AttachType.free;
				return true;
			}
			if (val == "[trade delete]")
			{
				attachType = AttachType.trade_delete;
				return true;
			}
		}
		attachType = null;
		return false;
	}

	public bool GetUsableJob(PvfPack pvf, out List<JobType> jobs)
	{
		jobs = new List<JobType>();
		if (GetSectionStringArray(pvf, "[usable job]", out List<string> list))
		{
			foreach (string item in list)
			{
				JobType? jobType = null;
				if (item != null)
				{
					switch (item.Length)
					{
					case 9:
						switch (item[1])
						{
						case 'f':
							if (item == "[fighter]")
							{
								jobType = JobType.女格斗家;
							}
							break;
						case 'a':
							if (item == "[at mage]")
							{
								jobType = JobType.男魔法师;
							}
							break;
						}
						break;
					case 8:
						switch (item[1])
						{
						case 'g':
							if (item == "[gunner]")
							{
								jobType = JobType.男神枪手;
							}
							break;
						case 'p':
							if (item == "[priest]")
							{
								jobType = JobType.男圣职者;
							}
							break;
						case 'k':
							if (item == "[knight]")
							{
								jobType = JobType.守护者;
							}
							break;
						}
						break;
					case 11:
						switch (item[4])
						{
						case 'g':
							if (item == "[at gunner]")
							{
								jobType = JobType.女神枪手;
							}
							break;
						case 'p':
							if (item == "[at priest]")
							{
								jobType = JobType.女圣职者;
							}
							break;
						}
						break;
					case 12:
						switch (item[1])
						{
						case 'a':
							if (item == "[at fighter]")
							{
								jobType = JobType.男格斗家;
							}
							break;
						case 'g':
							if (item == "[gun blader]")
							{
								jobType = JobType.枪剑士;
							}
							break;
						}
						break;
					case 10:
						if (item == "[swordman]")
						{
							jobType = JobType.鬼剑士;
						}
						break;
					case 6:
						if (item == "[mage]")
						{
							jobType = JobType.女魔法师;
						}
						break;
					case 7:
						if (item == "[thief]")
						{
							jobType = JobType.暗夜使者;
						}
						break;
					case 18:
						if (item == "[demonic swordman]")
						{
							jobType = JobType.黑暗武士;
						}
						break;
					case 14:
						if (item == "[creator mage]")
						{
							jobType = JobType.缔造者;
						}
						break;
					case 13:
						if (item == "[at swordman]")
						{
							jobType = JobType.女鬼剑士;
						}
						break;
					case 16:
						if (item == "[demonic lancer]")
						{
							jobType = JobType.魔枪士;
						}
						break;
					case 5:
						if (item == "[all]")
						{
							jobType = JobType.通用;
						}
						break;
					}
				}
				if (jobType.HasValue)
				{
					jobs.Add(jobType.Value);
				}
			}
			return jobs.Count > 0;
		}
		return false;
	}

	public string ConvertAttachTypeToString(AttachType type)
	{
		return type switch
		{
			AttachType.free => AppSetting.Instance.GetIlogger()?.GetStr("AttachType_Free"), 
			AttachType.sealing => AppSetting.Instance.GetIlogger()?.GetStr("AttachType_sealing"), 
			AttachType.trade => AppSetting.Instance.GetIlogger()?.GetStr("AttachType_trade"), 
			AttachType.account => AppSetting.Instance.GetIlogger()?.GetStr("AttachType_account"), 
			AttachType.trade_delete => AppSetting.Instance.GetIlogger()?.GetStr("AttachType_trade_delete"), 
			AttachType.sealing_trade => AppSetting.Instance.GetIlogger()?.GetStr("AttachType_sealing_trade"), 
			_ => string.Empty, 
		};
	}

	public bool GetItemAuraExplainData(PvfPack pvf, out List<ItemAuraExplainData>? items)
	{
		items = new List<ItemAuraExplainData>();
		if (Data == null || DataLen < 7)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId("[item aura]");
		if (stringTableId == -1)
		{
			return false;
		}
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			if (Data[i] != 5)
			{
				continue;
			}
			int startIndex = i + 1;
			if (BitConverter.ToInt32(Data, startIndex) == stringTableId)
			{
				ItemAuraExplainData itemAuraExplainData = new ItemAuraExplainData();
				if (i + 5 < DataLen && Data[i + 5] == 7)
				{
					itemAuraExplainData.Command = pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, i + 6));
				}
				if (i + 10 < DataLen && Data[i + 10] == 7)
				{
					itemAuraExplainData.CalculationType = pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, i + 11));
				}
				if (i + 15 < DataLen && Data[i + 15] == 2)
				{
					itemAuraExplainData.Value1 = BitConverter.ToInt32(Data, i + 16);
				}
				if (i + 20 < DataLen && Data[i + 20] == 2)
				{
					itemAuraExplainData.Value2 = BitConverter.ToInt32(Data, i + 21);
				}
				items.Add(itemAuraExplainData);
			}
		}
		return items.Count > 0;
	}

	public bool GetSkillLevelup(PvfPack pvf, out List<SkillLevelupData> items)
	{
		items = new List<SkillLevelupData>();
		if (Data == null || DataLen < 7)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId("[skill levelup]");
		if (stringTableId == -1)
		{
			return false;
		}
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			if (Data[i] != 5)
			{
				continue;
			}
			int startIndex = i + 1;
			if (BitConverter.ToInt32(Data, startIndex) != stringTableId)
			{
				continue;
			}
			for (int j = i + 5; j < DataLen - 4; j += 15)
			{
				if (Data[j] == 5)
				{
					return items.Count > 0;
				}
				SkillLevelupData skillLevelupData = new SkillLevelupData();
				if (j < DataLen && Data[j] == 7)
				{
					skillLevelupData.JobType = pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, j + 1));
				}
				if (j + 5 < DataLen && Data[j + 5] == 2)
				{
					skillLevelupData.SkillId = BitConverter.ToInt32(Data, j + 6);
				}
				if (j + 10 < DataLen && Data[j + 10] == 2)
				{
					skillLevelupData.UpLevel = BitConverter.ToInt32(Data, j + 11);
				}
				items.Add(skillLevelupData);
			}
			return items.Count > 0;
		}
		return items.Count > 0;
	}

	public bool GetItemGroupName(PvfPack pvf, out ItemGroupName? itemGroupName)
	{
		itemGroupName = null;
		if (GetSectionStringValue("[item group name]", pvf, out string val) && val != null)
		{
			switch (val.Length)
			{
			case 3:
				switch (val[0])
				{
				case 'a':
					if (!(val == "axe"))
					{
						break;
					}
					itemGroupName = ItemGroupName.战斧;
					return true;
				case 'r':
					if (!(val == "rod"))
					{
						break;
					}
					itemGroupName = ItemGroupName.魔杖;
					return true;
				}
				break;
			case 5:
				switch (val[2])
				{
				case 't':
					if (!(val == "totem"))
					{
						break;
					}
					itemGroupName = ItemGroupName.图腾;
					return true;
				case 'o':
					if (!(val == "cross"))
					{
						if (!(val == "broom"))
						{
							break;
						}
						itemGroupName = ItemGroupName.扫把;
						return true;
					}
					itemGroupName = ItemGroupName.十字架;
					return true;
				case 'e':
					if (!(val == "spear"))
					{
						break;
					}
					itemGroupName = ItemGroupName.矛;
					return true;
				case 'a':
					if (!(val == "staff"))
					{
						break;
					}
					itemGroupName = ItemGroupName.法杖;
					return true;
				case 'n':
					if (!(val == "tonfa"))
					{
						break;
					}
					itemGroupName = ItemGroupName.东方棍;
					return true;
				case 'i':
					if (!(val == "wrist"))
					{
						break;
					}
					itemGroupName = ItemGroupName.手镯;
					return true;
				}
				break;
			case 6:
				switch (val[0])
				{
				case 'r':
					if (!(val == "rosary"))
					{
						break;
					}
					itemGroupName = ItemGroupName.念珠;
					return true;
				case 's':
					if (!(val == "scythe"))
					{
						if (!(val == "ssword"))
						{
							break;
						}
						itemGroupName = ItemGroupName.短剑;
						return true;
					}
					itemGroupName = ItemGroupName.镰刀;
					return true;
				case 'b':
					if (!(val == "bowgun"))
					{
						if (!(val == "bglove"))
						{
							break;
						}
						itemGroupName = ItemGroupName.拳套;
						return true;
					}
					itemGroupName = ItemGroupName.手弩;
					return true;
				case 'm':
					if (!(val == "musket"))
					{
						break;
					}
					itemGroupName = ItemGroupName.步枪;
					return true;
				case 'k':
					if (!(val == "katana"))
					{
						break;
					}
					itemGroupName = ItemGroupName.太刀;
					return true;
				case 'd':
					if (!(val == "dagger"))
					{
						break;
					}
					itemGroupName = ItemGroupName.匕首;
					return true;
				case 'a':
					if (!(val == "amulet"))
					{
						break;
					}
					itemGroupName = ItemGroupName.项链;
					return true;
				case 'p':
					if (!(val == "potion"))
					{
						break;
					}
					itemGroupName = ItemGroupName.药剂;
					return true;
				}
				break;
			case 8:
				switch (val[3])
				{
				case 'o':
					if (!(val == "revolver"))
					{
						break;
					}
					itemGroupName = ItemGroupName.左轮枪;
					return true;
				case 'n':
					if (!(val == "gauntlet"))
					{
						break;
					}
					itemGroupName = ItemGroupName.臂铠;
					return true;
				case 'w':
					if (!(val == "ha waist"))
					{
						if (!(val == "la waist"))
						{
							if (!(val == "lt waist"))
							{
								if (!(val == "cl waist"))
								{
									if (!(val == "mt waist"))
									{
										break;
									}
									itemGroupName = ItemGroupName.板甲_腰带;
									return true;
								}
								itemGroupName = ItemGroupName.布甲_腰带;
								return true;
							}
							itemGroupName = ItemGroupName.皮甲_腰带;
							return true;
						}
						itemGroupName = ItemGroupName.轻甲_腰带;
						return true;
					}
					itemGroupName = ItemGroupName.重甲_腰带;
					return true;
				case 's':
					if (!(val == "ha shoes"))
					{
						if (!(val == "la shoes"))
						{
							if (!(val == "lt shoes"))
							{
								if (!(val == "cl shoes"))
								{
									if (!(val == "mt shoes"))
									{
										break;
									}
									itemGroupName = ItemGroupName.板甲_鞋子;
									return true;
								}
								itemGroupName = ItemGroupName.布甲_鞋子;
								return true;
							}
							itemGroupName = ItemGroupName.皮甲_鞋子;
							return true;
						}
						itemGroupName = ItemGroupName.轻甲_鞋子;
						return true;
					}
					itemGroupName = ItemGroupName.重甲_鞋子;
					return true;
				case 'p':
					if (!(val == "ha pants"))
					{
						if (!(val == "la pants"))
						{
							if (!(val == "lt pants"))
							{
								if (!(val == "cl pants"))
								{
									if (!(val == "mt pants"))
									{
										break;
									}
									itemGroupName = ItemGroupName.板甲_下装;
									return true;
								}
								itemGroupName = ItemGroupName.布甲_下装;
								return true;
							}
							itemGroupName = ItemGroupName.皮甲_下装;
							return true;
						}
						itemGroupName = ItemGroupName.轻甲_下装;
						return true;
					}
					itemGroupName = ItemGroupName.重甲_下装;
					return true;
				case 'd':
					if (!(val == "food etc"))
					{
						break;
					}
					itemGroupName = ItemGroupName.食物ETC;
					return true;
				}
				break;
			case 9:
				switch (val[0])
				{
				case 'a':
					if (!(val == "automatic"))
					{
						break;
					}
					itemGroupName = ItemGroupName.自动手枪;
					return true;
				case 's':
					if (!(val == "stuff etc"))
					{
						break;
					}
					itemGroupName = ItemGroupName.材料;
					return true;
				}
				break;
			case 7:
				switch (val[0])
				{
				case 'h':
					if (!(val == "hcannon"))
					{
						if (!(val == "ha coat"))
						{
							break;
						}
						itemGroupName = ItemGroupName.重甲_上衣;
						return true;
					}
					itemGroupName = ItemGroupName.手炮;
					return true;
				case 'b':
					if (!(val == "beamswd"))
					{
						break;
					}
					itemGroupName = ItemGroupName.光剑;
					return true;
				case 'k':
					if (!(val == "knuckle"))
					{
						break;
					}
					itemGroupName = ItemGroupName.手套;
					return true;
				case 't':
					if (!(val == "twinswd"))
					{
						break;
					}
					itemGroupName = ItemGroupName.双剑;
					return true;
				case 'l':
					if (!(val == "la coat"))
					{
						if (!(val == "lt coat"))
						{
							break;
						}
						itemGroupName = ItemGroupName.皮甲_上衣;
						return true;
					}
					itemGroupName = ItemGroupName.轻甲_上衣;
					return true;
				case 'c':
					if (!(val == "cl coat"))
					{
						break;
					}
					itemGroupName = ItemGroupName.布甲_上衣;
					return true;
				case 'm':
					if (!(val == "mt coat"))
					{
						break;
					}
					itemGroupName = ItemGroupName.板甲_上衣;
					return true;
				case 's':
					if (!(val == "support"))
					{
						break;
					}
					itemGroupName = ItemGroupName.辅助装备;
					return true;
				}
				break;
			case 4:
				switch (val[0])
				{
				case 'p':
					if (!(val == "pole"))
					{
						break;
					}
					itemGroupName = ItemGroupName.棍棒;
					return true;
				case 'l':
					if (!(val == "lswd"))
					{
						break;
					}
					itemGroupName = ItemGroupName.巨剑;
					return true;
				case 'c':
					if (!(val == "club"))
					{
						if (!(val == "claw"))
						{
							break;
						}
						itemGroupName = ItemGroupName.爪;
						return true;
					}
					itemGroupName = ItemGroupName.钝器;
					return true;
				case 'w':
					if (!(val == "wand"))
					{
						break;
					}
					itemGroupName = ItemGroupName.手杖;
					return true;
				case 'r':
					if (!(val == "ring"))
					{
						break;
					}
					itemGroupName = ItemGroupName.戒指;
					return true;
				case 'f':
					if (!(val == "food"))
					{
						break;
					}
					itemGroupName = ItemGroupName.食物;
					return true;
				}
				break;
			case 11:
				switch (val[2])
				{
				case ' ':
					if (!(val == "ha shoulder"))
					{
						if (!(val == "la shoulder"))
						{
							if (!(val == "lt shoulder"))
							{
								if (!(val == "cl shoulder"))
								{
									if (!(val == "mt shoulder"))
									{
										break;
									}
									itemGroupName = ItemGroupName.板甲_护肩;
									return true;
								}
								itemGroupName = ItemGroupName.布甲_护肩;
								return true;
							}
							itemGroupName = ItemGroupName.皮甲_护肩;
							return true;
						}
						itemGroupName = ItemGroupName.轻甲_护肩;
						return true;
					}
					itemGroupName = ItemGroupName.重甲_护肩;
					return true;
				case 'g':
					if (!(val == "magic stone"))
					{
						break;
					}
					itemGroupName = ItemGroupName.魔法石;
					return true;
				case 'a':
					if (!(val == "plant stuff"))
					{
						break;
					}
					itemGroupName = ItemGroupName.植物;
					return true;
				case 'o':
					if (!(val == "cloth stuff"))
					{
						break;
					}
					itemGroupName = ItemGroupName.布料;
					return true;
				}
				break;
			case 13:
				switch (val[0])
				{
				case 'e':
					if (!(val == "element stuff"))
					{
						break;
					}
					itemGroupName = ItemGroupName.元素;
					return true;
				case 'm':
					if (!(val == "mineral stuff"))
					{
						break;
					}
					itemGroupName = ItemGroupName.矿物;
					return true;
				}
				break;
			case 10:
				if (!(val == "cube stuff"))
				{
					break;
				}
				itemGroupName = ItemGroupName.晶体;
				return true;
			case 12:
				if (!(val == "animal stuff"))
				{
					break;
				}
				itemGroupName = ItemGroupName.动物;
				return true;
			}
		}
		return false;
	}

	public bool FindSectionIndex(int sectionId, out int indexOut)
	{
		indexOut = -1;
		if (Data == null || DataLen < 7)
		{
			return false;
		}
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			if (Data[i] == 5 && BitConverter.ToInt32(Data, i + 1) == sectionId)
			{
				indexOut = i;
				return true;
			}
		}
		return false;
	}

	public bool GetSectionValue(string sectionName, PvfPack pvf, out string? val)
	{
		int stringTableId = pvf.Strtable.GetStringTableId(sectionName);
		val = null;
		if (stringTableId == -1)
		{
			return false;
		}
		if (!FindSectionIndex(stringTableId, out var indexOut))
		{
			return false;
		}
		if (indexOut + 5 < DataLen)
		{
			val = GetNumericValue(indexOut);
			return true;
		}
		return false;
	}

	public bool GetSectionIntValue(string sectionName, PvfPack pvf, out int val)
	{
		val = -1;
		int stringTableId = pvf.Strtable.GetStringTableId(sectionName);
		if (stringTableId == -1)
		{
			return false;
		}
		if (!FindSectionIndex(stringTableId, out var indexOut))
		{
			return false;
		}
		if (indexOut + 5 < DataLen && Data[indexOut + 5] == 2)
		{
			val = BitConverter.ToInt32(Data, indexOut + 6);
			return true;
		}
		return false;
	}

	public bool GetSectionStringValue(string sectionName, PvfPack pvf, out string val)
	{
		int stringTableId = pvf.Strtable.GetStringTableId(sectionName);
		val = null;
		if (stringTableId == -1)
		{
			return false;
		}
		if (!FindSectionIndex(stringTableId, out var indexOut))
		{
			return false;
		}
		if (indexOut + 5 < DataLen)
		{
			val = pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, indexOut + 6));
			return true;
		}
		return false;
	}

	public bool GetSectionIntValue2(string sectionName, PvfPack pvf, out KeyValuePair<string?, string?>? val)
	{
		int stringTableId = pvf.Strtable.GetStringTableId(sectionName);
		val = null;
		if (stringTableId == -1)
		{
			return false;
		}
		if (!FindSectionIndex(stringTableId, out var indexOut))
		{
			return false;
		}
		if (indexOut + 10 < DataLen)
		{
			val = new KeyValuePair<string, string>(GetNumericValue(indexOut), GetNumericValue(indexOut + 5));
			return true;
		}
		return false;
	}

	public bool GetSectionIntArray(string sectionName, PvfPack pvf, out List<int> list)
	{
		int stringTableId = pvf.Strtable.GetStringTableId(sectionName);
		list = null;
		if (stringTableId == -1)
		{
			return false;
		}
		if (!FindSectionIndex(stringTableId, out var _))
		{
			return false;
		}
		byte[] data = Data;
		list = new List<int>();
		for (int i = stringTableId + 5; i < DataLen; i += 5)
		{
			byte b = data[i];
			if (data[i] == 5)
			{
				break;
			}
			if (b == 2 && i + 5 < DataLen)
			{
				list.Add(BitConverter.ToInt32(Data, i + 1));
				continue;
			}
			return false;
		}
		return true;
	}

	public bool GetNameText(PvfPack pvf, string sectionName, out string? name)
	{
		name = null;
		if (Data == null || DataLen < 7)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId(sectionName);
		if (stringTableId == -1)
		{
			return false;
		}
		for (int i = 2; i < DataLen - 9; i += 5)
		{
			if (Data[i] == 5 && BitConverter.ToInt32(Data, i + 1) == stringTableId)
			{
				if (i < DataLen - 14 && Data[i + 5] == 9 && Data[i + 10] == 10)
				{
					name = pvf.Strview.GetStrText(Data[i + 6], pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, i + 11)), autoConvertStr: true);
					return true;
				}
				if (Data[i + 5] == 7)
				{
					name = pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, i + 6), autoConvertStr: true);
					return true;
				}
			}
		}
		return false;
	}

	private string? GetNumericValue(int index)
	{
		return (ScriptType)Data[index + 5] switch
		{
			ScriptType.Int => BitConverter.ToInt32(Data, index + 6).ToString(),
			ScriptType.Float => DataHelper.FormatFloat(BitConverter.ToSingle(Data, index + 6)),
			_ => null, 
		};
	}

	public bool GetSectionIntArray(PvfPack pvf, string sectionName, out List<int> items)
	{
		items = new List<int>();
		if (Data == null || DataLen < 7)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId(sectionName);
		if (stringTableId == -1)
		{
			return false;
		}
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			if (Data[i] != 5)
			{
				continue;
			}
			int startIndex = i + 1;
			if (BitConverter.ToInt32(Data, startIndex) != stringTableId)
			{
				continue;
			}
			for (int j = i + 5; j < DataLen - 4; j += 5)
			{
				if (Data[j] == 5)
				{
					return items.Count > 0;
				}
				if (j < DataLen && Data[j] == 2)
				{
					items.Add(BitConverter.ToInt32(Data, j + 1));
				}
			}
			return items.Count > 0;
		}
		return items.Count > 0;
	}

	private bool GetSectionStringArray(PvfPack pvf, string sectionName, out List<string> items)
	{
		items = new List<string>();
		if (Data == null || DataLen < 7)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId(sectionName);
		if (stringTableId == -1)
		{
			return false;
		}
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			if (Data[i] != 5)
			{
				continue;
			}
			int startIndex = i + 1;
			if (BitConverter.ToInt32(Data, startIndex) != stringTableId)
			{
				continue;
			}
			for (int j = i + 5; j < DataLen - 4; j += 5)
			{
				if (Data[j] == 5)
				{
					return items.Count > 0;
				}
				if (j < DataLen && Data[j] == 7)
				{
					items.Add(pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, j + 1)));
				}
			}
			return items.Count > 0;
		}
		return items.Count > 0;
	}

	public bool GetSectionTypeIsStrArray(PvfPack pvf, string sectionName, out List<string> items)
	{
		items = new List<string>();
		if (Data == null || DataLen < 7)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId(sectionName);
		if (stringTableId == -1)
		{
			return false;
		}
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			if (Data[i] != 5)
			{
				continue;
			}
			int startIndex = i + 1;
			if (BitConverter.ToInt32(Data, startIndex) != stringTableId)
			{
				continue;
			}
			for (int j = i + 5; j < DataLen - 4; j += 5)
			{
				if (Data[j] == 5)
				{
					return items.Count > 0;
				}
				byte b = Data[j];
				if (j < DataLen && b == 7)
				{
					items.Add(pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, j + 1), autoConvertStr: true));
				}
				else if (j < DataLen && b == 10)
				{
					int strid = BitConverter.ToInt32(Data, j - 4);
					string stringItem = pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, j + 1));
					items.Add(pvf.Strview.GetStrText(strid, stringItem, autoConvertStr: true).Replace("\\n", "\r\n"));
				}
			}
			return items.Count > 0;
		}
		return items.Count > 0;
	}

	public bool GetStackableType(PvfPack pvf, out StackableType? type)
	{
		type = null;
		if (Data == null || DataLen < 7)
		{
			return false;
		}
		int stringTableId = pvf.Strtable.GetStringTableId("[stackable type]");
		if (stringTableId == -1)
		{
			return false;
		}
		for (int i = 2; i < DataLen - 4; i += 5)
		{
			if (Data[i] == 5 && BitConverter.ToInt32(Data, i + 1) == stringTableId && i + 10 < DataLen && Data[i + 5] == 7 && Data[i + 10] == 2)
			{
				string stringItem = pvf.Strtable.GetStringItem(BitConverter.ToInt32(Data, i + 6));
				int num = BitConverter.ToInt32(Data, i + 11);
				if (PvfFileHelper.ScriptContentToStackableType(stringItem, num, out type, out string _))
				{
					return true;
				}
				Ilogger ilogger = AppSetting.Instance.GetIlogger();
				if (ilogger != null)
				{
					ilogger.Error($"未能识别的道具类型 {stringItem}\t{num}");
				}
				return false;
			}
		}
		return false;
	}
}
