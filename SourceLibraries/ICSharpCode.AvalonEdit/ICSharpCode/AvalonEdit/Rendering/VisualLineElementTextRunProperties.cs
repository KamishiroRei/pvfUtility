using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering;

public class VisualLineElementTextRunProperties : TextRunProperties, ICloneable
{
	private Brush backgroundBrush;

	private BaselineAlignment baselineAlignment;

	private CultureInfo cultureInfo;

	private double fontHintingEmSize;

	private double fontRenderingEmSize;

	private Brush foregroundBrush;

	private Typeface typeface;

	private TextDecorationCollection textDecorations;

	private TextEffectCollection textEffects;

	private TextRunTypographyProperties typographyProperties;

	private NumberSubstitution numberSubstitution;

	public override Brush BackgroundBrush => backgroundBrush;

	public override BaselineAlignment BaselineAlignment => baselineAlignment;

	public override CultureInfo CultureInfo => cultureInfo;

	public override double FontHintingEmSize => fontHintingEmSize;

	public override double FontRenderingEmSize => fontRenderingEmSize;

	public override Brush ForegroundBrush => foregroundBrush;

	public override Typeface Typeface => typeface;

	public override TextDecorationCollection TextDecorations => textDecorations;

	public override TextEffectCollection TextEffects => textEffects;

	public override TextRunTypographyProperties TypographyProperties => typographyProperties;

	public override NumberSubstitution NumberSubstitution => numberSubstitution;

	public VisualLineElementTextRunProperties(TextRunProperties textRunProperties)
	{
		if (textRunProperties == null)
		{
			throw new ArgumentNullException("textRunProperties");
		}
		backgroundBrush = textRunProperties.BackgroundBrush;
		baselineAlignment = textRunProperties.BaselineAlignment;
		cultureInfo = textRunProperties.CultureInfo;
		fontHintingEmSize = textRunProperties.FontHintingEmSize;
		fontRenderingEmSize = textRunProperties.FontRenderingEmSize;
		foregroundBrush = textRunProperties.ForegroundBrush;
		typeface = textRunProperties.Typeface;
		textDecorations = textRunProperties.TextDecorations;
		if (textDecorations != null && !textDecorations.IsFrozen)
		{
			textDecorations = textDecorations.Clone();
		}
		textEffects = textRunProperties.TextEffects;
		if (textEffects != null && !textEffects.IsFrozen)
		{
			textEffects = textEffects.Clone();
		}
		typographyProperties = textRunProperties.TypographyProperties;
		numberSubstitution = textRunProperties.NumberSubstitution;
	}

	public virtual VisualLineElementTextRunProperties Clone()
	{
		return new VisualLineElementTextRunProperties(this);
	}

	object ICloneable.Clone()
	{
		return Clone();
	}

	public void SetBackgroundBrush(Brush value)
	{
		backgroundBrush = value;
	}

	public void SetBaselineAlignment(BaselineAlignment value)
	{
		baselineAlignment = value;
	}

	public void SetCultureInfo(CultureInfo value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		cultureInfo = value;
	}

	public void SetFontHintingEmSize(double value)
	{
		fontHintingEmSize = value;
	}

	public void SetFontRenderingEmSize(double value)
	{
		fontRenderingEmSize = value;
	}

	public void SetForegroundBrush(Brush value)
	{
		foregroundBrush = value;
	}

	public void SetTypeface(Typeface value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		typeface = value;
	}

	public void SetTextDecorations(TextDecorationCollection value)
	{
		if (textDecorations == null)
		{
			textDecorations = value;
		}
		else
		{
			textDecorations = new TextDecorationCollection(textDecorations.Union(value));
		}
	}

	public void SetTextEffects(TextEffectCollection value)
	{
		textEffects = value;
	}

	public void SetTypographyProperties(TextRunTypographyProperties value)
	{
		typographyProperties = value;
	}

	public void SetNumberSubstitution(NumberSubstitution value)
	{
		numberSubstitution = value;
	}
}
