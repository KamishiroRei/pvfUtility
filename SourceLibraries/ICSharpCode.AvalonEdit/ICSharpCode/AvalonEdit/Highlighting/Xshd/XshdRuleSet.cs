using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public class XshdRuleSet : XshdElement
{
	private readonly NullSafeCollection<XshdElement> elements = new NullSafeCollection<XshdElement>();

	public string Name { get; set; }

	public bool? IgnoreCase { get; set; }

	public IList<XshdElement> Elements => elements;

	public void AcceptElements(IXshdVisitor visitor)
	{
		foreach (XshdElement element in Elements)
		{
			element.AcceptVisitor(visitor);
		}
	}

	public override object AcceptVisitor(IXshdVisitor visitor)
	{
		return visitor.VisitRuleSet(this);
	}
}
