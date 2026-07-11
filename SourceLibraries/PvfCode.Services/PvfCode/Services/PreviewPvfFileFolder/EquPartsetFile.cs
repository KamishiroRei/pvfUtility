using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf;
using PvfCode.Services.PvfParsingNew;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class EquPartsetFile : ViewModelBase
{
	private readonly PvfFile gly8J8fd2p;

	private readonly PvfGroup C6W8rFxEWR;

	[CompilerGenerated]
	private List<PieceSetAbility> Ro88nfNoT8;

	[CompilerGenerated]
	private Dictionary<string, EquipmentPartSet> POQ8RxJJgL;

	[CompilerGenerated]
	private string Aq78SDjjbl;

	public List<PieceSetAbility> Items
	{
		[CompilerGenerated]
		get
		{
			return Ro88nfNoT8;
		}
		[CompilerGenerated]
		set
		{
			Ro88nfNoT8 = value;
		}
	}

	public string? ItemsText
	{
		get
		{
			if (Items == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			_ = Items.Count;
			foreach (PieceSetAbility item in Items)
			{
				num++;
				stringBuilder.AppendLine(item.Text);
			}
			return stringBuilder.ToString()?.Replace("\\n", "\r\n");
		}
	}

	public Dictionary<string, EquipmentPartSet> EquItems
	{
		[CompilerGenerated]
		get
		{
			return POQ8RxJJgL;
		}
		[CompilerGenerated]
		set
		{
			POQ8RxJJgL = value;
		}
	}

	public string Name
	{
		[CompilerGenerated]
		get
		{
			return Aq78SDjjbl;
		}
		[CompilerGenerated]
		set
		{
			Aq78SDjjbl = value;
		}
	}

	public EquPartsetFile(PvfFile file, PvfGroup pvf, Dictionary<string, EquipmentPartSet> equItems)
	{
		if (equItems != null)
		{
			foreach (KeyValuePair<string, EquipmentPartSet> equItem in equItems)
			{
				equItem.Value.SetReferencesFilePack(pvf);
			}
		}
		EquItems = equItems;
		RaisePropertyChanged("EquItems");
		gly8J8fd2p = file;
		C6W8rFxEWR = pvf;
		if (gly8J8fd2p != null)
		{
			Name = pvf.GetItemName(file);
			Q3D8NuTk1q();
		}
	}

	private void Q3D8NuTk1q()
	{
		ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(gly8J8fd2p, C6W8rFxEWR);
		scriptFileParserNew.PraseStructureMain();
		if (scriptFileParserNew.Sections.Count == 0)
		{
			return;
		}
		IEnumerable<SectionBase> enumerable = scriptFileParserNew.Sections.Where((SectionBase it) => it.GetSectionName() == "[piece set ability]");
		if (enumerable == null)
		{
			return;
		}
		Items = new List<PieceSetAbility>();
		foreach (PvfSection item in enumerable)
		{
			Items.Add(new PieceSetAbility(scriptFileParserNew, item.Children, C6W8rFxEWR));
		}
	}

	[Command]
	public void OnOpenFile(EquipmentPartSet equipmentPartSet)
	{
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		if (equipmentPartSet == null || equipmentPartSet.GetFile() == null)
		{
			ilogger.ShowMsg("文件不存在", isError: true);
			return;
		}
		bool flag = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
		IEnumerable<string> filePaths = equipmentPartSet.GetFilePaths();
		if (filePaths == null)
		{
			return;
		}
		if (flag)
		{
			ilogger.AddFileListToCurrentSearchPanel(filePaths);
			return;
		}
		if (filePaths.Count() > 1)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
			defaultInterpolatedStringHandler.AppendLiteral("当前套装位置共有");
			defaultInterpolatedStringHandler.AppendFormatted(filePaths.Count());
			defaultInterpolatedStringHandler.AppendLiteral("件装备 大于10件不建议同时用文档打开 确定要打开吗");
			if (ilogger.ShowDialog(defaultInterpolatedStringHandler.ToStringAndClear()) != MessageResult.Yes)
			{
				return;
			}
		}
		foreach (string item in filePaths)
		{
			ilogger.OpenPvfFileDocument(item);
		}
	}

	[Command]
	public void OnOpenPartFile()
	{
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
		{
			if (gly8J8fd2p == null)
			{
				ilogger.ShowMsg("套装信息文件不存在");
			}
			else
			{
				ilogger.OpenPvfFileDocument(gly8J8fd2p.FileName);
			}
			return;
		}
		bool flag = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
		if (EquItems == null || EquItems.Count() == 0)
		{
			if (Name != null && Name.Contains(" 未注册到：etc/equipmentpartset.etc") && C6W8rFxEWR.FileAny("etc/equipmentpartset.etc"))
			{
				ilogger.OpenPvfFileDocument("etc/equipmentpartset.etc");
			}
			return;
		}
		if (flag)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, EquipmentPartSet> equItem in EquItems)
			{
				IEnumerable<string> filePaths = equItem.Value.GetFilePaths();
				list.AddRange(filePaths);
			}
			if (gly8J8fd2p == null)
			{
				ilogger.Error("套装信息文件不存在");
			}
			else
			{
				list.Add(gly8J8fd2p.FileName);
			}
			ilogger.AddFileListToNewSearchPanel(list, Name);
			return;
		}
		foreach (KeyValuePair<string, EquipmentPartSet> equItem2 in EquItems)
		{
			if (equItem2.Value.GetFile() == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
				defaultInterpolatedStringHandler.AppendLiteral("套装部件：");
				defaultInterpolatedStringHandler.AppendFormatted(equItem2.Value.Name);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(equItem2.Value.EquType);
				defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
				ilogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				ilogger.OpenPvfFileDocument(equItem2.Value.GetFile().FileName);
			}
		}
		if (gly8J8fd2p == null)
		{
			ilogger.Error("套装信息文件不存在");
		}
		else
		{
			ilogger.OpenPvfFileDocument(gly8J8fd2p.FileName);
		}
	}

	[Command]
	public void ReferencesGridDoubleClick(RowClickArgs nodeClickArgs)
	{
		if (nodeClickArgs.Item is EquipmentPartSet.ReferencesRowViewModel { File: not null } referencesRowViewModel)
		{
			AppSetting.Instance.GetIlogger()?.OpenPvfFileDocument(referencesRowViewModel.File.FileName);
		}
	}
}
