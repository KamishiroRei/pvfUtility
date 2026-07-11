using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class OverloadInsightWindow : InsightWindow
{
	private OverloadViewer overloadViewer = new OverloadViewer();

	public IOverloadProvider Provider
	{
		get
		{
			return overloadViewer.Provider;
		}
		set
		{
			overloadViewer.Provider = value;
		}
	}

	public OverloadInsightWindow(TextArea textArea)
		: base(textArea)
	{
		overloadViewer.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
		base.Content = overloadViewer;
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!e.Handled && Provider != null && Provider.Count > 1)
		{
			switch (e.Key)
			{
			case Key.Up:
				e.Handled = true;
				overloadViewer.ChangeIndex(-1);
				break;
			case Key.Down:
				e.Handled = true;
				overloadViewer.ChangeIndex(1);
				break;
			}
			if (e.Handled)
			{
				UpdateLayout();
				UpdatePosition();
			}
		}
	}
}
