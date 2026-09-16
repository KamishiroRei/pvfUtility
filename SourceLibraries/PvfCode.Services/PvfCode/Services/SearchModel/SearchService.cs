using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.GMTool.Models;
using PvfCode.Services.SearchModel.Enums;
using Utools;

namespace PvfCode.Services.SearchModel;

public class SearchService
{
	private Ilogger logger;

	private readonly PvfGroup pvfGroup;

	private readonly SearchConfig config;

	private ConcurrentHashSet<string> searchResults;

	private readonly bool allowLog;

	public Ilogger Logger
	{
		get
		{
			if (logger == null)
			{
				logger = AppSetting.Instance.GetService<Ilogger>();
			}
			return logger;
		}
	}

	public SearchService(SearchConfig config, PvfGroup pvfGroup, bool allowLog = true)
	{
		this.config = config;
		searchResults = new ConcurrentHashSet<string>();
		this.pvfGroup = pvfGroup;
		this.allowLog = allowLog;
	}

	public Task<ResultData<HashSet<string>>> Search()
	{
		return Search(CancellationToken.None);
	}

	public async Task<ResultData<HashSet<string>>> Search(CancellationToken cancellationToken)
	{
		ResultData<HashSet<string>> re = new ResultData<HashSet<string>>();
		try
		{
			await Task.Delay(1);
			ResultData resultData = ValidateSearchConfiguration();
			if (resultData.IsError)
			{
				return new ResultData<HashSet<string>>
				{
					Msg = resultData.Msg
				};
			}
			if (config.UseRegularExpression && config.Type == SearchType.ScriptContent && config.ScriptContentSearchMode == ScriptContentSearchMode.基于文本 && !config.Trait)
			{
				try
				{
					config.Regex = new Regex(config.ScriptContent);
				}
				catch (Exception ex)
				{
					re.Msg = string.Format(AppSetting.Instance.GetIlogger().GetStr("mess_StringLinkRegexError"), ex.Message);
				}
			}
			HashSet<int> hashSet = new HashSet<int>();
			ConcurrentBag<string> concurrentBag = new ConcurrentBag<string>();
			Action<PvfFile> searchAction = null;
			switch (config.Type)
			{
			case SearchType.Num:
				searchAction = CreateNumericSearchAction(concurrentBag);
				break;
			case SearchType.Strings:
				searchAction = CreateStringReferenceSearchAction(hashSet, concurrentBag);
				break;
			case SearchType.FileName:
				searchAction = CreateFileNameSearchAction(concurrentBag);
				break;
			case SearchType.ScriptContent:
				searchAction = CreateScriptContentSearchAction(concurrentBag);
				break;
			case SearchType.Name:
				searchAction = CreateItemNameSearchAction(concurrentBag);
				break;
			}
			if (searchAction == null)
			{
				re.Msg = "ActionIsNull";
				return re;
			}
			List<PvfFile> list = new List<PvfFile>();
			bool flag = config.FileTypesString != null && config.FileTypesString.Count > 0;
			if (config.SourceType == SearchSourceType.AllFiles)
			{
				list = ((!flag) ? pvfGroup.FileList.Values.ToList() : (from it in pvfGroup.FileList
					where MachRemoveOrKeep(it.Key) && it.Value != null
					select it.Value).ToList());
			}
			else if (flag)
			{
				foreach (string item in config.SearchResult)
				{
					if (MachRemoveOrKeep(item) && pvfGroup.FileList.TryGetValue(item, out PvfFile value) && value != null)
					{
						list.Add(value);
					}
				}
			}
			else
			{
				foreach (string item2 in config.SearchResult)
				{
					if (pvfGroup.FileList.TryGetValue(item2, out PvfFile value2) && value2 != null)
					{
						list.Add(value2);
					}
				}
			}
			if (config.SourceType != SearchSourceType.AllFiles)
			{
				if (config.SourceType == SearchSourceType.InSearchResultRemove)
				{
					searchResults = new ConcurrentHashSet<string>(list.Select(it => it.FileName));
				}
				Parallel.ForEach(list, new ParallelOptions { CancellationToken = cancellationToken }, (item, parallelLoopState) =>
				{
					if (MatchesSearchPath(config.IsUseLikeSearchPath, item.FileName, config.SearchFolder))
					{
						searchAction(item);
					}
				});
			}
			else
			{
				Parallel.ForEach(list, new ParallelOptions { CancellationToken = cancellationToken }, (item, parallelLoopState) =>
				{
					if (MatchesSearchPath(config.IsUseLikeSearchPath, item.FileName, config.SearchFolder))
					{
						searchAction(item);
					}
				});
				foreach (string item3 in concurrentBag)
				{
					UpdateSearchResult(item3);
				}
			}
			if (allowLog)
			{
				string msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkSearchResult"), searchResults.Count);
				Logger?.Success(msg);
			}
			return new ResultData<HashSet<string>>
			{
				Data = searchResults.ToHashSet()
			};
		}
		catch (Exception ex2)
		{
			if (allowLog)
			{
				Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkSearchError"), config.Type, ex2.Message, ex2.Source, ex2.StackTrace));
			}
			re.Msg = ex2.Message;
		}
		return re;
	}

