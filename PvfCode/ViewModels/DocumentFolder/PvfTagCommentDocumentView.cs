using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using PvfCode.Controls;

namespace PvfCode.ViewModels.DocumentFolder;

public sealed class PvfTagCommentDocumentView : UserControl
{
	private PvfTagCommentDocument document;

	public PvfTagCommentDocumentView()
	{
		Grid root = new();
		root.SetResourceReference(Panel.BackgroundProperty, "EditorBackground");
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		root.RowDefinitions.Add(new RowDefinition());
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		TextBlock target = new()
		{
			Margin = new Thickness(10, 10, 10, 4),
			FontWeight = FontWeights.SemiBold
		};
		target.SetResourceReference(TextBlock.ForegroundProperty, "EditorForeground");
		target.SetBinding(TextBlock.TextProperty, new Binding(nameof(PvfTagCommentDocument.FileName)));
		Grid.SetRow(target, 0);
		root.Children.Add(target);

		Grid titleRow = new() { Margin = new Thickness(10, 0, 10, 8) };
		titleRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		titleRow.ColumnDefinitions.Add(new ColumnDefinition());
		TextBlock titleLabel = new()
		{
			Text = "Title",
			Margin = new Thickness(0, 0, 8, 0),
			VerticalAlignment = VerticalAlignment.Center
		};
		titleLabel.SetResourceReference(TextBlock.ForegroundProperty, "EditorForeground");
		titleRow.Children.Add(titleLabel);
		TextBox title = new()
		{
			MinHeight = 26
		};
		title.SetBinding(TextBox.TextProperty, EditBinding("Comment.Title"));
		AutomationProperties.SetAutomationId(title, "PvfTagCommentTitle");
		Grid.SetColumn(title, 1);
		titleRow.Children.Add(title);
		titleRow.SetBinding(IsEnabledProperty, new Binding(nameof(PvfTagCommentDocument.CanSave)));
		Grid.SetRow(titleRow, 1);
		root.Children.Add(titleRow);

		TabControl editors = new() { Margin = new Thickness(10, 0, 10, 8) };
		MarkdownEditorPreview comment = new();
		comment.SetBinding(MarkdownEditorPreview.TextProperty, EditBinding("Comment.Comment"));
		comment.SetBinding(MarkdownEditorPreview.PreviewTitleProperty, new Binding("Comment.Title"));
		editors.Items.Add(new TabItem { Header = "Comment Markdown", Content = comment });
		MarkdownEditorPreview official = new();
		official.SetBinding(MarkdownEditorPreview.TextProperty, EditBinding("Comment.OfficialDescription"));
		editors.Items.Add(new TabItem { Header = "Official Markdown", Content = official });
		editors.SetBinding(IsEnabledProperty, new Binding(nameof(PvfTagCommentDocument.CanSave)));
		Grid.SetRow(editors, 2);
		root.Children.Add(editors);

		StackPanel actions = new()
		{
			Orientation = Orientation.Horizontal,
			HorizontalAlignment = HorizontalAlignment.Right,
			Margin = new Thickness(10, 0, 10, 6)
		};
		Button close = new() { Content = "Close", MinWidth = 78, Margin = new Thickness(0, 0, 8, 0) };
		close.Click += (_, _) => document?.Close();
		Button save = new() { Content = "Save", MinWidth = 78 };
		save.SetBinding(IsEnabledProperty, new Binding(nameof(PvfTagCommentDocument.CanSave)));
		save.Click += (_, _) => document?.Save();
		AutomationProperties.SetAutomationId(save, "PvfTagCommentSave");
		actions.Children.Add(close);
		actions.Children.Add(save);
		Grid.SetRow(actions, 3);
		root.Children.Add(actions);

		TextBlock status = new()
		{
			Margin = new Thickness(10, 0, 10, 8),
			TextTrimming = TextTrimming.CharacterEllipsis
		};
		status.SetResourceReference(TextBlock.ForegroundProperty, "EditorForeground");
		status.SetBinding(TextBlock.TextProperty, new Binding(nameof(PvfTagCommentDocument.Status)));
		Grid.SetRow(status, 4);
		root.Children.Add(status);

		Content = root;
		DataContextChanged += OnDataContextChanged;
	}

	private static Binding EditBinding(string path)
	{
		return new Binding(path)
		{
			Mode = BindingMode.TwoWay,
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
		};
	}

	private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		document = e.NewValue as PvfTagCommentDocument;
		if (document != null)
		{
			_ = document.LoadAsync();
		}
	}
}
