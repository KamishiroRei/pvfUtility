using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Utils;

namespace HL.Xshtd;

[Serializable]
public class XshtdGlobalStyles : XshtdElement
{
	public IList<XshtdElement> Elements { get; private set; }

	public XshtdGlobalStyles()
	{
		Elements = new NullSafeCollection<XshtdElement>();
	}

	public void AcceptElements(IXshtdVisitor visitor)
	{
		foreach (XshtdElement element in Elements)
		{
			element.AcceptVisitor(visitor);
		}
	}

	public override object AcceptVisitor(IXshtdVisitor visitor)
	{
		return visitor.VisitGlobalStyles(this);
	}
}
