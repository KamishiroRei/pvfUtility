using System;

namespace ICSharpCode.AvalonEdit.Search;

public class SearchOptionsChangedEventArgs : EventArgs
{
	public string SearchPattern { get; private set; }

	public bool MatchCase { get; private set; }

	public bool UseRegex { get; private set; }

	public bool WholeWords { get; private set; }

	public SearchOptionsChangedEventArgs(string searchPattern, bool matchCase, bool useRegex, bool wholeWords)
	{
		SearchPattern = searchPattern;
		MatchCase = matchCase;
		UseRegex = useRegex;
		WholeWords = wholeWords;
	}
}
