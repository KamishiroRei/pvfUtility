using System;
using System.Collections.Generic;
using System.IO;
using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class ImagePacks2Options : ViewModelBase
{
	private string _imagePacks2Path = string.Empty;

	public string ImagePacks2Path
	{
		get => _imagePacks2Path;
		set
		{
			_imagePacks2Path = value;
			RaisePropertyChanged(nameof(ImagePacks2Path));
		}
	}

	/// <summary>每 PVF 记忆的 ImagePacks2 目录（键=PVF 完整路径，随 AppConfig.json 持久化）；
	/// 打开对应 PVF 时自动应用，避免多客户端共用时手动切换。</summary>
	public Dictionary<string, string> PvfImagePacks2Map { get; set; } =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

	/// <summary>取某 PVF 记忆的 ImagePacks2 目录；未记录返回 null。</summary>
	public string GetImagePacks2ForPvf(string pvfPath)
	{
		if (string.IsNullOrEmpty(pvfPath) || PvfImagePacks2Map.Count == 0)
		{
			return null;
		}
		return PvfImagePacks2Map.TryGetValue(NormalizeKey(pvfPath), out string value) ? value : null;
	}

	/// <summary>记录某 PVF 当前使用的 ImagePacks2 目录。</summary>
	public void SetImagePacks2ForPvf(string pvfPath, string imagePacks2Path)
	{
		if (string.IsNullOrEmpty(pvfPath))
		{
			return;
		}
		string key = NormalizeKey(pvfPath);
		if (string.IsNullOrEmpty(imagePacks2Path))
		{
			PvfImagePacks2Map.Remove(key);
		}
		else
		{
			PvfImagePacks2Map[key] = imagePacks2Path;
		}
		RaisePropertyChanged(nameof(PvfImagePacks2Map));
	}

	private static string NormalizeKey(string path)
		=> Path.GetFullPath(path).TrimEnd('\\', '/');

	[JsonIgnore]
	public bool IsLoaded
	{
		get
		{
			return GetProperty(() => IsLoaded);
		}
		set
		{
			SetProperty(() => IsLoaded, value);
		}
	}

	public int ImgCount
	{
		get
		{
			return GetProperty(() => ImgCount);
		}
		set
		{
			SetProperty(() => ImgCount, value, HKoLzD3Hw4);
		}
	}

	private void HKoLzD3Hw4()
	{
		_ = ImgCount;
	}

	public ImagePacks2Options()
	{
	}
}
