using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

[Serializable]
public class XshdSyntaxDefinition
{
	public string Name { get; set; }

	public IList<string> Extensions { get; private set; }

	public IList<XshdElement> Elements { get; private set; }

	public XshdSyntaxDefinition()
	{
		Elements = new NullSafeCollection<XshdElement>();
		Extensions = new NullSafeCollection<string>();
	}

	public void AcceptElements(IXshdVisitor visitor)
	{
		foreach (XshdElement element in Elements)
		{
			element.AcceptVisitor(visitor);
		}
	}
}
