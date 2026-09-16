using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using PvfCode.Dot;
using PvfCode.ViewModels;
using PvfCode.ViewModels.DocumentFolder;
using Utools;

namespace PvfCode.Views;

public class WindowPublicSetting : ThemedWindow, IComponentConnector, IStyleConnector
{
	private readonly string? initialMenuKey;

	internal WindowPublicSetting win;

	internal TreeListControl tree;

	internal TreeListColumn columnKey;

	internal TreeListView treeListView;

	internal ContentControl contentPanel;

	private bool _contentLoaded;

	public WindowPublicSetting(string? menuKey = null)
	{
		WindowPublicSettingViewModel dataContext = new WindowPublicSettingViewModel();
		base.DataContext = dataContext;
		InitializeComponent();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Height = primaryScreenHeight * 0.5;
		base.Width = primaryScreenWidth * 0.5;
		initialMenuKey = menuKey;
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		WindowPublicSettingViewModel obj = base.DataContext as WindowPublicSettingViewModel;
		obj.Dispose();
		obj.showLanguageRestartPrompt = false;
		contentPanel.Content = null;
		base.DataContext = null;
		Application.Current.MainWindow.Activate();
	}

	private async void OnLoaded(object sender, RoutedEventArgs e)
	{
		PopulateTreeMenu();
		WindowPublicSettingViewModel viewModel = (WindowPublicSettingViewModel)base.DataContext;
		viewModel.TreeMenu.NotifyObserversOfChange();
		await Task.Delay(100);
		await ((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			foreach (KeyValuePair<string, SettingMenuItem> item in (IEnumerable<KeyValuePair<string, SettingMenuItem>>)viewModel.TreeMenu)
			{
				if (item.Key == "通用")
				{
					viewModel.TreeSelectedItem = item;
					tree.SelectedItems = new List<KeyValuePair<string, SettingMenuItem>> { item };
				}
			}
			tree.View.ExpandAllNodes();
			if (!string.IsNullOrEmpty(initialMenuKey))
			{
				NavigateToMenu(initialMenuKey);
			}
		}, Array.Empty<object>());
	}

	private void PopulateTreeMenu()
	{
		ObservableConcurrentDictionaryEx<string, SettingMenuItem> treeMenu = ((WindowPublicSettingViewModel)base.DataContext).TreeMenu;
		treeMenu.Add(AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Common"), new SettingMenuItem
		{
			Data = (DataTemplate)FindResource("defaultSetting")
		});
		ObservableConcurrentDictionaryEx<string, SettingMenuItem> children = new ObservableConcurrentDictionaryEx<string, SettingMenuItem>
		{
			{
				AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting"),
				new SettingMenuItem
				{
					Data = (DataTemplate)FindResource("EditorSetting")
				}
			},
			{
				AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_EditorHighlightColor"),
				new SettingMenuItem
				{
					Data = (DataTemplate)FindResource("EditorhighlightedSetting")
				}
			},
			{
				AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_CodeIntelliSense"),
				new SettingMenuItem
				{
					Data = (DataTemplate)FindResource("ItemCodeHoverConfig"),
					Parname = AppSetting.Instance.GetIlogger().GetStr("ViewGlobalOptions_TreeMenu_Setting_TextEditor")
				}
			}
		};
		treeMenu.Add(AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_TextEditor"), new SettingMenuItem
		{
			Children = children,
			Data = (DataTemplate)FindResource("EditorSetting")
		});
		treeMenu.Add(AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_FileExplorer"), new SettingMenuItem
		{
			Data = (DataTemplate)FindResource("TreeListSettings")
		});
		treeMenu.Add(AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_AutoBackup"), new SettingMenuItem
		{
			Data = (DataTemplate)FindResource("AutoBackupPvfSettings")
		});
		treeMenu.Add(AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_GlobalSearch"), new SettingMenuItem
		{
			Data = (DataTemplate)FindResource("PublicSearchServiceOptions")
		});
		treeMenu.Add(AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_PvfDocumentGroup"), new SettingMenuItem
		{
			Data = (DataTemplate)FindResource("PvfDcoumentOptions")
		});
		ObservableConcurrentDictionaryEx<string, SettingMenuItem> children3 = new ObservableConcurrentDictionaryEx<string, SettingMenuItem>
		{
			{
				AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_ServerConfig"),
				new SettingMenuItem
				{
					Data = (DataTemplate)FindResource("GameServerOptions")
				}
			},
			{
				AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_ClientConfig"),
				new SettingMenuItem
				{
					Data = (DataTemplate)FindResource("GameLoginClientOptions")
				}
			},
			{
				AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_LoginAccountConfig"),
				new SettingMenuItem
				{
					Data = (DataTemplate)FindResource("GameLoginAccountOptions")
				}
			}
		};
		treeMenu.Add(AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_StartGameConfig"), new SettingMenuItem
		{
			Data = (DataTemplate)FindResource("GameServerOptions"),
			Children = children3
		});
		ObservableConcurrentDictionaryEx<string, SettingMenuItem> previewChildren = new ObservableConcurrentDictionaryEx<string, SettingMenuItem>
		{
			{
				"自动打开预览",
				new SettingMenuItem
				{
					Data = PreviewAutoOpenSettingsView.SettingsTemplate,
					Parname = "文件预览"
				}
			}
		};
		treeMenu.Add("文件预览", new SettingMenuItem
		{
			Children = previewChildren,
			Data = (DataTemplate)FindResource("PvfFilePreviewOptions")
		});
	}

