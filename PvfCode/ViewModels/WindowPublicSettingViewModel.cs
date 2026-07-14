using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.WindowsAPICodePack.Dialogs;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using PvfCode.Localization;
using PvfCode.Models.Pvf;
using PvfCode.Services.PvfParsingNew;
using PvfCode.ViewModels.TreeFolder;
using Utools;

namespace PvfCode.ViewModels;

public class WindowPublicSettingViewModel : ViewModelBase, IDisposable
{
	internal bool mf9FnuluQs;

	public TextDocument ScriptFileContentFormattingDocument { get; set; }

	public IHighlightingDefinition Highlighting { get; set; }

	public bool ShowSaveButton
	{
		get
		{
			return GetProperty(() => ShowSaveButton);
		}
		set
		{
			SetProperty(() => ShowSaveButton, value);
		}
	}

	public PVfTreeChildrenSelector ChildNodesSelector { get; set; }

	public ObservableConcurrentDictionaryEx<string, SettingMenuItem> TreeMenu { get; set; }

	public KeyValuePair<string, SettingMenuItem> TreeSelectedItem
	{
		get
		{
			return GetProperty(() => TreeSelectedItem);
		}
		set
		{
			SetProperty<KeyValuePair<string, SettingMenuItem>>(() => TreeSelectedItem, value, UpdateSaveButtonVisibility);
		}
	}

	public DataTemplate SelectedItemData
	{
		get
		{
			return GetProperty(() => SelectedItemData);
		}
		set
		{
			SetProperty<DataTemplate>(() => SelectedItemData, value);
		}
	}

	public TextDocument ItemCodeHoverDocument { get; set; }

	public WindowPublicSettingViewModel()
	{
		mf9FnuluQs = true;
		TreeMenu = new ObservableConcurrentDictionaryEx<string, SettingMenuItem>();
		ChildNodesSelector = new PVfTreeChildrenSelector(GetChildNodes);
		ScriptFileContentFormattingDocument = new TextDocument();
		ItemCodeHoverDocument = new TextDocument();
		ReadScriptFileContentFormatting();
		ReadItemCodeHoverXml();
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition("XML");
	}

	private IEnumerable GetChildNodes(object item)
	{
		if (item == null)
		{
			return null;
		}
		KeyValuePair<string, SettingMenuItem> keyValuePair = (KeyValuePair<string, SettingMenuItem>)item;
		if (!keyValuePair.Value.HaveChildren())
		{
			return null;
		}
		return keyValuePair.Value.Children;
	}

	private void UpdateSaveButtonVisibility()
	{
		if (TreeSelectedItem.Key == "代码智能提示" && TreeSelectedItem.Value.Parname == "文本编辑器")
		{
			ShowSaveButton = false;
		}
		else if (TreeSelectedItem.Key == "脚本文件格式化" && TreeSelectedItem.Value.Parname == "文本编辑器")
		{
			ShowSaveButton = false;
		}
		else
		{
			ShowSaveButton = true;
		}
	}

	[Command]
	public void OnSelectPvfBackupPath()
	{
		CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
		{
			IsFolderPicker = true,
			Title = AppSetting.Instance.GetIlogger().GetStr("mess_PleaseSelectPvfPath")
		};
		if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
		{
			AppSetting.Instance.PvfConfig.AutoTheBackupPvfConfig.BackupPath = commonOpenFileDialog.FileName;
		}
	}

	[Command]
	public void HotReloadScriptFileContentFormatting()
	{
		if (PraserInfoProviderConfiger.Init())
		{
			ReadScriptFileContentFormatting();
			AppCore.Logger.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_ReloadSuccess"));
		}
	}

	[Command]
	public void ReadScriptFileContentFormatting()
	{
		if (File.Exists(PraserInfoProviderConfiger.ConfigFilePath))
		{
			ScriptFileContentFormattingDocument.Text = File.ReadAllText(PraserInfoProviderConfiger.ConfigFilePath);
		}
		else
		{
			ScriptFileContentFormattingDocument.Text = "<!-- 文件不存在：" + PraserInfoProviderConfiger.ConfigFilePath + " -->";
		}
	}

