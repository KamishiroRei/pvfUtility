using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting;

internal class RichTextModelWriter : PlainRichTextWriter
{
	private readonly RichTextModel richTextModel;

	private readonly DocumentTextWriter documentTextWriter;

	private readonly Stack<HighlightingColor> colorStack = new Stack<HighlightingColor>();

	private HighlightingColor currentColor;

	private int currentColorBegin = -1;

	public int InsertionOffset
	{
		get
		{
			return documentTextWriter.InsertionOffset;
		}
		set
		{
			documentTextWriter.InsertionOffset = value;
		}
	}

	public RichTextModelWriter(RichTextModel richTextModel, IDocument document, int insertionOffset)
		: base(new DocumentTextWriter(document, insertionOffset))
	{
		if (richTextModel == null)
		{
			throw new ArgumentNullException("richTextModel");
		}
		this.richTextModel = richTextModel;
		documentTextWriter = (DocumentTextWriter)textWriter;
		currentColor = richTextModel.GetHighlightingAt(Math.Max(0, insertionOffset - 1));
	}

	protected override void BeginUnhandledSpan()
	{
		colorStack.Push(currentColor);
	}

	private void BeginColorSpan()
	{
		WriteIndentationIfNecessary();
		colorStack.Push(currentColor);
		currentColor = currentColor.Clone();
		currentColorBegin = documentTextWriter.InsertionOffset;
	}

	public override void EndSpan()
	{
		currentColor = colorStack.Pop();
		currentColorBegin = documentTextWriter.InsertionOffset;
	}

	protected override void AfterWrite()
	{
		base.AfterWrite();
		richTextModel.SetHighlighting(currentColorBegin, documentTextWriter.InsertionOffset - currentColorBegin, currentColor);
	}

	public override void BeginSpan(Color foregroundColor)
	{
		BeginColorSpan();
		currentColor.Foreground = new SimpleHighlightingBrush(foregroundColor);
		currentColor.Freeze();
	}

	public override void BeginSpan(FontFamily fontFamily)
	{
		BeginUnhandledSpan();
	}

	public override void BeginSpan(FontStyle fontStyle)
	{
		BeginColorSpan();
		currentColor.FontStyle = fontStyle;
		currentColor.Freeze();
	}

	public override void BeginSpan(FontWeight fontWeight)
	{
		BeginColorSpan();
		currentColor.FontWeight = fontWeight;
		currentColor.Freeze();
	}

	public override void BeginSpan(HighlightingColor highlightingColor)
	{
		BeginColorSpan();
		currentColor.MergeWith(highlightingColor);
		currentColor.Freeze();
	}
}
