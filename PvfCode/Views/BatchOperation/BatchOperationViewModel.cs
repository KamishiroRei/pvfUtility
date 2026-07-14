using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Dot;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.BatchOperation;
using PvfCode.Models.BatchOperation.Enums;
using PvfCode.Models.Macro;
using PvfCode.ViewModels;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Views.BatchOperation;

public class BatchOperationViewModel : ViewModelBase
{
	public bool RecordingLoading
	{
		get
		{
			return GetProperty(() => RecordingLoading);
		}
		set
		{
			SetProperty(() => RecordingLoading, value);
		}
	}

	public IHighlightingDefinition Highlighting { get; set; }

	public BatchOperationConfig Config { get; set; }

	public PvfTreeViewModel TreeViewModel { get; set; }

	public TreeFilesSourceType FilesSourceType
	{
		get
		{
			return GetProperty(() => FilesSourceType);
		}
		set
		{
			SetProperty(() => FilesSourceType, value);
		}
	}

	public List<TreeFilesSourceType> TreeFilesSourceTypeList => new List<TreeFilesSourceType>
	{
		TreeFilesSourceType.所有待处理文件,
		TreeFilesSourceType.选中文件
	};

	private List<BatchOperationConfig>? RecordedBatchOperationConfigs { get; set; }

	private bool IncludeSourceFilesInRecordedMacro { get; set; }

	public BatchOperationViewModel()
	{
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
		Config = new BatchOperationConfig();
		TreeViewModel = new PvfTreeViewModel(TreeViewType.BatchOperation);
	}

	public async void AddFiles(IEnumerable<string> fileList)
	{
		if (fileList != null)
		{
			await TreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(fileList));
		}
	}

	[Command]
	public async void SelectedFiles(bool isSearchResult)
	{
		IEnumerable<string> enumerable = AppCore.SelectPvfFileList(isSearchResult ? TreeViewType.SearchResult : TreeViewType.FileList);
		if (enumerable != null)
		{
			await TreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(enumerable));
		}
	}

	[Command]
	public void OnClearAllFilesCommand()
	{
		TreeViewModel.Clear();
	}

	[Command]
	public async void OnStart()
	{
		switch (Config.BatchOperationType)
		{
		case BatchOperationType.DefaultFindReplace:
			if (string.IsNullOrEmpty(Config.FindKeyword))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputSearchContent"));
				return;
			}
			break;
		case BatchOperationType.TraitFindReplce:
			if (string.IsNullOrEmpty(Config.FindStartKeyword))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputSearchContent"));
				return;
			}
			break;
		case BatchOperationType.AddContent:
			if (string.IsNullOrEmpty(Config.AddContent))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputAppendContent"));
				return;
			}
			break;
		case BatchOperationType.DeleteSection:
			if (string.IsNullOrEmpty(Config.DeleteSectionKeyword))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputTag"));
				return;
			}
			if (Config.HasEndSection)
			{
				ResultData resultData = Config.HasSection();
				if (resultData.IsError)
				{
					AppCore.ShowMsg(resultData.Msg);
					return;
				}
			}
			break;
		}
		IEnumerable<string> fileList = GetSourceFiles();
		if (fileList == null)
		{
			return;
		}
		Config.SourceFiles = fileList.ToHashSet();
		TreeViewModel.TreeGroupData.Loading = true;
		ResultData<IEnumerable<string>> resultData2 = await AppCore.ViewModelBase.PVF.BatchOperation(Config, AppCore.ShowBatchOperationDetailsCommand);
		if (resultData2.IsError)
		{
			AppCore.ShowMsg(resultData2.Msg, isError: true);
		}
		else
		{
			if (RecordingLoading)
			{
				BatchOperationConfig batchOperationConfig = Config.CloneData();
				if (IncludeSourceFilesInRecordedMacro)
				{
					batchOperationConfig.SourceFiles = fileList.ToHashSet();
				}
				else
				{
					batchOperationConfig.SourceFiles = null;
				}
				RecordedBatchOperationConfigs.Add(batchOperationConfig);
			}
			if (resultData2.Data != null && resultData2.Data.Any())
			{
				IEnumerable<string> data = resultData2.Data;
				List<KeyValuePair<string, PvfTreeFileBase>> rows = AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(data);
				NotifyFileNamesChanged(rows);
				rows = AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.FilePathGetTreeNode(data);
				NotifyFileNamesChanged(rows);
				rows = TreeViewModel.TreeGroupData.FilePathGetTreeNode(data);
				NotifyFileNamesChanged(rows);
			}
		}
		TreeViewModel.TreeGroupData.Loading = false;
	}

	private void NotifyFileNamesChanged(List<KeyValuePair<string, PvfTreeFileBase>> rows)
	{
		if (rows == null)
		{
			return;
		}
		foreach (KeyValuePair<string, PvfTreeFileBase> row in rows)
		{
			row.Value.FileNameDoNotify();
		}
	}

	[Command]
	public async void SelectedMacro(KeyValuePair<string, MacroData> row)
	{
		IEnumerable<string> enumerable = GetSourceFiles();
		if (enumerable == null || AppCore.Logger.ShowDialog(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExecuteMacroForWaitFile"), row.Key)) != MessageResult.Yes)
		{
			return;
		}
		TreeViewModel.TreeGroupData.Loading = true;
		List<BatchOperationConfig> batchOperationMacro = row.Value.GetBatchOperationMacro();
		foreach (BatchOperationConfig item in batchOperationMacro)
		{
			item.SourceFiles = enumerable.ToHashSet();
		}
		ResultData resultData = await AppCore.ViewModelBase.PVF.BatchOperation(batchOperationMacro, AppCore.ShowBatchOperationDetailsCommand);
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg, isError: true);
		}
		TreeViewModel.TreeGroupData.Loading = false;
	}

	private IEnumerable<string> GetSourceFiles()
	{
		if (TreeViewModel.TreeGroupData.Trees == null || !TreeViewModel.TreeGroupData.Trees.Any())
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseAddFileToBatchProcess"));
			return null;
		}
		if (FilesSourceType == TreeFilesSourceType.所有待处理文件)
		{
			List<string> allFilePaths = TreeViewModel.TreeGroupData.GetAllFilePaths();
			if (allFilePaths == null || allFilePaths.Count == 0)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseAddFileToBatchProcess"));
				return null;
			}
			return allFilePaths;
		}
		PooledSet<string> selectedFilePaths = TreeViewModel.GetSelectedFilePaths(GetTreeType.File);
		if (selectedFilePaths == null || selectedFilePaths.Count == 0)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFileToBatchProcess"));
			return null;
		}
		return selectedFilePaths;
	}

	[Command]
	public async void SetRecordingLoading(bool isLoading)
	{
		if (isLoading)
		{
			RecordedBatchOperationConfigs = new List<BatchOperationConfig>();
			IncludeSourceFilesInRecordedMacro = false;
		}
		else if (RecordingLoading && RecordedBatchOperationConfigs != null && RecordedBatchOperationConfigs.Count > 0)
		{
			MacroData macroData = new MacroData();
			macroData.SetData(RecordedBatchOperationConfigs);
			macroData.MacroType = MacroType.批量处理;
			await AppCore.SaveMacroData(macroData, AppSetting.Instance.GetIlogger()?.GetStr("mess_NewMacro"), Application.Current.MainWindow);
		}
		RecordingLoading = isLoading;
	}
}
