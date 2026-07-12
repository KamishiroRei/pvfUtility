using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Grid;
using PvfCode.Services.PreviewPvfFileFolder;
using Swordfish.NET.Collections;

namespace PvfCode.Views.Preview.Controls;

public class PvfItemAvatarListBoxGroupControl : UserControl, IComponentConnector
{
	public static readonly DependencyProperty ItemsProperty;

	internal GridControl Grid;

	internal GridColumn columnName;

	internal CardView View;

	private bool NLIHVPspDW;

	private ScrollViewer? cardScrollViewer;

	private ListBox? outerListBox;

	public ConcurrentObservableCollection<FileItemIconPreviewViewModel> Items
	{
		get
		{
			return (ConcurrentObservableCollection<FileItemIconPreviewViewModel>)((DependencyObject)this).GetValue(ItemsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemsProperty, (object)value);
		}
	}

	public PvfItemAvatarListBoxGroupControl()
	{
		InitializeComponent();
		Loaded += OnLoaded;
		AddHandler(Mouse.PreviewMouseWheelEvent, new MouseWheelEventHandler(OnPreviewMouseWheel), handledEventsToo: true);
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		ListBoxItem? listBoxItem = FindVisualParent<ListBoxItem>(this);
		if (listBoxItem != null)
		{
			// The original ordinary-booster template caps every group at 400px.
			// Selection boxes can contain enough cards for that cap to hide rows.
			listBoxItem.MaxHeight = double.PositiveInfinity;
		}
		outerListBox = FindVisualParent<ListBox>(this);
		cardScrollViewer = FindVisualChild<ScrollViewer>(Grid);
	}

	private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		if (outerListBox == null)
		{
			return;
		}
		bool innerCanScroll = cardScrollViewer != null && (e.Delta < 0
			? cardScrollViewer.VerticalOffset < cardScrollViewer.ScrollableHeight
			: cardScrollViewer.VerticalOffset > 0);
		if (innerCanScroll)
		{
			return;
		}

		e.Handled = true;
		outerListBox.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
		{
			RoutedEvent = Mouse.MouseWheelEvent,
			Source = this
		});
	}

	private static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
	{
		DependencyObject? current = VisualTreeHelper.GetParent(child);
		while (current != null)
		{
			if (current is T parent)
			{
				return parent;
			}
			current = VisualTreeHelper.GetParent(current);
		}
		return null;
	}

	private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
	{
		for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(parent, i);
			if (child is T result)
			{
				return result;
			}
			T? nested = FindVisualChild<T>(child);
			if (nested != null)
			{
				return nested;
			}
		}
		return null;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!NLIHVPspDW)
		{
			NLIHVPspDW = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/preview/controls/pvfitemavatarlistboxgroupcontrol.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			Grid = (GridControl)target;
			break;
		case 2:
			columnName = (GridColumn)target;
			break;
		case 3:
			View = (CardView)target;
			break;
		default:
			NLIHVPspDW = true;
			break;
		}
	}

	static PvfItemAvatarListBoxGroupControl()
	{
		ItemsProperty = DependencyProperty.Register("Items", typeof(ConcurrentObservableCollection<FileItemIconPreviewViewModel>), typeof(PvfItemAvatarListBoxGroupControl), new PropertyMetadata((PropertyChangedCallback)null));
	}
}
