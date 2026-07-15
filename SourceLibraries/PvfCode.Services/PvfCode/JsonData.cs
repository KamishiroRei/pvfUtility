using System.Collections.Generic;
using System.Windows.Media;
using Collections.Pooled;
using Newtonsoft.Json;
using PvfCode.NPK.Utils.Models;

namespace PvfCode;

public class JsonData
{
	public IDictionary<string, UtImgFile> NpkImgDIC { get; private set; }

	public Dictionary<int, string> NpkFilePathDic { get; set; }

	public Dictionary<string, Dictionary<int, ImageSource>> TreeFileIconList { get; private set; }

	public List<ImageSource> QuestTypeIconList { get; set; }

	public PooledDictionary<string, ImageSource> EmoIconList { get; set; }

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
