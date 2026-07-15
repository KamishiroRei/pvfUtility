using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip;

namespace PvfCode.Controls;

public class PvfCommentTooltipView : Border
{
	private const double CompactReadWidth = 280;
	private const double StandardMinWidth = 420;
	private ToolTipViewModel_SectionComment viewModel;

	public PvfCommentTooltipView()
	{
		MaxWidth = 720;
		MaxHeight = 760;
		Padding = new Thickness(1);
		BorderThickness = new Thickness(1);
		SetResourceReference(BorderBrushProperty, "EditorFoldingMarkerBrush");
		SetResourceReference(BackgroundProperty, "EditorBackground");
		Grid root = new();
		root.Children.Add(CreateReadPanel());
		Child = root;
		DataContextChanged += OnDataContextChanged;
		UpdateWidth();
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
		TextBlock metadata = new() { VerticalAlignment = VerticalAlignment.Center };
		metadata.SetResourceReference(TextBlock.ForegroundProperty, "EditorFoldingMarkerBrush");
		metadata.SetBinding(TextBlock.TextProperty, new Binding("Comment.Authors") { StringFormat = "Author: {0}" });
		footer.Children.Add(metadata);
		Button edit = new()
		{
			Content = "Edit",
			MinWidth = 72,
			Margin = new Thickness(12, 0, 0, 0),
			ToolTip = "Open the tag editor"
		};
		DockPanel.SetDock(edit, Dock.Right);
		edit.Click += (_, _) => viewModel?.OpenEditor();
		footer.Children.Insert(0, edit);
		Grid.SetRow(footer, 1);
		panel.Children.Add(footer);
		return panel;
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
		UpdateWidth();
	}

	private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
	{
		if (args.PropertyName == nameof(ToolTipViewModel_SectionComment.Comment))
		{
			UpdateWidth();
		}
	}

	private void UpdateWidth()
	{
		bool hasReadableContent = !string.IsNullOrWhiteSpace(viewModel?.Comment?.Comment) ||
			!string.IsNullOrWhiteSpace(viewModel?.Comment?.OfficialDescription);
		if (hasReadableContent)
		{
			Width = double.NaN;
			MinWidth = StandardMinWidth;
			return;
		}
		MinWidth = CompactReadWidth;
		Width = CompactReadWidth;
	}
}
