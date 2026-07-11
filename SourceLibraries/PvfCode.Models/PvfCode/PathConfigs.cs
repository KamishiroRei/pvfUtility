using System;
using System.Windows.Input;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using Swordfish.NET.Collections;

namespace PvfCode;

[JsonObject(MemberSerialization.OptOut)]
public class PathConfigs : ModelBase
{
	private ConcurrentObservableDictionary<string, DateTime> MDWWBaWHe;

	private ICommand CZJqUs9sk;

	public ConcurrentObservableDictionary<string, DateTime> PvfOpenLog
	{
		get
		{
			if (MDWWBaWHe == null)
			{
				MDWWBaWHe = new ConcurrentObservableDictionary<string, DateTime>();
			}
			return MDWWBaWHe;
		}
		set
		{
			MDWWBaWHe = value;
			DoNotify("PvfOpenLog");
		}
	}

	[JsonIgnore]
	public ICommand ClearPvfOpenLogcCommand
	{
		get
		{
			if (CZJqUs9sk == null)
			{
				CZJqUs9sk = new DelegateCommand(ClearPvfOpenLog);
			}
			return CZJqUs9sk;
		}
	}

	public void AddPvfOpenLog(string filePath)
	{
		if (PvfOpenLog.ContainsKey(filePath))
		{
			PvfOpenLog.Remove(filePath);
		}
		PvfOpenLog.Add(filePath, DateTime.Now);
		if (PvfOpenLog.Count > 20)
		{
			PvfOpenLog.Remove(PvfOpenLog.Keys[0]);
		}
	}

	public void ClearPvfOpenLog()
	{
		PvfOpenLog = null;
	}

	public PathConfigs()
	{
	}
}
