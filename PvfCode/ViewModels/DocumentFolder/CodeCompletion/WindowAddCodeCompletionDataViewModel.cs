using System;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.CodeCompletionModels;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class WindowAddCodeCompletionDataViewModel : ViewModelBase
{
	private readonly Action Close;

	[CompilerGenerated]
	private CodeCompletionData jGjud7xTyI;

	[CompilerGenerated]
	private TextDocument e6iueDvU51;

	[CompilerGenerated]
	private TextDocument Jbaut6TqRr;

	[CompilerGenerated]
	private string vbiubv5X6B;

	private readonly bool AgnuIV3xXl;

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

	public CodeCompletionData Data
	{
		[CompilerGenerated]
		get
		{
			return jGjud7xTyI;
		}
		[CompilerGenerated]
		set
		{
			jGjud7xTyI = value;
		}
	}

	public TextDocument Document
	{
		[CompilerGenerated]
		get
		{
			return e6iueDvU51;
		}
		[CompilerGenerated]
		set
		{
			e6iueDvU51 = value;
		}
	}

	public TextDocument TextDocumentDescription
	{
		[CompilerGenerated]
		get
		{
			return Jbaut6TqRr;
		}
		[CompilerGenerated]
		set
		{
			Jbaut6TqRr = value;
		}
	}

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

	public string Title
	{
		[CompilerGenerated]
		get
		{
			return vbiubv5X6B;
		}
		[CompilerGenerated]
		set
		{
			vbiubv5X6B = value;
		}
	}

	public WindowAddCodeCompletionDataViewModel(Action close, CodeCompletionData? data = null, bool isAdd = true)
	{
		try
		{
			AgnuIV3xXl = isAdd;
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
			Close = close;
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
			if (AgnuIV3xXl)
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
			Close();
			IsLoading = false;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "WindowAddCodeCompletionDataViewModel.OnSave");
		}
	}
}
