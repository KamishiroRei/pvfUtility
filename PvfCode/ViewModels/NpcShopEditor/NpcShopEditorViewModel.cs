using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using PvfCode.LoggerBase;
using PvfCode.Services.PvfParsingNew;
using Swordfish.NET.Collections;
using Utools;
using WinCopies.Util;

namespace PvfCode.ViewModels.NpcShopEditor;

public class NpcShopEditorViewModel : ViewModelBase
{
	private sealed class PurchaseFileUpdate
	{
		public PvfFile File;

		public List<NpcShopItem> Items = new List<NpcShopItem>();

		public bool UpdatePrice;

		public string PriceSection = string.Empty;

		public bool UpdateNeedMaterial;

		public string NeedMaterialSection = string.Empty;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass47_0
	{
		public ConcurrentObservableCollection<NpcShopItem> RVBdHnWW2O;

		public int YXEdhHh8ZP;

		public List<NpcShopPageViewModel> pY9dvhrrIo;

		public List<string> VgPdBTsMfM;

		public _003C_003Ec__DisplayClass47_0()
		{
		}

		internal void qyEdC09jS8(int code)
		{
			if (code == -2)
			{
				int count = RVBdHnWW2O.Count;
				if (count % 7 != 0)
				{
					for (int i = 0; i < 7 - count % 7; i++)
					{
						RVBdHnWW2O.Add(new NpcShopItem());
					}
				}
				RVBdHnWW2O = new ConcurrentObservableCollection<NpcShopItem>();
				YXEdhHh8ZP++;
				pY9dvhrrIo.Add(new NpcShopPageViewModel(PKHmaI3tNd(VgPdBTsMfM, YXEdhHh8ZP), RVBdHnWW2O));
			}
			else
			{
				RVBdHnWW2O.Add(new NpcShopItem(code));
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass56_0
	{
		public AutoSuggestEditQuerySubmittedEventArgs Si3drpR7Ex;

		public _003C_003Ec__DisplayClass56_0()
		{
		}

		internal bool LBZdFMiuw8(FindNpcShopSource it)
		{
			return it.Find(Si3drpR7Ex.Text);
		}
	}

	[CompilerGenerated]
	private PvfFile V5xm6k5wqU;

	private List<FindNpcShopSource> hf7m1JSlp9;

	[CompilerGenerated]
	private ConcurrentObservableCollection<NpcShopItemSource> PhdmwBFTGd;

	[CompilerGenerated]
	private ConcurrentObservableCollection<NpcShopItemSource> CywmoC0ED0;

	[CompilerGenerated]
	private ConcurrentObservableCollection<NpcShopPageViewModel> lbDmsLwH65;

	private NpcShopPageViewModel? observedCurrentPage;

	public PvfFile File
	{
		[CompilerGenerated]
		get
		{
			return V5xm6k5wqU;
		}
		[CompilerGenerated]
		set
		{
			V5xm6k5wqU = value;
		}
	}

	private PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	public int NpcId
	{
		get
		{
			return GetProperty(() => NpcId);
		}
		set
		{
			SetProperty(() => NpcId, value);
			RaisePropertyChanged("NpcName");
		}
	}

	public string NpcName
	{
		get
		{
			if (CurrentNpcShop == null)
			{
				return "请先选择商店";
			}
			if (NpcId == -1)
			{
				return "未设定NPC";
			}
			PvfFile pvfFile = Pvf.ListFileTable.ItemCodeConvertPvfFile(Pvf, NpcId, new string[1] { "npc" });
			if (pvfFile == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler.AppendLiteral("未知NPC：");
				defaultInterpolatedStringHandler.AppendFormatted(NpcId);
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			return Pvf.GetItemName(pvfFile);
		}
	}

	public bool ShopIsOpen => CurrentNpcShop != null;

	public string NpcShopSearchKeyword
	{
		get
		{
			return GetProperty(() => NpcShopSearchKeyword);
		}
		set
		{
			SetProperty<string>(() => NpcShopSearchKeyword, value);
		}
	}

	public FindNpcShopSource? CurrentNpcShop
	{
		get
		{
			return GetProperty(() => CurrentNpcShop);
		}
		set
		{
			SetProperty<FindNpcShopSource>(() => CurrentNpcShop, value);
			RaisePropertyChanged("ShopIsOpen");
			uxumQwudkj();
		}
	}

	public List<FindNpcShopSource> NpcShopList
	{
		get
		{
			if (hf7m1JSlp9 == null)
			{
				hf7m1JSlp9 = FindNpcShopSource.Create();
			}
			return hf7m1JSlp9;
		}
	}

	public ConcurrentObservableCollection<NpcShopItemSource> EquItems
	{
		[CompilerGenerated]
		get
		{
			return PhdmwBFTGd;
		}
		[CompilerGenerated]
		set
		{
			PhdmwBFTGd = value;
		}
	}

	public ConcurrentObservableCollection<NpcShopItemSource> StkItems
	{
		[CompilerGenerated]
		get
		{
			return CywmoC0ED0;
		}
		[CompilerGenerated]
		set
		{
			CywmoC0ED0 = value;
		}
	}

	public ConcurrentObservableCollection<NpcShopPageViewModel> Pages
	{
		[CompilerGenerated]
		get
		{
			return lbDmsLwH65;
		}
		[CompilerGenerated]
		set
		{
			lbDmsLwH65 = value;
		}
	}

	public NpcShopPageViewModel? CurrentPage
	{
		get
		{
			return GetProperty(() => CurrentPage);
		}
		set
		{
			if (ReferenceEquals(CurrentPage, value))
			{
				return;
			}
			SetProperty<NpcShopPageViewModel?>(() => CurrentPage, value);
			ObserveCurrentPage(value);
		}
	}

	public NpcShopItem? CurrentPurchaseItem
	{
		get => GetProperty(() => CurrentPurchaseItem);
		set
		{
			NpcShopItem? current = CurrentPurchaseItem;
			if (ReferenceEquals(current, value))
			{
				value?.EnsurePurchaseDataLoaded();
				return;
			}
			if (current != null)
			{
				current.PropertyChanged -= CurrentPurchaseItemPropertyChanged;
			}
			SetProperty<NpcShopItem?>(() => CurrentPurchaseItem, value);
			if (value != null)
			{
				value.PropertyChanged += CurrentPurchaseItemPropertyChanged;
				value.EnsurePurchaseDataLoaded();
			}
			RaisePropertyChanged(nameof(CurrentPurchaseItemCode));
			RaisePropertyChanged(nameof(CanEditCurrentPurchaseItemCode));
		}
	}

	public int? CurrentPurchaseItemCode
	{
		get => CurrentPurchaseItem?.ItemCode;
		set
		{
			if (!value.HasValue || CurrentPurchaseItem == null || CurrentPurchaseItem.ItemCode == value.Value)
			{
				return;
			}
			if (!CanEditCurrentPurchaseItemCode)
			{
				AppCore.ShowMsg("当前商品的购买属性尚未保存，请先编译保存后再修改物品ID。", isError: true);
				RaisePropertyChanged(nameof(CurrentPurchaseItemCode));
				return;
			}
			CurrentPurchaseItem.ItemCode = value.Value;
		}
	}

	public bool CanEditCurrentPurchaseItemCode =>
		CurrentPurchaseItem != null &&
		CurrentPurchaseItem is not NpcShopItemSource &&
		!CurrentPurchaseItem.IsPurchaseDataModified;

	public string Title
	{
		get
		{
			if (File == null)
			{
				return "NPC商店设计器";
			}
			return "NPC商店设计器-" + NpcName;
		}
	}

	public bool IsLoaded
	{
		get
		{
			return GetProperty(() => IsLoaded);
		}
		set
		{
			SetProperty(() => IsLoaded, value: true);
		}
	}

	[Command]
	public void OnClearNpcShopSearchKeyword()
	{
		NpcShopSearchKeyword = string.Empty;
	}

	public NpcShopEditorViewModel()
	{
		EquItems = new ConcurrentObservableCollection<NpcShopItemSource>();
		StkItems = new ConcurrentObservableCollection<NpcShopItemSource>();
		Pages = new ConcurrentObservableCollection<NpcShopPageViewModel>();
	}

	private void ObserveCurrentPage(NpcShopPageViewModel? page)
	{
		if (observedCurrentPage != null)
		{
			observedCurrentPage.PropertyChanged -= CurrentPagePropertyChanged;
		}
		observedCurrentPage = page;
		if (observedCurrentPage != null)
		{
			observedCurrentPage.PropertyChanged += CurrentPagePropertyChanged;
		}
		CurrentPurchaseItem = page?.CurrentItem;
	}

	private void CurrentPagePropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(NpcShopPageViewModel.CurrentItem))
		{
			CurrentPurchaseItem = observedCurrentPage?.CurrentItem;
		}
	}

	private void CurrentPurchaseItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(NpcShopItem.ItemCode))
		{
			RaisePropertyChanged(nameof(CurrentPurchaseItemCode));
			RaisePropertyChanged(nameof(CanEditCurrentPurchaseItemCode));
		}
		else if (e.PropertyName == nameof(NpcShopItem.IsPurchaseDataModified))
		{
			RaisePropertyChanged(nameof(CanEditCurrentPurchaseItemCode));
		}
	}

