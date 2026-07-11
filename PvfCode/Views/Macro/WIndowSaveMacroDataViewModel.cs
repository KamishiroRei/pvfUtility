using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Dot.Desktop;

namespace PvfCode.Views.Macro;

public class WIndowSaveMacroDataViewModel : ViewModelBase
{
	[CompilerGenerated]
	private IHighlightingDefinition BuChbXXp7u;

	private TextDocument QjKhIg66IM;

	[CompilerGenerated]
	private MacroDataRes bUQhEERtV9;

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

	public IHighlightingDefinition Highlighting
	{
		[CompilerGenerated]
		get
		{
			return BuChbXXp7u;
		}
		[CompilerGenerated]
		set
		{
			BuChbXXp7u = value;
		}
	}

	public TextDocument DetailedInstructionsDocument
	{
		get
		{
			return QjKhIg66IM;
		}
		set
		{
			QjKhIg66IM = value;
			RaisePropertyChanged("DetailedInstructionsDocument");
		}
	}

	public MacroDataRes MacroDataDto
	{
		[CompilerGenerated]
		get
		{
			return bUQhEERtV9;
		}
		[CompilerGenerated]
		set
		{
			bUQhEERtV9 = value;
		}
	}

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
