using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Editing;

namespace ICSharpCode.AvalonEdit.Search;

public class SearchInputHandler : TextAreaInputHandler
{
	private SearchPanel panel;

	public event EventHandler<SearchOptionsChangedEventArgs> SearchOptionsChanged
	{
		add
		{
			panel.SearchOptionsChanged += value;
		}
		remove
		{
			panel.SearchOptionsChanged -= value;
		}
	}

	internal SearchInputHandler(TextArea textArea, SearchPanel panel)
		: base(textArea)
	{
		RegisterCommands(base.CommandBindings);
		this.panel = panel;
	}

	internal void RegisterGlobalCommands(CommandBindingCollection commandBindings)
	{
		commandBindings.Add(new CommandBinding(ApplicationCommands.Find, ExecuteFind));
		commandBindings.Add(new CommandBinding(SearchCommands.FindNext, ExecuteFindNext, CanExecuteWithOpenSearchPanel));
		commandBindings.Add(new CommandBinding(SearchCommands.FindPrevious, ExecuteFindPrevious, CanExecuteWithOpenSearchPanel));
	}

	private void RegisterCommands(ICollection<CommandBinding> commandBindings)
	{
		commandBindings.Add(new CommandBinding(ApplicationCommands.Find, ExecuteFind));
		commandBindings.Add(new CommandBinding(SearchCommands.FindNext, ExecuteFindNext, CanExecuteWithOpenSearchPanel));
		commandBindings.Add(new CommandBinding(SearchCommands.FindPrevious, ExecuteFindPrevious, CanExecuteWithOpenSearchPanel));
		commandBindings.Add(new CommandBinding(SearchCommands.CloseSearchPanel, ExecuteCloseSearchPanel, CanExecuteWithOpenSearchPanel));
	}

	private void ExecuteFind(object sender, ExecutedRoutedEventArgs e)
	{
		panel.Open();
		if (!base.TextArea.Selection.IsEmpty && !base.TextArea.Selection.IsMultiline)
		{
			panel.SearchPattern = base.TextArea.Selection.GetText();
		}
		Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Input, (Action)delegate
		{
			panel.Reactivate();
		});
	}

	private void CanExecuteWithOpenSearchPanel(object sender, CanExecuteRoutedEventArgs e)
	{
		if (panel.IsClosed)
		{
			e.CanExecute = false;
			e.ContinueRouting = true;
		}
		else
		{
			e.CanExecute = true;
			e.Handled = true;
		}
	}

	private void ExecuteFindNext(object sender, ExecutedRoutedEventArgs e)
	{
		if (!panel.IsClosed)
		{
			panel.FindNext();
			e.Handled = true;
		}
	}

	private void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e)
	{
		if (!panel.IsClosed)
		{
			panel.FindPrevious();
			e.Handled = true;
		}
	}

	private void ExecuteCloseSearchPanel(object sender, ExecutedRoutedEventArgs e)
	{
		if (!panel.IsClosed)
		{
			panel.Close();
			e.Handled = true;
		}
	}
}
