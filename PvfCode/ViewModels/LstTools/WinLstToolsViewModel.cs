using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Dot;
using PvfCode.Models.Pvf;
using PvfCode.ViewModels.DocumentFolder;
using PvfCode.ViewModels.LstTools.AddarrayLstModels;
using WinCopies.Collections;

namespace PvfCode.ViewModels.LstTools;

public class WinLstToolsViewModel : ViewModelBase
{
	[CompilerGenerated]
	private TextDocument lQtmESCmdF;

	[CompilerGenerated]
	private TextDocument Es4mOex8tJ;

	[CompilerGenerated]
	private IHighlightingDefinition wEemK7Elp9;

	private readonly KeyValuePair<string, string>? xG1m9DREy2;

	[CompilerGenerated]
	private AddArrayLstModel knGmP1LjtQ;

	public KeyValuePair<string, string>? SelectedItem
	{
		get
		{
			return GetProperty(() => SelectedItem);
		}
		set
		{
			SetProperty<KeyValuePair<string, string>?>(() => SelectedItem, value);
		}
	}

	public bool IsLoading
	{
		get
		{
			return GetProperty(() => IsLoading);
		}
		set
		{
			SetProperty(() => IsLoading, value);
		}
	}

	public TextDocument Document
	{
		[CompilerGenerated]
		get
		{
			return lQtmESCmdF;
		}
		[CompilerGenerated]
		set
		{
			lQtmESCmdF = value;
		}
	}

	public TextDocument DeleteDocument
	{
		[CompilerGenerated]
		get
		{
			return Es4mOex8tJ;
		}
		[CompilerGenerated]
		set
		{
			Es4mOex8tJ = value;
		}
	}

	public IHighlightingDefinition Highlighting
	{
		[CompilerGenerated]
		get
		{
			return wEemK7Elp9;
		}
		[CompilerGenerated]
		set
		{
			wEemK7Elp9 = value;
		}
	}

	private PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	public AddArrayLstModel AddArrayLstModel
	{
		[CompilerGenerated]
		get
		{
			return knGmP1LjtQ;
		}
		[CompilerGenerated]
		set
		{
			knGmP1LjtQ = value;
		}
	}

