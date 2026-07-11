using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Search;

public class SearchPanel : Control
{
	private TextArea textArea;

	private SearchInputHandler handler;

	private TextDocument currentDocument;

	private SearchResultBackgroundRenderer renderer;

	private TextBox searchTextBox;

	private Popup dropdownPopup;

	private SearchPanelAdorner adorner;

	public static readonly DependencyProperty UseRegexProperty;

	public static readonly DependencyProperty MatchCaseProperty;

	public static readonly DependencyProperty WholeWordsProperty;

	public static readonly DependencyProperty SearchPatternProperty;

	public static readonly DependencyProperty MarkerBrushProperty;

	public static readonly DependencyProperty MarkerPenProperty;

	public static readonly DependencyProperty MarkerCornerRadiusProperty;

	public static readonly DependencyProperty LocalizationProperty;

	private ISearchStrategy strategy;

	private ToolTip messageView = new ToolTip
	{
		Placement = PlacementMode.Bottom,
		StaysOpen = true,
		Focusable = false
	};

	public bool UseRegex
	{
		get
		{
			return (bool)GetValue(UseRegexProperty);
		}
		set
		{
			SetValue(UseRegexProperty, value);
		}
	}

	public bool MatchCase
	{
		get
		{
			return (bool)GetValue(MatchCaseProperty);
		}
		set
		{
			SetValue(MatchCaseProperty, value);
		}
	}

	public bool WholeWords
	{
		get
		{
			return (bool)GetValue(WholeWordsProperty);
		}
		set
		{
			SetValue(WholeWordsProperty, value);
		}
	}

	public string SearchPattern
	{
		get
		{
			return (string)GetValue(SearchPatternProperty);
		}
		set
		{
			SetValue(SearchPatternProperty, value);
		}
	}

	public Brush MarkerBrush
	{
		get
		{
			return (Brush)GetValue(MarkerBrushProperty);
		}
		set
		{
			SetValue(MarkerBrushProperty, value);
		}
	}

	public Pen MarkerPen
	{
		get
		{
			return (Pen)GetValue(MarkerPenProperty);
		}
		set
		{
			SetValue(MarkerPenProperty, value);
		}
	}

	public double MarkerCornerRadius
	{
		get
		{
			return (double)GetValue(MarkerCornerRadiusProperty);
		}
		set
		{
			SetValue(MarkerCornerRadiusProperty, value);
		}
	}

	public Localization Localization
	{
		get
		{
			return (Localization)GetValue(LocalizationProperty);
		}
		set
		{
			SetValue(LocalizationProperty, value);
		}
	}

	public bool IsClosed { get; private set; }

	public event EventHandler<SearchOptionsChangedEventArgs> SearchOptionsChanged;

