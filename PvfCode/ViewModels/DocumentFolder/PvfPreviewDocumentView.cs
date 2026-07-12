using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PvfCode.ViewModels.DocumentFolder;

public sealed class PvfPreviewDocumentView : UserControl
{
	private static readonly Brush PageBrush = Brush("#17191E");
	private static readonly Brush ToolbarBrush = Brush("#202329");
	private static readonly Brush FrameBrush = Brush("#08090D");
	private static readonly Brush OutlineBorderBrush = Brush("#4B4E55");
	private static readonly Brush PrimaryTextBrush = Brush("#E1DED8");
	private static readonly Brush MutedTextBrush = Brush("#9B948B");
	private static readonly Brush GoldBrush = Brush("#D9C27A");
	private static readonly Brush BlueBrush = Brush("#7DB4FF");
	private static readonly Brush SetBrush = Brush("#D4B1FF");
	private static readonly Brush FlavorBrush = Brush("#8C8C8C");
	private static readonly Brush ShopBrush = Brush("#72D5B0");
	private static readonly Brush QuestBrush = Brush("#F0C36A");
	private static readonly Brush WarningBrush = Brush("#FF8A65");

	private readonly ComboBox tagSelector;
	private readonly ScrollViewer previewScroller;
	private readonly StackPanel previewHost;
	private readonly TextBlock sourcePath;
	private PvfPreviewDocument document;

	public PvfPreviewDocumentView()
	{
		Background = PageBrush;
		Foreground = PrimaryTextBrush;

		Grid root = new();
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
		root.Children.Add(CreateToolbar(out tagSelector, out sourcePath));

		Grid body = new();
		body.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

		previewHost = new StackPanel
		{
			Margin = new Thickness(14),
			HorizontalAlignment = HorizontalAlignment.Stretch
		};
		previewScroller = new ScrollViewer
		{
			HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
			Content = previewHost
		};
		body.Children.Add(previewScroller);

		Grid.SetRow(body, 1);
		root.Children.Add(body);
		Content = root;
		DataContextChanged += OnDataContextChanged;
		Loaded += (_, _) => document?.RefreshPreview();
	}

	private FrameworkElement CreateToolbar(out ComboBox selector, out TextBlock path)
	{
		Border border = new()
		{
			Background = ToolbarBrush,
			BorderBrush = OutlineBorderBrush,
			BorderThickness = new Thickness(0, 0, 0, 1),
			Padding = new Thickness(10, 7, 10, 7)
		};
		Grid grid = new();
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		TextBlock label = new()
		{
			Text = "快速跳转",
			Foreground = GoldBrush,
			FontWeight = FontWeights.SemiBold,
			Margin = new Thickness(0, 0, 9, 0),
			VerticalAlignment = VerticalAlignment.Center
		};
		selector = new ComboBox
		{
			MinWidth = 220,
			MaxWidth = 620,
			Height = 27,
			DisplayMemberPath = nameof(PvfPreviewTag.DisplayName),
			HorizontalAlignment = HorizontalAlignment.Stretch,
			ToolTip = "选择 TAG 并在源码编辑器中定位"
		};
		selector.SelectionChanged += OnTagSelected;

		Button refresh = new()
		{
			Width = 29,
			Height = 27,
			Margin = new Thickness(8, 0, 0, 0),
			Padding = new Thickness(0),
			Content = "↻",
			FontSize = 17,
			ToolTip = "刷新预览"
		};
		refresh.Click += (_, _) => document?.RefreshPreview();

		path = new TextBlock
		{
			Margin = new Thickness(0, 5, 0, 0),
			Foreground = MutedTextBrush,
			FontSize = 11,
			TextTrimming = TextTrimming.CharacterEllipsis
		};

		Grid.SetColumn(selector, 1);
		Grid.SetColumn(refresh, 2);
		Grid.SetRow(path, 1);
		Grid.SetColumnSpan(path, 3);
		grid.Children.Add(label);
		grid.Children.Add(selector);
		grid.Children.Add(refresh);
		grid.Children.Add(path);
		border.Child = grid;
		return border;
	}

