using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting;

[Serializable]
public class HighlightingRuleSet
{
	public string Name { get; set; }

	public IList<HighlightingSpan> Spans { get; private set; }

	public IList<HighlightingRule> Rules { get; private set; }

	public HighlightingRuleSet()
	{
		Spans = new NullSafeCollection<HighlightingSpan>();
		Rules = new NullSafeCollection<HighlightingRule>();
	}

	public override string ToString()
	{
		return "[" + GetType().Name + " " + Name + "]";
	}
}
