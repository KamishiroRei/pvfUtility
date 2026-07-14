using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PvfCode.Dot;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Models.Pvf.Enums.Stackable;
using PvfCode.Models.Pvf.PvfFileDiffModels.Enums;
using PvfCode.ViewModels.Diff;
using PvfCode.ViewModels.DocumentFolder;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.ViewModels.PvfDiffTool.Enums;
using PvfCode.ViewModels.TreeFolder;
using PvfCode.Views;
using Utools;

namespace PvfCode.ViewModels.PvfDiffTool;

public class PvfDiffToolViewModel : DocumentBase
{
	private Dictionary<string, List<PvfFileDiffType>?>? leftPathDiffs;

	private Dictionary<string, List<PvfFileDiffType>?>? rightPathDiffs;

	private ConcurrentDictionary<string, List<PvfFileDiffType>?>? contentDiffs;

	public new bool IsLoading
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

	public PvfGroup Pvf { get; set; }

	public PvfTreeViewModel TreeViewModelLeft { get; set; }

	public PvfTreeViewModel TreeViewModelRight { get; set; }

	public bool SelectedSynchronization
	{
		get
		{
			return GetProperty(() => SelectedSynchronization);
		}
		set
		{
			SetProperty(() => SelectedSynchronization, value);
		}
	}

	public bool ForbidVerticalScrollBarAnnotation
	{
		get
		{
			return GetProperty(() => ForbidVerticalScrollBarAnnotation);
		}
		set
		{
			SetProperty(() => ForbidVerticalScrollBarAnnotation, value, UpdateScrollBarAnnotations);
		}
	}

	public PvfDiffTreeShowFilesType PvfDiffTreeShowFilesType
	{
		get
		{
			return GetProperty(() => PvfDiffTreeShowFilesType);
		}
		set
		{
			SetProperty(() => PvfDiffTreeShowFilesType, value, OnShowFilesTypeChanged);
		}
	}

	public List<PvfDiffTreeShowFilesType> PvfDiffTreeShowFilesTypeslist { get; set; }

	public int LeftDiffCount
	{
		get
		{
			return GetProperty(() => LeftDiffCount);
		}
		set
		{
			SetProperty(() => LeftDiffCount, value);
		}
	}

	public int RightDiffCount
	{
		get
		{
			return GetProperty(() => RightDiffCount);
		}
		set
		{
			SetProperty(() => RightDiffCount, value);
		}
	}

	private void UpdateScrollBarAnnotations()
	{
		TreeViewModelLeft.ForbidVerticalScrollBarAnnotation = ForbidVerticalScrollBarAnnotation;
		TreeViewModelRight.ForbidVerticalScrollBarAnnotation = ForbidVerticalScrollBarAnnotation;
	}

	private async void OnShowFilesTypeChanged()
	{
		IsLoading = true;
		WindowLoading win = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_Loading"), Application.Current.MainWindow);
		win.Show();
		await RefreshTreesAsync();
		win.Close();
		IsLoading = false;
	}

	public PvfDiffToolViewModel()
		: base(AppSetting.Instance.GetIlogger()?.GetStr("DocumentName_PvfDiff"))
	{
		base.DocumentType = PvfFileDocumentType.PVF差异比较器;
		Pvf = new PvfGroup();
		TreeViewModelLeft = new PvfTreeViewModel(TreeViewType.PvfDiffLeft);
		TreeViewModelLeft.SelectedRowChangedEvent += OnLeftSelectedRowChanged;
		TreeViewModelLeft.EventNodeDoubleClick += OnLeftNodeDoubleClick;
		TreeViewModelLeft.EventDiffExtractSelected += ExtractSelectedFiles;
		TreeViewModelRight = new PvfTreeViewModel(TreeViewType.PvfDiffRight);
		TreeViewModelRight.EventNodeDoubleClick += OnRightNodeDoubleClick;
		TreeViewModelRight.SelectedRowChangedEvent += OnRightSelectedRowChanged;
		TreeViewModelRight.EventDiffExtractSelected += ExtractSelectedFiles;
		PvfDiffTreeShowFilesType = PvfDiffTreeShowFilesType.所有文件;
		PvfDiffTreeShowFilesTypeslist = new List<PvfDiffTreeShowFilesType>();
		foreach (EnumberEntity item in EnumberHelper.EnumToList<PvfDiffTreeShowFilesType>())
		{
			PvfDiffTreeShowFilesTypeslist.Add((PvfDiffTreeShowFilesType)item.EnumValue);
		}
		AppCore.Logger.Warning(AppSetting.Instance.GetIlogger()?.GetStr("PvfDiff_ToolTip1"));
		AppCore.Logger.Warning(AppSetting.Instance.GetIlogger()?.GetStr("PvfDiff_ToolTip2"));
		AppCore.Logger.Warning(AppSetting.Instance.GetIlogger()?.GetStr("PvfDiff_ToolTip3"));
	}

