using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using Nito.AsyncEx;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Models.Options.Enums;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services;
using PvfCode.Services.PreviewPvfFileFolder;
using PvfCode.Services.PvfParsingNew;
using Utools;
using pvfUtility.WebApi.Dto;

namespace PvfCode;

public class PvfGroup : PvfPack
{
	private readonly Ilogger xapgPoPUS;

	private readonly byte[] uRdz1sJCA;

	private readonly AsyncLock rLrIuFtK03;

	public PvfGroup()
	{
		uRdz1sJCA = new byte[41]
		{
			0, 84, 104, 105, 115, 32, 112, 118, 102, 32,
			80, 97, 99, 107, 32, 119, 97, 115, 32, 99,
			114, 101, 97, 116, 101, 100, 32, 98, 121, 32,
			112, 118, 102, 85, 116, 105, 108, 105, 116, 121,
			46
		};
		rLrIuFtK03 = new AsyncLock();
		xapgPoPUS = AppSetting.Instance.GetService<Ilogger>();
	}

	public void Clear()
	{
		if (base.FileList != null)
		{
			base.FileList = null;
		}
		base.ListFileTable.Clear();
		base.Strtable.Clear();
		base.Strview.Clear();
		base.PvfPackFilePath = null;
		base.PvfIsOpen = false;
		base.EquipmentPartSetTable.Clear();
	}

