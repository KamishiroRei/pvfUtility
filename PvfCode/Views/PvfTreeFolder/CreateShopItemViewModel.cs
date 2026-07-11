using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using PvfCode.ViewModels;

namespace PvfCode.Views.PvfTreeFolder;

public class CreateShopItemViewModel : ViewModelBase
{
	private readonly List<string> fqQHFBIb17;

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
		fqQHFBIb17 = files;
	}

	[Command]
	public async void OnOk(Window win)
	{
		List<PvfFile> files = AppCore.ViewModelBase.PVF.GetFiles(fqQHFBIb17);
		StringBuilder stringBuilder = new StringBuilder();
		int num = eUXHBmsWrF(Type);
		int num2 = 0;
		foreach (PvfFile item in files)
		{
			num++;
			if (item.ItemCode.HasValue)
			{
				if (Type == ShopSectionType.package)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder3 = stringBuilder2;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(19, 3, stringBuilder2);
					handler.AppendFormatted(num);
					handler.AppendLiteral("\t");
					handler.AppendFormatted(item.ItemCode);
					handler.AppendLiteral("\t0\t0\t");
					handler.AppendFormatted(Price);
					handler.AppendLiteral("\t``\t4\t0\t-1\t-1");
					stringBuilder3.AppendLine(ref handler);
				}
				else
				{
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder4 = stringBuilder2;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(14, 4, stringBuilder2);
					handler.AppendFormatted(num);
					handler.AppendLiteral("\t");
					handler.AppendFormatted(item.ItemCode);
					handler.AppendLiteral("\t");
					handler.AppendFormatted(ItemCount);
					handler.AppendLiteral("\t0\t0\t");
					handler.AppendFormatted(Price);
					handler.AppendLiteral("\t``\t0\t0");
					stringBuilder4.AppendLine(ref handler);
				}
				num2++;
			}
		}
		string fileText = AppCore.ViewModelBase.PVF.GetFileText("etc/newcashshop.etc");
		string text = "[/" + ubTHvU1dLT() + "]";
		if (!fileText.Contains(text))
		{
			throw new Exception("未能找到结束标签：" + text);
		}
		string fileText2 = fileText.Insert(fileText.LastIndexOf(text), Environment.NewLine + stringBuilder.ToString() + Environment.NewLine);
		AppCore.ViewModelBase.PVF.SaveFileText("etc/newcashshop.etc", fileText2);
		LoggerViewModel logger = AppCore.Logger;
		string appName = AppSetting.Instance.AppName;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
		defaultInterpolatedStringHandler.AppendLiteral("已生成：");
		defaultInterpolatedStringHandler.AppendFormatted(num2);
		defaultInterpolatedStringHandler.AppendLiteral("行。");
		logger.ShowNotification(new NotificationViewModel(appName, defaultInterpolatedStringHandler.ToStringAndClear(), Res.Instance.VisualStudioBlendLogo2015Pre_16x));
		win?.Close();
	}

	private string ubTHvU1dLT()
	{
		_ = Type;
		return Type.ToString() ?? "";
	}

	private int eUXHBmsWrF(ShopSectionType P_0)
	{
		string text = ubTHvU1dLT();
		int groupSize = (P_0 == ShopSectionType.package) ? 10 : 9;
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		if (!pVF.GetFile("etc/newcashshop.etc").GetSectionIntArray(pVF, "[" + text + "]", out List<int> items))
		{
			throw new Exception("未能获取到节点：[" + text + "]");
		}
		return items.Select((value, index) => new { Index = index, Value = value })
			.GroupBy(item => item.Index / groupSize)
			.SelectMany(group => group.Select(item => item.Value))
			.Max() + 1;
	}
}