	public WinLstToolsViewModel(KeyValuePair<string, string>? selectedItem = null)
	{
		AddArrayLstModel = new AddArrayLstModel();
		DeleteDocument = new TextDocument
		{
			Text = "//代码格式：可以用换行符分割:\r\n1\r\n2\r\n3\r\n//也可以用制表符分割：1\t2\t3\t\r\n\r\n//lst行格式：\r\n10018\t`character/common/jacket/cloth/vest_owool.equ`\r\n10019\t`character/common/jacket/cloth/vest_wool.equ`\r\n10020\t`character/common/jacket/cloth/robe_cfiber.equ`\r\n\r\n//文件完整路径格式：\r\nstackable/10000418_10000586.stk\r\nstackable/10000418_10000587.stk\r\nstackable//10000418_10000590.stk"
		};
		xG1m9DREy2 = selectedItem;
		Document = new TextDocument();
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.lst);
	}

	public void Loaded()
	{
		SelectedItem = xG1m9DREy2;
	}

	[Command]
	public void RowDoubleClick(RowClickArgs rowClickArgs)
	{
		if (SelectedItem.HasValue)
		{
			string value = SelectedItem.Value.Value;
			if (AppCore.ViewModelBase.PVF.FileAny(value))
			{
				AppCore.ViewModelBase.RootDocument.AddDocument(value, gotoNode: true);
			}
		}
	}

	[Command]
	public async void OnCheckRepeatAndInexistence(string filePath)
	{
		Document.Text = "";
		if (filePath == null)
		{
			if (!SelectedItem.HasValue)
			{
				return;
			}
			filePath = SelectedItem.Value.Value;
		}
		PvfGroup pvf = Pvf;
		if (!pvf.PvfIsOpen)
		{
			return;
		}
		PvfFile file = pvf.GetFile(filePath);
		if (file == null)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), filePath));
			return;
		}
		List<KeyValuePair<int, LstItem>> list = await pvf.LstFileToLstTab(file);
		if (list == null || list.Count == 0)
		{
			return;
		}
		HashSet<int> hashSet = new HashSet<int>();
		StringBuilder stringBuilder = new StringBuilder(AppSetting.Instance.GetIlogger()?.GetStr("mess_DuplicationOfCode_1"));
		StringBuilder stringBuilder2 = new StringBuilder(AppSetting.Instance.GetIlogger()?.GetStr("mess_FilePathNotExist_2"));
		foreach (KeyValuePair<int, LstItem> item in list)
		{
			if (hashSet.Contains(item.Key))
			{
				stringBuilder.AppendLine(item.Value.ToLstRow());
			}
			else
			{
				hashSet.Add(item.Key);
			}
			if (!pvf.FileAny(item.Value.FullPath))
			{
				stringBuilder2.AppendLine(item.Value.ToLstRow());
			}
		}
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.Append(stringBuilder);
		stringBuilder3.Append(stringBuilder2);
		Document.Text = stringBuilder3.ToString();
	}

	[Command]
	public async void OnDeleteRepeatAndInexistence(string filePath)
	{
		Document.Text = "";
		if (filePath == null)
		{
			if (!SelectedItem.HasValue)
			{
				return;
			}
			filePath = SelectedItem.Value.Value;
		}
		PvfGroup pvf = Pvf;
		if (!pvf.PvfIsOpen)
		{
			return;
		}
		PvfFile file = pvf.GetFile(filePath);
		if (file == null)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), filePath));
			return;
		}
		List<KeyValuePair<int, LstItem>> list = await pvf.LstFileToLstTab(file);
		if (list == null || list.Count == 0)
		{
			return;
		}
		HashSet<int> hashSet = new HashSet<int>();
		StringBuilder stringBuilder = new StringBuilder(AppSetting.Instance.GetIlogger()?.GetStr("mess_LstDeleteSuccess"));
		StringBuilder stringBuilder2 = new StringBuilder(AppSetting.Instance.GetIlogger()?.GetStr("mess_FilePathNotExist_2"));
		KeyValuePair<int, LstItem>[] array = list.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<int, LstItem> item = array[i];
			if (hashSet.Contains(item.Key))
			{
				stringBuilder.AppendLine(item.Value.ToLstRow());
				list.Remove(item);
			}
			else
			{
				hashSet.Add(item.Key);
			}
			if (!pvf.FileAny(item.Value.FullPath))
			{
				stringBuilder2.AppendLine(item.Value.ToLstRow());
				list.Remove(item);
			}
		}
		StringBuilder stringBuilder3 = new StringBuilder("#PVF_File\r\n");
		foreach (KeyValuePair<int, LstItem> item2 in list)
		{
			stringBuilder3.AppendLine(item2.Value.ToLstRow());
		}
		AppCore.ViewModelBase.PVF.SaveFileText(file, stringBuilder3.ToString());
		StringBuilder stringBuilder4 = new StringBuilder();
		stringBuilder4.Append(stringBuilder);
		stringBuilder4.Append(stringBuilder2);
		Document.Text = stringBuilder4.ToString();
	}

	[Command]
	public async void OnPathRepeat(string filePath)
	{
		Document.Text = "";
		if (filePath == null)
		{
			if (!SelectedItem.HasValue)
			{
				return;
			}
			filePath = SelectedItem.Value.Value;
		}
		PvfGroup pvfGroup = Pvf;
		if (!pvfGroup.PvfIsOpen)
		{
			return;
		}
		PvfFile file = pvfGroup.GetFile(filePath);
		if (file == null)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), filePath));
			return;
		}
		List<KeyValuePair<int, LstItem>> list = await pvfGroup.LstFileToLstTab(file);
		if (list == null || list.Count == 0)
		{
			return;
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		StringBuilder stringBuilder = new StringBuilder(AppSetting.Instance.GetIlogger()?.GetStr("mess_DuplicationOfPath_1"));
		foreach (KeyValuePair<int, LstItem> item in list)
		{
			if (dictionary.ContainsKey(item.Value.ItemPath))
			{
				stringBuilder.AppendLine(item.Value.ToLstRow());
			}
			else
			{
				dictionary.Add(item.Value.ItemPath, item.Key);
			}
		}
		Document.Text = stringBuilder.ToString();
	}

	[Command]
	public async void OnDeletePathRepeat(string filePath)
	{
		Document.Text = "";
		if (filePath == null)
		{
			if (!SelectedItem.HasValue)
			{
				return;
			}
			filePath = SelectedItem.Value.Value;
		}
		PvfGroup pvfGroup = Pvf;
		if (!pvfGroup.PvfIsOpen)
		{
			return;
		}
		PvfFile file = pvfGroup.GetFile(filePath);
		if (file == null)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), filePath));
			return;
		}
		List<KeyValuePair<int, LstItem>> list = await pvfGroup.LstFileToLstTab(file);
		if (list == null || list.Count == 0)
		{
			return;
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		StringBuilder stringBuilder = new StringBuilder(AppSetting.Instance.GetIlogger()?.GetStr("mess_LstDeleteSuccess"));
		foreach (KeyValuePair<int, LstItem> item in list)
		{
			if (dictionary.ContainsKey(item.Value.ItemPath))
			{
				stringBuilder.AppendLine(item.Value.ToLstRow());
			}
			else
			{
				dictionary.Add(item.Value.ItemPath, item.Key);
			}
		}
		StringBuilder stringBuilder2 = new StringBuilder("#PVF_File\r\n");
		foreach (KeyValuePair<string, int> item2 in dictionary)
		{
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(3, 2, stringBuilder3);
			handler.AppendFormatted(item2.Value);
			handler.AppendLiteral("\t`");
			handler.AppendFormatted(item2.Key);
			handler.AppendLiteral("`");
			stringBuilder3.AppendLine(ref handler);
		}
		AppCore.ViewModelBase.PVF.SaveFileText(filePath, stringBuilder2.ToString());
		Document.Text = stringBuilder.ToString();
	}

	[Command]
	public void OnExtractItemCodeOrItemName()
	{
		Document.Text = "";
		if (!AppCore.ViewModelBase.PVF.PvfIsOpen || !SelectedItem.HasValue || !AppCore.ViewModelBase.PVF.ListFileTable.CodeDic.ContainsKey(SelectedItem.Value.Key))
		{
			return;
		}
		Dictionary<int, LstItem> dictionary = AppCore.ViewModelBase.PVF.ListFileTable.CodeDic[SelectedItem.Value.Key];
		PvfGroup pvfGroup = Pvf;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (LstItem value in dictionary.Values)
		{
			PvfFile file = pvfGroup.GetFile(value.FullPath);
			if (file != null)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
				handler.AppendFormatted(value.ItemCode);
				handler.AppendLiteral("\t");
				handler.AppendFormatted(pvfGroup.GetItemName(file));
				stringBuilder2.AppendLine(ref handler);
			}
		}
		Document.Text = stringBuilder.ToString();
	}

	[Command]
	public async void OnSortLst(bool sortCode)
	{
		Document.Text = "";
		if (!SelectedItem.HasValue)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectLst"));
			return;
		}
		string value = SelectedItem.Value.Value;
		PvfFile file = Pvf.GetFile(value);
		List<KeyValuePair<int, LstItem>> list = await Pvf.LstFileToLstTab(file);
		if (list == null || list.Count == 0)
		{
			Document.Text = AppSetting.Instance.GetIlogger()?.GetStr("mess_CurrentLstNoSortContent");
			return;
		}
		if (sortCode)
		{
			Dictionary<int, LstItem> dictionary = new Dictionary<int, LstItem>();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<int, LstItem> item in list)
			{
				if (dictionary.ContainsKey(item.Key))
				{
					stringBuilder.AppendLine(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_DuplicationOfCode_2"), item.Key));
				}
				else
				{
					dictionary.Add(item.Key, item.Value);
				}
			}
			if (stringBuilder.Length > 0)
			{
				Document.Text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SortFailed"), stringBuilder.ToString());
				return;
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendLine("#PVF_File\r\n");
			dictionary = dictionary.OrderBy((KeyValuePair<int, LstItem> p) => p.Key).ToDictionary((KeyValuePair<int, LstItem> p) => p.Key, (KeyValuePair<int, LstItem> o) => o.Value);
			foreach (KeyValuePair<int, LstItem> item2 in dictionary)
			{
				stringBuilder2.AppendLine(item2.Value.ToLstRow());
			}
			AuJmepse83(stringBuilder2.ToString(), file.FileName);
			return;
		}
		StringBuilder stringBuilder3 = new StringBuilder();
		Dictionary<string, LstItem> dictionary2 = new Dictionary<string, LstItem>();
		foreach (KeyValuePair<int, LstItem> item3 in list)
		{
			if (dictionary2.ContainsKey(item3.Value.ItemPath))
			{
				stringBuilder3.AppendLine(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_DuplicationOfPath_2"), item3.Value.ItemPath));
			}
			else
			{
				dictionary2.Add(item3.Value.ItemPath, item3.Value);
			}
		}
		if (stringBuilder3.Length > 0)
		{
			Document.Text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SortFailed"), stringBuilder3.ToString());
			return;
		}
		StringBuilder stringBuilder4 = new StringBuilder();
		stringBuilder4.AppendLine("#PVF_File\r\n");
		dictionary2 = dictionary2.OrderBy((KeyValuePair<string, LstItem> p) => p.Key).ToDictionary((KeyValuePair<string, LstItem> p) => p.Key, (KeyValuePair<string, LstItem> o) => o.Value);
		foreach (KeyValuePair<string, LstItem> item4 in dictionary2)
		{
			stringBuilder4.AppendLine(item4.Value.ToLstRow());
		}
		AuJmepse83(stringBuilder4.ToString(), file.FileName);
	}

	private void AuJmepse83(string P_0, string P_1)
	{
		Pvf.SaveFileText(P_1, P_0);
		Document.Text = AppSetting.Instance.GetIlogger()?.GetStr("mess_SortSuccessAndSaved") + P_0;
		if (AppCore.ViewModelBase.RootDocument.CheckIsOpen(P_1, out DocumentBase docu) && docu != null && docu is PvfFileDocument pvfFileDocument)
		{
			pvfFileDocument.RefDocumentText();
		}
	}

	private void uPfmts75ra(string P_0)
	{
		Document.Text = P_0;
	}

	[Command]
	public async void OnAddArrayLst()
	{
		if (!SelectedItem.HasValue)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectLst"));
		}
		else
		{
			if (!Pvf.PvfIsOpen)
			{
				return;
			}
			if (string.IsNullOrEmpty(AddArrayLstModel.ArrarLstString))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputLst"));
				return;
			}
			string[] array = AddArrayLstModel.ArrarLstString.Split(new string[2]
			{
				"\r\n",
				"\t"
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length == 1)
			{
				uPfmts75ra(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_AddFailed"), array[0]));
				return;
			}
			List<KeyValuePair<int, string>> addDic = new List<KeyValuePair<int, string>>();
			int num = array.Length;
			int num2;
			for (num2 = 0; num2 < num; num2++)
			{
				string text = array[num2];
				if (num2 + 1 >= num)
				{
					uPfmts75ra(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_AddFailedFormatError"), text));
					return;
				}
				if (!int.TryParse(text, out var result))
				{
					uPfmts75ra(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_AddFailedNotNumber"), text));
					return;
				}
				string text2 = array[num2 + 1];
				if (text2[0] != '`' || text2[text2.Length - 1] != '`')
				{
					uPfmts75ra(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_AddFailedNotPath"), text2));
					return;
				}
				addDic.Add(new KeyValuePair<int, string>(result, text2));
				num2++;
			}
			string filePath = SelectedItem.Value.Value;
			ResultData<Dictionary<int, string>> resultData = await Pvf.LstFileTabCodeDic(filePath);
			if (resultData.IsError)
			{
				uPfmts75ra(resultData.Msg);
				return;
			}
			Dictionary<int, string> codeDic = resultData.Data;
			ResultData<Dictionary<string, int>> resultData2 = await Pvf.LstFileTabPathDic(filePath);
			if (resultData2.IsError)
			{
				uPfmts75ra(resultData2.Msg);
				return;
			}
			Dictionary<string, int> data = resultData2.Data;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			PvfFile file = Pvf.GetFile(filePath);
			_ = file.FilePathHeader;
			int num3 = 0;
			HashSet<int> hashSet = new HashSet<int>();
			HashSet<string> hashSet2 = new HashSet<string>();
			foreach (KeyValuePair<int, string> item in addDic)
			{
				int num4 = item.Key;
				StringBuilder stringBuilder3;
				StringBuilder.AppendInterpolatedStringHandler handler;
				if (codeDic.ContainsKey(num4) || hashSet.Contains(num4))
				{
					switch (AddArrayLstModel.CodeRepeatManageType)
					{
					case CodeRepeatManageType.自动生成编号:
						num4 = Pvf.ListFileTable.GetLstNumMax(filePath) + 1;
						break;
					case CodeRepeatManageType.跳过:
					{
						stringBuilder3 = stringBuilder2;
						StringBuilder stringBuilder4 = stringBuilder3;
						handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder3);
						handler.AppendFormatted(zZxmbQDY4G(item));
						handler.AppendLiteral(" ");
						handler.AppendFormatted(AppSetting.Instance.GetIlogger()?.GetStr("mess_DuplicationOfCode"));
						stringBuilder4.AppendLine(ref handler);
						continue;
					}
					}
				}
				string text3 = item.Value.Replace("`", string.Empty);
				if (data.Contains(text3) || hashSet2.Contains(text3))
				{
					switch (AddArrayLstModel.PathRepeatManageType)
					{
					case PathRepeatManageType.跳过:
					{
						stringBuilder3 = stringBuilder2;
						StringBuilder stringBuilder5 = stringBuilder3;
						handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder3);
						handler.AppendFormatted(zZxmbQDY4G(item));
						handler.AppendLiteral(" ");
						handler.AppendFormatted(AppSetting.Instance.GetIlogger()?.GetStr("mess_DuplicationOfCode"));
						stringBuilder5.AppendLine(ref handler);
						continue;
					}
					}
				}
				if (!Pvf.GetLstFullPath(file, text3, out string _))
				{
					switch (AddArrayLstModel.PathNotAnyManageType)
					{
					case PathRepeatManageType.跳过:
					{
						stringBuilder3 = stringBuilder2;
						StringBuilder stringBuilder6 = stringBuilder3;
						handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder3);
						handler.AppendFormatted(zZxmbQDY4G(item));
						handler.AppendLiteral(" ");
						handler.AppendFormatted(AppSetting.Instance.GetIlogger()?.GetStr("mess_FilePathNotExist_3"));
						stringBuilder6.AppendLine(ref handler);
						continue;
					}
					}
				}
				hashSet.Add(num4);
				hashSet2.Add(text3);
				stringBuilder3 = stringBuilder;
				StringBuilder stringBuilder7 = stringBuilder3;
				handler = new StringBuilder.AppendInterpolatedStringHandler(3, 2, stringBuilder3);
				handler.AppendFormatted(num4);
				handler.AppendLiteral("\t`");
				handler.AppendFormatted(text3);
				handler.AppendLiteral("`");
				stringBuilder7.AppendLine(ref handler);
				num3++;
			}
			if (num3 > 0)
			{
				string fileText = Pvf.GetFileText(file);
				PvfGroup pvfGroup = Pvf;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
				defaultInterpolatedStringHandler.AppendFormatted(fileText);
				defaultInterpolatedStringHandler.AppendLiteral("\r\n");
				defaultInterpolatedStringHandler.AppendFormatted(stringBuilder);
				pvfGroup.SaveFileText(file, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			uPfmts75ra(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_OperationComplete"), num3, addDic.Count - num3, stringBuilder2, stringBuilder));
		}
	}

	private string zZxmbQDY4G(KeyValuePair<int, string> keyValuePair)
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
		defaultInterpolatedStringHandler.AppendFormatted(keyValuePair.Key);
		defaultInterpolatedStringHandler.AppendLiteral("\t");
		defaultInterpolatedStringHandler.AppendFormatted(keyValuePair.Value);
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}
}
