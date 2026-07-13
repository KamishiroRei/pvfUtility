using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.Foldings;

public class FoldingScriptSection : FoldingScriptSectionBase
{
	public string SectionName { get; set; }

	public bool HasEndSection { get; set; }

	public List<TextSegment> Child { get; set; }

	public FoldingScriptSection()
	{
		Child = new List<TextSegment>();
	}
}
