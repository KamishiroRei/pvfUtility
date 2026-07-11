using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Editing;

public class TextAreaDefaultInputHandler : TextAreaInputHandler
{
	public TextAreaInputHandler CaretNavigation { get; private set; }

	public TextAreaInputHandler Editing { get; private set; }

	public ITextAreaInputHandler MouseSelection { get; private set; }

	public TextAreaDefaultInputHandler(TextArea textArea)
		: base(textArea)
	{
		base.NestedInputHandlers.Add(CaretNavigation = CaretNavigationCommandHandler.Create(textArea));
		base.NestedInputHandlers.Add(Editing = EditingCommandHandler.Create(textArea));
		base.NestedInputHandlers.Add(MouseSelection = new SelectionMouseHandler(textArea));
		base.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, ExecuteUndo, CanExecuteUndo));
		base.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, ExecuteRedo, CanExecuteRedo));
	}

	internal static KeyBinding CreateFrozenKeyBinding(ICommand command, ModifierKeys modifiers, Key key)
	{
		KeyBinding keyBinding = new KeyBinding(command, key, modifiers);
		if (keyBinding is Freezable freezable)
		{
			freezable.Freeze();
		}
		return keyBinding;
	}

	internal static void WorkaroundWPFMemoryLeak(List<InputBinding> inputBindings)
	{
		new UIElement().InputBindings.AddRange(inputBindings);
	}

	private UndoStack GetUndoStack()
	{
		return base.TextArea.Document?.UndoStack;
	}

	private void ExecuteUndo(object sender, ExecutedRoutedEventArgs e)
	{
		UndoStack undoStack = GetUndoStack();
		if (undoStack != null)
		{
			if (undoStack.CanUndo)
			{
				undoStack.Undo();
				base.TextArea.Caret.BringCaretToView();
			}
			e.Handled = true;
		}
	}

	private void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
	{
		UndoStack undoStack = GetUndoStack();
		if (undoStack != null)
		{
			e.Handled = true;
			e.CanExecute = undoStack.CanUndo;
		}
	}

	private void ExecuteRedo(object sender, ExecutedRoutedEventArgs e)
	{
		UndoStack undoStack = GetUndoStack();
		if (undoStack != null)
		{
			if (undoStack.CanRedo)
			{
				undoStack.Redo();
				base.TextArea.Caret.BringCaretToView();
			}
			e.Handled = true;
		}
	}

	private void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
	{
		UndoStack undoStack = GetUndoStack();
		if (undoStack != null)
		{
			e.Handled = true;
			e.CanExecute = undoStack.CanRedo;
		}
	}
}
