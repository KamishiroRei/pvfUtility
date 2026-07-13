using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Utils;
using PvfCode.Models.CodeCompletionModels;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class CompletionList : Control
{
	public static readonly DependencyProperty EmptyTemplateProperty;

	private CompletionListBox listBox;

	private string currentText;

	private ObservableCollection<CodeCompletionData> currentList;

	public bool IsFiltering { get; set; }

	public ControlTemplate EmptyTemplate
	{
		get
		{
			return (ControlTemplate)((DependencyObject)this).GetValue(EmptyTemplateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(EmptyTemplateProperty, (object)value);
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
			return listBox.L5uik30Kqg;
		}
	}

	public ObservableCollection<CodeCompletionData> CompletionData { get; set; }

	public CodeCompletionData SelectedItem
	{
		get
		{
			return ((listBox != null) ? listBox.SelectedItem : null) as CodeCompletionData;
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

	public event EventHandler CloseCompletionWindow;

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
		EmptyTemplateProperty = DependencyProperty.Register("EmptyTemplate", typeof(ControlTemplate), typeof(CompletionList), (PropertyMetadata)(object)new FrameworkPropertyMetadata());
		FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CompletionList), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)typeof(CompletionList)));
	}

	public void RequestInsertion(EventArgs e)
	{
		InsertionRequested?.Invoke(this, e);
	}

	public override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		try
		{
			listBox = GetTemplateChild("PART_ListBox") as CompletionListBox;
			if (listBox != null)
			{
				listBox.ItemsSource = CompletionData;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.OnApplyTemplate");
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
		if (listBox == null)
		{
			return;
		}
		try
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
		catch (Exception e2)
		{
			AppCore.Logger.ErrorUploadDialog(e2, "CompletionList.HandleKey");
		}
	}

	protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
	{
		base.OnMouseDoubleClick(e);
		try
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				object originalSource = e.OriginalSource;
				if (((DependencyObject)((originalSource is DependencyObject) ? originalSource : null)).VisualAncestorsAndSelf().TakeWhile(item => item != this).Any(item => item is ListBoxItem))
				{
					e.Handled = true;
					RequestInsertion(e);
				}
			}
		}
		catch (Exception e2)
		{
			AppCore.Logger.ErrorUploadDialog(e2, "CompletionList.OnMouseDoubleClick");
		}
	}

	public void ScrollIntoView(CodeCompletionData item)
	{
		try
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
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.ScrollIntoView");
		}
	}

	public void SelectItem(string text)
	{
		try
		{
			if (text != currentText)
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
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.SelectItem");
		}
	}

	private void SelectItemFiltering(string query)
	{
		try
		{
			var candidates = from item in (currentList != null && !string.IsNullOrEmpty(currentText) && !string.IsNullOrEmpty(query) && query.StartsWith(currentText, StringComparison.Ordinal)) ? currentList : CompletionData
				let quality = GetMatchQuality(item.Text, query)
				where quality > 0
				select new
				{
					Item = item,
					Quality = quality
				};
			var similarityResult = candidates.Similarity(candidate => candidate.Item.Text, query);
			CodeCompletionData selectedItem;
			if (similarityResult == null || similarityResult.SimilarityTargetList == null || !similarityResult.SimilarityTargetList.Any())
			{
				CloseCompletionWindow?.Invoke(this, null);
				selectedItem = listBox.SelectedIndex != -1 ? (CodeCompletionData)listBox.Items[listBox.SelectedIndex] : null;
			}
			else
			{
				selectedItem = similarityResult.SimilarityTargetList.FirstOrDefault().Item;
			}
			ObservableCollection<CodeCompletionData> filteredItems = new ObservableCollection<CodeCompletionData>();
			int bestIndex = -1;
			int bestQuality = -1;
			double bestPriority = 0.0;
			int index = 0;
			foreach (var candidate in candidates)
			{
				double priority = candidate.Item == selectedItem ? double.PositiveInfinity : candidate.Item.Priority;
				if (candidate.Quality > bestQuality || (candidate.Quality == bestQuality && priority > bestPriority))
				{
					bestIndex = index;
					bestPriority = priority;
					bestQuality = candidate.Quality;
				}
				filteredItems.Add(candidate.Item);
				index++;
			}
			currentList = filteredItems;
			listBox.ItemsSource = filteredItems;
			SelectIndexCentered(bestIndex);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.SelectItemFiltering2");
		}
	}

	private void SelectItemWithStart(string query)
	{
		if (string.IsNullOrEmpty(query))
		{
			return;
		}
		try
		{
			int selectedIndex = listBox.SelectedIndex;
			int bestIndex = -1;
			int bestQuality = -1;
			double bestPriority = 0.0;
			for (int i = 0; i < CompletionData.Count; i++)
			{
				int matchQuality = GetMatchQuality(CompletionData[i].Text, query);
				if (matchQuality >= 0)
				{
					double priority = CompletionData[i].Priority;
					if (bestQuality < matchQuality || (bestIndex != selectedIndex && (i != selectedIndex ? bestQuality == matchQuality && bestPriority < priority : bestQuality == matchQuality)))
					{
						bestIndex = i;
						bestPriority = priority;
						bestQuality = matchQuality;
					}
				}
			}
			SelectIndexCentered(bestIndex);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.SelectItemWithStart");
		}
	}

	private void SelectIndexCentered(int bestIndex)
	{
		try
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
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.SelectIndexCentered");
		}
	}

	private int GetMatchQuality(string itemText, string query)
	{
		try
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
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.GetMatchQuality");
		}
		return -1;
	}

	private static bool CamelCaseMatch(string text, string query)
	{
		try
		{
			IEnumerable<char> camelCaseCharacters = text.Take(1).Concat(text.Skip(1).Where(char.IsUpper));
			int num = 0;
			foreach (char character in camelCaseCharacters)
			{
				if (num > query.Length - 1)
				{
					return true;
				}
				if (char.ToUpperInvariant(query[num]) != char.ToUpperInvariant(character))
				{
					return false;
				}
				num++;
			}
			if (num >= query.Length)
			{
				return true;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.CamelCaseMatch");
		}
		return false;
	}

	public CompletionList()
	{
		IsFiltering = true;
		CompletionData = new ObservableCollection<CodeCompletionData>();
	}
}
