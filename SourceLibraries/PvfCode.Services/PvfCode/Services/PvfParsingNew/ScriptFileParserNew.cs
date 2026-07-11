using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PvfCode.Models.Pvf.Attributes;
using PvfCode.Services.PvfParsingNew.CustomSectionFormat;
using PvfCode.Services.PvfParsingNew.TableFormatters;

namespace PvfCode.Services.PvfParsingNew;

public class ScriptFileParserNew
{
	private readonly PvfGroup pvfGroup;

	private readonly List<ScriptItem> scriptItems;

	private readonly PvfFile sourceFile;

	public List<SectionBase> Sections;

	public ScriptFileParserNew(PvfFile file, PvfGroup pack)
	{
		sourceFile = file;
		pvfGroup = pack;
		scriptItems = new List<ScriptItem>();
		if (file.Data == null || file.DataLen < 7)
		{
			return;
		}
		for (int i = 2; i < file.DataLen - 4; i += 5)
		{
			byte b = file.Data[i];
			if (b >= 2 && b <= 10)
			{
				ScriptItem item = new ScriptItem
				{
					Type = (ScriptType)b,
					Data = BitConverter.ToInt32(file.Data, i + 1)
				};
				scriptItems.Add(item);
			}
		}
	}

	public void PraseStructureMain()
	{
		Sections = new List<SectionBase>();
		int num = 0;
		int count = scriptItems.Count;
		while (num < count)
		{
			ScriptItem scriptItem = scriptItems[num];
			if (scriptItem.Type == ScriptType.Section)
			{
				string text = GetStringTableValue(scriptItem.Data);
				string text2 = "[/" + text.Remove(0, 1);
				int num2;
				bool hasEndTag = TryFindEndSection(scriptItems, num, text2, out num2);
				PvfSection pvfSection = new PvfSection(text, hasEndTag, isRootSection: true);
				Sections.Add(pvfSection);
				if (pvfSection.HasEndTag)
				{
					ParseSectionChildren(pvfSection, scriptItems.GetRange(num, num2 + 1));
					num += num2 + 1;
				}
				else
				{
					ParseSectionChildren(pvfSection, scriptItems.GetRange(num, num2));
					num += num2;
				}
			}
			else
			{
				Sections.Add(new SectionBase(scriptItem));
				num++;
			}
		}
	}

	private void ParseSectionChildren(SectionBase parentSection, List<ScriptItem> sectionItems)
	{
		int count = sectionItems.Count;
		for (int i = 0; i < count; i++)
		{
			if (sectionItems[i].Type == ScriptType.Section && i != 0 && i != count - 1)
			{
				string text = GetStringTableValue(sectionItems[i].Data);
				if (string.IsNullOrEmpty(text))
				{
					throw new Exception("PraseSection失败 文件：" + sourceFile.FileName);
				}
				string text2 = "[/" + text.Remove(0, 1);
				int num;
				bool hasEndTag = TryFindEndSection(sectionItems, i, text2, out num);
				PvfSection pvfSection = new PvfSection(text, hasEndTag);
				parentSection.Children.Add(pvfSection);
				if (pvfSection.HasEndTag)
				{
					ParseSectionChildren(pvfSection, sectionItems.GetRange(i, num + 1));
					i = ((!parentSection.HasEndSection()) ? (i + (num + 1)) : (i + num));
				}
				else
				{
					ParseSectionChildren(pvfSection, sectionItems.GetRange(i, num));
					i = ((!parentSection.HasEndSection()) ? (i + num) : (i + (num - 1)));
				}
			}
			else
			{
				parentSection.Children.Add(new SectionBase(sectionItems[i]));
			}
		}
	}

