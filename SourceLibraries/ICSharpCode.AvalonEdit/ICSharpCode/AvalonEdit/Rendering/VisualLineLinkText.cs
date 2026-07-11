using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;

namespace ICSharpCode.AvalonEdit.Rendering;

public class VisualLineLinkText : VisualLineText
{
	public Uri NavigateUri { get; set; }

	public string TargetName { get; set; }

	public bool RequireControlModifierForClick { get; set; }

	public VisualLineLinkText(VisualLine parentVisualLine, int length)
		: base(parentVisualLine, length)
	{
		RequireControlModifierForClick = true;
	}

	public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
	{
		base.TextRunProperties.SetForegroundBrush(context.TextView.LinkTextForegroundBrush);
		base.TextRunProperties.SetBackgroundBrush(context.TextView.LinkTextBackgroundBrush);
		if (context.TextView.LinkTextUnderline)
		{
			base.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
		}
		return base.CreateTextRun(startVisualColumn, context);
	}

	protected virtual bool LinkIsClickable()
	{
		if (NavigateUri == null)
		{
			return false;
		}
		if (RequireControlModifierForClick)
		{
			return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
		}
		return true;
	}

	public override void OnQueryCursor(QueryCursorEventArgs e)
	{
		if (LinkIsClickable())
		{
			e.Handled = true;
			e.Cursor = Cursors.Hand;
		}
	}

	public override void OnMouseDown(MouseButtonEventArgs e)
	{
		if (e.ChangedButton != MouseButton.Left || e.Handled || !LinkIsClickable())
		{
			return;
		}
		RequestNavigateEventArgs e2 = new RequestNavigateEventArgs(NavigateUri, TargetName);
		e2.RoutedEvent = Hyperlink.RequestNavigateEvent;
		if (e.Source is FrameworkElement frameworkElement)
		{
			frameworkElement.RaiseEvent(e2);
		}
		if (!e2.Handled)
		{
			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = NavigateUri.ToString(),
					UseShellExecute = true
				});
			}
			catch
			{
			}
		}
		e.Handled = true;
	}

	protected override VisualLineText CreateInstance(int length)
	{
		return new VisualLineLinkText(base.ParentVisualLine, length)
		{
			NavigateUri = NavigateUri,
			TargetName = TargetName,
			RequireControlModifierForClick = RequireControlModifierForClick
		};
	}
}