	[Command]
	public void OnLoaded()
	{
		IsLoaded = true;
		Task.Run((Action)e9NmG0Y3i9);
		IsLoaded = false;
	}

	private void e9NmG0Y3i9()
	{
		foreach (KeyValuePair<string, PvfFile> item in Pvf.FileList.Where<KeyValuePair<string, PvfFile>>((KeyValuePair<string, PvfFile> it) => it.Value.FilePathHeader == "equipment" && it.Value.ItemCode.HasValue))
		{
			EquItems.Add(new NpcShopItemSource(item.Value));
		}
		foreach (KeyValuePair<string, PvfFile> item2 in Pvf.FileList.Where<KeyValuePair<string, PvfFile>>((KeyValuePair<string, PvfFile> it) => it.Value.FilePathHeader == "stackable" && it.Value.ItemCode.HasValue))
		{
			StkItems.Add(new NpcShopItemSource(item2.Value));
		}
	}

	private void IqEmx2a4Ib()
	{
		foreach (NpcShopItemSource item in EquItems.Concat(StkItems).Where(item => item.IsPurchaseDataModified))
		{
			item.ReloadPurchaseData();
		}
		CurrentPage = null;
		CurrentPurchaseItem = null;
		Pages.Clear();
		RaisePropertyChanged("NpcName");
	}

