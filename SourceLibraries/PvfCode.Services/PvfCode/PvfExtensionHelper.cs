using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Collections.Pooled;
using DevExpress.Mvvm;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Models.BatchOperation;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Models.Pvf.ImportModels;
using PvfCode.Models.Pvf.PvfFileDiffModels.Enums;
using PvfCode.Services;
using PvfCode.Services.ExtractFilesModel;
using PvfCode.Services.PvfParsingNew;
using PvfCode.Services.PvfParsingNew.EditorPrivew;
using Utools;

namespace PvfCode;

public static class PvfExtensionHelper
{
	private static Ilogger logger;

	private static Ilogger Logger => logger ??= AppSetting.Instance.GetService<Ilogger>();

	public static TreelistCommentRes? GetTreeListComment(this string fullPath)
	{
		AppSetting.Instance.PvfConfig.TreelistCommentDic.TryGetValue(fullPath, out TreelistCommentRes value);
		return value;
	}

	public static TreelistCommentRes? GetTreeListComment(this string fullPath, Dictionary<string, TreelistCommentRes> source)
	{
		if (source == null)
		{
			return null;
		}
		source.TryGetValue(fullPath, out TreelistCommentRes value);
		return value;
	}

	public static string GetFileText(this PvfGroup group, PvfFile file, EncodingType? encoding = null, bool? useCompatibleDecompiler = null, bool showAniError = true)
	{
		if (!encoding.HasValue)
		{
			encoding = AppSetting.Instance.PvfConfig.DefaultEncoding;
		}
		if (!useCompatibleDecompiler.HasValue)
		{
			useCompatibleDecompiler = AppSetting.Instance.PvfConfig.UseCompatibleDecompiler;
		}
		if (file.Data == null)
		{
			return string.Empty;
		}
		if (file.IsScriptFile)
		{
			if (file.FileType == PvfFileType.lst)
			{
				string fileName = file.FileName;
				if (fileName == "n_quest/epicquest.lst" || fileName == "n_quest/trainingquest.lst" || fileName == "n_quest/dailyrandomquest.lst")
				{
					if (!useCompatibleDecompiler.Value)
					{
						return new ScriptFileParserNew(file, group).PraseText();
					}
					return new ScriptFileCompilerOl(group).Decompile(file);
				}
				return new ScriptFileCompilerOl(group).Decompile(file);
			}
			if (!useCompatibleDecompiler.Value)
			{
				return new ScriptFileParserNew(file, group).PraseText();
			}
			return new ScriptFileCompilerOl(group).Decompile(file);
		}
		if (!file.IsBinaryAniFile)
		{
			if (file.FileType == PvfFileType.str)
			{
				return AppSetting.Instance.PvfConfig.FileTextTraditionalConvertSimplifiedAutoMethods(Encoding.GetEncoding((int)encoding.Value).GetString(file.Data).TrimEnd(new char[1]));
			}
			return Encoding.GetEncoding((int)encoding.Value).GetString(file.Data).TrimEnd(new char[1]);
		}
		var (flag, result) = BinaryAniCompiler.DecompileBinaryAni(file);
		if (flag)
		{
			return result;
		}
		if (showAniError)
		{
			Logger.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError2"));
		}
		return string.Empty;
	}

	public static string GetFileText(this PvfGroup group, string filePath, EncodingType? encoding = null, bool? useCompatibleDecompiler = null, bool showAniError = true)
	{
		return group.GetFileText(group.GetFile(filePath), encoding, useCompatibleDecompiler, showAniError);
	}

	public static async Task<ResultData> ExtractFiles(this PvfGroup group, ExtractConfig config)
	{
		return await new ServiceExtractFiles(group, config).Extract();
	}

	public static async Task<ResultData> ImportFiles(this PvfGroup pvf, ImportConfig config, bool is7z, string filePath7z = null)
	{
		return await new ServiceImportFiles(pvf, config).Import(is7z, filePath7z);
	}

