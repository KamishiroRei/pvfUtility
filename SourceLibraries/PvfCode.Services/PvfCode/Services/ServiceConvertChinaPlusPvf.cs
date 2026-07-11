using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Services.PvfParsingNew;
using PvfCode.Services.SearchModel;

namespace PvfCode.Services;

public class ServiceConvertChinaPlusPvf
{
	private readonly PvfGroup pvf;

	private readonly Ilogger logger;

	private readonly StringBuilder conversionLog;

	private readonly bool convertEquipmentPartSet;

	private readonly bool convertAddSections;

	private readonly Dictionary<string, int> addSectionValueCounts;

	public ServiceConvertChinaPlusPvf(PvfGroup pvf, bool convertEquipmentPartSet, bool convertAddSections)
	{
		conversionLog = new StringBuilder();
		addSectionValueCounts = new Dictionary<string, int>();
		this.convertEquipmentPartSet = convertEquipmentPartSet;
		this.convertAddSections = convertAddSections;
		logger = AppSetting.Instance.GetIlogger();
		this.pvf = pvf;
		InitializeAddSectionValueCounts();
	}

	public async Task Start()
	{
		if (convertEquipmentPartSet)
		{
			await ConvertEquipmentPartSet();
		}
		if (convertAddSections)
		{
			await ConvertAddSections();
		}
		logger.Success("转换完毕：" + conversionLog);
	}

	private Task ConvertEquipmentPartSet()
	{
		PvfFile file = pvf.GetFile("etc/equipmentpartset.etc");
		if (file == null)
		{
			return Task.CompletedTask;
		}
		ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(file, pvf);
		scriptFileParserNew.PraseStructureMain();
		if (scriptFileParserNew.Sections == null)
		{
			return Task.CompletedTask;
		}
		int num = 0;
		foreach (SectionBase section in scriptFileParserNew.Sections)
		{
			if (!(section.GetSectionName() == "[equipment part set]"))
			{
				continue;
			}
			num++;
			List<SectionBase> children = section.Children;
			if (children.Count < 5)
			{
				conversionLog.AppendLine($"ChinaPvf套装文件转换失败：[equipment part set]下的数值数量不正确 index:{num} file://etc/equipmentpartset.etc");
				continue;
			}
			if (children[2].Item.Type != ScriptType.String && children[2].Item.Type != ScriptType.StringLinkIndex)
			{
				conversionLog.AppendLine($"ChinaPvf套装文件转换失败：[equipment part set] index：{num}  下第2个值应为：string 此处为：{children[2].Item.Type} file://etc/equipmentpartset.etc");
				continue;
			}
			if (children[3].Item.Type != ScriptType.StringLinkIndex && children[3].Item.Type != ScriptType.String)
			{
				conversionLog.AppendLine($"ChinaPvf套装文件转换失败：[equipment part set]下第3个值应为：StringLinkIndex index:{num}  此处为：{children[2].Item.Type} file://etc/equipmentpartset.etc");
				continue;
			}
			ScriptItem nextItem = ((children[2].Item.Type == ScriptType.StringLinkIndex) ? children[3].Item : null);
			string itemText = children[2].Item.GetItemText(pvf, nextItem);
			nextItem = ((children[3].Item.Type == ScriptType.StringLinkIndex) ? children[4].Item : null);
			string itemText2 = children[3].Item.GetItemText(pvf, nextItem);
			if (string.IsNullOrEmpty(itemText))
			{
				conversionLog.AppendLine($"ChinaPvf套装文件转换失败： index:{num} [equipment part set]下的文件路径为空 file://etc/equipmentpartset.etc");
				continue;
			}
			if (string.IsNullOrEmpty(itemText2))
			{
				conversionLog.AppendLine($"ChinaPvf套装文件转换失败：index:{num} [equipment part set]下的[set name] 名称为空 file://etc/equipmentpartset.etc");
				continue;
			}
			if (children[3].Item.Type == ScriptType.StringLinkIndex)
			{
				children.RemoveRange(2, 2);
			}
			else
			{
				children.RemoveRange(3, 1);
			}
			itemText = "equipment/" + itemText.Replace("`", string.Empty).ToLower();
			PvfFile file2 = pvf.GetFile(itemText);
			if (file2 == null)
			{
				conversionLog.AppendLine($"ChinaPvf套装文件转换失败： index{num} [equipment part set]下的套装文件路径指向文件不存在：{itemText} 名称为空 file://etc/equipmentpartset.etc");
				continue;
			}
			ScriptFileParserNew scriptFileParserNew2 = new ScriptFileParserNew(file2, pvf);
			scriptFileParserNew2.PraseStructureMain();
			if (scriptFileParserNew2.Sections == null)
			{
				scriptFileParserNew2.Sections = new List<SectionBase>();
			}
			List<SectionBase> list = scriptFileParserNew2.Sections.FindAll((SectionBase it) => it.GetSectionName() == "[set name]");
			if (list != null)
			{
				foreach (SectionBase item in list)
				{
					scriptFileParserNew2.Sections.Remove(item);
				}
			}
			string fileText = "#PVF_File\r\n\r\n[set name]\r\n" + itemText2 + "\r\n" + scriptFileParserNew2.GetText().Replace("#PVF_File", "");
			pvf.SaveFileText(file2, fileText);
		}
		string text = scriptFileParserNew.GetText();
		pvf.SaveFileText(file, text);
		return Task.CompletedTask;
	}

