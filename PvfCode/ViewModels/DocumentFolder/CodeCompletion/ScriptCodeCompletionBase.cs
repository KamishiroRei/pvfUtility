using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using PvfCode.Controls.TextEditorFolder;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public abstract class ScriptCodeCompletionBase : IDisposable
{
	internal TextEditorBase Editor;

	internal PvfFileType? FileType;

	protected WindowCompletion completionWindow;

	[CompilerGenerated]
	private HashSet<char> J8diQE0WKt;

	internal TextDocument Document => Editor.Document;

	internal TextArea TextArea => Editor.TextArea;

	[SpecialName]
	[CompilerGenerated]
	internal HashSet<char> g0Iiy1tHWG()
	{
		return J8diQE0WKt;
	}

	[SpecialName]
	[CompilerGenerated]
	internal void WZxiiTZ6S8(HashSet<char> P_0)
	{
		J8diQE0WKt = P_0;
	}

	public ScriptCodeCompletionBase(TextEditorBase editor, PvfFileType? fileType)
	{
		J8diQE0WKt = new HashSet<char>();
		FileType = fileType;
		Editor = editor;
		tcKiS60xuW();
	}

	private void tcKiS60xuW()
	{
		try
		{
			Editor.TextArea.TextEntering += WJMi4vh2cA;
			Editor.TextArea.TextEntered += DlQiARpDvH;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "ScriptCodeCompletionBase.CompletionInit");
		}
	}

	private void DlQiARpDvH(object P_0, TextCompositionEventArgs P_1)
	{
		try
		{
			if (P_1.Text == "\n" || P_1.Text == "\r\n")
			{
				return;
			}
			if (FileType.HasValue && FileType == PvfFileType.nut && P_1.Text == ".")
			{
				f5YZC1mx9X(P_1);
				return;
			}
			int offset = Editor.TextArea.Caret.Offset;
			if (offset - 2 >= 0 && !string.IsNullOrWhiteSpace(Document.GetText(offset - 2, 1)) && completionWindow == null)
			{
				s7HiYlJT7I();
			}
			else
			{
				f5YZC1mx9X(P_1);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "ScriptCodeCompletionBase.CompletionInit");
		}
	}

	private void WJMi4vh2cA(object P_0, TextCompositionEventArgs P_1)
	{
		try
		{
			string text = P_1.Text;
			if (P_1.Text.Length > 0)
			{
				ilPZHkiS99(text);
			}
			if (P_1.Text.Length > 0 && completionWindow != null && !char.IsLetterOrDigit(P_1.Text[0]))
			{
				char item = P_1.Text[0];
				g0Iiy1tHWG().Contains(item);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "ScriptCodeCompletionBase.TextEditor_TextArea_TextEntering");
		}
	}

	internal abstract void f5YZC1mx9X(TextCompositionEventArgs e);

	internal abstract void ilPZHkiS99(string text);

	public void CloseWindow()
	{
		if (completionWindow != null)
		{
			try
			{
				completionWindow.Close();
			}
			catch (Exception)
			{
			}
			completionWindow = null;
		}
	}

	internal void s7HiYlJT7I()
	{
		if (completionWindow != null)
		{
			completionWindow.Close();
			completionWindow = null;
		}
	}

	public void Dispose()
	{
		if (Editor != null)
		{
			Editor.TextArea.TextEntering -= WJMi4vh2cA;
			Editor.TextArea.TextEntered -= DlQiARpDvH;
		}
	}
}