	public bool MachRemoveOrKeep(string filePath)
	{
		if (config.RemoveOrKeep == RemoveOrKeepFileType.保留)
		{
			return config.FileTypesString.Contains(Path.GetExtension(filePath));
		}
		return !config.FileTypesString.Contains(Path.GetExtension(filePath));
	}

	private ResultData ValidateSearchConfiguration()
	{
		ResultData resultData = new ResultData();
		if (config.Type == SearchType.Num)
		{
			int result2;
			if (config.Keyword.IndexOf('.') > 0)
			{
				if (!float.TryParse(config.Keyword, out var _))
				{
					resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FloatError");
				}
			}
			else if (!int.TryParse(config.Keyword, out result2))
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_IntError");
			}
		}
		return resultData;
	}

	private Action<PvfFile> CreateNumericSearchAction(ConcurrentBag<string> pendingResults)
	{
		int searchValue;
		if (config.Keyword.IndexOf('.') > 0)
		{
			float value = float.Parse(config.Keyword);
			searchValue = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
		}
		else
		{
			searchValue = int.Parse(config.Keyword);
		}
		bool updateDirectly = config.SourceType != SearchSourceType.AllFiles;
		return file =>
		{
			try
			{
				if (file.ContainsIntegerValue(pvfGroup, searchValue))
				{
					AddSearchResult(file, pendingResults, updateDirectly);
				}
			}
			catch (Exception ex)
			{
				Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_StringLinkSearchError"), config.Type, ex.Message, ex.Source, ex.StackTrace));
			}
		};
	}

	private Action<PvfFile> CreateStringReferenceSearchAction(HashSet<int> stringTableIndexes, ConcurrentBag<string> pendingResults)
	{
		string text = config.Keyword;
		if (AppSetting.Instance.PvfConfig.DefaultEncoding == EncodingType.TW)
		{
			text = ChineseHelper.ToTraditional(config.Keyword);
		}
		if (config.UseRegularExpression)
		{
			pvfGroup.Strtable.FindStringItem(stringTableIndexes, text, config.IsStartMatch, useLike: false, config.Regex);
			pvfGroup.Strview.SearchstrInFiles(stringTableIndexes, pvfGroup.Strtable, text, config.IsStartMatch, useLike: false, config.Regex);
		}
		else if (config.WholeWordMatch)
		{
			pvfGroup.Strtable.FindStringItem(stringTableIndexes, text, config.IsStartMatch, useLike: true, null);
			pvfGroup.Strview.SearchstrInFiles(stringTableIndexes, pvfGroup.Strtable, text, config.IsStartMatch, useLike: true, null);
		}
		else
		{
			pvfGroup.Strtable.FindStringItem(stringTableIndexes, text, config.IsStartMatch, useLike: false, null);
			pvfGroup.Strview.SearchstrInFiles(stringTableIndexes, pvfGroup.Strtable, text, config.IsStartMatch, useLike: false, null);
		}
		bool updateDirectly = config.SourceType != SearchSourceType.AllFiles;
		return file =>
		{
			if (file.ContainsStringTableReference(pvfGroup, stringTableIndexes, text, config.IsStartMatch))
			{
				AddSearchResult(file, pendingResults, updateDirectly);
			}
		};
	}

	private Action<PvfFile> CreateFileNameSearchAction(ConcurrentBag<string> pendingResults)
	{
		bool updateDirectly = config.SourceType != SearchSourceType.AllFiles;
		return file =>
		{
			if (config.UseRegularExpression)
			{
				if (config.Regex != null && config.Regex.IsMatch(file.FileName))
				{
					AddSearchResult(file, pendingResults, updateDirectly);
				}
			}
			else if (config.WholeWordMatch)
			{
				if (LikeOperator.LikeString(file.FileName, config.Keyword, CompareMethod.Binary))
				{
					AddSearchResult(file, pendingResults, updateDirectly);
				}
			}
			else
			{
				if (config.IsStartMatch && file.FileName.IndexOf(config.Keyword, StringComparison.OrdinalIgnoreCase) == 0)
				{
					AddSearchResult(file, pendingResults, updateDirectly);
				}
				if (!config.IsStartMatch && file.FileName.IndexOf(config.Keyword, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					AddSearchResult(file, pendingResults, updateDirectly);
				}
			}
		};
	}

	private Action<PvfFile> CreateScriptContentSearchAction(ConcurrentBag<string> pendingResults)
	{
		if (config.Trait)
		{
			return CreateScriptRangeSearchAction(pendingResults);
		}
		if (config.ScriptContentSearchMode == ScriptContentSearchMode.二进制)
		{
			// 统一管线：110/NKPI 的 GetBinaryForScan 返回经典视图，二进制序列按经典 token 编译匹配
			(bool success, byte[] binaryPattern) = new ScriptFileCompilerOl(pvfGroup).EncryptScriptText(config.ScriptContent, readOnly: true);
			if (success)
			{
				bool updateDirectly = config.SourceType != SearchSourceType.AllFiles;
				return file =>
				{
					if (file.ContainsBinarySequence(pvfGroup, binaryPattern))
					{
						AddSearchResult(file, pendingResults, updateDirectly);
					}
				};
			}
			Logger.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_SearchBinaryError"));
			return null;
		}
		return CreateScriptContentTextSearchAction(pendingResults);
	}

	private Action<PvfFile> CreateScriptContentTextSearchAction(ConcurrentBag<string> pendingResults)
	{
		bool addDirectly = config.SourceType != SearchSourceType.AllFiles;
		return file =>
		{
			if (file.FileName == "stringtable.bin")
			{
				return;
			}
			// 全量文本搜索：内容不落地 Data，搜完即释放，避免把整个包物化为常驻内存
			string fileText = pvfGroup.GetFileText(file, AppSetting.Instance.PvfConfig.DefaultEncoding, persistLazyData: false);
			if (fileText != null && ((!config.UseRegularExpression)
				? fileText.Length != 0 && fileText.IndexOf(config.ScriptContent, StringComparison.OrdinalIgnoreCase) != -1
				: config.Regex.IsMatch(fileText)))
			{
				AddSearchResult(file, pendingResults, addDirectly);
			}
		};
	}

	private Action<PvfFile> CreateScriptRangeSearchAction(ConcurrentBag<string> pendingResults)
	{
		bool updateDirectly = config.SourceType != SearchSourceType.AllFiles;
		return file =>
		{
			string fileText = pvfGroup.GetFileText(file, AppSetting.Instance.PvfConfig.DefaultEncoding, persistLazyData: false);
			if (fileText != null && fileText.Length != 0)
			{
				int startIndex = fileText.IndexOf(config.ScriptContentStart, StringComparison.Ordinal);
				if (startIndex != -1 && fileText.IndexOf(config.ScriptContentStop, startIndex, StringComparison.Ordinal) != -1)
				{
					AddSearchResult(file, pendingResults, updateDirectly);
				}
			}
		};
	}

	private Action<PvfFile> CreateItemNameSearchAction(ConcurrentBag<string> pendingResults)
	{
		string searchText = config.Keyword;
		if (AppSetting.Instance.PvfConfig.DefaultEncoding == EncodingType.TW)
		{
			if (AppSetting.Instance.PvfConfig.FileTextTraditionalConvertSimplified)
			{
				searchText = ChineseHelper.ToSimplified(config.Keyword);
			}
			else
			{
				searchText = ChineseHelper.ToTraditional(config.Keyword);
			}
		}
		bool updateDirectly = config.SourceType != SearchSourceType.AllFiles;
		return file =>
		{
			if (config.UseRegularExpression)
			{
				if (file.MatchesItemName(pvfGroup, config.IsStartMatch, searchText, false, config.Regex))
				{
					AddSearchResult(file, pendingResults, updateDirectly);
				}
			}
			else if (config.WholeWordMatch)
			{
				if (file.MatchesItemName(pvfGroup, false, searchText, false, null))
				{
					AddSearchResult(file, pendingResults, updateDirectly);
				}
			}
			else if (file.MatchesItemName(pvfGroup, config.IsStartMatch, searchText, true, null))
			{
				AddSearchResult(file, pendingResults, updateDirectly);
			}
		};
	}

	private void UpdateSearchResult(string fileName)
	{
		if (config.SourceType == SearchSourceType.InSearchResultRemove)
		{
			searchResults.Remove(fileName);
		}
		else
		{
			searchResults.Add(fileName);
		}
	}

	private void AddSearchResult(PvfFile file, ConcurrentBag<string> pendingResults, bool updateDirectly)
	{
		if (updateDirectly)
		{
			UpdateSearchResult(file.FileName);
		}
		else
		{
			pendingResults.Add(file.FileName);
		}
	}

	private static bool MatchesSearchPath(bool useExtendedPattern, string fileName, string searchFolder)
	{
		if (!useExtendedPattern || !PathsHelper.IsPathMatchEx(fileName, searchFolder))
		{
			if (!useExtendedPattern)
			{
				return PathsHelper.IsPathMatch(fileName, searchFolder);
			}
			return false;
		}
		return true;
	}

	public Task<ResultData<IEnumerable<ItemCodePostalData>>> SearchItemCode(string keyword, bool wholeWordMatch, bool isStartMatch)
	{
		ResultData<IEnumerable<ItemCodePostalData>> resultData = new ResultData<IEnumerable<ItemCodePostalData>>();
		string searchText = keyword;
		if (AppSetting.Instance.PvfConfig.DefaultEncoding == EncodingType.TW)
		{
			if (AppSetting.Instance.PvfConfig.FileTextTraditionalConvertSimplified)
			{
				searchText = ChineseHelper.ToSimplified(searchText);
			}
			else
			{
				searchText = ChineseHelper.ToTraditional(searchText);
			}
		}
		ConcurrentBag<ItemCodePostalData> matches = new ConcurrentBag<ItemCodePostalData>();
		Action<KeyValuePair<string, PvfFile>> addMatch = row =>
		{
			if (row.Value.MatchesItemName(pvfGroup, isStartMatch, searchText, !wholeWordMatch, null))
			{
				matches.Add(new ItemCodePostalData(row.Value, pvfGroup));
			}
		};
		Parallel.ForEach(from it in pvfGroup.FileList
			where it.Value.FilePathHeader == "equipment" || it.Value.FilePathHeader == "stackable"
			where it.Value.ItemCode.HasValue
			select it, (item, parallelLoopState) =>
		{
			addMatch(item);
		});
		resultData.Data = matches;
		return Task.FromResult(resultData);
	}
}
