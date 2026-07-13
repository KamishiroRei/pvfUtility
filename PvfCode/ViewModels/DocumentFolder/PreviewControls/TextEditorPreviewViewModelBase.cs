using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Controls.TextEditorFolder;

namespace PvfCode.ViewModels.DocumentFolder.PreviewControls;

public abstract class TextEditorPreviewViewModelBase : ViewModelBase
{
	internal TextEditorBase? Editor;

	internal PvfFile? File;

	public bool IsLoaded { get; set; }

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

	public bool IsVisibility
	{
		get
		{
			return GetProperty(() => IsVisibility);
		}
		set
		{
			SetProperty(() => IsVisibility, value);
		}
	}

	public TextEditorPreviewViewModelBase(TextEditorBase textEditorBase, PvfFile file)
	{
		Editor = textEditorBase;
		File = file;
		IsVisibility = true;
	}

	[Command]
	public void Loaded(string fileText)
	{
		if (File != null)
		{
			LoadData(fileText);
		}
	}

	[Command]
	public void OnUninstall2()
	{
		Uninstall();
	}

	[Command]
	public void OnRefresh(string fileText)
	{
		Refresh(fileText);
	}

	public abstract void LoadData(string fileText);

	public virtual void Dispose()
	{
	}

	public abstract void Refresh(string fileText);

	public abstract void Uninstall();
}
