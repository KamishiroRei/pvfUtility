using System;
using System.Collections.Generic;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

public class TextAreaInputHandler : ITextAreaInputHandler
{
	private readonly ObserveAddRemoveCollection<CommandBinding> commandBindings;

	private readonly ObserveAddRemoveCollection<InputBinding> inputBindings;

	private readonly ObserveAddRemoveCollection<ITextAreaInputHandler> nestedInputHandlers;

	private readonly TextArea textArea;

	private bool isAttached;

	public TextArea TextArea => textArea;

	public bool IsAttached => isAttached;

	public ICollection<CommandBinding> CommandBindings => commandBindings;

	public ICollection<InputBinding> InputBindings => inputBindings;

	public ICollection<ITextAreaInputHandler> NestedInputHandlers => nestedInputHandlers;

	public TextAreaInputHandler(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		this.textArea = textArea;
		commandBindings = new ObserveAddRemoveCollection<CommandBinding>(CommandBinding_Added, CommandBinding_Removed);
		inputBindings = new ObserveAddRemoveCollection<InputBinding>(InputBinding_Added, InputBinding_Removed);
		nestedInputHandlers = new ObserveAddRemoveCollection<ITextAreaInputHandler>(NestedInputHandler_Added, NestedInputHandler_Removed);
	}

	private void CommandBinding_Added(CommandBinding commandBinding)
	{
		if (isAttached)
		{
			textArea.CommandBindings.Add(commandBinding);
		}
	}

	private void CommandBinding_Removed(CommandBinding commandBinding)
	{
		if (isAttached)
		{
			textArea.CommandBindings.Remove(commandBinding);
		}
	}

	private void InputBinding_Added(InputBinding inputBinding)
	{
		if (isAttached)
		{
			textArea.InputBindings.Add(inputBinding);
		}
	}

	private void InputBinding_Removed(InputBinding inputBinding)
	{
		if (isAttached)
		{
			textArea.InputBindings.Remove(inputBinding);
		}
	}

	public void AddBinding(ICommand command, ModifierKeys modifiers, Key key, ExecutedRoutedEventHandler handler)
	{
		CommandBindings.Add(new CommandBinding(command, handler));
		InputBindings.Add(new KeyBinding(command, key, modifiers));
	}

	private void NestedInputHandler_Added(ITextAreaInputHandler handler)
	{
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		if (handler.TextArea != textArea)
		{
			throw new ArgumentException("The nested handler must be working for the same text area!");
		}
		if (isAttached)
		{
			handler.Attach();
		}
	}

	private void NestedInputHandler_Removed(ITextAreaInputHandler handler)
	{
		if (isAttached)
		{
			handler.Detach();
		}
	}

	public virtual void Attach()
	{
		if (isAttached)
		{
			throw new InvalidOperationException("Input handler is already attached");
		}
		isAttached = true;
		textArea.CommandBindings.AddRange(commandBindings);
		textArea.InputBindings.AddRange(inputBindings);
		foreach (ITextAreaInputHandler nestedInputHandler in nestedInputHandlers)
		{
			nestedInputHandler.Attach();
		}
	}

	public virtual void Detach()
	{
		if (!isAttached)
		{
			throw new InvalidOperationException("Input handler is not attached");
		}
		isAttached = false;
		foreach (CommandBinding commandBinding in commandBindings)
		{
			textArea.CommandBindings.Remove(commandBinding);
		}
		foreach (InputBinding inputBinding in inputBindings)
		{
			textArea.InputBindings.Remove(inputBinding);
		}
		foreach (ITextAreaInputHandler nestedInputHandler in nestedInputHandlers)
		{
			nestedInputHandler.Detach();
		}
	}
}
