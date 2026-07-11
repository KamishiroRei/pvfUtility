using System;
using System.Windows;
using System.Windows.Media.TextFormatting;

namespace ICSharpCode.AvalonEdit.Rendering;

public class InlineObjectElement : VisualLineElement
{
	public UIElement Element { get; private set; }

	public InlineObjectElement(int documentLength, UIElement element)
		: base(1, documentLength)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		Element = element;
	}

	public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		return new InlineObjectRun(1, base.TextRunProperties, Element);
	}
}
