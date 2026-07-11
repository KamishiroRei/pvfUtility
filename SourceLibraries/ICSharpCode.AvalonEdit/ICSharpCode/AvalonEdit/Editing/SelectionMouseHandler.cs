using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

internal sealed class SelectionMouseHandler : ITextAreaInputHandler
{
	private readonly TextArea textArea;

	private MouseSelectionMode mode;

	private AnchorSegment startWord;

	private Point possibleDragStartMousePos;

	private bool enableTextDragDrop;

	private object currentDragDescriptor;

	TextArea ITextAreaInputHandler.TextArea => textArea;

	public MouseSelectionMode MouseSelectionMode
	{
		get
		{
			return mode;
		}
		set
		{
			if (mode == value)
			{
				return;
			}
			if (value == MouseSelectionMode.None)
			{
				mode = MouseSelectionMode.None;
				textArea.ReleaseMouseCapture();
			}
			else if (textArea.CaptureMouse())
			{
				if (value != MouseSelectionMode.Normal && value != MouseSelectionMode.Rectangular)
				{
					throw new NotImplementedException("Programmatically starting mouse selection is only supported for normal and rectangular selections.");
				}
				mode = value;
			}
		}
	}

	internal SelectionMouseHandler(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		this.textArea = textArea;
	}

	static SelectionMouseHandler()
	{
		EventManager.RegisterClassHandler(typeof(TextArea), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
	}

	private static void OnLostMouseCapture(object sender, MouseEventArgs e)
	{
		TextArea textArea = (TextArea)sender;
		if (Mouse.Captured != textArea && textArea.DefaultInputHandler.MouseSelection is SelectionMouseHandler selectionMouseHandler)
		{
			selectionMouseHandler.mode = MouseSelectionMode.None;
		}
	}

	void ITextAreaInputHandler.Attach()
	{
		textArea.MouseLeftButtonDown += textArea_MouseLeftButtonDown;
		textArea.MouseMove += textArea_MouseMove;
		textArea.MouseLeftButtonUp += textArea_MouseLeftButtonUp;
		textArea.QueryCursor += textArea_QueryCursor;
		textArea.DocumentChanged += textArea_DocumentChanged;
		textArea.OptionChanged += textArea_OptionChanged;
		enableTextDragDrop = textArea.Options.EnableTextDragDrop;
		if (enableTextDragDrop)
		{
			AttachDragDrop();
		}
	}

	void ITextAreaInputHandler.Detach()
	{
		mode = MouseSelectionMode.None;
		textArea.MouseLeftButtonDown -= textArea_MouseLeftButtonDown;
		textArea.MouseMove -= textArea_MouseMove;
		textArea.MouseLeftButtonUp -= textArea_MouseLeftButtonUp;
		textArea.QueryCursor -= textArea_QueryCursor;
		textArea.DocumentChanged -= textArea_DocumentChanged;
		textArea.OptionChanged -= textArea_OptionChanged;
		if (enableTextDragDrop)
		{
			DetachDragDrop();
		}
	}

	private void AttachDragDrop()
	{
		textArea.AllowDrop = true;
		textArea.GiveFeedback += textArea_GiveFeedback;
		textArea.QueryContinueDrag += textArea_QueryContinueDrag;
		textArea.DragEnter += textArea_DragEnter;
		textArea.DragOver += textArea_DragOver;
		textArea.DragLeave += textArea_DragLeave;
		textArea.Drop += textArea_Drop;
	}

	private void DetachDragDrop()
	{
		textArea.AllowDrop = false;
		textArea.GiveFeedback -= textArea_GiveFeedback;
		textArea.QueryContinueDrag -= textArea_QueryContinueDrag;
		textArea.DragEnter -= textArea_DragEnter;
		textArea.DragOver -= textArea_DragOver;
		textArea.DragLeave -= textArea_DragLeave;
		textArea.Drop -= textArea_Drop;
	}

	private void textArea_OptionChanged(object sender, PropertyChangedEventArgs e)
	{
		bool flag = textArea.Options.EnableTextDragDrop;
		if (flag != enableTextDragDrop)
		{
			enableTextDragDrop = flag;
			if (flag)
			{
				AttachDragDrop();
			}
			else
			{
				DetachDragDrop();
			}
		}
	}

	private void textArea_DocumentChanged(object sender, EventArgs e)
	{
		if (mode != MouseSelectionMode.None)
		{
			mode = MouseSelectionMode.None;
			textArea.ReleaseMouseCapture();
		}
		startWord = null;
	}

	private void textArea_DragEnter(object sender, DragEventArgs e)
	{
		try
		{
			e.Effects = GetEffect(e);
			textArea.Caret.Show();
		}
		catch (Exception ex)
		{
			OnDragException(ex);
		}
	}

	private void textArea_DragOver(object sender, DragEventArgs e)
	{
		try
		{
			e.Effects = GetEffect(e);
		}
		catch (Exception ex)
		{
			OnDragException(ex);
		}
	}

	private DragDropEffects GetEffect(DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.UnicodeText, autoConvert: true))
		{
			e.Handled = true;
			int visualColumn;
			bool isAtEndOfLine;
			int offsetFromMousePosition = GetOffsetFromMousePosition(e.GetPosition(textArea.TextView), out visualColumn, out isAtEndOfLine);
			if (offsetFromMousePosition >= 0)
			{
				textArea.Caret.Position = new TextViewPosition(textArea.Document.GetLocation(offsetFromMousePosition), visualColumn)
				{
					IsAtEndOfLine = isAtEndOfLine
				};
				textArea.Caret.DesiredXPos = double.NaN;
				if (textArea.ReadOnlySectionProvider.CanInsert(offsetFromMousePosition))
				{
					if ((e.AllowedEffects & DragDropEffects.Move) == DragDropEffects.Move && (e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.ControlKey)
					{
						return DragDropEffects.Move;
					}
					return e.AllowedEffects & DragDropEffects.Copy;
				}
			}
		}
		return DragDropEffects.None;
	}

