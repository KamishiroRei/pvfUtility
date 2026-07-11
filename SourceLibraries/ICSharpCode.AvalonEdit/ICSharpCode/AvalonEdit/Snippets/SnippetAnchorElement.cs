using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets;

public sealed class SnippetAnchorElement : SnippetElement
{
	public string Name { get; private set; }

	public SnippetAnchorElement(string name)
	{
		Name = name;
	}

	public override void Insert(InsertionContext context)
	{
		TextAnchor textAnchor = context.Document.CreateAnchor(context.InsertionPosition);
		textAnchor.MovementType = AnchorMovementType.BeforeInsertion;
		textAnchor.SurviveDeletion = true;
		AnchorSegment segment = new AnchorSegment(textAnchor, textAnchor);
		context.RegisterActiveElement(this, new AnchorElement(segment, Name, context));
	}
}