	public async Task<ResultData> SavePvfPack(string filePath, bool isFastMode, IProgress<double> progress, bool notButtonClick = true)
	{
		Window loadingWin = null;
		if (AppSetting.Instance.PvfConfig.SavePvfLoadingDisableMainWindow && !notButtonClick)
		{
			AppSetting.Instance.MainWindowIsEnabled = false;
			loadingWin = xapgPoPUS.CreateLoadingWindow(AppSetting.Instance.GetIlogger()?.GetStr("mess_SavingPackage"));
			xapgPoPUS.ShowLoadingWindow(loadingWin);
		}
		ResultData resultData = await K5TVSpbde(filePath, progress, notButtonClick);
		if (resultData.IsError)
		{
			string directoryName = Path.GetDirectoryName(filePath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
			int num = 1;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
			defaultInterpolatedStringHandler.AppendFormatted(fileNameWithoutExtension);
			defaultInterpolatedStringHandler.AppendLiteral("(");
			defaultInterpolatedStringHandler.AppendFormatted(num);
			defaultInterpolatedStringHandler.AppendLiteral(").pvf");
			string text = Path.Combine(directoryName, defaultInterpolatedStringHandler.ToStringAndClear());
			while (File.Exists(text))
			{
				num++;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 2);
				defaultInterpolatedStringHandler2.AppendFormatted(fileNameWithoutExtension);
				defaultInterpolatedStringHandler2.AppendLiteral("(");
				defaultInterpolatedStringHandler2.AppendFormatted(num);
				defaultInterpolatedStringHandler2.AppendLiteral(").pvf");
				text = Path.Combine(directoryName, defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			xapgPoPUS.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SavePvfError"), resultData.Msg));
			string msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SavePvfError2"), text);
			if (!notButtonClick)
			{
				xapgPoPUS.ShowMsg(msg);
			}
			xapgPoPUS.Warning(msg);
			resultData = await K5TVSpbde(text, progress, notButtonClick);
		}
		AppSetting.Instance.MainWindowIsEnabled = true;
		xapgPoPUS.CloseLoadingWindow(loadingWin);
		return resultData;
	}

	private async Task<ResultData> K5TVSpbde(string P_0, IProgress<double> P_1, bool P_2)
	{
		ResultData re = new ResultData();
		try
		{
			using (await rLrIuFtK03.LockAsync())
			{
				using Stream output = File.Create(P_0);
				if (base.Strtable.IsStringTableUpdated)
				{
					GetFile("stringtable.bin")?.WriteFileData(base.Strtable.CreateStringTable());
				}
				List<PvfFile> list = base.FileList.Values.OrderBy((PvfFile x) => x.FileNameBytesChecksum).ToList();
				int count = list.Count;
				using BinaryWriter binaryWriter = new BinaryWriter(output);
				binaryWriter.Write(BitConverter.GetBytes(base._guidLen), 0, 4);
				binaryWriter.Write(base.Guid, 0, base._guidLen);
				binaryWriter.Write(BitConverter.GetBytes(base.FileVersion), 0, 4);
				byte[] sourceBytes = pjPkXF2mR(list, P_1);
				base._fileTreeChecksum = PvfAlgorithmHelper.CreateBuffKey(sourceBytes, base._fileTreeLength, (uint)base.FileList.Count);
				binaryWriter.Write(BitConverter.GetBytes(base._fileTreeLength), 0, 4);
				binaryWriter.Write(BitConverter.GetBytes(base._fileTreeChecksum), 0, 4);
				binaryWriter.Write(BitConverter.GetBytes(base.FileList.Count), 0, 4);
				binaryWriter.Write(PvfAlgorithmHelper.EncryptionPvf(sourceBytes, base._fileTreeLength, base._fileTreeChecksum), 0, base._fileTreeLength);
				int num = 0;
				foreach (PvfFile item in list)
				{
					int blockLength = item.GetBlockLength();
					if (blockLength > 0)
					{
						binaryWriter.Write(PvfAlgorithmHelper.EncryptionPvf(item.Data, blockLength, item.Checksum), 0, blockLength);
					}
					if (num % 512 == 0)
					{
						P_1?.Report(ProgressHelper.GetProgressNum(count + num, count * 2));
					}
					num++;
				}
				binaryWriter.Write(uRdz1sJCA);
				binaryWriter.Flush();
				P_1?.Report(100.0);
			}
		}
		catch (Exception ex)
		{
			re.Msg = ex.Message;
		}
		return re;
	}

	private byte[] pjPkXF2mR(IEnumerable<PvfFile> P_0, IProgress<double> P_1)
	{
		base._fileTreeLength = (base.FileList.Aggregate<KeyValuePair<string, PvfFile>, int>(0, (int num3, KeyValuePair<string, PvfFile> fileObj) => num3 + fileObj.Value.FileNameLen + 20) + 3) & -4;
		int num = 0;
		int num2 = 0;
		using MemoryStream memoryStream = new MemoryStream(base._fileTreeLength);
		memoryStream.SetLength(base._fileTreeLength);
		foreach (PvfFile item in P_0)
		{
			int fileNameLen = item.FileNameLen;
			int blockLength = item.GetBlockLength();
			memoryStream.Write(BitConverter.GetBytes(item.FileNameBytesChecksum), 0, 4);
			memoryStream.Write(BitConverter.GetBytes((uint)fileNameLen), 0, 4);
			memoryStream.Write(item.FileNameBytes, 0, fileNameLen);
			memoryStream.Write(BitConverter.GetBytes((uint)item.DataLen), 0, 4);
			memoryStream.Write(BitConverter.GetBytes(item.Checksum), 0, 4);
			memoryStream.Write(BitConverter.GetBytes((uint)num2), 0, 4);
			num2 += blockLength;
			if (num % 1024 == 0)
			{
				P_1?.Report(ProgressHelper.GetProgressNum(num, base.FileList.Count * 2));
			}
			num++;
		}
		return memoryStream.ToArray();
	}

	private bool jc4fepmkk(Stream P_0)
	{
		int num = uRdz1sJCA.Length;
		P_0.Seek(P_0.Length - num, SeekOrigin.Begin);
		for (int i = 0; i < num; i++)
		{
			if (uRdz1sJCA[i] != P_0.ReadByte())
			{
				P_0.Seek(0L, SeekOrigin.Begin);
				return false;
			}
		}
		P_0.Seek(0L, SeekOrigin.Begin);
		return true;
	}

	public Task<bool> OpenPvfPack(string path, IProgress<double> progress)
	{
		lock (this)
		{
			try
			{
				base.PvfPackFilePath = path;
				using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
				{
					HashSet<PvfFile> hashSet = new HashSet<PvfFile>();
					bool flag = jc4fepmkk(binaryReader.BaseStream);
					base._guidLen = binaryReader.ReadInt32();
					base.Guid = binaryReader.ReadBytes(base._guidLen);
					base.FileVersion = binaryReader.ReadInt32();
					base._fileTreeLength = binaryReader.ReadInt32();
					base._fileTreeChecksum = binaryReader.ReadUInt32();
					int num = binaryReader.ReadInt32();
					base.FileList = new Dictionary<string, PvfFile>();
					BinaryReader binaryReader2 = new BinaryReader(new MemoryStream(PvfAlgorithmHelper.DecryptionPvf(binaryReader.ReadBytes(base._fileTreeLength), base._fileTreeLength, base._fileTreeChecksum)));
					int num2 = 0;
					for (int i = 0; i < num; i++)
					{
						uint fileNameChecksum = binaryReader2.ReadUInt32();
						byte[] fileNameBytes = binaryReader2.ReadBytes(binaryReader2.ReadInt32());
						int dataLen = binaryReader2.ReadInt32();
						uint checksum = binaryReader2.ReadUInt32();
						int offset = binaryReader2.ReadInt32();
						PvfFile pvfFile = new PvfFile(fileNameChecksum, fileNameBytes, dataLen, checksum, offset);
						if (pvfFile.FileName.Split(new char[2] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries)[0].Contains("506807329_"))
						{
							num2++;
							continue;
						}
						if (flag || !base.FileList.ContainsKey(pvfFile.FileName))
						{
							base.FileList.Add(pvfFile.FileName, pvfFile);
						}
						else
						{
							string text = pvfFile.FileName;
							while (base.FileList.ContainsKey(text))
							{
								text += "(diff)";
							}
							base.FileList.Add(text, pvfFile);
							hashSet.Add(pvfFile);
						}
						if (i % 512 == 0)
						{
							progress.Report(ProgressHelper.GetProgressNum(i, num));
						}
					}
					long position = binaryReader.BaseStream.Position;
					foreach (KeyValuePair<string, PvfFile> item in base.FileList.Where<KeyValuePair<string, PvfFile>>((KeyValuePair<string, PvfFile> item) => item.Value.DataLen > 0))
					{
						item.Value.Offset += position;
						binaryReader.BaseStream.Seek(item.Value.Offset, SeekOrigin.Begin);
						item.Value.InitFile(binaryReader.ReadBytes(item.Value.GetBlockLength()));
						if (!flag && hashSet.Contains(item.Value))
						{
							item.Value.Rename(item.Key);
						}
					}
					progress.Report(100.0);
				}
				Task.Run(delegate
				{
					if (GetFile("stringtable.bin") != null)
					{
						base.Strtable.Loadstringtable(GetFile("stringtable.bin").Data, base.OverAllEncodingType, this);
					}
					else
					{
						base.Strtable.InitDefault();
						xapgPoPUS.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringTableNotFound"));
					}
					if (FileAny(AppSetting.Instance.PvfConfig.StringLstFileName))
					{
						base.Strview.InitStringData(GetFile(AppSetting.Instance.PvfConfig.StringLstFileName), this, base.OverAllEncodingType);
					}
					else
					{
						base.Strview.InitDefault();
						xapgPoPUS.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLstNotFound"), AppSetting.Instance.PvfConfig.StringLstFileName));
					}
					Task.Run(delegate
					{
						GetEquipmentpartsetInfo(this, out Dictionary<int, Dictionary<string, EquipmentPartSet>> dic);
						base.EquipmentPartSetTable.Init(this, dic);
					});
					Task.Run(delegate
					{
						this.Init();
					});
				});
				return Task.FromResult(result: true);
			}
			catch (Exception ex)
			{
				xapgPoPUS.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_OpenPackageError"), ex.Message));
				return Task.FromResult(result: false);
			}
		}
	}

	public bool GetEquipmentpartsetInfo(PvfPack pvf, out Dictionary<int, Dictionary<string, EquipmentPartSet>> dic)
	{
		dic = new Dictionary<int, Dictionary<string, EquipmentPartSet>>();
		PvfFile file = GetFile("etc/equipmentpartset.etc");
		if (file == null || file.Data == null)
		{
			return false;
		}
		string text = "equipment/";
		ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(file, this);
		scriptFileParserNew.PraseStructureMain();
		if (scriptFileParserNew.Sections == null || scriptFileParserNew.Sections.Count == 0)
		{
			return false;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (PvfSection item in scriptFileParserNew.Sections.Where((SectionBase it) => it is PvfSection && it.GetSectionName() == "[equipment part set]"))
		{
			int num = item.Children.Count;
			if (item.HasEndSection())
			{
				num--;
			}
			List<SectionBase> children = item.Children;
			if (num <= 3)
			{
				continue;
			}
			if (children[1].Item.Type != ScriptType.Int)
			{
				ScriptItem nextItem = ((children[1].Item.Type == ScriptType.StringLinkIndex) ? item.Children[2].Item : null);
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder3 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(46, 2, stringBuilder2);
				handler.AppendLiteral("套装文件错误 [equipment part set]内应以数字编号开头 错误类型为：");
				handler.AppendFormatted(children[1].Item.Type);
				handler.AppendLiteral(" 值：");
				handler.AppendFormatted(children[1].Item.GetItemText(this, nextItem));
				stringBuilder3.AppendLine(ref handler);
				continue;
			}
			int data = children[1].Item.Data;
			if (children[2].Item.Type != ScriptType.String)
			{
				ScriptItem nextItem2 = ((children[2].Item.Type == ScriptType.StringLinkIndex) ? item.Children[3].Item : null);
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder4 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(51, 2, stringBuilder2);
				handler.AppendLiteral("套装文件错误 [equipment part set]内第二个参数应为String型 错误类型：");
				handler.AppendFormatted(children[2].Item.Type);
				handler.AppendLiteral(" 值：");
				handler.AppendFormatted(children[2].Item.GetItemText(this, nextItem2));
				stringBuilder4.AppendLine(ref handler);
				continue;
			}
			string text2 = text + pvf.Strtable.GetStringItem(children[2].Item.Data);
			PvfFile file2 = pvf.GetFile(text2);
			if (file2 == null)
			{
				stringBuilder.AppendLine("套装文件错误 找不到套装信息文件：" + text2);
				continue;
			}
			int num2 = 0;
			Dictionary<string, EquipmentPartSet> dictionary = new Dictionary<string, EquipmentPartSet>();
			EquipmentPartSet equipmentPartSet = new EquipmentPartSet();
			for (int num3 = 3; num3 < num; num3++)
			{
				SectionBase sectionBase = children[num3];
				if (sectionBase == null || sectionBase is PvfSection)
				{
					continue;
				}
				num2++;
				switch (num2)
				{
				case 1:
				{
					if (children[num3].Item.Type != ScriptType.String && children[num3].Item.Type != ScriptType.StringLinkIndex)
					{
						ScriptItem nextItem5 = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? item.Children[num3 + 1].Item : null);
						StringBuilder stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder7 = stringBuilder2;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(41, 2, stringBuilder2);
						handler.AppendLiteral("套装文件错误 套装信息从第二行开始 第一个参数为 String型 当前类型：");
						handler.AppendFormatted(children[num3].Item.Type);
						handler.AppendLiteral(" 值：");
						handler.AppendFormatted(children[num3].Item.GetItemText(this, nextItem5));
						stringBuilder7.AppendLine(ref handler);
						break;
					}
					ScriptItem scriptItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? item.Children[num3 + 1].Item : null);
					equipmentPartSet = new EquipmentPartSet
					{
						Name = children[num3].Item.GetItemTextNotChar(this, scriptItem),
						ParFile = file2
					};
					if (scriptItem != null)
					{
						num3++;
					}
					continue;
				}
				case 2:
				{
					StringBuilder stringBuilder2;
					StringBuilder.AppendInterpolatedStringHandler handler;
					if (children[num3].Item.Type != ScriptType.String)
					{
						ScriptItem nextItem6 = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? item.Children[num3 + 1].Item : null);
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder8 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(41, 2, stringBuilder2);
						handler.AppendLiteral("套装文件错误 套装信息从第二行开始 第二个参数为 String型 当前类型：");
						handler.AppendFormatted(children[num3].Item.Type);
						handler.AppendLiteral(" 值：");
						handler.AppendFormatted(children[num3].Item.GetItemText(this, nextItem6));
						stringBuilder8.AppendLine(ref handler);
						break;
					}
					equipmentPartSet.EquType = children[num3].Item.GetItemTextNotChar(this);
					if (string.IsNullOrEmpty(equipmentPartSet.EquType))
					{
						stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder9 = stringBuilder2;
						handler = new StringBuilder.AppendInterpolatedStringHandler(27, 1, stringBuilder2);
						handler.AppendLiteral("套装文件错误 套装文件的装备类型不能为空！ 套装索引：");
						handler.AppendFormatted(data);
						stringBuilder9.AppendLine(ref handler);
						break;
					}
					if (!dictionary.ContainsKey(equipmentPartSet.EquType))
					{
						continue;
					}
					stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder10 = stringBuilder2;
					handler = new StringBuilder.AppendInterpolatedStringHandler(33, 2, stringBuilder2);
					handler.AppendLiteral("套装文件错误 套装文件的套装类型不能重复！ 套装索引：");
					handler.AppendFormatted(data);
					handler.AppendLiteral(" 重复索引：");
					handler.AppendFormatted(equipmentPartSet.EquType);
					stringBuilder10.AppendLine(ref handler);
					break;
				}
				case 3:
				{
					if (children[num3].Item.Type == ScriptType.Int)
					{
						continue;
					}
					ScriptItem nextItem4 = ((children[num3].Item.Type == ScriptType.StringLinkIndex) ? item.Children[num3 + 1].Item : null);
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder6 = stringBuilder2;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(38, 2, stringBuilder2);
					handler.AppendLiteral("套装文件错误 套装信息从第二行开始 第3个参数为 int型 当前类型：");
					handler.AppendFormatted(children[num3].Item.Type);
					handler.AppendLiteral(" 值：");
					handler.AppendFormatted(children[num3].Item.GetItemText(this, nextItem4));
					stringBuilder6.AppendLine(ref handler);
					break;
				}
				case 4:
					if (children[num3].Item.Type != ScriptType.Int)
					{
						ScriptItem nextItem3 = ((children[num3].Item.Type == ScriptType.StringLinkIndex) ? item.Children[num3 + 1].Item : null);
						StringBuilder stringBuilder2 = stringBuilder;
						StringBuilder stringBuilder5 = stringBuilder2;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(38, 2, stringBuilder2);
						handler.AppendLiteral("套装文件错误 套装信息从第二行开始 第4个参数为 int型 当前类型：");
						handler.AppendFormatted(children[num3].Item.Type);
						handler.AppendLiteral(" 值：");
						handler.AppendFormatted(children[num3].Item.GetItemText(this, nextItem3));
						stringBuilder5.AppendLine(ref handler);
						break;
					}
					num2 = 0;
					dictionary.Add(equipmentPartSet.EquType, equipmentPartSet);
					continue;
				default:
					continue;
				}
				break;
			}
			if (dictionary.Count > 0)
			{
				if (dic.ContainsKey(data))
				{
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder11 = stringBuilder2;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(16, 1, stringBuilder2);
					handler.AppendLiteral("套装文件错误 套装索引编号重复：");
					handler.AppendFormatted(data);
					stringBuilder11.AppendLine(ref handler);
				}
				else
				{
					dic.Add(data, dictionary);
				}
			}
		}
		if (stringBuilder.Length > 0)
		{
			AppSetting.Instance.GetIlogger()?.Error(stringBuilder.ToString());
		}
		return dic.Count > 0;
	}