	private bool TryFindEndSection(IReadOnlyList<ScriptItem> items, int startIndex, string endSectionName, out int sectionLength)
	{
		int stringTableId = pvfGroup.Strtable.GetStringTableId(endSectionName);
		int count = items.Count;
		if (stringTableId == -1)
		{
			for (int i = startIndex + 1; i < count; i++)
			{
				if (items[i].Type == ScriptType.Section)
				{
					sectionLength = i - startIndex;
					return false;
				}
			}
			sectionLength = count - startIndex;
			return false;
		}
		int? firstNestedSectionOffset = null;
		for (int j = startIndex + 1; j < count; j++)
		{
			if (items[j].Type == ScriptType.Section)
			{
				if (items[j].Data == stringTableId)
				{
					sectionLength = j - startIndex;
					return true;
				}
				if (!firstNestedSectionOffset.HasValue)
				{
					firstNestedSectionOffset = j - startIndex;
				}
			}
		}
		sectionLength = firstNestedSectionOffset ?? count - startIndex;
		return false;
	}

	private string GetStringTableValue(int index)
	{
		return pvfGroup.Strtable.GetStringItem(index);
	}

	public string PraseText()
	{
		PraseStructureMain();
		return GetText();
	}

	public string GetText()
	{
		StringBuilder stringBuilder = new StringBuilder("#PVF_File");
		stringBuilder.Append(Environment.NewLine);
		string fileName = sourceFile.FileName;
		Dictionary<string, List<CustomSectionFormatBase>> dic = PraserInfoProviderConfiger.GetDic(fileName);
		TableRowFormatterBase tableRowFormatter = PraserInfoProviderConfiger.GetTableRowFormatter(fileName, sourceFile.FilePathHeader);
		for (int i = 0; i < Sections.Count; i++)
		{
			SectionBase sectionBase = Sections[i];
			if (sectionBase is PvfSection)
			{
				CustomSectionFormatBase config = PraserInfoProviderConfiger.GetConfig(dic, sourceFile, pvfGroup, sectionBase.GetSectionName(), null);
				stringBuilder.Append(config.ProcessSectionText(pvfGroup, sourceFile, sectionBase, AppSetting.Instance.EditConfig.FirstColumnTab, dic, sectionBase.GetSectionName()));
				stringBuilder.Append(Environment.NewLine);
				continue;
			}
			if (tableRowFormatter == null)
			{
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(sectionBase.Item.GetItemText(pvfGroup, (sectionBase.Item.Type == ScriptType.StringLinkIndex) ? Sections[i + 1].Item : null));
				continue;
			}
			if (i == 0)
			{
				stringBuilder.Append(Environment.NewLine);
			}
			tableRowFormatter.AppendItem(stringBuilder, Sections, sectionBase, pvfGroup, i, out i);
		}
		return stringBuilder.ToString();
	}

	public string GetItemVlaue(SectionBase item, int i)
	{
		return item.Item.GetItemText(pvfGroup, (item.Item.Type == ScriptType.StringLinkIndex) ? Sections[i + 1].Item : null);
	}

	public List<WebApiFileData> WebApiGetFileData()
	{
		List<WebApiFileData> list = new List<WebApiFileData>();
		for (int i = 0; i < Sections.Count; i++)
		{
			SectionBase sectionBase = Sections[i];
			if (sectionBase is PvfSection pvfSection)
			{
				list.Add(CreateWebApiFileData(pvfSection));
				continue;
			}
			list.Add(new WebApiFileData
			{
				DataType = sectionBase.Item.Type,
				Value = sectionBase.Item.GetItemText(pvfGroup, (sectionBase.Item.Type == ScriptType.StringLinkIndex) ? Sections[i + 1].Item : null)
			});
		}
		return list;
	}

