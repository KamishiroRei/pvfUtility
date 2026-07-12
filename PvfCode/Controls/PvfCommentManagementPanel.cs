using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
#if RECOVERED_LEGACY_PVFCODE_DOT
using PvfCode.Compatibility;
#endif
using PvfCode.Dot.Desktop;
using PvfCode.ViewModels.Description.ViewTabComment;

namespace PvfCode.Controls;

public class PvfCommentManagementPanel : Border
{
	private readonly TextBox titleEditor;
	private readonly MarkdownEditorPreview commentEditor;
	private readonly MarkdownEditorPreview officialEditor;
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
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		root.RowDefinitions.Add(new RowDefinition());
		targetLabel = new TextBlock { Margin = new Thickness(0, 0, 0, 6), FontWeight = FontWeights.SemiBold };
		root.Children.Add(targetLabel);
		Label titleLabel = new() { Content = "Title" };
		Grid.SetRow(titleLabel, 1);
		root.Children.Add(titleLabel);
		titleEditor = new TextBox { Margin = new Thickness(0, 0, 0, 8) };
		titleEditor.TextChanged += (_, _) =>
		{
			if (!updating && target != null)
			{
#if RECOVERED_LEGACY_PVFCODE_DOT
				PvfCommentDtoCompatibility.SetTitle(target, titleEditor.Text);
#else
				target.Title = titleEditor.Text;
#endif
				commentEditor.PreviewTitle = titleEditor.Text;
			}
		};
		Grid.SetRow(titleEditor, 1);
		titleEditor.Margin = new Thickness(48, 0, 0, 8);
		root.Children.Add(titleEditor);

		TabControl tabs = new() { Margin = new Thickness(0, 4, 0, 0) };
		commentEditor = new MarkdownEditorPreview();
		commentEditor.TextChanged += (_, _) =>
		{
			if (!updating && target != null)
			{
				target.Comment = commentEditor.Text;
			}
		};
		tabs.Items.Add(new TabItem { Header = "Comment Markdown", Content = commentEditor });

		officialEditor = new MarkdownEditorPreview();
		officialEditor.TextChanged += (_, _) =>
		{
			if (!updating && target != null)
			{
#if RECOVERED_LEGACY_PVFCODE_DOT
				PvfCommentDtoCompatibility.SetOfficialDescription(target, officialEditor.Text);
#else
				target.OfficialDescription = officialEditor.Text;
#endif
			}
		};
		tabs.Items.Add(new TabItem { Header = "Official Markdown", Content = officialEditor });
		Grid.SetRow(tabs, 2);
		root.Children.Add(tabs);
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
#if RECOVERED_LEGACY_PVFCODE_DOT
		titleEditor.Text = PvfCommentDtoCompatibility.GetTitle(target);
		commentEditor.PreviewTitle = PvfCommentDtoCompatibility.GetTitle(target);
		commentEditor.Text = target?.Comment ?? string.Empty;
		officialEditor.Text = PvfCommentDtoCompatibility.GetOfficialDescription(target);
#else
		titleEditor.Text = target?.Title ?? string.Empty;
		commentEditor.PreviewTitle = target?.Title ?? string.Empty;
		commentEditor.Text = target?.Comment ?? string.Empty;
		officialEditor.Text = target?.OfficialDescription ?? string.Empty;
#endif
		updating = false;
	}
}
