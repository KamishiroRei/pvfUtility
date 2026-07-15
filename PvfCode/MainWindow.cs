using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
using DevExpress.Xpf.Layout.Core;
using PvfCode.Controls;
using PvfCode.Dot.Desktop;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.Options;
using PvfCode.OfficialAnnotations;
using PvfCode.Services;
using PvfCode.ViewModels;
using PvfCode.ViewModels.DocumentFolder;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.Views;
using PvfCode.Views.ChatGPT;
using PvfCode.Views.NpcShopEditor;
using PvfCode.Views.Dialogs;
using PvfCode.Views.ImportViews;
using PvfCode.Views.Tools;
using UnitComboLib.ViewModels;
using Utools;
using WinCopies.Util;
using AniDesignerWindow = PvfCode.Views.AniDesigner.WindowAniDesigner;

namespace PvfCode;

public class MainWindow : ThemedWindow, IComponentConnector, IStyleConnector
{
	private const double DefaultAiAssistantWidth = 380.0;

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

	private bool _isInitialized;

	private ChatGPTDocumentVm _aiAssistantViewModel;

	private ChatGPTMessDocument _aiAssistantView;

	private LayoutPanel _aiAssistantPanel;

	private bool _aiAssistantShowRequested;

	private bool _aiAssistantDisposed;

	internal MainWindow mainWindow;

	internal TaskbarButtonService taskbarButtonService;

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

	private bool _contentLoaded;

	public static string ApplicationID => "FunWithNotifications_19_1";

