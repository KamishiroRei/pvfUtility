using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting;

public class HighlightedLine
{
	private sealed class HtmlElement : IComparable<HtmlElement>
	{
		internal readonly int Offset;

		internal readonly int Nesting;

		internal readonly bool IsEnd;

		internal readonly HighlightingColor Color;

		public HtmlElement(int offset, int nesting, bool isEnd, HighlightingColor color)
		{
			Offset = offset;
			Nesting = nesting;
			IsEnd = isEnd;
			Color = color;
		}

		public int CompareTo(HtmlElement other)
		{
			int num = Offset.CompareTo(other.Offset);
			if (num != 0)
			{
				return num;
			}
			if (IsEnd != other.IsEnd)
			{
				if (IsEnd)
				{
					return -1;
				}
				return 1;
			}
			if (IsEnd)
			{
				return other.Nesting.CompareTo(Nesting);
			}
			return Nesting.CompareTo(other.Nesting);
		}
	}

	public IDocument Document { get; private set; }

	public IDocumentLine DocumentLine { get; private set; }

	public IList<HighlightedSection> Sections { get; private set; }

	public HighlightedLine(IDocument document, IDocumentLine documentLine)
	{
		if (document == null)
		{
			throw new ArgumentNullException("document");
		}
		Document = document;
		DocumentLine = documentLine;
		Sections = new NullSafeCollection<HighlightedSection>();
	}

	public void ValidateInvariants()
	{
		int offset = DocumentLine.Offset;
		int endOffset = DocumentLine.EndOffset;
		for (int i = 0; i < Sections.Count; i++)
		{
			HighlightedSection highlightedSection = Sections[i];
			if (highlightedSection.Offset < offset || highlightedSection.Length < 0 || highlightedSection.Offset + highlightedSection.Length > endOffset)
			{
				throw new InvalidOperationException("Section is outside line bounds");
			}
			for (int j = i + 1; j < Sections.Count; j++)
			{
				HighlightedSection highlightedSection2 = Sections[j];
				if (highlightedSection2.Offset < highlightedSection.Offset + highlightedSection.Length && (highlightedSection2.Offset < highlightedSection.Offset || highlightedSection2.Offset + highlightedSection2.Length > highlightedSection.Offset + highlightedSection.Length))
				{
					throw new InvalidOperationException("Sections are overlapping or incorrectly sorted.");
				}
			}
		}
	}

	public void MergeWith(HighlightedLine additionalLine)
	{
		if (additionalLine == null)
		{
			return;
		}
		int i = 0;
		Stack<int> stack = new Stack<int>();
		int endOffset = DocumentLine.EndOffset;
		stack.Push(endOffset);
		foreach (HighlightedSection section in additionalLine.Sections)
		{
			int newSectionStart = section.Offset;
			for (; i < Sections.Count; i++)
			{
				HighlightedSection highlightedSection = Sections[i];
				if (section.Offset < highlightedSection.Offset)
				{
					break;
				}
				while (highlightedSection.Offset > stack.Peek())
				{
					stack.Pop();
				}
				stack.Push(highlightedSection.Offset + highlightedSection.Length);
			}
			Stack<int> stack2 = new Stack<int>(stack.Reverse());
			int j;
			for (j = i; j < Sections.Count; j++)
			{
				HighlightedSection highlightedSection2 = Sections[j];
				if (section.Offset + section.Length <= highlightedSection2.Offset)
				{
					break;
				}
				Insert(ref j, ref newSectionStart, highlightedSection2.Offset, section.Color, stack2);
				while (highlightedSection2.Offset > stack2.Peek())
				{
					stack2.Pop();
				}
				stack2.Push(highlightedSection2.Offset + highlightedSection2.Length);
			}
			Insert(ref j, ref newSectionStart, section.Offset + section.Length, section.Color, stack2);
		}
	}

