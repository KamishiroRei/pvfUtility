using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Utils;

namespace HL.Xshtd;

[Serializable]
public class XshtdSyntaxDefinition : XshtdElement
{
	public string Name { get; set; }

	public IList<string> Extensions { get; private set; }

	public IList<XshtdElement> Elements { get; private set; }

	public XshtdSyntaxDefinition()
	{
		Elements = new NullSafeCollection<XshtdElement>();
		Extensions = new NullSafeCollection<string>();
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
		return visitor.VisitSyntaxDefinition(this);
	}
}
