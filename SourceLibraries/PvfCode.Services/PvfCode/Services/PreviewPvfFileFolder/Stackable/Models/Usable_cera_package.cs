using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

public class Usable_cera_package : ViewModelBase
{
	private readonly PvfGroup pvf;

	private readonly PvfFile file;

	public List<package_data> Items { get; set; }

	public Usable_cera_package(List<int> datas, PvfFile file, PvfGroup pvf)
	{
		this.pvf = pvf;
		this.file = file;
		if (datas == null || datas.Count == 0 || datas.Count % 2 != 0)
		{
			AppSetting.Instance.GetIlogger().Error("数据长度不正确 [package data] file://" + file.FileName);
			return;
		}
		Items = new List<package_data>();
		for (int i = 0; i < datas.Count; i += 2)
		{
			Items.Add(new package_data(datas[i], datas[i + 1], pvf));
		}
	}

	[Command]
	public void OnOpenAllFiles()
	{
		List<package_data> items = Items;
		if (items == null || !items.Any())
		{
			return;
		}
		List<PvfFile> files = new List<PvfFile> { file };
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		foreach (package_data item in Items)
		{
			PvfFile itemFile = item.GetFile();
			if (itemFile == null)
			{
				ilogger.Error($"找不到对应文件 代码：{item.ItemCode}");
			}
			else
			{
				files.Add(itemFile);
			}
		}
		if (files.Any())
		{
			ilogger.AddFileListToNewSearchPanel(files.Select(item => item.FileName), pvf.GetItemName(file));
		}
	}
}
