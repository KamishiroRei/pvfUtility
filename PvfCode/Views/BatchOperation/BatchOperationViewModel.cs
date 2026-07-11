using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private IHighlightingDefinition ewXBT7ICa8;

	[CompilerGenerated]
	private BatchOperationConfig WQuBCRFkvb;

	[CompilerGenerated]
	private PvfTreeViewModel VKgBHYsRtj;

	[CompilerGenerated]
	private List<BatchOperationConfig>? Ix1Bh2fi1b;

	[CompilerGenerated]
	private bool nb1BvfbCB2;

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

	public IHighlightingDefinition Highlighting
	{
		[CompilerGenerated]
		get
		{
			return ewXBT7ICa8;
		}
		[CompilerGenerated]
		set
		{
			ewXBT7ICa8 = value;
		}
	}

	public BatchOperationConfig Config
	{
		[CompilerGenerated]
		get
		{
			return WQuBCRFkvb;
		}
		[CompilerGenerated]
		set
		{
			WQuBCRFkvb = value;
		}
	}

	public PvfTreeViewModel TreeViewModel
	{
		[CompilerGenerated]
		get
		{
			return VKgBHYsRtj;
		}
		[CompilerGenerated]
		set
		{
			VKgBHYsRtj = value;
		}
	}

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

	private List<BatchOperationConfig>? rVPvzqZIDb
	{
		[CompilerGenerated]
		get
		{
			return Ix1Bh2fi1b;
		}
		[CompilerGenerated]
		set
		{
			Ix1Bh2fi1b = value;
		}
	}

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
		IEnumerable<string> fileList = ONsv3WO3ih();
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
				if (RjoBD9PpQY())
				{
					batchOperationConfig.SourceFiles = fileList.ToHashSet();
				}
				else
				{
					batchOperationConfig.SourceFiles = null;
				}
				rVPvzqZIDb.Add(batchOperationConfig);
			}
			if (resultData2.Data != null && resultData2.Data.Any())
			{
				IEnumerable<string> data = resultData2.Data;
				List<KeyValuePair<string, PvfTreeFileBase>> rows = AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(data);
				wmUvVBBLW3(rows);
				rows = AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.FilePathGetTreeNode(data);
				wmUvVBBLW3(rows);
				rows = TreeViewModel.TreeGroupData.FilePathGetTreeNode(data);
				wmUvVBBLW3(rows);
			}
		}
		TreeViewModel.TreeGroupData.Loading = false;
	}

	private void wmUvVBBLW3(List<KeyValuePair<string, PvfTreeFileBase>> rows)
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
		IEnumerable<string> enumerable = ONsv3WO3ih();
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

	private IEnumerable<string> ONsv3WO3ih()
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

	[SpecialName]
	[CompilerGenerated]
	private bool RjoBD9PpQY()
	{
		return nb1BvfbCB2;
	}

	[SpecialName]
	[CompilerGenerated]
	private void PcHBlA1D24(bool P_0)
	{
		nb1BvfbCB2 = P_0;
	}

	[Command]
	public async void SetRecordingLoading(bool isLoading)
	{
		if (isLoading)
		{
			rVPvzqZIDb = new List<BatchOperationConfig>();
			PcHBlA1D24(false);
		}
		else if (RecordingLoading && rVPvzqZIDb != null && rVPvzqZIDb.Count > 0)
		{
			MacroData macroData = new MacroData();
			macroData.SetData(rVPvzqZIDb);
			macroData.MacroType = MacroType.批量处理;
			await AppCore.SaveMacroData(macroData, AppSetting.Instance.GetIlogger()?.GetStr("mess_NewMacro"), Application.Current.MainWindow);
		}
		RecordingLoading = isLoading;
	}
}
