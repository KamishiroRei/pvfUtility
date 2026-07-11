using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.CodeCompletion;

public class CompletionList : Control
{
	private bool isFiltering = true;

	public static readonly DependencyProperty EmptyTemplateProperty;

	private CompletionListBox listBox;

	private ObservableCollection<ICompletionData> completionData = new ObservableCollection<ICompletionData>();

	private string currentText;

	private ObservableCollection<ICompletionData> currentList;

	public bool IsFiltering
	{
		get
		{
			return isFiltering;
		}
		set
		{
			isFiltering = value;
		}
	}

	public ControlTemplate EmptyTemplate
	{
		get
		{
			return (ControlTemplate)GetValue(EmptyTemplateProperty);
		}
		set
		{
			SetValue(EmptyTemplateProperty, value);
		}
	}

	public CompletionListBox ListBox
	{
		get
		{
			if (listBox == null)
			{
				ApplyTemplate();
			}
			return listBox;
		}
	}

	public ScrollViewer ScrollViewer
	{
		get
		{
			if (listBox == null)
			{
				return null;
			}
			return listBox.scrollViewer;
		}
	}

	public IList<ICompletionData> CompletionData => completionData;

	public ICompletionData SelectedItem
	{
		get
		{
			return ((listBox != null) ? listBox.SelectedItem : null) as ICompletionData;
		}
		set
		{
			if (listBox == null && value != null)
			{
				ApplyTemplate();
			}
			if (listBox != null)
			{
				listBox.SelectedItem = value;
			}
		}
	}

	public event EventHandler InsertionRequested;

	public event SelectionChangedEventHandler SelectionChanged
	{
		add
		{
			AddHandler(Selector.SelectionChangedEvent, value);
		}
		remove
		{
			RemoveHandler(Selector.SelectionChangedEvent, value);
		}
	}

