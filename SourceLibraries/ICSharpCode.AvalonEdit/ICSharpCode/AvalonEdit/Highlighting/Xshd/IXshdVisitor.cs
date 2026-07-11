namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

public interface IXshdVisitor
{
	object VisitRuleSet(XshdRuleSet ruleSet);

	object VisitColor(XshdColor color);

	object VisitKeywords(XshdKeywords keywords);

	object VisitSpan(XshdSpan span);

	object VisitImport(XshdImport import);

	object VisitRule(XshdRule rule);
}
