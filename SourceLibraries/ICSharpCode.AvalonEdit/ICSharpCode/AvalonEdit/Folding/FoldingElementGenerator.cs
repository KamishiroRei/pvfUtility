using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Folding;

public sealed class FoldingElementGenerator : VisualLineElementGenerator, ITextViewConnect
{
	private sealed class FoldingLineElement : FormattedTextElement
	{
		private readonly FoldingSection fs;

		internal Brush textBrush;

		public FoldingLineElement(FoldingSection fs, TextLine text, int documentLength)
			: base(text, documentLength)
		{
			this.fs = fs;
		}

		public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
		{
			return new FoldingLineTextRun(this, base.TextRunProperties)
			{
				textBrush = textBrush
			};
		}

		public override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
			{
				fs.IsFolded = false;
				e.Handled = true;
			}
			else
			{
				base.OnMouseDown(e);
			}
		}
	}

	private sealed class FoldingLineTextRun : FormattedTextRun
	{
		internal Brush textBrush;

		public FoldingLineTextRun(FormattedTextElement element, TextRunProperties properties)
			: base(element, properties)
		{
		}

		public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
		{
			TextEmbeddedObjectMetrics textEmbeddedObjectMetrics = Format(double.PositiveInfinity);
			drawingContext.DrawRectangle(rectangle: new Rect(origin.X, origin.Y - textEmbeddedObjectMetrics.Baseline, textEmbeddedObjectMetrics.Width, textEmbeddedObjectMetrics.Height), brush: null, pen: new Pen(textBrush, 1.0));
			base.Draw(drawingContext, origin, rightToLeft, sideways);
		}
	}

	private readonly List<TextView> textViews = new List<TextView>();

	private FoldingManager foldingManager;

	public static readonly Brush DefaultTextBrush = Brushes.Gray;

	private static Brush textBrush = DefaultTextBrush;

	public FoldingManager FoldingManager
	{
		get
		{
			return foldingManager;
		}
		set
		{
			if (foldingManager == value)
			{
				return;
			}
			if (foldingManager != null)
			{
				foreach (TextView textView in textViews)
				{
					foldingManager.RemoveFromTextView(textView);
				}
			}
			foldingManager = value;
			if (foldingManager == null)
			{
				return;
			}
			foreach (TextView textView2 in textViews)
			{
				foldingManager.AddToTextView(textView2);
			}
		}
	}

	public static Brush TextBrush
	{
		get
		{
			return textBrush;
		}
		set
		{
			textBrush = value;
		}
	}

	void ITextViewConnect.AddToTextView(TextView textView)
	{
		textViews.Add(textView);
		if (foldingManager != null)
		{
			foldingManager.AddToTextView(textView);
		}
	}

	void ITextViewConnect.RemoveFromTextView(TextView textView)
	{
		textViews.Remove(textView);
		if (foldingManager != null)
		{
			foldingManager.RemoveFromTextView(textView);
		}
	}

	public override void StartGeneration(ITextRunConstructionContext context)
	{
		base.StartGeneration(context);
		if (foldingManager != null)
		{
			if (!foldingManager.textViews.Contains(context.TextView))
			{
				throw new ArgumentException("Invalid TextView");
			}
			if (context.Document != foldingManager.document)
			{
				throw new ArgumentException("Invalid document");
			}
		}
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		if (foldingManager != null)
		{
			foreach (FoldingSection item in foldingManager.GetFoldingsContaining(startOffset))
			{
				if (item.IsFolded)
				{
					_ = item.EndOffset;
				}
			}
			return foldingManager.GetNextFoldedFoldingStart(startOffset);
		}
		return -1;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		if (foldingManager == null)
		{
			return null;
		}
		int num = -1;
		FoldingSection foldingSection = null;
		foreach (FoldingSection item in foldingManager.GetFoldingsContaining(offset))
		{
			if (item.IsFolded && item.EndOffset > num)
			{
				num = item.EndOffset;
				foldingSection = item;
			}
		}
		if (num > offset && foldingSection != null)
		{
			bool flag;
			do
			{
				flag = false;
				foreach (FoldingSection item2 in FoldingManager.GetFoldingsContaining(num))
				{
					if (item2.IsFolded && item2.EndOffset > num)
					{
						num = item2.EndOffset;
						flag = true;
					}
				}
			}
			while (flag);
			string text = foldingSection.Title;
			if (string.IsNullOrEmpty(text))
			{
				text = "...";
			}
			VisualLineElementTextRunProperties visualLineElementTextRunProperties = new VisualLineElementTextRunProperties(base.CurrentContext.GlobalTextRunProperties);
			visualLineElementTextRunProperties.SetForegroundBrush(textBrush);
			TextLine text2 = FormattedTextElement.PrepareText(TextFormatterFactory.Create(base.CurrentContext.TextView), text, visualLineElementTextRunProperties);
			return new FoldingLineElement(foldingSection, text2, num - offset)
			{
				textBrush = textBrush
			};
		}
		return null;
	}
}
