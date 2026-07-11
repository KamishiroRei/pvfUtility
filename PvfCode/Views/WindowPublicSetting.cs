using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		public WindowPublicSettingViewModel XotwNNp99a;

		public WindowPublicSetting nv5wzQ37NG;

		public _003C_003Ec__DisplayClass3_0()
		{
		}

		internal void WkewR4Rlhg()
		{
			foreach (KeyValuePair<string, SettingMenuItem> item in (IEnumerable<KeyValuePair<string, SettingMenuItem>>)XotwNNp99a.TreeMenu)
			{
				if (item.Key == "通用")
				{
					XotwNNp99a.TreeSelectedItem = item;
					nv5wzQ37NG.tree.SelectedItems = new List<KeyValuePair<string, SettingMenuItem>> { item };
				}
			}
			nv5wzQ37NG.tree.View.ExpandAllNodes();
			if (!string.IsNullOrEmpty(nv5wzQ37NG.MJsCiy1j7P))
			{
				nv5wzQ37NG.I3mCYpGIRU(nv5wzQ37NG.MJsCiy1j7P);
			}
		}
	}

	private readonly string? MJsCiy1j7P;

	internal WindowPublicSetting win;

	internal TreeListControl tree;

	internal TreeListColumn columnKey;

	internal TreeListView treeListView;

	internal ContentControl contentPanel;

	private bool LBaCuTy0vJ;

	public WindowPublicSetting(string? menuKey = null)
	{
		WindowPublicSettingViewModel dataContext = new WindowPublicSettingViewModel();
		base.DataContext = dataContext;
		InitializeComponent();
		double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
		double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
		base.Height = primaryScreenHeight * 0.5;
		base.Width = primaryScreenWidth * 0.5;
		MJsCiy1j7P = menuKey;
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		WindowPublicSettingViewModel obj = base.DataContext as WindowPublicSettingViewModel;
		obj.Dispose();
		obj.mf9FnuluQs = false;
		contentPanel.Content = null;
		base.DataContext = null;
		Application.Current.MainWindow.Activate();
	}

	private async void RDmC2jF5Bq(object P_0, RoutedEventArgs P_1)
	{
		_003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals10 = new _003C_003Ec__DisplayClass3_0();
		CS_0024_003C_003E8__locals10.nv5wzQ37NG = this;
		lbJCfIC6gA();
		CS_0024_003C_003E8__locals10.XotwNNp99a = (WindowPublicSettingViewModel)base.DataContext;
		CS_0024_003C_003E8__locals10.XotwNNp99a.TreeMenu.NotifyObserversOfChange();
		await Task.Delay(100);
		await ((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			foreach (KeyValuePair<string, SettingMenuItem> item in (IEnumerable<KeyValuePair<string, SettingMenuItem>>)CS_0024_003C_003E8__locals10.XotwNNp99a.TreeMenu)
			{
				if (item.Key == "通用")
				{
					CS_0024_003C_003E8__locals10.XotwNNp99a.TreeSelectedItem = item;
					CS_0024_003C_003E8__locals10.nv5wzQ37NG.tree.SelectedItems = new List<KeyValuePair<string, SettingMenuItem>> { item };
				}
			}
			CS_0024_003C_003E8__locals10.nv5wzQ37NG.tree.View.ExpandAllNodes();
			if (!string.IsNullOrEmpty(CS_0024_003C_003E8__locals10.nv5wzQ37NG.MJsCiy1j7P))
			{
				CS_0024_003C_003E8__locals10.nv5wzQ37NG.I3mCYpGIRU(CS_0024_003C_003E8__locals10.nv5wzQ37NG.MJsCiy1j7P);
			}
		}, Array.Empty<object>());
	}

	private void lbJCfIC6gA()
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
		treeMenu.Add("文件预览", new SettingMenuItem
		{
			Data = (DataTemplate)FindResource("PvfFilePreviewOptions")
		});
	}

	private void tAtC5kxQKJ(object P_0, EditValueChangedEventArgs P_1)
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

	private void OZXCS4Sh78(object P_0, RoutedEventArgs P_1)
	{
		Close();
	}

	private async void WjOCASXqep(object P_0, RoutedEventArgs P_1)
	{
		if (wjuC4P6Nj6())
		{
			await AppSetting.Instance.SaveSetting();
			Close();
		}
	}

	private bool wjuC4P6Nj6()
	{
		_ = (WindowPublicSettingViewModel)base.DataContext;
		if (AppSetting.Instance.BookMarkGroup.IsShare)
		{
			if (string.IsNullOrEmpty(AppSetting.Instance.BookMarkGroup.Title))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_BookMark);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputBookmarkName"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.BookMarkGroup.Instructions))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_BookMark);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputBookmarkDescription"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.BookMarkGroup.DetailedInstructions))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_BookMark);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputBookmarkDetail"));
				return false;
			}
		}
		if (AppSetting.Instance.StoreOptions.UploadFileListComment)
		{
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.FileListComment.Title))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_FileExplorerComment);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputExplorerAnnotationName"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.FileListComment.Description))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_FileExplorerComment);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputExplorerAnnotationDescription"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.FileListComment.DetailedInstructions))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_FileExplorerComment);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputExplorerAnnotationDetail"));
				return false;
			}
		}
		if (AppSetting.Instance.StoreOptions.UploadTabComment)
		{
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.TabComment.Title))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_TagTranslation);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputTagTranslationName"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.TabComment.Description))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_TagTranslation);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputTagTranslationDescription"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.TabComment.DetailedInstructions))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_TagTranslation);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputTagTranslationDetail"));
				return false;
			}
		}
		if (AppSetting.Instance.StoreOptions.UploadItemCodeHoverConfig)
		{
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.ItemCodeHoverConfig.Title))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_CodeIntelliSense);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputCodeIntelliSenseName"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.ItemCodeHoverConfig.Description))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_CodeIntelliSense);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputCodeIntelliSenseDescription"));
				return false;
			}
			if (string.IsNullOrEmpty(AppSetting.Instance.StoreOptions.ItemCodeHoverConfig.DetailedInstructions))
			{
				I3mCYpGIRU(AppSetting.Instance.LuanguageOptions.GoToStoreShareOption_CodeIntelliSense);
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputCodeIntelliSenseDetail"));
				return false;
			}
		}
		return true;
	}

	private bool I3mCYpGIRU(string P_0)
	{
		WindowPublicSettingViewModel windowPublicSettingViewModel = (WindowPublicSettingViewModel)base.DataContext;
		tree.View.ExpandAllNodes();
		string[] array = P_0.Split("\\", StringSplitOptions.RemoveEmptyEntries);
		P_0 = array[0];
		SettingMenuItem value;
		if (array.Length == 1)
		{
			foreach (KeyValuePair<string, SettingMenuItem> item in (IEnumerable<KeyValuePair<string, SettingMenuItem>>)windowPublicSettingViewModel.TreeMenu)
			{
				if (item.Key == P_0)
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

	private void t8QCy3X0Is(object P_0, DragEventArgs P_1)
	{
		_ = (WindowPublicSettingViewModel)base.DataContext;
		string[] array = (string[])P_1.Data.GetData(DataFormats.FileDrop);
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
		if (!LBaCuTy0vJ)
		{
			LBaCuTy0vJ = true;
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
			win.Loaded += RDmC2jF5Bq;
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
			((Button)target).Click += WjOCASXqep;
			break;
		case 27:
			((Button)target).Click += OZXCS4Sh78;
			break;
		default:
			LBaCuTy0vJ = true;
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
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 3:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 4:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 5:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 6:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 7:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 8:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 9:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 10:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 11:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 12:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 13:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 14:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 15:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 16:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 17:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 18:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 19:
			((PopupColorEdit)target).EditValueChanged += tAtC5kxQKJ;
			break;
		case 20:
			((ButtonEdit)target).Drop += t8QCy3X0Is;
			break;
		case 21:
			((Image)target).Drop += t8QCy3X0Is;
			break;
		}
	}
}