	private void OnEditorHighlightColorChanged(object sender, EditValueChangedEventArgs e)
	{
		ThemeSwitcher.Instance.UpdateTextEditorColorOptions();
		foreach (DocumentBase document in AppCore.ViewModelBase.RootDocument.Documents)
		{
			if (document is PvfFileDocument pvfFileDocument)
			{
				pvfFileDocument.GetEditor()?.TextArea.TextView.Redraw();
			}
		}
	}

	private void OnCancelClick(object sender, RoutedEventArgs e)
	{
		Close();
	}

	private async void OnSaveClick(object sender, RoutedEventArgs e)
	{
		if (ValidateSettings())
		{
			await AppSetting.Instance.SaveSetting();
			Close();
		}
	}

	private bool ValidateSettings()
	{
		_ = (WindowPublicSettingViewModel)base.DataContext;
		if (AppSetting.Instance.BookMarkGroup.IsShare)
		{
			if (string.IsNullOrEmpty(AppSetting.Instance.BookMarkGroup.Title))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_BookMark);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputBookmarkName"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.BookMarkGroup.Instructions))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_BookMark);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputBookmarkDescription"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.BookMarkGroup.DetailedInstructions))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_BookMark);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputBookmarkDetail"));
				return false;
			}
		}
		if (AppSetting.Instance.StoreOptions.UploadFileListComment)
		{
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.FileListComment.Title))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_FileExplorerComment);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputExplorerAnnotationName"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.FileListComment.Description))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_FileExplorerComment);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputExplorerAnnotationDescription"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.FileListComment.DetailedInstructions))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_FileExplorerComment);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputExplorerAnnotationDetail"));
				return false;
			}
		}
		if (AppSetting.Instance.StoreOptions.UploadTabComment)
		{
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.TabComment.Title))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_TagTranslation);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputTagTranslationName"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.TabComment.Description))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_TagTranslation);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputTagTranslationDescription"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.TabComment.DetailedInstructions))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_TagTranslation);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputTagTranslationDetail"));
				return false;
			}
		}
		if (AppSetting.Instance.StoreOptions.UploadItemCodeHoverConfig)
		{
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.ItemCodeHoverConfig.Title))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_CodeIntelliSense);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputCodeIntelliSenseName"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.ItemCodeHoverConfig.Description))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_CodeIntelliSense);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputCodeIntelliSenseDescription"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.ItemCodeHoverConfig.DetailedInstructions))
			{
				NavigateToMenu(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_CodeIntelliSense);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputCodeIntelliSenseDetail"));
				return false;
			}
		}
		return true;
	}

	private bool NavigateToMenu(string menuPath)
	{
		WindowPublicSettingViewModel windowPublicSettingViewModel = (WindowPublicSettingViewModel)base.DataContext;
		tree.View.ExpandAllNodes();
		string[] array = menuPath.Split("\\", StringSplitOptions.RemoveEmptyEntries);
		menuPath = array[0];
		SettingMenuItem value;
		if (array.Length == 1)
		{
			foreach (KeyValuePair<string, SettingMenuItem> item in (IEnumerable<KeyValuePair<string, SettingMenuItem>>)windowPublicSettingViewModel.TreeMenu)
			{
				if (item.Key == menuPath)
				{
					windowPublicSettingViewModel.TreeSelectedItem = item;
					tree.SelectedItems = new List<KeyValuePair<string, SettingMenuItem>> { item };
					return true;
				}
			}
		}
		else if (windowPublicSettingViewModel.TreeMenu.TryGetValue(array[0], out value))
		{
			foreach (KeyValuePair<string, SettingMenuItem> item2 in (IEnumerable<KeyValuePair<string, SettingMenuItem>>)value.Children)
			{
				if (item2.Key == array[1])
				{
					windowPublicSettingViewModel.TreeSelectedItem = item2;
					tree.SelectedItems = new List<KeyValuePair<string, SettingMenuItem>> { item2 };
					return true;
				}
			}
		}
		return false;
	}

	private void OnGameClientPathDrop(object sender, DragEventArgs e)
	{
		_ = (WindowPublicSettingViewModel)base.DataContext;
		string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
		if (array == null || array.Length == 0)
		{
			return;
		}
		string text = array[0];
		string text2 = Path.GetFileName(text).ToLower();
		if (!string.IsNullOrEmpty(Path.GetExtension(text).ToLower()))
		{
			if (text2 != "dnf.exe")
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PathNotExistDNF"), isError: true);
			}
			else if (!File.Exists(text))
			{
				AppCore.ShowMsg("路径不存在 dnf.exe", isError: true);
			}
			else
			{
				AppSetting.Instance.GameOptions.GameClientPath = Path.GetDirectoryName(text);
			}
		}
		else if (!File.Exists(Path.Combine(text, "dnf.exe")))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PathNotExistDNF"), isError: true);
		}
		else
		{
			AppSetting.Instance.GameOptions.GameClientPath = text;
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/windowpublicsetting.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			win = (WindowPublicSetting)target;
			win.Loaded += OnLoaded;
			break;
		case 22:
			tree = (TreeListControl)target;
			break;
		case 23:
			columnKey = (TreeListColumn)target;
			break;
		case 24:
			treeListView = (TreeListView)target;
			break;
		case 25:
			contentPanel = (ContentControl)target;
			break;
		case 26:
			((Button)target).Click += OnSaveClick;
			break;
		case 27:
			((Button)target).Click += OnCancelClick;
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
		case 2:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 3:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 4:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 5:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 6:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 7:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 8:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 9:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 10:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 11:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 12:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 13:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 14:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 15:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 16:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 17:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 18:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 19:
			((PopupColorEdit)target).EditValueChanged += OnEditorHighlightColorChanged;
			break;
		case 20:
			((ButtonEdit)target).Drop += OnGameClientPathDrop;
			break;
		case 21:
			((Image)target).Drop += OnGameClientPathDrop;
			break;
		}
	}
}
