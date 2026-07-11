using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets;

public sealed class AnchorElement : IActiveElement
{
	private AnchorSegment segment;

	private InsertionContext context;

	public bool IsEditable => false;

	public ISegment Segment => segment;

	public string Text
	{
		get
		{
			return context.Document.GetText(segment);
		}
		set
		{
			int offset = segment.Offset;
			int length = segment.Length;
			context.Document.Replace(offset, length, value);
			if (length == 0)
			{
				segment = new AnchorSegment(context.Document, offset, value.Length);
			}
		}
	}

	public string Name { get; private set; }

	public AnchorElement(AnchorSegment segment, string name, InsertionContext context)
	{
		this.segment = segment;
		this.context = context;
		Name = name;
	}

	public void OnInsertionCompleted()
	{
	}

	public void Deactivate(SnippetEventArgs e)
	{
	}
}
