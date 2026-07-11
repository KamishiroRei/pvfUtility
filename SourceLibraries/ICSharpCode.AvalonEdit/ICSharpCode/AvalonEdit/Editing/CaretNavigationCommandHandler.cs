using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

internal static class CaretNavigationCommandHandler
{
	private static readonly List<CommandBinding> CommandBindings;

	private static readonly List<InputBinding> InputBindings;

	public static TextAreaInputHandler Create(TextArea textArea)
	{
		TextAreaInputHandler textAreaInputHandler = new TextAreaInputHandler(textArea);
		textAreaInputHandler.CommandBindings.AddRange(CommandBindings);
		textAreaInputHandler.InputBindings.AddRange(InputBindings);
		return textAreaInputHandler;
	}

	private static void AddBinding(ICommand command, ModifierKeys modifiers, Key key, ExecutedRoutedEventHandler handler)
	{
		CommandBindings.Add(new CommandBinding(command, handler));
		InputBindings.Add(TextAreaDefaultInputHandler.CreateFrozenKeyBinding(command, modifiers, key));
	}

	static CaretNavigationCommandHandler()
	{
		CommandBindings = new List<CommandBinding>();
		InputBindings = new List<InputBinding>();
		AddBinding(EditingCommands.MoveLeftByCharacter, ModifierKeys.None, Key.Left, OnMoveCaret(CaretMovementType.CharLeft));
		AddBinding(EditingCommands.SelectLeftByCharacter, ModifierKeys.Shift, Key.Left, OnMoveCaretExtendSelection(CaretMovementType.CharLeft));
		AddBinding(RectangleSelection.BoxSelectLeftByCharacter, ModifierKeys.Alt | ModifierKeys.Shift, Key.Left, OnMoveCaretBoxSelection(CaretMovementType.CharLeft));
		AddBinding(EditingCommands.MoveRightByCharacter, ModifierKeys.None, Key.Right, OnMoveCaret(CaretMovementType.CharRight));
		AddBinding(EditingCommands.SelectRightByCharacter, ModifierKeys.Shift, Key.Right, OnMoveCaretExtendSelection(CaretMovementType.CharRight));
		AddBinding(RectangleSelection.BoxSelectRightByCharacter, ModifierKeys.Alt | ModifierKeys.Shift, Key.Right, OnMoveCaretBoxSelection(CaretMovementType.CharRight));
		AddBinding(EditingCommands.MoveLeftByWord, ModifierKeys.Control, Key.Left, OnMoveCaret(CaretMovementType.WordLeft));
		AddBinding(EditingCommands.SelectLeftByWord, ModifierKeys.Control | ModifierKeys.Shift, Key.Left, OnMoveCaretExtendSelection(CaretMovementType.WordLeft));
		AddBinding(RectangleSelection.BoxSelectLeftByWord, ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Shift, Key.Left, OnMoveCaretBoxSelection(CaretMovementType.WordLeft));
		AddBinding(EditingCommands.MoveRightByWord, ModifierKeys.Control, Key.Right, OnMoveCaret(CaretMovementType.WordRight));
		AddBinding(EditingCommands.SelectRightByWord, ModifierKeys.Control | ModifierKeys.Shift, Key.Right, OnMoveCaretExtendSelection(CaretMovementType.WordRight));
		AddBinding(RectangleSelection.BoxSelectRightByWord, ModifierKeys.Alt | ModifierKeys.Control | ModifierKeys.Shift, Key.Right, OnMoveCaretBoxSelection(CaretMovementType.WordRight));
		AddBinding(EditingCommands.MoveUpByLine, ModifierKeys.None, Key.Up, OnMoveCaret(CaretMovementType.LineUp));
		AddBinding(EditingCommands.SelectUpByLine, ModifierKeys.Shift, Key.Up, OnMoveCaretExtendSelection(CaretMovementType.LineUp));
		AddBinding(RectangleSelection.BoxSelectUpByLine, ModifierKeys.Alt | ModifierKeys.Shift, Key.Up, OnMoveCaretBoxSelection(CaretMovementType.LineUp));
		AddBinding(EditingCommands.MoveDownByLine, ModifierKeys.None, Key.Down, OnMoveCaret(CaretMovementType.LineDown));
		AddBinding(EditingCommands.SelectDownByLine, ModifierKeys.Shift, Key.Down, OnMoveCaretExtendSelection(CaretMovementType.LineDown));
		AddBinding(RectangleSelection.BoxSelectDownByLine, ModifierKeys.Alt | ModifierKeys.Shift, Key.Down, OnMoveCaretBoxSelection(CaretMovementType.LineDown));
		AddBinding(EditingCommands.MoveDownByPage, ModifierKeys.None, Key.Next, OnMoveCaret(CaretMovementType.PageDown));
		AddBinding(EditingCommands.SelectDownByPage, ModifierKeys.Shift, Key.Next, OnMoveCaretExtendSelection(CaretMovementType.PageDown));
		AddBinding(EditingCommands.MoveUpByPage, ModifierKeys.None, Key.Prior, OnMoveCaret(CaretMovementType.PageUp));
		AddBinding(EditingCommands.SelectUpByPage, ModifierKeys.Shift, Key.Prior, OnMoveCaretExtendSelection(CaretMovementType.PageUp));
		AddBinding(EditingCommands.MoveToLineStart, ModifierKeys.None, Key.Home, OnMoveCaret(CaretMovementType.LineStart));
		AddBinding(EditingCommands.SelectToLineStart, ModifierKeys.Shift, Key.Home, OnMoveCaretExtendSelection(CaretMovementType.LineStart));
		AddBinding(RectangleSelection.BoxSelectToLineStart, ModifierKeys.Alt | ModifierKeys.Shift, Key.Home, OnMoveCaretBoxSelection(CaretMovementType.LineStart));
		AddBinding(EditingCommands.MoveToLineEnd, ModifierKeys.None, Key.End, OnMoveCaret(CaretMovementType.LineEnd));
		AddBinding(EditingCommands.SelectToLineEnd, ModifierKeys.Shift, Key.End, OnMoveCaretExtendSelection(CaretMovementType.LineEnd));
		AddBinding(RectangleSelection.BoxSelectToLineEnd, ModifierKeys.Alt | ModifierKeys.Shift, Key.End, OnMoveCaretBoxSelection(CaretMovementType.LineEnd));
		AddBinding(EditingCommands.MoveToDocumentStart, ModifierKeys.Control, Key.Home, OnMoveCaret(CaretMovementType.DocumentStart));
		AddBinding(EditingCommands.SelectToDocumentStart, ModifierKeys.Control | ModifierKeys.Shift, Key.Home, OnMoveCaretExtendSelection(CaretMovementType.DocumentStart));
		AddBinding(EditingCommands.MoveToDocumentEnd, ModifierKeys.Control, Key.End, OnMoveCaret(CaretMovementType.DocumentEnd));
		AddBinding(EditingCommands.SelectToDocumentEnd, ModifierKeys.Control | ModifierKeys.Shift, Key.End, OnMoveCaretExtendSelection(CaretMovementType.DocumentEnd));
		CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, OnSelectAll));
		TextAreaDefaultInputHandler.WorkaroundWPFMemoryLeak(InputBindings);
	}

	private static void OnSelectAll(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea != null && textArea.Document != null)
		{
			args.Handled = true;
			textArea.Caret.Offset = textArea.Document.TextLength;
			textArea.Selection = Selection.Create(textArea, 0, textArea.Document.TextLength);
		}
	}

	private static TextArea GetTextArea(object target)
	{
		return target as TextArea;
	}

	private static ExecutedRoutedEventHandler OnMoveCaret(CaretMovementType direction)
	{
		return delegate(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null)
			{
				args.Handled = true;
				textArea.ClearSelection();
				MoveCaret(textArea, direction);
				textArea.Caret.BringCaretToView();
			}
		};
	}

	private static ExecutedRoutedEventHandler OnMoveCaretExtendSelection(CaretMovementType direction)
	{
		return delegate(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null)
			{
				args.Handled = true;
				TextViewPosition position = textArea.Caret.Position;
				MoveCaret(textArea, direction);
				textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(position, textArea.Caret.Position);
				textArea.Caret.BringCaretToView();
			}
		};
	}

	private static ExecutedRoutedEventHandler OnMoveCaretBoxSelection(CaretMovementType direction)
	{
		return delegate(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null)
			{
				args.Handled = true;
				if (textArea.Options.EnableRectangularSelection && !(textArea.Selection is RectangleSelection))
				{
					if (textArea.Selection.IsEmpty)
					{
						textArea.Selection = new RectangleSelection(textArea, textArea.Caret.Position, textArea.Caret.Position);
					}
					else
					{
						textArea.Selection = new RectangleSelection(textArea, textArea.Selection.StartPosition, textArea.Caret.Position);
					}
				}
				TextViewPosition position = textArea.Caret.Position;
				MoveCaret(textArea, direction);
				textArea.Selection = textArea.Selection.StartSelectionOrSetEndpoint(position, textArea.Caret.Position);
				textArea.Caret.BringCaretToView();
			}
		};
	}

	internal static void MoveCaret(TextArea textArea, CaretMovementType direction)
	{
		double desiredXPos = textArea.Caret.DesiredXPos;
		if (textArea.FlowDirection == FlowDirection.RightToLeft)
		{
			switch (direction)
			{
			case CaretMovementType.CharLeft:
				direction = CaretMovementType.CharRight;
				break;
			case CaretMovementType.CharRight:
				direction = CaretMovementType.CharLeft;
				break;
			case CaretMovementType.WordRight:
				direction = CaretMovementType.WordLeft;
				break;
			case CaretMovementType.WordLeft:
				direction = CaretMovementType.WordRight;
				break;
			}
		}
		textArea.Caret.Position = GetNewCaretPosition(textArea.TextView, textArea.Caret.Position, direction, textArea.Selection.EnableVirtualSpace, ref desiredXPos);
		textArea.Caret.DesiredXPos = desiredXPos;
	}

	internal static TextViewPosition GetNewCaretPosition(TextView textView, TextViewPosition caretPosition, CaretMovementType direction, bool enableVirtualSpace, ref double desiredXPos)
	{
		switch (direction)
		{
		case CaretMovementType.None:
			return caretPosition;
		case CaretMovementType.DocumentStart:
			desiredXPos = double.NaN;
			return new TextViewPosition(0, 0);
		case CaretMovementType.DocumentEnd:
			desiredXPos = double.NaN;
			return new TextViewPosition(textView.Document.GetLocation(textView.Document.TextLength));
		default:
		{
			DocumentLine lineByNumber = textView.Document.GetLineByNumber(caretPosition.Line);
			VisualLine orConstructVisualLine = textView.GetOrConstructVisualLine(lineByNumber);
			TextLine textLine = orConstructVisualLine.GetTextLine(caretPosition.VisualColumn, caretPosition.IsAtEndOfLine);
			switch (direction)
			{
			case CaretMovementType.CharLeft:
				desiredXPos = double.NaN;
				if (caretPosition.VisualColumn == 0 && enableVirtualSpace)
				{
					return caretPosition;
				}
				return GetPrevCaretPosition(textView, caretPosition, orConstructVisualLine, CaretPositioningMode.Normal, enableVirtualSpace);
			case CaretMovementType.Backspace:
				desiredXPos = double.NaN;
				return GetPrevCaretPosition(textView, caretPosition, orConstructVisualLine, CaretPositioningMode.EveryCodepoint, enableVirtualSpace);
			case CaretMovementType.CharRight:
				desiredXPos = double.NaN;
				return GetNextCaretPosition(textView, caretPosition, orConstructVisualLine, CaretPositioningMode.Normal, enableVirtualSpace);
			case CaretMovementType.WordLeft:
				desiredXPos = double.NaN;
				return GetPrevCaretPosition(textView, caretPosition, orConstructVisualLine, CaretPositioningMode.WordStart, enableVirtualSpace);
			case CaretMovementType.WordRight:
				desiredXPos = double.NaN;
				return GetNextCaretPosition(textView, caretPosition, orConstructVisualLine, CaretPositioningMode.WordStart, enableVirtualSpace);
			case CaretMovementType.LineUp:
			case CaretMovementType.LineDown:
			case CaretMovementType.PageUp:
			case CaretMovementType.PageDown:
				return GetUpDownCaretPosition(textView, caretPosition, direction, orConstructVisualLine, textLine, enableVirtualSpace, ref desiredXPos);
			case CaretMovementType.LineStart:
				desiredXPos = double.NaN;
				return GetStartOfLineCaretPosition(caretPosition.VisualColumn, orConstructVisualLine, textLine, enableVirtualSpace);
			case CaretMovementType.LineEnd:
				desiredXPos = double.NaN;
				return GetEndOfLineCaretPosition(orConstructVisualLine, textLine);
			default:
				throw new NotSupportedException(direction.ToString());
			}
		}
		}
	}

	private static TextViewPosition GetStartOfLineCaretPosition(int oldVC, VisualLine visualLine, TextLine textLine, bool enableVirtualSpace)
	{
		int num = visualLine.GetTextLineVisualStartColumn(textLine);
		if (num == 0)
		{
			num = visualLine.GetNextCaretPosition(num - 1, LogicalDirection.Forward, CaretPositioningMode.WordStart, enableVirtualSpace);
		}
		if (num < 0)
		{
			throw ThrowUtil.NoValidCaretPosition();
		}
		if (num == oldVC)
		{
			num = 0;
		}
		return visualLine.GetTextViewPosition(num);
	}

	private static TextViewPosition GetEndOfLineCaretPosition(VisualLine visualLine, TextLine textLine)
	{
		int visualColumn = visualLine.GetTextLineVisualStartColumn(textLine) + textLine.Length - textLine.NewlineLength;
		TextViewPosition textViewPosition = visualLine.GetTextViewPosition(visualColumn);
		textViewPosition.IsAtEndOfLine = true;
		return textViewPosition;
	}

	private static TextViewPosition GetNextCaretPosition(TextView textView, TextViewPosition caretPosition, VisualLine visualLine, CaretPositioningMode mode, bool enableVirtualSpace)
	{
		int nextCaretPosition = visualLine.GetNextCaretPosition(caretPosition.VisualColumn, LogicalDirection.Forward, mode, enableVirtualSpace);
		if (nextCaretPosition >= 0)
		{
			return visualLine.GetTextViewPosition(nextCaretPosition);
		}
		DocumentLine nextLine = visualLine.LastDocumentLine.NextLine;
		if (nextLine != null)
		{
			VisualLine orConstructVisualLine = textView.GetOrConstructVisualLine(nextLine);
			nextCaretPosition = orConstructVisualLine.GetNextCaretPosition(-1, LogicalDirection.Forward, mode, enableVirtualSpace);
			if (nextCaretPosition < 0)
			{
				throw ThrowUtil.NoValidCaretPosition();
			}
			return orConstructVisualLine.GetTextViewPosition(nextCaretPosition);
		}
		return new TextViewPosition(textView.Document.GetLocation(textView.Document.TextLength));
	}

	private static TextViewPosition GetPrevCaretPosition(TextView textView, TextViewPosition caretPosition, VisualLine visualLine, CaretPositioningMode mode, bool enableVirtualSpace)
	{
		int nextCaretPosition = visualLine.GetNextCaretPosition(caretPosition.VisualColumn, LogicalDirection.Backward, mode, enableVirtualSpace);
		if (nextCaretPosition >= 0)
		{
			return visualLine.GetTextViewPosition(nextCaretPosition);
		}
		DocumentLine previousLine = visualLine.FirstDocumentLine.PreviousLine;
		if (previousLine != null)
		{
			VisualLine orConstructVisualLine = textView.GetOrConstructVisualLine(previousLine);
			nextCaretPosition = orConstructVisualLine.GetNextCaretPosition(orConstructVisualLine.VisualLength + 1, LogicalDirection.Backward, mode, enableVirtualSpace);
			if (nextCaretPosition < 0)
			{
				throw ThrowUtil.NoValidCaretPosition();
			}
			return orConstructVisualLine.GetTextViewPosition(nextCaretPosition);
		}
		return new TextViewPosition(0, 0);
	}

	private static TextViewPosition GetUpDownCaretPosition(TextView textView, TextViewPosition caretPosition, CaretMovementType direction, VisualLine visualLine, TextLine textLine, bool enableVirtualSpace, ref double xPos)
	{
		if (double.IsNaN(xPos))
		{
			xPos = visualLine.GetTextLineVisualXPosition(textLine, caretPosition.VisualColumn);
		}
		VisualLine visualLine2 = visualLine;
		int num = visualLine.TextLines.IndexOf(textLine);
		TextLine textLine2;
		switch (direction)
		{
		case CaretMovementType.LineUp:
		{
			int num3 = visualLine.FirstDocumentLine.LineNumber - 1;
			if (num > 0)
			{
				textLine2 = visualLine.TextLines[num - 1];
			}
			else if (num3 >= 1)
			{
				DocumentLine lineByNumber2 = textView.Document.GetLineByNumber(num3);
				visualLine2 = textView.GetOrConstructVisualLine(lineByNumber2);
				textLine2 = visualLine2.TextLines[visualLine2.TextLines.Count - 1];
			}
			else
			{
				textLine2 = null;
			}
			break;
		}
		case CaretMovementType.LineDown:
		{
			int num2 = visualLine.LastDocumentLine.LineNumber + 1;
			if (num < visualLine.TextLines.Count - 1)
			{
				textLine2 = visualLine.TextLines[num + 1];
			}
			else if (num2 <= textView.Document.LineCount)
			{
				DocumentLine lineByNumber = textView.Document.GetLineByNumber(num2);
				visualLine2 = textView.GetOrConstructVisualLine(lineByNumber);
				textLine2 = visualLine2.TextLines[0];
			}
			else
			{
				textLine2 = null;
			}
			break;
		}
		case CaretMovementType.PageUp:
		case CaretMovementType.PageDown:
		{
			double textLineVisualYPosition = visualLine.GetTextLineVisualYPosition(textLine, VisualYPosition.LineMiddle);
			textLineVisualYPosition = ((direction != CaretMovementType.PageUp) ? (textLineVisualYPosition + textView.RenderSize.Height) : (textLineVisualYPosition - textView.RenderSize.Height));
			DocumentLine documentLineByVisualTop = textView.GetDocumentLineByVisualTop(textLineVisualYPosition);
			visualLine2 = textView.GetOrConstructVisualLine(documentLineByVisualTop);
			textLine2 = visualLine2.GetTextLineByVisualYPosition(textLineVisualYPosition);
			break;
		}
		default:
			throw new NotSupportedException(direction.ToString());
		}
		if (textLine2 != null)
		{
			double textLineVisualYPosition2 = visualLine2.GetTextLineVisualYPosition(textLine2, VisualYPosition.LineMiddle);
			int num4 = visualLine2.GetVisualColumn(new Point(xPos, textLineVisualYPosition2), enableVirtualSpace);
			int textLineVisualStartColumn = visualLine2.GetTextLineVisualStartColumn(textLine2);
			if (num4 >= textLineVisualStartColumn + textLine2.Length && num4 <= visualLine2.VisualLength)
			{
				num4 = textLineVisualStartColumn + textLine2.Length - 1;
			}
			return visualLine2.GetTextViewPosition(num4);
		}
		return caretPosition;
	}
}
