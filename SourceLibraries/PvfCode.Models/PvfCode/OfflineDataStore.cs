using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;

namespace PvfCode;

public static class OfflineDataStore
{
	private static readonly JsonSerializerSettings Settings = new()
	{
		Formatting = Formatting.Indented,
		NullValueHandling = NullValueHandling.Ignore,
		Converters = { new StringEnumConverter() }
	};

	public static string BookmarksPath => Path.Combine(AppSetting.AppBasePath, "Bookmarks.json");

	public static string DefaultsDirectory => Path.Combine(AppContext.BaseDirectory, "Defaults", "Options");

	public static void EnsureDefaults()
	{
		CopyDefaultIfMissing("AppConfig.json", Path.Combine(AppSetting.AppBasePath, "AppConfig.json"));
		CopyDefaultIfMissing("Bookmarks.json", BookmarksPath);
	}

	public static void Load(AppSetting setting)
	{
		setting.PvfConfig.TreelistCommentDic = NormalizeTreeComments(setting.PvfConfig.TreelistCommentDic);
		if (File.Exists(BookmarksPath))
		{
			setting.BookMarkGroup = JsonConvert.DeserializeObject<BookMarkGroupDto>(File.ReadAllText(BookmarksPath), Settings) ?? new BookMarkGroupDto();
		}
	}

	public static void Save(AppSetting setting)
	{
		setting.PvfConfig.TreelistCommentDic = NormalizeTreeComments(setting.PvfConfig.TreelistCommentDic);
		WriteJson(BookmarksPath, setting.BookMarkGroup);
	}

	private static Dictionary<string, TreelistCommentRes> NormalizeTreeComments(Dictionary<string, TreelistCommentRes> source)
	{
		Dictionary<string, TreelistCommentRes> result = new(StringComparer.OrdinalIgnoreCase);
		if (source == null)
		{
			return result;
		}
		foreach ((string rawPath, TreelistCommentRes entry) in source)
		{
			string path = NormalizePath(rawPath);
			if (string.IsNullOrEmpty(path) || entry == null)
			{
				continue;
			}
			entry.FilePath = path;
			result[path] = entry;
		}
		return result;
	}

	public static string NormalizePath(string? path)
	{
		return (path ?? string.Empty).Replace('\\', '/').Trim('/').ToLowerInvariant();
	}

	private static void WriteJson(string path, object value)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		string temporaryPath = path + ".tmp";
		File.WriteAllText(temporaryPath, JsonConvert.SerializeObject(value, Settings));
		File.Move(temporaryPath, path, true);
	}

	private static void CopyDefaultIfMissing(string relativePath, string targetPath)
	{
		if (File.Exists(targetPath))
		{
			return;
		}
		string sourcePath = Path.Combine(DefaultsDirectory, relativePath);
		if (!File.Exists(sourcePath))
		{
			return;
		}
		Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
		File.Copy(sourcePath, targetPath);
	}
}