	public bool Getavatar_select_abilityItems(PvfFile file, out List<avatar_select_ability_Base> items)
	{
		items = new List<avatar_select_ability_Base>();
		byte[] data = file.Data;
		int dataLen = file.DataLen;
		int stringTableId = base.Strtable.GetStringTableId("[avatar select ability]");
		if (stringTableId == -1)
		{
			return false;
		}
		if (data == null || dataLen < 7)
		{
			return false;
		}
		for (int i = 2; i < dataLen - 4; i += 5)
		{
			if (data[i] != 5)
			{
				continue;
			}
			int startIndex = i + 1;
			if (BitConverter.ToInt32(data, startIndex) != stringTableId)
			{
				continue;
			}
			for (int j = i + 5; j < dataLen - 4; j += 5)
			{
				if (data[j] == 5)
				{
					return items.Count > 0;
				}
				string stringItem = base.Strtable.GetStringItem(BitConverter.ToInt32(data, j + 1));
				if (stringItem == "[SKILL_LEVEL]")
				{
					if (j + 15 < dataLen)
					{
						if (data[j + 5] != 7)
						{
							return items.Count > 0;
						}
						if (data[j + 10] != 2)
						{
							return items.Count > 0;
						}
						if (data[j + 15] != 2)
						{
							return items.Count > 0;
						}
						string stringItem2 = base.Strtable.GetStringItem(BitConverter.ToInt32(data, j + 6));
						int skillId = BitConverter.ToInt32(data, j + 11);
						int level = BitConverter.ToInt32(data, j + 16);
						avatar_select_ability_Skill avatar_select_ability_Skill2 = new avatar_select_ability_Skill(stringItem, stringItem2, skillId, level);
						GetJobSkillName(stringItem2, skillId, out string skillName);
						avatar_select_ability_Skill2.SkillName = skillName;
						items.Add(avatar_select_ability_Skill2);
						j += 15;
					}
				}
				else if (j + 10 < dataLen)
				{
					if (data[j + 5] != 7)
					{
						return items.Count > 0;
					}
					if (data[j + 10] != 2)
					{
						return items.Count > 0;
					}
					string stringItem3 = base.Strtable.GetStringItem(BitConverter.ToInt32(data, j + 6));
					int value = BitConverter.ToInt32(data, j + 11);
					items.Add(new avatar_select_ability(stringItem, stringItem3, value));
					j += 10;
				}
			}
			return items.Count > 0;
		}
		return items.Count > 0;
	}

