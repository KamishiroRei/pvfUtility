using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using PvfCode.Dot;
using PvfCode.LoggerBase;
using PvfCode.Models.BatchOperation;
using PvfCode.Models.BatchOperation.Enums;
using PvfCode.Models.Pvf.Enums;
using Utools;

namespace PvfCode.Services;

public class ServiceBatchOperation
{
	private readonly List<BatchOperationConfig> configs;

	private BatchOperationConfig currentConfig;

	private List<PvfFile> files;

	private readonly PvfGroup pvf;

	private readonly ICommand<(IEnumerable<string>, IEnumerable<string>)> command;

	private readonly ConcurrentBag<string> successFiles;

	private readonly ConcurrentBag<string> errorFiles;

	public ServiceBatchOperation(PvfGroup pvf, BatchOperationConfig config, ICommand<(IEnumerable<string>, IEnumerable<string>)> command)
	{
		this.command = command;
		this.pvf = pvf;
		configs = new List<BatchOperationConfig> { config };
		successFiles = new ConcurrentBag<string>();
		errorFiles = new ConcurrentBag<string>();
	}

	public ServiceBatchOperation(PvfGroup pvf, List<BatchOperationConfig> configs, ICommand<(IEnumerable<string>, IEnumerable<string>)> command)
	{
		this.command = command;
		this.pvf = pvf;
		this.configs = configs;
		successFiles = new ConcurrentBag<string>();
		errorFiles = new ConcurrentBag<string>();
	}

	public async Task<ResultData<IEnumerable<string>>> StartMain()
	{
		foreach (BatchOperationConfig config in configs)
		{
			currentConfig = config;
			ResultData resultData = await ProcessCurrentConfiguration();
			if (resultData.IsError)
			{
				return new ResultData<IEnumerable<string>>
				{
					Msg = resultData.Msg,
					Data = successFiles
				};
			}
		}
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		string text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BatchProcessComplete"), successFiles.Count, errorFiles.Count);
		ilogger?.Success(text);
		await ilogger.ShowNotification(new NotificationViewModel<BatchOperationLog>(command: new DelegateCommand<BatchOperationLog>(_ =>
		{
			ShowBatchOperationLog(new BatchOperationLog
			{
				ErrorFileList = errorFiles,
				SuccessFileList = successFiles
			});
		}), title: AppSetting.Instance.AppName, message: text, icon: AppSetting.Instance.GetRes().VisualStudioBlendLogo2015Pre_16x, buttonTitle: AppSetting.Instance.GetIlogger()?.GetStr("mess_ViewDetails")));
		return new ResultData<IEnumerable<string>>
		{
			Data = successFiles
		};
	}

