using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass42_0
	{
		public PvfDiffToolViewModel zjnqkFYms1;

		public string AMyq00ByHo;

		public _003C_003Ec__DisplayClass42_0()
		{
		}

		internal Task<bool>? k6KqJYGnqK()
		{
			return zjnqkFYms1.Pvf.OpenPvfPack(AMyq00ByHo, AppCore.ViewModelBase.MainProgress);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass44_0
	{
		public IDictionary<string, List<PvfFileDiffType>?> ql0qXJZ3Vy;

		public PvfDiffToolViewModel AT2qpq0EiF;

		public IDictionary<string, List<PvfFileDiffType>?> N6kqUbsLeL;

		public _003C_003Ec__DisplayClass44_0()
		{
		}

		internal void uWtq7YuFUl()
		{
			ql0qXJZ3Vy = AT2qpq0EiF.YtXWM3pwVp(true);
			N6kqUbsLeL = AT2qpq0EiF.YtXWM3pwVp(false);
			AT2qpq0EiF.TreeViewModelLeft.ForbidVerticalScrollBarAnnotation = AT2qpq0EiF.LeftDiffCount > 20000;
			AT2qpq0EiF.TreeViewModelRight.ForbidVerticalScrollBarAnnotation = AT2qpq0EiF.RightDiffCount > 20000;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass57_0
	{
		public PvfGroup paCq87emy2;

		public PvfDiffToolViewModel dZnqM1HhSg;

		public _003C_003Ec__DisplayClass57_0()
		{
		}

		internal void RM5qc3CPWL(string item)
		{
			if (paCq87emy2.FileContentDiff(paCq87emy2.FileList[item], dZnqM1HhSg.Pvf, dZnqM1HhSg.Pvf.FileList[item], out List<PvfFileDiffType> diffs))
			{
				dZnqM1HhSg.lAjmhV4X6Y.TryAdd(item, diffs);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass63_0
	{
		public PvfGroup pvf;

		public string a4eq3XNPfI;

		public _003C_003Ec__DisplayClass63_0()
		{
		}

		internal Task<ResultData>? nsXqVOLIbi()
		{
			return pvf.SavePvfPack(a4eq3XNPfI, isFastMode: false, AppCore.ViewModelBase.MainProgress);
		}
	}

	[CompilerGenerated]
	private PvfGroup IydmvZvseq;

	[CompilerGenerated]
	private PvfTreeViewModel oA0mBlufql;

	[CompilerGenerated]
	private PvfTreeViewModel TGNmFU52tx;

	[CompilerGenerated]
	private List<PvfDiffTreeShowFilesType> nKEmr7TjRC;

	[CompilerGenerated]
	private Dictionary<string, List<PvfFileDiffType>?>? iJQmW6U9kB;

	[CompilerGenerated]
	private Dictionary<string, List<PvfFileDiffType>?>? jMFmmOSR7X;

	[CompilerGenerated]
	private ConcurrentDictionary<string, List<PvfFileDiffType>?>? eLlm26h84b;

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

	public PvfGroup Pvf
	{
		[CompilerGenerated]
		get
		{
			return IydmvZvseq;
		}
		[CompilerGenerated]
		set
		{
			IydmvZvseq = value;
		}
	}

	public PvfTreeViewModel TreeViewModelLeft
	{
		[CompilerGenerated]
		get
		{
			return oA0mBlufql;
		}
		[CompilerGenerated]
		set
		{
			oA0mBlufql = value;
		}
	}

	public PvfTreeViewModel TreeViewModelRight
	{
		[CompilerGenerated]
		get
		{
			return TGNmFU52tx;
		}
		[CompilerGenerated]
		set
		{
			TGNmFU52tx = value;
		}
	}

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
			SetProperty(() => ForbidVerticalScrollBarAnnotation, value, M3IWJR1jSo);
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
			SetProperty(() => PvfDiffTreeShowFilesType, value, dmrWk4So6a);
		}
	}

	public List<PvfDiffTreeShowFilesType> PvfDiffTreeShowFilesTypeslist
	{
		[CompilerGenerated]
		get
		{
			return nKEmr7TjRC;
		}
		[CompilerGenerated]
		set
		{
			nKEmr7TjRC = value;
		}
	}

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

	private Dictionary<string, List<PvfFileDiffType>?>? w28mD55mXu
	{
		[CompilerGenerated]
		get
		{
			return iJQmW6U9kB;
		}
		[CompilerGenerated]
		set
		{
			iJQmW6U9kB = value;
		}
	}

	private Dictionary<string, List<PvfFileDiffType>?>? zosmTlp0Bp
	{
		[CompilerGenerated]
		get
		{
			return jMFmmOSR7X;
		}
		[CompilerGenerated]
		set
		{
			jMFmmOSR7X = value;
		}
	}

	private ConcurrentDictionary<string, List<PvfFileDiffType>?>? lAjmhV4X6Y
	{
		[CompilerGenerated]
		get
		{
			return eLlm26h84b;
		}
		[CompilerGenerated]
		set
		{
			eLlm26h84b = value;
		}
	}

	private void M3IWJR1jSo()
	{
		TreeViewModelLeft.ForbidVerticalScrollBarAnnotation = ForbidVerticalScrollBarAnnotation;
		TreeViewModelRight.ForbidVerticalScrollBarAnnotation = ForbidVerticalScrollBarAnnotation;
	}

	private async void dmrWk4So6a()
	{
		IsLoading = true;
		WindowLoading win = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_Loading"), Application.Current.MainWindow);
		win.Show();
		await A2sWcQMOnq();
		win.Close();
		IsLoading = false;
	}

	public PvfDiffToolViewModel()
		: base(AppSetting.Instance.GetIlogger()?.GetStr("DocumentName_PvfDiff"))
	{
		base.DocumentType = PvfFileDocumentType.PVF差异比较器;
		Pvf = new PvfGroup();
		TreeViewModelLeft = new PvfTreeViewModel(TreeViewType.PvfDiffLeft);
		TreeViewModelLeft.SelectedRowChangedEvent += lsCWpLmlUq;
		TreeViewModelLeft.EventNodeDoubleClick += fx6WXKPqtR;
		TreeViewModelLeft.EventDiffExtractSelected += DBjW0ZjUq3;
		TreeViewModelRight = new PvfTreeViewModel(TreeViewType.PvfDiffRight);
		TreeViewModelRight.EventNodeDoubleClick += F8PW75uC7p;
		TreeViewModelRight.SelectedRowChangedEvent += fRfWUoniAe;
		TreeViewModelRight.EventDiffExtractSelected += DBjW0ZjUq3;
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

	private void DBjW0ZjUq3(TreeViewType P_0, IEnumerable<string> P_1)
	{
		ViewExtractFiles viewExtractFiles = new ViewExtractFiles((P_0 == TreeViewType.PvfDiffLeft) ? AppCore.ViewModelBase.PVF : Pvf, P_1);
		viewExtractFiles.Owner = Application.Current.MainWindow;
		viewExtractFiles.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		viewExtractFiles.Show();
	}

	private void F8PW75uC7p(KeyValuePair<string, PvfTreeFileBase> row)
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

	private void fx6WXKPqtR(KeyValuePair<string, PvfTreeFileBase> row)
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

	private void lsCWpLmlUq(KeyValuePair<string, PvfTreeFileBase> selectedRow)
	{
		if (SelectedSynchronization && TreeViewModelRight.TreeGroupData.Any(selectedRow.Value.FullPath))
		{
			TreeViewModelRight.GoToNode(selectedRow.Value.FullPath, showError: false);
		}
	}

	private void fRfWUoniAe(KeyValuePair<string, PvfTreeFileBase> selectedRow)
	{
		if (SelectedSynchronization && TreeViewModelLeft.TreeGroupData.Any(selectedRow.Value.FullPath))
		{
			TreeViewModelLeft.GoToNode(selectedRow.Value.FullPath, showError: false);
		}
	}

	[Command]
	public async void OnOpenPvf(string filePath)
	{
		_003C_003Ec__DisplayClass42_0 CS_0024_003C_003E8__locals10 = new _003C_003Ec__DisplayClass42_0();
		CS_0024_003C_003E8__locals10.zjnqkFYms1 = this;
		CS_0024_003C_003E8__locals10.AMyq00ByHo = filePath;
		if (string.IsNullOrEmpty(CS_0024_003C_003E8__locals10.AMyq00ByHo))
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
			CS_0024_003C_003E8__locals10.AMyq00ByHo = commonOpenFileDialog.FileName;
		}
		if (!File.Exists(CS_0024_003C_003E8__locals10.AMyq00ByHo))
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), CS_0024_003C_003E8__locals10.AMyq00ByHo), isError: true);
			if (AppSetting.Instance.PathConfig.PvfOpenLog.ContainsKey(CS_0024_003C_003E8__locals10.AMyq00ByHo))
			{
				AppSetting.Instance.PathConfig.PvfOpenLog.Remove(CS_0024_003C_003E8__locals10.AMyq00ByHo);
			}
		}
		else if (!(await Task.Run(() => CS_0024_003C_003E8__locals10.zjnqkFYms1.Pvf.OpenPvfPack(CS_0024_003C_003E8__locals10.AMyq00ByHo, AppCore.ViewModelBase.MainProgress))))
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
		await Task.Run((Func<Task?>)w9JW8fyFPs);
		await A2sWcQMOnq();
		win.Close();
	}

	private async Task A2sWcQMOnq()
	{
		_003C_003Ec__DisplayClass44_0 CS_0024_003C_003E8__locals13 = new _003C_003Ec__DisplayClass44_0();
		CS_0024_003C_003E8__locals13.AT2qpq0EiF = this;
		if (Pvf.PvfIsOpen)
		{
			TreeViewModelLeft.Clear();
			TreeViewModelRight.Clear();
			CS_0024_003C_003E8__locals13.ql0qXJZ3Vy = null;
			CS_0024_003C_003E8__locals13.N6kqUbsLeL = null;
			await Task.Run(delegate
			{
				CS_0024_003C_003E8__locals13.ql0qXJZ3Vy = CS_0024_003C_003E8__locals13.AT2qpq0EiF.YtXWM3pwVp(true);
				CS_0024_003C_003E8__locals13.N6kqUbsLeL = CS_0024_003C_003E8__locals13.AT2qpq0EiF.YtXWM3pwVp(false);
				CS_0024_003C_003E8__locals13.AT2qpq0EiF.TreeViewModelLeft.ForbidVerticalScrollBarAnnotation = CS_0024_003C_003E8__locals13.AT2qpq0EiF.LeftDiffCount > 20000;
				CS_0024_003C_003E8__locals13.AT2qpq0EiF.TreeViewModelRight.ForbidVerticalScrollBarAnnotation = CS_0024_003C_003E8__locals13.AT2qpq0EiF.RightDiffCount > 20000;
			});
			await TreeViewModelLeft.TreeGroupData.DiffCreateTrees(CS_0024_003C_003E8__locals13.ql0qXJZ3Vy, TreeViewType.PvfDiffLeft);
			await TreeViewModelRight.TreeGroupData.DiffCreateTrees(CS_0024_003C_003E8__locals13.N6kqUbsLeL, TreeViewType.PvfDiffRight);
		}
	}

	private Task w9JW8fyFPs()
	{
		_003C_003Ec__DisplayClass57_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass57_0();
		CS_0024_003C_003E8__locals7.dZnqM1HhSg = this;
		IsLoading = true;
		w28mD55mXu = new Dictionary<string, List<PvfFileDiffType>>();
		zosmTlp0Bp = new Dictionary<string, List<PvfFileDiffType>>();
		lAjmhV4X6Y = new ConcurrentDictionary<string, List<PvfFileDiffType>>();
		IEnumerable<string?> first = AppCore.ViewModelBase.PVF.FileList.Keys.DefaultIfEmpty();
		Dictionary<string, PvfFile>.KeyCollection keys = Pvf.FileList.Keys;
		IEnumerable<string> enumerable = first.Intersect<string>(keys);
		IEnumerable<string> source = first.Except<string>(enumerable);
		IEnumerable<string> source2 = keys.Except(enumerable);
		foreach (string item in source.ToHashSet())
		{
			w28mD55mXu.Add(item, new List<PvfFileDiffType> { PvfFileDiffType.FilePath });
		}
		foreach (string item2 in source2.ToHashSet())
		{
			zosmTlp0Bp.Add(item2, new List<PvfFileDiffType> { PvfFileDiffType.FilePath });
		}
		CS_0024_003C_003E8__locals7.paCq87emy2 = AppCore.ViewModelBase.PVF;
		Parallel.ForEach(enumerable.ToHashSet(), delegate(string item)
		{
			if (CS_0024_003C_003E8__locals7.paCq87emy2.FileContentDiff(CS_0024_003C_003E8__locals7.paCq87emy2.FileList[item], CS_0024_003C_003E8__locals7.dZnqM1HhSg.Pvf, CS_0024_003C_003E8__locals7.dZnqM1HhSg.Pvf.FileList[item], out List<PvfFileDiffType> diffs))
			{
				CS_0024_003C_003E8__locals7.dZnqM1HhSg.lAjmhV4X6Y.TryAdd(item, diffs);
			}
		});
		IsLoading = false;
		return Task.CompletedTask;
	}

	private IDictionary<string, List<PvfFileDiffType>?> YtXWM3pwVp(bool P_0)
	{
		if (w28mD55mXu == null)
		{
			return null;
		}
		Dictionary<string, PvfFile>.KeyCollection keyCollection = (P_0 ? AppCore.ViewModelBase.PVF.FileList.Keys : Pvf.FileList.Keys);
		ConcurrentDictionary<string, List<PvfFileDiffType>> concurrentDictionary = new ConcurrentDictionary<string, List<PvfFileDiffType>>();
		switch (PvfDiffTreeShowFilesType)
		{
		case PvfDiffTreeShowFilesType.路径差异:
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(P_0 ? w28mD55mXu : zosmTlp0Bp);
			if (P_0)
			{
				LeftDiffCount = concurrentDictionary.Count;
			}
			else
			{
				RightDiffCount = concurrentDictionary.Count;
			}
			break;
		case PvfDiffTreeShowFilesType.文件内容差异:
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(lAjmhV4X6Y);
			if (P_0)
			{
				LeftDiffCount = concurrentDictionary.Count;
			}
			else
			{
				RightDiffCount = concurrentDictionary.Count;
			}
			break;
		case PvfDiffTreeShowFilesType.路径差异和文件差异:
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(P_0 ? w28mD55mXu : zosmTlp0Bp);
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(lAjmhV4X6Y);
			if (P_0)
			{
				LeftDiffCount = concurrentDictionary.Count;
			}
			else
			{
				RightDiffCount = concurrentDictionary.Count;
			}
			break;
		case PvfDiffTreeShowFilesType.所有文件:
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(P_0 ? w28mD55mXu : zosmTlp0Bp);
			concurrentDictionary.AddRange<KeyValuePair<string, List<PvfFileDiffType>>>(lAjmhV4X6Y);
			if (P_0)
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
		lAjmhV4X6Y = null;
		w28mD55mXu = null;
		zosmTlp0Bp = null;
		Pvf.Clear();
		TreeViewModelLeft.Clear();
		TreeViewModelRight.Clear();
		await Task.Run((Func<Task?>)FbAW30ZQhf);
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
		await W4dWVD8Ffc(pvfGroup.PvfPackFilePath, pvfGroup);
	}

	private async Task W4dWVD8Ffc(string P_0, PvfGroup P_1)
	{
		_003C_003Ec__DisplayClass63_0 obj = new _003C_003Ec__DisplayClass63_0();
		obj.pvf = P_1;
		obj.a4eq3XNPfI = P_0;
		ResultData resultData = await Task.Run(() => obj.pvf.SavePvfPack(obj.a4eq3XNPfI, isFastMode: false, AppCore.ViewModelBase.MainProgress));
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
			await W4dWVD8Ffc(saveFileDialog.FileName, pvfGroup);
		}
	}

	private Task FbAW30ZQhf()
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
				if (string.IsNullOrEmpty(itemName) || !xngWRuU7cy(itemName))
				{
					list.Add(file);
				}
			}
		}
		foreach (PvfFile item2 in list)
		{
			string itemName2 = Pvf.GetItemName(item2.FileName);
			if (!string.IsNullOrEmpty(itemName2) && xngWRuU7cy(itemName2))
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
				if (!string.IsNullOrEmpty(itemName3) && !xngWRuU7cy(itemName3))
				{
					list2.Add(file2);
				}
			}
		}
		foreach (PvfFile item4 in list2)
		{
			string itemName4 = Pvf.GetItemName(item4.FileName);
			if (string.IsNullOrEmpty(itemName4) || !xngWRuU7cy(itemName4))
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

	private static bool xngWRuU7cy(string P_0)
	{
		return Regex.IsMatch(P_0, "\\p{IsCJKUnifiedIdeographs}");
	}

	public override void Dispose()
	{
		Clear(showDialog: false);
		TreeViewModelLeft.SelectedRowChangedEvent -= lsCWpLmlUq;
		TreeViewModelLeft.EventNodeDoubleClick -= fx6WXKPqtR;
		TreeViewModelLeft.EventDiffExtractSelected += DBjW0ZjUq3;
		TreeViewModelRight.EventNodeDoubleClick -= F8PW75uC7p;
		TreeViewModelRight.SelectedRowChangedEvent -= fRfWUoniAe;
		TreeViewModelRight.EventDiffExtractSelected -= DBjW0ZjUq3;
	}

	~PvfDiffToolViewModel()
	{
		Dispose();
	}
}
