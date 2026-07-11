using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Highlighting;

public sealed class RichTextModel
{
	private List<int> stateChangeOffsets = new List<int>();

	private List<HighlightingColor> stateChanges = new List<HighlightingColor>();

	private int GetIndexForOffset(int offset)
	{
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		int num = stateChangeOffsets.BinarySearch(offset);
		if (num < 0)
		{
			num = ~num;
			stateChanges.Insert(num, stateChanges[num - 1].Clone());
			stateChangeOffsets.Insert(num, offset);
		}
		return num;
	}

	private int GetIndexForOffsetUseExistingSegment(int offset)
	{
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		int num = stateChangeOffsets.BinarySearch(offset);
		if (num < 0)
		{
			num = ~num - 1;
		}
		return num;
	}

	private int GetEnd(int index)
	{
		if (index + 1 < stateChangeOffsets.Count)
		{
			return stateChangeOffsets[index + 1];
		}
		return int.MaxValue;
	}

	public RichTextModel()
	{
		stateChangeOffsets.Add(0);
		stateChanges.Add(new HighlightingColor());
	}

	internal RichTextModel(int[] stateChangeOffsets, HighlightingColor[] stateChanges)
	{
		this.stateChangeOffsets.AddRange(stateChangeOffsets);
		this.stateChanges.AddRange(stateChanges);
	}

	public void UpdateOffsets(TextChangeEventArgs e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		UpdateOffsets(e.GetNewOffset);
	}

	public void UpdateOffsets(OffsetChangeMap change)
	{
		if (change == null)
		{
			throw new ArgumentNullException("change");
		}
		UpdateOffsets(change.GetNewOffset);
	}

	public void UpdateOffsets(OffsetChangeMapEntry change)
	{
		UpdateOffsets(((OffsetChangeMapEntry)change).GetNewOffset);
	}

	private void UpdateOffsets(Func<int, AnchorMovementType, int> updateOffset)
	{
		int i = 1;
		int num = 1;
		for (; i < stateChangeOffsets.Count; i++)
		{
			int num2 = updateOffset(stateChangeOffsets[i], AnchorMovementType.Default);
			if (num2 == stateChangeOffsets[num - 1])
			{
				stateChanges[num - 1] = stateChanges[i];
				continue;
			}
			stateChangeOffsets[num] = num2;
			stateChanges[num] = stateChanges[i];
			num++;
		}
		stateChangeOffsets.RemoveRange(num, stateChangeOffsets.Count - num);
		stateChanges.RemoveRange(num, stateChanges.Count - num);
	}

	internal void Append(int offset, int[] newOffsets, HighlightingColor[] newColors)
	{
		while (stateChangeOffsets.Count > 0 && stateChangeOffsets.Last() >= offset)
		{
			stateChangeOffsets.RemoveAt(stateChangeOffsets.Count - 1);
			stateChanges.RemoveAt(stateChanges.Count - 1);
		}
		for (int i = 0; i < newOffsets.Length; i++)
		{
			stateChangeOffsets.Add(offset + newOffsets[i]);
			stateChanges.Add(newColors[i]);
		}
	}

	public HighlightingColor GetHighlightingAt(int offset)
	{
		return stateChanges[GetIndexForOffsetUseExistingSegment(offset)].Clone();
	}

	public void ApplyHighlighting(int offset, int length, HighlightingColor color)
	{
		if (color != null && !color.IsEmptyForMerge)
		{
			int indexForOffset = GetIndexForOffset(offset);
			int indexForOffset2 = GetIndexForOffset(offset + length);
			for (int i = indexForOffset; i < indexForOffset2; i++)
			{
				stateChanges[i].MergeWith(color);
			}
		}
	}

	public void SetHighlighting(int offset, int length, HighlightingColor color)
	{
		if (length > 0)
		{
			int indexForOffset = GetIndexForOffset(offset);
			int indexForOffset2 = GetIndexForOffset(offset + length);
			stateChanges[indexForOffset] = ((color != null) ? color.Clone() : new HighlightingColor());
			stateChanges.RemoveRange(indexForOffset + 1, indexForOffset2 - (indexForOffset + 1));
			stateChangeOffsets.RemoveRange(indexForOffset + 1, indexForOffset2 - (indexForOffset + 1));
		}
	}

	public void SetForeground(int offset, int length, HighlightingBrush brush)
	{
		int indexForOffset = GetIndexForOffset(offset);
		int indexForOffset2 = GetIndexForOffset(offset + length);
		for (int i = indexForOffset; i < indexForOffset2; i++)
		{
			stateChanges[i].Foreground = brush;
		}
	}

	public void SetBackground(int offset, int length, HighlightingBrush brush)
	{
		int indexForOffset = GetIndexForOffset(offset);
		int indexForOffset2 = GetIndexForOffset(offset + length);
		for (int i = indexForOffset; i < indexForOffset2; i++)
		{
			stateChanges[i].Background = brush;
		}
	}

	public void SetFontWeight(int offset, int length, FontWeight weight)
	{
		int indexForOffset = GetIndexForOffset(offset);
		int indexForOffset2 = GetIndexForOffset(offset + length);
		for (int i = indexForOffset; i < indexForOffset2; i++)
		{
			stateChanges[i].FontWeight = weight;
		}
	}

	public void SetFontStyle(int offset, int length, FontStyle style)
	{
		int indexForOffset = GetIndexForOffset(offset);
		int indexForOffset2 = GetIndexForOffset(offset + length);
		for (int i = indexForOffset; i < indexForOffset2; i++)
		{
			stateChanges[i].FontStyle = style;
		}
	}

	public IEnumerable<HighlightedSection> GetHighlightedSections(int offset, int length)
	{
		int index = GetIndexForOffsetUseExistingSegment(offset);
		int num = offset;
		int endOffset = offset + length;
		while (num < endOffset)
		{
			int endPos = Math.Min(endOffset, GetEnd(index));
			yield return new HighlightedSection
			{
				Offset = num,
				Length = endPos - num,
				Color = stateChanges[index].Clone()
			};
			num = endPos;
			index++;
		}
	}

	public Run[] CreateRuns(ITextSource textSource)
	{
		Run[] array = new Run[stateChanges.Count];
		for (int i = 0; i < array.Length; i++)
		{
			int num = stateChangeOffsets[i];
			int num2 = ((i + 1 < stateChangeOffsets.Count) ? stateChangeOffsets[i + 1] : textSource.TextLength);
			Run run = new Run(textSource.GetText(num, num2 - num));
			HighlightingColor state = stateChanges[i];
			RichText.ApplyColorToTextElement(run, state);
			array[i] = run;
		}
		return array;
	}
}
