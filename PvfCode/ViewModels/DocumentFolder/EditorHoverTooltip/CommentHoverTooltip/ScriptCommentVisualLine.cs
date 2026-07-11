using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip.CommentHoverTooltip;

public class ScriptCommentVisualLine : VisualLineText
{
	public bool MouseIsOver;

	public ScriptCommentVisualLine(VisualLine parentVisualLine, int length)
		: base(parentVisualLine, length)
	{
	}

	public override void OnQueryCursor(QueryCursorEventArgs e)
	{
		try
		{
			if (((int)Keyboard.Modifiers & 2) == 2)
			{
				e.Cursor = Cursors.Hand;
			}
			else
			{
				e.Cursor = Cursors.IBeam;
			}
			base.TextRunProperties.SetTextDecorations(TextDecorations.OverLine);
			e.Handled = true;
		}
		catch (Exception e2)
		{
			AppCore.Logger.ErrorUploadDialog(e2, "VisualLineReferenceText2.OnQueryCursor");
		}
	}

	public void Redraw(TextView view)
	{
		TextSegment segment = GetSegment();
		view.Redraw(segment.StartOffset, segment.Length, (DispatcherPriority)5);
	}

	public override void OnPreviewUp(MouseButtonEventArgs e)
	{
		base.TextRunProperties.SetTextDecorations(TextDecorations.OverLine);
	}

	public override void OnMouseHoverStopped(MouseEventArgs e)
	{
		base.TextRunProperties.SetTextDecorations(TextDecorations.OverLine);
		e.Handled = true;
	}

	private string GAUiH18NkP()
	{
		return base.ParentVisualLine.Document.GetText(GetSegment());
	}
}