	public override string GetItemName(string filePath)
	{
		base.FileList.TryGetValue(filePath, out PvfFile value);
		return GetItemName(value);
	}

	public override string GetItemName(PvfFile? file)
	{
		if (file == null)
		{
			return null;
		}
		if (base.Strtable == null)
		{
			return null;
		}
		if (!base.Strtable.IsAny())
		{
			return null;
		}
		if (!file.IsScriptFile)
		{
			return null;
		}
		PvfFileType fileType = file.FileType;
		string text;
		switch (fileType)
		{
		case PvfFileType.shp:
			text = PfOZLi4H6(file);
			break;
		case PvfFileType.equ:
			text = Y8RT9piSN(base.Strtable.NameLableOrSetNameLable, file);
			break;
		default:
		{
			int nameLable = base.Strtable.GetNameLable(fileType);
			text = AJu7TlQi7(nameLable, file);
			break;
		}
		}
		if (!string.IsNullOrEmpty(text))
		{
			return AppSetting.Instance.PvfConfig.FileTextTraditionalConvertSimplifiedAutoMethods(text);
		}
		return null;
	}

	private string AJu7TlQi7(int P_0, PvfFile P_1)
	{
		for (int i = 2; i < P_1.DataLen - 9; i += 5)
		{
			if (P_1.Data[i] == 5 && BitConverter.ToInt32(P_1.Data, i + 1) == P_0)
			{
				if (i < P_1.DataLen - 14 && P_1.Data[i + 5] == 9 && P_1.Data[i + 10] == 10)
				{
					return base.Strview.GetStrText(P_1.Data[i + 6], base.Strtable.GetStringItem(BitConverter.ToInt32(P_1.Data, i + 11)));
				}
				if (P_1.Data[i + 5] == 7)
				{
					return base.Strtable.GetStringItem(BitConverter.ToInt32(P_1.Data, i + 6));
				}
			}
		}
		return null;
	}

	private string Y8RT9piSN(HashSet<int> P_0, PvfFile P_1)
	{
		for (int i = 2; i < P_1.DataLen - 9; i += 5)
		{
			if (P_1.Data[i] == 5 && P_0.Contains(BitConverter.ToInt32(P_1.Data, i + 1)))
			{
				if (i < P_1.DataLen - 14 && P_1.Data[i + 5] == 9 && P_1.Data[i + 10] == 10)
				{
					return base.Strview.GetStrText(P_1.Data[i + 6], base.Strtable.GetStringItem(BitConverter.ToInt32(P_1.Data, i + 11)));
				}
				if (P_1.Data[i + 5] == 7)
				{
					return base.Strtable.GetStringItem(BitConverter.ToInt32(P_1.Data, i + 6));
				}
			}
		}
		return null;
	}