	private void Insert(ref int pos, ref int newSectionStart, int insertionEndPos, HighlightingColor color, Stack<int> insertionStack)
	{
		if (newSectionStart >= insertionEndPos)
		{
			return;
		}
		while (insertionStack.Peek() <= newSectionStart)
		{
			insertionStack.Pop();
		}
		while (insertionStack.Peek() < insertionEndPos)
		{
			int num = insertionStack.Pop();
			if (num > newSectionStart)
			{
				Sections.Insert(pos++, new HighlightedSection
				{
					Offset = newSectionStart,
					Length = num - newSectionStart,
					Color = color
				});
				newSectionStart = num;
			}
		}
		if (insertionEndPos > newSectionStart)
		{
			Sections.Insert(pos++, new HighlightedSection
			{
				Offset = newSectionStart,
				Length = insertionEndPos - newSectionStart,
				Color = color
			});
			newSectionStart = insertionEndPos;
		}
	}

	internal void WriteTo(RichTextWriter writer)
	{
		int offset = DocumentLine.Offset;
		WriteTo(writer, offset, offset + DocumentLine.Length);
	}

	internal void WriteTo(RichTextWriter writer, int startOffset, int endOffset)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		int offset = DocumentLine.Offset;
		int num = offset + DocumentLine.Length;
		if (startOffset < offset || startOffset > num)
		{
			throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be between " + offset + " and " + num);
		}
		if (endOffset < startOffset || endOffset > num)
		{
			throw new ArgumentOutOfRangeException("endOffset", endOffset, "Value must be between startOffset and " + num);
		}
		ISegment segment = new SimpleSegment(startOffset, endOffset - startOffset);
		List<HtmlElement> list = new List<HtmlElement>();
		for (int i = 0; i < Sections.Count; i++)
		{
			HighlightedSection highlightedSection = Sections[i];
			if (SimpleSegment.GetOverlap(highlightedSection, segment).Length > 0)
			{
				list.Add(new HtmlElement(highlightedSection.Offset, i, isEnd: false, highlightedSection.Color));
				list.Add(new HtmlElement(highlightedSection.Offset + highlightedSection.Length, i, isEnd: true, highlightedSection.Color));
			}
		}
		list.Sort();
		IDocument document = Document;
		int num2 = startOffset;
		foreach (HtmlElement item in list)
		{
			int num3 = Math.Min(item.Offset, endOffset);
			if (num3 > startOffset)
			{
				document.WriteTextTo(writer, num2, num3 - num2);
			}
			num2 = Math.Max(num2, num3);
			if (item.IsEnd)
			{
				writer.EndSpan();
			}
			else
			{
				writer.BeginSpan(item.Color);
			}
		}
		document.WriteTextTo(writer, num2, endOffset - num2);
	}

	public string ToHtml(HtmlOptions options = null)
	{
		StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
		using (HtmlRichTextWriter writer = new HtmlRichTextWriter(stringWriter, options))
		{
			WriteTo(writer);
		}
		return stringWriter.ToString();
	}

	public string ToHtml(int startOffset, int endOffset, HtmlOptions options = null)
	{
		StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
		using (HtmlRichTextWriter writer = new HtmlRichTextWriter(stringWriter, options))
		{
			WriteTo(writer, startOffset, endOffset);
		}
		return stringWriter.ToString();
	}

	public override string ToString()
	{
		return "[" + GetType().Name + " " + ToHtml() + "]";
	}

	[Obsolete("Use ToRichText() instead")]
	public HighlightedInlineBuilder ToInlineBuilder()
	{
		HighlightedInlineBuilder highlightedInlineBuilder = new HighlightedInlineBuilder(Document.GetText(DocumentLine));
		int offset = DocumentLine.Offset;
		foreach (HighlightedSection section in Sections)
		{
			highlightedInlineBuilder.SetHighlighting(section.Offset - offset, section.Length, section.Color);
		}
		return highlightedInlineBuilder;
	}

	public RichTextModel ToRichTextModel()
	{
		RichTextModel richTextModel = new RichTextModel();
		int offset = DocumentLine.Offset;
		foreach (HighlightedSection section in Sections)
		{
			richTextModel.ApplyHighlighting(section.Offset - offset, section.Length, section.Color);
		}
		return richTextModel;
	}

	public RichText ToRichText()
	{
		return new RichText(Document.GetText(DocumentLine), ToRichTextModel());
	}
}
