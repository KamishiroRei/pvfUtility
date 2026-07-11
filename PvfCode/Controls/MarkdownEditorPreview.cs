using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PvfCode.Controls;

public class MarkdownEditorPreview : Grid
{
	public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
		nameof(Text),
		typeof(string),
		typeof(MarkdownEditorPreview),
		new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

	public static readonly DependencyProperty PreviewTitleProperty = DependencyProperty.Register(
		nameof(PreviewTitle),
		typeof(string),
		typeof(MarkdownEditorPreview),
		new PropertyMetadata(string.Empty, OnPreviewTitleChanged));

	private static readonly SolidColorBrush EditorBackground = CreateFrozenBrush("#303030");
	private static readonly SolidColorBrush EditorForeground = CreateFrozenBrush("#F2F2F2");
	private static readonly SolidColorBrush EditorBorder = CreateFrozenBrush("#686868");

	private readonly TextBox editor;
	private readonly MarkdownDocumentViewer preview;
	private bool updating;

	public event EventHandler TextChanged;

	public string Text
	{
		get => (string)GetValue(TextProperty);
		set => SetValue(TextProperty, value);
	}

	public string PreviewTitle
	{
		get => (string)GetValue(PreviewTitleProperty);
		set => SetValue(PreviewTitleProperty, value);
	}

	public MarkdownEditorPreview()
	{
		MinHeight = 150;
		RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		RowDefinitions.Add(new RowDefinition());

		Panel toolbar = CreateToolbar();
		Grid.SetRow(toolbar, 0);
		Children.Add(toolbar);

		Grid body = new();
		body.ColumnDefinitions.Add(new ColumnDefinition());
		body.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
		body.ColumnDefinitions.Add(new ColumnDefinition());

		editor = new TextBox
		{
			AcceptsReturn = true,
			AcceptsTab = true,
			TextWrapping = TextWrapping.Wrap,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
			HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
			Background = EditorBackground,
			Foreground = EditorForeground,
			BorderBrush = EditorBorder,
			BorderThickness = new Thickness(1),
			FontFamily = new FontFamily("Consolas"),
			Padding = new Thickness(8)
		};
		editor.TextChanged += OnEditorTextChanged;
		body.Children.Add(editor);

		GridSplitter splitter = new()
		{
			Width = 5,
			HorizontalAlignment = HorizontalAlignment.Stretch,
			VerticalAlignment = VerticalAlignment.Stretch,
			Background = EditorBorder
		};
		Grid.SetColumn(splitter, 1);
		body.Children.Add(splitter);

		preview = new MarkdownDocumentViewer
		{
			Margin = new Thickness(8, 0, 0, 0)
		};
		Grid.SetColumn(preview, 2);
		body.Children.Add(preview);
		Grid.SetRow(body, 1);
		Children.Add(body);
		UpdatePreview();
	}

	private Panel CreateToolbar()
	{
		StackPanel toolbar = new()
		{
			Orientation = Orientation.Horizontal,
			Margin = new Thickness(0, 0, 0, 5)
		};
		toolbar.Children.Add(CreateToolButton("B", "Bold", () => WrapSelection("**", "**", "bold")));
		toolbar.Children.Add(CreateToolButton("I", "Italic", () => WrapSelection("*", "*", "italic")));
		toolbar.Children.Add(CreateToolButton("`", "Inline code", () => WrapSelection("`", "`", "code")));
		toolbar.Children.Add(CreateToolButton("[]", "Link", InsertLink));
		toolbar.Children.Add(CreateToolButton(">", "Quote", () => PrefixLines("> ")));
		toolbar.Children.Add(CreateToolButton("-", "List", () => PrefixLines("- ")));
		toolbar.Children.Add(CreateToolButton("|", "Table", InsertTable));
		toolbar.Children.Add(CreateToolButton("pvf", "PVF code block", InsertPvfCodeBlock, 44));
		return toolbar;
	}

	private Button CreateToolButton(string content, string tooltip, Action action, double width = 30)
	{
		Button button = new()
		{
			Content = content,
			ToolTip = tooltip,
			Width = width,
			Height = 26,
			Margin = new Thickness(0, 0, 4, 0),
			Padding = new Thickness(0)
		};
		button.Click += (_, _) => action();
		return button;
	}

