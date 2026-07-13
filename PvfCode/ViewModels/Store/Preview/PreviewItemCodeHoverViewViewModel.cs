using DevExpress.Mvvm;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;

namespace PvfCode.ViewModels.Store.Preview;

public class PreviewItemCodeHoverViewViewModel : ViewModelBase
{
	public TextDocument Document { get; set; }

	public IHighlightingDefinition Highlighting { get; set; }

	public PreviewItemCodeHoverViewViewModel(string text)
	{
		Highlighting = ThemeSwitcher.Instance.GetHighlightingDefinition("XML");
		Document = new TextDocument
		{
			Text = text
		};
	}
}
