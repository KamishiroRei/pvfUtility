using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.BatchOperation;
using PvfCode.Models.Macro;

namespace PvfCode;

public class MacroHelper : ViewModelBase
{
	private static MacroHelper _instance;

	public static MacroHelper Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new MacroHelper();
			}
			return _instance;
		}
	}

	[Command]
	public void OnRun(KeyValuePair<string, MacroData> data)
	{
		switch (data.Value.MacroType)
		{
		case MacroType.全局搜索:
			RunGlobalSearchMacro(data);
			break;
		case MacroType.批量处理:
			RunBatchOperationMacro(data);
			break;
		}
	}

	private async void RunGlobalSearchMacro(KeyValuePair<string, MacroData> macro)
	{
		WindowLoading loading = AppCore.CreateLoading(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExecutingMacro"), macro.Key), Application.Current.MainWindow);
		loading.Show();
		try
		{
			await Task.Run(async () =>
			{
				await AppCore.ViewModelBase.SearchResultViewModel.SearchUiViewModel.MacroSearchTask(macro);
			});
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg(ex.Message, isError: true);
		}
		loading.Close();
	}

	private async void RunBatchOperationMacro(KeyValuePair<string, MacroData> macro)
	{
		MacroData value = macro.Value;
		if (AppCore.Logger.ShowDialog(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExecutingBatchProcessMacro"), macro.Key)) != MessageResult.Yes)
		{
			return;
		}
		HashSet<string> hashSet = AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.TreeGroupData.GetAllFilePaths().ToHashSet();
		if (hashSet == null || hashSet.Count == 0)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_SearchResultIsEmpty"));
			return;
		}
		List<BatchOperationConfig> batchOperationMacro = value.GetBatchOperationMacro();
		foreach (BatchOperationConfig item in batchOperationMacro)
		{
			item.SourceFiles = hashSet;
		}
		WindowLoading loading = AppCore.CreateLoading(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExecutingBatchProcessMacro2"), macro.Key), Application.Current.MainWindow);
		loading.Show();
		ResultData resultData = await AppCore.ViewModelBase.PVF.BatchOperation(batchOperationMacro, AppCore.ShowBatchOperationDetailsCommand);
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg, isError: true);
		}
		loading.Close();
	}

	public MacroHelper()
	{
	}
}
