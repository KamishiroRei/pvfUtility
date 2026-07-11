using System;
using System.Windows.Media;

namespace HL.Xshtd;

[Serializable]
public class XshtdGlobalStyle : XshtdElement
{
	private readonly XshtdGlobalStyles _styles;

	public string TypeName { get; set; }

	public Color? foreground { get; set; }

	public Color? background { get; set; }

	public Color? bordercolor { get; set; }

	public XshtdGlobalStyle(XshtdGlobalStyles styles)
		: this()
	{
		_styles = styles;
	}

	protected XshtdGlobalStyle()
	{
	}

	public override object AcceptVisitor(IXshtdVisitor visitor)
	{
		return visitor.VisitGlobalStyle(_styles, this);
	}
}
