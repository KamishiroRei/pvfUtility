using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private PvfTreeViewModel Vo1TzFD0wG;

	private readonly Action Close;

	private IEnumerable<string> BlECDmMFDV;

	private readonly PvfGroup Pvf;

	private ViewExtractFiles DuqClb754w;

	public PvfTreeViewModel TreeViewModel
	{
		[CompilerGenerated]
		get
		{
			return Vo1TzFD0wG;
		}
		[CompilerGenerated]
		set
		{
			Vo1TzFD0wG = value;
		}
	}

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
		Pvf = AppCore.ViewModelBase.PVF;
		BlECDmMFDV = files;
		Close = close;
		TreeViewModel = new PvfTreeViewModel(TreeViewType.ExtractFiles);
		if (!Config.ExtractTo7zip && string.IsNullOrEmpty(Config.TargetPath))
		{
			Config.TargetPath = AppCore.ViewModelBase.PVF.PvfPackDefaultExtractDir;
		}
	}

	public ViewExtractFilesViewModels(Action close, PvfGroup pvf, IEnumerable<string> files = null)
	{
		IsDiffExtract = true;
		Pvf = pvf;
		BlECDmMFDV = files;
		Close = close;
		TreeViewModel = new PvfTreeViewModel(TreeViewType.ExtractFiles);
		if (!Config.ExtractTo7zip && string.IsNullOrEmpty(Config.TargetPath))
		{
			Config.TargetPath = AppCore.ViewModelBase.PVF.PvfPackDefaultExtractDir;
		}
	}

	public async void Loaded(object sender)
	{
		DuqClb754w = (ViewExtractFiles)sender;
		DuqClb754w.SizeToContent = SizeToContent.Manual;
		if (BlECDmMFDV != null && BlECDmMFDV.Count() > 0)
		{
			IsLoading = true;
			await TreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(BlECDmMFDV));
			IsLoading = false;
			BlECDmMFDV = null;
		}
	}

	[Command]
	public async void SelectedFiles(bool isSearchResult)
	{
		IEnumerable<string> enumerable = AppCore.SelectPvfFileList(isSearchResult ? TreeViewType.SearchResult : TreeViewType.FileList);
		if (enumerable != null)
		{
			IsLoading = true;
			await TreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(enumerable), Pvf);
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
			if (commonSaveFileDialog.ShowDialog(DuqClb754w) == CommonFileDialogResult.Ok)
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
			if (commonOpenFileDialog.ShowDialog(DuqClb754w) == CommonFileDialogResult.Ok)
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
		ResultData resultData = Qe0TNIcdvu();
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg, isError: true);
			return;
		}
		Pvf.ExtractFiles(Config);
		await AppSetting.Instance.SaveSetting();
		Close?.Invoke();
	}

	private ResultData Qe0TNIcdvu()
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
			string text = Config.TargetPath;
			if (Config.ExtractTo7zip)
			{
				text = Path.GetDirectoryName(Config.TargetPath);
				if (text == null)
				{
					text = "";
				}
			}
			if (!Directory.Exists(text))
			{
				try
				{
					Directory.CreateDirectory(text);
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