	public static bool FindFilePath(this PvfGroup pvf, string rightFilePath, out string? fullPath)
	{
		if (pvf.FileList == null || pvf.FileList.Count == 0)
		{
			fullPath = null;
			return false;
		}
		string matchedPath = null;
		Parallel.ForEach(pvf.FileList.Keys, (item, state) =>
		{
			if (item.Contains(rightFilePath))
			{
				matchedPath = item;
				state.Break();
			}
		});
		fullPath = matchedPath;
		return fullPath != null;
	}

	public static IEnumerable<string> FindFilePath(this PvfGroup pvf, string key)
	{
		if (pvf.FileList == null || pvf.FileList.Count == 0)
		{
			return null;
		}
		ConcurrentBag<string> matches = new ConcurrentBag<string>();
		Parallel.ForEach(pvf.FileList.Keys, (item, state) =>
		{
			if (item.Contains(key))
			{
				matches.Add(key);
				if (matches.Count() == 10)
				{
					state.Break();
				}
			}
		});
		return matches;
	}

	public static bool FindFilePath(this PvfGroup pvf, string header, string rightFilePath, out string? fullPath)
	{
		if (pvf.FileList == null || pvf.FileList.Count == 0)
		{
			fullPath = null;
			return false;
		}
		header = header.ToLower();
		int headerLength = header.Length;
		string matchedPath = null;
		Parallel.ForEach(pvf.FileList.Keys, (item, state) =>
		{
			if (item.Length > headerLength && item.Substring(0, headerLength) == header && item.EndsWith(rightFilePath))
			{
				matchedPath = item;
				state.Break();
			}
		});
		fullPath = matchedPath;
		return fullPath != null;
	}

	public static bool FindFileLableBottomNumber(this PvfGroup pvf, PvfFile file, string lableText, out int number)
	{
		number = 0;
		var (flag, array) = new ScriptFileCompilerOl(pvf).EncryptScriptText(lableText, readOnly: true);
		if (!flag || array == null || array.Length == 0)
		{
			return false;
		}
		if (file == null)
		{
			return false;
		}
		if (!file.IsScriptFile)
		{
			return false;
		}
		int num = array.Length;
		if (file.DataLen < num)
		{
			return false;
		}
		int num2 = -1;
		for (int i = 2; i < file.DataLen; i += 5)
		{
			if (file.Data[i] == array[0] && BitConverter.ToInt32(file.Data, i + 1) == BitConverter.ToInt32(array, 1))
			{
				if (array.Length == 5)
				{
					num2 = i;
					break;
				}
				if (i + num > file.DataLen)
				{
					return false;
				}
				byte[] array2 = new byte[num];
				Buffer.BlockCopy(file.Data, i, array2, 0, num);
				if (DataHelper.BytesEquals(array2, array))
				{
					num2 = i;
					break;
				}
			}
		}
		if (num2 + 5 <= file.Data.Length)
		{
			num2 += 5;
			if (file.Data[num2] == 2)
			{
				number = BitConverter.ToInt32(file.Data, num2 + 1);
				return true;
			}
		}
		return false;
	}

	public static async Task<ResultData<IEnumerable<string>>> BatchOperation(this PvfGroup pvf, BatchOperationConfig config, ICommand<(IEnumerable<string>, IEnumerable<string>)> command)
	{
		return await Task.Run(() => new ServiceBatchOperation(pvf, config, command).StartMain());
	}

	public static async Task<ResultData> BatchOperation(this PvfGroup pvf, List<BatchOperationConfig> configs, ICommand<(IEnumerable<string>, IEnumerable<string>)> command)
	{
		return await Task.Run(() => new ServiceBatchOperation(pvf, configs, command).StartMain());
	}

