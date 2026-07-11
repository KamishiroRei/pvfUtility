using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using PvfCode.Dot.Desktop.Enums;

namespace PvfCode;

public class ServicePvfTabComment
{
	private sealed class CommentFile
	{
		public int SchemaVersion { get; set; } = 1;

		public string FileType { get; set; }

		public List<PvfCommentDto> Comments { get; set; } = new();
	}

	private static readonly SemaphoreSlim FileLock = new(1, 1);
	private static readonly JsonSerializerSettings JsonSettings = new()
	{
		Formatting = Formatting.Indented,
		NullValueHandling = NullValueHandling.Ignore,
		Converters = { new StringEnumConverter() }
	};
	private static ServicePvfTabComment instance;

	public static string CommentsDirectory => Path.Combine(AppSetting.AppBasePath, "PvfComments");

	public static ServicePvfTabComment Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ServicePvfTabComment();
				instance.CreateTab();
			}
			return instance;
		}
	}

	public void CreateTab()
	{
		try
		{
			Directory.CreateDirectory(CommentsDirectory);
			bool hasUserComments = Directory.EnumerateFiles(CommentsDirectory, "*.json").Any();
			if (!hasUserComments)
			{
				CopyBundledDefaults();
			}
		}
		catch (Exception ex)
		{
			AppSetting.Instance.GetIlogger()?.Error("旧版 PVF 注释迁移失败：" + ex.Message);
		}
	}

	private static void CopyBundledDefaults()
	{
		string sourceDirectory = Path.Combine(AppContext.BaseDirectory, "Defaults", "Options", "PvfComments");
		if (!Directory.Exists(sourceDirectory))
		{
			return;
		}
		foreach (string sourcePath in Directory.EnumerateFiles(sourceDirectory, "*.json"))
		{
			string targetPath = Path.Combine(CommentsDirectory, Path.GetFileName(sourcePath));
			if (!File.Exists(targetPath))
			{
				File.Copy(sourcePath, targetPath);
			}
		}
	}

	public bool TabAny()
	{
		return Directory.Exists(CommentsDirectory) && Directory.EnumerateFiles(CommentsDirectory, "*.json").Any();
	}

	public async Task AddRagned(List<PvfCommentDto> list)
	{
		await FileLock.WaitAsync();
		try
		{
			List<PvfCommentDto> current = LoadAll();
			foreach (PvfCommentDto item in list)
			{
				Upsert(current, item);
			}
			SaveAll(current);
		}
		finally
		{
			FileLock.Release();
		}
	}

	public async Task<int> Count()
	{
		ResultData<List<PvfCommentDto>> result = await GetList();
		return result.Data?.Count ?? 0;
	}

	public async Task<ResultData<List<PvfCommentDto>>> GetList()
	{
		await FileLock.WaitAsync();
		try
		{
			return new ResultData<List<PvfCommentDto>> { Data = LoadAll() };
		}
		catch (Exception ex)
		{
			return new ResultData<List<PvfCommentDto>> { Msg = ex.Message, Data = new List<PvfCommentDto>() };
		}
		finally
		{
			FileLock.Release();
		}
	}

	public async Task<ResultData<PvfCommentDto>> GetPvfComment(PvfCommentDtoRes res)
	{
		await FileLock.WaitAsync();
		try
		{
			string section = NormalizeSection(res.Section);
			PvfCommentDto match = LoadFile(res.FileType).FirstOrDefault(item =>
				item.FileType == res.FileType &&
				item.PvfCommentType == res.PvfCommentType &&
				string.Equals(NormalizeSection(item.Section), section, StringComparison.OrdinalIgnoreCase));
			return match == null
				? new ResultData<PvfCommentDto> { Msg = AppSetting.Instance.GetIlogger()?.GetStr("mess_ToTranslate") }
				: new ResultData<PvfCommentDto> { Data = match.CloneData() };
		}
		catch (Exception ex)
		{
			return new ResultData<PvfCommentDto> { Msg = "Err:" + ex.Message };
		}
		finally
		{
			FileLock.Release();
		}
	}

	public async Task<ResultData> AddComment(PvfCommentDto dto)
	{
		await FileLock.WaitAsync();
		try
		{
			dto.Section = NormalizeSection(dto.Section);
			List<PvfCommentDto> items = LoadFile(dto.FileType);
			PvfCommentDto previous = items.FirstOrDefault(item => SameKey(item, dto));
			if (previous != null)
			{
				dto.Create = previous.Create;
				items.Remove(previous);
			}
			else if (dto.Create == default)
			{
				dto.Create = DateTime.Now;
			}
			dto.Id = 0;
			dto.UpdateTime = DateTime.Now;
			items.Add(dto.CloneData());
			SaveFile(dto.FileType, items);
			return new ResultData();
		}
		catch (Exception ex)
		{
			return new ResultData { Msg = ex.Message };
		}
		finally
		{
			FileLock.Release();
		}
	}

	public async Task<ResultData> ClearAddRanged(List<PvfCommentDto> list)
	{
		await FileLock.WaitAsync();
		try
		{
			Directory.CreateDirectory(CommentsDirectory);
			foreach (string file in Directory.EnumerateFiles(CommentsDirectory, "*.json"))
			{
				File.Delete(file);
			}
			SaveAll(list);
			return new ResultData();
		}
		catch (Exception ex)
		{
			return new ResultData { Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SaveTranslationCommentFailed"), ex.Message) };
		}
		finally
		{
			FileLock.Release();
		}
	}

	private static List<PvfCommentDto> LoadAll()
	{
		Directory.CreateDirectory(CommentsDirectory);
		return Directory.EnumerateFiles(CommentsDirectory, "*.json")
			.SelectMany(LoadJsonFile)
			.ToList();
	}

	private static List<PvfCommentDto> LoadFile(PvfFileType? fileType)
	{
		string path = GetFilePath(fileType);
		return File.Exists(path) ? LoadJsonFile(path) : new List<PvfCommentDto>();
	}

	private static List<PvfCommentDto> LoadJsonFile(string path)
	{
		CommentFile file = JsonConvert.DeserializeObject<CommentFile>(File.ReadAllText(path), JsonSettings);
		return file?.Comments ?? new List<PvfCommentDto>();
	}

	private static void SaveAll(IEnumerable<PvfCommentDto> items)
	{
		foreach (IGrouping<PvfFileType?, PvfCommentDto> group in items.GroupBy(item => item.FileType))
		{
			List<PvfCommentDto> unique = new();
			foreach (PvfCommentDto item in group)
			{
				Upsert(unique, item);
			}
			SaveFile(group.Key, unique);
		}
	}

	private static void SaveFile(PvfFileType? fileType, List<PvfCommentDto> items)
	{
		Directory.CreateDirectory(CommentsDirectory);
		string path = GetFilePath(fileType);
		string temporaryPath = path + ".tmp";
		CommentFile file = new()
		{
			FileType = fileType?.ToString() ?? "unknown",
			Comments = items.OrderBy(item => item.PvfCommentType).ThenBy(item => item.Section, StringComparer.OrdinalIgnoreCase).ToList()
		};
		File.WriteAllText(temporaryPath, JsonConvert.SerializeObject(file, JsonSettings));
		File.Move(temporaryPath, path, true);
	}

	private static string GetFilePath(PvfFileType? fileType)
	{
		string suffix = (fileType?.ToString() ?? "unknown").ToLowerInvariant();
		return Path.Combine(CommentsDirectory, suffix + ".json");
	}

	private static void Upsert(List<PvfCommentDto> items, PvfCommentDto value)
	{
		value.Section = NormalizeSection(value.Section);
		items.RemoveAll(item => SameKey(item, value));
		items.Add(value.CloneData());
	}

	private static bool SameKey(PvfCommentDto left, PvfCommentDto right)
	{
		return left.FileType == right.FileType &&
			left.PvfCommentType == right.PvfCommentType &&
			string.Equals(NormalizeSection(left.Section), NormalizeSection(right.Section), StringComparison.OrdinalIgnoreCase);
	}

	private static string NormalizeSection(string section)
	{
		return (section ?? string.Empty).Replace("[/", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty).Replace("`", string.Empty).Trim();
	}
}