	private void ExtractSelectedFiles(TreeViewType treeViewType, IEnumerable<string> filePaths)
	{
		ViewExtractFiles viewExtractFiles = new ViewExtractFiles((treeViewType == TreeViewType.PvfDiffLeft) ? AppCore.ViewModelBase.PVF : Pvf, filePaths);
		viewExtractFiles.Owner = Application.Current.MainWindow;
		viewExtractFiles.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		viewExtractFiles.Show();
	}

	private void OnRightNodeDoubleClick(KeyValuePair<string, PvfTreeFileBase> row)
	{
		if (row.Value.IsFile)
		{
			DiffSource leftSource = null;
			if (AppCore.ViewModelBase.PVF.FileAny(row.Value.FullPath))
			{
				leftSource = new DiffSource(AppCore.ViewModelBase.PVF, row.Value.FullPath);
			}
			DiffSource rightSource = new DiffSource(Pvf, row.Value.FullPath);
			AppCore.ViewModelBase.BarsVm.OpenDiffWindow(leftSource, rightSource);
		}
	}

	private void OnLeftNodeDoubleClick(KeyValuePair<string, PvfTreeFileBase> row)
	{
		if (row.Value.IsFile)
		{
			DiffSource rightSource = null;
			if (Pvf.FileAny(row.Value.FullPath))
			{
				rightSource = new DiffSource(Pvf, row.Value.FullPath);
			}
			DiffSource leftSource = new DiffSource(AppCore.ViewModelBase.PVF, row.Value.FullPath);
			AppCore.ViewModelBase.BarsVm.OpenDiffWindow(leftSource, rightSource);
		}
	}

	private void OnLeftSelectedRowChanged(KeyValuePair<string, PvfTreeFileBase> selectedRow)
	{
		if (SelectedSynchronization && TreeViewModelRight.TreeGroupData.Any(selectedRow.Value.FullPath))
		{
			TreeViewModelRight.GoToNode(selectedRow.Value.FullPath, showError: false);
		}
	}

	private void OnRightSelectedRowChanged(KeyValuePair<string, PvfTreeFileBase> selectedRow)
	{
		if (SelectedSynchronization && TreeViewModelLeft.TreeGroupData.Any(selectedRow.Value.FullPath))
		{
			TreeViewModelLeft.GoToNode(selectedRow.Value.FullPath, showError: false);
		}
	}