	private async Task ConvertAddSections()
	{
		foreach (KeyValuePair<string, int> item in addSectionValueCounts)
		{
			await ConvertAddSection(item);
		}
		await RemoveSectionFromEquipmentFiles("[add value]");
	}

	private void InitializeAddSectionValueCounts()
	{
		addSectionValueCounts.Add("[add physical attack]", 1);
		addSectionValueCounts.Add("[add magical attack]", 1);
		addSectionValueCounts.Add("[add physical defense]", 1);
		addSectionValueCounts.Add("[add magical defense]", 1);
		addSectionValueCounts.Add("[add attack speed]", 1);
		addSectionValueCounts.Add("[add cast speed]", 1);
		addSectionValueCounts.Add("[add move speed]", 1);
		addSectionValueCounts.Add("[add physical critical hit]", 1);
		addSectionValueCounts.Add("[add magical critical hit]", 1);
		addSectionValueCounts.Add("[add stuck resistance]", 1);
		addSectionValueCounts.Add("[add anti evil]", 1);
		addSectionValueCounts.Add("[add price]", 1);
		addSectionValueCounts.Add("[add stuck]", 1);
		addSectionValueCounts.Add("[add equipment physical attack]", 2);
		addSectionValueCounts.Add("[add equipment magical attack]", 2);
		addSectionValueCounts.Add("[add equipment physical defense]", 2);
		addSectionValueCounts.Add("[add equipment magical defense]", 2);
		addSectionValueCounts.Add("[add separate attack]", 2);
	}

