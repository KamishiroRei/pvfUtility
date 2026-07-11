using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

internal static class EditingCommandHandler
{
	private enum DefaultSegmentType
	{
		None,
		WholeDocument,
		CurrentLine
	}

	private static readonly List<CommandBinding> CommandBindings;

	private static readonly List<InputBinding> InputBindings;

	private const string LineSelectedType = "MSDEVLineSelect";

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

	static EditingCommandHandler()
	{
		CommandBindings = new List<CommandBinding>();
		InputBindings = new List<InputBinding>();
		CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, OnDelete(CaretMovementType.None), CanDelete));
		AddBinding(EditingCommands.Delete, ModifierKeys.None, Key.Delete, OnDelete(CaretMovementType.CharRight));
		AddBinding(EditingCommands.DeleteNextWord, ModifierKeys.Control, Key.Delete, OnDelete(CaretMovementType.WordRight));
		AddBinding(EditingCommands.Backspace, ModifierKeys.None, Key.Back, OnDelete(CaretMovementType.Backspace));
		InputBindings.Add(TextAreaDefaultInputHandler.CreateFrozenKeyBinding(EditingCommands.Backspace, ModifierKeys.Shift, Key.Back));
		AddBinding(EditingCommands.DeletePreviousWord, ModifierKeys.Control, Key.Back, OnDelete(CaretMovementType.WordLeft));
		AddBinding(EditingCommands.EnterParagraphBreak, ModifierKeys.None, Key.Return, OnEnter);
		AddBinding(EditingCommands.EnterLineBreak, ModifierKeys.Shift, Key.Return, OnEnter);
		AddBinding(EditingCommands.TabForward, ModifierKeys.None, Key.Tab, OnTab);
		AddBinding(EditingCommands.TabBackward, ModifierKeys.Shift, Key.Tab, OnShiftTab);
		CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, OnCopy, CanCutOrCopy));
		CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, OnCut, CanCutOrCopy));
		CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste, CanPaste));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.ToggleOverstrike, OnToggleOverstrike));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.DeleteLine, OnDeleteLine));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.RemoveLeadingWhitespace, OnRemoveLeadingWhitespace));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.RemoveTrailingWhitespace, OnRemoveTrailingWhitespace));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertToUppercase, OnConvertToUpperCase));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertToLowercase, OnConvertToLowerCase));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertToTitleCase, OnConvertToTitleCase));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.InvertCase, OnInvertCase));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertTabsToSpaces, OnConvertTabsToSpaces));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertSpacesToTabs, OnConvertSpacesToTabs));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertLeadingTabsToSpaces, OnConvertLeadingTabsToSpaces));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.ConvertLeadingSpacesToTabs, OnConvertLeadingSpacesToTabs));
		CommandBindings.Add(new CommandBinding(AvalonEditCommands.IndentSelection, OnIndentSelection));
		TextAreaDefaultInputHandler.WorkaroundWPFMemoryLeak(InputBindings);
	}

	private static TextArea GetTextArea(object target)
	{
		return target as TextArea;
	}

	private static void TransformSelectedLines(Action<TextArea, DocumentLine> transformLine, object target, ExecutedRoutedEventArgs args, DefaultSegmentType defaultSegmentType)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea == null || textArea.Document == null)
		{
			return;
		}
		using (textArea.Document.RunUpdate())
		{
			DocumentLine documentLine;
			DocumentLine documentLine2;
			if (textArea.Selection.IsEmpty)
			{
				switch (defaultSegmentType)
				{
				case DefaultSegmentType.CurrentLine:
					documentLine = (documentLine2 = textArea.Document.GetLineByNumber(textArea.Caret.Line));
					break;
				case DefaultSegmentType.WholeDocument:
					documentLine = textArea.Document.Lines.First();
					documentLine2 = textArea.Document.Lines.Last();
					break;
				default:
					documentLine = (documentLine2 = null);
					break;
				}
			}
			else
			{
				ISegment surroundingSegment = textArea.Selection.SurroundingSegment;
				documentLine = textArea.Document.GetLineByOffset(surroundingSegment.Offset);
				documentLine2 = textArea.Document.GetLineByOffset(surroundingSegment.EndOffset);
				if (documentLine != documentLine2 && documentLine2.Offset == surroundingSegment.EndOffset)
				{
					documentLine2 = documentLine2.PreviousLine;
				}
			}
			if (documentLine != null)
			{
				transformLine(textArea, documentLine);
				while (documentLine != documentLine2)
				{
					documentLine = documentLine.NextLine;
					transformLine(textArea, documentLine);
				}
			}
		}
		textArea.Caret.BringCaretToView();
		args.Handled = true;
	}

	private static void TransformSelectedSegments(Action<TextArea, ISegment> transformSegment, object target, ExecutedRoutedEventArgs args, DefaultSegmentType defaultSegmentType)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea == null || textArea.Document == null)
		{
			return;
		}
		using (textArea.Document.RunUpdate())
		{
			IEnumerable<ISegment> enumerable = ((!textArea.Selection.IsEmpty) ? textArea.Selection.Segments.Cast<ISegment>() : (defaultSegmentType switch
			{
				DefaultSegmentType.CurrentLine => new ISegment[1] { textArea.Document.GetLineByNumber(textArea.Caret.Line) }, 
				DefaultSegmentType.WholeDocument => textArea.Document.Lines.Cast<ISegment>(), 
				_ => null, 
			}));
			if (enumerable != null)
			{
				foreach (ISegment item in enumerable.Reverse())
				{
					foreach (ISegment item2 in textArea.GetDeletableSegments(item).Reverse())
					{
						transformSegment(textArea, item2);
					}
				}
			}
		}
		textArea.Caret.BringCaretToView();
		args.Handled = true;
	}

	private static void OnEnter(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea != null && textArea.IsKeyboardFocused)
		{
			textArea.PerformTextInput("\n");
			args.Handled = true;
		}
	}

	private static void OnTab(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea == null || textArea.Document == null)
		{
			return;
		}
		using (textArea.Document.RunUpdate())
		{
			if (textArea.Selection.IsMultiline)
			{
				ISegment surroundingSegment = textArea.Selection.SurroundingSegment;
				DocumentLine lineByOffset = textArea.Document.GetLineByOffset(surroundingSegment.Offset);
				DocumentLine documentLine = textArea.Document.GetLineByOffset(surroundingSegment.EndOffset);
				if (lineByOffset != documentLine && documentLine.Offset == surroundingSegment.EndOffset)
				{
					documentLine = documentLine.PreviousLine;
				}
				DocumentLine documentLine2 = lineByOffset;
				while (true)
				{
					int offset = documentLine2.Offset;
					if (textArea.ReadOnlySectionProvider.CanInsert(offset))
					{
						textArea.Document.Replace(offset, 0, textArea.Options.IndentationString, OffsetChangeMappingType.KeepAnchorBeforeInsertion);
					}
					if (documentLine2 == documentLine)
					{
						break;
					}
					documentLine2 = documentLine2.NextLine;
				}
			}
			else
			{
				string indentationString = textArea.Options.GetIndentationString(textArea.Caret.Column);
				textArea.ReplaceSelectionWithText(indentationString);
			}
		}
		textArea.Caret.BringCaretToView();
		args.Handled = true;
	}

	private static void OnShiftTab(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedLines(delegate(TextArea textArea, DocumentLine line)
		{
			int offset = line.Offset;
			ISegment singleIndentationSegment = TextUtilities.GetSingleIndentationSegment(textArea.Document, offset, textArea.Options.IndentationSize);
			if (singleIndentationSegment.Length > 0)
			{
				singleIndentationSegment = textArea.GetDeletableSegments(singleIndentationSegment).FirstOrDefault();
				if (singleIndentationSegment != null && singleIndentationSegment.Length > 0)
				{
					textArea.Document.Remove(singleIndentationSegment.Offset, singleIndentationSegment.Length);
				}
			}
		}, target, args, DefaultSegmentType.CurrentLine);
	}

	private static ExecutedRoutedEventHandler OnDelete(CaretMovementType caretMovement)
	{
		return delegate(object target, ExecutedRoutedEventArgs args)
		{
			TextArea textArea = GetTextArea(target);
			if (textArea != null && textArea.Document != null)
			{
				if (textArea.Selection.IsEmpty)
				{
					TextViewPosition position = textArea.Caret.Position;
					bool enableVirtualSpace = textArea.Options.EnableVirtualSpace;
					if (caretMovement == CaretMovementType.CharRight)
					{
						enableVirtualSpace = false;
					}
					double desiredXPos = textArea.Caret.DesiredXPos;
					TextViewPosition endPosition = CaretNavigationCommandHandler.GetNewCaretPosition(textArea.TextView, position, caretMovement, enableVirtualSpace, ref desiredXPos);
					if (endPosition.Line < 1 || endPosition.Column < 1)
					{
						endPosition = new TextViewPosition(Math.Max(endPosition.Line, 1), Math.Max(endPosition.Column, 1));
					}
					if (textArea.Selection is RectangleSelection && position.Line != endPosition.Line)
					{
						return;
					}
					textArea.Selection.StartSelectionOrSetEndpoint(position, endPosition).ReplaceSelectionWithText(string.Empty);
				}
				else
				{
					textArea.RemoveSelectedText();
				}
				textArea.Caret.BringCaretToView();
				args.Handled = true;
			}
		};
	}

	private static void CanDelete(object target, CanExecuteRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea != null && textArea.Document != null)
		{
			args.CanExecute = !textArea.Selection.IsEmpty;
			args.Handled = true;
		}
	}

	private static void CanCutOrCopy(object target, CanExecuteRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea != null && textArea.Document != null)
		{
			args.CanExecute = textArea.Options.CutCopyWholeLine || !textArea.Selection.IsEmpty;
			args.Handled = true;
		}
	}

	private static void OnCopy(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea != null && textArea.Document != null)
		{
			if (textArea.Selection.IsEmpty && textArea.Options.CutCopyWholeLine)
			{
				DocumentLine lineByNumber = textArea.Document.GetLineByNumber(textArea.Caret.Line);
				CopyWholeLine(textArea, lineByNumber);
			}
			else
			{
				CopySelectedText(textArea);
			}
			args.Handled = true;
		}
	}

	private static void OnCut(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea == null || textArea.Document == null)
		{
			return;
		}
		if (textArea.Selection.IsEmpty && textArea.Options.CutCopyWholeLine)
		{
			DocumentLine lineByNumber = textArea.Document.GetLineByNumber(textArea.Caret.Line);
			if (CopyWholeLine(textArea, lineByNumber))
			{
				ISegment[] deletableSegments = textArea.GetDeletableSegments(new SimpleSegment(lineByNumber.Offset, lineByNumber.TotalLength));
				for (int num = deletableSegments.Length - 1; num >= 0; num--)
				{
					textArea.Document.Remove(deletableSegments[num]);
				}
			}
		}
		else if (CopySelectedText(textArea))
		{
			textArea.RemoveSelectedText();
		}
		textArea.Caret.BringCaretToView();
		args.Handled = true;
	}

	private static bool CopySelectedText(TextArea textArea)
	{
		DataObject dataObject = textArea.Selection.CreateDataObject(textArea);
		DataObjectCopyingEventArgs e = new DataObjectCopyingEventArgs(dataObject, isDragDrop: false);
		textArea.RaiseEvent(e);
		if (e.CommandCancelled)
		{
			return false;
		}
		try
		{
			Clipboard.SetDataObject(dataObject, copy: true);
		}
		catch (ExternalException)
		{
		}
		string text = textArea.Selection.GetText();
		text = TextUtilities.NormalizeNewLines(text, Environment.NewLine);
		textArea.OnTextCopied(new TextEventArgs(text));
		return true;
	}

	public static bool ConfirmDataFormat(TextArea textArea, DataObject dataObject, string format)
	{
		DataObjectSettingDataEventArgs e = new DataObjectSettingDataEventArgs(dataObject, format);
		textArea.RaiseEvent(e);
		return !e.CommandCancelled;
	}

	private static bool CopyWholeLine(TextArea textArea, DocumentLine line)
	{
		ISegment segment = new SimpleSegment(line.Offset, line.TotalLength);
		string text = textArea.Document.GetText(segment);
		text = TextUtilities.NormalizeNewLines(text, Environment.NewLine);
		DataObject dataObject = new DataObject();
		if (ConfirmDataFormat(textArea, dataObject, DataFormats.UnicodeText))
		{
			dataObject.SetText(text);
		}
		if (ConfirmDataFormat(textArea, dataObject, DataFormats.Html))
		{
			IHighlighter highlighter = textArea.GetService(typeof(IHighlighter)) as IHighlighter;
			HtmlClipboard.SetHtml(dataObject, HtmlClipboard.CreateHtmlFragment(textArea.Document, highlighter, segment, new HtmlOptions(textArea.Options)));
		}
		if (ConfirmDataFormat(textArea, dataObject, "MSDEVLineSelect"))
		{
			MemoryStream memoryStream = new MemoryStream(1);
			memoryStream.WriteByte(1);
			dataObject.SetData("MSDEVLineSelect", memoryStream, autoConvert: false);
		}
		DataObjectCopyingEventArgs e = new DataObjectCopyingEventArgs(dataObject, isDragDrop: false);
		textArea.RaiseEvent(e);
		if (e.CommandCancelled)
		{
			return false;
		}
		try
		{
			Clipboard.SetDataObject(dataObject, copy: true);
		}
		catch (ExternalException)
		{
			return false;
		}
		textArea.OnTextCopied(new TextEventArgs(text));
		return true;
	}

	private static void CanPaste(object target, CanExecuteRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea != null && textArea.Document != null)
		{
			args.CanExecute = textArea.ReadOnlySectionProvider.CanInsert(textArea.Caret.Offset) && Clipboard.ContainsText();
			args.Handled = true;
		}
	}

	private static void OnPaste(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea == null || textArea.Document == null)
		{
			return;
		}
		IDataObject dataObject;
		try
		{
			dataObject = Clipboard.GetDataObject();
		}
		catch (ExternalException)
		{
			return;
		}
		if (dataObject == null)
		{
			return;
		}
		DataObjectPastingEventArgs e = new DataObjectPastingEventArgs(dataObject, isDragDrop: false, DataFormats.UnicodeText);
		textArea.RaiseEvent(e);
		if (e.CommandCancelled)
		{
			return;
		}
		string textToPaste = GetTextToPaste(e, textArea);
		if (!string.IsNullOrEmpty(textToPaste))
		{
			dataObject = e.DataObject;
			bool num = textArea.Options.CutCopyWholeLine && dataObject.GetDataPresent("MSDEVLineSelect");
			bool dataPresent = dataObject.GetDataPresent("AvalonEditRectangularSelection");
			if (num)
			{
				DocumentLine lineByNumber = textArea.Document.GetLineByNumber(textArea.Caret.Line);
				if (textArea.ReadOnlySectionProvider.CanInsert(lineByNumber.Offset))
				{
					textArea.Document.Insert(lineByNumber.Offset, textToPaste);
				}
			}
			else if (dataPresent && textArea.Selection.IsEmpty && !(textArea.Selection is RectangleSelection))
			{
				if (!RectangleSelection.PerformRectangularPaste(textArea, textArea.Caret.Position, textToPaste, selectInsertedText: false))
				{
					textArea.ReplaceSelectionWithText(textToPaste);
				}
			}
			else
			{
				textArea.ReplaceSelectionWithText(textToPaste);
			}
		}
		textArea.Caret.BringCaretToView();
		args.Handled = true;
	}

	internal static string GetTextToPaste(DataObjectPastingEventArgs pastingEventArgs, TextArea textArea)
	{
		IDataObject dataObject = pastingEventArgs.DataObject;
		if (dataObject == null)
		{
			return null;
		}
		try
		{
			string input;
			if (pastingEventArgs.FormatToApply != null && dataObject.GetDataPresent(pastingEventArgs.FormatToApply))
			{
				input = (string)dataObject.GetData(pastingEventArgs.FormatToApply);
			}
			else if (pastingEventArgs.FormatToApply != DataFormats.UnicodeText && dataObject.GetDataPresent(DataFormats.UnicodeText))
			{
				input = (string)dataObject.GetData(DataFormats.UnicodeText);
			}
			else
			{
				if (!(pastingEventArgs.FormatToApply != DataFormats.Text) || !dataObject.GetDataPresent(DataFormats.Text))
				{
					return null;
				}
				input = (string)dataObject.GetData(DataFormats.Text);
			}
			string newLineFromDocument = TextUtilities.GetNewLineFromDocument(textArea.Document, textArea.Caret.Line);
			input = TextUtilities.NormalizeNewLines(input, newLineFromDocument);
			return textArea.Options.ConvertTabsToSpaces ? input.Replace("\t", new string(' ', textArea.Options.IndentationSize)) : input;
		}
		catch (OutOfMemoryException)
		{
			return null;
		}
		catch (COMException)
		{
			return null;
		}
	}

	private static void OnToggleOverstrike(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea != null && textArea.Options.AllowToggleOverstrikeMode)
		{
			textArea.OverstrikeMode = !textArea.OverstrikeMode;
		}
	}

	private static void OnDeleteLine(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea != null && textArea.Document != null)
		{
			int number;
			int number2;
			if (textArea.Selection.Length == 0)
			{
				number = (number2 = textArea.Caret.Line);
			}
			else
			{
				number = Math.Min(textArea.Selection.StartPosition.Line, textArea.Selection.EndPosition.Line);
				number2 = Math.Max(textArea.Selection.StartPosition.Line, textArea.Selection.EndPosition.Line);
			}
			DocumentLine lineByNumber = textArea.Document.GetLineByNumber(number);
			DocumentLine lineByNumber2 = textArea.Document.GetLineByNumber(number2);
			textArea.Selection = Selection.Create(textArea, lineByNumber.Offset, lineByNumber2.Offset + lineByNumber2.TotalLength);
			textArea.RemoveSelectedText();
			args.Handled = true;
		}
	}

	private static void OnRemoveLeadingWhitespace(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedLines(delegate(TextArea textArea, DocumentLine line)
		{
			textArea.Document.Remove(TextUtilities.GetLeadingWhitespace(textArea.Document, line));
		}, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void OnRemoveTrailingWhitespace(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedLines(delegate(TextArea textArea, DocumentLine line)
		{
			textArea.Document.Remove(TextUtilities.GetTrailingWhitespace(textArea.Document, line));
		}, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void OnConvertTabsToSpaces(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedSegments(ConvertTabsToSpaces, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void OnConvertLeadingTabsToSpaces(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedLines(delegate(TextArea textArea, DocumentLine line)
		{
			ConvertTabsToSpaces(textArea, TextUtilities.GetLeadingWhitespace(textArea.Document, line));
		}, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void ConvertTabsToSpaces(TextArea textArea, ISegment segment)
	{
		TextDocument document = textArea.Document;
		int num = segment.EndOffset;
		string text = new string(' ', textArea.Options.IndentationSize);
		for (int i = segment.Offset; i < num; i++)
		{
			if (document.GetCharAt(i) == '\t')
			{
				document.Replace(i, 1, text, OffsetChangeMappingType.CharacterReplace);
				num += text.Length - 1;
			}
		}
	}

	private static void OnConvertSpacesToTabs(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedSegments(ConvertSpacesToTabs, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void OnConvertLeadingSpacesToTabs(object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedLines(delegate(TextArea textArea, DocumentLine line)
		{
			ConvertSpacesToTabs(textArea, TextUtilities.GetLeadingWhitespace(textArea.Document, line));
		}, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void ConvertSpacesToTabs(TextArea textArea, ISegment segment)
	{
		TextDocument document = textArea.Document;
		int num = segment.EndOffset;
		int indentationSize = textArea.Options.IndentationSize;
		int num2 = 0;
		for (int i = segment.Offset; i < num; i++)
		{
			if (document.GetCharAt(i) == ' ')
			{
				num2++;
				if (num2 == indentationSize)
				{
					document.Replace(i - (indentationSize - 1), indentationSize, "\t", OffsetChangeMappingType.CharacterReplace);
					num2 = 0;
					i -= indentationSize - 1;
					num -= indentationSize - 1;
				}
			}
			else
			{
				num2 = 0;
			}
		}
	}

	private static void ConvertCase(Func<string, string> transformText, object target, ExecutedRoutedEventArgs args)
	{
		TransformSelectedSegments(delegate(TextArea textArea, ISegment segment)
		{
			string text = textArea.Document.GetText(segment);
			string text2 = transformText(text);
			textArea.Document.Replace(segment.Offset, segment.Length, text2, OffsetChangeMappingType.CharacterReplace);
		}, target, args, DefaultSegmentType.WholeDocument);
	}

	private static void OnConvertToUpperCase(object target, ExecutedRoutedEventArgs args)
	{
		ConvertCase(CultureInfo.CurrentCulture.TextInfo.ToUpper, target, args);
	}

	private static void OnConvertToLowerCase(object target, ExecutedRoutedEventArgs args)
	{
		ConvertCase(CultureInfo.CurrentCulture.TextInfo.ToLower, target, args);
	}

	private static void OnConvertToTitleCase(object target, ExecutedRoutedEventArgs args)
	{
		ConvertCase(CultureInfo.CurrentCulture.TextInfo.ToTitleCase, target, args);
	}

	private static void OnInvertCase(object target, ExecutedRoutedEventArgs args)
	{
		ConvertCase(InvertCase, target, args);
	}

	private static string InvertCase(string text)
	{
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		char[] array = text.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			array[i] = (char.IsUpper(c) ? char.ToLower(c, currentCulture) : char.ToUpper(c, currentCulture));
		}
		return new string(array);
	}

	private static void OnIndentSelection(object target, ExecutedRoutedEventArgs args)
	{
		TextArea textArea = GetTextArea(target);
		if (textArea == null || textArea.Document == null || textArea.IndentationStrategy == null)
		{
			return;
		}
		using (textArea.Document.RunUpdate())
		{
			int beginLine;
			int endLine;
			if (textArea.Selection.IsEmpty)
			{
				beginLine = 1;
				endLine = textArea.Document.LineCount;
			}
			else
			{
				beginLine = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.Offset).LineNumber;
				endLine = textArea.Document.GetLineByOffset(textArea.Selection.SurroundingSegment.EndOffset).LineNumber;
			}
			textArea.IndentationStrategy.IndentLines(textArea.Document, beginLine, endLine);
		}
		textArea.Caret.BringCaretToView();
		args.Handled = true;
	}
}
