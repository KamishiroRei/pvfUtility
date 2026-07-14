using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.Models.Options.Enums;
using PvfCode.Models.Pvf;
using PvfCode.Services.PvfParsingNew;
using PvfCode.ViewModels.independent_drop.DropList;
using PvfCode.ViewModels.independent_drop.Enums;
using PvfCode.ViewModels.independent_drop.ValidationRules;
using PvfCode.Views.independent_drop;
using Swordfish.NET.Collections;
using ViewModels.independent_drop;
using WinCopies.Collections;
using WinCopies.Util;

namespace PvfCode.ViewModels.independent_drop;

public class Independent_drop_ViewModel : ViewModelBase, IDisposable
{
	private readonly Action Close;

	private DropListRowData selectedItem;

	private ConcurrentObservableCollection<DropListRowData> allDropItems;

	private ConcurrentObservableCollection<DropListRowData> SearchResult;

	private const string FilePath = "etc/independent_drop.etc";

	protected virtual IMessageBoxService MessageBoxService => GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);

	public bool DeleteWork { get; set; }

	public DropListRowData SelectedItem
	{
		get
		{
			return selectedItem;
		}
		set
		{
			selectedItem = value;
			if (!DeleteWork)
			{
				RaisePropertyChanged("SelectedItem");
			}
		}
	}

	public List<DropListRowData> SelectedItems { get; set; }

	public int DropItemsCount
	{
		get
		{
			if (DropItems != null)
			{
				return DropItems.Count;
			}
			return 0;
		}
	}

	public Dictionary<int, string> CharacTypeDic
	{
		get
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			dictionary.Add(-1, AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_NoLimit"));
			foreach (KeyValuePair<int, LstItem> item in PVF.ListFileTable.CodeDic["character"])
			{
				if (!dictionary.ContainsKey(item.Key))
				{
					string value = PVF.GetItemName(item.Value.FullPath);
					if (string.IsNullOrEmpty(value))
					{
						value = AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_NoName");
					}
					dictionary.Add(item.Key, value);
				}
			}
			return dictionary;
		}
	}

	public ConcurrentObservableCollection<DropListRowData> DropItems
	{
		get
		{
			if (HasSearchKeyword)
			{
				return SearchResult;
			}
			return allDropItems;
		}
		set
		{
			allDropItems = value;
			RaisePropertyChanged("DropItems");
			RaisePropertyChanged("DropItemsCount");
		}
	}

	private Dictionary<int, KeyValuePair<List<ListItem>, LstItem>> IndependentDropLists { get; set; }

	public Dungeon_drop_rate_balance Dungeon_drop_rate_balance { get; set; }

	private PvfGroup PVF => AppCore.ViewModelBase.PVF;

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

	public DropListRowData SelectedItem2 { get; set; }

	public ListItem ItemCodeSelectedItem
	{
		get
		{
			return GetProperty(() => ItemCodeSelectedItem);
		}
		set
		{
			SetProperty<ListItem>(() => ItemCodeSelectedItem, value);
		}
	}

	public ListItem ItemCodeSelectedItem2
	{
		get
		{
			return GetProperty(() => ItemCodeSelectedItem2);
		}
		set
		{
			SetProperty<ListItem>(() => ItemCodeSelectedItem2, value);
		}
	}

	public List<ListItem> ItemCodeSelectedItems { get; set; }

	public int SetSelectedItemsDropWeightValue
	{
		get
		{
			return GetProperty(() => SetSelectedItemsDropWeightValue);
		}
		set
		{
			SetProperty(() => SetSelectedItemsDropWeightValue, value);
		}
	}

	public string SearchKeyword
	{
		get
		{
			return GetProperty(() => SearchKeyword);
		}
		set
		{
			SetProperty<string>(() => SearchKeyword, value, OnSearchKeywordChanged);
		}
	}

	public SearchConfig Config { get; set; }

	public void Dispose()
	{
		SelectedItem = null;
		SelectedItems = null;
		allDropItems = null;
		SearchResult = null;
		DropItems = null;
		IndependentDropLists = null;
	}

	private bool HasSearchKeyword => !string.IsNullOrEmpty(SearchKeyword);

	public Independent_drop_ViewModel(Action close)
	{
		Config = new SearchConfig();
		Close = close;
		DropItems = new ConcurrentObservableCollection<DropListRowData>();
		SearchResult = new ConcurrentObservableCollection<DropListRowData>();
		SelectedItems = new List<DropListRowData>();
		ItemCodeSelectedItems = new List<ListItem>();
	}

	[Command]
	public async void Loaded()
	{
		await Task.Run((Action)Init);
		await Task.Delay(500);
		SelectedItem2 = SelectedItem;
	}

	public async void Init()
	{
		IsLoading = true;
		ResultData resultData = LoadIndependentDropLists();
		if (resultData.IsError)
		{
			AppCore.Logger.Error(resultData.Msg);
			AppCore.ShowMsg(MessageBoxService, resultData.Msg, isError: true);
			IsLoading = false;
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				Close();
			});
			return;
		}
		ResultData resultData2 = await LoadIndependentDropDataAsync();
		if (resultData2.IsError)
		{
			AppCore.Logger.Error(resultData2.Msg);
			AppCore.ShowMsg(MessageBoxService, resultData2.Msg, isError: true);
			IsLoading = false;
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
			{
				Close();
			});
		}
		else
		{
			IsLoading = false;
		}
	}

	private ResultData LoadIndependentDropLists()
	{
		ResultData resultData = new ResultData();
		if (!PVF.FileAny("etc/independentdrop.lst"))
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstLoadFail"), "etc/independentdrop.lst");
			return resultData;
		}
		ResultData<Dictionary<int, LstItem>> lstDicTable = PVF.GetLstDicTable("etc/independentdrop.lst");
		if (!lstDicTable.IsError)
		{
			IndependentDropLists = new Dictionary<int, KeyValuePair<List<ListItem>, LstItem>>();
			foreach (KeyValuePair<int, LstItem> datum in lstDicTable.Data)
			{
				if (!PVF.FileList.TryGetValue(datum.Value.FullPath, out PvfFile value))
				{
					resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstItemPathNotExist"), datum.Value.ItemPath);
					return resultData;
				}
				ResultData<List<ListItem>> resultData2 = ParseIndependentDropList(value);
				if (resultData2.IsError)
				{
					return resultData2;
				}
				if (!IndependentDropLists.ContainsKey(datum.Key))
				{
					IndependentDropLists.Add(datum.Key, new KeyValuePair<List<ListItem>, LstItem>(resultData2.Data, datum.Value));
				}
			}
		}
		return lstDicTable;
	}

	private ResultData<List<ListItem>> ParseIndependentDropList(PvfFile file)
	{
		ResultData<List<ListItem>> resultData = new ResultData<List<ListItem>>();
		ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(file, PVF);
		scriptFileParserNew.PraseStructureMain();
		List<SectionBase> sections = scriptFileParserNew.Sections;
		if (sections.Count != 1)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstFormatError"), file.FileName);
			return resultData;
		}
		if (sections[0].GetSectionName() != "[list]")
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstFormatError_NoListTag"), file.FileName);
			return resultData;
		}
		sections = sections[0].Children;
		if (sections.Count == 0)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstFormatError"), file.FileName);
			return resultData;
		}
		if (sections.Count % 2 != 0)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstFormatError_NoPair"), file.FileName);
			return resultData;
		}
		int num = 0;
		ListItem listItem = null;
		List<ListItem> list = new List<ListItem>();
		int num2 = 0;
		foreach (SectionBase item in sections)
		{
			num2++;
			if (num2 == 1)
			{
				continue;
			}
			string itemText = item.Item.GetItemText(PVF);
			if (!(itemText == "[/list]"))
			{
				if (!int.TryParse(itemText, out var result))
				{
					resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstFormatError_NoInt32"), file.FileName, itemText);
					return resultData;
				}
				num++;
				switch (num)
				{
				case 1:
					listItem = new ListItem();
					listItem.ItemCode = result;
					break;
				case 2:
					listItem.DropWeight = result;
					num = 0;
					list.Add(listItem);
					break;
				}
			}
		}
		resultData.Data = list;
		return resultData;
	}

	private async Task<ResultData> LoadIndependentDropDataAsync()
	{
		ResultData re = new ResultData();
		if (!PVF.FileList.TryGetValue("etc/independent_drop.etc", out PvfFile value))
		{
			re.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), value.FileName);
			return re;
		}
		ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(value, PVF);
		scriptFileParserNew.PraseStructureMain();
		List<SectionBase> sections = scriptFileParserNew.Sections;
		for (int i = 0; i < sections.Count; i++)
		{
			SectionBase sectionBase = sections[i];
			if (sectionBase is PvfSection)
			{
				string sectionName = sectionBase.GetSectionName();
				if (sectionName == "[independent drop]")
				{
					ResultData resultData = await ParseIndependentDropSectionAsync(scriptFileParserNew, sectionBase);
					if (resultData.IsError)
					{
						return resultData;
					}
				}
				else if (sectionName == "[dungeon drop rate balance]")
				{
					ResultData resultData2 = ParseDungeonDropRateBalance(sectionBase);
					if (resultData2.IsError)
					{
						return resultData2;
					}
				}
				continue;
			}
			re.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstDataError"), scriptFileParserNew.GetItemVlaue(sectionBase, i));
			return re;
		}
		return re;
	}

	private async Task<ResultData> ParseIndependentDropSectionAsync(ScriptFileParserNew parser, SectionBase section)
	{
		ResultData re = new ResultData();
		int count = section.Children.Count;
		int num = 0;
		List<string> list = new List<string>();
		ConcurrentObservableCollection<DropListRowData> items = new ConcurrentObservableCollection<DropListRowData>();
		DropListRowData dropListRowData = null;
		for (int i = 1; i < count; i++)
		{
			SectionBase sectionBase = section.Children[i];
			if (sectionBase is PvfSection)
			{
				if (list.Count != 17)
				{
					re.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstDataError_17"), string.Join("\t", list));
					return re;
				}
				if (dropListRowData.DropType == DropType.List)
				{
					ResultData resultData = ParseInlineDropList(sectionBase, dropListRowData);
					if (resultData.IsError)
					{
						return resultData;
					}
				}
				else
				{
					if (dropListRowData.DropType != DropType.DropFile)
					{
						continue;
					}
					if (sectionBase.Children.Count != 3)
					{
						re.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstDataError_DropFile"), DropType.DropFile, "etc/independentdrop.lst", string.Join("\t", list));
						return re;
					}
					string itemText = sectionBase.Children[1].Item.GetItemText(PVF);
					if (!int.TryParse(itemText, out var result))
					{
						if (itemText == "[/list]")
						{
							re.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_IndependentdropLstDataError_NoListTag");
							return re;
						}
						re.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstDataError_NotInt32"), itemText);
						return re;
					}
					if (!IndependentDropLists.TryGetValue(result, out KeyValuePair<List<ListItem>, LstItem> value))
					{
						value = new KeyValuePair<List<ListItem>, LstItem>(new List<ListItem>(), new LstItem("aaa", "bbb", -1));
					}
					RequiredValidationRule.DIC = IndependentDropLists;
					dropListRowData.DropList_independentdrop = new DropList_independentdrop(result, value.Key, value.Value.FullPath, IndependentDropLists);
				}
			}
			else
			{
				ScriptItem nextItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? section.Children[i + 1].Item : null);
				string itemText2 = sectionBase.Item.GetItemText(PVF, nextItem);
				if (itemText2 == "[/list]")
				{
					re.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_IndependentdropLstDataError_ExtraListTag");
					return re;
				}
				num++;
				if (num == 1)
				{
					list = new List<string>();
				}
				list.Add(itemText2);
				if (num == 17)
				{
					num = 0;
					dropListRowData = new DropListRowData(list, new DropList_independentdrop(-1, new List<ListItem>(), "", IndependentDropLists));
					items.Add(dropListRowData);
				}
				if (i == count && num != 17)
				{
					re.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstDataError_17"), string.Join("\t", list));
					return re;
				}
			}
		}
		DropItems = new ConcurrentObservableCollection<DropListRowData>();
		await Task.Delay(1);
		DropItems.AddRange(items);
		RaiseDropItemsCountChanged();
		return re;
	}

	private ResultData ParseInlineDropList(SectionBase section, DropListRowData row)
	{
		ResultData resultData = new ResultData();
		int count = section.Children.Count;
		int num = 0;
		ConcurrentObservableCollection<ListItem> concurrentObservableCollection = new ConcurrentObservableCollection<ListItem>();
		DropListList dropListList = new DropListList(concurrentObservableCollection);
		ListItem listItem = new ListItem();
		List<int> list = new List<int>();
		row.DropListList = dropListList;
		for (int i = 1; i < count; i++)
		{
			SectionBase sectionBase = section.Children[i];
			if (sectionBase is PvfSection)
			{
				resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstDataError_ExtraTag"), string.Join("\t", row.Datas), sectionBase.GetSectionName());
				return resultData;
			}
			ScriptItem nextItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? section.Children[i + 1].Item : null);
			string itemText = sectionBase.Item.GetItemText(PVF, nextItem);
			if (!(itemText == "[/list]"))
			{
				if (!int.TryParse(itemText, out var result))
				{
					resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstFormatError_NoInt32_2"), itemText, string.Join("\t", row.Datas));
					return resultData;
				}
				num++;
				switch (num)
				{
				case 1:
					list = new List<int>();
					listItem = new ListItem();
					listItem.ItemCode = result;
					break;
				case 2:
					listItem.DropWeight = result;
					concurrentObservableCollection.Add(listItem);
					num = 0;
					break;
				}
				list.Add(result);
				if (i == count && num != 2)
				{
					resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_IndependentdropLstFormatError_NoInt32_2_2"), string.Join("\t", row.Datas));
					return resultData;
				}
			}
		}
		return resultData;
	}

	private ResultData ParseDungeonDropRateBalance(SectionBase section)
	{
		ResultData resultData = new ResultData();
		try
		{
			List<int> list = new List<int>();
			int count = section.Children.Count;
			for (int i = 1; i < count; i++)
			{
				SectionBase sectionBase = section.Children[i];
				if (sectionBase is PvfSection)
				{
					resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_IndependentdropLstFormatError_NoInt32_3");
					return resultData;
				}
				ScriptItem nextItem = ((sectionBase.Item.Type == ScriptType.StringLinkIndex) ? section.Children[i + 1].Item : null);
				string itemText = sectionBase.Item.GetItemText(PVF, nextItem);
				if (!(itemText == "[/dungeon drop rate balance]"))
				{
					if (!int.TryParse(itemText, out var result))
					{
						resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ValueNotInt32"), itemText);
						return resultData;
					}
					list.Add(result);
				}
			}
			Dungeon_drop_rate_balance = new Dungeon_drop_rate_balance(list);
		}
		catch (Exception ex)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_LoadDungeonDropRateBalanceError"), ex.Message);
		}
		return resultData;
	}

	[Command]
	public void OnCopyMonsterDropToText(IList<DropListRowData> rows)
	{
		if (rows == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (DropListRowData row in rows)
		{
			stringBuilder.AppendLine(row.GetText());
		}
		AppCore.CopyString(stringBuilder.ToString());
		AppCore.Logger.Success("选中怪物的独立掉落信息已复制到剪贴板");
	}

	[Command]
	public void OnSave()
	{
		IsLoading = true;
		StringBuilder stringBuilder = new StringBuilder("#PVF_File\r\n");
		stringBuilder.AppendLine("[independent drop]");
		foreach (DropListRowData item in allDropItems)
		{
			stringBuilder.AppendLine(item.GetText());
		}
		stringBuilder.AppendLine("[/independent drop]");
		if (Dungeon_drop_rate_balance != null)
		{
			stringBuilder.AppendLine("[dungeon drop rate balance]");
			stringBuilder.AppendLine(Dungeon_drop_rate_balance.ToText());
			stringBuilder.AppendLine("[/dungeon drop rate balance]");
		}
		PVF.SaveFileText("etc/independent_drop.etc", stringBuilder.ToString());
		AppCore.ShowMsg(MessageBoxService, AppSetting.Instance.GetIlogger()?.GetStr("mess_CompileSuccess"));
		IsLoading = false;
	}

	[Command]
	public void OnDeleteSelectedMonster()
	{
		if (SelectedItems != null && SelectedItems.Any() && AppCore.Logger.ShowDialogResult(MessageBoxService, string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_DeleteMonsterConfirm"), SelectedItems.Count)) == MessageResult.Yes)
		{
			DeleteWork = true;
			allDropItems.RemoveRange(SelectedItems.ToArray());
			if (HasSearchKeyword)
			{
				SearchResult.RemoveRange(SelectedItems.ToArray());
			}
			DeleteWork = false;
			RaisePropertyChanged("SelectedItem");
			RaiseDropItemsCountChanged();
		}
	}

	[Command]
	public void OnAddMonster()
	{
		DropListRowData dropListRowData = new DropListRowData("0\t-1\t0\t1000000\t1000000\t1000000\t1000000\t1000000\t1\t1\t1\t1\t1\t0\t0\t-1\t1\t".Split("\t", StringSplitOptions.RemoveEmptyEntries).ToList(), new DropList_independentdrop(-1, new List<ListItem>(), "", IndependentDropLists));
		dropListRowData.DropListList = new DropListList(new ConcurrentObservableCollection<ListItem>
		{
			new ListItem
			{
				ItemCode = -1,
				DropWeight = 1000
			}
		});
		if (AppSetting.Instance.InsertIndependent_drop_ListOrder == InsertListOrder.首行插入)
		{
			allDropItems.Insert(0, dropListRowData);
		}
		else
		{
			allDropItems.Add(dropListRowData);
		}
		if (HasSearchKeyword)
		{
			if (AppSetting.Instance.InsertIndependent_drop_ListOrder == InsertListOrder.首行插入)
			{
				SearchResult.Insert(0, dropListRowData);
			}
			else
			{
				SearchResult.Add(dropListRowData);
			}
		}
		SelectedItem = dropListRowData;
		SelectedItems.Add(dropListRowData);
		SelectedItem2 = dropListRowData;
		RaisePropertyChanged("SelectedItem2");
		RaiseDropItemsCountChanged();
	}

	[Command]
	public void OnSelectMonsterSetValue(Window win)
	{
		string item = ((SelectedItem.MonsterType == MonsterType.APC) ? "aicharacter" : "monster");
		int? num = AppCore.ShowItemCodeSelectView((SelectedItem.MonsterType == MonsterType.APC) ? AppSetting.Instance.GetIlogger().GetStr("mess_PleaseSelectApc") : AppSetting.Instance.GetIlogger().GetStr("mess_PleaseSelectMonster"), win, new List<string> { item }, initItems: true);
		if (num.HasValue)
		{
			SelectedItem.MosterOrApcId = num.Value;
		}
	}

	private void RaiseDropItemsCountChanged()
	{
		RaisePropertyChanged("DropItemsCount");
	}

	[Command]
	public void OnShowDropTypeSpecification()
	{
		AppCore.ShowMsg(MessageBoxService, AppSetting.Instance.GetIlogger()?.GetStr("mess_Independent_drop_DropType_manual"), isError: false, AppSetting.Instance.GetIlogger()?.GetStr("mess_DropType_manual"));
	}

	[Command]
	public void OnAddDroptItemCode(Window win)
	{
		if (SelectedItem == null)
		{
			return;
		}
		ViewDropSelectItemCodes viewDropSelectItemCodes = new ViewDropSelectItemCodes
		{
			Owner = win,
			WindowStartupLocation = WindowStartupLocation.CenterScreen
		};
		if (viewDropSelectItemCodes.ShowDialog().Value)
		{
			IEnumerable<ListItem> selectedItmes = viewDropSelectItemCodes.GetSelectedItmes();
			if (selectedItmes != null && selectedItmes.Any())
			{
				SelectedItem?.DropListBase?.AddRange(selectedItmes.ToList());
				ListItem itemCodeSelectedItem = (ItemCodeSelectedItem = selectedItmes.FirstOrDefault());
				ItemCodeSelectedItem2 = itemCodeSelectedItem;
				SelectedItem?.DropListBase?.UpdateCount();
			}
		}
	}

	[Command]
	public void OnAddDropItemCodes(Window owner)
	{
		ViewAddItemCodeArray viewAddItemCodeArray = new ViewAddItemCodeArray
		{
			Owner = owner,
			WindowStartupLocation = WindowStartupLocation.CenterScreen
		};
		viewAddItemCodeArray.ShowDialog();
		if (viewAddItemCodeArray.DialogResult.Value && viewAddItemCodeArray.XHEhKC1k16 != null && viewAddItemCodeArray.XHEhKC1k16.Count > 0)
		{
			SelectedItem.DropListBase.AddRange(viewAddItemCodeArray.XHEhKC1k16);
			ListItem item = (ItemCodeSelectedItem2 = (ItemCodeSelectedItem = viewAddItemCodeArray.XHEhKC1k16[viewAddItemCodeArray.XHEhKC1k16.Count - 1]));
			ItemCodeSelectedItems.Add(item);
			RaisePropertyChanged("ItemCodeSelectedItems");
			SelectedItem?.DropListBase?.UpdateCount();
		}
	}

	[Command]
	public void OnSetSelectedItemsDropWeight()
	{
		if (ItemCodeSelectedItems == null || !ItemCodeSelectedItems.Any())
		{
			return;
		}
		foreach (ListItem itemCodeSelectedItem in ItemCodeSelectedItems)
		{
			itemCodeSelectedItem.DropWeight = SetSelectedItemsDropWeightValue;
		}
	}

	[Command]
	public void OnSelectItemCode(Window owner)
	{
		int? num = AppCore.ShowItemCodeSelectView(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectItem"), owner, new List<string>
		{
			"equipment",
			"stackable"
		}, initItems: true);
		if (num.HasValue)
		{
			SelectedItem.ItemCode = num.Value;
		}
	}

	[Command]
	public void OnSelectItemCodeList(Window owner)
	{
		AppCore.ShowItemCodeListSelectView(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectItemBatch"), owner, new List<string>
		{
			"equipment",
			"stackable"
		}).Count();
	}

	[Command]
	public void OnDeleteSelectedDropItemCode()
	{
		if (ItemCodeSelectedItems != null && ItemCodeSelectedItems.Any() && AppCore.Logger.ShowDialogResult(MessageBoxService, string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_DeleteItemConfirm"), ItemCodeSelectedItems.Count)) == MessageResult.Yes)
		{
			SelectedItem?.DropListBase?.Items?.RemoveRange(ItemCodeSelectedItems.ToArray());
			SelectedItem?.DropListBase?.UpdateCount();
		}
	}

	[Command]
	public void OnGoToIndependentdropLstFile()
	{
		AppCore.ViewModelBase.RootDocument.AddDocument("etc/independentdrop.lst", gotoNode: true);
	}

	[Command]
	public void OnCopyName(string name)
	{
		AppCore.CopyString(name);
	}

	private void OnSearchKeywordChanged()
	{
		if (string.IsNullOrEmpty(SearchKeyword))
		{
			SearchResult = new ConcurrentObservableCollection<DropListRowData>();
			RaisePropertyChanged("DropItems");
			RaiseDropItemsCountChanged();
		}
	}

	[Command]
	public async void OnSearch(SearchType searchType)
	{
		if (!string.IsNullOrEmpty(SearchKeyword))
		{
			Config.SearchKeyword = SearchKeyword;
			Config.SearchType = searchType;
			Config.KeywordConvertNumberList();
			await Task.Run(SearchAsync);
		}
	}

	private Task SearchAsync()
	{
		ConcurrentBag<DropListRowData> results = new ConcurrentBag<DropListRowData>();
		Parallel.ForEach(allDropItems, item =>
		{
			if (item.Search(Config))
			{
				results.Add(item);
			}
		});
		SearchResult = new ConcurrentObservableCollection<DropListRowData>();
		ConcurrentObservableCollection<DropListRowData> searchResult = SearchResult;
		IEnumerable<DropListRowData> array = results;
		searchResult.AddRange(in array);
		RaisePropertyChanged("DropItems");
		RaiseDropItemsCountChanged();
		return Task.CompletedTask;
	}
}
