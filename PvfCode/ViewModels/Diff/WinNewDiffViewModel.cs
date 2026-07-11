using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Mvvm.DataAnnotations;
using Microsoft.WindowsAPICodePack.Dialogs;
using PvfCode.Controls.VisualCodeEditors;
using PvfCode.Dot;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.ViewModels.Diff;

public class WinNewDiffViewModel : VsCodeEditorModelBase, IDisposable
{
	public DiffSource LeftSource
	{
		get
		{
			return GetProperty(() => LeftSource);
		}
		set
		{
			SetProperty<DiffSource>(() => LeftSource, value);
		}
	}

	public DiffSource RightSource
	{
		get
		{
			return GetProperty(() => RightSource);
		}
		set
		{
			SetProperty<DiffSource>(() => RightSource, value);
		}
	}

	public override void Loaded(object sender)
	{
		base.EditorBase = (VsCodeEditor)sender;
		base.IsLoaded = true;
		RefEditor();
	}

	public async void RefEditor()
	{
		if (!base.IsLoaded)
		{
			return;
		}
		if (LeftSource != null)
		{
			ResultData<string> fileText = LeftSource.GetFileText(AppSetting.Instance.PvfConfig.DefaultEncoding);
			if (!fileText.IsError)
			{
				await base.EditorBase.SetDiffLeftText(fileText.Data, LeftSource.GetLanguageType());
			}
			else
			{
				AppCore.ShowMsg(fileText.Msg, isError: true);
			}
		}
		if (RightSource != null)
		{
			ResultData<string> fileText2 = RightSource.GetFileText(AppSetting.Instance.PvfConfig.DefaultEncoding);
			if (fileText2.IsError)
			{
				AppCore.ShowMsg(fileText2.Msg, isError: true);
			}
			else
			{
				await base.EditorBase.SetDiffRightText(fileText2.Data, RightSource.GetLanguageType());
			}
		}
	}

	[Command]
	public void LeftDocuemntSelectDiffFile(bool fromSelectDisk)
	{
		X23GOSmHaK(fromSelectDisk, true);
	}

	[Command]
	public void RightDocumentSelectDiffFile(bool fromSelectDisk)
	{
		X23GOSmHaK(fromSelectDisk, false);
	}

	[Command]
	public async void SaveDocument(bool isLeft)
	{
		DiffSource source = (isLeft ? LeftSource : RightSource);
		if (source != null)
		{
			string text = ((!isLeft) ? (await base.EditorBase.GetRightDocumentText()) : (await base.EditorBase.GetLeftDocumentText()));
			string newText = text;
			ResultData resultData = source.SaveFileText(newText);
			if (resultData.IsError)
			{
				AppCore.ShowMsg(resultData.Msg, isError: true);
			}
			else
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_SaveSuccess"));
			}
		}
	}

	[Command]
	public async void OnGoToNextDiffFile()
	{
		await (base.EditorBase?.GoToNextDiffLine());
	}

	private void X23GOSmHaK(bool P_0, bool P_1)
	{
		Encoding.GetEncoding((int)AppSetting.Instance.PvfConfig.DefaultEncoding);
		DiffSource diffSource;
		if (P_0)
		{
			CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
			{
				Title = AppSetting.Instance.GetIlogger().GetStr("DocumentDiffEditor_SelectFileToCompare"),
				IsFolderPicker = false,
				Multiselect = false,
				AllowPropertyEditing = true,
				EnsurePathExists = true,
				EnsureValidNames = true
			};
			if (commonOpenFileDialog.ShowDialog() != CommonFileDialogResult.Ok)
			{
				return;
			}
			string fileName = commonOpenFileDialog.FileName;
			diffSource = new DiffSource(fileName);
		}
		else
		{
			if (!AppCore.ViewModelBase.PVF.PvfIsOpen)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseLoadPvfPackFirst"));
				return;
			}
			IEnumerable<string> enumerable = AppCore.SelectPvfFileList(TreeViewType.FileList);
			if (enumerable == null || enumerable.Count() == 0)
			{
				return;
			}
			string fileName = enumerable.ToList()[0];
			if (!AppCore.ViewModelBase.PVF.FileAny(fileName))
			{
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), fileName), isError: true);
				return;
			}
			diffSource = new DiffSource(AppCore.ViewModelBase.PVF, fileName);
		}
		if (P_1)
		{
			LeftSource = diffSource;
		}
		else
		{
			RightSource = diffSource;
		}
		RefEditor();
	}

	public void Dispose()
	{
		LeftSource = null;
		RightSource = null;
		base.EditorBase?.Clear();
		base.EditorBase?.Dispose();
	}

	public WinNewDiffViewModel()
	{
	}
}
