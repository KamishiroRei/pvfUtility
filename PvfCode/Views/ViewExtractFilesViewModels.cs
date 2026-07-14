using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.WindowsAPICodePack.Dialogs;
using PvfCode.Dot;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.ExtractFilesModel;
using PvfCode.ViewModels;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Views;

public class ViewExtractFilesViewModels : ViewModelBase, IDisposable
{
	private readonly Action close;

	private IEnumerable<string> sourceFiles;

	private readonly PvfGroup pvf;

	private ViewExtractFiles view;

	public PvfTreeViewModel TreeViewModel { get; set; }

	public ExtractConfig Config => AppSetting.Instance.PvfConfig.ExtractConfig;

	public bool IsDiffExtract
	{
		get
		{
			return GetProperty(() => IsDiffExtract);
		}
		set
		{
			SetProperty(() => IsDiffExtract, value);
		}
	}

	public bool IsLoading
	{
		get
		{
			return GetProperty(() => IsLoading);
		}
		set
		{
			SetProperty(() => IsLoading, value);
		}
	}

	public ViewExtractFilesViewModels(Action close, IEnumerable<string> files = null)
	{
		pvf = AppCore.ViewModelBase.PVF;
		sourceFiles = files;
		this.close = close;
		TreeViewModel = new PvfTreeViewModel(TreeViewType.ExtractFiles);
		if (!Config.ExtractTo7zip && string.IsNullOrEmpty(Config.TargetPath))
		{
			Config.TargetPath = AppCore.ViewModelBase.PVF.PvfPackDefaultExtractDir;
		}
	}

	public ViewExtractFilesViewModels(Action close, PvfGroup pvf, IEnumerable<string> files = null)
	{
		IsDiffExtract = true;
		this.pvf = pvf;
		sourceFiles = files;
		this.close = close;
		TreeViewModel = new PvfTreeViewModel(TreeViewType.ExtractFiles);
		if (!Config.ExtractTo7zip && string.IsNullOrEmpty(Config.TargetPath))
		{
			Config.TargetPath = AppCore.ViewModelBase.PVF.PvfPackDefaultExtractDir;
		}
	}

	public async void Loaded(object sender)
	{
		view = (ViewExtractFiles)sender;
		view.SizeToContent = SizeToContent.Manual;
		if (sourceFiles != null && sourceFiles.Count() > 0)
		{
			IsLoading = true;
			await TreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(sourceFiles));
			IsLoading = false;
			sourceFiles = null;
		}
	}

	[Command]
	public async void SelectedFiles(bool isSearchResult)
	{
		IEnumerable<string> enumerable = AppCore.SelectPvfFileList(isSearchResult ? TreeViewType.SearchResult : TreeViewType.FileList);
		if (enumerable != null)
		{
			IsLoading = true;
			await TreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(enumerable), pvf);
			IsLoading = false;
		}
	}

	[Command]
	public void OnSelectTargetPath()
	{
		if (Config.ExtractTo7zip)
		{
			CommonSaveFileDialog commonSaveFileDialog = new CommonSaveFileDialog
			{
				Title = AppSetting.Instance.GetIlogger().GetStr("mess_Select7zFileSavePath")
			};
			commonSaveFileDialog.Filters.Add(new CommonFileDialogFilter(AppSetting.Instance.GetIlogger()?.GetStr("mess_7zFile"), ".7z"));
			commonSaveFileDialog.DefaultFileName = "Script.7z";
			if (commonSaveFileDialog.ShowDialog(view) == CommonFileDialogResult.Ok)
			{
				Config.TargetPath = commonSaveFileDialog.FileName;
			}
		}
		else
		{
			CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				Title = AppSetting.Instance.GetIlogger().GetStr("Title_SelectFolder")
			};
			if (commonOpenFileDialog.ShowDialog(view) == CommonFileDialogResult.Ok)
			{
				Config.TargetPath = commonOpenFileDialog.FileName;
			}
		}
	}

	[Command]
	public void OnClearAllFiles()
	{
		TreeViewModel.Clear();
	}

	[Command]
	public async void OnExtractStart()
	{
		Config.SourceFiles = TreeViewModel.TreeGroupData.GetAllFilePaths().ToHashSet();
		ResultData resultData = ValidateAndPrepareConfig();
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg, isError: true);
			return;
		}
		pvf.ExtractFiles(Config);
		await AppSetting.Instance.SaveSetting();
		close?.Invoke();
	}

	private ResultData ValidateAndPrepareConfig()
	{
		ResultData resultData = new ResultData();
		if (string.IsNullOrEmpty(Config.TargetPath))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_InvalidSavePath");
			return resultData;
		}
		if (Config.SourceFiles == null || Config.SourceFiles.Count == 0)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_PleaseSelectFileToExtract");
			return resultData;
		}
		if (Config.FileTypes != null && Config.FileTypes.Count > 0)
		{
			List<string> list = new List<string>();
			HashSet<string> hashSet = Config.FileTypes.ToHashSet();
			if (Config.RemoveOrKeepFileType == RemoveOrKeepFileType.保留)
			{
				foreach (string sourceFile in Config.SourceFiles)
				{
					if (hashSet.Contains(Path.GetExtension(sourceFile)))
					{
						list.Add(sourceFile);
					}
				}
			}
			else
			{
				foreach (string sourceFile2 in Config.SourceFiles)
				{
					if (!hashSet.Contains(Path.GetExtension(sourceFile2)))
					{
						list.Add(sourceFile2);
					}
				}
			}
			if (list.Count == 0)
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_NoFileCanBeExtractedAfterFilter");
				return resultData;
			}
			Config.SourceFiles = list.ToHashSet();
		}
		if (!Config.ExtractTo7zip)
		{
			string targetPath = Config.TargetPath;
			if (!Directory.Exists(targetPath))
			{
				try
				{
					Directory.CreateDirectory(targetPath);
				}
				catch (Exception ex)
				{
					resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ErrorWhenCreateFolder"), ex.Message);
					return resultData;
				}
			}
		}
		return resultData;
	}

	public void Dispose()
	{
		TreeViewModel.Clear();
	}
}
