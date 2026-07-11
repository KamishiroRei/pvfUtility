using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Editing;
using PvfCode.Controls.TextEditorFolder;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class OverloadInsightWindow : InsightWindow
{
	private OverloadViewer L0NuajbU2h;

	public IOverloadProvider Provider
	{
		get
		{
			return L0NuajbU2h.Provider;
		}
		set
		{
			L0NuajbU2h.Provider = value;
		}
	}

	public OverloadInsightWindow(TextEditorBase editorBase, TextArea textArea, int startOffSet, int endOffSet)
		: base(editorBase, textArea, startOffSet, endOffSet)
	{
		L0NuajbU2h = new OverloadViewer();
		L0NuajbU2h.Margin = new Thickness(2.0, 0.0, 0.0, 0.0);
		base.Content = L0NuajbU2h;
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (e.Handled || Provider == null || Provider.Count <= 1)
		{
			return;
		}
		Key key = e.Key;
		if ((int)key != 24)
		{
			if ((int)key == 26)
			{
				e.Handled = true;
				L0NuajbU2h.ChangeIndex(1);
			}
		}
		else
		{
			e.Handled = true;
			L0NuajbU2h.ChangeIndex(-1);
		}
		if (e.Handled)
		{
			UpdateLayout();
			UpdatePosition();
		}
	}
}