	[Command]
	public async void SaveScriptFileContentFormatting()
	{
		if (string.IsNullOrEmpty(ScriptFileContentFormattingDocument.Text))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_ConfigFileCannotBeEmpty"));
			return;
		}
		try
		{
			await File.WriteAllTextAsync(PraserInfoProviderConfiger.ConfigFilePath, ScriptFileContentFormattingDocument.Text);
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_SaveSuccess"));
			HotReloadScriptFileContentFormatting();
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg(ex.Message, isError: true);
		}
	}

	[Command]
	public void OpenScriptFileContentFormatting()
	{
		try
		{
			if (File.Exists(PraserInfoProviderConfiger.ConfigFilePath))
			{
				FileHelper.OpenFolderAndSelectFile(PraserInfoProviderConfiger.ConfigFilePath);
			}
			else
			{
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), PraserInfoProviderConfiger.ConfigFilePath), isError: true);
			}
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg(ex.Message, isError: true);
		}
	}

	private void ReadItemCodeHoverXml()
	{
		try
		{
			if (File.Exists(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath))
			{
				ItemCodeHoverDocument.Text = File.ReadAllText(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath);
			}
			else
			{
				ItemCodeHoverDocument.Text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath);
			}
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ReadCodeIntelliSenseConfigError"), ex.Message), isError: true);
		}
	}

	[Command]
	public async void OnSaveItemCodeHoverXml()
	{
		if (string.IsNullOrEmpty(ItemCodeHoverDocument.Text))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_ConfigFileCannotBeEmpty"));
			return;
		}
		try
		{
			await File.WriteAllTextAsync(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath, ItemCodeHoverDocument.Text);
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_SaveSuccess"));
			HotReloadItemCodeHoverXml();
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg(ex.Message, isError: true);
		}
	}

	[Command]
	public void HotReloadItemCodeHoverXml()
	{
		if (AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.XmlToModel(showErrDialog: true))
		{
			ReadItemCodeHoverXml();
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_ReloadSuccess"));
		}
	}

	[Command]
	public void OpenItemCodeHoverXml()
	{
		try
		{
			if (File.Exists(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath))
			{
				FileHelper.OpenFolderAndSelectFile(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath);
			}
			else
			{
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath), isError: true);
			}
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg(ex.Message, isError: true);
		}
	}

	[Command]
	public void OnSelectGameClientPath()
	{
		CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
		{
			IsFolderPicker = true,
			Title = AppSetting.Instance.GetIlogger().GetStr("Title_SelectFolder")
		};
		if (commonOpenFileDialog.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok)
		{
			if (!File.Exists(Path.Combine(commonOpenFileDialog.FileName, "dnf.exe")))
			{
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_DnfExeNotExist"), commonOpenFileDialog.FileName), isError: true);
				AppSetting.Instance.GameOptions.GameClientPath = string.Empty;
			}
			else
			{
				AppSetting.Instance.GameOptions.GameClientPath = commonOpenFileDialog.FileName;
			}
		}
	}

	[Command]
	public async void OnOnWritePublicPem()
	{
		try
		{
			AppSetting.Instance.GameOptions.GameServerOptions.PemPrivateKey = lang.privatekey_pem;
			string savePath = Path.Combine(AppSetting.AppBasePath, "pemKey\\publickey.pem");
			FileHelper.CheckDir(Path.GetDirectoryName(savePath));
			await File.WriteAllTextAsync(savePath, lang.publickey_pem);
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_ServicePublicKeyGenerateSuccess"));
			FileHelper.OpenFolderAndSelectFile(savePath);
		}
		catch (Exception)
		{
			throw;
		}
	}

	[Command]
	public async void OnLangTypeChanged()
	{
		if (TreeSelectedItem.Key == null || TreeSelectedItem.Key != AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Common"))
		{
			return;
		}
		await LanguageResourceManager.ApplyLanguageAsync(AppSetting.Instance.CurrentLang);
		if (File.Exists(AppSetting.LayoutSavePath))
		{
			try
			{
				File.Delete(AppSetting.LayoutSavePath);
			}
			catch (Exception)
			{
			}
		}
		if (mf9FnuluQs)
		{
			MessageBox.Show(Application.Current.MainWindow, "切换语言后建议重新启动：pvfUtility\r\n언어 전환 후 재시작 권장: pvfUtility\r\nA restart is recommended after switching languages: pvfUtility");
		}
	}

	public void Dispose()
	{
		TreeMenu = null;
		SelectedItemData = null;
		ScriptFileContentFormattingDocument = null;
	}
}
