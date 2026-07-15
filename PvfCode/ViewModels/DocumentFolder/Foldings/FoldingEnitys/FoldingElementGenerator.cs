using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

public sealed class FoldingElementGenerator : VisualLineElementGenerator, ITextViewConnect
{
	public sealed class FoldingLineElement : FormattedTextElement
	{
		internal readonly FoldingSection Section;

		internal Brush cbEbq3JOqa;

		public FoldingLineElement(FoldingSection fs, TextLine text, int documentLength)
			: base(text, documentLength)
		{
			Section = fs;
		}

		public FoldingLineElement(TextLine text, int documentLength)
			: base(text, documentLength)
		{
		}

		public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
		{
			return new TYbvDHbdEADREJdBaVx(this, base.TextRunProperties)
			{
				cdQbeaLs5w = cbEbq3JOqa
			};
		}

		public override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
			{
				Section.IsFolded = false;
				e.Handled = true;
			}
			else
			{
				base.OnMouseDown(e);
			}
		}

		public override void OnQueryCursor(QueryCursorEventArgs e)
		{
			e.Cursor = Cursors.Arrow;
			e.Handled = true;
		}
	}

	private sealed class TYbvDHbdEADREJdBaVx : FormattedTextRun
	{
		internal Brush cdQbeaLs5w;

		public TYbvDHbdEADREJdBaVx(FormattedTextElement P_0, TextRunProperties P_1)
			: base(P_0, P_1)
		{
		}

		public override void Draw(DrawingContext P_0, Point P_1, bool P_2, bool P_3)
		{
			TextEmbeddedObjectMetrics textEmbeddedObjectMetrics = Format(double.PositiveInfinity);
			Rect rectangle = default(Rect);
			rectangle = new Rect(P_1.X, P_1.Y - textEmbeddedObjectMetrics.Baseline, textEmbeddedObjectMetrics.Width, textEmbeddedObjectMetrics.Height);
			P_0.DrawRectangle(null, new Pen(cdQbeaLs5w, 1.0), rectangle);
			base.Draw(P_0, P_1, P_2, P_3);
		}
	}

	private readonly List<TextView> eqHYnZTJpC;

	private FoldingManager UctYqs4wyB;

	public static readonly Brush DefaultTextBrush;

	private static Brush y5kYdR2Npd;

	public FoldingManager FoldingManager
	{
		get
		{
			return UctYqs4wyB;
		}
		set
		{
			if (UctYqs4wyB == value)
			{
				return;
			}
			if (UctYqs4wyB != null)
			{
				foreach (TextView item in eqHYnZTJpC)
				{
					UctYqs4wyB.NHyYbHHCqJ(item);
				}
			}
			UctYqs4wyB = value;
			if (UctYqs4wyB == null)
			{
				return;
			}
			foreach (TextView item2 in eqHYnZTJpC)
			{
				UctYqs4wyB.vGQYtHRdp6(item2);
			}
		}
	}

	public static Brush TextBrush
	{
		get
		{
			return y5kYdR2Npd;
		}
		set
		{
			y5kYdR2Npd = value;
		}
	}

	void ITextViewConnect.AddToTextView(TextView textView)
	{
		eqHYnZTJpC.Add(textView);
		if (UctYqs4wyB != null)
		{
			UctYqs4wyB.vGQYtHRdp6(textView);
		}
	}

	void ITextViewConnect.RemoveFromTextView(TextView textView)
	{
		eqHYnZTJpC.Remove(textView);
		if (UctYqs4wyB != null)
		{
			UctYqs4wyB.NHyYbHHCqJ(textView);
		}
	}

	public override void StartGeneration(ITextRunConstructionContext context)
	{
		base.StartGeneration(context);
		if (UctYqs4wyB != null)
		{
			if (!UctYqs4wyB.mIhYKQMGji.Contains(context.TextView))
			{
				throw new ArgumentException("Invalid TextView");
			}
			if (context.Document != UctYqs4wyB.d6MYOknguE)
			{
				throw new ArgumentException("Invalid document");
			}
		}
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		if (UctYqs4wyB != null)
		{
			foreach (FoldingSection item in UctYqs4wyB.GetFoldingsContaining(startOffset))
			{
				if (item.IsFolded && item.EndOffset > startOffset)
				{
					return startOffset;
				}
			}
			return UctYqs4wyB.GetNextFoldedFoldingStart(startOffset);
		}
		return -1;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		if (UctYqs4wyB == null)
		{
			return null;
		}
		int num = -1;
		FoldingSection foldingSection = null;
		foreach (FoldingSection item in UctYqs4wyB.GetFoldingsContaining(offset))
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
				text = " . . . ";
			}
			VisualLineElementTextRunProperties visualLineElementTextRunProperties = new VisualLineElementTextRunProperties(base.CurrentContext.GlobalTextRunProperties);
			visualLineElementTextRunProperties.SetForegroundBrush(y5kYdR2Npd);
			TextLine text2 = FormattedTextElement.PrepareText(TextFormatterFactory.Create((DependencyObject)(object)base.CurrentContext.TextView), text, visualLineElementTextRunProperties);
			return new FoldingLineElement(foldingSection, text2, num - offset)
			{
				cbEbq3JOqa = y5kYdR2Npd
			};
		}
		return null;
	}

	public FoldingElementGenerator()
	{
		eqHYnZTJpC = new List<TextView>();
	}

	static FoldingElementGenerator()
	{
		DefaultTextBrush = Brushes.Gray;
		y5kYdR2Npd = DefaultTextBrush;
	}
}
