using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Collections.Pooled;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.LoggerBase;
using PvfCode.ViewModels.DocumentFolder;
using PvfCode.Views;

namespace PvfCode.ViewModels.TreeFolder.Drop;

internal class LstFileDropHandler : DropDocumentModelBase
{
	public override async void Drop(PooledSet<string> fileList, TextEditorBase editor, PvfFileDocument viewModel)
	{
		if (fileList == null || fileList.Count == 0)
		{
			AppCore.Logger.Warning(AppSetting.Instance.GetIlogger()?.GetStr("mess_DeliverFail_NoFile"));
			return;
		}

		List<PvfFile> files = AppCore.ViewModelBase.PVF.GetFiles(fileList);
		if (files == null || files.Count == 0)
		{
			AppCore.Logger.Warning(AppSetting.Instance.GetIlogger()?.GetStr("mess_DeliverFail_NoFile"));
			return;
		}

		PvfGroup pvf = AppCore.ViewModelBase.PVF;
		PvfFile lstFile = viewModel.File;
		string lstFileName = lstFile.FileName;
		PooledDictionary<int, string> registeredPathsById = new PooledDictionary<int, string>();
		PooledDictionary<string, int> registeredIdsByPath = new PooledDictionary<string, int>();
		string lstPathHeader = lstFile.GetLstPathHeader();
		string documentText = viewModel.Document.Text;
		if (!string.IsNullOrEmpty(documentText))
		{
			string[] lines = documentText.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in lines)
			{
				string[] columns = line.Split("\t", StringSplitOptions.RemoveEmptyEntries);
				if (columns.Length == 2 && int.TryParse(columns[0], out int itemId))
				{
					string fullPath = lstPathHeader + "/" + columns[1].Replace("`", string.Empty).ToLower();
					if (!registeredPathsById.ContainsKey(itemId))
					{
						registeredPathsById.Add(itemId, fullPath);
					}
					if (!registeredIdsByPath.ContainsKey(fullPath))
					{
						registeredIdsByPath.Add(fullPath, itemId);
					}
				}
			}
		}

		if (!AppSetting.Instance.PvfConfig.LstExtensions.ContainsKey(lstFileName))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_DeliverFail_NotSupport"));
			return;
		}

		PvfFileType expectedFileType = AppSetting.Instance.PvfConfig.LstExtensions[lstFileName];
		PooledList<string> duplicateFiles = new PooledList<string>();
		PooledList<string> mismatchedFiles = new PooledList<string>();
		StringBuilder appendedLines = new StringBuilder();
		int maxItemId = pvf.ListFileTable.GetLstNumMax(lstFileName);
		int relativePathStart = lstPathHeader.Length + 1;
		int successCount = 0;

		foreach (PvfFile file in files)
		{
			string filePath = file.FileName;
			if (file.FileType != expectedFileType)
			{
				mismatchedFiles.Add(filePath);
				continue;
			}
			if (registeredIdsByPath.ContainsKey(filePath))
			{
				duplicateFiles.Add(filePath);
				continue;
			}

			int itemId;
			if (int.TryParse(Path.GetFileNameWithoutExtension(filePath), out int parsedId) &&
				!registeredPathsById.ContainsKey(parsedId))
			{
				itemId = parsedId;
			}
			else
			{
				maxItemId++;
				itemId = maxItemId;
			}

			string relativePath = filePath.Substring(relativePathStart, filePath.Length - relativePathStart);
			appendedLines.AppendLine($"{itemId}\t`{relativePath}`");
			successCount++;
		}

		pvf.ListFileTable.SaveLstNumMax(lstFileName, maxItemId);
		if (appendedLines.Length > 0)
		{
			DocumentAppend(appendedLines, editor);
		}

		StringBuilder summary = new StringBuilder(
			string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SuccessCount"), successCount));
		if (duplicateFiles.Count > 0)
		{
			summary.Append(string.Format(
				AppSetting.Instance.GetIlogger()?.GetStr("mess_DuplicateCount"),
				duplicateFiles.Count));
		}
		if (mismatchedFiles.Count > 0)
		{
			summary.Append(string.Format(
				AppSetting.Instance.GetIlogger()?.GetStr("mess_NotMatchCount"),
				mismatchedFiles.Count));
		}

		AppCore.Logger.Success(summary.ToString());
		await AppCore.Logger.ShowNotification(new NotificationViewModel(
			AppSetting.Instance.GetIlogger()?.GetStr("mess_DeliverResult"),
			summary.ToString(),
			Res.Instance.VisualStudioBlendLogo2015Pre_16x,
			AppSetting.Instance.GetIlogger()?.GetStr("mess_Detail"),
			() => ShowDropDetails(appendedLines, mismatchedFiles, duplicateFiles)));
	}

	public void ShowDropDetails(
		StringBuilder appendedLines,
		PooledList<string> mismatchedFiles,
		PooledList<string> duplicateFiles)
	{
		StringBuilder details = new StringBuilder();
		details.AppendLine(string.Format(
			AppSetting.Instance.GetIlogger()?.GetStr("mess_RegisterSuccess"),
			appendedLines));
		details.AppendLine(string.Format(
			AppSetting.Instance.GetIlogger()?.GetStr("mess_FileTypeNotMatch"),
			string.Join("\r\n", mismatchedFiles)));
		details.AppendLine(string.Format(
			AppSetting.Instance.GetIlogger()?.GetStr("mess_HaveAlreadyRegistered"),
			string.Join("\r\n", duplicateFiles)));
		AppCore.ShowDefaultScriptEditorWindow(new ViewScriptEditorViewModel(
			AppSetting.Instance.GetIlogger()?.GetStr("mess_DeliverResult"),
			details.ToString())
		{
			IsReadOnly = true
		});
	}
}
