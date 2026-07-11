using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PvfCode.Controls;

public class MarkdownDocumentViewer : FlowDocumentScrollViewer
{
	public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
		nameof(Title), typeof(string), typeof(MarkdownDocumentViewer), new PropertyMetadata(string.Empty, OnMarkdownChanged));

	public static readonly DependencyProperty MarkdownProperty = DependencyProperty.Register(
		nameof(Markdown), typeof(string), typeof(MarkdownDocumentViewer), new PropertyMetadata(string.Empty, OnMarkdownChanged));

	public static readonly DependencyProperty OfficialDescriptionProperty = DependencyProperty.Register(
		nameof(OfficialDescription), typeof(string), typeof(MarkdownDocumentViewer), new PropertyMetadata(string.Empty, OnMarkdownChanged));

	private static readonly Regex InlineTokens = new(
		@"(`[^`]+`|\*\*[^*]+\*\*|__[^_]+__|\*[^*]+\*|_[^_]+_|\[[^\]]+\]\([^\)]+\))",
		RegexOptions.Compiled);

	public string Title
	{
		get => (string)GetValue(TitleProperty);
		set => SetValue(TitleProperty, value);
	}

	public string Markdown
	{
		get => (string)GetValue(MarkdownProperty);
		set => SetValue(MarkdownProperty, value);
	}

	public string OfficialDescription
	{
		get => (string)GetValue(OfficialDescriptionProperty);
		set => SetValue(OfficialDescriptionProperty, value);
	}

	public MarkdownDocumentViewer()
	{
		IsToolBarVisible = false;
		VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
		HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
		Padding = new Thickness(12);
		Background = Brushes.Transparent;
		RebuildDocument();
	}

	private static void OnMarkdownChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		((MarkdownDocumentViewer)sender).RebuildDocument();
	}

	private void RebuildDocument()
	{
		FlowDocument document = new()
		{
			PagePadding = new Thickness(0),
			FontFamily = FontFamily,
			FontSize = FontSize,
			Foreground = Foreground,
			TextAlignment = TextAlignment.Left
		};
		if (!string.IsNullOrWhiteSpace(Title))
		{
			Paragraph title = CreateParagraph(Title.Trim());
			title.FontSize = 20;
			title.FontWeight = FontWeights.SemiBold;
			title.Margin = new Thickness(0, 0, 0, 10);
			document.Blocks.Add(title);
		}
		AppendMarkdown(document, Markdown);
		if (!string.IsNullOrWhiteSpace(OfficialDescription))
		{
			Paragraph heading = new(new Run("Official Description"))
			{
				FontSize = 14,
				FontWeight = FontWeights.SemiBold,
				Margin = new Thickness(0, 14, 0, 6)
			};
			document.Blocks.Add(heading);
			AppendMarkdown(document, OfficialDescription);
		}
		Document = document;
	}

	private static void AppendMarkdown(FlowDocument document, string markdown)
	{
		if (string.IsNullOrWhiteSpace(markdown))
		{
			return;
		}
		string[] lines = markdown.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
		bool inCodeBlock = false;
		Paragraph codeBlock = null;
		foreach (string rawLine in lines)
		{
			string line = rawLine ?? string.Empty;
			if (line.TrimStart().StartsWith("```", StringComparison.Ordinal))
			{
				if (inCodeBlock && codeBlock != null)
				{
					document.Blocks.Add(codeBlock);
					codeBlock = null;
				}
				inCodeBlock = !inCodeBlock;
				if (inCodeBlock)
				{
					codeBlock = CreateCodeBlock();
				}
				continue;
			}
			if (inCodeBlock)
			{
				if (codeBlock.Inlines.Count > 0)
				{
					codeBlock.Inlines.Add(new LineBreak());
				}
				codeBlock.Inlines.Add(new Run(line));
				continue;
			}
			if (string.IsNullOrWhiteSpace(line))
			{
				continue;
			}
			string trimmed = line.TrimStart();
			if (trimmed == "---" || trimmed == "***")
			{
				document.Blocks.Add(new BlockUIContainer(new Border { Height = 1, Margin = new Thickness(0, 8, 0, 8), Background = Brushes.Gray }));
				continue;
			}
			int headingLevel = CountHeadingPrefix(trimmed);
			if (headingLevel > 0)
			{
				Paragraph heading = CreateParagraph(trimmed[(headingLevel + 1)..]);
				heading.FontSize = Math.Max(14, 24 - headingLevel * 2);
				heading.FontWeight = FontWeights.SemiBold;
				heading.Margin = new Thickness(0, 8, 0, 4);
				document.Blocks.Add(heading);
				continue;
			}
			bool quote = trimmed.StartsWith("> ", StringComparison.Ordinal);
			bool bullet = trimmed.StartsWith("- ", StringComparison.Ordinal) || trimmed.StartsWith("* ", StringComparison.Ordinal) || trimmed.StartsWith("+ ", StringComparison.Ordinal);
			Match ordered = Regex.Match(trimmed, @"^\d+\.\s+");
			string prefix = string.Empty;
			if (quote)
			{
				trimmed = trimmed[2..];
				prefix = "| ";
			}
			else if (bullet)
			{
				trimmed = trimmed[2..];
				prefix = "• ";
			}
			else if (ordered.Success)
			{
				prefix = ordered.Value.Trim() + " ";
				trimmed = trimmed[ordered.Length..];
			}
			Paragraph paragraph = CreateParagraph(prefix + trimmed);
			paragraph.Margin = new Thickness((bullet || ordered.Success) ? 12 : 0, 2, 0, 4);
			if (quote)
			{
				paragraph.FontStyle = FontStyles.Italic;
				paragraph.Foreground = Brushes.Gray;
			}
			document.Blocks.Add(paragraph);
		}
		if (codeBlock != null)
		{
			document.Blocks.Add(codeBlock);
		}
	}

	private static Paragraph CreateParagraph(string text)
	{
		Paragraph paragraph = new() { Margin = new Thickness(0, 2, 0, 4) };
		int offset = 0;
		foreach (Match match in InlineTokens.Matches(text))
		{
			if (match.Index > offset)
			{
				paragraph.Inlines.Add(new Run(text[offset..match.Index]));
			}
			string token = match.Value;
			Inline inline;
			if (token.StartsWith("`", StringComparison.Ordinal))
			{
				inline = new Run(token[1..^1]) { FontFamily = new FontFamily("Consolas"), Background = Brushes.DimGray, Foreground = Brushes.White };
			}
			else if (token.StartsWith("**", StringComparison.Ordinal) || token.StartsWith("__", StringComparison.Ordinal))
			{
				inline = new Bold(new Run(token[2..^2]));
			}
			else if (token.StartsWith("[", StringComparison.Ordinal))
			{
				int end = token.IndexOf("](", StringComparison.Ordinal);
				inline = new Underline(new Run(end > 0 ? token[1..end] : token));
			}
			else
			{
				inline = new Italic(new Run(token[1..^1]));
			}
			paragraph.Inlines.Add(inline);
			offset = match.Index + match.Length;
		}
		if (offset < text.Length)
		{
			paragraph.Inlines.Add(new Run(text[offset..]));
		}
		return paragraph;
	}

	private static Paragraph CreateCodeBlock()
	{
		return new Paragraph
		{
			FontFamily = new FontFamily("Consolas"),
			Background = Brushes.DimGray,
			Foreground = Brushes.White,
			Padding = new Thickness(8),
			Margin = new Thickness(0, 6, 0, 6)
		};
	}

	private static int CountHeadingPrefix(string value)
	{
		int count = 0;
		while (count < value.Length && value[count] == '#' && count < 6)
		{
			count++;
		}
		return count > 0 && count < value.Length && value[count] == ' ' ? count : 0;
	}
}