	private void uxumQwudkj()
	{
		_003C_003Ec__DisplayClass47_0 CS_0024_003C_003E8__locals22 = new _003C_003Ec__DisplayClass47_0();
		IqEmx2a4Ib();
		if (CurrentNpcShop == null || CurrentNpcShop.File == null)
		{
			return;
		}
		File = CurrentNpcShop.File;
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		if (File == null)
		{
			ilogger.Error("NPC商店预览创建失败 文件不能为NULL");
			return;
		}
		int stringTableId = Pvf.Strtable.GetStringTableId("[sell item]");
		if (stringTableId == -1 || !File.FindSectionIndex(stringTableId, out var indexOut))
		{
			return;
		}
		byte[] data = File.Data;
		List<int> list = new List<int>();
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = indexOut + 5; i < File.DataLen; i += 5)
		{
			byte b = data[i];
			if (data[i] == 5)
			{
				break;
			}
			if (b == 2 && i < File.DataLen)
			{
				list.Add(BitConverter.ToInt32(File.Data, i + 1));
				continue;
			}
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(31, 2, stringBuilder2);
			handler.AppendLiteral("商店物品数据类型只能是int 已忽略该值类型：");
			handler.AppendFormatted(b);
			handler.AppendLiteral(" file://");
			handler.AppendFormatted(File.FileName);
			stringBuilder2.AppendLine(ref handler);
		}
		if (stringBuilder.Length > 0)
		{
			ilogger.Error(stringBuilder.ToString());
		}
		if (File.GetNpcId(Pvf, out var npcId))
		{
			NpcId = npcId;
			RaisePropertyChanged("NpcName");
		}
		if (!File.GetSectionTypeIsStrArray(Pvf, "[tab name]", out CS_0024_003C_003E8__locals22.VgPdBTsMfM))
		{
			CS_0024_003C_003E8__locals22.VgPdBTsMfM = new List<string>();
		}
		CS_0024_003C_003E8__locals22.YXEdhHh8ZP = 0;
		CS_0024_003C_003E8__locals22.RVBdHnWW2O = new ConcurrentObservableCollection<NpcShopItem>();
		CS_0024_003C_003E8__locals22.pY9dvhrrIo = new List<NpcShopPageViewModel>();
		CS_0024_003C_003E8__locals22.pY9dvhrrIo.Add(new NpcShopPageViewModel(PKHmaI3tNd(CS_0024_003C_003E8__locals22.VgPdBTsMfM, CS_0024_003C_003E8__locals22.YXEdhHh8ZP), CS_0024_003C_003E8__locals22.RVBdHnWW2O)
		{
			IsSelected = true
		});
		list.ForEach(delegate(int code)
		{
			if (code == -2)
			{
				int count2 = CS_0024_003C_003E8__locals22.RVBdHnWW2O.Count;
				if (count2 % 7 != 0)
				{
					for (int j = 0; j < 7 - count2 % 7; j++)
					{
						CS_0024_003C_003E8__locals22.RVBdHnWW2O.Add(new NpcShopItem());
					}
				}
				CS_0024_003C_003E8__locals22.RVBdHnWW2O = new ConcurrentObservableCollection<NpcShopItem>();
				CS_0024_003C_003E8__locals22.YXEdhHh8ZP++;
				CS_0024_003C_003E8__locals22.pY9dvhrrIo.Add(new NpcShopPageViewModel(PKHmaI3tNd(CS_0024_003C_003E8__locals22.VgPdBTsMfM, CS_0024_003C_003E8__locals22.YXEdhHh8ZP), CS_0024_003C_003E8__locals22.RVBdHnWW2O));
			}
			else
			{
				CS_0024_003C_003E8__locals22.RVBdHnWW2O.Add(new NpcShopItem(code));
			}
		});
		if (CS_0024_003C_003E8__locals22.pY9dvhrrIo.Count > 0)
		{
			int count = CS_0024_003C_003E8__locals22.pY9dvhrrIo.Last().Items.Count;
			if (count % 7 != 0)
			{
				for (int num = 0; num < 7 - count % 7; num++)
				{
					CS_0024_003C_003E8__locals22.RVBdHnWW2O.Add(new NpcShopItem());
				}
			}
		}
		Pages.AddRange(CS_0024_003C_003E8__locals22.pY9dvhrrIo);
		CurrentPage = Pages.FirstOrDefault();
	}

