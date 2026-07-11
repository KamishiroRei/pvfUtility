using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PvfCode.Models.Options.Editor;

namespace PvfCode.Services.PvfParsingNew.CustomSectionFormat;

public abstract class CustomSectionFormatBase : ICloneable
{
	public bool AppNewLine { get; set; }

	public string? ParentSectionName { get; set; }

	public List<KeyValuePair<string, ValidationSectionData>>? ValidationSectionList { get; set; }

	public object Clone()
	{
		return MemberwiseClone();
	}

	public bool CheckParentSectionName(string sectionName)
	{
		if (string.IsNullOrEmpty(ParentSectionName))
		{
			return true;
		}
		return ParentSectionName == sectionName;
	}

	public bool ValidationSection(PvfFile file, PvfGroup pvf)
	{
		if (ValidationSectionList == null)
		{
			return true;
		}
		Dictionary<string, string> dictionary = new ScriptFileCompilerOl(pvf).DecompileDic(file);
		int num = 0;
		foreach (KeyValuePair<string, ValidationSectionData> validationSection in ValidationSectionList)
		{
			if (dictionary.TryGetValue(validationSection.Key, out var value))
			{
				if (validationSection.Value.Index.HasValue)
				{
					if (!string.IsNullOrEmpty(value))
					{
						string[] array = value.Split("\t", StringSplitOptions.RemoveEmptyEntries);
						if (array != null && array.Any() && validationSection.Value.Index.Value < array.Count() && array[validationSection.Value.Index.Value] == validationSection.Value.Value)
						{
							num++;
						}
					}
				}
				else if (validationSection.Value.Value == value.Replace("\t", string.Empty))
				{
					num++;
				}
				continue;
			}
			return false;
		}
		return ValidationSectionList.Count == num;
	}

	public abstract string ProcessSectionText(PvfGroup pvf, PvfFile file, SectionBase section, int layer, Dictionary<string, List<CustomSectionFormatBase>>? dic = null, string? ParentSectionName = null);

	public virtual bool Check()
	{
		return true;
	}

	public virtual void AddEndTab(StringBuilder sb, int count, int index)
	{
		if (count == index)
		{
			if (AppSetting.Instance.EditConfig.AddEndTab)
			{
				sb.Append("\t");
			}
		}
		else
		{
			sb.Append("\t");
		}
	}

	public virtual void AppEndTabNew(StringBuilder sb, int count, int index, int layer)
	{
		bool flag = count == index;
		if (AppNewLine || count == index)
		{
			if (AppSetting.Instance.EditConfig.AddEndTab)
			{
				sb.Append("\t");
			}
		}
		else
		{
			sb.Append("\t");
		}
		if (AppNewLine && !flag)
		{
			AppendNewLineAndIndent(sb, layer);
		}
		AppNewLine = false;
	}

	internal void AppendNewLineAndIndent(StringBuilder output, int indentationLevel)
	{
		output.Append(Environment.NewLine);
		for (int i = 0; i < indentationLevel; i++)
		{
			output.Append("\t");
		}
	}

	protected CustomSectionFormatBase()
	{
	}
}
