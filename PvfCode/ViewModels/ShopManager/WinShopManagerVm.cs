using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Views.PvfTreeFolder;
using Utools;

namespace PvfCode.ViewModels.ShopManager;

public class WinShopManagerVm : ViewModelBase
{
	private PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	[Command]
	public void OnPackageCodeSort()
	{
		try
		{
			string fileText = Pvf.GetFileText("etc/newcashshop.etc");
			if (!fileText.Contains("[package]"))
			{
				throw new Exception("商城中不存在礼包：[package]节点");
			}
			string[] array = StrHelper.Between(fileText, "[package]", "[/package]").Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
			if (array == null || !array.Any())
			{
				throw new Exception("没有可排序的内容");
			}
			int num = sOgWY0yXgP(ShopSectionType.package);
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split("\t", StringSplitOptions.RemoveEmptyEntries);
				array3[0] = num.ToString();
				stringBuilder.AppendLine(string.Join("\t", array3));
				num++;
			}
			if (!StrHelper.TraitFindReplceMain(fileText, StringComparison.Ordinal, "[package]", "[/package]", "[package]\r\n" + stringBuilder.ToString() + "\r\n[/package]", out var newText))
			{
				throw new Exception("替换内容失败");
			}
			AppCore.ViewModelBase.PVF.SaveFileText("etc/newcashshop.etc", newText);
			AppCore.ShowMsg("OK");
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg(ex.Message, isError: true);
		}
	}

	private string h5FW4h1IpM(ShopSectionType P_0)
	{
		return P_0.ToString() ?? "";
	}

	private int sOgWY0yXgP(ShopSectionType P_0)
	{
		string text = h5FW4h1IpM(P_0);
		int groupSize = (P_0 == ShopSectionType.package) ? 10 : 9;
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		if (!pVF.GetFile("etc/newcashshop.etc").GetSectionIntArray(pVF, "[" + text + "]", out List<int> items))
		{
			throw new Exception("未能获取到节点：[" + text + "]");
		}
		return items.Select((value, index) => new { Index = index, Value = value })
			.GroupBy(item => item.Index / groupSize)
			.Select(group => group.First().Value)
			.Max() + 1;
	}

	public WinShopManagerVm()
	{
	}
}
