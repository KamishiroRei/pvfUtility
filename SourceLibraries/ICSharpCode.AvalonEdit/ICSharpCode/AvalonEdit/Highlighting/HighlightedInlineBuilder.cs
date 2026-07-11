using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting;

[Obsolete("Use RichText / RichTextModel instead")]
public sealed class HighlightedInlineBuilder
{
	private readonly string text;

	private List<int> stateChangeOffsets = new List<int>();

	private List<HighlightingColor> stateChanges = new List<HighlightingColor>();

	public string Text => text;

	private static HighlightingBrush MakeBrush(Brush b)
	{
		if (b is SolidColorBrush brush)
		{
			return new SimpleHighlightingBrush(brush);
		}
		return null;
	}

	private int GetIndexForOffset(int offset)
	{
		if (offset < 0 || offset > text.Length)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		int num = stateChangeOffsets.BinarySearch(offset);
		if (num < 0)
		{
			num = ~num;
			if (offset < text.Length)
			{
				stateChanges.Insert(num, stateChanges[num - 1].Clone());
				stateChangeOffsets.Insert(num, offset);
			}
		}
		return num;
	}

	public HighlightedInlineBuilder(string text)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		this.text = text;
		stateChangeOffsets.Add(0);
		stateChanges.Add(new HighlightingColor());
	}

	public HighlightedInlineBuilder(RichText text)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		this.text = text.Text;
		stateChangeOffsets.AddRange(text.stateChangeOffsets);
		stateChanges.AddRange(text.stateChanges);
	}

	private HighlightedInlineBuilder(string text, List<int> offsets, List<HighlightingColor> states)
	{
		this.text = text;
		stateChangeOffsets = offsets;
		stateChanges = states;
	}

	public void SetHighlighting(int offset, int length, HighlightingColor color)
	{
		if (color == null)
		{
			throw new ArgumentNullException("color");
		}
		if (color.Foreground != null || color.Background != null || color.FontStyle.HasValue || color.FontWeight.HasValue || color.Underline.HasValue)
		{
			int indexForOffset = GetIndexForOffset(offset);
			int indexForOffset2 = GetIndexForOffset(offset + length);
			for (int i = indexForOffset; i < indexForOffset2; i++)
			{
				stateChanges[i].MergeWith(color);
			}
		}
	}

	public void SetForeground(int offset, int length, Brush brush)
	{
		int indexForOffset = GetIndexForOffset(offset);
		int indexForOffset2 = GetIndexForOffset(offset + length);
		HighlightingBrush foreground = MakeBrush(brush);
		for (int i = indexForOffset; i < indexForOffset2; i++)
		{
			stateChanges[i].Foreground = foreground;
		}
	}

	public void SetBackground(int offset, int length, Brush brush)
	{
		int indexForOffset = GetIndexForOffset(offset);
		int indexForOffset2 = GetIndexForOffset(offset + length);
		HighlightingBrush background = MakeBrush(brush);
		for (int i = indexForOffset; i < indexForOffset2; i++)
		{
			stateChanges[i].Background = background;
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

	public Run[] CreateRuns()
	{
		return ToRichText().CreateRuns();
	}

	public RichText ToRichText()
	{
		return new RichText(text, stateChangeOffsets.ToArray(), stateChanges.Select(FreezableHelper.GetFrozenClone).ToArray());
	}

	public HighlightedInlineBuilder Clone()
	{
		return new HighlightedInlineBuilder(text, stateChangeOffsets.ToList(), stateChanges.Select((HighlightingColor sc) => sc.Clone()).ToList());
	}
}
