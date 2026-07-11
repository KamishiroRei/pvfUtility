using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.BatchOperation;
using Utools;

namespace PvfCode.Models.Macro;

public class MacroData
{
	private ObservableConcurrentDictionaryEx<string, MacroData> j7wZmcIp8B;

	[CompilerGenerated]
	private bool VoUZSB7aGF;

	[CompilerGenerated]
	private int Rq3ZBvWLyw;

	[CompilerGenerated]
	private object jueZ4y5LVp;

	[CompilerGenerated]
	private bool zRYZCkS4J4;

	[CompilerGenerated]
	private MacroType aNkZvpFwmq;

	public ObservableConcurrentDictionaryEx<string, MacroData> Children
	{
		get
		{
			if (j7wZmcIp8B == null)
			{
				j7wZmcIp8B = new ObservableConcurrentDictionaryEx<string, MacroData>();
			}
			return j7wZmcIp8B;
		}
		set
		{
			j7wZmcIp8B = value;
		}
	}

	public bool IsFile
	{
		[CompilerGenerated]
		get
		{
			return VoUZSB7aGF;
		}
		[CompilerGenerated]
		set
		{
			VoUZSB7aGF = value;
		}
	}

	public int Sort
	{
		[CompilerGenerated]
		get
		{
			return Rq3ZBvWLyw;
		}
		[CompilerGenerated]
		set
		{
			Rq3ZBvWLyw = value;
		}
	}

	public object Data
	{
		[CompilerGenerated]
		get
		{
			return jueZ4y5LVp;
		}
		[CompilerGenerated]
		set
		{
			jueZ4y5LVp = value;
		}
	}

	public bool IsRoot
	{
		[CompilerGenerated]
		get
		{
			return zRYZCkS4J4;
		}
		[CompilerGenerated]
		set
		{
			zRYZCkS4J4 = value;
		}
	}

	public MacroType MacroType
	{
		[CompilerGenerated]
		get
		{
			return aNkZvpFwmq;
		}
		[CompilerGenerated]
		set
		{
			aNkZvpFwmq = value;
		}
	}

	public MacroData()
	{
	}

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
		if (j7wZmcIp8B != null)
		{
			return j7wZmcIp8B.Any();
		}
		return false;
	}

	public List<BatchOperationConfig> GetBatchOperationMacro()
	{
		return GetData<BatchOperationConfig>();
	}
}
