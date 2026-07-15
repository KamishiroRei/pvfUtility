using System.Collections.Generic;
using System.Linq;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.BatchOperation;
using Utools;

namespace PvfCode.Models.Macro;

public class MacroData
{
	private ObservableConcurrentDictionaryEx<string, MacroData> children;

	public ObservableConcurrentDictionaryEx<string, MacroData> Children
	{
		get
		{
			if (children == null)
			{
				children = new ObservableConcurrentDictionaryEx<string, MacroData>();
			}
			return children;
		}
		set
		{
			children = value;
		}
	}

	public bool IsFile { get; set; }

	public int Sort { get; set; }

	public object Data { get; set; }

	public bool IsRoot { get; set; }

	public MacroType MacroType { get; set; }

	public void SetData(object obj)
	{
		Data = obj.ToJson();
	}

	public List<T> GetData<T>()
	{
		return Data.ToString().JsonToObject<List<T>>();
	}

	public bool HaveChildren()
	{
		if (children != null)
		{
			return children.Any();
		}
		return false;
	}

	public List<BatchOperationConfig> GetBatchOperationMacro()
	{
		return GetData<BatchOperationConfig>();
	}
}