	private void textArea_DragLeave(object sender, DragEventArgs e)
	{
		try
		{
			e.Handled = true;
			if (!textArea.IsKeyboardFocusWithin)
			{
				textArea.Caret.Hide();
			}
		}
		catch (Exception ex)
		{
			OnDragException(ex);
		}
	}

	private void textArea_Drop(object sender, DragEventArgs e)
	{
		try
		{
			if ((e.Effects = GetEffect(e)) == DragDropEffects.None)
			{
				return;
			}
			int offset = textArea.Caret.Offset;
			if (mode == MouseSelectionMode.Drag && textArea.Selection.Contains(offset))
			{
				e.Effects = DragDropEffects.None;
			}
			else
			{
				DataObjectPastingEventArgs e2 = new DataObjectPastingEventArgs(e.Data, isDragDrop: true, DataFormats.UnicodeText);
				textArea.RaiseEvent(e2);
				if (e2.CommandCancelled)
				{
					return;
				}
				string textToPaste = EditingCommandHandler.GetTextToPaste(e2, textArea);
				if (textToPaste == null)
				{
					return;
				}
				bool dataPresent = e2.DataObject.GetDataPresent("AvalonEditRectangularSelection");
				textArea.Document.UndoStack.StartUndoGroup(currentDragDescriptor);
				try
				{
					if (!dataPresent || !RectangleSelection.PerformRectangularPaste(textArea, textArea.Caret.Position, textToPaste, selectInsertedText: true))
					{
						textArea.Document.Insert(offset, textToPaste);
						textArea.Selection = Selection.Create(textArea, offset, offset + textToPaste.Length);
					}
				}
				finally
				{
					textArea.Document.UndoStack.EndUndoGroup();
				}
			}
			e.Handled = true;
		}
		catch (Exception ex)
		{
			OnDragException(ex);
		}
	}

	private void OnDragException(Exception ex)
	{
		textArea.Dispatcher.BeginInvoke(DispatcherPriority.Send, (Action)delegate
		{
			throw new DragDropException("Exception during drag'n'drop", ex);
		});
	}

	private void textArea_GiveFeedback(object sender, GiveFeedbackEventArgs e)
	{
		try
		{
			e.UseDefaultCursors = true;
			e.Handled = true;
		}
		catch (Exception ex)
		{
			OnDragException(ex);
		}
	}

