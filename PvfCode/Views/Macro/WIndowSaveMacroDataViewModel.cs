using DevExpress.Mvvm;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Dot.Desktop;

namespace PvfCode.Views.Macro;

public class WIndowSaveMacroDataViewModel : ViewModelBase
{
	private TextDocument detailedInstructionsDocument;

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

	public IHighlightingDefinition Highlighting { get; set; }

	public TextDocument DetailedInstructionsDocument
	{
		get
		{
			return detailedInstructionsDocument;
		}
		set
		{
			detailedInstructionsDocument = value;
			RaisePropertyChanged("DetailedInstructionsDocument");
		}
	}

	public MacroDataRes MacroDataDto { get; set; }

	public bool IsShare
	{
		get
		{
			return GetProperty(() => IsShare);
		}
		set
		{
			SetProperty(() => IsShare, value);
		}
	}

	public WIndowSaveMacroDataViewModel()
	{
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
		DetailedInstructionsDocument = new TextDocument
		{
			Text = AppSetting.Instance.GetIlogger().GetStr("WindowSaveMacroData_Label_DetailDescription2")
		};
		MacroDataDto = new MacroDataRes();
	}
}
