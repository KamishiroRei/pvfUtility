using System;

namespace ICSharpCode.AvalonEdit.Snippets;

public interface IReplaceableActiveElement : IActiveElement
{
	string Text { get; }

	event EventHandler TextChanged;
}
