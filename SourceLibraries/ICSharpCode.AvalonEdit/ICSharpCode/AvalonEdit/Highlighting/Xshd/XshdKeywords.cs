using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public class XshdKeywords : XshdElement
{
	private readonly NullSafeCollection<string> words = new NullSafeCollection<string>();

	public XshdReference<XshdColor> ColorReference { get; set; }

	public IList<string> Words => words;

	public override object AcceptVisitor(IXshdVisitor visitor)
	{
		return visitor.VisitKeywords(this);
	}
}