	private async Task ConvertAddSection(KeyValuePair<string, int> source)
	{
		string taiWanSectionName = source.Key.Remove(1, 4);
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = source.Key,
			SearchFolder = "equipment"
		}, pvf, allowLog: false).Search();
		if (resultData == null || resultData.Data == null || resultData.Data.Count <= 0)
		{
			return;
		}
		foreach (PvfFile file in pvf.GetFiles(resultData.Data))
		{
			try
			{
				ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(file, pvf);
				scriptFileParserNew.PraseStructureMain();
				if (scriptFileParserNew.Sections.Any((SectionBase it) => it.GetSectionName() == "[piece set ability]"))
				{
					List<SectionBase> list = scriptFileParserNew.Sections.FindAll((SectionBase it) => it.GetSectionName() == "[piece set ability]");
					if (list != null)
					{
						bool flag = false;
						foreach (SectionBase item in list)
						{
							if (ConvertNestedAddSections(file, item.Children, source, taiWanSectionName))
							{
								flag = true;
							}
						}
						if (flag)
						{
							pvf.SaveFileText(file, scriptFileParserNew.GetText());
						}
					}
				}
				if (MergeAddSection(file, scriptFileParserNew.Sections, source, taiWanSectionName))
				{
					pvf.SaveFileText(file, scriptFileParserNew.GetText());
				}
			}
			catch (Exception ex)
			{
				conversionLog.AppendLine($"转换Add标签失败： file://{file.FileName} error:{ex.Message}");
			}
		}
	}

	private bool ConvertNestedAddSections(PvfFile file, List<SectionBase> sections, KeyValuePair<string, int> source, string targetSectionName)
	{
		bool converted = false;
		foreach (SectionBase item in sections)
		{
			if (!(item is PvfSection) || item.Children == null || item.Children.Count <= 0)
			{
				continue;
			}
			bool itemConverted = MergeAddSection(file, item.Children, source, targetSectionName);
			if (!converted && itemConverted)
			{
				converted = true;
			}
			if (item.HasChildrenSection())
			{
				itemConverted = ConvertNestedAddSections(file, item.Children, source, targetSectionName);
				if (!converted && itemConverted)
				{
					converted = true;
				}
			}
		}
		return converted;
	}

	private bool MergeAddSection(PvfFile file, List<SectionBase> sections, KeyValuePair<string, int> source, string targetSectionName)
	{
		SectionBase sourceSection = sections.Find(it => it.GetSectionName() == source.Key);
		if (sourceSection == null)
		{
			return false;
		}

		SectionBase targetSection = sections.Find(it => it.GetSectionName() == targetSectionName);
		if (targetSection == null)
		{
			sourceSection.SetSectionName(targetSectionName);
		}
		else
		{
			if (targetSection.GetValueCount() != source.Value)
			{
				conversionLog.AppendLine($"转换失败：{targetSection.GetSectionName()}标签下的值应为：{source.Value}个参数 file://{file.FileName}");
				return false;
			}
			if (sourceSection.GetValueCount() != source.Value)
			{
				conversionLog.AppendLine($"转换失败：{sourceSection.GetSectionName()}标签下的值应为{source.Value}个参数 file://{file.FileName}");
				return false;
			}

			List<SectionBase> targetValues = targetSection.Children;
			List<SectionBase> sourceValues = sourceSection.Children;
			for (int index = 1; index < targetValues.Count; index++)
			{
				SectionBase targetValue = targetValues[index];
				SectionBase sourceValue = sourceValues[index];
				if (targetValue.Item.Type != ScriptType.Int && targetValue.Item.Type != ScriptType.Float)
				{
					conversionLog.AppendLine($"转换失败：{targetSection.GetSectionName()}标签下的值不为：int Index{index} file://{file.FileName}");
				}
				else if (sourceValue.Item.Type != ScriptType.Int && sourceValue.Item.Type != ScriptType.Float)
				{
					conversionLog.AppendLine($"转换失败：{sourceSection.GetSectionName()}标签下的值不为：int Index{index} file://{file.FileName}");
				}
				else
				{
					int targetNumber = double.TryParse(targetValue.Item.GetItemText(pvf, targetValue.Item), out double targetResult) ? (int)targetResult : targetValue.Item.Data;
					int sourceNumber = double.TryParse(sourceValue.Item.GetItemText(pvf, sourceValue.Item), out double sourceResult) ? (int)sourceResult : sourceValue.Item.Data;
					targetValue.Item.Data = targetNumber + sourceNumber;
				}
			}
			sections.Remove(sourceSection);
		}
		return true;
	}

	private async Task RemoveSectionFromEquipmentFiles(string sectionName)
	{
		ResultData<HashSet<string>> resultData = await new SearchService(new SearchConfig
		{
			Keyword = sectionName,
			SearchFolder = "equipment"
		}, pvf, allowLog: false).Search();
		if (resultData == null || resultData.Data == null || resultData.Data.Count <= 0)
		{
			return;
		}
		foreach (PvfFile file in pvf.GetFiles(resultData.Data))
		{
			ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(file, pvf);
			scriptFileParserNew.PraseStructureMain();
			List<SectionBase> list = scriptFileParserNew.Sections.FindAll(it => it.GetSectionName() == sectionName);
			if (list == null)
			{
				continue;
			}
			foreach (SectionBase item in list)
			{
				scriptFileParserNew.Sections.Remove(item);
			}
			pvf.SaveFileText(file, scriptFileParserNew.GetText());
		}
	}
}
