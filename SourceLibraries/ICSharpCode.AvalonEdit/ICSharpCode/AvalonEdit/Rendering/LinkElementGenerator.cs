using System;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering;

public class LinkElementGenerator : VisualLineElementGenerator, IBuiltinElementGenerator
{
	internal static readonly Regex defaultLinkRegex = new Regex("\\b(https?://|ftp://|www\\.)[\\w\\d\\._/\\-~%@()+:?&=#!]*[\\w\\d/]");

	internal static readonly Regex defaultMailRegex = new Regex("\\b[\\w\\d\\.\\-]+\\@[\\w\\d\\.\\-]+\\.[a-z]{2,6}\\b");

	private readonly Regex linkRegex;

	public bool RequireControlModifierForClick { get; set; }

	public LinkElementGenerator()
	{
		linkRegex = defaultLinkRegex;
		RequireControlModifierForClick = true;
	}

	protected LinkElementGenerator(Regex regex)
		: this()
	{
		if (regex == null)
		{
			throw new ArgumentNullException("regex");
		}
		linkRegex = regex;
	}

	void IBuiltinElementGenerator.FetchOptions(TextEditorOptions options)
	{
		RequireControlModifierForClick = options.RequireControlModifierForHyperlinkClick;
	}

	private Match GetMatch(int startOffset, out int matchOffset)
	{
		int endOffset = base.CurrentContext.VisualLine.LastDocumentLine.EndOffset;
		StringSegment text = base.CurrentContext.GetText(startOffset, endOffset - startOffset);
		Match match = linkRegex.Match(text.Text, text.Offset, text.Count);
		matchOffset = (match.Success ? (match.Index - text.Offset + startOffset) : (-1));
		return match;
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		GetMatch(startOffset, out var matchOffset);
		return matchOffset;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		int matchOffset;
		Match match = GetMatch(offset, out matchOffset);
		if (match.Success && matchOffset == offset)
		{
			return ConstructElementFromMatch(match);
		}
		return null;
	}

	protected virtual VisualLineElement ConstructElementFromMatch(Match m)
	{
		Uri uriFromMatch = GetUriFromMatch(m);
		if (uriFromMatch == null)
		{
			return null;
		}
		return new VisualLineLinkText(base.CurrentContext.VisualLine, m.Length)
		{
			NavigateUri = uriFromMatch,
			RequireControlModifierForClick = RequireControlModifierForClick
		};
	}

	protected virtual Uri GetUriFromMatch(Match match)
	{
		string text = match.Value;
		if (text.StartsWith("www.", StringComparison.Ordinal))
		{
			text = "http://" + text;
		}
		if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
		{
			return new Uri(text);
		}
		return null;
	}
}
