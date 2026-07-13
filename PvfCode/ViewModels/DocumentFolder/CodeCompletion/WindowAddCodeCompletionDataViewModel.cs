using System;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.CodeCompletionModels;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class WindowAddCodeCompletionDataViewModel : ViewModelBase
{
	private readonly Action closeWindow;

	private readonly bool isAdd;

	private bool IsLoading
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

	public CodeCompletionData Data { get; set; }

	public TextDocument Document { get; set; }

	public TextDocument TextDocumentDescription { get; set; }

	public IHighlightingDefinition Highlighting
	{
		get
		{
			return GetProperty(() => Highlighting);
		}
		set
		{
			SetProperty<IHighlightingDefinition>(() => Highlighting, value);
		}
	}

	public string Title { get; set; }

	public WindowAddCodeCompletionDataViewModel(Action close, CodeCompletionData? data = null, bool isAdd = true)
	{
		try
		{
			this.isAdd = isAdd;
			Title = (isAdd ? AppSetting.Instance.GetIlogger().GetStr("WindowAddCodeCompletionData_WindowAddDataTitle") : AppSetting.Instance.GetIlogger().GetStr("WindowAddCodeCompletionData_WindowEditDataTitle"));
			Data = data;
			TextDocumentDescription = new TextDocument();
			Document = new TextDocument();
			if (data.CodeCompletScriptType == CodeCompletScriptType.Nut)
			{
				Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.nut);
			}
			else
			{
				Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
			}
			closeWindow = close;
			if (Data == null)
			{
				Data = new CodeCompletionData
				{
					IsShare = true
				};
			}
			else
			{
				if (data.Description == null)
				{
					TextDocumentDescription.Text = AppSetting.Instance.GetIlogger()?.GetStr("WindowAddCodeCompletionData_TextDocumentDescriptionDefaultValue");
				}
				else
				{
					TextDocumentDescription.Text = data.Description;
				}
				if (data.CompleteText == null)
				{
					Document.Text = AppSetting.Instance.GetIlogger()?.GetStr("WindowAddCodeCompletionData_CompleteTextDefaultValue");
				}
				else
				{
					Document.Text = data.CompleteText;
				}
			}
			if (this.isAdd)
			{
				Document.Text = AppSetting.Instance.GetIlogger()?.GetStr("WindowAddCodeCompletionData_CompleteTextDefaultValue2");
				TextDocumentDescription.Text = AppSetting.Instance.GetIlogger()?.GetStr("WindowAddCodeCompletionData_TextDocumentDescriptionDefaultValue");
			}
			if (Data != null)
			{
				data.NickNames = AppCore.NickNameTemp;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowAddCodeCompletionDataViewModel.WindowAddCodeCompletionDataViewModel");
		}
	}

	public void EditValueChanged(object sender)
	{
		try
		{
			switch (Data.CodeCompletScriptType)
			{
			case CodeCompletScriptType.Script:
				Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
				break;
			case CodeCompletScriptType.Nut:
				Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.nut);
				break;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowAddCodeCompletionDataViewModel.EditValueChanged");
		}
	}

	[Command]
	public async void OnSave()
	{
		try
		{
			if (string.IsNullOrEmpty(Data.Text))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputTitle"));
				return;
			}
			if (string.IsNullOrEmpty(Document.Text))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputCompleteText"));
				return;
			}
			IsLoading = true;
			Data.CompleteText = Document.Text;
			Data.Description = TextDocumentDescription.Text;
			AppSetting.Instance.EditConfig.SaveCompletionDatas(Data);
			await AppSetting.Instance.SaveSetting();
			if (!string.IsNullOrEmpty(Data.NickNames))
			{
				AppCore.NickNameTemp = Data.NickNames;
			}
			closeWindow();
			IsLoading = false;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowAddCodeCompletionDataViewModel.OnSave");
		}
	}
}
