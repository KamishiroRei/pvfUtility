using System;
using System.Collections.Generic;
using System.Linq;
using PvfCode.Dot.Desktop.Enums;
using Utools;

namespace PvfCode.Models.Macro;

public class MacroGroup : ModelBase
{
	private ObservableConcurrentDictionaryEx<string, MacroData> trees;

	public ObservableConcurrentDictionaryEx<string, MacroData> Trees
	{
		get
		{
			if (trees == null)
			{
				trees = new ObservableConcurrentDictionaryEx<string, MacroData>();
			}
			return trees;
		}
		set
		{
			trees = value;
			DoNotify("Trees");
			foreach (object value2 in Enum.GetValues(typeof(MacroType)))
			{
				DoNotify(value2.ToString());
			}
		}
	}

	public ObservableConcurrentDictionaryEx<string, MacroData> 全局搜索
	{
		get
		{
			if (!Trees.TryGetValue(MacroType.全局搜索.ToString(), out MacroData value))
			{
				return null;
			}
			return value.Children;
		}
	}

	public ObservableConcurrentDictionaryEx<string, MacroData> 批量处理
	{
		get
		{
			if (!Trees.TryGetValue(MacroType.批量处理.ToString(), out MacroData value))
			{
				return null;
			}
			return value.Children;
		}
	}

	public void Init()
	{
		foreach (object value in Enum.GetValues(typeof(MacroType)))
		{
			MacroType macroType = (MacroType)Enum.Parse(typeof(MacroType), value.ToString());
			if (!Trees.ContainsKey(macroType.ToString()))
			{
				switch (macroType)
				{
				case MacroType.全局搜索:
					Trees.Add(value.ToString(), new MacroData
					{
						IsRoot = true
					});
					break;
				case MacroType.批量处理:
					Trees.Add(value.ToString(), new MacroData
					{
						IsRoot = true
					});
					break;
				}
			}
		}
	}

	public void DoNotifyTrees()
	{
		DoNotify("Trees");
		foreach (object value in Enum.GetValues(typeof(MacroType)))
		{
			DoNotify(value.ToString());
		}
	}

	public string CheckTitle(IDictionary<string, MacroData> source, string title)
	{
		string text = title;
		int num = 0;
		while (source.ContainsKey(text))
		{
			text = $"{title}({num})";
			num++;
		}
		return text;
	}

	public KeyValuePair<string, MacroData> AddNode(string title, MacroData bookMarkData, ObservableConcurrentDictionaryEx<string, MacroData>? source)
	{
		string uniqueTitle = CheckTitle(source, title);
		source.AddTry(uniqueTitle, bookMarkData);
		return source.First(item => item.Key == uniqueTitle);
	}

	public ObservableConcurrentDictionaryEx<string, MacroData> GetMacros(MacroType type)
	{
		return Trees[type.ToString()].Children;
	}

}
