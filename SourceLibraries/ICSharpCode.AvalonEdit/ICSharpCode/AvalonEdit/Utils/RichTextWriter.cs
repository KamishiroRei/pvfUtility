using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.AvalonEdit.Utils;

internal abstract class RichTextWriter : TextWriter
{
	protected abstract void BeginUnhandledSpan();

	public void Write(RichText richText)
	{
		Write(richText, 0, richText.Length);
	}

	public virtual void Write(RichText richText, int offset, int length)
	{
		foreach (HighlightedSection highlightedSection in richText.GetHighlightedSections(offset, length))
		{
			BeginSpan(highlightedSection.Color);
			((TextWriter)this).Write(richText.Text.Substring(highlightedSection.Offset, highlightedSection.Length));
			EndSpan();
		}
	}

	public virtual void BeginSpan(Color foregroundColor)
	{
		BeginUnhandledSpan();
	}

	public virtual void BeginSpan(FontWeight fontWeight)
	{
		BeginUnhandledSpan();
	}

	public virtual void BeginSpan(FontStyle fontStyle)
	{
		BeginUnhandledSpan();
	}

	public virtual void BeginSpan(FontFamily fontFamily)
	{
		BeginUnhandledSpan();
	}

	public virtual void BeginSpan(HighlightingColor highlightingColor)
	{
		BeginUnhandledSpan();
	}

	public virtual void BeginHyperlinkSpan(Uri uri)
	{
		BeginUnhandledSpan();
	}

	public abstract void EndSpan();

	public abstract void Indent();

	public abstract void Unindent();
}
