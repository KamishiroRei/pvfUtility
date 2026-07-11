using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Document;

public interface ITextSourceVersion
{
	bool BelongsToSameDocumentAs(ITextSourceVersion other);

	int CompareAge(ITextSourceVersion other);

	IEnumerable<TextChangeEventArgs> GetChangesTo(ITextSourceVersion other);

	int MoveOffsetTo(ITextSourceVersion other, int oldOffset, AnchorMovementType movement = AnchorMovementType.Default);
}
