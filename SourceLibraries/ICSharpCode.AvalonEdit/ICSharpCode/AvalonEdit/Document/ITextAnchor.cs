using System;

namespace ICSharpCode.AvalonEdit.Document;

public interface ITextAnchor
{
	TextLocation Location { get; }

	int Offset { get; }

	AnchorMovementType MovementType { get; set; }

	bool SurviveDeletion { get; set; }

	bool IsDeleted { get; }

	int Line { get; }

	int Column { get; }

	event EventHandler Deleted;
}