	private static void MarkerBrushChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is SearchPanel searchPanel)
		{
			searchPanel.renderer.MarkerBrush = (Brush)e.NewValue;
		}
	}

	private static void MarkerPenChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is SearchPanel searchPanel)
		{
			searchPanel.renderer.MarkerPen = (Pen)e.NewValue;
		}
	}

	private static void MarkerCornerRadiusChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is SearchPanel searchPanel)
		{
			searchPanel.renderer.MarkerCornerRadius = (double)e.NewValue;
		}
	}

	static SearchPanel()
	{
		UseRegexProperty = DependencyProperty.Register("UseRegex", typeof(bool), typeof(SearchPanel), new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));
		MatchCaseProperty = DependencyProperty.Register("MatchCase", typeof(bool), typeof(SearchPanel), new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));
		WholeWordsProperty = DependencyProperty.Register("WholeWords", typeof(bool), typeof(SearchPanel), new FrameworkPropertyMetadata(false, SearchPatternChangedCallback));
		SearchPatternProperty = DependencyProperty.Register("SearchPattern", typeof(string), typeof(SearchPanel), new FrameworkPropertyMetadata("", SearchPatternChangedCallback));
		MarkerBrushProperty = DependencyProperty.Register("MarkerBrush", typeof(Brush), typeof(SearchPanel), new FrameworkPropertyMetadata(Brushes.LightGreen, MarkerBrushChangedCallback));
		MarkerPenProperty = DependencyProperty.Register("MarkerPen", typeof(Pen), typeof(SearchPanel), new PropertyMetadata(null, MarkerPenChangedCallback));
		MarkerCornerRadiusProperty = DependencyProperty.Register("MarkerCornerRadius", typeof(double), typeof(SearchPanel), new PropertyMetadata(3.0, MarkerCornerRadiusChangedCallback));
		LocalizationProperty = DependencyProperty.Register("Localization", typeof(Localization), typeof(SearchPanel), new FrameworkPropertyMetadata(new Localization()));
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchPanel), new FrameworkPropertyMetadata(typeof(SearchPanel)));
	}

	private static void SearchPatternChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is SearchPanel searchPanel)
		{
			searchPanel.ValidateSearchText();
			searchPanel.UpdateSearch();
		}
	}

	private void UpdateSearch()
	{
		if (renderer.CurrentResults.Any())
		{
			messageView.IsOpen = false;
		}
		strategy = SearchStrategyFactory.Create(SearchPattern ?? "", !MatchCase, WholeWords, UseRegex ? SearchMode.RegEx : SearchMode.Normal);
		OnSearchOptionsChanged(new SearchOptionsChangedEventArgs(SearchPattern, MatchCase, UseRegex, WholeWords));
		DoSearch(changeSelection: true);
	}

	private SearchPanel()
	{
	}

	public static SearchPanel Install(TextEditor editor)
	{
		if (editor == null)
		{
			throw new ArgumentNullException("editor");
		}
		return Install(editor.TextArea);
	}

	public static SearchPanel Install(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		SearchPanel searchPanel = new SearchPanel();
		searchPanel.AttachInternal(textArea);
		searchPanel.handler = new SearchInputHandler(textArea, searchPanel);
		textArea.DefaultInputHandler.NestedInputHandlers.Add(searchPanel.handler);
		return searchPanel;
	}

	public void RegisterCommands(CommandBindingCollection commandBindings)
	{
		handler.RegisterGlobalCommands(commandBindings);
	}

	public void Uninstall()
	{
		Close();
		textArea.DocumentChanged -= textArea_DocumentChanged;
		if (currentDocument != null)
		{
			currentDocument.TextChanged -= textArea_Document_TextChanged;
		}
		textArea.DefaultInputHandler.NestedInputHandlers.Remove(handler);
	}

	private void AttachInternal(TextArea textArea)
	{
		this.textArea = textArea;
		adorner = new SearchPanelAdorner(textArea, this);
		base.DataContext = this;
		renderer = new SearchResultBackgroundRenderer();
		currentDocument = textArea.Document;
		if (currentDocument != null)
		{
			currentDocument.TextChanged += textArea_Document_TextChanged;
		}
		textArea.DocumentChanged += textArea_DocumentChanged;
		base.KeyDown += SearchLayerKeyDown;
		base.CommandBindings.Add(new CommandBinding(SearchCommands.FindNext, delegate
		{
			FindNext();
		}));
		base.CommandBindings.Add(new CommandBinding(SearchCommands.FindPrevious, delegate
		{
			FindPrevious();
		}));
		base.CommandBindings.Add(new CommandBinding(SearchCommands.CloseSearchPanel, delegate
		{
			Close();
		}));
		IsClosed = true;
	}

	private void textArea_DocumentChanged(object sender, EventArgs e)
	{
		if (currentDocument != null)
		{
			currentDocument.TextChanged -= textArea_Document_TextChanged;
		}
		currentDocument = textArea.Document;
		if (currentDocument != null)
		{
			currentDocument.TextChanged += textArea_Document_TextChanged;
			DoSearch(changeSelection: false);
		}
	}

	private void textArea_Document_TextChanged(object sender, EventArgs e)
	{
		DoSearch(changeSelection: false);
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		searchTextBox = base.Template.FindName("PART_searchTextBox", this) as TextBox;
		dropdownPopup = base.Template.FindName("PART_dropdownPopup", this) as Popup;
	}

	private void ValidateSearchText()
	{
		if (searchTextBox == null)
		{
			return;
		}
		BindingExpression bindingExpression = searchTextBox.GetBindingExpression(TextBox.TextProperty);
		try
		{
			if (bindingExpression != null)
			{
				Validation.ClearInvalid(bindingExpression);
			}
			UpdateSearch();
		}
		catch (SearchPatternException ex)
		{
			ValidationError validationError = new ValidationError(bindingExpression.ParentBinding.ValidationRules[0], bindingExpression, ex.Message, ex);
			Validation.MarkInvalid(bindingExpression, validationError);
		}
	}

	public void Reactivate()
	{
		if (searchTextBox != null)
		{
			searchTextBox.Focus();
			searchTextBox.SelectAll();
		}
	}

	public void FindNext()
	{
		SearchResult searchResult = renderer.CurrentResults.FindFirstSegmentWithStartAfter(textArea.Caret.Offset + 1);
		if (searchResult == null)
		{
			searchResult = renderer.CurrentResults.FirstSegment;
		}
		if (searchResult != null)
		{
			SelectResult(searchResult);
		}
	}

	public void FindPrevious()
	{
		SearchResult searchResult = renderer.CurrentResults.FindFirstSegmentWithStartAfter(textArea.Caret.Offset);
		if (searchResult != null)
		{
			searchResult = renderer.CurrentResults.GetPreviousSegment(searchResult);
		}
		if (searchResult == null)
		{
			searchResult = renderer.CurrentResults.LastSegment;
		}
		if (searchResult != null)
		{
			SelectResult(searchResult);
		}
	}

	private void DoSearch(bool changeSelection)
	{
		if (IsClosed)
		{
			return;
		}
		renderer.CurrentResults.Clear();
		if (!string.IsNullOrEmpty(SearchPattern))
		{
			int offset = textArea.Caret.Offset;
			if (changeSelection)
			{
				textArea.ClearSelection();
			}
			foreach (SearchResult item in strategy.FindAll(textArea.Document, 0, textArea.Document.TextLength))
			{
				if (changeSelection && item.StartOffset >= offset)
				{
					SelectResult(item);
					changeSelection = false;
				}
				renderer.CurrentResults.Add(item);
			}
			if (!renderer.CurrentResults.Any())
			{
				messageView.IsOpen = true;
				messageView.Content = Localization.NoMatchesFoundText;
				messageView.PlacementTarget = searchTextBox;
			}
			else
			{
				messageView.IsOpen = false;
			}
		}
		textArea.TextView.InvalidateLayer(KnownLayer.Selection);
	}

	private void SelectResult(SearchResult result)
	{
		textArea.Caret.Offset = result.StartOffset;
		textArea.Selection = Selection.Create(textArea, result.StartOffset, result.EndOffset);
		textArea.Caret.BringCaretToView();
		textArea.Caret.Show();
	}

	private void SearchLayerKeyDown(object sender, KeyEventArgs e)
	{
		switch (e.Key)
		{
		case Key.Return:
			e.Handled = true;
			if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
			{
				FindPrevious();
			}
			else
			{
				FindNext();
			}
			if (searchTextBox != null)
			{
				ValidationError validationError = Validation.GetErrors(searchTextBox).FirstOrDefault();
				if (validationError != null)
				{
					messageView.Content = Localization.ErrorText + " " + validationError.ErrorContent;
					messageView.PlacementTarget = searchTextBox;
					messageView.IsOpen = true;
				}
			}
			break;
		case Key.Escape:
			e.Handled = true;
			Close();
			break;
		}
	}

	public void Close()
	{
		bool isKeyboardFocusWithin = base.IsKeyboardFocusWithin;
		AdornerLayer.GetAdornerLayer(textArea)?.Remove(adorner);
		if (dropdownPopup != null)
		{
			dropdownPopup.IsOpen = false;
		}
		messageView.IsOpen = false;
		textArea.TextView.BackgroundRenderers.Remove(renderer);
		if (isKeyboardFocusWithin)
		{
			textArea.Focus();
		}
		IsClosed = true;
		renderer.CurrentResults.Clear();
	}

	public void Open()
	{
		if (IsClosed)
		{
			AdornerLayer.GetAdornerLayer(textArea)?.Add(adorner);
			textArea.TextView.BackgroundRenderers.Add(renderer);
			IsClosed = false;
			DoSearch(changeSelection: false);
		}
	}

	protected virtual void OnSearchOptionsChanged(SearchOptionsChangedEventArgs e)
	{
		if (this.SearchOptionsChanged != null)
		{
			this.SearchOptionsChanged(this, e);
		}
	}
}
