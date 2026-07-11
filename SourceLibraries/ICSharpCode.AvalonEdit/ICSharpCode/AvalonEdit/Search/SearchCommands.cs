using System.Windows.Input;

namespace ICSharpCode.AvalonEdit.Search;

public static class SearchCommands
{
	public static readonly RoutedCommand FindNext = new RoutedCommand("FindNext", typeof(SearchPanel), new InputGestureCollection
	{
		new KeyGesture(Key.F3)
	});

	public static readonly RoutedCommand FindPrevious = new RoutedCommand("FindPrevious", typeof(SearchPanel), new InputGestureCollection
	{
		new KeyGesture(Key.F3, ModifierKeys.Shift)
	});

	public static readonly RoutedCommand CloseSearchPanel = new RoutedCommand("CloseSearchPanel", typeof(SearchPanel), new InputGestureCollection
	{
		new KeyGesture(Key.Escape)
	});
}
