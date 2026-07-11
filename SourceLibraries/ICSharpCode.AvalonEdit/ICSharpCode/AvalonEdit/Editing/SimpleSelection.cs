using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class SimpleSelection : Selection
{
	private readonly TextViewPosition start;

	private readonly TextViewPosition end;

	private readonly int startOffset;

	private readonly int endOffset;

	public override IEnumerable<SelectionSegment> Segments => ExtensionMethods.Sequence(new SelectionSegment(startOffset, start.VisualColumn, endOffset, end.VisualColumn));

	public override ISegment SurroundingSegment => new SelectionSegment(startOffset, endOffset);

	public override TextViewPosition StartPosition => start;

	public override TextViewPosition EndPosition => end;

	public override bool IsEmpty
	{
		get
		{
			if (startOffset == endOffset)
			{
				return start.VisualColumn == end.VisualColumn;
			}
			return false;
		}
	}

	public override int Length => Math.Abs(endOffset - startOffset);

	internal SimpleSelection(TextArea textArea, TextViewPosition start, TextViewPosition end)
		: base(textArea)
	{
		this.start = start;
		this.end = end;
		startOffset = textArea.Document.GetOffset(start.Location);
		endOffset = textArea.Document.GetOffset(end.Location);
	}

	public override void ReplaceSelectionWithText(string newText)
	{
		if (newText == null)
		{
			throw new ArgumentNullException("newText");
		}
		using (textArea.Document.RunUpdate())
		{
			ISegment[] deletableSegments = textArea.GetDeletableSegments(SurroundingSegment);
			for (int num = deletableSegments.Length - 1; num >= 0; num--)
			{
				if (num == deletableSegments.Length - 1)
				{
					if (deletableSegments[num].Offset == SurroundingSegment.Offset && deletableSegments[num].Length == SurroundingSegment.Length)
					{
						newText = AddSpacesIfRequired(newText, start, end);
					}
					if (string.IsNullOrEmpty(newText))
					{
						if (start.CompareTo(end) <= 0)
						{
							textArea.Caret.Position = start;
						}
						else
						{
							textArea.Caret.Position = end;
						}
					}
					else
					{
						textArea.Caret.Offset = deletableSegments[num].EndOffset;
					}
					textArea.Document.Replace(deletableSegments[num], newText);
				}
				else
				{
					textArea.Document.Remove(deletableSegments[num]);
				}
			}
			if (deletableSegments.Length != 0)
			{
				textArea.ClearSelection();
			}
		}
	}

	public override Selection UpdateOnDocumentChange(DocumentChangeEventArgs e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		int num;
		int num2;
		if (startOffset <= endOffset)
		{
			num = e.GetNewOffset(startOffset);
			num2 = Math.Max(num, e.GetNewOffset(endOffset, AnchorMovementType.BeforeInsertion));
		}
		else
		{
			num2 = e.GetNewOffset(endOffset);
			num = Math.Max(num2, e.GetNewOffset(startOffset, AnchorMovementType.BeforeInsertion));
		}
		return Selection.Create(textArea, new TextViewPosition(textArea.Document.GetLocation(num), start.VisualColumn), new TextViewPosition(textArea.Document.GetLocation(num2), end.VisualColumn));
	}

	public override Selection SetEndpoint(TextViewPosition endPosition)
	{
		return Selection.Create(textArea, start, endPosition);
	}

	public override Selection StartSelectionOrSetEndpoint(TextViewPosition startPosition, TextViewPosition endPosition)
	{
		if (textArea.Document == null)
		{
			throw ThrowUtil.NoDocumentAssigned();
		}
		return Selection.Create(textArea, start, endPosition);
	}

	public override int GetHashCode()
	{
		return startOffset * 27811 + endOffset + textArea.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SimpleSelection simpleSelection))
		{
			return false;
		}
		if (start.Equals(simpleSelection.start) && end.Equals(simpleSelection.end) && startOffset == simpleSelection.startOffset && endOffset == simpleSelection.endOffset)
		{
			return textArea == simpleSelection.textArea;
		}
		return false;
	}

	public override string ToString()
	{
		return "[SimpleSelection Start=" + start.ToString() + " End=" + end.ToString() + "]";
	}
}
