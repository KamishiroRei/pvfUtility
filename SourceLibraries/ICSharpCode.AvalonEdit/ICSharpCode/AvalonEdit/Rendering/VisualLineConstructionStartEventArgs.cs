using System;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Rendering;

public class VisualLineConstructionStartEventArgs : EventArgs
{
	public DocumentLine FirstLineInView { get; private set; }

	public VisualLineConstructionStartEventArgs(DocumentLine firstLineInView)
	{
		if (firstLineInView == null)
		{
			throw new ArgumentNullException("firstLineInView");
		}
		FirstLineInView = firstLineInView;
	}
}
