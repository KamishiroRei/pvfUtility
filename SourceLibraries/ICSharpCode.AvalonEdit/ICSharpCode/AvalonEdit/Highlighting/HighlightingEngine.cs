using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting;

public class HighlightingEngine
{
	private readonly HighlightingRuleSet mainRuleSet;

	private ImmutableStack<HighlightingSpan> spanStack = ImmutableStack<HighlightingSpan>.Empty;

	private string lineText;

	private int lineStartOffset;

	private int position;

	private HighlightedLine highlightedLine;

	private static readonly HighlightingRuleSet emptyRuleSet = new HighlightingRuleSet
	{
		Name = "EmptyRuleSet"
	};

	private Stack<HighlightedSection> highlightedSectionStack;

	private HighlightedSection lastPoppedSection;

	public ImmutableStack<HighlightingSpan> CurrentSpanStack
	{
		get
		{
			return spanStack;
		}
		set
		{
			spanStack = value ?? ImmutableStack<HighlightingSpan>.Empty;
		}
	}

	private HighlightingRuleSet CurrentRuleSet
	{
		get
		{
			if (spanStack.IsEmpty)
			{
				return mainRuleSet;
			}
			return spanStack.Peek().RuleSet ?? emptyRuleSet;
		}
	}

	public HighlightingEngine(HighlightingRuleSet mainRuleSet)
	{
		if (mainRuleSet == null)
		{
			throw new ArgumentNullException("mainRuleSet");
		}
		this.mainRuleSet = mainRuleSet;
	}

	public HighlightedLine HighlightLine(IDocument document, IDocumentLine line)
	{
		lineStartOffset = line.Offset;
		lineText = document.GetText(line);
		try
		{
			highlightedLine = new HighlightedLine(document, line);
			HighlightLineInternal();
			return highlightedLine;
		}
		finally
		{
			highlightedLine = null;
			lineText = null;
			lineStartOffset = 0;
		}
	}

	public void ScanLine(IDocument document, IDocumentLine line)
	{
		lineText = document.GetText(line);
		try
		{
			HighlightLineInternal();
		}
		finally
		{
			lineText = null;
		}
	}

