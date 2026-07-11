using System;
using System.Windows.Documents;

namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class SnippetTextElement : SnippetElement
{
	private string text;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	public override void Insert(InsertionContext context)
	{
		if (text != null)
		{
			context.InsertText(text);
		}
	}

	public override Inline ToTextRun()
	{
		return new Run(text ?? string.Empty);
	}
}