	private void RebuildPreview()
	{
		previewHost.Children.Clear();
		PvfRichPreview preview = document?.RichPreview;
		if (preview == null)
		{
			previewHost.Children.Add(new TextBlock { Text = "正在生成预览...", Foreground = MutedTextBrush });
			return;
		}

		Border frame = new()
		{
			MinWidth = 360,
			MaxWidth = 900,
			HorizontalAlignment = HorizontalAlignment.Left,
			Background = FrameBrush,
			BorderBrush = Brush("#74716A"),
			BorderThickness = new Thickness(1),
			Padding = new Thickness(12),
			CornerRadius = new CornerRadius(2)
		};
		StackPanel content = new();
		content.Children.Add(CreatePreviewHeader(preview));
		content.Children.Add(new TextBlock
		{
			Text = preview.SourcePath,
			Foreground = Brush("#777777"),
			FontSize = 10,
			Margin = new Thickness(0, 7, 0, 0),
			TextWrapping = TextWrapping.Wrap
		});
		content.Children.Add(CreateSeparator());

		if (!string.IsNullOrEmpty(preview.Message))
		{
			content.Children.Add(new TextBlock
			{
				Text = preview.Message,
				Foreground = WarningBrush,
				TextWrapping = TextWrapping.Wrap,
				Margin = new Thickness(0, 2, 0, 7)
			});
		}
		if (document.IsAniPreview)
		{
			content.Children.Add(CreateAniSurface());
		}
		if (preview.SkillTreeNodes.Count > 0)
		{
			content.Children.Add(CreateSkillTree(preview.SkillTreeNodes));
		}
		foreach (PvfPreviewSection section in preview.Sections)
		{
			content.Children.Add(CreateSection(section));
		}
		frame.Child = content;
		previewHost.Children.Add(frame);
	}