	static CompletionList()
	{
		EmptyTemplateProperty = DependencyProperty.Register("EmptyTemplate", typeof(ControlTemplate), typeof(CompletionList), new FrameworkPropertyMetadata());
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CompletionList), new FrameworkPropertyMetadata(typeof(CompletionList)));
	}

	public void RequestInsertion(EventArgs e)
	{
		if (this.InsertionRequested != null)
		{
			this.InsertionRequested(this, e);
		}
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		listBox = GetTemplateChild("PART_ListBox") as CompletionListBox;
		if (listBox != null)
		{
			listBox.ItemsSource = completionData;
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		base.OnKeyDown(e);
		if (!e.Handled)
		{
			HandleKey(e);
		}
	}

	public void HandleKey(KeyEventArgs e)
	{
		if (listBox != null)
		{
			switch (e.Key)
			{
			case Key.Down:
				e.Handled = true;
				listBox.SelectIndex(listBox.SelectedIndex + 1);
				break;
			case Key.Up:
				e.Handled = true;
				listBox.SelectIndex(listBox.SelectedIndex - 1);
				break;
			case Key.Next:
				e.Handled = true;
				listBox.SelectIndex(listBox.SelectedIndex + listBox.VisibleItemCount);
				break;
			case Key.Prior:
				e.Handled = true;
				listBox.SelectIndex(listBox.SelectedIndex - listBox.VisibleItemCount);
				break;
			case Key.Home:
				e.Handled = true;
				listBox.SelectIndex(0);
				break;
			case Key.End:
				e.Handled = true;
				listBox.SelectIndex(listBox.Items.Count - 1);
				break;
			case Key.Tab:
			case Key.Return:
				e.Handled = true;
				RequestInsertion(e);
				break;
			}
		}
	}

	protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
	{
		base.OnMouseDoubleClick(e);
		if (e.ChangedButton == MouseButton.Left && (e.OriginalSource as DependencyObject).VisualAncestorsAndSelf().TakeWhile((DependencyObject obj) => obj != this).Any((DependencyObject obj) => obj is ListBoxItem))
		{
			e.Handled = true;
			RequestInsertion(e);
		}
	}

	public void ScrollIntoView(ICompletionData item)
	{
		if (listBox == null)
		{
			ApplyTemplate();
		}
		if (listBox != null)
		{
			listBox.ScrollIntoView(item);
		}
	}

	public void SelectItem(string text)
	{
		if (!(text == currentText))
		{
			if (listBox == null)
			{
				ApplyTemplate();
			}
			if (IsFiltering)
			{
				SelectItemFiltering(text);
			}
			else
			{
				SelectItemWithStart(text);
			}
			currentText = text;
		}
	}

	private void SelectItemFiltering(string query)
	{
		var enumerable = from item in (currentList != null && !string.IsNullOrEmpty(currentText) && !string.IsNullOrEmpty(query) && query.StartsWith(currentText, StringComparison.Ordinal)) ? currentList : this.completionData
			let quality = GetMatchQuality(item.Text, query)
			where quality > 0
			select new
			{
				Item = item,
				Quality = quality
			};
		ICompletionData completionData = ((listBox.SelectedIndex != -1) ? ((ICompletionData)listBox.Items[listBox.SelectedIndex]) : null);
		ObservableCollection<ICompletionData> observableCollection = new ObservableCollection<ICompletionData>();
		int bestIndex = -1;
		int num = -1;
		double num2 = 0.0;
		int num3 = 0;
		foreach (var item in enumerable)
		{
			double num4 = ((item.Item == completionData) ? double.PositiveInfinity : item.Item.Priority);
			int quality = item.Quality;
			if (quality > num || (quality == num && num4 > num2))
			{
				bestIndex = num3;
				num2 = num4;
				num = quality;
			}
			observableCollection.Add(item.Item);
			num3++;
		}
		currentList = observableCollection;
		listBox.ItemsSource = observableCollection;
		SelectIndexCentered(bestIndex);
	}

	private void SelectItemWithStart(string query)
	{
		if (string.IsNullOrEmpty(query))
		{
			return;
		}
		int selectedIndex = listBox.SelectedIndex;
		int num = -1;
		int num2 = -1;
		double num3 = 0.0;
		for (int i = 0; i < completionData.Count; i++)
		{
			int matchQuality = GetMatchQuality(completionData[i].Text, query);
			if (matchQuality >= 0)
			{
				double priority = completionData[i].Priority;
				if (num2 < matchQuality || (num != selectedIndex && ((i != selectedIndex) ? (num2 == matchQuality && num3 < priority) : (num2 == matchQuality))))
				{
					num = i;
					num3 = priority;
					num2 = matchQuality;
				}
			}
		}
		SelectIndexCentered(num);
	}

	private void SelectIndexCentered(int bestIndex)
	{
		if (bestIndex < 0)
		{
			listBox.ClearSelection();
			return;
		}
		int firstVisibleItem = listBox.FirstVisibleItem;
		if (bestIndex < firstVisibleItem || firstVisibleItem + listBox.VisibleItemCount <= bestIndex)
		{
			listBox.CenterViewOn(bestIndex);
			listBox.SelectIndex(bestIndex);
		}
		else
		{
			listBox.SelectIndex(bestIndex);
		}
	}

	private int GetMatchQuality(string itemText, string query)
	{
		if (itemText == null)
		{
			throw new ArgumentNullException("itemText", "ICompletionData.Text returned null");
		}
		if (query == itemText)
		{
			return 8;
		}
		if (string.Equals(itemText, query, StringComparison.InvariantCultureIgnoreCase))
		{
			return 7;
		}
		if (itemText.StartsWith(query, StringComparison.InvariantCulture))
		{
			return 6;
		}
		if (itemText.StartsWith(query, StringComparison.InvariantCultureIgnoreCase))
		{
			return 5;
		}
		bool? flag = null;
		if (query.Length <= 2)
		{
			flag = CamelCaseMatch(itemText, query);
			if (flag == true)
			{
				return 4;
			}
		}
		if (IsFiltering)
		{
			if (itemText.IndexOf(query, StringComparison.InvariantCulture) >= 0)
			{
				return 3;
			}
			if (itemText.IndexOf(query, StringComparison.InvariantCultureIgnoreCase) >= 0)
			{
				return 2;
			}
		}
		if (!flag.HasValue)
		{
			flag = CamelCaseMatch(itemText, query);
		}
		if (flag == true)
		{
			return 1;
		}
		return -1;
	}

	private static bool CamelCaseMatch(string text, string query)
	{
		IEnumerable<char> enumerable = text.Take(1).Concat(text.Skip(1).Where(char.IsUpper));
		int num = 0;
		foreach (char item in enumerable)
		{
			if (num > query.Length - 1)
			{
				return true;
			}
			if (char.ToUpperInvariant(query[num]) != char.ToUpperInvariant(item))
			{
				return false;
			}
			num++;
		}
		if (num >= query.Length)
		{
			return true;
		}
		return false;
	}
}
