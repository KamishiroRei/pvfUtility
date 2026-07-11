namespace ICSharpCode.AvalonEdit.Document;

public interface IDocumentLine : ISegment
{
	int TotalLength { get; }

	int DelimiterLength { get; }

	int LineNumber { get; }

	IDocumentLine PreviousLine { get; }

	IDocumentLine NextLine { get; }

	bool IsDeleted { get; }
}
