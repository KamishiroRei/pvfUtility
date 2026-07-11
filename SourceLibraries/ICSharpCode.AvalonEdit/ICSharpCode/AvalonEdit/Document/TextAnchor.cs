using System;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document;

public sealed class TextAnchor : ITextAnchor
{
	private readonly TextDocument document;

	internal TextAnchorNode node;

	public TextDocument Document => document;

	public AnchorMovementType MovementType { get; set; }

	public bool SurviveDeletion { get; set; }

	public bool IsDeleted => node == null;

	public int Offset
	{
		get
		{
			TextAnchorNode parent = node;
			if (parent == null)
			{
				throw new InvalidOperationException();
			}
			int num = parent.length;
			if (parent.left != null)
			{
				num += parent.left.totalLength;
			}
			while (parent.parent != null)
			{
				if (parent == parent.parent.right)
				{
					if (parent.parent.left != null)
					{
						num += parent.parent.left.totalLength;
					}
					num += parent.parent.length;
				}
				parent = parent.parent;
			}
			return num;
		}
	}

	public int Line => document.GetLineByOffset(Offset).LineNumber;

	public int Column
	{
		get
		{
			int offset = Offset;
			return offset - document.GetLineByOffset(offset).Offset + 1;
		}
	}

	public TextLocation Location => document.GetLocation(Offset);

	public event EventHandler Deleted;

	internal TextAnchor(TextDocument document)
	{
		this.document = document;
	}

	internal void OnDeleted(DelayedEvents delayedEvents)
	{
		node = null;
		delayedEvents.DelayedRaise(this.Deleted, this, EventArgs.Empty);
	}

	public override string ToString()
	{
		return "[TextAnchor Offset=" + Offset + "]";
	}
}