	private static string PKHmaI3tNd(List<string> P_0, int P_1)
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
		defaultInterpolatedStringHandler.AppendLiteral("商店");
		defaultInterpolatedStringHandler.AppendFormatted(P_1);
		string text = defaultInterpolatedStringHandler.ToStringAndClear();
		if (P_0 != null && P_0.Any() && P_1 < P_0.Count)
		{
			text = P_0[P_1];
			if (string.IsNullOrEmpty(text))
			{
				text = "商店";
			}
		}
		return text;
	}

	[Command]
	public void Test()
	{
		AppCore.Logger.Error(CurrentPage.SelectedItems.Count.ToString());
		CurrentPage.SelectedItems.AddRange(CurrentPage.Items);
	}

	[Command]
	public void OnCloseShop()
	{
		if (AppCore.Logger.ShowDialog("确定要关闭商店：[" + NpcName + "] 吗？未保存的数据将会丢失") == MessageResult.Yes)
		{
			CurrentNpcShop = null;
		}
	}

	[Command]
	public void OnSaveShop()
	{
		if (CurrentNpcShop?.File == null || File == null)
		{
			AppCore.ShowMsg("请先选择要保存的NPC商店。", isError: true);
			return;
		}
		if (!TryCreatePurchaseFileUpdates(out List<PurchaseFileUpdate> updates, out string? validationError))
		{
			AppCore.ShowMsg(validationError ?? "商品购买属性验证失败。", isError: true);
			return;
		}

		List<(PurchaseFileUpdate Update, string Text)> purchaseTexts = new List<(PurchaseFileUpdate, string)>();
		string shopText;
		try
		{
			foreach (PurchaseFileUpdate update in updates)
			{
				purchaseTexts.Add((update, BuildPurchaseFileText(update)));
			}
			shopText = BuildShopFileText();
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg("生成商店保存内容失败：" + ex.Message, isError: true);
			return;
		}

		int savedPurchaseFileCount = 0;
		foreach ((PurchaseFileUpdate update, string text) in purchaseTexts)
		{
			if (!Pvf.SaveFileText(update.File, text))
			{
				AppCore.ShowMsg(
					$"商品文件保存失败：{update.File.FileName}\r\n已保存 {savedPurchaseFileCount}/{updates.Count} 个商品文件；商店文件未保存。",
					isError: true);
				return;
			}
			savedPurchaseFileCount++;
			foreach (NpcShopItem item in update.Items)
			{
				item.ReloadPurchaseData();
			}
		}

		if (!Pvf.SaveFileText(File, shopText))
		{
			AppCore.ShowMsg(
				$"商店文件保存失败：{File.FileName}\r\n已保存 {savedPurchaseFileCount}/{updates.Count} 个商品文件；商店文件未保存。",
				isError: true);
			return;
		}
		AppCore.ShowMsg($"编译成功，已保存 {savedPurchaseFileCount} 个商品文件和商店文件。");
	}

	private bool TryCreatePurchaseFileUpdates(out List<PurchaseFileUpdate> updates, out string? error)
	{
		updates = new List<PurchaseFileUpdate>();
		error = null;
		List<NpcShopItem> allItems = Pages
			.SelectMany(page => page.Items)
			.Concat(EquItems.Cast<NpcShopItem>())
			.Concat(StkItems.Cast<NpcShopItem>())
			.Distinct()
			.ToList();
		List<NpcShopItem> dirtyItems = allItems.Where(item => item.IsPurchaseDataModified).ToList();
		foreach (NpcShopItem item in dirtyItems)
		{
			if (!item.CanEditPurchaseData || item.File == null)
			{
				error = $"物品 {item.ItemCode} 没有可编辑的equ/stk商品文件。";
				return false;
			}
			if (!item.TryValidatePurchaseData(out string? itemError))
			{
				error = $"商品文件 {item.File.FileName}：{itemError}";
				return false;
			}
		}

		foreach (IGrouping<string, NpcShopItem> group in dirtyItems.GroupBy(
			item => item.File.FileName,
			StringComparer.OrdinalIgnoreCase))
		{
			List<string> priceSections = group
				.Where(item => item.IsPriceModified)
				.Select(item => NpcShopPurchaseSectionFormatter.FormatPrice(item.Price))
				.Distinct(StringComparer.Ordinal)
				.ToList();
			if (priceSections.Count > 1)
			{
				error = $"商品文件 {group.Key} 的金币价格存在多个不同修改，未写入任何文件。";
				return false;
			}

			List<string> materialSections = group
				.Where(item => item.IsNeedMaterialModified)
				.Select(item => NpcShopPurchaseSectionFormatter.FormatNeedMaterial(
					item.NeedMaterialItemCode,
					item.NeedMaterialCount,
					item.AdditionalNeedMaterialValues))
				.Distinct(StringComparer.Ordinal)
				.ToList();
			if (materialSections.Count > 1)
			{
				error = $"商品文件 {group.Key} 的所需材料存在多个不同修改，未写入任何文件。";
				return false;
			}

			PurchaseFileUpdate update = new PurchaseFileUpdate
			{
				File = group.First().File,
				Items = allItems.Where(item => item.File != null && string.Equals(
					item.File.FileName,
					group.Key,
					StringComparison.OrdinalIgnoreCase)).ToList(),
				UpdatePrice = priceSections.Count == 1,
				PriceSection = priceSections.FirstOrDefault() ?? string.Empty,
				UpdateNeedMaterial = materialSections.Count == 1,
				NeedMaterialSection = materialSections.FirstOrDefault() ?? string.Empty
			};
			updates.Add(update);
		}
		return true;
	}

	private string BuildPurchaseFileText(PurchaseFileUpdate update)
	{
		ScriptFileParserNew parser = new ScriptFileParserNew(update.File, Pvf);
		parser.PraseStructureMain();
		parser.Sections.RemoveAll(section =>
			(update.UpdatePrice && string.Equals(section.GetSectionName(), "[price]", StringComparison.OrdinalIgnoreCase)) ||
			(update.UpdateNeedMaterial && string.Equals(section.GetSectionName(), "[need material]", StringComparison.OrdinalIgnoreCase)));
		return AppendSections(parser.GetText(), update.PriceSection + update.NeedMaterialSection);
	}

	private string BuildShopFileText()
	{
		StringBuilder sellItems = new StringBuilder("[sell item]\r\n");
		StringBuilder tabNames = new StringBuilder("[tab name]\r\n");
		int pageNumber = 1;
		foreach (NpcShopPageViewModel page in Pages)
		{
			if (page.Items != null && page.Items.Any())
			{
				sellItems.AppendLine(page.ToString());
				tabNames.AppendLine($"`{page.Title}`");
			}
			if (pageNumber != Pages.Count)
			{
				sellItems.AppendLine("-2\t");
			}
			pageNumber++;
		}
		sellItems.AppendLine("[/sell item]");
		tabNames.AppendLine("[/tab name]");

		ScriptFileParserNew parser = new ScriptFileParserNew(File, Pvf);
		parser.PraseStructureMain();
		parser.Sections.RemoveAll(section =>
			string.Equals(section.GetSectionName(), "[sell item]", StringComparison.OrdinalIgnoreCase) ||
			string.Equals(section.GetSectionName(), "[tab name]", StringComparison.OrdinalIgnoreCase));
		return AppendSections(parser.GetText(), sellItems.ToString() + tabNames.ToString());
	}

	private static string AppendSections(string originalText, string sections)
	{
		StringBuilder output = new StringBuilder(originalText ?? string.Empty);
		if (output.Length > 0 && output[output.Length - 1] != '\r' && output[output.Length - 1] != '\n')
		{
			output.AppendLine();
		}
		output.Append(sections);
		return output.ToString();
	}

	[Command]
	public void OnAddPage()
	{
		ConcurrentObservableCollection<NpcShopItem> concurrentObservableCollection = new ConcurrentObservableCollection<NpcShopItem>();
		for (int i = 0; i < 49; i++)
		{
			concurrentObservableCollection.Add(new NpcShopItem());
		}
		NpcShopPageViewModel npcShopPageViewModel = new NpcShopPageViewModel("NpcShop", concurrentObservableCollection);
		Pages.Add(npcShopPageViewModel);
		CurrentPage = npcShopPageViewModel;
	}

	[Command]
	public void OnDeletePage(NpcShopPageViewModel page)
	{
		if (page?.Items.Any(item => item.IsPurchaseDataModified) == true)
		{
			AppCore.ShowMsg("当前页包含尚未保存的商品购买属性，请先编译保存。", isError: true);
			return;
		}
		if (page != null && AppCore.Logger.ShowDialog("确定要删除当前页：" + page.Title + "？") == MessageResult.Yes)
		{
			Pages.Remove(page);
			if (Pages.Count > 0)
			{
				CurrentPage = Pages.FirstOrDefault();
			}
		}
	}

	[Command]
	public void OnMoveLeftPage()
	{
		if (CurrentPage != null)
		{
			int num = Pages.IndexOf(CurrentPage);
			if (num > 0)
			{
				Pages.SwapItem(CurrentPage, Pages[num - 1]);
			}
		}
	}

	[Command]
	public void OnMoveRightPage()
	{
		if (CurrentPage != null)
		{
			int num = Pages.IndexOf(CurrentPage);
			if (num < Pages.Count - 1)
			{
				Pages.SwapItem(CurrentPage, Pages[num + 1]);
			}
		}
	}

	[Command]
	public void FindNpsShopQuerySubmitted(Tuple<object, object> tuple)
	{
		_003C_003Ec__DisplayClass56_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass56_0();
		CS_0024_003C_003E8__locals4.Si3drpR7Ex = (AutoSuggestEditQuerySubmittedEventArgs)tuple.Item2;
		AutoSuggestEdit autoSuggestEdit = (AutoSuggestEdit)tuple.Item1;
		if (CS_0024_003C_003E8__locals4.Si3drpR7Ex.Text != null && CS_0024_003C_003E8__locals4.Si3drpR7Ex.Text.Length > 0)
		{
			IEnumerable<FindNpcShopSource> enumerable = NpcShopList.Where((FindNpcShopSource it) => it.Find(CS_0024_003C_003E8__locals4.Si3drpR7Ex.Text));
			if (enumerable != null && enumerable.Any())
			{
				autoSuggestEdit.ItemsSource = enumerable;
			}
		}
		else
		{
			autoSuggestEdit.ItemsSource = NpcShopList;
		}
	}

	[Command]
	public void FindNpcShopSuggestionChosen(AutoSuggestEditSuggestionChosenEventArgs e)
	{
		CurrentNpcShop = (FindNpcShopSource)e.SelectedItem;
	}

	[Command]
	public void DragRecordOver(DragRecordOverEventArgs args)
	{
		if (args.IsFromOutside && typeof(NpcShopItem).IsAssignableFrom(args.GetRecordType()))
		{
			args.Effects = DragDropEffects.Move;
			args.Handled = true;
		}
	}

	[Command]
	public void ShopPageDropRecord(DropRecordEventArgs args)
	{
		args.Effects = DragDropEffects.None;
		args.Handled = true;
		RecordDragDropData recordDragDropData = (RecordDragDropData)args.Data.GetData(typeof(RecordDragDropData));
		NpcShopItem npcShopItem = (NpcShopItem)args.TargetRecord;
		List<NpcShopItem> list = (from it in recordDragDropData.Records.OfType<NpcShopItemSource>()
			select new NpcShopItem(it.ItemCode)).ToList();
		if (npcShopItem == null && CurrentPage.Items.Count > 0)
		{
			npcShopItem = CurrentPage.Items.Last();
		}
		int num = 0;
		if (npcShopItem != null)
		{
			num = CurrentPage.Items.IndexOf(npcShopItem);
		}
		if (args.GetRecordType().ToString() == typeof(NpcShopItem).ToString())
		{
			if (recordDragDropData == null)
			{
				return;
			}
			if (recordDragDropData.Records.Length == 1)
			{
				NpcShopItem npcShopItem2 = recordDragDropData.Records.FirstOrDefault() as NpcShopItem;
				CurrentPage?.Items?.SwapItem(npcShopItem, npcShopItem2);
				CurrentPage.SelectedItems.Clear();
				CurrentPage.SelectedItems.Add(npcShopItem2);
				CurrentPage.CurrentItem = npcShopItem2;
				return;
			}
		}
		else
		{
			if (recordDragDropData == null)
			{
				return;
			}
			if (npcShopItem == null)
			{
				CurrentPage.Items.AddRange(list);
				CurrentPage.CheckItems();
				return;
			}
			if (npcShopItem.ItemCode == -1)
			{
				int num2 = list.Count();
				bool flag = true;
				for (int num3 = 0; num3 < num2; num3++)
				{
					if (num + num3 < CurrentPage.Items.Count && CurrentPage.Items[num + num3].ItemCode != -1)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					int count = CurrentPage.Items.Count;
					int num4 = -1;
					for (int num5 = 0; num5 < num2; num5++)
					{
						if (num + num5 >= count)
						{
							num4 = num5;
							break;
						}
						CurrentPage.Items[num + num5] = list.ElementAt(num5);
					}
					if (num4 != -1)
					{
						CurrentPage.Items.AddRange(list.Skip(num4));
					}
				}
				else
				{
					CurrentPage.Items.InsertRange(num, list);
				}
			}
			else
			{
				CurrentPage.Items.InsertRange(num + 1, list);
			}
			CurrentPage.CheckItems();
		}
		CurrentPage.CurrentItem = list.FirstOrDefault();
	}
}
