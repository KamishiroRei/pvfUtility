using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace PvfCode.ViewModels.Store.Preview;

public class PreviewItemCodeHoverViewViewModel : ViewModelBase
{
	[CompilerGenerated]
	private TextDocument RDeW5fVF8k;

	[CompilerGenerated]
	private IHighlightingDefinition bvoWSNJcbw;

	public TextDocument Document
	{
		[CompilerGenerated]
		get
		{
			return RDeW5fVF8k;
		}
		[CompilerGenerated]
		set
		{
			RDeW5fVF8k = value;
		}
	}

	public IHighlightingDefinition Highlighting
	{
		[CompilerGenerated]
		get
		{
			return bvoWSNJcbw;
		}
		[CompilerGenerated]
		set
		{
			bvoWSNJcbw = value;
		}
	}

	public PreviewItemCodeHoverViewViewModel(string text)
	{
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition("XML");
		Document = new TextDocument
		{
			Text = text
		};
	}
}
