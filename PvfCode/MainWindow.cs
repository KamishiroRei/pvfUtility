using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Editors;
using PvfCode.Controls;
using PvfCode.Dot.Desktop;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.Options;
using PvfCode.Services;
using PvfCode.ViewModels;
using PvfCode.ViewModels.DocumentFolder;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.Views;
using PvfCode.Views.NpcShopEditor;
using PvfCode.Views.Dialogs;
using PvfCode.Views.ImportViews;
using UnitComboLib.ViewModels;
using Utools;
using WinCopies.Util;
using AniDesignerWindow = PvfCode.Views.AniDesigner.WindowAniDesigner;

namespace PvfCode;

public class MainWindow : ThemedWindow, IComponentConnector, IStyleConnector
{
	private static readonly string[] OnlineMenuResourceKeys =
	{
		"mainWin_bar_Main_subItem_BookMark_BookMarkStore",
		"mainWin_bar_Main_subItem_Macro_MacroStore",
		"mainWin_bar_Main_subItem_ExtensionStore",
		"mainWin_bar_Main_subItem_ExtensionStore_MacroStore",
		"mainWin_bar_Main_subItem_ExtensionStore_BookMarkStore",
		"mainWin_bar_Main_subItem_ExtensionStore_TreeListCommentStore",
		"mainWin_bar_Main_subItem_ExtensionStore_SectionTranslateStore",
		"mainWin_bar_Main_subItem_ExtensionStore_CodeIntelliSenseStore",
		"GlobalSearchWindow_MacroStore"
	};

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass5_0
	{
		public KeyValuePair<string, BookMarkDto> item;

		public _003C_003Ec__DisplayClass5_0()
		{
		}

		internal void z0MwYXoufi()
		{
			AppCore.ViewModelBase.RootDocument.BookMarkOpenDocument(item.Value.FilePath);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass6_0
	{
		public KeyValuePair<string, BookMarkDto> item;

		public _003C_003Ec__DisplayClass6_0()
		{
		}

		internal void ySvwyRCTAI()
		{
			AppCore.ViewModelBase.RootDocument.BookMarkOpenDocument(item.Value.FilePath);
		}
	}

	private bool LXtjTCGkg9;

	internal MainWindow mainWindow;

	internal TaskbarButtonService tttt;

	internal NotificationService notificationService;

	internal BarSubItem subFile;

	internal BarSubItem subPvfOpenLog;

	internal BarSubItem subBookmark;

	internal DockLayoutManager DemoDockContainer;

	internal LayoutGroup Root;

	internal LayoutPanel FilelistLayoutPanel;

	internal PvfTreeViewGroup treeListControlGroupEx;

	internal DocumentGroup DocumentHost;

	internal LayoutPanel FindView;

	internal LayoutPanel outPutView;

	internal LayoutPanel ErrorListPanel;

	internal LayoutPanel findResultView;

	private bool lryjCUlPWT;

	public static string ApplicationID => "FunWithNotifications_19_1";

	public MainWindow()
	{
		PvfSkillTreeColorBehavior.Initialize();
		base.DataContext = (AppCore.ViewModelBase = new MainWindowViewModel());
		InitializeComponent();
		HideOnlineFeatures();
		HideDevelopmentTestButton();
		Dispatcher.BeginInvoke((Action)(() =>
		{
			HideOnlineFeatures();
			HideDevelopmentTestButton();
		}), DispatcherPriority.ApplicationIdle);
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Height = primaryScreenHeight * 0.8;
		base.Width = primaryScreenWidth * 0.8;
		base.Loaded += a1ZlXjd326;
		subFile.Popup += kscl7af6Nn;
		subBookmark.Popup += gXPlkxAdHB;
		(AppSetting.Instance.EditConfig.SizeUnitLabel as UnitViewModel).EventScreenPointsChanged += yY3lJYMd1s;
	}

	private void HideOnlineFeatures()
	{
		BarManager barManager = Content as BarManager ?? BarManager.GetBarManager(this);
		if (barManager == null)
		{
			return;
		}

		HeaderItems.Clear();
		HashSet<string> onlineCaptions = GetOnlineMenuCaptions();

		foreach (BarItem item in barManager.Items)
		{
			if (onlineCaptions.Contains(item.Content?.ToString() ?? string.Empty))
			{
				item.IsVisible = false;
			}
		}
		HideOnlineFeatureLinks(this, onlineCaptions);
	}

	private static HashSet<string> GetOnlineMenuCaptions()
	{
		HashSet<string> captions = OnlineMenuResourceKeys
			.Select(key => Application.Current?.TryFindResource(key)?.ToString())
			.Where(value => !string.IsNullOrWhiteSpace(value))
			.ToHashSet(StringComparer.Ordinal);
		captions.Add("chatGPT");
		return captions;
	}

	private static void HideOnlineFeatureLinks(DependencyObject parent, HashSet<string> onlineCaptions)
	{
		int childCount = VisualTreeHelper.GetChildrenCount(parent);
		for (int index = 0; index < childCount; index++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(parent, index);
			if (child is LightweightBarItemLinkControl linkControl &&
				onlineCaptions.Contains(linkControl.ActualContent?.ToString() ?? string.Empty))
			{
				linkControl.Link.IsVisible = false;
				linkControl.Link.Item.IsVisible = false;
				linkControl.Visibility = Visibility.Collapsed;
			}
			HideOnlineFeatureLinks(child, onlineCaptions);
		}
	}

	private void HideDevelopmentTestButton()
	{
		BarManager barManager = Content as BarManager ?? BarManager.GetBarManager(this);
		BarButtonItem testButton = barManager?.Items
			.OfType<BarButtonItem>()
			.FirstOrDefault(item => string.Equals(item.Content?.ToString(), "Test", StringComparison.Ordinal));
		if (testButton != null)
		{
			testButton.IsVisible = false;
		}
		HideDevelopmentTestLinks(this);
	}

	private static void HideDevelopmentTestLinks(DependencyObject parent)
	{
		int childCount = VisualTreeHelper.GetChildrenCount(parent);
		for (int index = 0; index < childCount; index++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(parent, index);
			if (child is LightweightBarItemLinkControl linkControl &&
				string.Equals(linkControl.ActualContent?.ToString(), "Test", StringComparison.Ordinal))
			{
				linkControl.Link.IsVisible = false;
				linkControl.Link.Item.IsVisible = false;
				linkControl.Visibility = Visibility.Collapsed;
			}
			HideDevelopmentTestLinks(child);
		}
	}

	private async void mGBlZlcndb(object? sender, EventArgs P_1)
	{
		EventManager.RegisterClassHandler(typeof(LightweightBarItemLinkControl), FrameworkElement.LoadedEvent, (RoutedEventHandler)OnBarItemLinkLoaded);
		EventManager.RegisterClassHandler(typeof(BarItem), FrameworkElement.LoadedEvent, (RoutedEventHandler)SetInitialToolTipDelay);
		EventManager.RegisterClassHandler(typeof(Label), FrameworkElement.LoadedEvent, (RoutedEventHandler)OnLabelLoaded);
		EventManager.RegisterClassHandler(typeof(TextBlock), FrameworkElement.LoadedEvent, (RoutedEventHandler)OnTextBlockLoaded);
		EventManager.RegisterClassHandler(typeof(SearchControl), FrameworkElement.LoadedEvent, (RoutedEventHandler)OnSearchControlLoaded);
		EventManager.RegisterClassHandler(typeof(PopupColorEdit), FrameworkElement.LoadedEvent, (RoutedEventHandler)SetInitialToolTipDelay);
		EventManager.RegisterClassHandler(typeof(HyperlinkEdit), FrameworkElement.LoadedEvent, (RoutedEventHandler)OnHyperlinkEditLoaded);
		if (!AppSetting.Instance.FirstTime.HasValue)
		{
			AppSetting.Instance.FirstTime = false;
			WindowSelectDefaultTheme windowSelectDefaultTheme = new WindowSelectDefaultTheme();
			windowSelectDefaultTheme.Owner = this;
			windowSelectDefaultTheme.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			windowSelectDefaultTheme.Show();
			await AppSetting.Instance.SaveSetting();
		}
		AppSetting.Instance.TreeSetting.EventTreeShowNpkIconChanged += TreeSetting_EventTreeShowNpkIconChanged;
	}

	private static void SetInitialToolTipDelay(object sender, RoutedEventArgs e)
	{
		ToolTipService.SetInitialShowDelay((DependencyObject)sender, 50);
	}

	private static void OnBarItemLinkLoaded(object sender, RoutedEventArgs e)
	{
		SetInitialToolTipDelay(sender, e);
		LightweightBarItemLinkControl linkControl = (LightweightBarItemLinkControl)sender;
		string content = linkControl.ActualContent?.ToString();
		if (GetOnlineMenuCaptions().Contains(content ?? string.Empty))
		{
			linkControl.Link.IsVisible = false;
			linkControl.Link.Item.IsVisible = false;
			linkControl.Visibility = Visibility.Collapsed;
		}
		else if (content == "GotoLine")
		{
			linkControl.Link.Item.Content = "跳转到偏移量";
		}
		else if (content == "DocumentTextEditor_BarToolControl_CommentSelectedLine")
		{
			linkControl.Link.Item.Content = "注释选中行";
		}
	}

	private static void OnSearchControlLoaded(object sender, RoutedEventArgs e)
	{
		SearchControl searchControl = (SearchControl)sender;
		if (Window.GetWindow(searchControl) is WinNpcShopEditor)
		{
			searchControl.Style = new Style(typeof(SearchControl));
			searchControl.Background = System.Windows.Media.Brushes.Transparent;
		}
	}

	private static void OnLabelLoaded(object sender, RoutedEventArgs e)
	{
		SetInitialToolTipDelay(sender, e);
		Label label = (Label)sender;
		Window owner = Window.GetWindow(label);
		if (owner is WinNpcShopEditor && string.Equals(label.Content?.ToString(), "asdf", StringComparison.Ordinal))
		{
			SearchControl searchControl = FindVisualParent<SearchControl>(label);
			if (searchControl != null)
			{
				searchControl.Style = new Style(typeof(SearchControl));
				searchControl.Background = System.Windows.Media.Brushes.Transparent;
			}
			label.Visibility = Visibility.Collapsed;
		}
		else if (owner is AniDesignerWindow && string.Equals(label.Content?.ToString(), "开发阶段", StringComparison.Ordinal))
		{
			label.Visibility = Visibility.Collapsed;
		}
	}

	private static void OnTextBlockLoaded(object sender, RoutedEventArgs e)
	{
		TextBlock textBlock = (TextBlock)sender;
		if (Window.GetWindow(textBlock) is not WinNpcShopEditor || !string.Equals(textBlock.Text?.Trim(), "Справка", StringComparison.Ordinal))
		{
			return;
		}
		SimpleButton helpButton = FindVisualParent<SimpleButton>(textBlock);
		if (helpButton != null)
		{
			helpButton.Visibility = Visibility.Collapsed;
			StackPanel searchPanel = FindVisualParent<StackPanel>(helpButton);
			if (searchPanel != null)
			{
				searchPanel.Background = System.Windows.Media.Brushes.Transparent;
			}
		}
	}

	private static void OnHyperlinkEditLoaded(object sender, RoutedEventArgs e)
	{
		SetInitialToolTipDelay(sender, e);
		HyperlinkEdit hyperlink = (HyperlinkEdit)sender;
		if (string.Equals(hyperlink.EditValue?.ToString(), "赞助商 xxx.qq.com", StringComparison.Ordinal) &&
			string.Equals(hyperlink.NavigationUrl?.ToString(), "https://www.baidu.com", StringComparison.OrdinalIgnoreCase))
		{
			hyperlink.IsEnabled = false;
			hyperlink.Visibility = Visibility.Collapsed;
		}
	}

	private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
	{
		DependencyObject parent = VisualTreeHelper.GetParent(child);
		while (parent != null)
		{
			if (parent is T match)
			{
				return match;
			}
			parent = VisualTreeHelper.GetParent(parent);
		}
		return null;
	}

	private async void TreeSetting_EventTreeShowNpkIconChanged()
	{
		if (ImagePack2Service.Instance.Count == 0)
		{
			return;
		}
		AppCore.ViewModelBase.ImagePacks2ViewModel.IsLoading = true;
		try
		{
			if (AppSetting.Instance.TreeSetting.TreeShowNpkIcon && !ImagePack2Service.Instance.IsLoadedTreeIcon() && !ImagePack2Service.Instance.IsWork)
			{
				if (!string.IsNullOrEmpty(AppSetting.Instance.ImagePacks2Options.ImagePacks2Path) && AppCore.ViewModelBase.PVF.PvfIsOpen)
				{
					await Task.Run(delegate
					{
						ImagePack2Service.Instance.LoadTreeIcons(AppCore.ViewModelBase.PVF);
					});
					await AppCore.ClearMemory();
				}
			}
			else if (!AppSetting.Instance.TreeSetting.TreeShowNpkIcon)
			{
				ImagePack2Service.Instance.ClearTreeIcon();
				await AppCore.ClearMemory();
				AppCore.Logger.Success(AppCore.Logger.GetStrNoReplace("Mess_NpkIconsApplyToFileExplorer_Prohibited"));
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TreeSetting_EventTreeShowNpkIconChanged");
		}
		AppCore.ViewModelBase.ImagePacks2ViewModel.IsLoading = false;
	}

	private async void yY3lJYMd1s()
	{
		await AppSetting.Instance.SaveSetting();
	}

	private void gXPlkxAdHB(object? sender, EventArgs P_1)
	{
		foreach (BarItemLinkBase dynamicLink in subBookmark.ItemLinks.ToArray().Skip(1))
		{
			subBookmark.ItemLinks.Remove(dynamicLink);
		}
		subBookmark.ItemLinks.Add(new BarItemSeparator());
		foreach (KeyValuePair<string, BookMarkDto> menuItem in GetBookmarkMenuItems())
		{
			_003C_003Ec__DisplayClass5_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass5_0();
			CS_0024_003C_003E8__locals7.item = menuItem;
			if (CS_0024_003C_003E8__locals7.item.Value.IsFile)
			{
				subBookmark.ItemLinks.Add(new BarButtonItem
				{
					Content = CS_0024_003C_003E8__locals7.item.Key,
					Glyph = Res.Instance.TreeFiles.Script_16x,
					Command = new DelegateCommand(delegate
					{
						AppCore.ViewModelBase.RootDocument.BookMarkOpenDocument(CS_0024_003C_003E8__locals7.item.Value.FilePath);
					})
				});
				continue;
			}
			BarSubItem barSubItem = new BarSubItem
			{
				Content = CS_0024_003C_003E8__locals7.item.Key,
				Glyph = Res.Instance.TreeFiles.FolderClosed
			};
			subBookmark.ItemLinks.Add(barSubItem);
			if (CS_0024_003C_003E8__locals7.item.Value.HaveChildren())
			{
				BeIl0g7GfH(CS_0024_003C_003E8__locals7.item.Value.Children, barSubItem);
			}
		}
	}

	private static IEnumerable<KeyValuePair<string, BookMarkDto>> GetBookmarkMenuItems()
	{
		BookMarkDto root = AppSetting.Instance.BookMarkGroup.Trees.Values.FirstOrDefault();
		if (root == null)
		{
			yield break;
		}
		foreach (KeyValuePair<string, BookMarkDto> item in OrderBookmarks(root.Children))
		{
			if (string.Equals(item.Key, "默认书签", StringComparison.Ordinal) && !item.Value.IsFile)
			{
				foreach (KeyValuePair<string, BookMarkDto> defaultItem in OrderBookmarks(item.Value.Children))
				{
					yield return defaultItem;
				}
				continue;
			}
			yield return item;
		}
	}

	private static IOrderedEnumerable<KeyValuePair<string, BookMarkDto>> OrderBookmarks(IEnumerable<KeyValuePair<string, BookMarkDto>> bookmarks)
	{
		return bookmarks
			.OrderBy(item => item.Value.Sort)
			.ThenBy(item => item.Key, StringComparer.CurrentCulture);
	}

	private void BeIl0g7GfH(IDictionary<string, BookMarkDto> P_0, BarSubItem P_1)
	{
		using Dictionary<string, BookMarkDto>.Enumerator enumerator = P_0.OrderBy<KeyValuePair<string, BookMarkDto>, int>((KeyValuePair<string, BookMarkDto> x) => x.Value.Sort).ToDictionary((KeyValuePair<string, BookMarkDto> x) => x.Key, (KeyValuePair<string, BookMarkDto> x) => x.Value).GetEnumerator();
		while (enumerator.MoveNext())
		{
			_003C_003Ec__DisplayClass6_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass6_0();
			CS_0024_003C_003E8__locals7.item = enumerator.Current;
			if (CS_0024_003C_003E8__locals7.item.Value.IsFile)
			{
				P_1.ItemLinks.Add(new BarButtonItem
				{
					Content = CS_0024_003C_003E8__locals7.item.Key,
					Glyph = Res.Instance.TreeFiles.Script_16x,
					Command = new DelegateCommand(delegate
					{
						AppCore.ViewModelBase.RootDocument.BookMarkOpenDocument(CS_0024_003C_003E8__locals7.item.Value.FilePath);
					})
				});
				continue;
			}
			BarSubItem barSubItem = new BarSubItem
			{
				Content = CS_0024_003C_003E8__locals7.item.Key,
				Glyph = Res.Instance.TreeFiles.FolderClosed
			};
			P_1.ItemLinks.Add(barSubItem);
			if (CS_0024_003C_003E8__locals7.item.Value.HaveChildren())
			{
				BeIl0g7GfH(CS_0024_003C_003E8__locals7.item.Value.Children, barSubItem);
			}
		}
	}

	private void kscl7af6Nn(object? sender, EventArgs P_1)
	{
		bool flag = false;
		BarItemLinkBase[] array = subFile.ItemLinks.ToArray();
		foreach (BarItemLinkBase barItemLinkBase in array)
		{
			if (flag)
			{
				subFile.ItemLinks.Remove(barItemLinkBase);
			}
			if (barItemLinkBase.ActualContent != null && barItemLinkBase.ActualContent.ToString() == AppCore.Logger.GetStr("mainWin_bar_Main_subItem_ClearPvfPackOpenLogs"))
			{
				flag = true;
			}
		}
		subFile.ItemLinks.Add(new BarItemSeparator());
		foreach (BarItemLinkBase itemLink in subPvfOpenLog.ItemLinks)
		{
			subFile.ItemLinks.Add(itemLink);
		}
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		if (AppCore.ViewModelBase.PVF.PvfIsOpen && AppCore.Logger.ShowDialog(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ExitApp"), AppSetting.Instance.AppName)) != MessageResult.Yes)
		{
			e.Cancel = true;
			return;
		}
		RgVlpJNt5a();
		try
		{
			WebApiServer.Instance.Stop();
		}
		catch (Exception)
		{
		}
		App.OnExit();
	}

	private void a1ZlXjd326(object P_0, RoutedEventArgs P_1)
	{
		if (!LXtjTCGkg9)
		{
			LXtjTCGkg9 = true;
			AppSetting.Instance.EditConfig.InitFoldingGuideLineBurshs(this);
			BaWlUHWDu1();
			mGBlZlcndb(null, null);
		}
	}

	private async void RgVlpJNt5a()
	{
		try
		{
			using MemoryStream memStream = new MemoryStream();
			DemoDockContainer.SaveLayoutToStream(memStream);
			memStream.Seek(0L, SeekOrigin.Begin);
			string encryptStr = BytesHelper.BytesToString(BytesHelper.StreamToBytes(memStream));
			await File.WriteAllTextAsync(AppSetting.LayoutSavePath, encryptStr.TextEncrypt(AppSetting.ConfigPwd));
		}
		catch (Exception ex)
		{
			AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger().GetStr("mess_SaveLayoutError"), ex.Message));
		}
	}

	private async void BaWlUHWDu1()
	{
		try
		{
			string text = Application.ResourceAssembly.GetName().Version.ToString();
			if (text != AppSetting.Instance.Version)
			{
				AppSetting.Instance.Version = text;
				try
				{
					if (File.Exists(AppSetting.LayoutSavePath))
					{
						File.Delete(AppSetting.LayoutSavePath);
					}
				}
				catch (Exception)
				{
				}
			}
			if (File.Exists(AppSetting.LayoutSavePath))
			{
				try
				{
					Stream stream = BytesHelper.StringToBytes((await File.ReadAllTextAsync(AppSetting.LayoutSavePath)).TextDecrypt(AppSetting.ConfigPwd)).BytesToStream();
					stream.Seek(0L, SeekOrigin.Begin);
					DemoDockContainer.RestoreLayoutFromStream(stream);
					return;
				}
				catch (Exception)
				{
					return;
				}
			}
		}
		catch (Exception ex3)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_LoadLayoutError"), ex3.Message));
		}
	}

	private void eNclcdP4GG(object P_0, ItemClickEventArgs P_1)
	{
		if (LXtjTCGkg9)
		{
			AppSetting.Instance.GetIlogger()?.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ModifySuccess"), AppSetting.Instance.AppName));
		}
	}

	private void Tqll8q95Ns(object P_0, ShowingMenuEventArgs P_1)
	{
		BarButtonItem barButtonItem = null;
		BarButtonItem barButtonItem2 = null;
		BarButtonItem barButtonItem3 = null;
		IBarItem[] array = P_1.Menu.Items.ToArray();
		foreach (IBarItem barItem in array)
		{
			if (!(barItem is BarButtonItem))
			{
				continue;
			}
			BarButtonItem barButtonItem4 = barItem as BarButtonItem;
			if (!(barButtonItem4.Content is string text) || text == null)
			{
				continue;
			}
			switch (text.Length)
			{
			case 4:
				switch (text[0])
				{
				case 'D':
					if (text == "Dock")
					{
						barButtonItem4.Content = AppSetting.Instance.GetIlogger().GetStr("mess_Document_ContextMenu_Name_Dock");
					}
					break;
				case 'H':
					if (text == "Hide")
					{
						barButtonItem4.Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_Hide");
						barButtonItem2 = barButtonItem4;
					}
					break;
				}
				break;
			case 5:
				switch (text[0])
				{
				case 'F':
					if (text == "Float")
					{
						barButtonItem4.Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_Float");
						if (AppSetting.Instance.PvfDocumentOptions.CaptionNotAllowMenuItemNameIsFloat)
						{
							barButtonItem4.IsVisible = false;
						}
					}
					break;
				case 'C':
					if (text == "Close")
					{
						barButtonItem4.Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_Hide");
						barButtonItem2 = barButtonItem4;
					}
					break;
				}
				break;
			case 9:
				if (text == "Auto Hide")
				{
					barButtonItem4.Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_AutoHide");
					barButtonItem3 = barButtonItem4;
				}
				break;
			case 18:
				if (text == "Close all but this")
				{
					barButtonItem4.Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_CloseAllExceptThis");
					barButtonItem = barButtonItem4;
					barButtonItem.Command = null;
					barButtonItem.ItemClick -= T4klMgUCnt;
					barButtonItem.ItemClick += T4klMgUCnt;
				}
				break;
			case 7:
				if (text == "Pin Tab")
				{
					barButtonItem4.IsVisible = false;
				}
				break;
			case 24:
				if (text == "New horizontal tab group")
				{
					barButtonItem4.Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_NewVerticalDocumentGroup");
					if (AppSetting.Instance.PvfDocumentOptions.CaptionShowNotAllowItemNewHorizontalTabGroup)
					{
						barButtonItem4.IsVisible = false;
					}
				}
				break;
			case 22:
				if (text == "New vertical tab group")
				{
					barButtonItem4.Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_NewHorizontalDocumentGroup");
					if (AppSetting.Instance.PvfDocumentOptions.CaptionNotAllowMenuItemNewVerticalTabGroup)
					{
						barButtonItem4.IsVisible = false;
					}
				}
				break;
			case 26:
				if (text == "Move to previous tab group")
				{
					barButtonItem4.Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_MoveToMainDocumentGroup");
				}
				break;
			}
		}
		if (barButtonItem != null)
		{
			CommonBarItemCollection items = P_1.Menu.Items;
			if (barButtonItem2 != null)
			{
				items.Remove(barButtonItem2);
				barButtonItem2.Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_Close");
				barButtonItem2.KeyGesture = new KeyGesture((Key)66, (ModifierKeys)2);
			}
			items.Remove(barButtonItem);
			BarButtonItem barButtonItem5 = new BarButtonItem
			{
				Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_CloseRightDocuments")
			};
			barButtonItem5.ItemClick -= ihXl39VClR;
			barButtonItem5.ItemClick += ihXl39VClR;
			BarButtonItem barButtonItem6 = new BarButtonItem
			{
				Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_CloseLeftDocuments")
			};
			barButtonItem6.ItemClick -= lQAlRjdWxI;
			barButtonItem6.ItemClick += lQAlRjdWxI;
			BarButtonItem barButtonItem7 = new BarButtonItem
			{
				Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_CloseAllDocuments"),
				Glyph = Res.Instance.ClearWindowContent
			};
			barButtonItem7.ItemClick -= rOVlVpTRPT;
			barButtonItem7.ItemClick += rOVlVpTRPT;
			items.Add(barButtonItem7);
			items.Add(new BarItemSeparator());
			if (barButtonItem2 != null)
			{
				items.Insert(0, barButtonItem2);
			}
			items.Insert(1, barButtonItem);
			items.Insert(2, new BarItemSeparator());
			items.Insert(3, barButtonItem5);
			items.Insert(4, barButtonItem6);
			if (barButtonItem3 != null)
			{
				items.Remove(barButtonItem3);
			}
		}
	}

	private void T4klMgUCnt(object P_0, ItemClickEventArgs P_1)
	{
		string documentPath = AppCore.ViewModelBase.RootDocument.Documents.Where((DocumentBase it) => it.IsActive).FirstOrDefault().DocumentPath;
		Dictionary<string, DocumentBase> dictionary = new Dictionary<string, DocumentBase>();
		DocumentBase[] array = AppCore.ViewModelBase.RootDocument.Documents.ToArray();
		foreach (DocumentBase documentBase in array)
		{
			if (documentBase.DocumentPath != documentPath)
			{
				dictionary.Add(documentBase.DocumentPath, documentBase);
			}
		}
		if (dictionary != null && dictionary.Count > 0)
		{
			IEnumerable<string> enumerable = from it in dictionary.Values
				where it is PvfFileDocument pvfFileDocument && pvfFileDocument.TextIsChanged
				select ((PvfFileDocument)it).FullPath;
			if (enumerable.Any())
			{
				switch (AppCore.ShowDialogStringListViewModelResult(new DialogStringListViewModel(enumerable, AppSetting.Instance.GetIlogger()?.GetStr("mess_ExistUnsavedFile"), AppSetting.Instance.GetIlogger()?.GetStr("common_Save"), AppSetting.Instance.GetIlogger()?.GetStr("common_NotSave"), AppSetting.Instance.GetIlogger()?.GetStr("common_Cancel"))))
				{
				case DialogStringListViewModelResult.Cancel:
					return;
				case DialogStringListViewModelResult.Yes:
					foreach (PvfFileDocument item in dictionary.Values.Where((DocumentBase it) => it is PvfFileDocument pvfFileDocument && pvfFileDocument.TextIsChanged))
					{
						item.OnSave();
					}
					break;
				}
			}
		}
		AppCore.IsSaveAllDocument = true;
		foreach (KeyValuePair<string, DocumentBase> item2 in dictionary)
		{
			AppCore.ViewModelBase.RootDocument.RemoveDocument(item2.Key, isShowDialog: false);
		}
		AppCore.IsSaveAllDocument = false;
	}

	private void rOVlVpTRPT(object P_0, ItemClickEventArgs P_1)
	{
		List<string> notSaveFiles = AppCore.ViewModelBase.RootDocument.GetNotSaveFiles();
		if (notSaveFiles != null && notSaveFiles.Count > 0)
		{
			switch (AppCore.ShowDialogStringListViewModelResult(new DialogStringListViewModel(notSaveFiles, AppSetting.Instance.GetIlogger()?.GetStr("mess_ExistUnsavedFile"), AppSetting.Instance.GetIlogger()?.GetStr("common_Save"), AppSetting.Instance.GetIlogger()?.GetStr("common_NotSave"), AppSetting.Instance.GetIlogger()?.GetStr("common_Cancel"))))
			{
			case DialogStringListViewModelResult.Cancel:
				return;
			case DialogStringListViewModelResult.Yes:
				AppCore.ViewModelBase.RootDocument.SaveAllDocument();
				break;
			}
		}
		AppCore.ViewModelBase.RootDocument.Clear();
	}

	private void ihXl39VClR(object P_0, ItemClickEventArgs P_1)
	{
		BaseLayoutItemCollection items = DocumentHost.Items;
		bool flag = false;
		Dictionary<string, DocumentBase> dictionary = new Dictionary<string, DocumentBase>();
		BaseLayoutItem[] array = items.ToArray();
		foreach (BaseLayoutItem baseLayoutItem in array)
		{
			if (flag)
			{
				DocumentBase documentBase = (baseLayoutItem as DocumentPanel).Content as DocumentBase;
				dictionary.Add(documentBase.DocumentPath, documentBase);
			}
			if (baseLayoutItem.IsActive)
			{
				flag = true;
			}
		}
		if (dictionary.Values.Where((DocumentBase it) => it is PvfFileDocument pvfFileDocument && pvfFileDocument.TextIsChanged).Any())
		{
			IEnumerable<PvfFileDocument> enumerable = from it in dictionary.Values
				where it is PvfFileDocument pvfFileDocument && pvfFileDocument.TextIsChanged
				select (PvfFileDocument)it;
			switch (AppCore.ShowDialogStringListViewModelResult(new DialogStringListViewModel(enumerable.Select((PvfFileDocument it) => it.FullPath), AppSetting.Instance.GetIlogger()?.GetStr("mess_ExistUnsavedFile"), AppSetting.Instance.GetIlogger()?.GetStr("common_Save"), AppSetting.Instance.GetIlogger()?.GetStr("common_NotSave"), AppSetting.Instance.GetIlogger()?.GetStr("common_Cancel"))))
			{
			case DialogStringListViewModelResult.Cancel:
				return;
			case DialogStringListViewModelResult.Yes:
				foreach (PvfFileDocument item in enumerable)
				{
					item.OnSave();
				}
				break;
			}
		}
		AppCore.IsSaveAllDocument = true;
		foreach (KeyValuePair<string, DocumentBase> item2 in dictionary)
		{
			AppCore.ViewModelBase.RootDocument.RemoveDocument(item2.Key, isShowDialog: false);
		}
		AppCore.IsSaveAllDocument = false;
	}

	private void lQAlRjdWxI(object P_0, ItemClickEventArgs P_1)
	{
		BaseLayoutItemCollection items = DocumentHost.Items;
		bool flag = false;
		List<BaseLayoutItem> list = items.ToList();
		list.Reverse();
		Dictionary<string, DocumentBase> dictionary = new Dictionary<string, DocumentBase>();
		foreach (BaseLayoutItem item in list)
		{
			if (flag)
			{
				DocumentBase documentBase = (item as DocumentPanel).Content as DocumentBase;
				dictionary.Add(documentBase.DocumentPath, documentBase);
			}
			if (item.IsActive)
			{
				flag = true;
			}
		}
		if (dictionary.Values.Where((DocumentBase it) => it is PvfFileDocument pvfFileDocument && pvfFileDocument.TextIsChanged).Any())
		{
			IEnumerable<PvfFileDocument> enumerable = from it in dictionary.Values
				where it is PvfFileDocument pvfFileDocument && pvfFileDocument.TextIsChanged
				select (PvfFileDocument)it;
			switch (AppCore.ShowDialogStringListViewModelResult(new DialogStringListViewModel(enumerable.Select((PvfFileDocument it) => it.FullPath), AppSetting.Instance.GetIlogger()?.GetStr("mess_ExistUnsavedFile"), AppSetting.Instance.GetIlogger()?.GetStr("common_Save"), AppSetting.Instance.GetIlogger()?.GetStr("common_NotSave"), AppSetting.Instance.GetIlogger()?.GetStr("common_Cancel"))))
			{
			case DialogStringListViewModelResult.Cancel:
				return;
			case DialogStringListViewModelResult.Yes:
				foreach (PvfFileDocument item2 in enumerable)
				{
					item2.OnSave();
				}
				break;
			}
		}
		AppCore.IsSaveAllDocument = true;
		foreach (KeyValuePair<string, DocumentBase> item3 in dictionary)
		{
			AppCore.ViewModelBase.RootDocument.RemoveDocument(item3.Key, isShowDialog: false);
		}
		AppCore.IsSaveAllDocument = false;
	}

	private async void co4lNaLNxk(object P_0, ItemClickEventArgs P_1)
	{
		try
		{
			if (AppCore.ViewModelBase.RootDocument.Documents.Any())
			{
				if (AppCore.Logger.ShowDialog(AppSetting.Instance.GetIlogger()?.GetStr("mess_ResetWindowLayout")) != MessageResult.Yes)
				{
					return;
				}
				AppCore.ViewModelBase.RootDocument.Documents.Clear();
			}
			await File.WriteAllBytesAsync(AppSetting.LayoutSavePath, Resource1.pvfUtilityLayout);
			Stream stream = BytesHelper.StringToBytes((await File.ReadAllTextAsync(AppSetting.LayoutSavePath)).TextDecrypt(AppSetting.ConfigPwd)).BytesToStream();
			stream.Seek(0L, SeekOrigin.Begin);
			DemoDockContainer.RestoreLayoutFromStream(stream);
			AppCore.ViewModelBase.RootDocument.AddControl(PvfFileDocumentType.起始页);
			RgVlpJNt5a();
		}
		catch (Exception ex)
		{
			AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ResetWindowLayoutError"), ex.Message));
		}
	}

	private void sFclzyLgqy(object P_0, KeyEventArgs P_1)
	{
		if ((int)P_1.Key != 6)
		{
			ButtonEdit buttonEdit = (ButtonEdit)P_0;
			if (!string.IsNullOrEmpty(buttonEdit.SelectedText) && buttonEdit.SelectedText.Contains("\r\n") && buttonEdit.SelectedText == buttonEdit.Text)
			{
				buttonEdit.Text = string.Empty;
			}
		}
	}

	private void hLtjDEeYDP(object P_0, ItemCancelEventArgs P_1)
	{
		if (P_1.Item.DataContext is DocumentBase)
		{
			return;
		}
		if (P_1.Item is FloatGroup)
		{
			List<BaseLayoutItem> list = new List<BaseLayoutItem>();
			GetAllItems(P_1.Item, list);
			{
				foreach (BaseLayoutItem item in list)
				{
					if (!(item.DataContext is DocumentBase))
					{
						P_1.Cancel = true;
					}
				}
				return;
			}
		}
		if (P_1.Item is LayoutPanel layoutPanel)
		{
			if (!(layoutPanel.Content is ViewImportFilesPanel))
			{
				P_1.Cancel = true;
			}
		}
		else
		{
			P_1.Cancel = true;
		}
	}

	public List<BaseLayoutItem> GetAllItems(BaseLayoutItem item, List<BaseLayoutItem> list)
	{
		if (item is LayoutGroup layoutGroup)
		{
			foreach (BaseLayoutItem item2 in layoutGroup.Items)
			{
				GetAllItems(item2, list);
			}
		}
		else
		{
			list.Add(item);
		}
		return list;
	}

	private void m2mjjPdBWF(object P_0, RoutedEventArgs P_1)
	{
		if (AppCore.ViewModelBase.PVF.PvfIsOpen && ((int)Keyboard.Modifiers & 2) == 2)
		{
			try
			{
				FileHelper.OpenFolderAndSelectFile(AppCore.ViewModelBase.PVF.PvfPackFilePath);
			}
			catch (Exception ex)
			{
				AppCore.ShowMsg(ex.Message, isError: true);
			}
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!lryjCUlPWT)
		{
			lryjCUlPWT = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/mainwindow.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			mainWindow = (MainWindow)target;
			break;
		case 2:
			tttt = (TaskbarButtonService)target;
			break;
		case 3:
			notificationService = (NotificationService)target;
			break;
		case 4:
			subFile = (BarSubItem)target;
			break;
		case 5:
			subPvfOpenLog = (BarSubItem)target;
			break;
		case 6:
			((BarCheckItem)target).CheckedChanged += eNclcdP4GG;
			break;
		case 7:
			((BarButtonItem)target).ItemClick += co4lNaLNxk;
			break;
		case 8:
			subBookmark = (BarSubItem)target;
			break;
		case 11:
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += hLtjDEeYDP;
			DemoDockContainer.ShowingMenu += Tqll8q95Ns;
			break;
		case 12:
			Root = (LayoutGroup)target;
			break;
		case 13:
			FilelistLayoutPanel = (LayoutPanel)target;
			break;
		case 14:
			treeListControlGroupEx = (PvfTreeViewGroup)target;
			break;
		case 15:
			DocumentHost = (DocumentGroup)target;
			break;
		case 16:
			FindView = (LayoutPanel)target;
			break;
		case 17:
			outPutView = (LayoutPanel)target;
			break;
		case 18:
			ErrorListPanel = (LayoutPanel)target;
			break;
		case 19:
			findResultView = (LayoutPanel)target;
			break;
		default:
			lryjCUlPWT = true;
			break;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IStyleConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 9:
			((SimpleButton)target).Click += m2mjjPdBWF;
			break;
		case 10:
			((ButtonEdit)target).PreviewKeyDown += sFclzyLgqy;
			break;
		}
	}
}