	private void WrapSelection(string before, string after, string placeholder)
	{
		int start = editor.SelectionStart;
		int length = editor.SelectionLength;
		string selected = editor.SelectedText;
		string inner = string.IsNullOrEmpty(selected) ? placeholder : selected;
		editor.SelectedText = before + inner + after;
		editor.Focus();
		if (length == 0)
		{
			editor.Select(start + before.Length, inner.Length);
		}
		else
		{
			editor.CaretIndex = start + before.Length + inner.Length + after.Length;
		}
	}

	private void InsertLink()
	{
		int start = editor.SelectionStart;
		string selected = string.IsNullOrEmpty(editor.SelectedText) ? "text" : editor.SelectedText;
		string replacement = "[" + selected + "](https://)";
		editor.SelectedText = replacement;
		editor.Focus();
		editor.Select(start + selected.Length + 3, 8);
	}

	private void PrefixLines(string prefix)
	{
		int selectionStart = editor.SelectionStart;
		int selectionLength = editor.SelectionLength;
		int lineStart = editor.GetLineIndexFromCharacterIndex(selectionStart);
		int lineEnd = editor.GetLineIndexFromCharacterIndex(selectionStart + selectionLength);
		string[] lines = new string[Math.Max(1, lineEnd - lineStart + 1)];
		for (int i = 0; i < lines.Length; i++)
		{
			int lineIndex = lineStart + i;
			lines[i] = prefix + editor.GetLineText(lineIndex).TrimEnd('\r', '\n');
		}
		int replaceStart = editor.GetCharacterIndexFromLineIndex(lineStart);
		int replaceEnd = lineEnd + 1 < editor.LineCount
			? editor.GetCharacterIndexFromLineIndex(lineEnd + 1)
			: editor.Text.Length;
		editor.Select(replaceStart, replaceEnd - replaceStart);
		editor.SelectedText = string.Join(Environment.NewLine, lines);
		editor.Focus();
		editor.Select(replaceStart, string.Join(Environment.NewLine, lines).Length);
	}

	private void InsertTable()
	{
		InsertBlock("|字段|意义|\n|---|---|\n|`[value]`|说明|");
	}

	private void InsertPvfCodeBlock()
	{
		string selected = string.IsNullOrWhiteSpace(editor.SelectedText) ? "[tag] value" : editor.SelectedText.Trim('\r', '\n');
		InsertBlock("```pvf\n" + selected + "\n```");
	}

	private void InsertBlock(string block)
	{
		int start = editor.SelectionStart;
		int end = editor.SelectionStart + editor.SelectionLength;
		string prefix = start > 0 && !editor.Text[..start].EndsWith(Environment.NewLine, StringComparison.Ordinal) ? Environment.NewLine : string.Empty;
		string suffix = end < editor.Text.Length && !editor.Text[end..].StartsWith(Environment.NewLine, StringComparison.Ordinal) ? Environment.NewLine : string.Empty;
		editor.SelectedText = prefix + block + suffix;
		editor.Focus();
		editor.CaretIndex = start + prefix.Length + block.Length;
	}

	private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		((MarkdownEditorPreview)sender).UpdateText((string)args.NewValue);
	}

	private static void OnPreviewTitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		MarkdownEditorPreview control = (MarkdownEditorPreview)sender;
		control.preview.Title = (string)args.NewValue ?? string.Empty;
	}

	private void OnEditorTextChanged(object sender, TextChangedEventArgs args)
	{
		if (updating)
		{
			return;
		}
		updating = true;
		SetCurrentValue(TextProperty, editor.Text);
		UpdatePreview();
		updating = false;
		TextChanged?.Invoke(this, EventArgs.Empty);
	}

	private void UpdateText(string value)
	{
		string next = value ?? string.Empty;
		if (editor.Text == next)
		{
			UpdatePreview();
			return;
		}
		updating = true;
		editor.Text = next;
		editor.CaretIndex = Math.Min(editor.CaretIndex, editor.Text.Length);
		UpdatePreview();
		updating = false;
	}

	private void UpdatePreview()
	{
		preview.Markdown = editor.Text ?? string.Empty;
		preview.Title = PreviewTitle ?? string.Empty;
	}

	private static SolidColorBrush CreateFrozenBrush(string color)
	{
		SolidColorBrush brush = new((Color)ColorConverter.ConvertFromString(color));
		brush.Freeze();
		return brush;
	}
}
