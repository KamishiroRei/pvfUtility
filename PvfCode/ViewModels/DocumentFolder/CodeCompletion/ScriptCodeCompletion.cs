using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass1_0
	{
		public TextCompositionEventArgs sVgIwjZeX9;

		public ScriptCodeCompletion rqeIoiJBDO;

		public _003C_003Ec__DisplayClass1_0()
		{
		}

		internal bool olBI668rew(CodeCompletionData it)
		{
			return it.Text.Contains(sVgIwjZeX9.Text);
		}

		internal void vXEI1OaT0U(object? _003Cp0_003E, EventArgs _003Cp1_003E)
		{
			rqeIoiJBDO.Editor.CodeCompletionIsOpen = false;
			rqeIoiJBDO.completionWindow = null;
		}
	}

	public ScriptCodeCompletion(TextEditorBase P_0, PvfFileType? P_1)
		: base(P_0, P_1)
	{
		g0Iiy1tHWG().Add(' ');
		g0Iiy1tHWG().Add('`');
		g0Iiy1tHWG().Add('[');
		g0Iiy1tHWG().Add(']');
		g0Iiy1tHWG().Add('/');
	}

	internal override void f5YZC1mx9X(TextCompositionEventArgs P_0)
	{
		_003C_003Ec__DisplayClass1_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass1_0();
		CS_0024_003C_003E8__locals6.sVgIwjZeX9 = P_0;
		CS_0024_003C_003E8__locals6.rqeIoiJBDO = this;
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
					s7HiYlJT7I();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Curlybraces, higSections, offset2))
				{
					s7HiYlJT7I();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Digits, higSections, offset2))
				{
					s7HiYlJT7I();
					return;
				}
				if (Editor.OffSetIsHiglig(HighlightingType.Comment, higSections, offset2))
				{
					s7HiYlJT7I();
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
			if ((!(CS_0024_003C_003E8__locals6.sVgIwjZeX9.Text == " ") || completionWindow != null) && (completionWindow != null || AppSetting.Instance.EditConfig.CompletionDatas.Any((CodeCompletionData it) => it.Text.Contains(CS_0024_003C_003E8__locals6.sVgIwjZeX9.Text))))
			{
				List<CodeCompletionData> list = AppSetting.Instance.EditConfig.CompletionDatas.FindAll((CodeCompletionData it) => it.CodeCompletScriptType == CodeCompletScriptType.Script);
				list.Sort((CodeCompletionData a, CodeCompletionData b) => a.Text.CompareTo(b.Text));
				int endOffset;
				int num;
				if (!highlightingType.HasValue)
				{
					num = (endOffset = base.TextArea.Caret.Offset);
					num--;
				}
				else
				{
					HighlightedSection higSection = Editor.GetHigSection(highlightingType.Value, offset);
					num = higSection.Offset;
					endOffset = higSection.EndOffset;
				}
				completionWindow = new WindowCompletion(Editor, base.TextArea, FileType, num, endOffset);
				completionWindow.CompletionList.CompletionData.AddRange(list.ToArray());
				Editor.CodeCompletionIsOpen = true;
				completionWindow.Show();
				completionWindow.Closed += delegate
				{
					CS_0024_003C_003E8__locals6.rqeIoiJBDO.Editor.CodeCompletionIsOpen = false;
					CS_0024_003C_003E8__locals6.rqeIoiJBDO.completionWindow = null;
				};
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "ScriptCodeCompletion.ShowCompletionCodeWindow");
		}
	}

	internal override void ilPZHkiS99(string P_0)
	{
		try
		{
			int offset = Editor.TextArea.Caret.Offset;
			if (P_0 == "`")
			{
				Editor.Document.Insert(offset, "`");
				base.TextArea.Caret.Offset--;
			}
			else if (P_0 == "[" && (offset + 1 > base.Document.TextLength || !(base.Document.GetText(offset, 1) == "]")))
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