	private string PfOZLi4H6(PvfFile? file)
	{
		if (file.GetNpcId(this, out var npcId) && base.ListFileTable.CodeDic.TryGetValue("npc", out Dictionary<int, LstItem> value) && value.TryGetValue(npcId, out var value2))
		{
			string text = GetItemName(GetFile(value2.FullPath));
			if (text != null)
			{
				text = AppSetting.Instance.GetIlogger()?.GetStr("mess_Shop") + "-" + text;
			}
			return text;
		}
		return null;
	}

	public void DeleteFile(string filePath)
	{
		if (base.FileList.ContainsKey(filePath))
		{
			base.FileList.Remove(filePath);
		}
	}

	public DeleteFilesResultDto WebApiDeleteFiles(IEnumerable<string> files)
	{
		DeleteFilesResultDto deleteFilesResultDto = new DeleteFilesResultDto();
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (string file in files)
		{
			if (base.FileList.ContainsKey(file))
			{
				base.FileList.Remove(file);
				list.Add(file);
			}
			else
			{
				list2.Add(file);
			}
		}
		deleteFilesResultDto.SuccessFiles = list;
		deleteFilesResultDto.ErrorFiles = list2;
		return deleteFilesResultDto;
	}

	public void DeleteFiles(IEnumerable<string> filePaths)
	{
		foreach (string filePath in filePaths)
		{
			DeleteFile(filePath);
		}
	}

	public void RenameFile(string olFilPath, string newFilePath)
	{
		PvfFile file = GetFile(olFilPath);
		if (file == null)
		{
			xapgPoPUS.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_RenameError"), olFilPath));
			return;
		}
		base.FileList.Remove(olFilPath);
		file.Rename(newFilePath);
		base.FileList.Add(file.FileName, file);
	}

	public void RenameFolder(string olPath, string olFolderName, string newName)
	{
		int num = olPath.LastIndexOf("/");
		string path = newName;
		if (num != -1)
		{
			path = olPath.Remove(num, olFolderName.Length + 1) + "/" + newName;
		}
		path = PathsHelper.PathFix(path);
		string text = PathsHelper.PathFix(olPath);
		PvfFile[] fileObjs = GetFileObjs(text);
		int length = text.Length;
		PvfFile[] array = fileObjs;
		foreach (PvfFile pvfFile in array)
		{
			string fileName = pvfFile.FileName;
			base.FileList.Remove(fileName);
			string newFileName = path + fileName.Remove(0, length);
			pvfFile.Rename(newFileName);
			base.FileList.Add(pvfFile.FileName, pvfFile);
		}
	}

