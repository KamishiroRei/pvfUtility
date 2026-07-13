using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	private IEnumerable<string> successFiles;

	private IEnumerable<string> errorFiles;

	public PvfTreeViewModel SuccessTreeViewModel { get; set; }

	public PvfTreeViewModel ErrorTreeViewModel { get; set; }

	public WindowBatchOperationDetailsViewModel(IEnumerable<string> successFiles, IEnumerable<string> errorFiles)
	{
		this.errorFiles = errorFiles;
		this.successFiles = successFiles;
		SuccessTreeViewModel = new PvfTreeViewModel(TreeViewType.BatchOperationLog);
		ErrorTreeViewModel = new PvfTreeViewModel(TreeViewType.BatchOperationLog);
	}

	public void Loaded()
	{
		SuccessTreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(successFiles));
		ErrorTreeViewModel.TreeGroupData.CreateTrees(new PooledList<string>(errorFiles));
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
		successFiles = null;
		errorFiles = null;
	}
}