	public static Task<List<KeyValuePair<int, LstItem>>> LstFileToLstTab(this PvfGroup pvf, PvfFile file)
	{
		if (file.FileType != PvfFileType.lst)
		{
			return null;
		}
		Stringtable strtable = pvf.Strtable;
		List<KeyValuePair<int, LstItem>> list = new List<KeyValuePair<int, LstItem>>();
		byte[] data = file.Data;
		int dataLen = file.DataLen;
		string filePathHeader = file.FilePathHeader;
		for (int i = 2; i < dataLen; i += 10)
		{
			int num = BitConverter.ToInt32(data, i + 1);
			try
			{
				string stringItem = strtable.GetStringItem(BitConverter.ToInt32(file.Data, i + 1 + 5));
				list.Add(new KeyValuePair<int, LstItem>(num, new LstItem(filePathHeader, stringItem, num)));
			}
			catch (Exception)
			{
				Logger.Error(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError3"));
			}
		}
		return Task.FromResult(list);
	}

	public static Task<ResultData<Dictionary<int, string>>> LstFileTabCodeDic(this PvfGroup pvf, PvfFile file)
	{
		ResultData<Dictionary<int, string>> resultData = new ResultData<Dictionary<int, string>>();
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		if (file.FileType != PvfFileType.lst)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError4"), file.FileName);
			return Task.FromResult(resultData);
		}
		Stringtable strtable = pvf.Strtable;
		byte[] data = file.Data;
		int dataLen = file.DataLen;
		for (int i = 2; i < dataLen; i += 10)
		{
			int key = BitConverter.ToInt32(data, i + 1);
			if (!dictionary.ContainsKey(key))
			{
				try
				{
					string stringItem = strtable.GetStringItem(BitConverter.ToInt32(file.Data, i + 1 + 5));
					dictionary.Add(key, stringItem);
				}
				catch (Exception)
				{
					resultData.Msg = AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError3");
					return Task.FromResult(resultData);
				}
			}
		}
		resultData.Data = dictionary;
		return Task.FromResult(resultData);
	}