	public bool ExtractFile(Stream stream, PvfFile file, bool decompileBinaryAni, bool decompileScript, bool convertConvertSimplifiedChinese, bool isOlWebApi = false, bool? useCompatibleDecompiler = null)
	{
		if (file == null)
		{
			return false;
		}
		if (file.DataLen <= 0)
		{
			return true;
		}
		if (file.IsBinaryAniFile && decompileBinaryAni)
		{
			var (flag, s) = BinaryAniCompiler.DecompileBinaryAni(file);
			if (flag)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
				stream.Seek(0L, SeekOrigin.Begin);
			}
			return flag;
		}
		if (file.IsScriptFile && decompileScript)
		{
			if (!useCompatibleDecompiler.HasValue)
			{
				useCompatibleDecompiler = ((!isOlWebApi) ? new bool?(AppSetting.Instance.PvfConfig.UseCompatibleDecompiler) : ((!AppSetting.Instance.ClientApiOptions.UseCompatibleDecompiler) ? new bool?(AppSetting.Instance.PvfConfig.UseCompatibleDecompiler) : new bool?(true)));
			}
			string text = (useCompatibleDecompiler.Value ? new ScriptFileCompilerOl(this).Decompile(file) : new ScriptFileParserNew(file, this).PraseText());
			if (convertConvertSimplifiedChinese)
			{
				text = ChineseHelper.ToSimplified(text);
			}
			byte[] bytes2 = Encoding.UTF8.GetBytes(text);
			stream.Write(bytes2, 0, bytes2.Length);
			stream.Seek(0L, SeekOrigin.Begin);
			return true;
		}
		if (AppSetting.Instance.PvfConfig.DefaultEncoding == EncodingType.TW && convertConvertSimplifiedChinese)
		{
			string source = Encoding.GetEncoding(950).GetString(file.Data);
			source = ChineseHelper.ToSimplified(source);
			byte[] bytes3 = Encoding.UTF8.GetBytes(source);
			stream.Write(bytes3, 0, bytes3.Length);
			stream.Seek(0L, SeekOrigin.Begin);
		}
		else
		{
			stream.Write(file.Data, 0, file.DataLen);
			stream.Seek(0L, SeekOrigin.Begin);
		}
		return true;
	}

	public override bool SaveFileText(PvfFile file, string fileText, EncodingType? encoding = null)
	{
		if (!encoding.HasValue)
		{
			encoding = base.OverAllEncodingType;
		}
		if (file.FileType == PvfFileType.str)
		{
			fileText = AppSetting.Instance.PvfConfig.StrTableAndStrViewConvertStrContent(fileText);
		}
		if (file.IsScriptFile)
		{
			if (fileText.Length >= 9 && fileText.Substring(0, 9) == "#PVF_File")
			{
				return SaveFileAsScript(file, fileText);
			}
			if (xapgPoPUS.ShowDialog(AppSetting.Instance.GetIlogger()?.GetStr("mess_SaveAsScript")) == MessageResult.Yes)
			{
				return SaveFileAsScript(file, fileText);
			}
			return SaveFileAsTextFile(file, fileText, encoding.Value);
		}
		if (file.IsBinaryAniFile)
		{
			return SaveFileAsBinaryAni(file, fileText);
		}
		if (fileText.Length > 10 && fileText.Substring(0, 9) == "#PVF_File")
		{
			return SaveFileAsScript(file, fileText);
		}
		return SaveFileAsTextFile(file, fileText, encoding.Value);
	}

	public override bool SaveFileText(string filePath, string fileText, EncodingType? encoding = null)
	{
		PvfFile file = GetFile(filePath);
		if (file == null)
		{
			xapgPoPUS?.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SaveFileError"), filePath));
			return false;
		}
		return SaveFileText(file, fileText, encoding);
	}

	public bool SaveFileAsScript(PvfFile file, string fileText)
	{
		byte[] array = new ScriptFileCompilerOl(this).Compile(file, fileText);
		if (array != null)
		{
			file.WriteFileData(array);
			if (file.FileType == PvfFileType.lst)
			{
				if (file.FileName == "n_string.lst")
				{
					base.Strview.InitStringData(file, this, base.OverAllEncodingType);
					return true;
				}
				if (file.FileName.IndexOf('/') < 0)
				{
					return true;
				}
				ServiceItemCodeTable.l5AefSxFwx(file, this);
				xapgPoPUS?.Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ReloadLstSuccess"), file.FileName));
			}
			return true;
		}
		return false;
	}

	public bool SaveFileAsBinaryAni(PvfFile file, string fileText)
	{
		var (flag, fileData, item) = BinaryAniCompiler.CompileBinaryAni(fileText, file.FileName);
		if (!flag)
		{
			xapgPoPUS.Error(new List<ErrorItem> { item });
			xapgPoPUS.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError10"));
		}
		else
		{
			file.WriteFileData(fileData);
		}
		return true;
	}

	public bool SaveFileAsTextFile(PvfFile file, string fileText, EncodingType encoding)
	{
		switch (encoding)
		{
		case EncodingType.CN:
			fileText = ChineseHelper.ToSimplified(fileText);
			break;
		case EncodingType.UTF8:
		{
			byte[] bytes = Encoding.UTF8.GetBytes(fileText);
			file.WriteFileData(bytes);
			break;
		}
		default:
			throw new ArgumentOutOfRangeException("encoding", encoding, null);
		case EncodingType.JP:
		case EncodingType.KR:
		case EncodingType.TW:
		case EncodingType.Unicode:
			break;
		}
		byte[] bytes2 = Encoding.GetEncoding((int)encoding).GetBytes(fileText);
		file.WriteFileData(bytes2);
		if (file.FileName.IndexOf(".str", StringComparison.OrdinalIgnoreCase) > 0)
		{
			base.Strview.ReloadstrFile(file.FileName, fileText, this);
		}
		return true;
	}

	public bool ImportUpdateFile(PvfFile file, Stream stream, string fileName, bool compileScript, bool compileBinaryAni, bool convertChinese, bool compileChinaScriptFile = false, bool compile70PlusAni = false)
	{
		_ = stream?.Length;
		if (UpdateFile(file, stream, compileScript, compileBinaryAni, convertChinese, compileChinaScriptFile, compile70PlusAni))
		{
			return true;
		}
		xapgPoPUS.Warning(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportError"), fileName));
		return false;
	}

	public bool ImportNewFile(string filePath, Stream stream, bool compileScript, bool compileBinaryAni, bool convertChinese)
	{
		PvfFile pvfFile = new PvfFile(filePath);
		bool flag = ImportUpdateFile(pvfFile, stream, filePath, compileScript, compileBinaryAni, convertChinese);
		if (flag)
		{
			filePath = pvfFile.FileName;
			lock (this)
			{
				base.FileList.TryAdd(filePath, pvfFile);
			}
		}
		return flag;
	}

	public bool UpdateFile(PvfFile file, Stream stream, bool compileScript, bool compileBinaryAni, bool convertChinese, bool compileChinaScriptFile = false, bool compile70PlusAni = false)
	{
		if (stream == null || stream.Length <= 0)
		{
			return true;
		}
		byte[] array = new byte[stream.Length];
		stream.Seek(0L, SeekOrigin.Begin);
		stream.Read(array, 0, array.Length);
		string text = Encoding.UTF8.GetString(array).TrimEnd(new char[1]);
		if (convertChinese)
		{
			text = ChineseHelper.ToTraditional(text);
			array = Encoding.UTF8.GetBytes(text);
		}
		if (compileBinaryAni && file.IsBinaryAniFile && text.Length > 10 && text.Substring(0, 9).ToLower() == "#pvf_file")
		{
			var (flag, fileData, item) = BinaryAniCompiler.CompileBinaryAni(text, file.FileName, compile70PlusAni);
			if (!flag)
			{
				xapgPoPUS.Error(new List<ErrorItem> { item });
				return false;
			}
			file.WriteFileData(fileData);
			return true;
		}
		if (compileScript && text.Length > 10 && text.Substring(0, 9).ToLower() == "#pvf_file")
		{
			byte[] array2 = new ScriptFileCompilerOl(this).Compile(file, text, compileChinaScriptFile);
			if (array2 == null)
			{
				return false;
			}
			file.WriteFileData(array2);
			return true;
		}
		file.WriteFileData(array);
		return true;
	}

	public IEnumerable<string> MoveFile(string path, string newPath, bool cut)
	{
		lock (base.FileList)
		{
			if (base.FileList.ContainsKey(path))
			{
				newPath = PathsHelper.PathFix(newPath);
				string item;
				if (cut)
				{
					PvfFile file = GetFile(path);
					if (file == null)
					{
						return null;
					}
					string text = newPath + file.ShortName;
					while (GetFile(text) != null)
					{
						text += "(copy)";
					}
					file.Rename(text);
					base.FileList.Remove(path);
					base.FileList.Add(text, file);
					item = text;
				}
				else
				{
					PvfFile file2 = GetFile(path);
					if (file2 == null)
					{
						return null;
					}
					string text2 = newPath + file2.ShortName;
					PvfFile pvfFile = new PvfFile();
					while (GetFile(text2) != null)
					{
						text2 += "(copy)";
					}
					pvfFile.InitNewCopyFile(file2.Data, text2);
					base.FileList.Add(text2, pvfFile);
					item = text2;
				}
				return new List<string> { item };
			}
			newPath = PathsHelper.PathFix(newPath);
			int count = (path.Contains("/") ? (path.LastIndexOf('/') + 1) : 0);
			path = PathsHelper.PathFix(path);
			List<string> list = new List<string>();
			if (cut)
			{
				PvfFile[] fileObjs = GetFileObjs(path);
				foreach (PvfFile pvfFile2 in fileObjs)
				{
					string text3 = newPath + pvfFile2.FileName.Remove(0, count);
					while (GetFile(text3) != null)
					{
						text3 += "(copy)";
					}
					base.FileList.Remove(pvfFile2.FileName);
					pvfFile2.Rename(text3);
					list.Add(text3);
					base.FileList.Add(text3, pvfFile2);
				}
			}
			else
			{
				PvfFile[] fileObjs = GetFileObjs(path);
				foreach (PvfFile pvfFile3 in fileObjs)
				{
					string text4 = newPath + pvfFile3.FileName.Remove(0, count);
					while (GetFile(text4) != null)
					{
						text4 += "(copy)";
					}
					PvfFile pvfFile4 = new PvfFile();
					pvfFile4.InitNewCopyFile(pvfFile3.Data, text4);
					list.Add(pvfFile4.FileName);
					base.FileList.Add(text4, pvfFile4);
				}
			}
			return list;
		}
	}

	public ConcurrentDictionary<string, ConcurrentDictionary<int, string>> FilesToLstDic(IEnumerable<PvfFile> files)
	{
		ConcurrentDictionary<string, ConcurrentDictionary<int, string>> concurrentDictionary = new ConcurrentDictionary<string, ConcurrentDictionary<int, string>>();
		foreach (PvfFile file in files)
		{
			if (!file.IsScriptFile)
			{
				continue;
			}
			KeyValuePair<int, string>? keyValuePair = file.ToLstItem();
			if (keyValuePair.HasValue)
			{
				string key = file.GetLstPathHeader();
				if (base.ListFileTable.LstFilePaths.ContainsKey(key))
				{
					key = base.ListFileTable.LstFilePaths[key];
				}
				if (!concurrentDictionary.ContainsKey(key))
				{
					concurrentDictionary.TryAdd(key, new ConcurrentDictionary<int, string>(new List<KeyValuePair<int, string>> { keyValuePair.Value }));
				}
				else if (!concurrentDictionary[key].ContainsKey(keyValuePair.Value.Key))
				{
					concurrentDictionary[key].TryAdd(keyValuePair.Value.Key, keyValuePair.Value.Value);
				}
			}
		}
		return concurrentDictionary;
	}

	public ConcurrentDictionary<string, ConcurrentDictionary<int, string>> FilesToLstDic(IEnumerable<string> fileList)
	{
		List<PvfFile> list = new List<PvfFile>();
		foreach (string file in fileList)
		{
			if (base.FileList.TryGetValue(file, out PvfFile value))
			{
				list.Add(value);
			}
		}
		return FilesToLstDic(list);
	}

	public string GetItemCodeAndItemNames(IEnumerable<string> fileList, out int outCount)
	{
		int num = 0;
		List<string> list = new List<string>();
		foreach (string file in fileList)
		{
			if (!base.FileList.TryGetValue(file, out PvfFile value))
			{
				continue;
			}
			int? itemCode = value.ItemCode;
			string itemName = GetItemName(value);
			if (itemCode.HasValue || itemName != null)
			{
				num++;
				if (AppSetting.Instance.TreeSetting.TreeListGetItemNameAndItemCodeFormat == TreeListGetItemNameAndItemCodeFormat.名称在前_代码在后)
				{
					list.Add($"{itemName}{AppSetting.Instance.TreeSetting.GetItemNameAndItemCodeSplitChar}{itemCode}");
				}
				else
				{
					list.Add($"{itemCode}{AppSetting.Instance.TreeSetting.GetItemNameAndItemCodeSplitChar}{itemName}");
				}
			}
		}
		if (AppSetting.Instance.TreeSetting.GetItemNameAndItemCodeIsSort)
		{
			list.Sort((string a, string b) => a.CompareTo(b));
		}
		outCount = num;
		return string.Join("\r\n", list);
	}

	public void AddNewEmptyPvfFile()
	{
		base.FileList = new Dictionary<string, PvfFile>();
		base.Guid = Encoding.UTF8.GetBytes("fa08bf71-4395-6a4b-a3e3-261789fee129");
		base.FileVersion = int.Parse("66282");
		base._guidLen = 36;
		base.Strtable.InitDefault();
		base.Strview.InitDefault();
		base.PvfIsOpen = true;
		string text = "test.txt";
		base.FileList.Add(text, new PvfFile(text));
		SaveFileText(text, "//测试文件，请删除\r\n//Test file, please delete\r\n//테스트 파일, 삭제해 주세요\r\n//テストファイル、削除してください");
		string text2 = "stringtable.bin";
		base.FileList.Add(text2, new PvfFile(text2));
	}

	public bool GetJobSkillName(string jobType, int skillId, out string? skillName)
	{
		skillName = null;
		if (jobType == null)
		{
			goto IL_064e;
		}
		switch (jobType.Length)
		{
		case 8:
			break;
		case 9:
			goto IL_0094;
		case 11:
			goto IL_00b1;
		case 12:
			goto IL_00ce;
		case 10:
			goto IL_01a9;
		case 6:
			goto IL_0236;
		case 18:
			goto IL_0314;
		case 14:
			goto IL_033f;
		case 13:
			goto IL_036a;
		case 7:
			goto IL_039b;
		case 16:
			goto IL_03c6;
		default:
			goto IL_064e;
		}
		char c = jobType[1];
		string text;
		if ((uint)c <= 103u)
		{
			if (c != 'c')
			{
				if (c != 'g' || !(jobType == "[gunner]"))
				{
					goto IL_064e;
				}
				text = "skill/gunner";
			}
			else
			{
				if (!(jobType == "[common]"))
				{
					goto IL_064e;
				}
				text = "skill/swordman";
			}
		}
		else if (c != 'k')
		{
			if (c != 'p' || !(jobType == "[priest]"))
			{
				goto IL_064e;
			}
			text = "skill/priest";
		}
		else
		{
			if (!(jobType == "[knight]"))
			{
				goto IL_064e;
			}
			text = "skill/knight";
		}
		goto IL_0650;
		IL_0650:
		if (text == null)
		{
			return false;
		}
		return CdlhwhD2s(text, skillId, out skillName);
		IL_0236:
		if (!(jobType == "[mage]"))
		{
			goto IL_064e;
		}
		text = "skill/mage";
		goto IL_0650;
		IL_036a:
		if (!(jobType == "[at swordman]"))
		{
			goto IL_064e;
		}
		text = "skill/atswordman";
		goto IL_0650;
		IL_039b:
		if (!(jobType == "[thief]"))
		{
			goto IL_064e;
		}
		text = "skill/thief";
		goto IL_0650;
		IL_03c6:
		if (!(jobType == "[demonic lancer]"))
		{
			goto IL_064e;
		}
		text = "skill/demoniclancer";
		goto IL_0650;
		IL_0314:
		if (!(jobType == "[demonic swordman]"))
		{
			goto IL_064e;
		}
		text = "skill/demonicswordman";
		goto IL_0650;
		IL_0094:
		c = jobType[1];
		if (c != 'a')
		{
			if (c != 'f' || !(jobType == "[fighter]"))
			{
				goto IL_064e;
			}
			text = "skill/fighter";
		}
		else
		{
			if (!(jobType == "[at mage]"))
			{
				goto IL_064e;
			}
			text = "skill/atmage";
		}
		goto IL_0650;
		IL_033f:
		if (!(jobType == "[creator mage]"))
		{
			goto IL_064e;
		}
		text = "skill/creatormage";
		goto IL_0650;
		IL_00ce:
		c = jobType[1];
		if (c != 'a')
		{
			if (c != 'g' || !(jobType == "[gun blader]"))
			{
				goto IL_064e;
			}
			text = "skill/gunblader";
		}
		else
		{
			if (!(jobType == "[at fighter]"))
			{
				goto IL_064e;
			}
			text = "skill/atfighter";
		}
		goto IL_0650;
		IL_00b1:
		c = jobType[4];
		if (c != 'g')
		{
			if (c != 'p' || !(jobType == "[at priest]"))
			{
				goto IL_064e;
			}
			text = "skill/atpriest";
		}
		else
		{
			if (!(jobType == "[at gunner]"))
			{
				goto IL_064e;
			}
			text = "skill/atgunner";
		}
		goto IL_0650;
		IL_064e:
		text = null;
		goto IL_0650;
		IL_01a9:
		if (!(jobType == "[swordman]"))
		{
			goto IL_064e;
		}
		text = "skill/swordman";
		goto IL_0650;
	}

	private bool CdlhwhD2s(string P_0, int P_1, out string P_2)
	{
		string text = base.ListFileTable.ItemCodeConvertFilePath(P_0, P_1);
		if (text == null)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 2);
			defaultInterpolatedStringHandler.AppendLiteral("在：");
			defaultInterpolatedStringHandler.AppendFormatted(P_0);
			defaultInterpolatedStringHandler.AppendLiteral("找不到 技能ID：");
			defaultInterpolatedStringHandler.AppendFormatted(P_1);
			P_2 = defaultInterpolatedStringHandler.ToStringAndClear();
			return false;
		}
		P_2 = GetItemName(text);
		if (string.IsNullOrEmpty(P_2))
		{
			P_2 = "未知技能";
		}
		return true;
	}

	public async void Tr()
	{
		PvfFile[] fileObjs = GetFileObjs("equipment");
		HashSet<string> hashSet = new HashSet<string>
		{
			"equipment/equipment.lst",
			"equipment/equipment.kor.str"
		};
		PvfFile[] array = fileObjs;
		foreach (PvfFile pvfFile in array)
		{
			if (pvfFile.ItemCode.HasValue)
			{
				hashSet.Add(pvfFile.FileName);
			}
		}
		PvfFile[] fileObjs2 = GetFileObjs("stackable");
		hashSet.Add("stackable/stackable.kor.str");
		hashSet.Add("stackable/stackable.lst");
		array = fileObjs2;
		foreach (PvfFile pvfFile2 in array)
		{
			if (pvfFile2.ItemCode.HasValue)
			{
				hashSet.Add(pvfFile2.FileName);
			}
		}
		PvfFile[] fileObjs3 = GetFileObjs("skill");
		hashSet.Add("skill/skill.kor.str");
		array = fileObjs3;
		foreach (PvfFile pvfFile3 in array)
		{
			if (pvfFile3.ItemCode.HasValue || pvfFile3.FileType == PvfFileType.lst)
			{
				hashSet.Add(pvfFile3.FileName);
			}
		}
		hashSet.Add("n_string.lst");
		hashSet.Add("stringtable.bin");
		hashSet.Add("etc/equipmentpartset.etc");
		KeyValuePair<string, PvfFile>[] array2 = base.FileList.ToArray();
		for (int i = 0; i < array2.Length; i++)
		{
			KeyValuePair<string, PvfFile> keyValuePair = array2[i];
			if (!hashSet.Contains(keyValuePair.Key))
			{
				base.FileList.Remove(keyValuePair.Key);
			}
		}
		await base.Strtable.DeletingInvalidReferences(this);
		xapgPoPUS.ShowMsg("SUCCESS");
	}

	[CompilerGenerated]
	private void Fl0dMnEGo()
	{
		if (GetFile("stringtable.bin") != null)
		{
			base.Strtable.Loadstringtable(GetFile("stringtable.bin").Data, base.OverAllEncodingType, this);
		}
		else
		{
			base.Strtable.InitDefault();
			xapgPoPUS.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringTableNotFound"));
		}
		if (FileAny(AppSetting.Instance.PvfConfig.StringLstFileName))
		{
			base.Strview.InitStringData(GetFile(AppSetting.Instance.PvfConfig.StringLstFileName), this, base.OverAllEncodingType);
		}
		else
		{
			base.Strview.InitDefault();
			xapgPoPUS.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLstNotFound"), AppSetting.Instance.PvfConfig.StringLstFileName));
		}
		Task.Run(delegate
		{
			GetEquipmentpartsetInfo(this, out Dictionary<int, Dictionary<string, EquipmentPartSet>> dic);
			base.EquipmentPartSetTable.Init(this, dic);
		});
		Task.Run(delegate
		{
			this.Init();
		});
	}

	[CompilerGenerated]
	private void w2y97kNQi()
	{
		GetEquipmentpartsetInfo(this, out Dictionary<int, Dictionary<string, EquipmentPartSet>> dic);
		base.EquipmentPartSetTable.Init(this, dic);
	}

	[CompilerGenerated]
	private void FosqgQUwu()
	{
		this.Init();
	}
}
