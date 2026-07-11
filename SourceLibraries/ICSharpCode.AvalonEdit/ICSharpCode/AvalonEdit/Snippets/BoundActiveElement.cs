using System;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets;

internal sealed class BoundActiveElement : IActiveElement
{
	private InsertionContext context;

	private SnippetReplaceableTextElement targetSnippetElement;

	private SnippetBoundElement boundElement;

	internal IReplaceableActiveElement targetElement;

	private AnchorSegment segment;

	public bool IsEditable => false;

	public ISegment Segment => segment;

	public BoundActiveElement(InsertionContext context, SnippetReplaceableTextElement targetSnippetElement, SnippetBoundElement boundElement, AnchorSegment segment)
	{
		this.context = context;
		this.targetSnippetElement = targetSnippetElement;
		this.boundElement = boundElement;
		this.segment = segment;
	}

	public void OnInsertionCompleted()
	{
		targetElement = context.GetActiveElement(targetSnippetElement) as IReplaceableActiveElement;
		if (targetElement != null)
		{
			targetElement.TextChanged += targetElement_TextChanged;
		}
	}

	private void targetElement_TextChanged(object sender, EventArgs e)
	{
		if (!(SimpleSegment.GetOverlap(segment, targetElement.Segment) == SimpleSegment.Invalid))
		{
			return;
		}
		int offset = segment.Offset;
		int length = segment.Length;
		string text = boundElement.ConvertText(targetElement.Text);
		if (length != text.Length || text != context.Document.GetText(offset, length))
		{
			context.Document.Replace(offset, length, text);
			if (length == 0)
			{
				segment = new AnchorSegment(context.Document, offset, text.Length);
			}
		}
	}

	public void Deactivate(SnippetEventArgs e)
	{
	}
}
