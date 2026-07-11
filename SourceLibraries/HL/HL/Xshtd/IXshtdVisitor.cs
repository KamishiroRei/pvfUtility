namespace HL.Xshtd;

public interface IXshtdVisitor
{
	object VisitColor(XshtdSyntaxDefinition syntax, XshtdColor color);

	object VisitSyntaxDefinition(XshtdSyntaxDefinition syntax);

	object VisitGlobalStyles(XshtdGlobalStyles globStyles);

	object VisitGlobalStyle(XshtdGlobalStyles globStyles, XshtdGlobalStyle style);
}