	private async Task<ResultData> ProcessCurrentConfiguration()
	{
		ResultData resultData = new ResultData();
		if (currentConfig.FileTypes != null && currentConfig.FileTypes.Count > 0)
		{
			List<string> list = new List<string>();
			if (currentConfig.RemoveOrKeepFileType == RemoveOrKeepFileType.保留)
			{
				foreach (string sourceFile in currentConfig.SourceFiles)
				{
					if (currentConfig.FileTypes.Contains(Path.GetExtension(sourceFile)))
					{
						list.Add(sourceFile);
					}
				}
			}
			else
			{
				foreach (string sourceFile2 in currentConfig.SourceFiles)
				{
					if (!currentConfig.FileTypes.Contains(Path.GetExtension(sourceFile2)))
					{
						list.Add(sourceFile2);
					}
				}
			}
			currentConfig.SourceFiles = list.ToHashSet();
			if (currentConfig.SourceFiles.Count == 0)
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_BatchProcessNoFile");
				return resultData;
			}
		}
		files = pvf.GetFiles(currentConfig.SourceFiles);
		switch (currentConfig.BatchOperationType)
		{
		case BatchOperationType.DefaultFindReplace:
			resultData = await RunFindReplace();
			break;
		case BatchOperationType.TraitFindReplce:
			resultData = await RunTraitFindReplace();
			break;
		case BatchOperationType.AddContent:
			resultData = await AddContent();
			break;
		case BatchOperationType.DeleteSection:
			resultData = await RunDeleteSection();
			break;
		}
		currentConfig.SourceFiles = null;
		return resultData;
	}

	private void ShowBatchOperationLog(BatchOperationLog log)
	{
		command.Execute((log.SuccessFileList, log.ErrorFileList));
	}

	private Task<ResultData> RunTraitFindReplace()
	{
		ResultData resultData = new ResultData();
		try
		{
			Action<PvfFile> action = CreateTraitReplaceAction();
			foreach (PvfFile item in files)
			{
				action(item);
			}
		}
		catch (Exception ex)
		{
			resultData.Msg = ex.Message;
		}
		return Task.FromResult(resultData);
	}

	private Action<PvfFile> CreateTraitReplaceAction()
	{
		string startKeyword = currentConfig.FindStartKeyword;
		string endKeyword = currentConfig.FindEndKeyword;
		string replacement = currentConfig.TraitReplaceKeyword;
		StringComparison comparison = currentConfig.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
		return file =>
		{
			string newText = pvf.GetFileText(file, AppSetting.Instance.PvfConfig.DefaultEncoding);
			if (newText != null)
			{
				if (StrHelper.TraitFindReplceMain(newText, comparison, startKeyword, endKeyword, replacement, out newText))
				{
					if (pvf.SaveFileText(file, newText, AppSetting.Instance.PvfConfig.DefaultEncoding))
					{
						successFiles.Add(file.FileName);
					}
					else
					{
						errorFiles.Add(file.FileName);
					}
				}
				else
				{
					errorFiles.Add(file.FileName);
				}
			}
			else
			{
				errorFiles.Add(file.FileName);
			}
		};
	}

	private Task<ResultData> RunFindReplace()
	{
		ResultData resultData = new ResultData();
		try
		{
			Action<PvfFile> action = CreateFindReplaceAction();
			foreach (PvfFile item in files)
			{
				action(item);
			}
		}
		catch (Exception ex)
		{
			resultData.Msg = ex.Message;
		}
		return Task.FromResult(resultData);
	}

	private Action<PvfFile> CreateFindReplaceAction()
	{
		Regex regex;
		string replacement;
		try
		{
			RegexOptions regexOptions = RegexOptions.Multiline | RegexOptions.Compiled;
			if (!currentConfig.CaseSensitive)
			{
				regexOptions |= RegexOptions.IgnoreCase;
			}
			string pattern = currentConfig.FindKeyword;
			if (!currentConfig.RegularExpression)
			{
				pattern = Regex.Escape(currentConfig.FindKeyword);
			}
			regex = new Regex(pattern, regexOptions);
			replacement = StrHelper.Transform(currentConfig.ReplaceKeyword);
			if (currentConfig.WholeWordMatch)
			{
				replacement = string.Format("\\b{0}\\b", replacement);
			}
		}
		catch (Exception ex)
		{
			throw new Exception(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_BatchProcessRegexError"), ex.Message));
		}
		return file =>
		{
			string fileText = pvf.GetFileText(file, AppSetting.Instance.PvfConfig.DefaultEncoding);
			if (fileText != null && regex.IsMatch(fileText))
			{
				fileText = regex.Replace(fileText, replacement);
				if (pvf.SaveFileText(file, fileText, AppSetting.Instance.PvfConfig.DefaultEncoding))
				{
					successFiles.Add(file.FileName);
				}
			}
			else
			{
				errorFiles.Add(file.FileName);
			}
		};
	}

	private Action<PvfFile> CreateDeleteSectionAction()
	{
		string deleteSectionKeyword = currentConfig.DeleteSectionKeyword;
		StringComparison comparison = StringComparison.OrdinalIgnoreCase;
		string endSectionName = currentConfig.GetEndSectionName();
		if (currentConfig.HasEndSection)
		{
			return file =>
			{
				string newText = pvf.GetFileText(file, AppSetting.Instance.PvfConfig.DefaultEncoding);
				if (!string.IsNullOrEmpty(newText))
				{
					if (StrHelper.TraitFindReplceMain(newText, comparison, deleteSectionKeyword, endSectionName, string.Empty, out newText))
					{
						if (pvf.SaveFileText(file, newText))
						{
							successFiles.Add(file.FileName);
						}
						else
						{
							errorFiles.Add(file.FileName);
						}
					}
					else
					{
						errorFiles.Add(file.FileName);
					}
				}
				else
				{
					errorFiles.Add(file.FileName);
				}
			};
		}

		return file =>
		{
			string fileText = pvf.GetFileText(file, AppSetting.Instance.PvfConfig.DefaultEncoding);
			if (!string.IsNullOrEmpty(fileText))
			{
				string[] lines = fileText.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
				if (lines.Any())
				{
					List<string> lineList = lines.ToList();
					int sectionIndex = lineList.Select(it => it.Replace("\t", string.Empty)).ToList().IndexOf(deleteSectionKeyword);
					bool changed = false;
					while (sectionIndex != -1)
					{
						if (currentConfig.KeepDeleteSection)
						{
							if (sectionIndex + currentConfig.DeleteSectionLineNumber < lineList.Count)
							{
								lineList.RemoveRange(sectionIndex + 1, currentConfig.DeleteSectionLineNumber);
								changed = true;
							}
						}
						else if (sectionIndex + currentConfig.DeleteSectionLineNumber <= lineList.Count)
						{
							lineList.RemoveRange(sectionIndex, currentConfig.DeleteSectionLineNumber + 1);
							changed = true;
						}
						sectionIndex = sectionIndex + 1 < lineList.Count - 1
							? lineList.Select(it => it.Replace("\t", string.Empty)).ToList().IndexOf(deleteSectionKeyword, sectionIndex + 1)
							: -1;
					}
					if (changed)
					{
						if (pvf.SaveFileText(file, string.Join("\r\n", lineList)))
						{
							successFiles.Add(file.FileName);
						}
						else
						{
							errorFiles.Add(file.FileName);
						}
					}
					else
					{
						errorFiles.Add(file.FileName);
					}
				}
			}
		};
	}

	private Task<ResultData> RunDeleteSection()
	{
		ResultData resultData = new ResultData();
		try
		{
			Action<PvfFile> action = CreateDeleteSectionAction();
			foreach (PvfFile item in files)
			{
				action(item);
			}
		}
		catch (Exception ex)
		{
			resultData.Msg = ex.Message;
		}
		return Task.FromResult(resultData);
	}

	public Task<ResultData> AddContent()
	{
		ResultData resultData = new ResultData();
		try
		{
			Action<PvfFile> action = file =>
			{
				string fileText = pvf.GetFileText(file, AppSetting.Instance.PvfConfig.DefaultEncoding);
				pvf.SaveFileText(file, fileText + "\r\n" + currentConfig.AddContent);
			};
			foreach (PvfFile item in files)
			{
				action(item);
				successFiles.Add(item.FileName);
			}
		}
		catch (Exception ex)
		{
			resultData.Msg = ex.Message;
		}
		return Task.FromResult(resultData);
	}
}
