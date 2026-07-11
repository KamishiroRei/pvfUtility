using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Collections.Pooled;
using Newtonsoft.Json;
using PvfCode.NPK.Utils.Models;

namespace PvfCode;

public class JsonData
{
	[CompilerGenerated]
	private IDictionary<string, UtImgFile> tF7mi1VIP;

	[CompilerGenerated]
	private Dictionary<int, string> rET4rpeMQ;

	[CompilerGenerated]
	private Dictionary<string, Dictionary<int, ImageSource>> my6QAsHa7;

	[CompilerGenerated]
	private List<ImageSource> CcyNWf4P4;

	[CompilerGenerated]
	private PooledDictionary<string, ImageSource> D2kJuRgWh;

	public IDictionary<string, UtImgFile> NpkImgDIC
	{
		[CompilerGenerated]
		get
		{
			return tF7mi1VIP;
		}
		[CompilerGenerated]
		private set
		{
			tF7mi1VIP = value;
		}
	}

	public Dictionary<int, string> NpkFilePathDic
	{
		[CompilerGenerated]
		get
		{
			return rET4rpeMQ;
		}
		[CompilerGenerated]
		set
		{
			rET4rpeMQ = value;
		}
	}

	public Dictionary<string, Dictionary<int, ImageSource>> TreeFileIconList
	{
		[CompilerGenerated]
		get
		{
			return my6QAsHa7;
		}
		[CompilerGenerated]
		private set
		{
			my6QAsHa7 = value;
		}
	}

	public List<ImageSource> QuestTypeIconList
	{
		[CompilerGenerated]
		get
		{
			return CcyNWf4P4;
		}
		[CompilerGenerated]
		set
		{
			CcyNWf4P4 = value;
		}
	}

	public PooledDictionary<string, ImageSource> EmoIconList
	{
		[CompilerGenerated]
		get
		{
			return D2kJuRgWh;
		}
		[CompilerGenerated]
		set
		{
			D2kJuRgWh = value;
		}
	}

	public string ToJson()
	{
		return JsonConvert.SerializeObject(this);
	}

	public static JsonData De(string json)
	{
		return JsonConvert.DeserializeObject<JsonData>(json);
	}

	public JsonData()
	{
	}
}
