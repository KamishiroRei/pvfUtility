using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

public class Usable_cera_package : ViewModelBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public Ilogger kbgyZJ5602;

		public List<PvfFile> Cv1yhrq0l2;

		public _003C_003Ec__DisplayClass7_0()
		{
		}

		internal void FGoyTmnCjE(package_data it)
		{
			PvfFile file = it.GetFile();
			if (file == null)
			{
				Ilogger ilogger = kbgyZJ5602;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
				defaultInterpolatedStringHandler.AppendLiteral("找不到对应文件 代码：");
				defaultInterpolatedStringHandler.AppendFormatted(it.ItemCode);
				ilogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				Cv1yhrq0l2.Add(file);
			}
		}
	}

	[CompilerGenerated]
	private List<package_data> pCljgMaIua;

	private readonly PvfGroup brcjzDvagU;

	private readonly PvfFile BWnMuRAh66;

	public List<package_data> Items
	{
		[CompilerGenerated]
		get
		{
			return pCljgMaIua;
		}
		[CompilerGenerated]
		set
		{
			pCljgMaIua = value;
		}
	}

	public Usable_cera_package(List<int> datas, PvfFile file, PvfGroup pvf)
	{
		brcjzDvagU = pvf;
		BWnMuRAh66 = file;
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
		_003C_003Ec__DisplayClass7_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass7_0();
		CS_0024_003C_003E8__locals7.Cv1yhrq0l2 = new List<PvfFile> { BWnMuRAh66 };
		CS_0024_003C_003E8__locals7.kbgyZJ5602 = AppSetting.Instance.GetIlogger();
		Items.ForEach(delegate(package_data it)
		{
			PvfFile file = it.GetFile();
			if (file == null)
			{
				Ilogger ilogger = CS_0024_003C_003E8__locals7.kbgyZJ5602;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
				defaultInterpolatedStringHandler.AppendLiteral("找不到对应文件 代码：");
				defaultInterpolatedStringHandler.AppendFormatted(it.ItemCode);
				ilogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				CS_0024_003C_003E8__locals7.Cv1yhrq0l2.Add(file);
			}
		});
		if (CS_0024_003C_003E8__locals7.Cv1yhrq0l2.Any())
		{
			CS_0024_003C_003E8__locals7.kbgyZJ5602.AddFileListToNewSearchPanel(CS_0024_003C_003E8__locals7.Cv1yhrq0l2.Select((PvfFile it) => it.FileName), brcjzDvagU.GetItemName(BWnMuRAh66));
		}
	}
}
