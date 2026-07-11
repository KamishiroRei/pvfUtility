using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using TextEditLib;

namespace PvfCode.Controls.TextEditorFolder;

internal class ScriptCommentController
{
	private readonly IHighlighter highlighter;

	private readonly TextEdit editor;

	private TextDocument Document => editor.Document;

	public ScriptCommentController(IHighlighter highlighter, TextEdit editor)
	{
		this.highlighter = highlighter;
		this.editor = editor;
	}

	internal void CommentCode()
	{
		try
		{
			if (IsSelected(out Selection selection))
			{
				if (selection.IsMultiline)
				{
					CommentSelection(selection);
					return;
				}

				int startOffset = editor.Document.GetOffset(selection.StartPosition.Location);
				int endOffset = editor.Document.GetOffset(selection.EndPosition.Location);
				int selectionStart = Math.Min(startOffset, endOffset);
				int selectionEnd = Math.Max(startOffset, endOffset);
				foreach (HighlightedSection section in editor.GetCaretHighlightedSections())
				{
					if (section.Color.Name == "Comment" &&
						section.Offset == selectionStart &&
						section.Length == selection.Length)
					{
						string text = Document.GetText(section);
						if (text.Substring(0, 2) == "/*")
						{
							text = text.Substring(2, text.Length - 2);
							text = text.Substring(0, text.Length - 1);
							Document.Replace(section, text);
						}
						else if (text.Substring(0, 2) == "//")
						{
							Document.Replace(section, text.Substring(2, text.Length - 2));
						}
						return;
					}
				}

				editor.Document.Insert(selectionStart, "/*");
				editor.Document.Insert(selectionEnd + 2, "*/");
				return;
			}

			DocumentLine documentLine = editor.GetCaretDocumentLine();
			if (documentLine != null)
			{
				if (editor.GetCaretHighlightedSections().Count > 0 && IsLineFullyCommented(documentLine.LineNumber))
				{
					ClearCurrentLineComments();
				}
				else if (!string.IsNullOrEmpty(Document.GetText(documentLine.Offset, documentLine.Length)))
				{
					editor.Document.Insert(documentLine.Offset, "//");
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "NutCommentCode.CommentCode");
		}
	}

	private void CommentSelection(Selection selection)
	{
		try
		{
			SelectionSegment segment = selection.Segments.ToList()[0];
			int startOffset = segment.StartOffset;
			int endOffset = segment.EndOffset;
			int startLine = Document.GetLineByOffset(startOffset).LineNumber;
			int endLine = Document.GetLineByOffset(endOffset).LineNumber;
			if (!SelectionContainsComment(startOffset, startLine, endLine))
			{
				editor.Document.Insert(startOffset, "/*");
				editor.Document.Insert(endOffset + 2, "*/");
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "NutCommentCode.MultLineCommentCode");
		}
	}

	private bool SelectionContainsComment(int startOffset, int startLine, int endLine)
	{
		try
		{
			List<HighlightedSection> sections = new List<HighlightedSection>();
			for (int line = startLine; line <= endLine; line++)
			{
				HighlightedLine highlightedLine = highlighter.HighlightLine(line);
				if (highlightedLine == null || highlightedLine.Sections.Count == 0)
				{
					return false;
				}
				sections.AddRange(highlightedLine.Sections);
			}

			return sections.Count > 0 &&
				sections[0].Offset == startOffset &&
				sections.Any(section => section.Color.Name == "Comment");
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "NutCommentCode.MultLineCommentCode");
			return false;
		}
	}

	public bool IsLineFullyCommented(int lineNumber)
	{
		try
		{
			IHighlighter lineHighlighter = (IHighlighter)editor.TextArea.GetService(typeof(IHighlighter));
			if (lineHighlighter == null)
			{
				return false;
			}

			HighlightedLine highlightedLine = lineHighlighter.HighlightLine(lineNumber);
			return !highlightedLine.Sections.Any(section => section.Color.Name != "Comment");
		}
		catch (Exception)
		{
			return false;
		}
	}

	private void ClearCurrentLineComments()
	{
		try
		{
			IList<HighlightedSection> sections = editor.GetCaretHighlightedSections();
			if (sections.Count <= 0)
			{
				return;
			}

			foreach (HighlightedSection section in sections)
			{
				if (section.Color.Name != "Comment")
				{
					continue;
				}

				string text = Document.GetText(section);
				if (text.Length < 2)
				{
					continue;
				}

				if (text.Substring(0, 2) == "//")
				{
					editor.Document.Replace(section, text.Substring(2, text.Length - 2));
				}
				else if (text.Substring(0, 2) == "/*")
				{
					text = text.Substring(2, text.Length - 2);
					text = text.Substring(0, text.Length - 2);
					editor.Document.Replace(section, text);
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "NutCommentCode.ClearLineCommentCode");
		}
	}

	private bool IsSelected(out Selection selection)
	{
		try
		{
			selection = editor.TextArea.Selection;
			return selection.Length > 0;
		}
		catch (Exception e)
		{
			selection = null;
			AppCore.Logger.ErrorUploadDialog(e, "NutCommentCode.IsSelected");
			return false;
		}
	}

	public void ClearComments()
	{
		try
		{
			if (IsSelected(out Selection selection))
			{
				selection.ReplaceSelectionWithText(
					selection.GetText().Replace("//", null).Replace("/*", null).Replace("*/", null));
			}
			else
			{
				ClearCurrentLineComments();
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "NutCommentCode.ClearCommentCode");
		}
	}
}
