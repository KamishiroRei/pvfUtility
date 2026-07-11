using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Highlighting;

public class HighlightingColorizer : DocumentColorizingTransformer
{
	private readonly IHighlightingDefinition definition;

	private TextView textView;

	private IHighlighter highlighter;

	private bool isFixedHighlighter;

	private bool isInHighlightingGroup;

	private DocumentLine lastColorizedLine;

	private int lineNumberBeingColorized;

	public HighlightingColorizer(IHighlightingDefinition definition)
	{
		if (definition == null)
		{
			throw new ArgumentNullException("definition");
		}
		this.definition = definition;
	}

	public HighlightingColorizer(IHighlighter highlighter)
	{
		if (highlighter == null)
		{
			throw new ArgumentNullException("highlighter");
		}
		this.highlighter = highlighter;
		isFixedHighlighter = true;
	}

	protected HighlightingColorizer()
	{
	}

	private void textView_DocumentChanged(object sender, EventArgs e)
	{
		TextView textView = (TextView)sender;
		DeregisterServices(textView);
		RegisterServices(textView);
	}

	protected virtual void DeregisterServices(TextView textView)
	{
		if (highlighter == null)
		{
			return;
		}
		if (isInHighlightingGroup)
		{
			highlighter.EndHighlighting();
			isInHighlightingGroup = false;
		}
		highlighter.HighlightingStateChanged -= OnHighlightStateChanged;
		if (textView.Services.GetService(typeof(IHighlighter)) == highlighter)
		{
			textView.Services.RemoveService(typeof(IHighlighter));
		}
		if (!isFixedHighlighter)
		{
			if (highlighter != null)
			{
				highlighter.Dispose();
			}
			highlighter = null;
		}
	}

	protected virtual void RegisterServices(TextView textView)
	{
		if (textView.Document == null)
		{
			return;
		}
		if (!isFixedHighlighter)
		{
			highlighter = ((textView.Document != null) ? CreateHighlighter(textView, textView.Document) : null);
		}
		if (highlighter != null && highlighter.Document == textView.Document)
		{
			if (textView.Services.GetService(typeof(IHighlighter)) == null)
			{
				textView.Services.AddService(typeof(IHighlighter), highlighter);
			}
			highlighter.HighlightingStateChanged += OnHighlightStateChanged;
		}
	}

	protected virtual IHighlighter CreateHighlighter(TextView textView, TextDocument document)
	{
		if (definition != null)
		{
			return new DocumentHighlighter(document, definition);
		}
		throw new NotSupportedException("Cannot create a highlighter because no IHighlightingDefinition was specified, and the CreateHighlighter() method was not overridden.");
	}

	protected override void OnAddToTextView(TextView textView)
	{
		if (this.textView != null)
		{
			throw new InvalidOperationException("Cannot use a HighlightingColorizer instance in multiple text views. Please create a separate instance for each text view.");
		}
		base.OnAddToTextView(textView);
		this.textView = textView;
		textView.DocumentChanged += textView_DocumentChanged;
		textView.VisualLineConstructionStarting += textView_VisualLineConstructionStarting;
		textView.VisualLinesChanged += textView_VisualLinesChanged;
		RegisterServices(textView);
	}

	protected override void OnRemoveFromTextView(TextView textView)
	{
		DeregisterServices(textView);
		textView.DocumentChanged -= textView_DocumentChanged;
		textView.VisualLineConstructionStarting -= textView_VisualLineConstructionStarting;
		textView.VisualLinesChanged -= textView_VisualLinesChanged;
		base.OnRemoveFromTextView(textView);
		this.textView = null;
	}

	private void textView_VisualLineConstructionStarting(object sender, VisualLineConstructionStartEventArgs e)
	{
		if (highlighter != null)
		{
			lineNumberBeingColorized = e.FirstLineInView.LineNumber - 1;
			if (!isInHighlightingGroup)
			{
				highlighter.BeginHighlighting();
				isInHighlightingGroup = true;
			}
			highlighter.UpdateHighlightingState(lineNumberBeingColorized);
			lineNumberBeingColorized = 0;
		}
	}