	private FrameworkElement CreatePreviewHeader(PvfRichPreview preview)
	{
		Grid grid = new();
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(42) });
		grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

		Border iconFrame = new()
		{
			Width = 36,
			Height = 36,
			BorderBrush = OutlineBorderBrush,
			BorderThickness = new Thickness(1),
			Background = Brush("#1B1B1B")
		};
		if (preview.Icon != null)
		{
			iconFrame.Child = new Image { Source = preview.Icon, Stretch = Stretch.Uniform, SnapsToDevicePixels = true };
		}

		StackPanel metadata = new();
		metadata.Children.Add(new TextBlock
		{
			Text = preview.Title,
			Foreground = preview.SkillKind.HasValue ? PvfSkillClassifier.GetBrush(preview.SkillKind.Value) : RarityBrush(preview.Rarity),
			FontSize = 15,
			FontWeight = FontWeights.SemiBold,
			TextWrapping = TextWrapping.Wrap
		});
		string subtitle = preview.ItemCode.HasValue ? $"{preview.Subtitle}   <{preview.ItemCode.Value}>" : preview.Subtitle;
		metadata.Children.Add(new TextBlock { Text = subtitle, Foreground = Brush("#AAA39A"), FontSize = 11, Margin = new Thickness(0, 2, 0, 0) });
		if (preview.Badges.Count > 0)
		{
			WrapPanel badges = new() { Margin = new Thickness(0, 5, 0, 0) };
			foreach (string badge in preview.Badges)
			{
				badges.Children.Add(new Border
				{
					BorderBrush = Brush("#5F574A"),
					BorderThickness = new Thickness(1),
					Padding = new Thickness(5, 1, 5, 1),
					Margin = new Thickness(0, 0, 4, 3),
					Child = new TextBlock { Text = badge, Foreground = GoldBrush, FontSize = 10 }
				});
			}
			metadata.Children.Add(badges);
		}

		Grid.SetColumn(metadata, 1);
		grid.Children.Add(iconFrame);
		grid.Children.Add(metadata);
		return grid;
	}

	private FrameworkElement CreateAniSurface()
	{
		Grid grid = new()
		{
			Height = 330,
			Margin = new Thickness(0, 2, 0, 9),
			Background = Brush("#111318")
		};
		grid.Children.Add(new Image
		{
			Stretch = Stretch.None,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			SnapsToDevicePixels = true
		});
		Image image = (Image)grid.Children[0];
		image.SetBinding(Image.SourceProperty, new Binding("AniPreviewViewModel.Item.ImageSource") { Source = document });
		TextBlock status = new()
		{
			Foreground = MutedTextBrush,
			Margin = new Thickness(10),
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Bottom,
			TextAlignment = TextAlignment.Center
		};
		status.SetBinding(TextBlock.TextProperty, new Binding(nameof(PvfPreviewDocument.AniPreviewStatus)) { Source = document });
		grid.Children.Add(status);
		return new Border { BorderBrush = OutlineBorderBrush, BorderThickness = new Thickness(1), Child = grid };
	}

	private FrameworkElement CreateSkillTree(IReadOnlyList<PvfPreviewNode> nodes)
	{
		StackPanel panel = new() { Margin = new Thickness(0, 2, 0, 8) };
		panel.Children.Add(CreateTagButton("技能树节点", nodes.FirstOrDefault()?.Tag, GoldBrush, 11, FontWeights.Normal));
		Canvas canvas = new()
		{
			Width = 700,
			Height = 320,
			Background = Brush("#111318"),
			ClipToBounds = true
		};
		double minX = nodes.Min(node => node.X);
		double maxX = nodes.Max(node => node.X);
		double minY = nodes.Min(node => node.Y);
		double maxY = nodes.Max(node => node.Y);
		double scaleX = 630 / Math.Max(1, maxX - minX);
		double scaleY = 250 / Math.Max(1, maxY - minY);
		double scale = Math.Min(1.5, Math.Min(scaleX, scaleY));
		foreach (PvfPreviewNode node in nodes)
		{
			double x = 35 + (node.X - minX) * scale;
			double y = 35 + (node.Y - minY) * scale;
			Button button = new()
			{
				Width = 78,
				Height = 34,
				Padding = new Thickness(3),
				Content = new TextBlock
				{
					Text = $"{node.Code}\n{node.Name}",
					FontSize = 9,
					TextAlignment = TextAlignment.Center,
					TextTrimming = TextTrimming.CharacterEllipsis
				},
				ToolTip = $"[{node.Tag?.Name}] 第 {node.Tag?.LineNumber} 行",
				Cursor = Cursors.Hand
			};
			button.Click += (_, _) => document?.JumpToTag(node.Tag);
			Canvas.SetLeft(button, Math.Max(0, Math.Min(canvas.Width - button.Width, x - button.Width / 2)));
			Canvas.SetTop(button, Math.Max(0, Math.Min(canvas.Height - button.Height, y - button.Height / 2)));
			canvas.Children.Add(button);
		}
		panel.Children.Add(new Border { BorderBrush = OutlineBorderBrush, BorderThickness = new Thickness(1), Child = canvas });
		return panel;
	}

	private FrameworkElement CreateSection(PvfPreviewSection section)
	{
		StackPanel panel = new() { Margin = new Thickness(0, 2, 0, 7) };
		panel.Children.Add(CreateTagButton(section.Title, section.Tag, GoldBrush, 11, FontWeights.Normal));
		foreach (PvfPreviewField field in section.Fields)
		{
			Grid row = new() { MinHeight = 18 };
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, MinWidth = 112 });
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			FrameworkElement label = CreateTagButton(field.Label, field.Tag, MutedTextBrush, 12, FontWeights.Normal);
			TextBlock value = new()
			{
				Text = field.Value,
				Foreground = ToneBrush(field.Tone == PvfPreviewTone.Normal ? section.Tone : field.Tone),
				TextWrapping = TextWrapping.Wrap,
				Margin = new Thickness(8, 1, 0, 1)
			};
			Grid.SetColumn(value, 1);
			row.Children.Add(label);
			row.Children.Add(value);
			panel.Children.Add(row);
		}
		foreach (PvfPreviewLine line in section.Lines)
		{
			panel.Children.Add(CreateTagButton(line.Text, line.Tag, ToneBrush(section.Tone), 12, FontWeights.Normal, true));
		}
		foreach (PvfPreviewTable table in section.Tables)
		{
			panel.Children.Add(CreateTable(table));
		}
		if (section.Entries.Count > 0)
		{
			Border entriesFrame = new()
			{
				BorderBrush = Brush("#343943"),
				BorderThickness = new Thickness(1),
				Background = Brush("#0B0D12"),
				Margin = new Thickness(0, 3, 0, 0)
			};
			StackPanel entries = new();
			foreach (PvfPreviewEntry entry in section.Entries.Take(80))
			{
				Grid row = new() { MinHeight = 29, Margin = new Thickness(6, 2, 6, 2) };
				row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
				StackPanel text = new();
				text.Children.Add(CreateTagButton(entry.DisplayName, entry.Tag, ToneBrush(section.Tone), 12, FontWeights.SemiBold));
				if (!string.IsNullOrEmpty(entry.Detail))
				{
					text.Children.Add(new TextBlock { Text = entry.Detail, Foreground = FlavorBrush, FontSize = 10, TextWrapping = TextWrapping.Wrap });
				}
				TextBlock lineNumber = new() { Text = entry.Tag == null ? string.Empty : $"L{entry.Tag.LineNumber}", Foreground = MutedTextBrush, FontSize = 10, VerticalAlignment = VerticalAlignment.Center };
				Grid.SetColumn(lineNumber, 1);
				row.Children.Add(text);
				row.Children.Add(lineNumber);
				entries.Children.Add(row);
			}
			entriesFrame.Child = entries;
			panel.Children.Add(entriesFrame);
		}
		return panel;
	}

	private FrameworkElement CreateTable(PvfPreviewTable table)
	{
		StackPanel panel = new() { Margin = new Thickness(0, 5, 0, 3) };
		panel.Children.Add(CreateTagButton(table.Caption, table.Tag, Brush("#B7B0A4"), 10, FontWeights.Normal));
		Grid grid = new() { Background = Brush("#0B0D12") };
		for (int column = 0; column < table.Headers.Count; column++)
		{
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, MinWidth = column == 0 ? 58 : 92 });
		}
		grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		for (int column = 0; column < table.Headers.Count; column++)
		{
			Border header = CreateTableCell(new TextBlock
			{
				Text = table.Headers[column],
				Foreground = GoldBrush,
				FontSize = 11,
				FontWeight = FontWeights.SemiBold,
				TextWrapping = TextWrapping.Wrap
			}, Brush("#171B24"));
			Grid.SetColumn(header, column);
			grid.Children.Add(header);
		}
		for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
		{
			PvfPreviewTableRow row = table.Rows[rowIndex];
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			for (int column = 0; column < table.Headers.Count; column++)
			{
				string value = column < row.Cells.Count ? row.Cells[column] : string.Empty;
				FrameworkElement content = column == 0 && row.Target != null
					? CreateTagButton(value, row.Target, Brush("#AAA39A"), 11, FontWeights.Normal)
					: new TextBlock { Text = value, Foreground = column == 0 ? Brush("#AAA39A") : Brush("#D6DFEF"), FontSize = 11, TextWrapping = TextWrapping.Wrap };
				Border cell = CreateTableCell(content, Brushes.Transparent);
				Grid.SetRow(cell, rowIndex + 1);
				Grid.SetColumn(cell, column);
				grid.Children.Add(cell);
			}
		}
		ScrollViewer scroller = new()
		{
			MaxHeight = 270,
			HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
			Content = grid
		};
		panel.Children.Add(new Border
		{
			BorderBrush = Brush("#343943"),
			BorderThickness = new Thickness(1),
			Child = scroller
		});
		return panel;
	}

	private static Border CreateTableCell(FrameworkElement content, Brush background)
	{
		return new Border
		{
			Background = background,
			BorderBrush = Brush("#303641"),
			BorderThickness = new Thickness(0, 0, 1, 1),
			Padding = new Thickness(6, 3, 6, 3),
			Child = content
		};
	}

	private FrameworkElement CreateTagButton(string text, PvfPreviewTag tag, Brush foreground, double fontSize, FontWeight weight, bool wrap = false)
	{
		if (tag == null)
		{
			return new TextBlock
			{
				Text = text,
				Foreground = foreground,
				FontSize = fontSize,
				FontWeight = weight,
				TextWrapping = wrap ? TextWrapping.Wrap : TextWrapping.NoWrap,
				Margin = new Thickness(0, 1, 0, 1)
			};
		}
		Button button = new()
		{
			Content = new TextBlock
			{
				Text = text,
				Foreground = foreground,
				FontSize = fontSize,
				FontWeight = weight,
				TextWrapping = wrap ? TextWrapping.Wrap : TextWrapping.NoWrap,
				TextAlignment = TextAlignment.Left
			},
			HorizontalContentAlignment = HorizontalAlignment.Left,
			HorizontalAlignment = HorizontalAlignment.Stretch,
			Background = Brushes.Transparent,
			BorderThickness = new Thickness(0),
			Padding = new Thickness(0, 1, 0, 1),
			Cursor = Cursors.Hand,
			ToolTip = $"跳转到 [{tag.Name}]，第 {tag.LineNumber} 行"
		};
		button.Click += (_, _) => document?.JumpToTag(tag);
		return button;
	}

	private static FrameworkElement CreateSeparator()
	{
		return new Border { Height = 1, Background = Brush("#303033"), Margin = new Thickness(0, 8, 0, 7) };
	}

	private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		if (document != null)
		{
			document.PropertyChanged -= OnDocumentPropertyChanged;
		}
		document = e.NewValue as PvfPreviewDocument;
		if (document != null)
		{
			document.PropertyChanged += OnDocumentPropertyChanged;
		}
		RefreshTagList();
		RebuildPreview();
	}

	private void OnDocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(PvfPreviewDocument.RichPreview))
		{
			RefreshTagList();
			RebuildPreview();
		}
		else if (e.PropertyName == nameof(PvfPreviewDocument.SourcePath))
		{
			sourcePath.Text = document?.SourcePath ?? string.Empty;
		}
	}

	private void RefreshTagList()
	{
		if (document == null)
		{
			tagSelector.ItemsSource = null;
			sourcePath.Text = string.Empty;
			return;
		}
		tagSelector.ItemsSource = document.Tags;
		sourcePath.Text = document.SourcePath;
	}

	private void OnTagSelected(object sender, SelectionChangedEventArgs e)
	{
		if (tagSelector.SelectedItem is PvfPreviewTag tag)
		{
			document?.JumpToTag(tag);
			tagSelector.SelectedItem = null;
		}
	}

	private static Brush ToneBrush(PvfPreviewTone tone)
	{
		return tone switch
		{
			PvfPreviewTone.Blue or PvfPreviewTone.Skill => BlueBrush,
			PvfPreviewTone.Flavor => FlavorBrush,
			PvfPreviewTone.Set => SetBrush,
			PvfPreviewTone.Shop => ShopBrush,
			PvfPreviewTone.Quest => QuestBrush,
			PvfPreviewTone.Warning => WarningBrush,
			_ => PrimaryTextBrush
		};
	}

	private static Brush RarityBrush(int? rarity)
	{
		return rarity switch
		{
			1 => Brush("#68D5ED"),
			2 => Brush("#B36BFF"),
			3 => Brush("#FF4DF2"),
			4 => Brush("#FFB100"),
			5 => Brush("#FF6666"),
			6 => Brush("#FF7800"),
			7 => Brush("#36E6FF"),
			_ => Brush("#F1F1F1")
		};
	}

	private static SolidColorBrush Brush(string color)
	{
		SolidColorBrush brush = new((Color)ColorConverter.ConvertFromString(color));
		brush.Freeze();
		return brush;
	}
}
