using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Highlighting;

public class RichText
{
	public static readonly RichText Empty = new RichText(string.Empty);

	private readonly string text;

	internal readonly int[] stateChangeOffsets;

	internal readonly HighlightingColor[] stateChanges;

	public string Text => text;

	public int Length => text.Length;

	public RichText(string text, RichTextModel model = null)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		this.text = text;
		if (model != null)
		{
			HighlightedSection[] array = model.GetHighlightedSections(0, text.Length).ToArray();
			stateChangeOffsets = new int[array.Length];
			stateChanges = new HighlightingColor[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				stateChangeOffsets[i] = array[i].Offset;
				stateChanges[i] = array[i].Color;
			}
		}
		else
		{
			stateChangeOffsets = new int[1];
			stateChanges = new HighlightingColor[1] { HighlightingColor.Empty };
		}
	}

	internal RichText(string text, int[] offsets, HighlightingColor[] states)
	{
		this.text = text;
		stateChangeOffsets = offsets;
		stateChanges = states;
	}

	private int GetIndexForOffset(int offset)
	{
		if (offset < 0 || offset > text.Length)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		int num = Array.BinarySearch(stateChangeOffsets, offset);
		if (num < 0)
		{
			num = ~num - 1;
		}
		return num;
	}

	private int GetEnd(int index)
	{
		if (index + 1 < stateChangeOffsets.Length)
		{
			return stateChangeOffsets[index + 1];
		}
		return text.Length;
	}

	public HighlightingColor GetHighlightingAt(int offset)
	{
		return stateChanges[GetIndexForOffset(offset)];
	}

	public IEnumerable<HighlightedSection> GetHighlightedSections(int offset, int length)
	{
		int index = GetIndexForOffset(offset);
		int num = offset;
		int endOffset = offset + length;
		while (num < endOffset)
		{
			int endPos = Math.Min(endOffset, GetEnd(index));
			yield return new HighlightedSection
			{
				Offset = num,
				Length = endPos - num,
				Color = stateChanges[index]
			};
			num = endPos;
			index++;
		}
	}

	public RichTextModel ToRichTextModel()
	{
		return new RichTextModel(stateChangeOffsets, stateChanges.Select((HighlightingColor ch) => ch.Clone()).ToArray());
	}

	public override string ToString()
	{
		return text;
	}

	public Run[] CreateRuns()
	{
		Run[] array = new Run[stateChanges.Length];
		for (int i = 0; i < array.Length; i++)
		{
			int num = stateChangeOffsets[i];
			int num2 = ((i + 1 < stateChangeOffsets.Length) ? stateChangeOffsets[i + 1] : text.Length);
			Run run = new Run(text.Substring(num, num2 - num));
			HighlightingColor state = stateChanges[i];
			ApplyColorToTextElement(run, state);
			array[i] = run;
		}
		return array;
	}

	internal static void ApplyColorToTextElement(TextElement r, HighlightingColor state)
	{
		if (state.Foreground != null)
		{
			r.Foreground = state.Foreground.GetBrush(null);
		}
		if (state.Background != null)
		{
			r.Background = state.Background.GetBrush(null);
		}
		if (state.FontWeight.HasValue)
		{
			r.FontWeight = state.FontWeight.Value;
		}
		if (state.FontStyle.HasValue)
		{
			r.FontStyle = state.FontStyle.Value;
		}
	}

	public string ToHtml(HtmlOptions options = null)
	{
		StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
		using (HtmlRichTextWriter htmlRichTextWriter = new HtmlRichTextWriter(stringWriter, options))
		{
			htmlRichTextWriter.Write(this);
		}
		return stringWriter.ToString();
	}

	public string ToHtml(int offset, int length, HtmlOptions options = null)
	{
		StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
		using (HtmlRichTextWriter htmlRichTextWriter = new HtmlRichTextWriter(stringWriter, options))
		{
			htmlRichTextWriter.Write(this, offset, length);
		}
		return stringWriter.ToString();
	}

	public RichText Substring(int offset, int length)
	{
		if (offset == 0 && length == Length)
		{
			return this;
		}
		string obj = text.Substring(offset, length);
		RichTextModel richTextModel = ToRichTextModel();
		richTextModel.UpdateOffsets(new OffsetChangeMap(2)
		{
			new OffsetChangeMapEntry(offset + length, text.Length - offset - length, 0),
			new OffsetChangeMapEntry(0, offset, 0)
		});
		return new RichText(obj, richTextModel);
	}

	public static RichText Concat(params RichText[] texts)
	{
		if (texts == null || texts.Length == 0)
		{
			return Empty;
		}
		if (texts.Length == 1)
		{
			return texts[0];
		}
		string text = string.Concat(texts.Select((RichText txt) => txt.text));
		RichTextModel richTextModel = texts[0].ToRichTextModel();
		int num = texts[0].Length;
		for (int num2 = 1; num2 < texts.Length; num2++)
		{
			richTextModel.Append(num, texts[num2].stateChangeOffsets, texts[num2].stateChanges);
			num += texts[num2].Length;
		}
		return new RichText(text, richTextModel);
	}

	public static RichText operator +(RichText a, RichText b)
	{
		return Concat(a, b);
	}

	public static implicit operator RichText(string text)
	{
		if (text != null)
		{
			return new RichText(text);
		}
		return null;
	}
}
