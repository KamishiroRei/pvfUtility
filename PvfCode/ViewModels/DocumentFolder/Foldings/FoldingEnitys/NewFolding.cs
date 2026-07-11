using System;
using System.Runtime.CompilerServices;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

public class NewFolding : ISegment
{
	[CompilerGenerated]
	private int liAyB7Q81V;

	[CompilerGenerated]
	private int KpmyFE5Jt9;

	[CompilerGenerated]
	private string FEwyrTjiKs;

	[CompilerGenerated]
	private bool v0FyWH3R59;

	[CompilerGenerated]
	private bool ghqymSEbjn;

	public int StartOffset
	{
		[CompilerGenerated]
		get
		{
			return liAyB7Q81V;
		}
		[CompilerGenerated]
		set
		{
			liAyB7Q81V = value;
		}
	}

	public int EndOffset
	{
		[CompilerGenerated]
		get
		{
			return KpmyFE5Jt9;
		}
		[CompilerGenerated]
		set
		{
			KpmyFE5Jt9 = value;
		}
	}

	public string Name
	{
		[CompilerGenerated]
		get
		{
			return FEwyrTjiKs;
		}
		[CompilerGenerated]
		set
		{
			FEwyrTjiKs = value;
		}
	}

	public bool DefaultClosed
	{
		[CompilerGenerated]
		get
		{
			return v0FyWH3R59;
		}
		[CompilerGenerated]
		set
		{
			v0FyWH3R59 = value;
		}
	}

	public bool IsDefinition
	{
		[CompilerGenerated]
		get
		{
			return ghqymSEbjn;
		}
		[CompilerGenerated]
		set
		{
			ghqymSEbjn = value;
		}
	}

	int ISegment.Offset => StartOffset;

	int ISegment.Length => EndOffset - StartOffset;

	public NewFolding()
	{
	}

	public NewFolding(int start, int end)
	{
		if (start > end)
		{
			throw new ArgumentException("'start' must be less than 'end'");
		}
		StartOffset = start;
		EndOffset = end;
		Name = null;
		DefaultClosed = false;
	}
}
