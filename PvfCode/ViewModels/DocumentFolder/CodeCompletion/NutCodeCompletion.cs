using System;
using System.Collections.Generic;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.CodeCompletionModels;
using PvfCode.ViewModels.DocumentFolder.CodeCompletion;
using Utools;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

internal class NutCodeCompletion : ScriptCodeCompletionBase
{
	public NutCodeCompletion(TextEditorBase editor, PvfFileType? fileType)
		: base(editor, fileType)
	{
	}

	internal override void ShowCompletionWindow(TextCompositionEventArgs e)
	{
		try
		{
			int offset = Editor.TextArea.Caret.Offset;
			HighlightingType? highlightingType = null;
			if (offset > 0)
			{
				int offset2 = --offset;
				if (Editor.OffSetIsHiglig(HighlightingType.FilePath, offset2))
				{
					CloseCompletionWindow();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Curlybraces, offset2))
				{
					CloseCompletionWindow();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Digits, offset2))
				{
					CloseCompletionWindow();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Comment, offset2))
				{
					CloseCompletionWindow();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.CommentMarkerSetHackUndone, offset2))
				{
					CloseCompletionWindow();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.CommentMarkerSetTodo, offset2))
				{
					CloseCompletionWindow();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.MethodCall, offset2))
				{
					CloseCompletionWindow();
					return;
				}
			}
			if (Editor.OffSetIsHiglig(HighlightingType.Keywords, offset))
			{
				highlightingType = HighlightingType.Keywords;
			}
			else if (Editor.OffSetIsHiglig(HighlightingType.Keywords2, offset))
			{
				highlightingType = HighlightingType.Keywords2;
			}
			else if (Editor.OffSetIsHiglig(HighlightingType.String, offset) && !Editor.OffSetIsHiglig(HighlightingType.FilePath, offset))
			{
				highlightingType = HighlightingType.String;
			}
			else if (Editor.OffSetIsHiglig(HighlightingType.GGenObject, offset))
			{
				highlightingType = HighlightingType.GGenObject;
			}
			if (e.Text == " " && completionWindow == null)
			{
				CloseCompletionWindow();
				return;
			}
			List<CodeCompletionData> completionData = AppSetting.Instance.EditConfig.CompletionDatas.FindAll(item => item.CodeCompletScriptType == CodeCompletScriptType.Nut);
			completionData.Sort((left, right) => left.Text.CompareTo(right.Text));
			if (completionWindow != null || completionData == null)
			{
				return;
			}
			int endOffset;
			int startOffset;
			if (!highlightingType.HasValue)
			{
				if (e.Text == ".")
				{
					startOffset = endOffset = base.TextArea.Caret.Offset + 1;
				}
				else
				{
					startOffset = endOffset = base.TextArea.Caret.Offset;
					startOffset--;
				}
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
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "NutCodeCompletion.ShowCompletionCodeWindow");
		}
	}

	internal override void InsertMatchingDelimiter(string text)
	{
		try
		{
			int offset = Editor.TextArea.Caret.Offset;
			if (Editor.OffSetIsHiglig(HighlightingType.Section, offset) || Editor.OffSetIsHiglig(HighlightingType.SectionEnd, offset) || Editor.OffSetIsHiglig(HighlightingType.String, offset))
			{
				return;
			}
			if (text == "'")
			{
				Editor.Document.Insert(offset, "'");
				base.TextArea.Caret.Offset--;
			}
			else if (text == "[")
			{
				if (offset + 1 > base.Document.TextLength || !(base.Document.GetText(offset, 1) == "]"))
				{
					base.Document.Insert(offset, "]");
					base.TextArea.Caret.Offset--;
				}
			}
			else if (text == "{")
			{
				base.Document.Insert(offset, "}");
				base.TextArea.Caret.Offset--;
			}
			else if (text == "\"")
			{
				base.Document.Insert(offset, "\"");
				base.TextArea.Caret.Offset--;
			}
			else if (text == "(")
			{
				base.Document.Insert(offset, ")");
				base.TextArea.Caret.Offset--;
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "NutCodeCompletion.CompletionCode");
		}
	}
}
