using System;
using System.Text;

namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class SnippetSelectionElement : SnippetElement
{
	public int Indentation { get; set; }

	public override void Insert(InsertionContext context)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Indentation; i++)
		{
			stringBuilder.Append(context.Tab);
		}
		string text = stringBuilder.ToString();
		string text2 = context.SelectedText.TrimStart(' ', '\t');
		text2 = text2.Replace(context.LineTerminator, context.LineTerminator + text);
		context.Document.Insert(context.InsertionPosition, text2);
		context.InsertionPosition += text2.Length;
		if (string.IsNullOrEmpty(context.SelectedText))
		{
			SnippetCaretElement.SetCaret(context);
		}
	}
}