	public static Task<ResultData<Dictionary<int, string>>> LstFileTabCodeDic(this PvfGroup pvf, string lstFilePath)
	{
		ResultData<Dictionary<int, string>> resultData = new ResultData<Dictionary<int, string>>();
		if (lstFilePath == null)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathCannotBeEmpty");
			return Task.FromResult(resultData);
		}
		if (!pvf.FileList.TryGetValue(lstFilePath, out PvfFile value))
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger().GetStr("mess_FileNotExist"), lstFilePath);
			return Task.FromResult(resultData);
		}
		return pvf.LstFileTabCodeDic(value);
	}

	public static Task<ResultData<Dictionary<string, int>>> LstFileTabPathDic(this PvfGroup pvf, PvfFile file)
	{
		ResultData<Dictionary<string, int>> resultData = new ResultData<Dictionary<string, int>>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		if (file.FileType != PvfFileType.lst)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError4"), file.FileName);
			return Task.FromResult(resultData);
		}
		Stringtable strtable = pvf.Strtable;
		byte[] data = file.Data;
		int dataLen = file.DataLen;
		for (int i = 2; i < dataLen; i += 10)
		{
			try
			{
				int value = BitConverter.ToInt32(data, i + 1);
				string stringItem = strtable.GetStringItem(BitConverter.ToInt32(file.Data, i + 1 + 5));
				if (!dictionary.ContainsKey(stringItem))
				{
					dictionary.Add(stringItem, value);
				}
			}
			catch (Exception)
			{
				resultData.Msg = AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError3");
				return Task.FromResult(resultData);
			}
		}
		resultData.Data = dictionary;
		return Task.FromResult(resultData);
	}

	public static Task<ResultData<Dictionary<string, int>>> LstFileTabPathDic(this PvfGroup pvf, string lstFilePath)
	{
		ResultData<Dictionary<string, int>> resultData = new ResultData<Dictionary<string, int>>();
		if (lstFilePath == null)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathCannotBeEmpty");
			return Task.FromResult(resultData);
		}
		if (!pvf.FileList.TryGetValue(lstFilePath, out PvfFile value))
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), lstFilePath);
			return Task.FromResult(resultData);
		}
		return pvf.LstFileTabPathDic(value);
	}

	public static ResultData<Dictionary<int, LstItem>> GetLstDicTable(this PvfGroup pvf, PvfFile file)
	{
		ResultData<Dictionary<int, LstItem>> resultData = new ResultData<Dictionary<int, LstItem>>();
		if (file.FileType != PvfFileType.lst)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError4"), file.FileName);
			return resultData;
		}
		Stringtable strtable = pvf.Strtable;
		Dictionary<int, LstItem> dictionary = new Dictionary<int, LstItem>();
		byte[] data = file.Data;
		int dataLen = file.DataLen;
		string filePathHeader = file.FilePathHeader;
		for (int i = 2; i < dataLen; i += 10)
		{
			int num = BitConverter.ToInt32(data, i + 1);
			if (!dictionary.ContainsKey(num))
			{
				try
				{
					string stringItem = strtable.GetStringItem(BitConverter.ToInt32(file.Data, i + 1 + 5));
					dictionary.Add(num, new LstItem(filePathHeader, stringItem, num));
				}
				catch (Exception)
				{
					resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError5"), num, file.FileName);
					return resultData;
				}
			}
		}
		resultData.Data = dictionary;
		return resultData;
	}

	public static ResultData<Dictionary<int, LstItem>> GetLstDicTable(this PvfGroup pvf, string filePath)
	{
		ResultData<Dictionary<int, LstItem>> resultData = new ResultData<Dictionary<int, LstItem>>();
		if (!pvf.FileList.TryGetValue(filePath, out PvfFile value))
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), filePath);
		}
		return pvf.GetLstDicTable(value);
	}

	public static Task<List<string>> GetLstFiles(this PvfGroup pvf)
	{
		ConcurrentBag<string> lstFiles = new ConcurrentBag<string>();
		Parallel.ForEach(pvf.FileList.Values, item =>
		{
			if (item.FileType == PvfFileType.lst)
			{
				lstFiles.Add(item.FileName);
			}
		});
		return Task.FromResult(lstFiles.ToList());
	}

	public static bool FileContentDiff(this PvfGroup pvf, PvfFile file, PvfGroup pvfRight, PvfFile file2, out List<PvfFileDiffType>? diffs)
	{
		diffs = new List<PvfFileDiffType>();
		if (file.FileType == PvfFileType.nut)
		{
			string fileText = pvf.GetFileText(file);
			string fileText2 = pvfRight.GetFileText(file2);
			try
			{
				fileText = new JSBeautify(fileText, new JSBeautifyOptions()).GetResult();
				fileText2 = new JSBeautify(fileText2, new JSBeautifyOptions()).GetResult();
				StrHelper.TraitFindReplceMain(fileText, StringComparison.OrdinalIgnoreCase, "/*", "*/", string.Empty, out fileText);
				StrHelper.TraitFindReplceMain(fileText2, StringComparison.OrdinalIgnoreCase, "/*", "*/", string.Empty, out fileText2);
				if (RemoveCommentOnlyLines(fileText) != RemoveCommentOnlyLines(fileText2))
				{
					diffs.Add(PvfFileDiffType.FileContent);
				}
			}
			catch (Exception)
			{
				AppSetting.Instance.GetIlogger()?.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_PVFCompareError"), file.FileName, file2.FileName));
			}
		}
		else
		{
			if (file.IsScriptFile && file.ItemCode != file2.ItemCode)
			{
				diffs.Add(PvfFileDiffType.ItemCode);
			}
			if (!pvf.HasEquivalentFileContent(file, file2, pvfRight))
			{
				diffs.Add(PvfFileDiffType.FileContent);
			}
		}
		return diffs.Count > 0;
	}

	private static string RemoveCommentOnlyLines(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		string[] lines = text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string line in lines)
		{
			string compactLine = line.Replace(" ", string.Empty).Replace("\t", string.Empty);
			if (compactLine.Length < 2 || compactLine.Substring(0, 2) != "//")
			{
				stringBuilder.AppendLine(line);
			}
		}
		return stringBuilder.ToString();
	}

	private static bool HasEquivalentFileContent(this PvfGroup pvf, PvfFile file, PvfFile otherFile, PvfGroup otherPvf)
	{
		if (file.IsScriptFile != otherFile?.IsScriptFile)
		{
			return false;
		}
		if (!file.IsScriptFile && !otherFile.IsScriptFile)
		{
			return DataHelper.BytesEquals(otherFile.Data, file.Data);
		}
		if (otherFile.DataLen != file.DataLen)
		{
			return false;
		}
		for (int i = 2; i < file.DataLen - 4; i += 5)
		{
			byte valueType = file.Data[i];
			if ((valueType == 5 || valueType == 6 || valueType == 7 || valueType == 8 || valueType == 10) && pvf.Strtable.GetStringItem(BitConverter.ToInt32(file.Data, i + 1)) != otherPvf.Strtable.GetStringItem(BitConverter.ToInt32(otherFile.Data, i + 1)))
			{
				return false;
			}
			if ((valueType == 9 || valueType == 4 || valueType == 2) && BitConverter.ToInt32(file.Data, i + 1) != BitConverter.ToInt32(otherFile.Data, i + 1))
			{
				return false;
			}
		}
		return true;
	}

	public static ResultData<ImageSource> GetScriptIconSource(this PvfGroup pvf, PvfFile file)
	{
		if (ImagePack2Service.Instance.GetIcon(pvf, file, out ImageSource imageSource))
		{
			return new ResultData<ImageSource>
			{
				Data = imageSource
			};
		}
		return new ResultData<ImageSource>();
	}

	public static ResultData<ImageSource> GetScriptIconSource(this PvfGroup pvf, string filePath)
	{
		PvfFile file = pvf.GetFile(filePath);
		if (file == null)
		{
			return new ResultData<ImageSource>
			{
				Msg = string.Format(AppSetting.Instance.GetIlogger().GetStr("mess_FileNotExist"), filePath)
			};
		}
		return pvf.GetScriptIconSource(file);
	}

	public static ResultData RegLst(this PvfGroup pvf, string filePath)
	{
		ResultData resultData = new ResultData();
		try
		{
			string lstPathHeader = pvf.GetFile(filePath).GetLstPathHeader();
			if (string.IsNullOrEmpty(lstPathHeader))
			{
				resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError6"), lstPathHeader);
				return resultData;
			}
			if (!pvf.ListFileTable.LstFilePaths.TryGetValue(lstPathHeader, out string value))
			{
				resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BinaryAniReadError7"), lstPathHeader);
				return resultData;
			}
			string fileText = pvf.GetFileText(pvf.GetFile(value));
			int value2 = pvf.ListFileTable.GetLstNumMax(value) + 1;
			int num = filePath.IndexOf("/");
			string value3 = filePath.Substring(num + 1, filePath.Length - num - 1);
			fileText += $"\r\n{value2}\t`{value3}`";
			if (!pvf.SaveFileText(value, fileText))
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_BinaryAniReadError8");
			}
		}
		catch (Exception ex)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FailedToRegisterTheLst"), ex.Message);
		}
		return resultData;
	}

	public static ResultData<PooledList<PrivewAniData>> GetPrivewAniData(this PvfFile file)
	{
		(bool, PooledList<PrivewAniData>) aniPrivewData = BinaryAniCompiler.GetAniPrivewData(file);
		if (aniPrivewData.Item1)
		{
			return new ResultData<PooledList<PrivewAniData>>
			{
				Data = aniPrivewData.Item2
			};
		}
		return new ResultData<PooledList<PrivewAniData>>
		{
			Msg = AppSetting.Instance.GetIlogger().GetStr("mess_BinaryAniReadError9")
		};
	}
}
