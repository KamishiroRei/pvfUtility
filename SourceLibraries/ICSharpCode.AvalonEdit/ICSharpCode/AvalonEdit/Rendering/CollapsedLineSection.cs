using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Rendering;

public sealed class CollapsedLineSection
{
	private DocumentLine start;

	private DocumentLine end;

	private HeightTree heightTree;

	private const string ID = "";

	public bool IsCollapsed => start != null;

	public DocumentLine Start
	{
		get
		{
			return start;
		}
		internal set
		{
			start = value;
		}
	}

	public DocumentLine End
	{
		get
		{
			return end;
		}
		internal set
		{
			end = value;
		}
	}

	internal CollapsedLineSection(HeightTree heightTree, DocumentLine start, DocumentLine end)
	{
		this.heightTree = heightTree;
		this.start = start;
		this.end = end;
	}

	public void Uncollapse()
	{
		if (start != null)
		{
			if (!heightTree.IsDisposed)
			{
				heightTree.Uncollapse(this);
			}
			start = null;
			end = null;
		}
	}

	public override string ToString()
	{
		return "[CollapsedSection Start=" + ((start != null) ? start.LineNumber.ToString() : "null") + " End=" + ((end != null) ? end.LineNumber.ToString() : "null") + "]";
	}
}
