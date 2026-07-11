using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Utils;

namespace HL.Xshtd;

[Serializable]
public class XhstdThemeDefinition : XshtdElement
{
	public string Name { get; set; }

	public IList<XshtdElement> Elements { get; private set; }

	public XshtdGlobalStyles GlobalStyleElements { get; private set; }

	public XhstdThemeDefinition()
	{
		Elements = new NullSafeCollection<XshtdElement>();
		GlobalStyleElements = new XshtdGlobalStyles();
	}

	public override object AcceptVisitor(IXshtdVisitor visitor)
	{
		foreach (XshtdElement element in Elements)
		{
			element.AcceptVisitor(visitor);
		}
		foreach (XshtdElement element2 in GlobalStyleElements.Elements)
		{
			element2.AcceptVisitor(visitor);
		}
		return null;
	}
}
