using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Collections.Pooled;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.Win32;
using PvfCode.ViewModels;
using PvfCode.ViewModels.TreeFolder;
using Utools;

namespace PvfCode.Views.BatchOperation;

public class WindowBatchOperationDetailsViewModel : ViewModelBase
{
	[CompilerGenerated]
	private PvfTreeViewModel WU2BA4pUJ0;

	[CompilerGenerated]
	private PvfTreeViewModel bqmB4W9dBo;

	[CompilerGenerated]
	private IEnumerable<string> OwBBYKP3Mw;

	[CompilerGenerated]
	private IEnumerable<string> eM8ByaRoc9;

	public PvfTreeViewModel SuccessTreeViewModel
	{
		[CompilerGenerated]
		get
		{
			return WU2BA4pUJ0;
		}
		[CompilerGenerated]
		set
		{
			WU2BA4pUJ0 = value;
		}
	}

	public PvfTreeViewModel ErrorTreeViewModel
	{
		[CompilerGenerated]
		get
		{
			return bqmB4W9dBo;
		}
		[CompilerGenerated]
		set
		{
			bqmB4W9dBo = value;
		}
	}

	[SpecialName]
	[CompilerGenerated]
	private IEnumerable<string> QX4BWUrTr0()
	{
		return OwBBYKP3Mw;
	}

	[SpecialName]
	[CompilerGenerated]
	private void FZJBmCnyp6(IEnumerable<string> P_0)
	{
		OwBBYKP3Mw = P_0;
	}

	[SpecialName]
	[CompilerGenerated]
	private IEnumerable<string> DkoBfmKIbc()
	{
		return eM8ByaRoc9;
	}

	[SpecialName]
	[CompilerGenerated]
	private void rydB5FDw1d(IEnumerable<string> P_0)
	{
		eM8ByaRoc9 = P_0;
	}

	public WindowBatchOperationDetailsViewModel(IEnumerable<string> successFiles, IEnumerable<string> errorFiles)
	{
		rydB5FDw1d(errorFiles);
		FZJBmCnyp6(successFiles);
		SuccessTreeViewModel = new PvfTreeViewModel(TreeViewType.BatchOperationLog);
		ErrorTreeViewModel = new PvfTreeViewModel(TreeViewType.BatchOperationLog);
	}

	public void Loaded()
	{
		SuccessTreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(QX4BWUrTr0()));
		ErrorTreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(DkoBfmKIbc()));
	}

	[Command]
	public async void ImportFilesForTxt(bool isError)
	{
		string fileName = "ErrorList_" + DateTime.Now.ToLogTime();
		IEnumerable<string> allFilePaths;
		if (isError)
		{
			allFilePaths = ErrorTreeViewModel.TreeGroupData.GetAllFilePaths();
		}
		else
		{
			allFilePaths = SuccessTreeViewModel.TreeGroupData.GetAllFilePaths();
			fileName = "SuccessList_" + DateTime.Now.ToLogTime();
		}
		if (allFilePaths.Any())
		{
			string contents = string.Join("\r\n", allFilePaths);
			string filter = AppSetting.Instance.GetIlogger()?.GetStr("WindowBatchOperationDetails_Title") + " (*.txt)|*.txt";
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = filter,
				FileName = fileName
			};
			bool? flag = saveFileDialog.ShowDialog();
			if (flag.HasValue && flag.Value)
			{
				await File.WriteAllTextAsync(saveFileDialog.FileName, contents);
			}
		}
	}

	public void Clear()
	{
		SuccessTreeViewModel.Clear();
		ErrorTreeViewModel.Clear();
		FZJBmCnyp6(null);
		rydB5FDw1d(null);
	}
}
