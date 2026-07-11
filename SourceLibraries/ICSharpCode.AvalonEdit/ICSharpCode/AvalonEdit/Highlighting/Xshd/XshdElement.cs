using System;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public abstract class XshdElement
{
	public int LineNumber { get; set; }

	public int ColumnNumber { get; set; }

	public abstract object AcceptVisitor(IXshdVisitor visitor);
}
