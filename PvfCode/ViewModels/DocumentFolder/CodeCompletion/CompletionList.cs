using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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
	[CompilerGenerated]
	private bool ysZiOyuxfO;

	public static readonly DependencyProperty EmptyTemplateProperty;

	[CompilerGenerated]
	private EventHandler xASiKntf83;

	[CompilerGenerated]
	private EventHandler wLoi9E8jSH;

	private CompletionListBox listBox;

	[CompilerGenerated]
	private ObservableCollection<CodeCompletionData> dvriP9mJdZ;

	private string HG9iZIoAd3;

	private ObservableCollection<CodeCompletionData> hbiiJc0u6m;

	public bool IsFiltering
	{
		get
		{
			return qTEib8CBNP();
		}
		set
		{
			F9diIEXhqO(value);
		}
	}

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

	public ObservableCollection<CodeCompletionData> CompletionData
	{
		[CompilerGenerated]
		get
		{
			return dvriP9mJdZ;
		}
		[CompilerGenerated]
		set
		{
			dvriP9mJdZ = value;
		}
	}

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

	public event EventHandler InsertionRequested
	{
		[CompilerGenerated]
		add
		{
			EventHandler eventHandler = xASiKntf83;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref xASiKntf83, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler eventHandler = xASiKntf83;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref xASiKntf83, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public event EventHandler CloseCompletionWindow
	{
		[CompilerGenerated]
		add
		{
			EventHandler eventHandler = wLoi9E8jSH;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref wLoi9E8jSH, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler eventHandler = wLoi9E8jSH;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref wLoi9E8jSH, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

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

	[SpecialName]
	[CompilerGenerated]
	private bool qTEib8CBNP()
	{
		return ysZiOyuxfO;
	}

	[SpecialName]
	[CompilerGenerated]
	private void F9diIEXhqO(bool P_0)
	{
		ysZiOyuxfO = P_0;
	}

	public void RequestInsertion(EventArgs e)
	{
		if (xASiKntf83 != null)
		{
			xASiKntf83(this, e);
		}
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
			Key key = e.Key;
			if ((int)key != 3 && (int)key != 6)
			{
				switch ((int)key - 19)
				{
				case 7:
					e.Handled = true;
					listBox.SelectIndex(listBox.SelectedIndex + 1);
					break;
				case 5:
					e.Handled = true;
					listBox.SelectIndex(listBox.SelectedIndex - 1);
					break;
				case 1:
					e.Handled = true;
					listBox.SelectIndex(listBox.SelectedIndex + listBox.VisibleItemCount);
					break;
				case 0:
					e.Handled = true;
					listBox.SelectIndex(listBox.SelectedIndex - listBox.VisibleItemCount);
					break;
				case 3:
					e.Handled = true;
					listBox.SelectIndex(0);
					break;
				case 2:
					e.Handled = true;
					listBox.SelectIndex(listBox.Items.Count - 1);
					break;
				case 4:
				case 6:
					break;
				}
			}
			else
			{
				e.Handled = true;
				RequestInsertion(e);
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
				if (((DependencyObject)((originalSource is DependencyObject) ? originalSource : null)).VisualAncestorsAndSelf().TakeWhile((DependencyObject P_0) => (object)P_0 != this).Any((DependencyObject obj) => obj is ListBoxItem))
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
			if (!(text == HG9iZIoAd3))
			{
				if (listBox == null)
				{
					ApplyTemplate();
				}
				if (IsFiltering)
				{
					CHhisdMFKx(text);
				}
				else
				{
					hoYin8U8G0(text);
				}
				HG9iZIoAd3 = text;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.SelectItem");
		}
	}

	private void CHhisdMFKx(string P_0)
	{
		try
		{
			var enumerable = from item in (hbiiJc0u6m != null && !string.IsNullOrEmpty(HG9iZIoAd3) && !string.IsNullOrEmpty(P_0) && P_0.StartsWith(HG9iZIoAd3, StringComparison.Ordinal)) ? hbiiJc0u6m : CompletionData
				let quality = J8cidywF32(item.Text, P_0)
				where quality > 0
				select new
				{
					Item = item,
					Quality = quality
				};
			var similarityResultInfo = enumerable.Similarity(it => it.Item.Text, P_0);
			CodeCompletionData codeCompletionData;
			if (similarityResultInfo == null || similarityResultInfo.SimilarityTargetList == null || !similarityResultInfo.SimilarityTargetList.Any())
			{
				wLoi9E8jSH?.Invoke(this, null);
				codeCompletionData = ((listBox.SelectedIndex != -1) ? ((CodeCompletionData)listBox.Items[listBox.SelectedIndex]) : null);
			}
			else
			{
				codeCompletionData = similarityResultInfo.SimilarityTargetList.FirstOrDefault().Item;
			}
			ObservableCollection<CodeCompletionData> observableCollection = new ObservableCollection<CodeCompletionData>();
			int num = -1;
			int num2 = -1;
			double num3 = 0.0;
			int num4 = 0;
			foreach (var item in enumerable)
			{
				double num5 = ((item.Item == codeCompletionData) ? double.PositiveInfinity : item.Item.Priority);
				int quality = item.Quality;
				if (quality > num2 || (quality == num2 && num5 > num3))
				{
					num = num4;
					num3 = num5;
					num2 = quality;
				}
				observableCollection.Add(item.Item);
				num4++;
			}
			hbiiJc0u6m = observableCollection;
			listBox.ItemsSource = observableCollection;
			q6SiqxJLeH(num);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.SelectItemFiltering2");
		}
	}

	private void hgYiLxA2mk(string P_0)
	{
		var enumerable = from item in (hbiiJc0u6m != null && !string.IsNullOrEmpty(HG9iZIoAd3) && !string.IsNullOrEmpty(P_0) && P_0.StartsWith(HG9iZIoAd3, StringComparison.Ordinal)) ? hbiiJc0u6m : CompletionData
			let quality = J8cidywF32(item.Text, P_0)
			where quality > 0
			select new
			{
				Item = item,
				Quality = quality
			};
		CodeCompletionData codeCompletionData = ((listBox.SelectedIndex != -1) ? ((CodeCompletionData)listBox.Items[listBox.SelectedIndex]) : null);
		ObservableCollection<CodeCompletionData> observableCollection = new ObservableCollection<CodeCompletionData>();
		int num = -1;
		int num2 = -1;
		double num3 = 0.0;
		int num4 = 0;
		foreach (var item in enumerable)
		{
			double num5 = ((item.Item == codeCompletionData) ? double.PositiveInfinity : item.Item.Priority);
			int quality = item.Quality;
			if (quality > num2 || (quality == num2 && num5 > num3))
			{
				num = num4;
				num3 = num5;
				num2 = quality;
			}
			observableCollection.Add(item.Item);
			num4++;
		}
		hbiiJc0u6m = observableCollection;
		listBox.ItemsSource = observableCollection;
		q6SiqxJLeH(num);
	}

	private void hoYin8U8G0(string P_0)
	{
		if (string.IsNullOrEmpty(P_0))
		{
			return;
		}
		try
		{
			int selectedIndex = listBox.SelectedIndex;
			int num = -1;
			int num2 = -1;
			double num3 = 0.0;
			for (int i = 0; i < CompletionData.Count; i++)
			{
				int num4 = J8cidywF32(CompletionData[i].Text, P_0);
				if (num4 >= 0)
				{
					double priority = CompletionData[i].Priority;
					if (num2 < num4 || (num != selectedIndex && ((i != selectedIndex) ? (num2 == num4 && num3 < priority) : (num2 == num4))))
					{
						num = i;
						num3 = priority;
						num2 = num4;
					}
				}
			}
			q6SiqxJLeH(num);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.SelectItemWithStart");
		}
	}

	private void q6SiqxJLeH(int P_0)
	{
		try
		{
			if (P_0 < 0)
			{
				listBox.ClearSelection();
				return;
			}
			int firstVisibleItem = listBox.FirstVisibleItem;
			if (P_0 < firstVisibleItem || firstVisibleItem + listBox.VisibleItemCount <= P_0)
			{
				listBox.CenterViewOn(P_0);
				listBox.SelectIndex(P_0);
			}
			else
			{
				listBox.SelectIndex(P_0);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "CompletionList.SelectIndexCentered");
		}
	}

	private int J8cidywF32(string P_0, string P_1)
	{
		try
		{
			if (P_0 == null)
			{
				throw new ArgumentNullException("itemText", "ICompletionData.Text returned null");
			}
			if (P_1 == P_0)
			{
				return 8;
			}
			if (string.Equals(P_0, P_1, StringComparison.InvariantCultureIgnoreCase))
			{
				return 7;
			}
			if (P_0.StartsWith(P_1, StringComparison.InvariantCulture))
			{
				return 6;
			}
			if (P_0.StartsWith(P_1, StringComparison.InvariantCultureIgnoreCase))
			{
				return 5;
			}
			bool? flag = null;
			if (P_1.Length <= 2)
			{
				flag = NtuieoYvb8(P_0, P_1);
				if (flag == true)
				{
					return 4;
				}
			}
			if (IsFiltering)
			{
				if (P_0.IndexOf(P_1, StringComparison.InvariantCulture) >= 0)
				{
					return 3;
				}
				if (P_0.IndexOf(P_1, StringComparison.InvariantCultureIgnoreCase) >= 0)
				{
					return 2;
				}
			}
			if (!flag.HasValue)
			{
				flag = NtuieoYvb8(P_0, P_1);
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

	private static bool NtuieoYvb8(string P_0, string P_1)
	{
		try
		{
			IEnumerable<char> enumerable = P_0.Take(1).Concat(P_0.Skip(1).Where(char.IsUpper));
			int num = 0;
			foreach (char item in enumerable)
			{
				if (num > P_1.Length - 1)
				{
					return true;
				}
				if (char.ToUpperInvariant(P_1[num]) != char.ToUpperInvariant(item))
				{
					return false;
				}
				num++;
			}
			if (num >= P_1.Length)
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
		ysZiOyuxfO = true;
		dvriP9mJdZ = new ObservableCollection<CodeCompletionData>();
	}

	[CompilerGenerated]
	private bool rc8ityI5ia(DependencyObject P_0)
	{
		return (object)P_0 != this;
	}
}