	[Command]
	public async void OnOpenPvf(string filePath)
	{
		if (string.IsNullOrEmpty(filePath))
		{
			CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
			{
				Title = AppSetting.Instance.GetIlogger().GetStr("OpenPvfPackDialog_Title"),
				IsFolderPicker = false,
				Multiselect = false,
				AllowPropertyEditing = true,
				EnsurePathExists = true,
				EnsureValidNames = true
			};
			commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter(AppSetting.Instance.GetIlogger()?.GetStr("SavePvfPackFileDialogFilterName"), ".pvf"));
			if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok)
			{
				return;
			}
			filePath = commonOpenFileDialog.FileName;
		}
		if (!File.Exists(filePath))
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), filePath), isError: true);
			if (AppSetting.Instance.PathConfig.PvfOpenLog.ContainsKey(filePath))
			{
				AppSetting.Instance.PathConfig.PvfOpenLog.Remove(filePath);
			}
		}
		else if (!(await Task.Run(() => Pvf.OpenPvfPack(filePath, AppCore.ViewModelBase.MainProgress))))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_CannotOpenPvfPack"), isError: true);
		}
		else
		{
			Pvf.PvfIsOpen = true;
		}
	}

	[Command]
	public async void OnStartPvfDiff()
	{
		if (!AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseLoadGlobalPvfPackFirst"));
			return;
		}
		WindowLoading win = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_PvfDiffing"), Application.Current.MainWindow);
		win.Show();
		await Task.Run(CalculateDiffsAsync);
		await RefreshTreesAsync();
		win.Close();
	}

	private async Task RefreshTreesAsync()
	{
		if (Pvf.PvfIsOpen)
		{
			TreeViewModelLeft.Clear();
			TreeViewModelRight.Clear();
			IDictionary<string, List<PvfFileDiffType>?> leftFiles = null;
			IDictionary<string, List<PvfFileDiffType>?> rightFiles = null;
			await Task.Run(() =>
			{
				leftFiles = GetVisibleFiles(true);
				rightFiles = GetVisibleFiles(false);
				TreeViewModelLeft.ForbidVerticalScrollBarAnnotation = LeftDiffCount > 20000;
				TreeViewModelRight.ForbidVerticalScrollBarAnnotation = RightDiffCount > 20000;
			});
			await TreeViewModelLeft.TreeGroupData.DiffCreateTrees(leftFiles, TreeViewType.PvfDiffLeft);
			await TreeViewModelRight.TreeGroupData.DiffCreateTrees(rightFiles, TreeViewType.PvfDiffRight);
		}
	}

	private Task CalculateDiffsAsync()
	{
		IsLoading = true;
		leftPathDiffs = new Dictionary<string, List<PvfFileDiffType>>();
		rightPathDiffs = new Dictionary<string, List<PvfFileDiffType>>();
		contentDiffs = new ConcurrentDictionary<string, List<PvfFileDiffType>>();
		IEnumerable<string?> first = AppCore.ViewModelBase.PVF.FileList.Keys.DefaultIfEmpty();
		Dictionary<string, PvfFile>.KeyCollection keys = Pvf.FileList.Keys;
		IEnumerable<string> enumerable = first.Intersect<string>(keys);
		IEnumerable<string> source = first.Except<string>(enumerable);
		IEnumerable<string> source2 = keys.Except(enumerable);
		foreach (string item in source.ToHashSet())
		{
			leftPathDiffs.Add(item, new List<PvfFileDiffType> { PvfFileDiffType.FilePath });
		}
		foreach (string item2 in source2.ToHashSet())
		{
			rightPathDiffs.Add(item2, new List<PvfFileDiffType> { PvfFileDiffType.FilePath });
		}
		PvfGroup mainPvf = AppCore.ViewModelBase.PVF;
		Parallel.ForEach(enumerable.ToHashSet(), item =>
		{
			if (mainPvf.FileContentDiff(mainPvf.FileList[item], Pvf, Pvf.FileList[item], out List<PvfFileDiffType> diffs))
			{
				contentDiffs.TryAdd(item, diffs);
			}
		});
		IsLoading = false;
		return Task.CompletedTask;
	}

	private IDictionary<string, List<PvfFileDiffType>?> GetVisibleFiles(bool isLeft)
	{
		if (leftPathDiffs == null)
		{
			return null;
		}
		Dictionary<string, PvfFile>.KeyCollection keyCollection = (isLeft ? AppCore.ViewModelBase.PVF.FileList.Keys : Pvf.FileList.Keys);
		ConcurrentDictionary<string, List<PvfFileDiffType>> concurrentDictionary = new ConcurrentDictionary<string, List<PvfFileDiffType>>();
		switch (PvfDiffTreeShowFilesType)
		{
		case PvfDiffTreeShowFilesType.路径差异:
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(isLeft ? leftPathDiffs : rightPathDiffs);
			if (isLeft)
			{
				LeftDiffCount = concurrentDictionary.Count;
			}
			else
			{
				RightDiffCount = concurrentDictionary.Count;
			}
			break;
		case PvfDiffTreeShowFilesType.文件内容差异:
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(contentDiffs);
			if (isLeft)
			{
				LeftDiffCount = concurrentDictionary.Count;
			}
			else
			{
				RightDiffCount = concurrentDictionary.Count;
			}
			break;
		case PvfDiffTreeShowFilesType.路径差异和文件差异:
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(isLeft ? leftPathDiffs : rightPathDiffs);
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(contentDiffs);
			if (isLeft)
			{
				LeftDiffCount = concurrentDictionary.Count;
			}
			else
			{
				RightDiffCount = concurrentDictionary.Count;
			}
			break;
		case PvfDiffTreeShowFilesType.所有文件:
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(isLeft ? leftPathDiffs : rightPathDiffs);
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(contentDiffs);
			if (isLeft)
			{
				LeftDiffCount = concurrentDictionary.Count;
			}
			else
			{
				RightDiffCount = concurrentDictionary.Count;
			}
			foreach (string item in keyCollection)
			{
				if (!concurrentDictionary.ContainsKey(item))
				{
					concurrentDictionary.TryAdd(item, null);
				}
			}
			break;
		}
		return concurrentDictionary;
	}

	[Command]
	public void OnExtract(bool isLeft)
	{
		IEnumerable<string> enumerable = (isLeft ? TreeViewModelLeft.GetSelectedFilePaths(GetTreeType.File) : TreeViewModelRight.GetSelectedFilePaths(GetTreeType.File));
		if (enumerable == null || !enumerable.Any())
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectFileFirst"), isError: true);
			return;
		}
		ViewExtractFiles viewExtractFiles = new ViewExtractFiles(isLeft ? AppCore.ViewModelBase.PVF : Pvf, enumerable);
		viewExtractFiles.Owner = Application.Current.MainWindow;
		viewExtractFiles.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		viewExtractFiles.Show();
	}

	[Command]
	public async void Clear(bool showDialog = true)
	{
		IsLoading = true;
		Window win = null;
		if (showDialog)
		{
			win = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_CleaningGarbage"), Application.Current.MainWindow);
			win.Show();
		}
		contentDiffs = null;
		leftPathDiffs = null;
		rightPathDiffs = null;
		Pvf.Clear();
		TreeViewModelLeft.Clear();
		TreeViewModelRight.Clear();
		await Task.Run(ClearMemoryAsync);
		if (showDialog)
		{
			win?.Close();
		}
		IsLoading = false;
	}

	[Command]
	public void OnGotoFilePath(bool isLeft)
	{
		PvfGroup pvfGroup = (isLeft ? AppCore.ViewModelBase.PVF : Pvf);
		if (pvfGroup != null && pvfGroup.PvfIsOpen)
		{
			FileHelper.OpenFolderAndSelectFile(pvfGroup.PvfPackFilePath);
		}
	}

	[Command]
	public async void SavePvfPack(bool isLeft)
	{
		PvfGroup pvfGroup = (isLeft ? AppCore.ViewModelBase.PVF : Pvf);
		await SavePvfPackAsync(pvfGroup.PvfPackFilePath, pvfGroup);
	}

	private async Task SavePvfPackAsync(string filePath, PvfGroup pvf)
	{
		ResultData resultData = await Task.Run(() => pvf.SavePvfPack(filePath, isFastMode: false, AppCore.ViewModelBase.MainProgress));
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg);
		}
	}

	[Command]
	public async void SaveAsPvfPack(object[] obj)
	{
		bool num = Convert.ToBoolean(obj[1]);
		string filter = AppSetting.Instance.GetIlogger()?.GetStr("SavePvfPackFileDialogFilterName") + " (*.pvf)|*.pvf";
		PvfGroup pvfGroup = (num ? AppCore.ViewModelBase.PVF : Pvf);
		SaveFileDialog saveFileDialog = new SaveFileDialog
		{
			Filter = filter,
			FileName = Path.GetFileName(pvfGroup.PvfPackFilePath),
			CheckFileExists = false
		};
		bool? flag = saveFileDialog.ShowDialog(Application.Current.MainWindow);
		if (flag.HasValue && flag.Value)
		{
			await SavePvfPackAsync(saveFileDialog.FileName, pvfGroup);
		}
	}

	private Task ClearMemoryAsync()
	{
		WindowsEx.ClearMemorySilent(Process.GetCurrentProcess());
		return Task.CompletedTask;
	}

	[Command]
	public void ConverterPvfDiffShowMode()
	{
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		IEnumerable<string> files = pVF.GetFiles("stackable");
		List<PvfFile> list = new List<PvfFile>();
		foreach (string item in files)
		{
			PvfFile file = pVF.GetFile(item);
			if (file.GetStackableType(pVF, out var type) && type == StackableType.消耗品_点卷礼包_0)
			{
				string itemName = pVF.GetItemName(file);
				if (string.IsNullOrEmpty(itemName) || !ContainsChinese(itemName))
				{
					list.Add(file);
				}
			}
		}
		foreach (PvfFile item2 in list)
		{
			string itemName2 = Pvf.GetItemName(item2.FileName);
			if (!string.IsNullOrEmpty(itemName2) && ContainsChinese(itemName2))
			{
				string fileText = pVF.GetFileText(item2);
				if (!string.IsNullOrEmpty(fileText))
				{
					fileText = ((!fileText.Contains("[name]")) ? fileText.Insert(0, "[name]\r\n" + itemName2 + "\r\n") : Regex.Replace(fileText, "(?<=\\[name\\]\\s*`).*?(?=`)", itemName2));
					pVF.SaveFileText(item2, fileText);
				}
			}
		}
		IEnumerable<string> files2 = pVF.GetFiles("equipment");
		List<PvfFile> list2 = new List<PvfFile>();
		foreach (string item3 in files2)
		{
			PvfFile file2 = pVF.GetFile(item3);
			if (!file2.GetEquipmentType(pVF, out var re))
			{
				continue;
			}
			EquipmentType value = re.Value;
			if (value == EquipmentType.称号 || (uint)(value - 12) <= 1u || (uint)(value - 17) <= 9u)
			{
				string itemName3 = pVF.GetItemName(file2);
				if (!string.IsNullOrEmpty(itemName3) && !ContainsChinese(itemName3))
				{
					list2.Add(file2);
				}
			}
		}
		foreach (PvfFile item4 in list2)
		{
			string itemName4 = Pvf.GetItemName(item4.FileName);
			if (string.IsNullOrEmpty(itemName4) || !ContainsChinese(itemName4))
			{
				continue;
			}
			string fileText2 = pVF.GetFileText(item4);
			if (!string.IsNullOrEmpty(fileText2))
			{
				if (fileText2.Contains("[name]"))
				{
					fileText2 = Regex.Replace(fileText2, "(?<=\\[name\\]\\s*`).*?(?=`)", itemName4);
					pVF.SaveFileText(item4, fileText2);
				}
				else
				{
					fileText2 = fileText2.Insert(0, "[name]\r\n" + itemName4 + "\r\n");
				}
				pVF.SaveFileText(item4, fileText2);
			}
		}
	}

	private static bool ContainsChinese(string text)
	{
		return Regex.IsMatch(text, "\\p{IsCJKUnifiedIdeographs}");
	}

	public override void Dispose()
	{
		Clear(showDialog: false);
		TreeViewModelLeft.SelectedRowChangedEvent -= OnLeftSelectedRowChanged;
		TreeViewModelLeft.EventNodeDoubleClick -= OnLeftNodeDoubleClick;
		TreeViewModelLeft.EventDiffExtractSelected += ExtractSelectedFiles;
		TreeViewModelRight.EventNodeDoubleClick -= OnRightNodeDoubleClick;
		TreeViewModelRight.SelectedRowChangedEvent -= OnRightSelectedRowChanged;
		TreeViewModelRight.EventDiffExtractSelected -= ExtractSelectedFiles;
	}

	~PvfDiffToolViewModel()
	{
		Dispose();
	}
}
