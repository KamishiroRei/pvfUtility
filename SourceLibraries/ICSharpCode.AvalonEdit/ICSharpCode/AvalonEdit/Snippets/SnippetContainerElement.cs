using System;
using System.Collections.Generic;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public class SnippetContainerElement : SnippetElement
{
	private NullSafeCollection<SnippetElement> elements = new NullSafeCollection<SnippetElement>();

	public IList<SnippetElement> Elements => elements;

	public override void Insert(InsertionContext context)
	{
		foreach (SnippetElement element in Elements)
		{
			element.Insert(context);
		}
	}

	public override Inline ToTextRun()
	{
		Span span = new Span();
		foreach (SnippetElement element in Elements)
		{
			Inline inline = element.ToTextRun();
			if (inline != null)
			{
				span.Inlines.Add(inline);
			}
		}
		return span;
	}
}
