using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.Foldings;

public class FoldingScriptSection : FoldingScriptSectionBase
{
	[CompilerGenerated]
	private string ODZYmb8AQM;

	[CompilerGenerated]
	private bool lMUY2mFnel;

	[CompilerGenerated]
	private List<TextSegment> DOtYfhln0s;

	public string SectionName
	{
		[CompilerGenerated]
		get
		{
			return ODZYmb8AQM;
		}
		[CompilerGenerated]
		set
		{
			ODZYmb8AQM = value;
		}
	}

	public bool HasEndSection
	{
		[CompilerGenerated]
		get
		{
			return lMUY2mFnel;
		}
		[CompilerGenerated]
		set
		{
			lMUY2mFnel = value;
		}
	}

	public List<TextSegment> Child
	{
		[CompilerGenerated]
		get
		{
			return DOtYfhln0s;
		}
		[CompilerGenerated]
		set
		{
			DOtYfhln0s = value;
		}
	}

	public FoldingScriptSection()
	{
		Child = new List<TextSegment>();
	}
}
