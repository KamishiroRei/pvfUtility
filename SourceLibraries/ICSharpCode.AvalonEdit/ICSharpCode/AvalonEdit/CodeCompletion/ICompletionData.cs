using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.AvalonEdit.CodeCompletion;

public interface ICompletionData
{
	ImageSource Image { get; }

	string Text { get; }

	object Content { get; }

	object Description { get; }

	double Priority { get; }

	void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs);
}
