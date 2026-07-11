using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private string MS8uQQbJ2S;

	public NutCodeCompletion(TextEditorBase P_0, PvfFileType? P_1)
		: base(P_0, P_1)
	{
		MS8uQQbJ2S = "";
		g0Iiy1tHWG().Add('\'');
		g0Iiy1tHWG().Add('[');
		g0Iiy1tHWG().Add(']');
		g0Iiy1tHWG().Add('.');
	}

	[SpecialName]
	[CompilerGenerated]
	private string hxYuuCX7jv()
	{
		return MS8uQQbJ2S;
	}

	[SpecialName]
	[CompilerGenerated]
	private void IwZuGgjTiv(string P_0)
	{
		MS8uQQbJ2S = P_0;
	}

	internal override void f5YZC1mx9X(TextCompositionEventArgs P_0)
	{
		try
		{
			IwZuGgjTiv(hxYuuCX7jv() + P_0.Text);
			int offset = Editor.TextArea.Caret.Offset;
			HighlightingType? highlightingType = null;
			if (offset > 0)
			{
				int offset2 = --offset;
				if (Editor.OffSetIsHiglig(HighlightingType.FilePath, offset2))
				{
					s7HiYlJT7I();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Curlybraces, offset2))
				{
					s7HiYlJT7I();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Digits, offset2))
				{
					s7HiYlJT7I();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Comment, offset2))
				{
					s7HiYlJT7I();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.CommentMarkerSetHackUndone, offset2))
				{
					s7HiYlJT7I();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.CommentMarkerSetTodo, offset2))
				{
					s7HiYlJT7I();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.MethodCall, offset2))
				{
					s7HiYlJT7I();
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
			if (P_0.Text == " " && completionWindow == null)
			{
				s7HiYlJT7I();
				return;
			}
			List<CodeCompletionData> list = AppSetting.Instance.EditConfig.CompletionDatas.FindAll((CodeCompletionData it) => it.CodeCompletScriptType == CodeCompletScriptType.Nut);
			list.Sort((CodeCompletionData a, CodeCompletionData b) => a.Text.CompareTo(b.Text));
			if (completionWindow != null || list == null)
			{
				return;
			}
			int endOffset;
			int startOffSet;
			if (!highlightingType.HasValue)
			{
				if (P_0.Text == ".")
				{
					startOffSet = (endOffset = base.TextArea.Caret.Offset + 1);
				}
				else
				{
					startOffSet = (endOffset = base.TextArea.Caret.Offset);
					startOffSet--;
				}
			}
			else
			{
				HighlightedSection higSection = Editor.GetHigSection(highlightingType.Value, offset);
				startOffSet = higSection.Offset;
				endOffset = higSection.EndOffset;
			}
			completionWindow = new WindowCompletion(Editor, base.TextArea, FileType, startOffSet, endOffset);
			completionWindow.CompletionList.CompletionData.AddRange(list.ToArray());
			Editor.CodeCompletionIsOpen = true;
			completionWindow.Show();
			completionWindow.Closed += delegate
			{
				Editor.CodeCompletionIsOpen = false;
				completionWindow = null;
				IwZuGgjTiv("");
			};
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "NutCodeCompletion.ShowCompletionCodeWindow");
		}
	}

	internal override void ilPZHkiS99(string P_0)
	{
		try
		{
			int offset = Editor.TextArea.Caret.Offset;
			if (Editor.OffSetIsHiglig(HighlightingType.Section, offset) || Editor.OffSetIsHiglig(HighlightingType.SectionEnd, offset) || Editor.OffSetIsHiglig(HighlightingType.String, offset))
			{
				return;
			}
			if (P_0 == "'")
			{
				Editor.Document.Insert(offset, "'");
				base.TextArea.Caret.Offset--;
			}
			else if (P_0 == "[")
			{
				if (offset + 1 > base.Document.TextLength || !(base.Document.GetText(offset, 1) == "]"))
				{
					base.Document.Insert(offset, "]");
					base.TextArea.Caret.Offset--;
				}
			}
			else if (P_0 == "{")
			{
				base.Document.Insert(offset, "}");
				base.TextArea.Caret.Offset--;
			}
			else if (P_0 == "\"")
			{
				base.Document.Insert(offset, "\"");
				base.TextArea.Caret.Offset--;
			}
			else if (P_0 == "(")
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

	[CompilerGenerated]
	private void yvHuiBYE6P(object? _003Cp0_003E, EventArgs P_1)
	{
		Editor.CodeCompletionIsOpen = false;
		completionWindow = null;
		IwZuGgjTiv("");
	}
}