	private void textView_VisualLinesChanged(object sender, EventArgs e)
	{
		if (highlighter != null && isInHighlightingGroup)
		{
			highlighter.EndHighlighting();
			isInHighlightingGroup = false;
		}
	}

	protected override void Colorize(ITextRunConstructionContext context)
	{
		lastColorizedLine = null;
		base.Colorize(context);
		if (lastColorizedLine != context.VisualLine.LastDocumentLine && highlighter != null)
		{
			lineNumberBeingColorized = context.VisualLine.LastDocumentLine.LineNumber;
			highlighter.UpdateHighlightingState(lineNumberBeingColorized);
			lineNumberBeingColorized = 0;
		}
		lastColorizedLine = null;
	}

	protected override void ColorizeLine(DocumentLine line)
	{
		if (highlighter != null)
		{
			lineNumberBeingColorized = line.LineNumber;
			HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumberBeingColorized);
			lineNumberBeingColorized = 0;
			foreach (HighlightedSection section in highlightedLine.Sections)
			{
				if (!IsEmptyColor(section.Color))
				{
					ChangeLinePart(section.Offset, section.Offset + section.Length, delegate(VisualLineElement visualLineElement)
					{
						ApplyColorToElement(visualLineElement, section.Color);
					});
				}
			}
		}
		lastColorizedLine = line;
	}

	internal static bool IsEmptyColor(HighlightingColor color)
	{
		if (color == null)
		{
			return true;
		}
		if (color.Background == null && color.Foreground == null && !color.FontStyle.HasValue && !color.FontWeight.HasValue && !color.Underline.HasValue)
		{
			return !color.Strikethrough.HasValue;
		}
		return false;
	}

	protected virtual void ApplyColorToElement(VisualLineElement element, HighlightingColor color)
	{
		ApplyColorToElement(element, color, base.CurrentContext);
	}

	internal static void ApplyColorToElement(VisualLineElement element, HighlightingColor color, ITextRunConstructionContext context)
	{
		if (color.Foreground != null)
		{
			Brush brush = color.Foreground.GetBrush(context);
			if (brush != null)
			{
				element.TextRunProperties.SetForegroundBrush(brush);
			}
		}
		if (color.Background != null)
		{
			Brush brush2 = color.Background.GetBrush(context);
			if (brush2 != null)
			{
				element.BackgroundBrush = brush2;
			}
		}
		if (color.FontStyle.HasValue || color.FontWeight.HasValue || color.FontFamily != null)
		{
			Typeface typeface = element.TextRunProperties.Typeface;
			element.TextRunProperties.SetTypeface(new Typeface(color.FontFamily ?? typeface.FontFamily, color.FontStyle ?? typeface.Style, color.FontWeight ?? typeface.Weight, typeface.Stretch));
		}
		if (color.Underline == true)
		{
			element.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
		}
		if (color.Strikethrough == true)
		{
			element.TextRunProperties.SetTextDecorations(TextDecorations.Strikethrough);
		}
		if (color.FontSize.HasValue)
		{
			element.TextRunProperties.SetFontRenderingEmSize(color.FontSize.Value);
		}
	}

	private void OnHighlightStateChanged(int fromLineNumber, int toLineNumber)
	{
		if (lineNumberBeingColorized == 0 || toLineNumber > lineNumberBeingColorized)
		{
			if (fromLineNumber == toLineNumber)
			{
				textView.Redraw(textView.Document.GetLineByNumber(fromLineNumber));
				return;
			}
			DocumentLine lineByNumber = textView.Document.GetLineByNumber(fromLineNumber);
			DocumentLine lineByNumber2 = textView.Document.GetLineByNumber(toLineNumber);
			int offset = lineByNumber.Offset;
			textView.Redraw(offset, lineByNumber2.EndOffset - offset);
		}
	}
}
