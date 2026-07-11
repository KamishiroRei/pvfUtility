using DevExpress.Mvvm;
using PvfCode.ViewModels.VsCodeEditorViewModels.Enums;

namespace PvfCode.Controls.VisualCodeEditors;

public abstract class VsCodeEditorModelBase : ViewModelBase
{
	public bool IsLoaded
	{
		get
		{
			return GetProperty(() => IsLoaded);
		}
		set
		{
			SetProperty(() => IsLoaded, value);
		}
	}

	public VsCodeEditor EditorBase { get; set; }

	public LanguageType Language
	{
		get
		{
			return GetProperty(() => Language);
		}
		set
		{
			SetProperty(() => Language, value);
		}
	}

	public abstract void Loaded(object sender);

	protected VsCodeEditorModelBase()
	{
	}
}
