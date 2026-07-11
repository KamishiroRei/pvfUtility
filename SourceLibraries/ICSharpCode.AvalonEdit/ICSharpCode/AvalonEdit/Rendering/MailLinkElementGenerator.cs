using System;
using System.Text.RegularExpressions;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class MailLinkElementGenerator : LinkElementGenerator
{
	public MailLinkElementGenerator()
		: base(LinkElementGenerator.defaultMailRegex)
	{
	}

	protected override Uri GetUriFromMatch(Match match)
	{
		string uriString = "mailto:" + match.Value;
		if (Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
		{
			return new Uri(uriString);
		}
		return null;
	}
}
