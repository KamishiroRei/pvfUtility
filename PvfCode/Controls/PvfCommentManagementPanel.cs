using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PvfCode.Dot.Desktop;
using PvfCode.ViewModels.Description.ViewTabComment;

namespace PvfCode.Controls;

public class PvfCommentManagementPanel : Border
{
	private readonly TextBox titleEditor;
	private readonly TextBox officialEditor;
	private readonly MarkdownDocumentViewer preview;
	private readonly TextBlock targetLabel;
	private ViewTabCommentViewModel viewModel;
	private PvfCommentDto target;
	private bool updating;

	public PvfCommentManagementPanel()
	{
		BorderThickness = new Thickness(0, 1, 0, 0);
		BorderBrush = System.Windows.Media.Brushes.Gray;
		Padding = new Thickness(10);
		Grid root = new();
		root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
		root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
		Grid editor = new() { Margin = new Thickness(0, 0, 12, 0) };
		editor.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		editor.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		editor.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		editor.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		editor.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		editor.RowDefinitions.Add(new RowDefinition());
		targetLabel = new TextBlock { Margin = new Thickness(0, 0, 0, 6), FontWeight = FontWeights.SemiBold };
		editor.Children.Add(targetLabel);
		Label titleLabel = new() { Content = "Title" };
		Grid.SetRow(titleLabel, 1);
		editor.Children.Add(titleLabel);
		titleEditor = new TextBox { Margin = new Thickness(0, 0, 0, 8) };
		titleEditor.TextChanged += (_, _) =>
		{
			if (!updating && target != null)
			{
				target.Title = titleEditor.Text;
			}
		};
		Grid.SetRow(titleEditor, 2);
		editor.Children.Add(titleEditor);
		Label officialLabel = new() { Content = "Official Description (Markdown)" };
		Grid.SetRow(officialLabel, 3);
		editor.Children.Add(officialLabel);
		officialEditor = new TextBox
		{
			AcceptsReturn = true,
			AcceptsTab = true,
			TextWrapping = TextWrapping.Wrap,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto
		};
		officialEditor.TextChanged += (_, _) =>
		{
			if (!updating && target != null)
			{
				target.OfficialDescription = officialEditor.Text;
			}
		};
		Grid.SetRow(officialEditor, 4);
		editor.Children.Add(officialEditor);
		root.Children.Add(editor);
		preview = new MarkdownDocumentViewer();
		GroupBox previewGroup = new() { Header = "Markdown preview", Content = preview };
		Grid.SetColumn(previewGroup, 1);
		root.Children.Add(previewGroup);
		Child = root;
		DataContextChanged += OnDataContextChanged;
	}

	private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
	{
		if (viewModel != null)
		{
			viewModel.PropertyChanged -= OnViewModelPropertyChanged;
		}
		viewModel = args.NewValue as ViewTabCommentViewModel;
		if (viewModel != null)
		{
			viewModel.PropertyChanged += OnViewModelPropertyChanged;
		}
		SelectTarget();
	}

	private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
	{
		if (args.PropertyName == nameof(ViewTabCommentViewModel.SelectedItem))
		{
			SelectTarget();
		}
	}

	private void SelectTarget()
	{
		if (target != null)
		{
			target.PropertyChanged -= OnTargetPropertyChanged;
		}
		target = viewModel?.SelectedItem ?? viewModel?.AddData;
		if (target != null)
		{
			target.PropertyChanged += OnTargetPropertyChanged;
		}
		RefreshFields();
	}

	private void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs args)
	{
		RefreshFields();
	}

	private void RefreshFields()
	{
		updating = true;
		targetLabel.Text = target == null ? string.Empty : $"{target.FileType}: [{target.Section}]";
		titleEditor.Text = target?.Title ?? string.Empty;
		officialEditor.Text = target?.OfficialDescription ?? string.Empty;
		preview.Title = target?.Title ?? string.Empty;
		preview.Markdown = target?.Comment ?? string.Empty;
		preview.OfficialDescription = target?.OfficialDescription ?? string.Empty;
		updating = false;
	}
}
