using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Highlighting;

public interface IHighlighter : IDisposable
{
	IDocument Document { get; }

	HighlightingColor DefaultTextColor { get; }

	event HighlightingStateChangedEventHandler HighlightingStateChanged;

	IEnumerable<HighlightingColor> GetColorStack(int lineNumber);

	HighlightedLine HighlightLine(int lineNumber);

	void UpdateHighlightingState(int lineNumber);

	void BeginHighlighting();

	void EndHighlighting();

	HighlightingColor GetNamedColor(string name);
}