	private void HighlightLineInternal()
	{
		position = 0;
		ResetColorStack();
		HighlightingRuleSet currentRuleSet = CurrentRuleSet;
		Stack<Match[]> stack = new Stack<Match[]>();
		Match[] array = AllocateMatchArray(currentRuleSet.Spans.Count);
		Match match = null;
		while (true)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null || (array[i].Success && array[i].Index < position))
				{
					array[i] = currentRuleSet.Spans[i].StartExpression.Match(lineText, position);
				}
			}
			if (match == null && !spanStack.IsEmpty)
			{
				match = spanStack.Peek().EndExpression.Match(lineText, position);
			}
			Match match2 = Minimum(array, match);
			if (match2 == null)
			{
				break;
			}
			HighlightNonSpans(match2.Index);
			if (match2 == match)
			{
				HighlightingSpan highlightingSpan = spanStack.Peek();
				if (!highlightingSpan.SpanColorIncludesEnd)
				{
					PopColor();
				}
				PushColor(highlightingSpan.EndColor);
				position = match2.Index + match2.Length;
				PopColor();
				if (highlightingSpan.SpanColorIncludesEnd)
				{
					PopColor();
				}
				spanStack = spanStack.Pop();
				currentRuleSet = CurrentRuleSet;
				if (stack.Count > 0)
				{
					array = stack.Pop();
					int num = currentRuleSet.Spans.IndexOf(highlightingSpan);
					if (array[num].Index == position)
					{
						throw new InvalidOperationException("A highlighting span matched 0 characters, which would cause an endless loop.\nChange the highlighting definition so that either the start or the end regex matches at least one character.\nStart regex: " + highlightingSpan.StartExpression?.ToString() + "\nEnd regex: " + highlightingSpan.EndExpression);
					}
				}
				else
				{
					array = AllocateMatchArray(currentRuleSet.Spans.Count);
				}
			}
			else
			{
				int index = Array.IndexOf(array, match2);
				HighlightingSpan highlightingSpan2 = currentRuleSet.Spans[index];
				spanStack = spanStack.Push(highlightingSpan2);
				currentRuleSet = CurrentRuleSet;
				stack.Push(array);
				array = AllocateMatchArray(currentRuleSet.Spans.Count);
				if (highlightingSpan2.SpanColorIncludesStart)
				{
					PushColor(highlightingSpan2.SpanColor);
				}
				PushColor(highlightingSpan2.StartColor);
				position = match2.Index + match2.Length;
				PopColor();
				if (!highlightingSpan2.SpanColorIncludesStart)
				{
					PushColor(highlightingSpan2.SpanColor);
				}
			}
			match = null;
		}
		HighlightNonSpans(lineText.Length);
		PopAllColors();
	}

	private void HighlightNonSpans(int until)
	{
		if (position == until)
		{
			return;
		}
		if (highlightedLine != null)
		{
			IList<HighlightingRule> rules = CurrentRuleSet.Rules;
			Match[] array = AllocateMatchArray(rules.Count);
			while (true)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == null || (array[i].Success && array[i].Index < position))
					{
						array[i] = rules[i].Regex.Match(lineText, position, until - position);
					}
				}
				Match match = Minimum(array, null);
				if (match == null)
				{
					break;
				}
				position = match.Index;
				int index = Array.IndexOf(array, match);
				if (match.Length == 0)
				{
					throw new InvalidOperationException("A highlighting rule matched 0 characters, which would cause an endless loop.\nChange the highlighting definition so that the rule matches at least one character.\nRegex: " + rules[index].Regex);
				}
				PushColor(rules[index].Color);
				position = match.Index + match.Length;
				PopColor();
			}
		}
		position = until;
	}

	private void ResetColorStack()
	{
		lastPoppedSection = null;
		if (highlightedLine == null)
		{
			highlightedSectionStack = null;
			return;
		}
		highlightedSectionStack = new Stack<HighlightedSection>();
		foreach (HighlightingSpan item in spanStack.Reverse())
		{
			PushColor(item.SpanColor);
		}
	}

	private void PushColor(HighlightingColor color)
	{
		if (highlightedLine != null)
		{
			if (color == null)
			{
				highlightedSectionStack.Push(null);
				return;
			}
			if (lastPoppedSection != null && lastPoppedSection.Color == color && lastPoppedSection.Offset + lastPoppedSection.Length == position + lineStartOffset)
			{
				highlightedSectionStack.Push(lastPoppedSection);
				lastPoppedSection = null;
				return;
			}
			HighlightedSection item = new HighlightedSection
			{
				Offset = position + lineStartOffset,
				Color = color
			};
			highlightedLine.Sections.Add(item);
			highlightedSectionStack.Push(item);
			lastPoppedSection = null;
		}
	}

	private void PopColor()
	{
		if (highlightedLine == null)
		{
			return;
		}
		HighlightedSection highlightedSection = highlightedSectionStack.Pop();
		if (highlightedSection != null)
		{
			highlightedSection.Length = position + lineStartOffset - highlightedSection.Offset;
			if (highlightedSection.Length == 0)
			{
				highlightedLine.Sections.Remove(highlightedSection);
			}
			else
			{
				lastPoppedSection = highlightedSection;
			}
		}
	}

	private void PopAllColors()
	{
		if (highlightedSectionStack != null)
		{
			while (highlightedSectionStack.Count > 0)
			{
				PopColor();
			}
		}
	}

	private static Match Minimum(Match[] arr, Match endSpanMatch)
	{
		Match match = null;
		foreach (Match match2 in arr)
		{
			if (match2.Success && (match == null || match2.Index < match.Index))
			{
				match = match2;
			}
		}
		if (endSpanMatch != null && endSpanMatch.Success && (match == null || endSpanMatch.Index < match.Index))
		{
			return endSpanMatch;
		}
		return match;
	}

	private static Match[] AllocateMatchArray(int count)
	{
		if (count == 0)
		{
			return Empty<Match>.Array;
		}
		return new Match[count];
	}
}
