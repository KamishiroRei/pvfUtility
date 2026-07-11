using System;
using System.Windows.Documents;

namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class SnippetReplaceableTextElement : SnippetTextElement
{
	public override void Insert(InsertionContext context)
	{
		int insertionPosition = context.InsertionPosition;
		base.Insert(context);
		int insertionPosition2 = context.InsertionPosition;
		context.RegisterActiveElement(this, new ReplaceableActiveElement(context, insertionPosition, insertionPosition2));
	}

	public override Inline ToTextRun()
	{
		return new Italic(base.ToTextRun());
	}
}
