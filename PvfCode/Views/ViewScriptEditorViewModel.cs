using System;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace PvfCode.Views;

public class ViewScriptEditorViewModel : ViewModelBase, IDisposable
{
	public bool IsReadOnly
	{
		get
		{
			return GetProperty(() => IsReadOnly);
		}
		set
		{
			SetProperty(() => IsReadOnly, value);
		}
	}

	public string Title
	{
		get
		{
			return GetProperty(() => Title);
		}
		set
		{
			SetProperty<string>(() => Title, value);
		}
	}

	public int Width
	{
		get
		{
			return GetProperty(() => Width);
		}
		set
		{
			SetProperty(() => Width, value);
		}
	}

	public int Height
	{
		get
		{
			return GetProperty(() => Height);
		}
		set
		{
			SetProperty(() => Height, value);
		}
	}

	public TextDocument Document
	{
		get
		{
			return GetProperty(() => Document);
		}
		set
		{
			SetProperty<TextDocument>(() => Document, value);
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

	public ViewScriptEditorViewModel(string title, string text)
	{
		Title = title;
		Document = new TextDocument();
		if (text == null)
		{
			text = string.Empty;
		}
		Document.Text = text;
		cCUCvE6ce7();
		ThemeSwitcher.Instance.PvfCodeThemeChangedEvent += Ba5CBRooRR;
	}

	private void cCUCvE6ce7()
	{
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
	}

	private void Ba5CBRooRR()
	{
		cCUCvE6ce7();
	}

	public void Dispose()
	{
		ThemeSwitcher.Instance.PvfCodeThemeChangedEvent -= Ba5CBRooRR;
	}

	[SpecialName]
	private IHighlighter G4gCFAYRfs()
	{
		return new DocumentHighlighter(Document, Highlighting);
	}
}