	private void textArea_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
	{
		try
		{
			if (e.EscapePressed)
			{
				e.Action = DragAction.Cancel;
			}
			else if ((e.KeyStates & DragDropKeyStates.LeftMouseButton) != DragDropKeyStates.LeftMouseButton)
			{
				e.Action = DragAction.Drop;
			}
			else
			{
				e.Action = DragAction.Continue;
			}
			e.Handled = true;
		}
		catch (Exception ex)
		{
			OnDragException(ex);
		}
	}

	private void StartDrag()
	{
		textArea.ReleaseMouseCapture();
		mode = MouseSelectionMode.Drag;
		DataObject dataObject = textArea.Selection.CreateDataObject(textArea);
		DragDropEffects dragDropEffects = DragDropEffects.All;
		List<AnchorSegment> list = textArea.Selection.Segments.Select((SelectionSegment s) => new AnchorSegment(textArea.Document, s)).ToList();
		foreach (AnchorSegment item in list)
		{
			ISegment[] deletableSegments = textArea.GetDeletableSegments(item);
			if (deletableSegments.Length != 1 || deletableSegments[0].Offset != ((ISegment)item).Offset || deletableSegments[0].EndOffset != ((ISegment)item).EndOffset)
			{
				dragDropEffects &= ~DragDropEffects.Move;
			}
		}
		DataObjectCopyingEventArgs e = new DataObjectCopyingEventArgs(dataObject, isDragDrop: true);
		textArea.RaiseEvent(e);
		if (e.CommandCancelled)
		{
			return;
		}
		object obj = (currentDragDescriptor = new object());
		DragDropEffects dragDropEffects2;
		using (textArea.AllowCaretOutsideSelection())
		{
			TextViewPosition position = textArea.Caret.Position;
			try
			{
				dragDropEffects2 = DragDrop.DoDragDrop(textArea, dataObject, dragDropEffects);
			}
			catch (COMException)
			{
				return;
			}
			if (dragDropEffects2 == DragDropEffects.None)
			{
				textArea.Caret.Position = position;
			}
		}
		currentDragDescriptor = null;
		if (list == null || dragDropEffects2 != DragDropEffects.Move || (dragDropEffects & DragDropEffects.Move) != DragDropEffects.Move)
		{
			return;
		}
		bool flag = obj == textArea.Document.UndoStack.LastGroupDescriptor;
		if (flag)
		{
			textArea.Document.UndoStack.StartContinuedUndoGroup();
		}
		textArea.Document.BeginUpdate();
		try
		{
			foreach (AnchorSegment item2 in list)
			{
				textArea.Document.Remove(((ISegment)item2).Offset, ((ISegment)item2).Length);
			}
		}
		finally
		{
			textArea.Document.EndUpdate();
			if (flag)
			{
				textArea.Document.UndoStack.EndUndoGroup();
			}
		}
	}

	private void textArea_QueryCursor(object sender, QueryCursorEventArgs e)
	{
		if (e.Handled)
		{
			return;
		}
		if (mode != MouseSelectionMode.None)
		{
			e.Cursor = Cursors.IBeam;
			e.Handled = true;
		}
		else
		{
			if (!textArea.TextView.VisualLinesValid)
			{
				return;
			}
			Point position = e.GetPosition(textArea.TextView);
			if (position.X >= 0.0 && position.Y >= 0.0 && position.X <= textArea.TextView.ActualWidth && position.Y <= textArea.TextView.ActualHeight)
			{
				int visualColumn;
				bool isAtEndOfLine;
				int offsetFromMousePosition = GetOffsetFromMousePosition(e, out visualColumn, out isAtEndOfLine);
				if (enableTextDragDrop && textArea.Selection.Contains(offsetFromMousePosition))
				{
					e.Cursor = Cursors.Arrow;
				}
				else
				{
					e.Cursor = Cursors.IBeam;
				}
				e.Handled = true;
			}
		}
	}

