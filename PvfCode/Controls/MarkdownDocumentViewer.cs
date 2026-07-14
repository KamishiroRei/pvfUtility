using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using PvfCode.OfficialAnnotations;
using MdBlock = Markdig.Syntax.Block;
using MdTable = Markdig.Extensions.Tables.Table;
using MdTableCell = Markdig.Extensions.Tables.TableCell;
using MdTableColumnDefinition = Markdig.Extensions.Tables.TableColumnDefinition;
using MdTableRow = Markdig.Extensions.Tables.TableRow;
using WpfBlock = System.Windows.Documents.Block;
using WpfHyperlink = System.Windows.Documents.Hyperlink;
using WpfList = System.Windows.Documents.List;
using WpfListItem = System.Windows.Documents.ListItem;
using WpfTable = System.Windows.Documents.Table;
using WpfTableCell = System.Windows.Documents.TableCell;
using WpfTableRow = System.Windows.Documents.TableRow;
using WpfTableRowGroup = System.Windows.Documents.TableRowGroup;

namespace PvfCode.Controls;

public class MarkdownDocumentViewer : FlowDocumentScrollViewer
{
	public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
		nameof(Title), typeof(string), typeof(MarkdownDocumentViewer), new PropertyMetadata(string.Empty, OnMarkdownChanged));

	public static readonly DependencyProperty MarkdownProperty = DependencyProperty.Register(
		nameof(Markdown), typeof(string), typeof(MarkdownDocumentViewer), new PropertyMetadata(string.Empty, OnMarkdownChanged));

	public static readonly DependencyProperty OfficialDescriptionProperty = DependencyProperty.Register(
		nameof(OfficialDescription), typeof(string), typeof(MarkdownDocumentViewer), new PropertyMetadata(string.Empty, OnMarkdownChanged));

	private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
		.UseAdvancedExtensions()
		.Build();

	private static readonly FontFamily CodeFontFamily = new("Consolas");
	private const string EditorBackgroundResource = "EditorBackground";
	private const string EditorForegroundResource = "EditorForeground";
	private const string CodeBackgroundResource = "EditorFindKeyWordTextBoxBackBrush";
	private const string LinkForegroundResource = "EditorLinkTextForegroundBrush";
	private const string MutedForegroundResource = "EditorFoldingMarkerBrush";

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
		SetResourceReference(BackgroundProperty, EditorBackgroundResource);
		SetResourceReference(ForegroundProperty, EditorForegroundResource);
		RebuildDocument();
	}

	public static FlowDocument RenderDocument(
		string title,
		string markdown,
		string officialDescription,
		FontFamily fontFamily = null,
		double fontSize = 12,
		Brush foreground = null)
	{
		FlowDocument document = new()
		{
			PagePadding = new Thickness(0),
			FontFamily = fontFamily ?? SystemFonts.MessageFontFamily,
			FontSize = fontSize,
			TextAlignment = TextAlignment.Left
		};
		document.SetResourceReference(FlowDocument.BackgroundProperty, EditorBackgroundResource);
		if (foreground == null)
		{
			document.SetResourceReference(FlowDocument.ForegroundProperty, EditorForegroundResource);
		}
		else
		{
			document.Foreground = foreground;
		}
		if (!string.IsNullOrWhiteSpace(title))
		{
			Paragraph titleParagraph = CreateParagraph(title.Trim());
			titleParagraph.FontSize = Math.Max(fontSize + 6, 20);
			titleParagraph.FontWeight = FontWeights.SemiBold;
			titleParagraph.Margin = new Thickness(0, 0, 0, 10);
			document.Blocks.Add(titleParagraph);
		}
		AppendMarkdown(document.Blocks, OfficialAnnotationLinks.LinkifyOfficialExamples(markdown));
		if (!string.IsNullOrWhiteSpace(officialDescription))
		{
			if (document.Blocks.Count > 0)
			{
				document.Blocks.Add(CreateThematicBreak());
			}
			AppendMarkdown(document.Blocks, OfficialAnnotationLinks.LinkifyOfficialExamples(officialDescription));
		}
		return document;
	}

	private static void OnMarkdownChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		((MarkdownDocumentViewer)sender).RebuildDocument();
	}

	private void RebuildDocument()
	{
		Document = RenderDocument(Title, Markdown, OfficialDescription, FontFamily, FontSize);
	}

	private static void AppendMarkdown(BlockCollection blocks, string markdown)
	{
		if (string.IsNullOrWhiteSpace(markdown))
		{
			return;
		}
		MarkdownDocument document = Markdig.Markdown.Parse(markdown, Pipeline);
		AppendBlocks(blocks, document);
	}

	private static void AppendBlocks(BlockCollection blocks, ContainerBlock container)
	{
		foreach (MdBlock block in container)
		{
			AppendBlock(blocks, block);
		}
	}

	private static void AppendBlock(BlockCollection blocks, MdBlock block)
	{
		switch (block)
		{
		case ParagraphBlock paragraph:
			if (paragraph.Inline != null)
			{
				blocks.Add(CreateParagraph(paragraph.Inline));
			}
			break;
		case HeadingBlock heading:
			blocks.Add(CreateHeading(heading));
			break;
		case ThematicBreakBlock:
			blocks.Add(CreateThematicBreak());
			break;
		case QuoteBlock quote:
			blocks.Add(CreateQuote(quote));
			break;
		case ListBlock list:
			blocks.Add(CreateList(list));
			break;
		case MdTable table:
			blocks.Add(CreateTable(table));
			break;
		case CodeBlock code:
			blocks.Add(CreateCodeBlock(code));
			break;
		case HtmlBlock html:
			blocks.Add(CreateCodeBlock(html));
			break;
		case ContainerBlock container:
			Section section = new() { Margin = new Thickness(0, 2, 0, 4) };
			AppendBlocks(section.Blocks, container);
			if (section.Blocks.Count > 0)
			{
				blocks.Add(section);
			}
			break;
		case LeafBlock leaf when leaf.Inline != null:
			blocks.Add(CreateParagraph(leaf.Inline));
			break;
		default:
			string fallback = block.ToString();
			if (!string.IsNullOrWhiteSpace(fallback))
			{
				blocks.Add(CreateParagraph(fallback));
			}
			break;
		}
	}

	private static Paragraph CreateHeading(HeadingBlock heading)
	{
		Paragraph paragraph = CreateParagraph(heading.Inline);
		paragraph.FontSize = Math.Max(14, 26 - heading.Level * 2);
		paragraph.FontWeight = FontWeights.SemiBold;
		paragraph.Margin = new Thickness(0, heading.Level <= 2 ? 12 : 8, 0, 6);
		return paragraph;
	}

	private static Section CreateQuote(QuoteBlock quote)
	{
		Section section = new()
		{
			Margin = new Thickness(0, 6, 0, 6),
			Padding = new Thickness(10, 2, 0, 2),
			BorderThickness = new Thickness(3, 0, 0, 0)
		};
		section.SetResourceReference(Section.BorderBrushProperty, MutedForegroundResource);
		section.SetResourceReference(Section.ForegroundProperty, MutedForegroundResource);
		AppendBlocks(section.Blocks, quote);
		return section;
	}

	private static WpfList CreateList(ListBlock listBlock)
	{
		WpfList list = new()
		{
			MarkerStyle = listBlock.IsOrdered ? TextMarkerStyle.Decimal : TextMarkerStyle.Disc,
			Margin = new Thickness(18, 4, 0, 6),
			Padding = new Thickness(14, 0, 0, 0)
		};
		foreach (ListItemBlock itemBlock in listBlock.OfType<ListItemBlock>())
		{
			WpfListItem item = new() { Margin = new Thickness(0, 1, 0, 2) };
			AppendBlocks(item.Blocks, itemBlock);
			if (item.Blocks.Count == 0)
			{
				item.Blocks.Add(new Paragraph());
			}
			list.ListItems.Add(item);
		}
		return list;
	}

	private static WpfTable CreateTable(MdTable tableBlock)
	{
		WpfTable table = new()
		{
			CellSpacing = 0,
			Margin = new Thickness(0, 8, 0, 10)
		};
		int columns = Math.Max(
			tableBlock.ColumnDefinitions?.Count ?? 0,
			tableBlock.OfType<MdTableRow>().Select(row => row.OfType<MdTableCell>().Count()).DefaultIfEmpty(0).Max());
		for (int i = 0; i < Math.Max(columns, 1); i++)
		{
			table.Columns.Add(new TableColumn());
		}
		WpfTableRowGroup group = new();
		foreach (MdTableRow sourceRow in tableBlock.OfType<MdTableRow>())
		{
			WpfTableRow row = new();
			foreach (MdTableCell sourceCell in sourceRow.OfType<MdTableCell>())
			{
				WpfTableCell cell = new()
				{
					BorderThickness = new Thickness(1),
					Padding = new Thickness(8, 5, 8, 5),
					ColumnSpan = Math.Max(1, sourceCell.ColumnSpan)
				};
				cell.SetResourceReference(WpfTableCell.BorderBrushProperty, MutedForegroundResource);
				if (sourceRow.IsHeader)
				{
					cell.SetResourceReference(WpfTableCell.BackgroundProperty, CodeBackgroundResource);
					cell.FontWeight = FontWeights.SemiBold;
				}
				AppendBlocks(cell.Blocks, sourceCell);
				if (cell.Blocks.Count == 0)
				{
					cell.Blocks.Add(new Paragraph());
				}
				FormatTableCellBlocks(tableBlock, sourceCell, cell.Blocks);
				row.Cells.Add(cell);
			}
			group.Rows.Add(row);
		}
		table.RowGroups.Add(group);
		return table;
	}

	private static void FormatTableCellBlocks(MdTable table, MdTableCell cell, BlockCollection blocks)
	{
		foreach (WpfBlock block in blocks)
		{
			FormatTableCellBlock(table, cell, block);
		}
	}

	private static void FormatTableCellBlock(MdTable table, MdTableCell cell, WpfBlock block)
	{
		switch (block)
		{
		case Paragraph paragraph:
			paragraph.Margin = new Thickness(0);
			SetTableCellAlignment(table, cell, paragraph);
			break;
		case Section section:
			section.Margin = new Thickness(0);
			FormatTableCellBlocks(table, cell, section.Blocks);
			break;
		case WpfList list:
			list.Margin = new Thickness(18, 0, 0, 0);
			foreach (WpfListItem item in list.ListItems)
			{
				FormatTableCellBlocks(table, cell, item.Blocks);
			}
			break;
		}
	}

	private static void SetTableCellAlignment(MdTable table, MdTableCell cell, Paragraph paragraph)
	{
		MdTableColumnDefinition definition = null;
		if (table.ColumnDefinitions != null &&
			cell.ColumnIndex >= 0 &&
			cell.ColumnIndex < table.ColumnDefinitions.Count)
		{
			definition = table.ColumnDefinitions[cell.ColumnIndex];
		}
		paragraph.TextAlignment = definition?.Alignment switch
		{
			TableColumnAlign.Center => TextAlignment.Center,
			TableColumnAlign.Right => TextAlignment.Right,
			_ => TextAlignment.Left
		};
	}

	private static Paragraph CreateCodeBlock(LeafBlock code)
	{
		Paragraph paragraph = new()
		{
			FontFamily = CodeFontFamily,
			Padding = new Thickness(9),
			Margin = new Thickness(0, 7, 0, 8)
		};
		paragraph.SetResourceReference(Paragraph.BackgroundProperty, CodeBackgroundResource);
		paragraph.SetResourceReference(Paragraph.ForegroundProperty, EditorForegroundResource);
		string text = code.Lines.ToString().TrimEnd('\r', '\n');
		string[] lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
		for (int i = 0; i < lines.Length; i++)
		{
			if (i > 0)
			{
				paragraph.Inlines.Add(new LineBreak());
			}
			paragraph.Inlines.Add(new Run(lines[i]));
		}
		return paragraph;
	}

	private static BlockUIContainer CreateThematicBreak()
	{
		Border border = new()
		{
			Height = 1,
			Margin = new Thickness(0, 10, 0, 10)
		};
		border.SetResourceReference(Border.BackgroundProperty, MutedForegroundResource);
		return new BlockUIContainer(border);
	}

	private static Paragraph CreateParagraph(ContainerInline inline)
	{
		Paragraph paragraph = CreateParagraph();
		AppendInlines(paragraph.Inlines, inline);
		return paragraph;
	}

	private static Paragraph CreateParagraph(string text)
	{
		Paragraph paragraph = CreateParagraph();
		paragraph.Inlines.Add(new Run(text ?? string.Empty));
		return paragraph;
	}

	private static Paragraph CreateParagraph()
	{
		return new Paragraph { Margin = new Thickness(0, 2, 0, 5) };
	}

	private static void AppendInlines(InlineCollection target, ContainerInline container)
	{
		if (container == null)
		{
			return;
		}
		for (Markdig.Syntax.Inlines.Inline inline = container.FirstChild; inline != null; inline = inline.NextSibling)
		{
			AppendInline(target, inline);
		}
	}

	private static void AppendInline(InlineCollection target, Markdig.Syntax.Inlines.Inline inline)
	{
		switch (inline)
		{
		case LiteralInline literal:
			target.Add(new Run(literal.Content.ToString()));
			break;
		case CodeInline code:
			Run codeRun = new(code.Content) { FontFamily = CodeFontFamily };
			codeRun.SetResourceReference(Run.BackgroundProperty, CodeBackgroundResource);
			codeRun.SetResourceReference(Run.ForegroundProperty, EditorForegroundResource);
			target.Add(codeRun);
			break;
		case EmphasisInline emphasis:
			target.Add(CreateEmphasis(emphasis));
			break;
		case LinkInline link:
			target.Add(CreateLink(link));
			break;
		case AutolinkInline autolink:
			target.Add(CreateAutolink(autolink));
			break;
		case LineBreakInline:
			target.Add(new LineBreak());
			break;
		case HtmlEntityInline entity:
			target.Add(new Run(entity.Transcoded.ToString()));
			break;
		case HtmlInline html:
			target.Add(new Run(html.Tag));
			break;
		case TaskList task:
			Run taskRun = new(task.Checked ? "[x] " : "[ ] ") { FontFamily = CodeFontFamily };
			taskRun.SetResourceReference(Run.ForegroundProperty, MutedForegroundResource);
			target.Add(taskRun);
			break;
		case ContainerInline nested:
			AppendInlines(target, nested);
			break;
		default:
			string fallback = inline.ToString();
			if (!string.IsNullOrEmpty(fallback))
			{
				target.Add(new Run(fallback));
			}
			break;
		}
	}

	private static Span CreateEmphasis(EmphasisInline emphasis)
	{
		Span span;
		if (emphasis.DelimiterChar == '~')
		{
			span = new Span { TextDecorations = TextDecorations.Strikethrough };
		}
		else if (emphasis.DelimiterCount >= 2)
		{
			span = new Bold();
		}
		else
		{
			span = new Italic();
		}
		AppendInlines(span.Inlines, emphasis);
		return span;
	}

	private static Span CreateLink(LinkInline link)
	{
		if (link.IsImage)
		{
			Span imageText = new();
			Run imageLabel = new("Image: ");
			imageLabel.SetResourceReference(Run.ForegroundProperty, MutedForegroundResource);
			imageText.Inlines.Add(imageLabel);
			AppendInlines(imageText.Inlines, link);
			if (!string.IsNullOrWhiteSpace(link.Url))
			{
				Run imageUrl = new(" (" + link.Url + ")");
				imageUrl.SetResourceReference(Run.ForegroundProperty, MutedForegroundResource);
				imageText.Inlines.Add(imageUrl);
			}
			return imageText;
		}
		WpfHyperlink hyperlink = new();
		hyperlink.SetResourceReference(WpfHyperlink.ForegroundProperty, LinkForegroundResource);
		AppendInlines(hyperlink.Inlines, link);
		if (hyperlink.Inlines.Count == 0 && !string.IsNullOrWhiteSpace(link.Url))
		{
			hyperlink.Inlines.Add(new Run(link.Url));
		}
		AttachNavigation(hyperlink, link.Url);
		return hyperlink;
	}

	private static Span CreateAutolink(AutolinkInline autolink)
	{
		string urlText = autolink.Url.ToString();
		WpfHyperlink hyperlink = new(new Run(urlText));
		hyperlink.SetResourceReference(WpfHyperlink.ForegroundProperty, LinkForegroundResource);
		string url = autolink.IsEmail ? "mailto:" + urlText : urlText;
		AttachNavigation(hyperlink, url);
		return hyperlink;
	}

	private static void AttachNavigation(WpfHyperlink hyperlink, string url)
	{
		if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
		{
			return;
		}
		hyperlink.NavigateUri = uri;
		hyperlink.RequestNavigate += (_, args) =>
		{
			if (OfficialAnnotationLinks.TryGetFileName(args.Uri, out string fileName))
			{
				OfficialAnnotationLinks.RequestOpen(fileName);
				args.Handled = true;
				return;
			}
			try
			{
				Process.Start(new ProcessStartInfo(args.Uri.AbsoluteUri) { UseShellExecute = true });
				args.Handled = true;
			}
			catch
			{
				args.Handled = true;
			}
		};
	}

}