	private WebApiFileData CreateWebApiFileData(PvfSection section)
	{
		WebApiFileData webApiFileData = new WebApiFileData();
		int num = section.Children.Count;
		if (section.HasEndSection())
		{
			num--;
		}
		webApiFileData.SectionName = section.GetSectionName();
		webApiFileData.DataType = ScriptType.Section;
		webApiFileData.HasEndSection = section.HasEndSection();
		for (int i = 1; i < num; i++)
		{
			SectionBase sectionBase = section.Children[i];
			if (sectionBase is PvfSection pvfSection)
			{
				webApiFileData.Children.Add(CreateWebApiFileData(pvfSection));
				continue;
			}
			ScriptItem scriptItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? section.Children[i + 1].Item : null);
			if (scriptItem != null)
			{
				i++;
			}
			string itemText = sectionBase.Item.GetItemText(pvfGroup, scriptItem);
			webApiFileData.Children.Add(new WebApiFileData
			{
				DataType = sectionBase.Item.Type,
				Value = itemText
			});
		}
		return webApiFileData;
	}

	public bool GetSectionValue(PvfGroup pvf, string sectionName, List<SectionBase> sections, out string? val)
	{
		val = null;
		if (sections == null)
		{
			return false;
		}
		SectionBase sectionBase = sections.FirstOrDefault(item => item.GetSectionName() == sectionName);
		if (sectionBase != null && sectionBase.Children.Count >= 2)
		{
			ScriptItem nextItem = null;
			if (sectionBase.Children[1].Item.Type == ScriptType.StringLinkIndex && sectionBase.Children.Count >= 3)
			{
				nextItem = sectionBase.Children[2].Item;
			}
			val = sectionBase.Children[1].Item.GetItemTextNotChar(pvf, nextItem);
			return true;
		}
		return false;
	}

	public bool GetSkillLevelup(List<SectionBase> sectionRoots, out List<SkillLevelupData> items)
	{
		items = new List<SkillLevelupData>();
		if (sectionRoots == null)
		{
			return false;
		}
		SectionBase sectionBase = sectionRoots.Where((SectionBase it) => it.GetSectionName() == "[skill levelup]").FirstOrDefault();
		if (sectionBase == null)
		{
			return false;
		}
		if (sectionBase.Children.Count < 2)
		{
			return false;
		}
		int num = (sectionBase.HasEndSection() ? (sectionBase.Children.Count - 2) : (sectionBase.Children.Count - 1));
		if (num % 3 == 0)
		{
			int num2 = num / 3;
			List<SectionBase> children = sectionBase.Children;
			int num3 = 1;
			for (int num4 = 1; num4 < num2 + 1; num4++)
			{
				List<SectionBase> range = children.GetRange(num3, 3);
				SkillLevelupData skillLevelupData = new SkillLevelupData();
				if (range[0].Item.Type == ScriptType.String)
				{
					skillLevelupData.JobType = range[0].Item.GetItemTextNotChar(pvfGroup);
				}
				if (range[1].Item.Type == ScriptType.Int)
				{
					skillLevelupData.SkillId = range[1].Item.Data;
				}
				if (range[2].Item.Type == ScriptType.Int)
				{
					skillLevelupData.UpLevel = range[2].Item.Data;
				}
				items.Add(skillLevelupData);
				num3 += 3;
			}
		}
		return items.Count > 0;
	}

	public bool GetGetSkillLevelUpString(List<SectionBase> sections, out StringBuilder bui)
	{
		bui = new StringBuilder();
		if (sections == null)
		{
			return false;
		}
		if (!GetSkillLevelup(sections, out List<SkillLevelupData> items))
		{
			return false;
		}
		foreach (SkillLevelupData item in items)
		{
			string jobType = item.JobType;
			string text;
			char c;
			if (jobType != null)
			{
				switch (jobType.Length)
				{
				case 8:
					break;
				case 9:
					goto IL_00d9;
				case 11:
					goto IL_00fa;
				case 12:
					goto IL_011b;
				case 5:
					goto IL_01f9;
				case 10:
					goto IL_0225;
				case 6:
					goto IL_02b1;
				case 18:
					goto IL_038d;
				case 14:
					goto IL_03bf;
				case 13:
					goto IL_03ec;
				case 7:
					goto IL_041e;
				case 16:
					goto IL_044a;
				default:
					goto IL_06d1;
				}
				c = jobType[1];
				if ((uint)c > 103u)
				{
					if (c != 'k')
					{
						if (c != 'p' || !(jobType == "[priest]"))
						{
							goto IL_06d1;
						}
						text = "skill/priest";
					}
					else
					{
						if (!(jobType == "[knight]"))
						{
							goto IL_06d1;
						}
						text = "skill/knight";
					}
					goto IL_06d3;
				}
				if (c != 'c')
				{
					if (c == 'g' && jobType == "[gunner]")
					{
						text = "skill/gunner";
						goto IL_06d3;
					}
				}
				else if (jobType == "[common]")
				{
					goto IL_047c;
				}
			}
			goto IL_06d1;
			IL_0225:
			if (!(jobType == "[swordman]"))
			{
				goto IL_06d1;
			}
			text = "skill/swordman";
			goto IL_06d3;
			IL_06d3:
			if (text == null)
			{
				bui.AppendLine("无法识别的职业类型 请联系作者");
				return true;
			}
			if (TryGetSkillName(text, item.SkillId, out string value))
			{
				PvfFileHelper.JobDefaultStringConvertEnum(item.JobType, out var jobType2);
				StringBuilder stringBuilder = bui;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(12, 3, stringBuilder);
				handler.AppendLiteral("[");
				handler.AppendFormatted(value);
				handler.AppendLiteral("]技能lv +");
				handler.AppendFormatted(item.UpLevel);
				handler.AppendLiteral("  [");
				handler.AppendFormatted(jobType2);
				handler.AppendLiteral("]");
				stringBuilder.AppendLine(ref handler);
			}
			else
			{
				bui.AppendLine(value);
			}
			continue;
			IL_044a:
			if (!(jobType == "[demonic lancer]"))
			{
				goto IL_06d1;
			}
			text = "skill/demoniclancer";
			goto IL_06d3;
			IL_01f9:
			if (jobType == "[all]")
			{
				goto IL_047c;
			}
			goto IL_06d1;
			IL_041e:
			if (!(jobType == "[thief]"))
			{
				goto IL_06d1;
			}
			text = "skill/thief";
			goto IL_06d3;
			IL_00d9:
			c = jobType[1];
			if (c != 'a')
			{
				if (c != 'f' || !(jobType == "[fighter]"))
				{
					goto IL_06d1;
				}
				text = "skill/fighter";
			}
			else
			{
				if (!(jobType == "[at mage]"))
				{
					goto IL_06d1;
				}
				text = "skill/atmage";
			}
			goto IL_06d3;
			IL_03ec:
			if (!(jobType == "[at swordman]"))
			{
				goto IL_06d1;
			}
			text = "skill/atswordman";
			goto IL_06d3;
			IL_00fa:
			c = jobType[4];
			if (c != 'g')
			{
				if (c != 'p' || !(jobType == "[at priest]"))
				{
					goto IL_06d1;
				}
				text = "skill/atpriest";
			}
			else
			{
				if (!(jobType == "[at gunner]"))
				{
					goto IL_06d1;
				}
				text = "skill/atgunner";
			}
			goto IL_06d3;
			IL_03bf:
			if (!(jobType == "[creator mage]"))
			{
				goto IL_06d1;
			}
			text = "skill/creatormage";
			goto IL_06d3;
			IL_047c:
			text = "skill/swordman";
			goto IL_06d3;
			IL_038d:
			if (!(jobType == "[demonic swordman]"))
			{
				goto IL_06d1;
			}
			text = "skill/demonicswordman";
			goto IL_06d3;
			IL_06d1:
			text = null;
			goto IL_06d3;
			IL_02b1:
			if (!(jobType == "[mage]"))
			{
				goto IL_06d1;
			}
			text = "skill/mage";
			goto IL_06d3;
			IL_011b:
			c = jobType[1];
			if (c != 'a')
			{
				if (c != 'g' || !(jobType == "[gun blader]"))
				{
					goto IL_06d1;
				}
				text = "skill/gunblader";
			}
			else
			{
				if (!(jobType == "[at fighter]"))
				{
					goto IL_06d1;
				}
				text = "skill/atfighter";
			}
			goto IL_06d3;
		}
		return bui.Length > 0;
	}

	private bool TryGetSkillName(string skillDirectory, int skillId, out string skillName)
	{
		string skillPath = pvfGroup.ListFileTable.ItemCodeConvertFilePath(skillDirectory, skillId);
		if (skillPath == null)
		{
			skillName = $"在：{skillDirectory}找不到 技能ID：{skillId}";
			return false;
		}
		skillName = pvfGroup.GetItemName(skillPath);
		if (string.IsNullOrEmpty(skillName))
		{
			skillName = "未知技能";
		}
		return true;
	}

	public bool GetItemAuraExplainData(List<SectionBase> sections, out List<ItemAuraExplainData>? items)
	{
		items = new List<ItemAuraExplainData>();
		if (sections == null || sections.Count == 0)
		{
			return false;
		}
		IEnumerable<SectionBase> enumerable = sections.Where((SectionBase it) => it.GetSectionName() == "[item aura]");
		if (enumerable == null || !enumerable.Any())
		{
			return false;
		}
		foreach (SectionBase item in enumerable)
		{
			if (item.Children != null && item.Children.Count == 5)
			{
				ItemAuraExplainData itemAuraExplainData = new ItemAuraExplainData();
				if (item.Children[1].Item.Type == ScriptType.String)
				{
					itemAuraExplainData.Command = item.Children[1].Item.GetItemTextNotChar(pvfGroup);
				}
				if (item.Children[2].Item.Type == ScriptType.String)
				{
					itemAuraExplainData.CalculationType = item.Children[2].Item.GetItemTextNotChar(pvfGroup);
				}
				if (item.Children[3].Item.Type == ScriptType.Int)
				{
					itemAuraExplainData.Value1 = item.Children[3].Item.Data;
				}
				if (item.Children[4].Item.Type == ScriptType.Int)
				{
					itemAuraExplainData.Value1 = item.Children[4].Item.Data;
				}
				items.Add(itemAuraExplainData);
			}
		}
		return items.Count > 0;
	}

	public bool GetItemAuraExplainDataString(List<SectionBase> sections, out StringBuilder bui)
	{
		bui = new StringBuilder();
		if (sections == null)
		{
			return false;
		}
		if (!GetItemAuraExplainData(sections, out List<ItemAuraExplainData> items) || items == null || items.Count == 0)
		{
			return false;
		}
		foreach (ItemAuraExplainData item in items)
		{
			string command = item.Command;
			if (command == null)
			{
				continue;
			}
			switch (command.Length)
			{
			case 16:
				switch (command[2])
				{
				case 'y':
					if (command == "physical defense")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder32 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的体力。");
						stringBuilder32.AppendLine(ref handler);
					}
					break;
				case 't':
					if (command == "water resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder28 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的冰属性抗性。");
						stringBuilder28.AppendLine(ref handler);
					}
					break;
				case 'g':
					if (command == "light resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder30 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的光属性抗性。");
						stringBuilder30.AppendLine(ref handler);
					}
					break;
				case 'i':
					if (command == "blind resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder33 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内队员的失明抗性增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("点。");
						stringBuilder33.AppendLine(ref handler);
					}
					break;
				case 'e':
					if (command == "sleep resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder31 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内队员的睡眠抗性增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("点。");
						stringBuilder31.AppendLine(ref handler);
					}
					break;
				case 'r':
					if (command == "curse resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder29 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内队员的诅咒抗性增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("点。");
						stringBuilder29.AppendLine(ref handler);
					}
					break;
				case 'o':
					if (command == "stone resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder27 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内队员的石化抗性增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("点。");
						stringBuilder27.AppendLine(ref handler);
					}
					break;
				}
				break;
			case 15:
				switch (command[3])
				{
				case 's':
					if (command == "physical attack")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder24 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的力量。");
						stringBuilder24.AppendLine(ref handler);
					}
					break;
				case 'i':
					if (command == "magical defense")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder20 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的精神。");
						stringBuilder20.AppendLine(ref handler);
					}
					break;
				case 'e':
					if (command == "fire resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder22 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的火属性抗性。");
						stringBuilder22.AppendLine(ref handler);
					}
					break;
				case 'k':
					if (command == "dark resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder25 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的暗属性抗性。");
						stringBuilder25.AppendLine(ref handler);
					}
					break;
				case 'n':
					if (command == "stun resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder23 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内队员的眩晕抗性增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("点。");
						stringBuilder23.AppendLine(ref handler);
					}
					break;
				case 'd':
					if (command == "hold resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder21 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内队员的束缚抗性增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("点。");
						stringBuilder21.AppendLine(ref handler);
					}
					break;
				case 'w':
					if (command == "slow resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder19 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内队员的缓速抗性增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("点。");
						stringBuilder19.AppendLine(ref handler);
					}
					break;
				}
				break;
			case 25:
				switch (command[10])
				{
				case 'p':
					if (command == "equipment physical attack")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder17 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的物理攻击力。");
						stringBuilder17.AppendLine(ref handler);
					}
					break;
				case 'm':
					if (command == "equipment magical defense")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder16 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的魔法防御力。");
						stringBuilder16.AppendLine(ref handler);
					}
					break;
				}
				break;
			case 24:
				switch (command[0])
				{
				case 'e':
					if (command == "equipment magical attack")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder14 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的魔法攻击力。");
						stringBuilder14.AppendLine(ref handler);
					}
					break;
				case 'a':
					if (command == "all elemental resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder13 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(27, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的所有属性抗性。");
						stringBuilder13.AppendLine(ref handler);
					}
					break;
				}
				break;
			case 10:
				switch (command[0])
				{
				case 'j':
					if (command == "jump power")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder10 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(24, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的跳跃力。");
						stringBuilder10.AppendLine(ref handler);
					}
					break;
				case 'm':
					if (command == "move speed")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder11 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(25, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的移动速度。");
						stringBuilder11.AppendLine(ref handler);
					}
					break;
				case 'c':
					if (command == "cast speed")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder9 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(25, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的施放速度。");
						stringBuilder9.AppendLine(ref handler);
					}
					break;
				}
				break;
			case 17:
				switch (command[0])
				{
				case 'f':
					if (command == "freeze resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder7 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内队员的冰冻抗性增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("点。");
						stringBuilder7.AppendLine(ref handler);
					}
					break;
				case 'p':
					if (command == "poison resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder6 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内队员的中毒抗性增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("点。");
						stringBuilder6.AppendLine(ref handler);
					}
					break;
				}
				break;
			case 19:
				switch (command[0])
				{
				case 'b':
					if (command == "bleeding resistance")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder4 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
						handler.AppendLiteral("  装备时，使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内队员的出血抗性增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("点。");
						stringBuilder4.AppendLine(ref handler);
					}
					break;
				case 'm':
					if (command == "magicalcritical hit")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder3 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(27, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("%的魔法暴击率。");
						stringBuilder3.AppendLine(ref handler);
					}
					break;
				}
				break;
			case 6:
				switch (command[0])
				{
				case 'h':
					if (command == "hp max")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder40 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(22, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的HP");
						stringBuilder40.AppendLine(ref handler);
					}
					break;
				case 'm':
					if (command == "mp max")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder39 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(22, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的MP");
						stringBuilder39.AppendLine(ref handler);
					}
					break;
				}
				break;
			case 12:
				switch (command[0])
				{
				case 'a':
					if (command == "attack speed")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder37 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(25, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的攻击速度。");
						stringBuilder37.AppendLine(ref handler);
					}
					break;
				case 'w':
					if (command == "water attack")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder38 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的冰属性强化。");
						stringBuilder38.AppendLine(ref handler);
					}
					break;
				case 'l':
					if (command == "light attack")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder36 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的光属性强化。");
						stringBuilder36.AppendLine(ref handler);
					}
					break;
				}
				break;
			case 11:
				switch (command[0])
				{
				case 'f':
					if (command == "fire attack")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder35 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的火属性强化。");
						stringBuilder35.AppendLine(ref handler);
					}
					break;
				case 'd':
					if (command == "dark attack")
					{
						StringBuilder stringBuilder = bui;
						StringBuilder stringBuilder34 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
						handler.AppendLiteral("  装备时，可以使");
						handler.AppendFormatted(item.Value2);
						handler.AppendLiteral("px范围内的队员增加");
						handler.AppendFormatted(item.Value1);
						handler.AppendLiteral("的暗属性强化。");
						stringBuilder34.AppendLine(ref handler);
					}
					break;
				}
				break;
			case 14:
				if (command == "magical attack")
				{
					StringBuilder stringBuilder = bui;
					StringBuilder stringBuilder26 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
					handler.AppendLiteral("  装备时，可以使");
					handler.AppendFormatted(item.Value2);
					handler.AppendLiteral("px范围内的队员增加");
					handler.AppendFormatted(item.Value1);
					handler.AppendLiteral("的智力。");
					stringBuilder26.AppendLine(ref handler);
				}
				break;
			case 26:
				if (command == "equipment physical defense")
				{
					StringBuilder stringBuilder = bui;
					StringBuilder stringBuilder18 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
					handler.AppendLiteral("  装备时，可以使");
					handler.AppendFormatted(item.Value2);
					handler.AppendLiteral("px范围内的队员增加");
					handler.AppendFormatted(item.Value1);
					handler.AppendLiteral("的物理防御力。");
					stringBuilder18.AppendLine(ref handler);
				}
				break;
			case 27:
				if (command == "all activestatus resistance")
				{
					StringBuilder stringBuilder = bui;
					StringBuilder stringBuilder15 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(30, 2, stringBuilder);
					handler.AppendLiteral("  装备时，可以使");
					handler.AppendFormatted(item.Value2);
					handler.AppendLiteral("px范围内的队员增加");
					handler.AppendFormatted(item.Value1);
					handler.AppendLiteral("的所有异常状态的抗性。");
					stringBuilder15.AppendLine(ref handler);
				}
				break;
			case 18:
				if (command == "confuse resistance")
				{
					StringBuilder stringBuilder = bui;
					StringBuilder stringBuilder12 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
					handler.AppendLiteral("  装备时，使");
					handler.AppendFormatted(item.Value2);
					handler.AppendLiteral("px范围内队员的混乱抗性增加");
					handler.AppendFormatted(item.Value1);
					handler.AppendLiteral("点。");
					stringBuilder12.AppendLine(ref handler);
				}
				break;
			case 20:
				if (command == "lightning resistance")
				{
					StringBuilder stringBuilder = bui;
					StringBuilder stringBuilder8 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(23, 2, stringBuilder);
					handler.AppendLiteral("  装备时，使");
					handler.AppendFormatted(item.Value2);
					handler.AppendLiteral("px范围内队员的感电抗性增加");
					handler.AppendFormatted(item.Value1);
					handler.AppendLiteral("点。");
					stringBuilder8.AppendLine(ref handler);
				}
				break;
			case 21:
				if (command == "physical critical hit")
				{
					StringBuilder stringBuilder = bui;
					StringBuilder stringBuilder5 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(27, 2, stringBuilder);
					handler.AppendLiteral("  装备时，可以使");
					handler.AppendFormatted(item.Value2);
					handler.AppendLiteral("px范围内的队员增加");
					handler.AppendFormatted(item.Value1);
					handler.AppendLiteral("%的物理暴击率。");
					stringBuilder5.AppendLine(ref handler);
				}
				break;
			case 13:
				if (command == "hp regen rate")
				{
					StringBuilder stringBuilder = bui;
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(26, 2, stringBuilder);
					handler.AppendLiteral("  装备时，可以使");
					handler.AppendFormatted(item.Value2);
					handler.AppendLiteral("px范围内的队员每分钟恢复");
					handler.AppendFormatted(item.Value1);
					handler.AppendLiteral("的HP。");
					stringBuilder2.AppendLine(ref handler);
				}
				break;
			}
		}
		return bui.Length > 0;
	}

	public bool GetSectionValueArray(SectionBase? sectionB, out List<string> arr)
	{
		arr = new List<string>();
		if (sectionB == null)
		{
			return false;
		}
		PvfSection pvfSection = (PvfSection)sectionB;
		if (pvfSection.Children == null || pvfSection.Children.Count == 0)
		{
			return false;
		}
		int num = pvfSection.Children.Count;
		if (pvfSection.HasEndSection())
		{
			num--;
		}
		List<SectionBase> children = pvfSection.Children;
		for (int i = 1; i < num; i++)
		{
			SectionBase sectionBase = children[i];
			if (sectionBase != null)
			{
				ScriptItem scriptItem = null;
				if (sectionBase.Item.Type == ScriptType.StringLinkIndex)
				{
					scriptItem = children[i + 1].Item;
				}
				arr.Add(sectionBase.Item.GetItemTextNotChar(pvfGroup, scriptItem));
				if (scriptItem != null)
				{
					i++;
				}
			}
		}
		return arr.Count > 0;
	}

	public bool GetSectionIntArray(SectionBase? sectionB, out List<int> arr)
	{
		arr = new List<int>();
		if (sectionB == null)
		{
			return false;
		}
		PvfSection pvfSection = (PvfSection)sectionB;
		if (pvfSection.Children == null || pvfSection.Children.Count == 0)
		{
			return false;
		}
		int num = pvfSection.Children.Count;
		if (pvfSection.HasEndSection())
		{
			num--;
		}
		List<SectionBase> children = pvfSection.Children;
		for (int i = 1; i < num; i++)
		{
			SectionBase sectionBase = children[i];
			if (sectionBase != null)
			{
				if (sectionBase.Item.Type != ScriptType.Int)
				{
					return false;
				}
				arr.Add(sectionBase.Item.Data);
			}
		}
		return arr.Count > 0;
	}

	public bool GetSectionStringArray(SectionBase? sectionB, out List<string> arr)
	{
		arr = new List<string>();
		if (sectionB == null)
		{
			return false;
		}
		PvfSection pvfSection = (PvfSection)sectionB;
		if (pvfSection.Children == null || pvfSection.Children.Count == 0)
		{
			return false;
		}
		int num = pvfSection.Children.Count;
		if (pvfSection.HasEndSection())
		{
			num--;
		}
		List<SectionBase> children = pvfSection.Children;
		for (int i = 1; i < num; i++)
		{
			SectionBase sectionBase = children[i];
			if (sectionBase != null)
			{
				if (sectionBase.Item.Type != ScriptType.String)
				{
					return false;
				}
				arr.Add(pvfGroup.Strtable.GetStringItem(sectionBase.Item.Data));
			}
		}
		return arr.Count > 0;
	}

	public void RemoveSection(string sectionName)
	{
		if (Sections == null)
		{
			return;
		}
		SectionBase section = Sections.FirstOrDefault(item => item.GetSectionName() == sectionName);
		if (section != null)
		{
			Sections.Remove(section);
		}
	}
}
