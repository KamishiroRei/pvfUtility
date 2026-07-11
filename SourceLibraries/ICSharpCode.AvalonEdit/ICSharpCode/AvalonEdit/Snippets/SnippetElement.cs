using System;
using System.Windows.Documents;

namespace ICSharpCode.AvalonEdit.Snippets;

[Serializable]
public abstract class SnippetElement
{
	public abstract void Insert(InsertionContext context);

	public virtual Inline ToTextRun()
	{
		return null;
	}
}
