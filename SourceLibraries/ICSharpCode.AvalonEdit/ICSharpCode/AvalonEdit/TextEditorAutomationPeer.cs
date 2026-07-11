using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit;

public class TextEditorAutomationPeer : FrameworkElementAutomationPeer, IValueProvider
{
	private TextEditor TextEditor => (TextEditor)base.Owner;

	string IValueProvider.Value => TextEditor.Text;

	bool IValueProvider.IsReadOnly => TextEditor.IsReadOnly;

	public TextEditorAutomationPeer(TextEditor owner)
		: base(owner)
	{
	}

	void IValueProvider.SetValue(string value)
	{
		TextEditor.Text = value;
	}

	protected override AutomationControlType GetAutomationControlTypeCore()
	{
		return AutomationControlType.Document;
	}

	public override object GetPattern(PatternInterface patternInterface)
	{
		switch (patternInterface)
		{
		case PatternInterface.Value:
			return this;
		case PatternInterface.Scroll:
		{
			ScrollViewer scrollViewer = TextEditor.ScrollViewer;
			if (scrollViewer != null)
			{
				return UIElementAutomationPeer.FromElement(scrollViewer);
			}
			break;
		}
		}
		if (patternInterface == PatternInterface.Text)
		{
			return UIElementAutomationPeer.FromElement(TextEditor.TextArea);
		}
		return base.GetPattern(patternInterface);
	}

	internal void RaiseIsReadOnlyChanged(bool oldValue, bool newValue)
	{
		RaisePropertyChangedEvent(ValuePatternIdentifiers.IsReadOnlyProperty, Boxes.Box(oldValue), Boxes.Box(newValue));
	}
}
