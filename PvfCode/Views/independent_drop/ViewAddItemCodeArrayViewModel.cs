using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using PvfCode.LoggerBase;
using PvfCode.ViewModels.independent_drop.DropList;

namespace PvfCode.Views.independent_drop;

public class ViewAddItemCodeArrayViewModel : ViewModelBase
{
	public string Str
	{
		get
		{
			return GetProperty(() => Str);
		}
		set
		{
			SetProperty<string>(() => Str, value);
			YesButtonIsEnable = !string.IsNullOrEmpty(value);
		}
	}

	public int DropWeight
	{
		get
		{
			return GetProperty(() => DropWeight);
		}
		set
		{
			SetProperty(() => DropWeight, value);
		}
	}

	public bool YesButtonIsEnable
	{
		get
		{
			return GetProperty(() => YesButtonIsEnable);
		}
		set
		{
			SetProperty(() => YesButtonIsEnable, value);
		}
	}

	public bool CheckItemCodeAny
	{
		get
		{
			return GetProperty(() => CheckItemCodeAny);
		}
		set
		{
			SetProperty(() => CheckItemCodeAny, value);
		}
	}

	public bool RemoveRepeat
	{
		get
		{
			return GetProperty(() => RemoveRepeat);
		}
		set
		{
			SetProperty(() => RemoveRepeat, value);
		}
	}

	public ViewAddItemCodeArrayViewModel()
	{
		DropWeight = 1000;
		RemoveRepeat = true;
		CheckItemCodeAny = true;
		Str = "100300011\t100300012\t100300013\t100300014\t100300015";
	}

	private List<ListItem> BuildItems(out string errorMessage)
	{
		errorMessage = null;
		StringBuilder stringBuilder = new StringBuilder();
		List<ListItem> list = new List<ListItem>();
		IEnumerable<string> enumerable = Str.Split("\t", StringSplitOptions.RemoveEmptyEntries);
		if (RemoveRepeat)
		{
			enumerable = enumerable.ToHashSet();
		}
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		List<string> lstNames = new List<string>
		{
			"equipment",
			"stackable"
		};
		foreach (string item in enumerable)
		{
			if (!int.TryParse(item, out var result))
			{
				stringBuilder.AppendLine($"错误：{item}非数字");
			}
			else if (CheckItemCodeAny)
			{
				if (pVF.ListFileTable.ItemCodeConvertFilePath(lstNames, result) == null)
				{
					stringBuilder.AppendLine($"错误：{item} 在装备和道具中找不到该代码");
				}
				else
				{
					list.Add(new ListItem
					{
						ItemCode = result,
						DropWeight = DropWeight
					});
				}
			}
			else
			{
				list.Add(new ListItem
				{
					ItemCode = result,
					DropWeight = DropWeight
				});
			}
		}
		errorMessage = stringBuilder.ToString();
		return list;
	}

	[Command]
	public void OnYes(ViewAddItemCodeArray win)
	{
		List<ListItem> list = BuildItems(out string errorMessage);
		if (list.Count > 0)
		{
			win.XHEhKC1k16 = list;
			win.DialogResult = true;
		}
		if (!string.IsNullOrEmpty(errorMessage))
		{
			DelegateCommand<string> command = new DelegateCommand<string>(_ => ShowError(errorMessage));
			AppCore.Logger.ShowNotification(new NotificationViewModel<string>(AppSetting.Instance.AppName, "批量添加独立掉落时有一些错误您应当阅读", AppSetting.Instance.GetRes()?.VisualStudioBlendLogo2015Pre_16x, "详情", command));
		}
	}

	private void ShowError(string err)
	{
		AppCore.ShowDefaultScriptEditorWindow(new ViewScriptEditorViewModel("批量添加独立掉落代码错误信息", err));
	}

	[Command]
	public void OnCancel(ThemedWindow win)
	{
		win.DialogResult = false;
	}
}
