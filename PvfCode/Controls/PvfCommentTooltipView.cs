using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip;

namespace PvfCode.Controls;

public class PvfCommentTooltipView : Border
{
	private static readonly SolidColorBrush TooltipBackground = CreateTooltipBackground();

	private readonly Grid readPanel;
	private readonly Grid editPanel;
	private ToolTipViewModel_SectionComment viewModel;

	public PvfCommentTooltipView()
	{
		MaxWidth = 720;
		MaxHeight = 760;
		MinWidth = 420;
		Padding = new Thickness(1);
		BorderThickness = new Thickness(1);
		BorderBrush = Brushes.Gray;
		Background = TooltipBackground;
		Grid root = new();
		readPanel = CreateReadPanel();
		editPanel = CreateEditPanel();
		root.Children.Add(readPanel);
		root.Children.Add(editPanel);
		Child = root;
		DataContextChanged += OnDataContextChanged;
		UpdateMode();
	}

	private static SolidColorBrush CreateTooltipBackground()
	{
		SolidColorBrush brush = new(Color.FromRgb(0x2D, 0x2D, 0x30));
		brush.Freeze();
		return brush;
	}

	private Grid CreateReadPanel()
	{
		Grid panel = new();
		panel.RowDefinitions.Add(new RowDefinition());
		panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		MarkdownDocumentViewer viewer = new();
		viewer.SetBinding(MarkdownDocumentViewer.TitleProperty, new Binding("Comment.Title"));
		viewer.SetBinding(MarkdownDocumentViewer.MarkdownProperty, new Binding("Comment.Comment"));
		viewer.SetBinding(MarkdownDocumentViewer.OfficialDescriptionProperty, new Binding("Comment.OfficialDescription"));
		panel.Children.Add(viewer);
		DockPanel footer = new() { Margin = new Thickness(10, 4, 10, 8) };
		TextBlock metadata = new() { Foreground = System.Windows.Media.Brushes.Gray, VerticalAlignment = VerticalAlignment.Center };
		metadata.SetBinding(TextBlock.TextProperty, new Binding("Comment.Authors") { StringFormat = "Author: {0}" });
		footer.Children.Add(metadata);
		Button edit = new() { Content = "Edit", MinWidth = 72, Margin = new Thickness(12, 0, 0, 0) };
		DockPanel.SetDock(edit, Dock.Right);
		edit.Click += (_, _) =>
		{
			if (viewModel != null)
			{
				viewModel.IsEditing = true;
			}
		};
		footer.Children.Insert(0, edit);
		Grid.SetRow(footer, 1);
		panel.Children.Add(footer);
		return panel;
	}

	private Grid CreateEditPanel()
	{
		Grid panel = new() { Margin = new Thickness(10) };
		panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		panel.RowDefinitions.Add(new RowDefinition());
		panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		panel.RowDefinitions.Add(new RowDefinition());
		panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		Label titleLabel = new() { Content = "Title" };
		panel.Children.Add(titleLabel);
		TextBox title = new() { Margin = new Thickness(0, 0, 0, 8) };
		title.SetBinding(TextBox.TextProperty, EditBinding("Comment.Title"));
		Grid.SetRow(title, 1);
		panel.Children.Add(title);
		Label commentLabel = new() { Content = "Comment (Markdown)" };
		Grid.SetRow(commentLabel, 2);
		panel.Children.Add(commentLabel);
		MarkdownEditorPreview comment = new() { MinHeight = 150 };
		comment.SetBinding(MarkdownEditorPreview.TextProperty, EditBinding("Document.Text"));
		comment.SetBinding(MarkdownEditorPreview.PreviewTitleProperty, new Binding("Comment.Title"));
		Grid.SetRow(comment, 3);
		panel.Children.Add(comment);
		Label officialLabel = new() { Content = "Official Description (Markdown)" };
		Grid.SetRow(officialLabel, 4);
		panel.Children.Add(officialLabel);
		MarkdownEditorPreview official = new() { MinHeight = 150 };
		official.Margin = new Thickness(0, 8, 0, 8);
		official.SetBinding(MarkdownEditorPreview.TextProperty, EditBinding("Comment.OfficialDescription"));
		Grid.SetRow(official, 5);
		panel.Children.Add(official);
		StackPanel actions = new() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
		Button cancel = new() { Content = "Close editor", MinWidth = 90, Margin = new Thickness(0, 0, 8, 0) };
		cancel.Click += (_, _) =>
		{
			if (viewModel != null)
			{
				viewModel.IsEditing = false;
			}
		};
		Button save = new() { Content = "Save", MinWidth = 72 };
		save.Click += (_, _) => viewModel?.Save();
		actions.Children.Add(cancel);
		actions.Children.Add(save);
		Grid.SetRow(actions, 6);
		panel.Children.Add(actions);
		return panel;
	}

	private static Binding EditBinding(string path)
	{
		return new Binding(path)
		{
			Mode = BindingMode.TwoWay,
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
		};
	}

	private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
	{
		if (viewModel != null)
		{
			viewModel.PropertyChanged -= OnViewModelPropertyChanged;
		}
		viewModel = args.NewValue as ToolTipViewModel_SectionComment;
		if (viewModel != null)
		{
			viewModel.PropertyChanged += OnViewModelPropertyChanged;
		}
		UpdateMode();
	}

	private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
	{
		if (args.PropertyName == nameof(ToolTipViewModel_SectionComment.IsEditing))
		{
			UpdateMode();
		}
	}

	private void UpdateMode()
	{
		bool editing = viewModel?.IsEditing == true;
		readPanel.Visibility = editing ? Visibility.Collapsed : Visibility.Visible;
		editPanel.Visibility = editing ? Visibility.Visible : Visibility.Collapsed;
	}
}