	private void textArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		mode = MouseSelectionMode.None;
		if (textArea.Document == null)
		{
			return;
		}
		if (!e.Handled && e.ChangedButton == MouseButton.Left)
		{
			ModifierKeys modifiers = Keyboard.Modifiers;
			bool flag = (modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
			if (enableTextDragDrop && e.ClickCount == 1 && !flag)
			{
				int visualColumn;
				bool isAtEndOfLine;
				int offsetFromMousePosition = GetOffsetFromMousePosition(e, out visualColumn, out isAtEndOfLine);
				if (textArea.Selection.Contains(offsetFromMousePosition))
				{
					if (textArea.CaptureMouse())
					{
						mode = MouseSelectionMode.PossibleDragStart;
						possibleDragStartMousePos = e.GetPosition(textArea);
					}
					e.Handled = true;
					return;
				}
			}
			TextViewPosition position = textArea.Caret.Position;
			SetCaretOffsetToMousePosition(e);
			if (!flag)
			{
				textArea.ClearSelection();
			}
			if (textArea.CaptureMouse())
			{
				if ((modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && textArea.Options.EnableRectangularSelection)
				{
					mode = MouseSelectionMode.Rectangular;
					if (flag && textArea.Selection is RectangleSelection)
					{
						textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(position, textArea.Caret.Position);
					}
				}
				else if (e.ClickCount == 1 && (modifiers & ModifierKeys.Control) == 0)
				{
					mode = MouseSelectionMode.Normal;
					if (flag && !(textArea.Selection is RectangleSelection))
					{
						textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(position, textArea.Caret.Position);
					}
				}
				else
				{
					SimpleSegment simpleSegment;
					if (e.ClickCount == 3)
					{
						mode = MouseSelectionMode.WholeLine;
						simpleSegment = GetLineAtMousePosition(e);
					}
					else
					{
						mode = MouseSelectionMode.WholeWord;
						simpleSegment = GetWordAtMousePosition(e);
					}
					if (simpleSegment == SimpleSegment.Invalid)
					{
						mode = MouseSelectionMode.None;
						textArea.ReleaseMouseCapture();
						return;
					}
					if (flag && !textArea.Selection.IsEmpty)
					{
						if (simpleSegment.Offset < textArea.Selection.SurroundingSegment.Offset)
						{
							textArea.Selection = textArea.Selection.SetEndpoint(new TextViewPosition(textArea.Document.GetLocation(simpleSegment.Offset)));
						}
						else if (simpleSegment.EndOffset > textArea.Selection.SurroundingSegment.EndOffset)
						{
							textArea.Selection = textArea.Selection.SetEndpoint(new TextViewPosition(textArea.Document.GetLocation(simpleSegment.EndOffset)));
						}
						startWord = new AnchorSegment(textArea.Document, textArea.Selection.SurroundingSegment);
					}
					else
					{
						textArea.Selection = Selection.Create(textArea, simpleSegment.Offset, simpleSegment.EndOffset);
						startWord = new AnchorSegment(textArea.Document, simpleSegment.Offset, simpleSegment.Length);
					}
				}
			}
		}
		e.Handled = true;
	}

	private SimpleSegment GetWordAtMousePosition(MouseEventArgs e)
	{
		TextView textView = textArea.TextView;
		if (textView == null)
		{
			return SimpleSegment.Invalid;
		}
		Point position = e.GetPosition(textView);
		if (position.Y < 0.0)
		{
			position.Y = 0.0;
		}
		if (position.Y > textView.ActualHeight)
		{
			position.Y = textView.ActualHeight;
		}
		position += textView.ScrollOffset;
		VisualLine visualLineFromVisualTop = textView.GetVisualLineFromVisualTop(position.Y);
		if (visualLineFromVisualTop != null)
		{
			int visualColumn = visualLineFromVisualTop.GetVisualColumn(position, textArea.Selection.EnableVirtualSpace);
			int num = visualLineFromVisualTop.GetNextCaretPosition(visualColumn + 1, LogicalDirection.Backward, CaretPositioningMode.WordStartOrSymbol, textArea.Selection.EnableVirtualSpace);
			if (num == -1)
			{
				num = 0;
			}
			int num2 = visualLineFromVisualTop.GetNextCaretPosition(num, LogicalDirection.Forward, CaretPositioningMode.WordBorderOrSymbol, textArea.Selection.EnableVirtualSpace);
			if (num2 == -1)
			{
				num2 = visualLineFromVisualTop.VisualLength;
			}
			int offset = visualLineFromVisualTop.FirstDocumentLine.Offset;
			int num3 = visualLineFromVisualTop.GetRelativeOffset(num) + offset;
			int num4 = visualLineFromVisualTop.GetRelativeOffset(num2) + offset;
			return new SimpleSegment(num3, num4 - num3);
		}
		return SimpleSegment.Invalid;
	}

	private SimpleSegment GetLineAtMousePosition(MouseEventArgs e)
	{
		TextView textView = textArea.TextView;
		if (textView == null)
		{
			return SimpleSegment.Invalid;
		}
		Point position = e.GetPosition(textView);
		if (position.Y < 0.0)
		{
			position.Y = 0.0;
		}
		if (position.Y > textView.ActualHeight)
		{
			position.Y = textView.ActualHeight;
		}
		VisualLine visualLineFromVisualTop = textView.GetVisualLineFromVisualTop((position + textView.ScrollOffset).Y);
		if (visualLineFromVisualTop != null)
		{
			return new SimpleSegment(visualLineFromVisualTop.StartOffset, visualLineFromVisualTop.LastDocumentLine.EndOffset - visualLineFromVisualTop.StartOffset);
		}
		return SimpleSegment.Invalid;
	}

	private int GetOffsetFromMousePosition(MouseEventArgs e, out int visualColumn, out bool isAtEndOfLine)
	{
		return GetOffsetFromMousePosition(e.GetPosition(textArea.TextView), out visualColumn, out isAtEndOfLine);
	}

	private int GetOffsetFromMousePosition(Point positionRelativeToTextView, out int visualColumn, out bool isAtEndOfLine)
	{
		visualColumn = 0;
		TextView textView = textArea.TextView;
		Point point = positionRelativeToTextView;
		if (point.Y < 0.0)
		{
			point.Y = 0.0;
		}
		if (point.Y > textView.ActualHeight)
		{
			point.Y = textView.ActualHeight;
		}
		point += textView.ScrollOffset;
		if (point.Y >= textView.DocumentHeight)
		{
			point.Y = textView.DocumentHeight - 0.01;
		}
		VisualLine visualLineFromVisualTop = textView.GetVisualLineFromVisualTop(point.Y);
		if (visualLineFromVisualTop != null)
		{
			visualColumn = visualLineFromVisualTop.GetVisualColumn(point, textArea.Selection.EnableVirtualSpace, out isAtEndOfLine);
			return visualLineFromVisualTop.GetRelativeOffset(visualColumn) + visualLineFromVisualTop.FirstDocumentLine.Offset;
		}
		isAtEndOfLine = false;
		return -1;
	}

	private int GetOffsetFromMousePositionFirstTextLineOnly(Point positionRelativeToTextView, out int visualColumn)
	{
		visualColumn = 0;
		TextView textView = textArea.TextView;
		Point point = positionRelativeToTextView;
		if (point.Y < 0.0)
		{
			point.Y = 0.0;
		}
		if (point.Y > textView.ActualHeight)
		{
			point.Y = textView.ActualHeight;
		}
		point += textView.ScrollOffset;
		if (point.Y >= textView.DocumentHeight)
		{
			point.Y = textView.DocumentHeight - 0.01;
		}
		VisualLine visualLineFromVisualTop = textView.GetVisualLineFromVisualTop(point.Y);
		if (visualLineFromVisualTop != null)
		{
			visualColumn = visualLineFromVisualTop.GetVisualColumn(visualLineFromVisualTop.TextLines.First(), point.X, textArea.Selection.EnableVirtualSpace);
			return visualLineFromVisualTop.GetRelativeOffset(visualColumn) + visualLineFromVisualTop.FirstDocumentLine.Offset;
		}
		return -1;
	}

	private void textArea_MouseMove(object sender, MouseEventArgs e)
	{
		if (e.Handled)
		{
			return;
		}
		if (mode == MouseSelectionMode.Normal || mode == MouseSelectionMode.WholeWord || mode == MouseSelectionMode.WholeLine || mode == MouseSelectionMode.Rectangular)
		{
			e.Handled = true;
			if (textArea.TextView.VisualLinesValid)
			{
				ExtendSelectionToMouse(e);
			}
		}
		else if (mode == MouseSelectionMode.PossibleDragStart)
		{
			e.Handled = true;
			Vector vector = e.GetPosition(textArea) - possibleDragStartMousePos;
			if (Math.Abs(vector.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(vector.Y) > SystemParameters.MinimumVerticalDragDistance)
			{
				StartDrag();
			}
		}
	}

	private void SetCaretOffsetToMousePosition(MouseEventArgs e)
	{
		SetCaretOffsetToMousePosition(e, null);
	}

	private void SetCaretOffsetToMousePosition(MouseEventArgs e, ISegment allowedSegment)
	{
		int num;
		int visualColumn;
		bool isAtEndOfLine;
		if (mode == MouseSelectionMode.Rectangular)
		{
			num = GetOffsetFromMousePositionFirstTextLineOnly(e.GetPosition(textArea.TextView), out visualColumn);
			isAtEndOfLine = true;
		}
		else
		{
			num = GetOffsetFromMousePosition(e, out visualColumn, out isAtEndOfLine);
		}
		if (allowedSegment != null)
		{
			num = num.CoerceValue(allowedSegment.Offset, allowedSegment.EndOffset);
		}
		if (num >= 0)
		{
			textArea.Caret.Position = new TextViewPosition(textArea.Document.GetLocation(num), visualColumn)
			{
				IsAtEndOfLine = isAtEndOfLine
			};
			textArea.Caret.DesiredXPos = double.NaN;
		}
	}

	private void ExtendSelectionToMouse(MouseEventArgs e)
	{
		TextViewPosition position = textArea.Caret.Position;
		if (mode == MouseSelectionMode.Normal || mode == MouseSelectionMode.Rectangular)
		{
			SetCaretOffsetToMousePosition(e);
			if (mode == MouseSelectionMode.Normal && textArea.Selection is RectangleSelection)
			{
				textArea.Selection = new SimpleSelection(textArea, position, textArea.Caret.Position);
			}
			else if (mode == MouseSelectionMode.Rectangular && !(textArea.Selection is RectangleSelection))
			{
				textArea.Selection = new RectangleSelection(textArea, position, textArea.Caret.Position);
			}
			else
			{
				textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(position, textArea.Caret.Position);
			}
		}
		else if (mode == MouseSelectionMode.WholeWord || mode == MouseSelectionMode.WholeLine)
		{
			SimpleSegment simpleSegment = ((mode == MouseSelectionMode.WholeLine) ? GetLineAtMousePosition(e) : GetWordAtMousePosition(e));
			if (simpleSegment != SimpleSegment.Invalid)
			{
				textArea.Selection = Selection.Create(textArea, Math.Min(simpleSegment.Offset, startWord.Offset), Math.Max(simpleSegment.EndOffset, startWord.EndOffset));
				if (simpleSegment.Offset < startWord.Offset)
				{
					textArea.Caret.Offset = simpleSegment.Offset;
				}
				else
				{
					textArea.Caret.Offset = Math.Max(simpleSegment.EndOffset, startWord.EndOffset);
				}
			}
		}
		textArea.Caret.BringCaretToView(5.0);
	}

	private void textArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		if (mode != MouseSelectionMode.None && !e.Handled)
		{
			e.Handled = true;
			if (mode == MouseSelectionMode.PossibleDragStart)
			{
				SetCaretOffsetToMousePosition(e);
				textArea.ClearSelection();
			}
			else if (mode == MouseSelectionMode.Normal || mode == MouseSelectionMode.WholeWord || mode == MouseSelectionMode.WholeLine || mode == MouseSelectionMode.Rectangular)
			{
				ExtendSelectionToMouse(e);
			}
			mode = MouseSelectionMode.None;
			textArea.ReleaseMouseCapture();
		}
	}
}
