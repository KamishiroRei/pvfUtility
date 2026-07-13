using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using PvfCode.ViewModels;

namespace PvfCode.Views.PvfTreeFolder;

public class CreateShopItemViewModel : ViewModelBase
{
	private readonly List<string> selectedFilePaths;

	public int Price
	{
		get
		{
			return GetProperty(() => Price);
		}
		set
		{
			SetProperty(() => Price, value);
		}
	}

	public int ItemCount
	{
		get
		{
			return GetProperty(() => ItemCount);
		}
		set
		{
			SetProperty(() => ItemCount, value);
		}
	}

	public Visibility ItemCountVisibility
	{
		get
		{
			if (Type == ShopSectionType.package)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}
	}

	public ShopSectionType Type
	{
		get
		{
			return GetProperty(() => Type);
		}
		set
		{
			SetProperty(() => Type, value);
			RaisePropertyChanged("ItemCountVisibility");
		}
	}

	public CreateShopItemViewModel(List<string> files)
	{
		selectedFilePaths = files;
	}

	[Command]
	public async void OnOk(Window win)
	{
		List<PvfFile> files = AppCore.ViewModelBase.PVF.GetFiles(selectedFilePaths);
		StringBuilder stringBuilder = new StringBuilder();
		int rowNumber = GetNextRowNumber(Type);
		int generatedRowCount = 0;
		foreach (PvfFile file in files)
		{
			rowNumber++;
			if (file.ItemCode.HasValue)
			{
				if (Type == ShopSectionType.package)
				{
					stringBuilder.AppendLine($"{rowNumber}\t{file.ItemCode}\t0\t0\t{Price}\t``\t4\t0\t-1\t-1");
				}
				else
				{
					stringBuilder.AppendLine($"{rowNumber}\t{file.ItemCode}\t{ItemCount}\t0\t0\t{Price}\t``\t0\t0");
				}
				generatedRowCount++;
			}
		}
		string fileText = AppCore.ViewModelBase.PVF.GetFileText("etc/newcashshop.etc");
		string sectionEndTag = "[/" + GetSectionName() + "]";
		if (!fileText.Contains(sectionEndTag))
		{
			throw new Exception("未能找到结束标签：" + sectionEndTag);
		}
		string updatedFileText = fileText.Insert(fileText.LastIndexOf(sectionEndTag), Environment.NewLine + stringBuilder.ToString() + Environment.NewLine);
		AppCore.ViewModelBase.PVF.SaveFileText("etc/newcashshop.etc", updatedFileText);
		LoggerViewModel logger = AppCore.Logger;
		string appName = AppSetting.Instance.AppName;
		logger.ShowNotification(new NotificationViewModel(appName, $"已生成：{generatedRowCount}行。", Res.Instance.VisualStudioBlendLogo2015Pre_16x));
		win?.Close();
	}

	private string GetSectionName()
	{
		return Type.ToString() ?? "";
	}

	private int GetNextRowNumber(ShopSectionType sectionType)
	{
		string sectionName = GetSectionName();
		int groupSize = sectionType == ShopSectionType.package ? 10 : 9;
		PvfGroup pvf = AppCore.ViewModelBase.PVF;
		if (!pvf.GetFile("etc/newcashshop.etc").GetSectionIntArray(pvf, "[" + sectionName + "]", out List<int> items))
		{
			throw new Exception("未能获取到节点：[" + sectionName + "]");
		}
		return items.Select((value, index) => new { Index = index, Value = value })
			.GroupBy(item => item.Index / groupSize)
			.SelectMany(group => group.Select(item => item.Value))
			.Max() + 1;
	}
}
