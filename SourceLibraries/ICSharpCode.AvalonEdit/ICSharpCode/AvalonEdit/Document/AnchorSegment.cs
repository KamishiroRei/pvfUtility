using System;
using System.Globalization;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document;

public sealed class AnchorSegment : ISegment
{
	private readonly TextAnchor start;

	private readonly TextAnchor end;

	public int Offset => start.Offset;

	public int Length => Math.Max(0, end.Offset - start.Offset);

	public int EndOffset => Math.Max(start.Offset, end.Offset);

	public AnchorSegment(TextAnchor start, TextAnchor end)
	{
		if (start == null)
		{
			throw new ArgumentNullException("start");
		}
		if (end == null)
		{
			throw new ArgumentNullException("end");
		}
		if (!start.SurviveDeletion)
		{
			throw new ArgumentException("Anchors for AnchorSegment must use SurviveDeletion", "start");
		}
		if (!end.SurviveDeletion)
		{
			throw new ArgumentException("Anchors for AnchorSegment must use SurviveDeletion", "end");
		}
		this.start = start;
		this.end = end;
	}

	public AnchorSegment(TextDocument document, ISegment segment)
		: this(document, ThrowUtil.CheckNotNull(segment, "segment").Offset, segment.Length)
	{
	}

	public AnchorSegment(TextDocument document, int offset, int length)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		start = document.CreateAnchor(offset);
		start.SurviveDeletion = true;
		start.MovementType = AnchorMovementType.AfterInsertion;
		end = document.CreateAnchor(offset + length);
		end.SurviveDeletion = true;
		end.MovementType = AnchorMovementType.BeforeInsertion;
	}

	public override string ToString()
	{
		return "[Offset=" + Offset.ToString(CultureInfo.InvariantCulture) + ", EndOffset=" + EndOffset.ToString(CultureInfo.InvariantCulture) + "]";
	}
}
