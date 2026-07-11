using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering;

internal sealed class SingleCharacterElementGenerator : VisualLineElementGenerator, IBuiltinElementGenerator
{
	private sealed class SpaceTextElement : FormattedTextElement
	{
		public SpaceTextElement(TextLine textLine)
			: base(textLine, 1)
		{
			base.BreakBefore = LineBreakCondition.BreakPossible;
			base.BreakAfter = LineBreakCondition.BreakDesired;
		}

		public override int GetNextCaretPosition(int visualColumn, LogicalDirection direction, CaretPositioningMode mode)
		{
			if (mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint)
			{
				return base.GetNextCaretPosition(visualColumn, direction, mode);
			}
			return -1;
		}

		public override bool IsWhitespace(int visualColumn)
		{
			return true;
		}
	}

	private sealed class TabTextElement : VisualLineElement
	{
		internal readonly TextLine text;

		public TabTextElement(TextLine text)
			: base(2, 1)
		{
			this.text = text;
		}

		public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
		{
			if (startVisualColumn == base.VisualColumn)
			{
				return new TabGlyphRun(this, base.TextRunProperties);
			}
			if (startVisualColumn == base.VisualColumn + 1)
			{
				return new TextCharacters("\t", 0, 1, base.TextRunProperties);
			}
			throw new ArgumentOutOfRangeException("startVisualColumn");
		}

		public override int GetNextCaretPosition(int visualColumn, LogicalDirection direction, CaretPositioningMode mode)
		{
			if (mode == CaretPositioningMode.Normal || mode == CaretPositioningMode.EveryCodepoint)
			{
				return base.GetNextCaretPosition(visualColumn, direction, mode);
			}
			return -1;
		}

		public override bool IsWhitespace(int visualColumn)
		{
			return true;
		}
	}

	private sealed class TabGlyphRun : TextEmbeddedObject
	{
		private readonly TabTextElement element;

		private TextRunProperties properties;

		public override LineBreakCondition BreakBefore => LineBreakCondition.BreakPossible;

		public override LineBreakCondition BreakAfter => LineBreakCondition.BreakRestrained;

		public override bool HasFixedSize => true;

		public override CharacterBufferReference CharacterBufferReference => default(CharacterBufferReference);

		public override int Length => 1;

		public override TextRunProperties Properties => properties;

		public TabGlyphRun(TabTextElement element, TextRunProperties properties)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			this.properties = properties;
			this.element = element;
		}

		public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
		{
			return new TextEmbeddedObjectMetrics(Math.Min(0.0, element.text.WidthIncludingTrailingWhitespace - 1.0), element.text.Height, element.text.Baseline);
		}

		public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
		{
			double width = Math.Min(0.0, element.text.WidthIncludingTrailingWhitespace - 1.0);
			return new Rect(0.0, 0.0, width, element.text.Height);
		}

		public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
		{
			origin.Y -= element.text.Baseline;
			element.text.Draw(drawingContext, origin, InvertAxes.None);
		}
	}

	private sealed class SpecialCharacterBoxElement : FormattedTextElement
	{
		public SpecialCharacterBoxElement(TextLine text)
			: base(text, 1)
		{
		}

		public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
		{
			return new SpecialCharacterTextRun(this, base.TextRunProperties);
		}
	}

	private sealed class SpecialCharacterTextRun : FormattedTextRun
	{
		private static readonly SolidColorBrush darkGrayBrush;

		static SpecialCharacterTextRun()
		{
			darkGrayBrush = new SolidColorBrush(Color.FromArgb(200, 128, 128, 128));
			darkGrayBrush.Freeze();
		}

		public SpecialCharacterTextRun(FormattedTextElement element, TextRunProperties properties)
			: base(element, properties)
		{
		}

		public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
		{
			Point origin2 = new Point(origin.X + 1.5, origin.Y);
			TextEmbeddedObjectMetrics textEmbeddedObjectMetrics = base.Format(double.PositiveInfinity);
			drawingContext.DrawRoundedRectangle(rectangle: new Rect(origin2.X - 0.5, origin2.Y - textEmbeddedObjectMetrics.Baseline, textEmbeddedObjectMetrics.Width + 2.0, textEmbeddedObjectMetrics.Height), brush: darkGrayBrush, pen: null, radiusX: 2.5, radiusY: 2.5);
			base.Draw(drawingContext, origin2, rightToLeft, sideways);
		}

		public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
		{
			TextEmbeddedObjectMetrics textEmbeddedObjectMetrics = base.Format(remainingParagraphWidth);
			return new TextEmbeddedObjectMetrics(textEmbeddedObjectMetrics.Width + 3.0, textEmbeddedObjectMetrics.Height, textEmbeddedObjectMetrics.Baseline);
		}

		public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
		{
			Rect result = base.ComputeBoundingBox(rightToLeft, sideways);
			result.Width += 3.0;
			return result;
		}
	}

	public bool ShowSpaces { get; set; }

	public bool ShowTabs { get; set; }

	public bool ShowBoxForControlCharacters { get; set; }

	public SingleCharacterElementGenerator()
	{
		ShowSpaces = true;
		ShowTabs = true;
		ShowBoxForControlCharacters = true;
	}

	void IBuiltinElementGenerator.FetchOptions(TextEditorOptions options)
	{
		ShowSpaces = options.ShowSpaces;
		ShowTabs = options.ShowTabs;
		ShowBoxForControlCharacters = options.ShowBoxForControlCharacters;
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		DocumentLine lastDocumentLine = base.CurrentContext.VisualLine.LastDocumentLine;
		StringSegment text = base.CurrentContext.GetText(startOffset, lastDocumentLine.EndOffset - startOffset);
		for (int i = 0; i < text.Count; i++)
		{
			char c = text.Text[text.Offset + i];
			switch (c)
			{
			case ' ':
				if (ShowSpaces)
				{
					return startOffset + i;
				}
				break;
			case '\t':
				if (ShowTabs)
				{
					return startOffset + i;
				}
				break;
			default:
				if (ShowBoxForControlCharacters && char.IsControl(c))
				{
					return startOffset + i;
				}
				break;
			}
		}
		return -1;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		char charAt = base.CurrentContext.Document.GetCharAt(offset);
		if (ShowSpaces && charAt == ' ')
		{
			return new SpaceTextElement(base.CurrentContext.TextView.cachedElements.GetTextForNonPrintableCharacter("•", base.CurrentContext));
		}
		if (ShowTabs && charAt == '\t')
		{
			return new FormattedTextElement(base.CurrentContext.TextView.cachedElements.GetTextForNonPrintableCharacter(" ⟶ ", base.CurrentContext), 1);
		}
		if (ShowBoxForControlCharacters && char.IsControl(charAt))
		{
			VisualLineElementTextRunProperties visualLineElementTextRunProperties = new VisualLineElementTextRunProperties(base.CurrentContext.GlobalTextRunProperties);
			visualLineElementTextRunProperties.SetForegroundBrush(Brushes.White);
			return new SpecialCharacterBoxElement(FormattedTextElement.PrepareText(TextFormatterFactory.Create(base.CurrentContext.TextView), TextUtilities.GetControlCharacterName(charAt), visualLineElementTextRunProperties));
		}
		return null;
	}
}
