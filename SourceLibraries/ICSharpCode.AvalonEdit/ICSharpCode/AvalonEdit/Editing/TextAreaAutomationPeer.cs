using System;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Editing;

internal class TextAreaAutomationPeer : FrameworkElementAutomationPeer, IValueProvider, ITextProvider
{
	private TextArea TextArea => (TextArea)base.Owner;

	internal IRawElementProviderSimple Provider => ProviderFromPeer(this);

	public bool IsReadOnly => TextArea.ReadOnlySectionProvider == ReadOnlySectionDocument.Instance;

	public string Value => TextArea.Document.Text;

	public ITextRangeProvider DocumentRange => new TextRangeProvider(TextArea, TextArea.Document, 0, TextArea.Document.TextLength);

	public SupportedTextSelection SupportedTextSelection => SupportedTextSelection.Single;

	public TextAreaAutomationPeer(TextArea owner)
		: base(owner)
	{
		owner.Caret.PositionChanged += OnSelectionChanged;
		owner.SelectionChanged += OnSelectionChanged;
	}

	private void OnSelectionChanged(object sender, EventArgs e)
	{
		RaiseAutomationEvent(AutomationEvents.TextPatternOnTextSelectionChanged);
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Document;
	}

	public void SetValue(string value)
	{
		TextArea.Document.Text = value;
	}

	public ITextRangeProvider[] GetSelection()
	{
		if (TextArea.Selection.IsEmpty)
		{
			TextAnchor textAnchor = TextArea.Document.CreateAnchor(TextArea.Caret.Offset);
			textAnchor.SurviveDeletion = true;
			return new ITextRangeProvider[1]
			{
				new TextRangeProvider(TextArea, TextArea.Document, new AnchorSegment(textAnchor, textAnchor))
			};
		}
		return TextArea.Selection.Segments.Select((SelectionSegment s) => new TextRangeProvider(TextArea, TextArea.Document, s)).ToArray();
	}

	public ITextRangeProvider[] GetVisibleRanges()
	{
		throw new NotImplementedException();
	}

	public ITextRangeProvider RangeFromChild(IRawElementProviderSimple childElement)
	{
		throw new NotImplementedException();
	}

	public ITextRangeProvider RangeFromPoint(Point screenLocation)
	{
		throw new NotImplementedException();
	}

	public override object GetPattern(PatternInterface patternInterface)
	{
		switch (patternInterface)
		{
		case PatternInterface.Text:
			return this;
		case PatternInterface.Value:
			return this;
		case PatternInterface.Scroll:
			if (TextArea.GetService(typeof(TextEditor)) is TextEditor element)
			{
				AutomationPeer automationPeer = UIElementAutomationPeer.FromElement(element);
				if (automationPeer != null)
				{
					return automationPeer.GetPattern(patternInterface);
				}
			}
			break;
		}
		return base.GetPattern(patternInterface);
	}
}
