using System;

namespace HL.Xshtd;

[Serializable]
public abstract class XshtdElement
{
	public int LineNumber { get; set; }

	public int ColumnNumber { get; set; }

	public abstract object AcceptVisitor(IXshtdVisitor visitor);
}
