using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.CodeCompletionModels;
using PvfCode.ViewModels.DocumentFolder.CodeCompletion;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

internal class ScriptCodeCompletion : ScriptCodeCompletionBase
{
	public ScriptCodeCompletion(TextEditorBase editor, PvfFileType? fileType)
		: base(editor, fileType)
	{
	}

	internal override void ShowCompletionWindow(TextCompositionEventArgs e)
	{
		try
		{
			int offset = Editor.TextArea.Caret.Offset;
			IEnumerable<HighlightedSection> higSections = Editor.GetHigSections(offset);
			HighlightingType? highlightingType = null;
			if (offset > 0)
			{
				int offset2 = --offset;
				if (Editor.OffSetIsHiglig(HighlightingType.FilePath, higSections, offset2))
				{
					CloseCompletionWindow();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Curlybraces, higSections, offset2))
				{
					CloseCompletionWindow();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Digits, higSections, offset2))
				{
					CloseCompletionWindow();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Comment, higSections, offset2))
				{
					CloseCompletionWindow();
					return;
				}
			}
			if (completionWindow != null && PresentationSource.FromVisual(completionWindow) != null)
			{
				return;
			}
			if (Editor.OffSetIsHiglig(HighlightingType.Section, higSections, offset))
			{
				highlightingType = HighlightingType.Section;
				if (offset > 0 && Editor.Document.GetText(offset - 1, 1) != "[")
				{
					return;
				}
			}
			else if (Editor.OffSetIsHiglig(HighlightingType.SectionEnd, higSections, offset))
			{
				highlightingType = HighlightingType.SectionEnd;
				if (offset > 0)
				{
					string text = Editor.Document.GetText(offset - 1, 1);
					if (text != "[" || text != "/")
					{
						return;
					}
				}
			}
			else if (Editor.OffSetIsHiglig(HighlightingType.String, higSections, offset))
			{
				highlightingType = HighlightingType.String;
				if (offset > 0 && Editor.Document.GetText(offset - 1, 1) != "`")
				{
					return;
				}
			}
			if ((e.Text != " " || completionWindow != null) && (completionWindow != null || AppSetting.Instance.EditConfig.CompletionDatas.Any(item => item.Text.Contains(e.Text))))
			{
				List<CodeCompletionData> completionData = AppSetting.Instance.EditConfig.CompletionDatas.FindAll(item => item.CodeCompletScriptType == CodeCompletScriptType.Script);
				completionData.Sort((left, right) => left.Text.CompareTo(right.Text));
				int endOffset;
				int startOffset;
				if (!highlightingType.HasValue)
				{
					startOffset = endOffset = base.TextArea.Caret.Offset;
					startOffset--;
				}
				else
				{
					HighlightedSection higSection = Editor.GetHigSection(highlightingType.Value, offset);
					startOffset = higSection.Offset;
					endOffset = higSection.EndOffset;
				}
				completionWindow = new WindowCompletion(Editor, base.TextArea, FileType, startOffset, endOffset);
				completionWindow.CompletionList.CompletionData.AddRange(completionData.ToArray());
				Editor.CodeCompletionIsOpen = true;
				completionWindow.Show();
				completionWindow.Closed += delegate
				{
					Editor.CodeCompletionIsOpen = false;
					completionWindow = null;
				};
			}
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "ScriptCodeCompletion.ShowCompletionCodeWindow");
		}
	}

	internal override void InsertMatchingDelimiter(string text)
	{
		try
		{
			int offset = Editor.TextArea.Caret.Offset;
			if (text == "`")
			{
				Editor.Document.Insert(offset, "`");
				base.TextArea.Caret.Offset--;
			}
			else if (text == "[" && (offset + 1 > base.Document.TextLength || !(base.Document.GetText(offset, 1) == "]")))
			{
				base.Document.Insert(offset, "]");
				base.TextArea.Caret.Offset--;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "ScriptCodeCompletion.CompletionCode");
		}
	}
}