	public MainWindow()
	{
		PvfSkillTreeColorBehavior.Initialize();
		base.DataContext = (AppCore.ViewModelBase = new MainWindowViewModel());
		InitializeComponent();
		OfficialAnnotationLinks.OpenRequested += OnOfficialAnnotationOpenRequested;
		EnsureAiAssistantPanelDocked(restoreFindView: true);
		HideOnlineFeatures();
		EnableAiAssistantToolbarItem();
		EnsureDropRateManagementMenuItem();
		EnsureOfficialAnnotationMenuItem();
		HideDevelopmentTestButton();
		Dispatcher.BeginInvoke((Action)(() =>
		{
			HideOnlineFeatures();
			EnableAiAssistantToolbarItem();
			EnsureDropRateManagementMenuItem();
			EnsureOfficialAnnotationMenuItem();
			EnsureAiAssistantPanelDocked();
			HideDevelopmentTestButton();
		}), DispatcherPriority.ApplicationIdle);
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Height = primaryScreenHeight * 0.8;
		base.Width = primaryScreenWidth * 0.8;
		base.Loaded += OnLoaded;
		subFile.Popup += OnFileMenuPopup;
		subBookmark.Popup += OnBookmarkMenuPopup;
		(AppSetting.Instance.EditConfig.SizeUnitLabel as UnitViewModel).EventScreenPointsChanged += OnScreenPointsChanged;
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

	private void EnableAiAssistantToolbarItem()
	{
		BarManager barManager = Content as BarManager ?? BarManager.GetBarManager(this);
		BarButtonItem button = barManager?.Items
			.OfType<BarButtonItem>()
			.FirstOrDefault(item =>
			{
				string commandPath = BindingOperations
					.GetBindingExpression(item, BarItem.CommandProperty)
					?.ParentBinding.Path?.Path;
				return string.Equals(commandPath, "BarsVm.OnOpenChatGPTDocumentCommand", StringComparison.Ordinal) ||
					string.Equals(item.Content?.ToString(), "chatGPT", StringComparison.OrdinalIgnoreCase) ||
					(item.Glyph?.ToString()?.Contains("chatgpt.png", StringComparison.OrdinalIgnoreCase) ?? false);
			});
		if (button != null)
		{
			button.Content = "AI 助手";
			button.IsEnabled = true;
			button.IsVisible = true;
		}
		EnableAiAssistantToolbarLinks(this);
	}

	private void EnsureDropRateManagementMenuItem()
	{
		BarManager barManager = Content as BarManager ?? BarManager.GetBarManager(this);
		if (barManager == null)
		{
			return;
		}

		BarSubItem toolsMenu = FindToolsMenuItem(this) ?? barManager.Items.OfType<BarSubItem>()
			.FirstOrDefault(menu => menu.Content?.ToString()?.StartsWith("工具", StringComparison.Ordinal) == true);
		if (toolsMenu == null)
		{
			return;
		}
		if (toolsMenu.ItemLinks.Any(link =>
			string.Equals(link.Item?.Content?.ToString(), "深渊/翻牌爆率管理", StringComparison.Ordinal)))
		{
			return;
		}

		BarButtonItem item = new()
		{
			Content = "深渊/翻牌爆率管理",
			Command = new DelegateCommand(OpenDropRateManagementWindow)
		};
		BindingOperations.SetBinding(item, ContentElement.IsEnabledProperty, new Binding("PVF.PvfIsOpen")
		{
			Source = AppCore.ViewModelBase,
			Mode = BindingMode.OneWay
		});
		int independentDropIndex = toolsMenu.ItemLinks
			.Select((link, index) => new { link, index })
			.FirstOrDefault(value => string.Equals(
				BindingOperations.GetBindingExpression(value.link.Item, BarItem.CommandProperty)?.ParentBinding.Path?.Path,
				"BarsVm.OnOpenViewIndependent_dropCommand",
				StringComparison.Ordinal))?.index ?? -1;
		toolsMenu.ItemLinks.Insert(independentDropIndex + 1, item);
	}

	private void EnsureOfficialAnnotationMenuItem()
	{
		BarManager barManager = Content as BarManager ?? BarManager.GetBarManager(this);
		if (barManager == null)
		{
			return;
		}

		BarSubItem toolsMenu = FindToolsMenuItem(this) ?? barManager.Items.OfType<BarSubItem>()
			.FirstOrDefault(menu => menu.Content?.ToString()?.StartsWith("工具", StringComparison.Ordinal) == true);
		if (toolsMenu == null || toolsMenu.ItemLinks.Any(link =>
			string.Equals(link.Item?.Content?.ToString(), "官方注释文档", StringComparison.Ordinal)))
		{
			return;
		}

		BarButtonItem item = new()
		{
			Content = "官方注释文档",
			Command = new DelegateCommand(OpenOfficialAnnotationDocument)
		};
		int publishIndex = toolsMenu.ItemLinks
			.Select((link, index) => new { link, index })
			.FirstOrDefault(value => string.Equals(
				BindingOperations.GetBindingExpression(value.link.Item, BarItem.CommandProperty)?.ParentBinding.Path?.Path,
				"BarsVm.OnPvfReleaseCommand",
				StringComparison.Ordinal) || string.Equals(
				value.link.Item?.Content?.ToString(),
				Application.Current?.TryFindResource("mainWin_bar_Main_subItem_Tools_Publish")?.ToString(),
				StringComparison.Ordinal))?.index ?? -1;
		toolsMenu.ItemLinks.Insert(publishIndex >= 0 ? publishIndex + 1 : toolsMenu.ItemLinks.Count, item);
	}

	private static void OpenOfficialAnnotationDocument()
	{
		AppCore.ViewModelBase.RootDocument.OpenOfficialAnnotation();
	}

	private void OnOfficialAnnotationOpenRequested(string fileName)
	{
		Dispatcher.BeginInvoke((Action)(() =>
		{
			AppCore.ViewModelBase.RootDocument.OpenOfficialAnnotation(fileName);
		}), DispatcherPriority.Normal);
	}

	private static BarSubItem FindToolsMenuItem(DependencyObject parent)
	{
		int childCount = VisualTreeHelper.GetChildrenCount(parent);
		for (int index = 0; index < childCount; index++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(parent, index);
			if (child is LightweightBarItemLinkControl linkControl &&
				linkControl.ActualContent?.ToString()?.StartsWith("工具", StringComparison.Ordinal) == true &&
				linkControl.Link.Item is BarSubItem toolsMenu)
			{
				return toolsMenu;
			}
			BarSubItem nestedResult = FindToolsMenuItem(child);
			if (nestedResult != null)
			{
				return nestedResult;
			}
		}
		return null;
	}

	private static void OpenDropRateManagementWindow()
	{
		try
		{
			if (!AppCore.ViewModelBase.PVF.PvfIsOpen)
			{
				AppCore.ShowMsg("请先打开 PVF 文件。", isError: true, caption: "深渊/翻牌爆率管理");
				return;
			}

			DropRateManagementWindow window = new()
			{
				Owner = Application.Current.MainWindow
			};
			window.Show();
		}
		catch (Exception exception)
		{
			AppCore.ShowMsg(exception.Message, isError: true, caption: "深渊/翻牌爆率管理");
		}
	}

	private static void EnableAiAssistantToolbarLinks(DependencyObject parent)
	{
		int childCount = VisualTreeHelper.GetChildrenCount(parent);
		for (int index = 0; index < childCount; index++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(parent, index);
			if (child is LightweightBarItemLinkControl linkControl &&
				(string.Equals(linkControl.ActualContent?.ToString(), "chatGPT", StringComparison.OrdinalIgnoreCase) ||
				 string.Equals(linkControl.Link.Item.Content?.ToString(), "chatGPT", StringComparison.OrdinalIgnoreCase)))
			{
				linkControl.Link.Item.Content = "AI 助手";
				linkControl.Link.Item.IsEnabled = true;
				linkControl.Link.Item.IsVisible = true;
				linkControl.Link.IsVisible = true;
				linkControl.IsEnabled = true;
				linkControl.Visibility = Visibility.Visible;
			}
			EnableAiAssistantToolbarLinks(child);
		}
	}

	private static HashSet<string> GetOnlineMenuCaptions()
	{
		return OnlineMenuResourceKeys
			.Select(key => Application.Current?.TryFindResource(key)?.ToString())
			.Where(value => !string.IsNullOrWhiteSpace(value))
			.ToHashSet(StringComparer.Ordinal);
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

	private void EnsureAiAssistantPanelDocked(bool restoreFindView = false)
	{
		if (_aiAssistantDisposed || DemoDockContainer?.DockController == null || Root == null)
		{
			return;
		}

		if (_aiAssistantViewModel == null)
		{
			_aiAssistantViewModel = new ChatGPTDocumentVm();
		}
		if (_aiAssistantView == null)
		{
			_aiAssistantView = new ChatGPTMessDocument
			{
				DataContext = _aiAssistantViewModel
			};
		}
		if (_aiAssistantPanel == null)
		{
			_aiAssistantPanel = new LayoutPanel
			{
				Name = "AiAssistantView",
				Caption = "AI 助手",
				Padding = new Thickness(0),
				ItemWidth = new GridLength(DefaultAiAssistantWidth),
				ClosingBehavior = ClosingBehavior.HideToClosedPanelsCollection,
				DataContext = _aiAssistantViewModel,
				Content = _aiAssistantView
			};
		}

		if (restoreFindView && FindView != null)
		{
			RestorePanel(FindView);
		}

		BaseLayoutItem workspace = Root.Items.FirstOrDefault(item => !ReferenceEquals(item, _aiAssistantPanel));
		if (workspace == null)
		{
			return;
		}

		bool isRightmostRootPanel = ReferenceEquals(_aiAssistantPanel.Parent, Root) &&
			Root.Items.IndexOf(_aiAssistantPanel) == Root.Items.Count - 1;
		if (!isRightmostRootPanel)
		{
			if (_aiAssistantPanel.IsClosed)
			{
				DemoDockContainer.DockController.Restore(_aiAssistantPanel);
			}
			DemoDockContainer.DockController.RemoveItem(_aiAssistantPanel);
			DemoDockContainer.DockController.Dock(_aiAssistantPanel, workspace, DockType.Right);
			_aiAssistantPanel.ItemWidth = new GridLength(DefaultAiAssistantWidth);
		}

		if (!_aiAssistantShowRequested)
		{
			_aiAssistantPanel.Visibility = Visibility.Collapsed;
		}
	}

	private void RestorePanel(LayoutPanel panel)
	{
		if (panel.IsClosed)
		{
			DemoDockContainer.DockController.Restore(panel);
		}
		panel.Visibility = Visibility.Visible;
	}

	public void ShowAiAssistantPanel()
	{
		_aiAssistantShowRequested = true;
		EnsureAiAssistantPanelDocked(restoreFindView: true);
		if (_aiAssistantPanel == null)
		{
			return;
		}

		RestorePanel(_aiAssistantPanel);
		EnsureAiAssistantPanelDocked(restoreFindView: true);
		DemoDockContainer.Activate(_aiAssistantPanel);
		_aiAssistantView?.FocusPrompt();
	}

	private void DisposeAiAssistant()
	{
		if (_aiAssistantDisposed)
		{
			return;
		}

		_aiAssistantDisposed = true;
		_aiAssistantViewModel?.Dispose();
		if (_aiAssistantView != null)
		{
			_aiAssistantView.DataContext = null;
		}
		if (_aiAssistantPanel != null)
		{
			_aiAssistantPanel.Content = null;
			_aiAssistantPanel.DataContext = null;
		}
	}

	private async void InitializeAfterLoad()
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
		if (string.Equals(content, "chatGPT", StringComparison.OrdinalIgnoreCase) ||
			string.Equals(content, "AI 助手", StringComparison.Ordinal))
		{
			linkControl.Link.Item.Content = "AI 助手";
			linkControl.Link.IsVisible = true;
			linkControl.Link.Item.IsVisible = true;
			linkControl.Link.Item.IsEnabled = true;
			linkControl.IsEnabled = true;
			linkControl.Visibility = Visibility.Visible;
		}
		else if (GetOnlineMenuCaptions().Contains(content ?? string.Empty))
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

	private async void OnScreenPointsChanged()
	{
		await AppSetting.Instance.SaveSetting();
	}

	private void OnBookmarkMenuPopup(object? sender, EventArgs e)
	{
		foreach (BarItemLinkBase dynamicLink in subBookmark.ItemLinks.ToArray().Skip(1))
		{
			subBookmark.ItemLinks.Remove(dynamicLink);
		}
		subBookmark.ItemLinks.Add(new BarItemSeparator());
		foreach (KeyValuePair<string, BookMarkDto> menuItem in GetBookmarkMenuItems())
		{
			AddBookmarkMenuItem(subBookmark, menuItem);
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

	private void PopulateBookmarkMenu(IDictionary<string, BookMarkDto> bookmarks, BarSubItem parentMenu)
	{
		foreach (KeyValuePair<string, BookMarkDto> item in bookmarks.OrderBy(item => item.Value.Sort))
		{
			AddBookmarkMenuItem(parentMenu, item);
		}
	}

	private void AddBookmarkMenuItem(BarSubItem parentMenu, KeyValuePair<string, BookMarkDto> item)
	{
		if (item.Value.IsFile)
		{
			parentMenu.ItemLinks.Add(new BarButtonItem
			{
				Content = item.Key,
				Glyph = Res.Instance.TreeFiles.Script_16x,
				Command = new DelegateCommand(delegate
				{
					AppCore.ViewModelBase.RootDocument.BookMarkOpenDocument(item.Value.FilePath);
				})
			});
			return;
		}
		BarSubItem submenu = new BarSubItem
		{
			Content = item.Key,
			Glyph = Res.Instance.TreeFiles.FolderClosed
		};
		parentMenu.ItemLinks.Add(submenu);
		if (item.Value.HaveChildren())
		{
			PopulateBookmarkMenu(item.Value.Children, submenu);
		}
	}

	private void OnFileMenuPopup(object? sender, EventArgs e)
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
		SaveLayout();
		OfficialAnnotationLinks.OpenRequested -= OnOfficialAnnotationOpenRequested;
		try
		{
			WebApiServer.Instance.Stop();
		}
		catch (Exception)
		{
		}
		DisposeAiAssistant();
		App.OnExit();
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		if (!_isInitialized)
		{
			_isInitialized = true;
			AppSetting.Instance.EditConfig.InitFoldingGuideLineBurshs(this);
			RestoreLayout();
			InitializeAfterLoad();
		}
	}

	private async void SaveLayout()
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

	private async void RestoreLayout()
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
		finally
		{
			EnsureAiAssistantPanelDocked();
		}
	}

	private void OnSettingsCheckItemChanged(object sender, ItemClickEventArgs e)
	{
		if (_isInitialized)
		{
			AppSetting.Instance.GetIlogger()?.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ModifySuccess"), AppSetting.Instance.AppName));
		}
	}

	private void OnDockLayoutManagerShowingMenu(object sender, ShowingMenuEventArgs e)
	{
		BarButtonItem barButtonItem = null;
		BarButtonItem barButtonItem2 = null;
		BarButtonItem barButtonItem3 = null;
		IBarItem[] array = e.Menu.Items.ToArray();
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
					barButtonItem.ItemClick -= CloseAllDocumentsExceptActive;
					barButtonItem.ItemClick += CloseAllDocumentsExceptActive;
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
			CommonBarItemCollection items = e.Menu.Items;
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
			barButtonItem5.ItemClick -= CloseDocumentsToRight;
			barButtonItem5.ItemClick += CloseDocumentsToRight;
			BarButtonItem barButtonItem6 = new BarButtonItem
			{
				Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_CloseLeftDocuments")
			};
			barButtonItem6.ItemClick -= CloseDocumentsToLeft;
			barButtonItem6.ItemClick += CloseDocumentsToLeft;
			BarButtonItem barButtonItem7 = new BarButtonItem
			{
				Content = AppSetting.Instance.GetIlogger()?.GetStr("mess_Document_ContextMenu_Name_CloseAllDocuments"),
				Glyph = Res.Instance.ClearWindowContent
			};
			barButtonItem7.ItemClick -= CloseAllDocuments;
			barButtonItem7.ItemClick += CloseAllDocuments;
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

	private void CloseAllDocumentsExceptActive(object sender, ItemClickEventArgs e)
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

	private void CloseAllDocuments(object sender, ItemClickEventArgs e)
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

	private void CloseDocumentsToRight(object sender, ItemClickEventArgs e)
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

	private void CloseDocumentsToLeft(object sender, ItemClickEventArgs e)
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

	private async void ResetWindowLayout(object sender, ItemClickEventArgs e)
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
			EnsureAiAssistantPanelDocked();
			AppCore.ViewModelBase.RootDocument.AddControl(PvfFileDocumentType.起始页);
			SaveLayout();
		}
		catch (Exception ex)
		{
			AppCore.Logger.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ResetWindowLayoutError"), ex.Message));
		}
	}

	private void OnButtonEditPreviewKeyDown(object sender, KeyEventArgs e)
	{
		if ((int)e.Key != 6)
		{
			ButtonEdit buttonEdit = (ButtonEdit)sender;
			if (!string.IsNullOrEmpty(buttonEdit.SelectedText) && buttonEdit.SelectedText.Contains("\r\n") && buttonEdit.SelectedText == buttonEdit.Text)
			{
				buttonEdit.Text = string.Empty;
			}
		}
	}

	private void OnDockItemClosing(object sender, ItemCancelEventArgs e)
	{
		if (ReferenceEquals(e.Item, _aiAssistantPanel))
		{
			_aiAssistantViewModel?.CancelCurrentRequest();
			return;
		}
		if (e.Item.DataContext is DocumentBase)
		{
			return;
		}
		if (e.Item is FloatGroup)
		{
			List<BaseLayoutItem> list = new List<BaseLayoutItem>();
			GetAllItems(e.Item, list);
			{
				foreach (BaseLayoutItem item in list)
				{
					if (!(item.DataContext is DocumentBase))
					{
						e.Cancel = true;
					}
				}
				return;
			}
		}
		if (e.Item is LayoutPanel layoutPanel)
		{
			if (!(layoutPanel.Content is ViewImportFilesPanel))
			{
				e.Cancel = true;
			}
		}
		else
		{
			e.Cancel = true;
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

	private void OnPvfPathButtonClick(object sender, RoutedEventArgs e)
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
		if (!_contentLoaded)
		{
			_contentLoaded = true;
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
			taskbarButtonService = (TaskbarButtonService)target;
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
			((BarCheckItem)target).CheckedChanged += OnSettingsCheckItemChanged;
			break;
		case 7:
			((BarButtonItem)target).ItemClick += ResetWindowLayout;
			break;
		case 8:
			subBookmark = (BarSubItem)target;
			break;
		case 11:
			DemoDockContainer = (DockLayoutManager)target;
			DemoDockContainer.DockItemClosing += OnDockItemClosing;
			DemoDockContainer.ShowingMenu += OnDockLayoutManagerShowingMenu;
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
			_contentLoaded = true;
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
			((SimpleButton)target).Click += OnPvfPathButtonClick;
			break;
		case 10:
			((ButtonEdit)target).PreviewKeyDown += OnButtonEditPreviewKeyDown;
			break;
		}
	}
}
