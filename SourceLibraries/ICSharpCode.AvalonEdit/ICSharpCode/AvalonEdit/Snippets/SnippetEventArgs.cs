using System;

namespace ICSharpCode.AvalonEdit.Snippets;

public class SnippetEventArgs : EventArgs
{
	public DeactivateReason Reason { get; private set; }

	public SnippetEventArgs(DeactivateReason reason)
	{
		Reason = reason;
	}
}
