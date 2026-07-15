using System.Collections.Generic;
using System.Linq;
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
	private readonly PvfFile file;

	private readonly PvfGroup pvf;

	public List<PieceSetAbility> Items { get; set; }

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

	public Dictionary<string, EquipmentPartSet> EquItems { get; set; }

	public string Name { get; set; }

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
		this.file = file;
		this.pvf = pvf;
		if (this.file != null)
		{
			Name = pvf.GetItemName(file);
			LoadItems();
		}
	}

	private void LoadItems()
	{
		ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(file, pvf);
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
			Items.Add(new PieceSetAbility(scriptFileParserNew, item.Children, pvf));
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
			if (ilogger.ShowDialog($"当前套装位置共有{filePaths.Count()}件装备 大于10件不建议同时用文档打开 确定要打开吗") != MessageResult.Yes)
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
			if (file == null)
			{
				ilogger.ShowMsg("套装信息文件不存在");
			}
			else
			{
				ilogger.OpenPvfFileDocument(file.FileName);
			}
			return;
		}
		bool flag = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
		if (EquItems == null || EquItems.Count() == 0)
		{
			if (Name != null && Name.Contains(" 未注册到：etc/equipmentpartset.etc") && pvf.FileAny("etc/equipmentpartset.etc"))
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
			if (file == null)
			{
				ilogger.Error("套装信息文件不存在");
			}
			else
			{
				list.Add(file.FileName);
			}
			ilogger.AddFileListToNewSearchPanel(list, Name);
			return;
		}
		foreach (KeyValuePair<string, EquipmentPartSet> equItem2 in EquItems)
		{
			if (equItem2.Value.GetFile() == null)
			{
				ilogger.Error($"套装部件：{equItem2.Value.Name} {equItem2.Value.EquType} 不存在");
			}
			else
			{
				ilogger.OpenPvfFileDocument(equItem2.Value.GetFile().FileName);
			}
		}
		if (file == null)
		{
			ilogger.Error("套装信息文件不存在");
		}
		else
		{
			ilogger.OpenPvfFileDocument(file.FileName);
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
