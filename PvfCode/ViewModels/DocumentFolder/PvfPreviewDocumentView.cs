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
		if (preview.SkillTreeGroups.Count > 0)
		{
			content.Children.Add(CreateSkillTrees(preview.SkillTreeGroups));
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

	private FrameworkElement CreateSkillTrees(IReadOnlyList<PvfPreviewSkillTreeGroup> groups)
	{
		StackPanel panel = new() { Margin = new Thickness(0, 2, 0, 8) };
		panel.Children.Add(new TextBlock
		{
			Text = "技能树",
			Foreground = GoldBrush,
			FontSize = 11,
			Margin = new Thickness(0, 1, 0, 4)
		});
		foreach (PvfPreviewSkillTreeGroup group in groups)
		{
			panel.Children.Add(CreateSkillTreeGroup(group));
		}
		return panel;
	}

	private FrameworkElement CreateSkillTreeGroup(PvfPreviewSkillTreeGroup group)
	{
		const double canvasWidth = 800;
		IReadOnlyList<PvfPreviewNode> nodes = group.Nodes;
		double canvasHeight = SkillTreeHeight(nodes);
		Canvas canvas = new()
		{
			Width = canvasWidth,
			Height = canvasHeight,
			Background = Brush("#111318"),
			ClipToBounds = true,
			SnapsToDevicePixels = true
		};
		Dictionary<PvfPreviewNode, Point> positions = SkillTreePositions(nodes, canvasWidth, canvasHeight);
		Dictionary<int, Point> positionsByCode = new();
		foreach (PvfPreviewNode node in nodes)
		{
			positionsByCode[node.Code] = positions[node];
		}
		foreach (PvfPreviewNode node in nodes)
		{
			Point from = positions[node];
			foreach (int nextSkill in node.NextSkills)
			{
				if (!positionsByCode.TryGetValue(nextSkill, out Point to))
				{
					continue;
				}
				canvas.Children.Add(new Line
				{
					X1 = from.X,
					Y1 = from.Y,
					X2 = to.X,
					Y2 = to.Y,
					Stroke = Brush("#667184"),
					StrokeThickness = 1.5,
					IsHitTestVisible = false
				});
			}
		}
		foreach (PvfPreviewNode node in nodes)
		{
			Point position = positions[node];
			FrameworkElement nodeContent = node.Icon != null
				? new Image { Source = node.Icon, Stretch = Stretch.Uniform, SnapsToDevicePixels = true }
				: new TextBlock
				{
					Text = node.Code.ToString(),
					Foreground = node.Name == "未解析" ? Brush("#777777") : PrimaryTextBrush,
					FontSize = 9,
					TextAlignment = TextAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center
				};
			Button button = new()
			{
				Width = 36,
				Height = 36,
				Padding = new Thickness(2),
				Content = nodeContent,
				Background = Brush(node.Name == "未解析" ? "#1B1B1B" : "#101114"),
				BorderBrush = Brush(node.IsCommon ? "#D8B657" : node.Name == "未解析" ? "#575757" : "#84735A"),
				BorderThickness = new Thickness(1),
				ToolTip = $"ID: {node.Code}\n名称: {node.Name}\n[{node.Tag?.Name}] 第 {node.Tag?.LineNumber} 行",
				Cursor = Cursors.Hand,
				SnapsToDevicePixels = true
			};
			button.Click += (_, _) => document?.JumpToTag(node.Tag);
			Canvas.SetLeft(button, Math.Max(0, Math.Min(canvas.Width - button.Width, position.X - button.Width / 2)));
			Canvas.SetTop(button, Math.Max(0, Math.Min(canvas.Height - button.Height, position.Y - button.Height / 2)));
			canvas.Children.Add(button);
		}

		StackPanel content = new();
		content.Children.Add(new Border
		{
			Height = 27,
			Padding = new Thickness(8, 4, 8, 4),
			Background = Brush("#0B0D12"),
			BorderBrush = Brush("#38352F"),
			BorderThickness = new Thickness(0, 0, 0, 1),
			Child = new TextBlock
			{
				Text = group.Title,
				Foreground = GoldBrush,
				FontSize = 11,
				TextTrimming = TextTrimming.CharacterEllipsis
			}
		});
		content.Children.Add(canvas);
		return new Border
		{
			Margin = new Thickness(0, 0, 0, 9),
			BorderBrush = OutlineBorderBrush,
			BorderThickness = new Thickness(1),
			Child = content
		};
	}

	private static Dictionary<PvfPreviewNode, Point> SkillTreePositions(IReadOnlyList<PvfPreviewNode> nodes, double width, double height)
	{
		List<PvfPreviewNode> positioned = nodes.Where(node => node.X.HasValue && node.Y.HasValue).ToList();
		double minX = positioned.Count > 0 ? positioned.Min(node => node.X.Value) : 0;
		double maxX = positioned.Count > 0 ? positioned.Max(node => node.X.Value) : 1;
		double minY = positioned.Count > 0 ? positioned.Min(node => node.Y.Value) : 0;
		double maxY = positioned.Count > 0 ? positioned.Max(node => node.Y.Value) : 1;
		double spanX = Math.Max(1, maxX - minX);
		double spanY = Math.Max(1, maxY - minY);
		int fallbackColumns = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(Math.Max(1, nodes.Count))));
		Dictionary<PvfPreviewNode, Point> result = new();
		for (int index = 0; index < nodes.Count; index++)
		{
			PvfPreviewNode node = nodes[index];
			if (node.X.HasValue && node.Y.HasValue)
			{
				result[node] = new Point(
					48 + ((node.X.Value - minX) / spanX) * (width - 96),
					48 + ((node.Y.Value - minY) / spanY) * (height - 96));
				continue;
			}
			int column = index % fallbackColumns;
			int row = index / fallbackColumns;
			result[node] = new Point(
				48 + (column / (double)Math.Max(1, fallbackColumns - 1)) * (width - 96),
				48 + row * 54);
		}
		return result;
	}

	private static double SkillTreeHeight(IReadOnlyList<PvfPreviewNode> nodes)
	{
		List<double> ys = nodes.Where(node => node.Y.HasValue).Select(node => node.Y.Value).ToList();
		if (ys.Count == 0)
		{
			return Math.Min(620, Math.Max(260, Math.Ceiling(nodes.Count / 8d) * 62));
		}
		double span = Math.Max(1, ys.Max() - ys.Min());
		return Math.Min(760, Math.Max(260, Math.Round(span + 120)));
	}

	private FrameworkElement CreateSection(PvfPreviewSection section)
	{
		StackPanel panel = new() { Margin = new Thickness(0, 2, 0, 7) };
		panel.Children.Add(CreateTagButton(section.Title, section.Tag, GoldBrush, 11, FontWeights.Normal));
		foreach (PvfPreviewField field in section.Fields)
		{
			Grid row = new() { MinHeight = 18 };
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, MinWidth = 112 });
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = field.Icon == null ? new GridLength(0) : new GridLength(26) });
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			FrameworkElement label = CreateTagButton(field.Label, field.Tag, MutedTextBrush, 12, FontWeights.Normal);
			if (field.Icon != null)
			{
				FrameworkElement icon = CreateReferenceIcon(field.Icon, 20);
				Grid.SetColumn(icon, 1);
				row.Children.Add(icon);
			}
			TextBlock value = new()
			{
				Text = field.Value,
				Foreground = ToneBrush(field.Tone == PvfPreviewTone.Normal ? section.Tone : field.Tone),
				TextWrapping = TextWrapping.Wrap,
				Margin = new Thickness(8, 1, 0, 1)
			};
			Grid.SetColumn(value, 2);
			row.Children.Add(label);
			row.Children.Add(value);
			panel.Children.Add(row);
		}
		foreach (PvfPreviewLine line in section.Lines)
		{
			Grid lineRow = new() { MinHeight = line.Icon == null ? 18 : 26 };
			lineRow.ColumnDefinitions.Add(new ColumnDefinition { Width = line.Icon == null ? new GridLength(0) : new GridLength(30) });
			lineRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			if (line.Icon != null)
			{
				lineRow.Children.Add(CreateReferenceIcon(line.Icon, 24));
			}
			FrameworkElement lineText = CreateTagButton(line.Text, line.Tag, ToneBrush(section.Tone), 12, FontWeights.Normal, true);
			Grid.SetColumn(lineText, 1);
			lineRow.Children.Add(lineText);
			panel.Children.Add(lineRow);
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
			IEnumerable<PvfPreviewEntry> visibleEntries = section.ShowAllEntries ? section.Entries : section.Entries.Take(80);
			foreach (PvfPreviewEntry entry in visibleEntries)
			{
				Grid row = new() { MinHeight = entry.Icon == null ? 29 : 34, Margin = new Thickness(6, 2, 6, 2) };
				row.ColumnDefinitions.Add(new ColumnDefinition { Width = entry.Icon == null ? new GridLength(0) : new GridLength(36) });
				row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
				row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
				if (entry.Icon != null)
				{
					row.Children.Add(CreateReferenceIcon(entry.Icon, 30));
				}
				StackPanel text = new();
				text.Children.Add(CreateTagButton(entry.DisplayName, entry.Tag, ToneBrush(section.Tone), 12, FontWeights.SemiBold));
				if (!string.IsNullOrEmpty(entry.Detail))
				{
					text.Children.Add(new TextBlock { Text = entry.Detail, Foreground = FlavorBrush, FontSize = 10, TextWrapping = TextWrapping.Wrap });
				}
				TextBlock lineNumber = new() { Text = entry.Tag == null ? string.Empty : $"L{entry.Tag.LineNumber}", Foreground = MutedTextBrush, FontSize = 10, VerticalAlignment = VerticalAlignment.Center };
				Grid.SetColumn(text, 1);
				Grid.SetColumn(lineNumber, 2);
				row.Children.Add(text);
				row.Children.Add(lineNumber);
				entries.Children.Add(row);
			}
			entriesFrame.Child = entries;
			panel.Children.Add(entriesFrame);
		}
		return panel;
	}

	private static FrameworkElement CreateReferenceIcon(ImageSource source, double size)
	{
		return new Border
		{
			Width = size,
			Height = size,
			BorderBrush = Brush("#343943"),
			BorderThickness = new Thickness(1),
			Background = Brush("#111318"),
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Left,
			Child = new Image
			{
				Source = source,
				Stretch = Stretch.Uniform,
				SnapsToDevicePixels = true
			}
		};
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
