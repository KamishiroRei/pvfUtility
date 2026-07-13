using System;
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

	internal TextDocument Document => Editor.Document;

	internal TextArea TextArea => Editor.TextArea;

	public ScriptCodeCompletionBase(TextEditorBase editor, PvfFileType? fileType)
	{
		FileType = fileType;
		Editor = editor;
		AttachEvents();
	}

	private void AttachEvents()
	{
		try
		{
			Editor.TextArea.TextEntering += OnTextEntering;
			Editor.TextArea.TextEntered += OnTextEntered;
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "ScriptCodeCompletionBase.CompletionInit");
		}
	}

	private void OnTextEntered(object sender, TextCompositionEventArgs e)
	{
		try
		{
			if (e.Text == "\n" || e.Text == "\r\n")
			{
				return;
			}
			if (FileType.HasValue && FileType == PvfFileType.nut && e.Text == ".")
			{
				ShowCompletionWindow(e);
				return;
			}
			int offset = Editor.TextArea.Caret.Offset;
			if (offset - 2 >= 0 && !string.IsNullOrWhiteSpace(Document.GetText(offset - 2, 1)) && completionWindow == null)
			{
				CloseCompletionWindow();
			}
			else
			{
				ShowCompletionWindow(e);
			}
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "ScriptCodeCompletionBase.CompletionInit");
		}
	}

	private void OnTextEntering(object sender, TextCompositionEventArgs e)
	{
		try
		{
			if (e.Text.Length > 0)
			{
				InsertMatchingDelimiter(e.Text);
			}
		}
		catch (Exception exception)
		{
			AppCore.Logger.ErrorUploadDialog(exception, "ScriptCodeCompletionBase.TextEditor_TextArea_TextEntering");
		}
	}

	internal abstract void ShowCompletionWindow(TextCompositionEventArgs e);

	internal abstract void InsertMatchingDelimiter(string text);

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

	internal void CloseCompletionWindow()
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
			Editor.TextArea.TextEntering -= OnTextEntering;
			Editor.TextArea.TextEntered -= OnTextEntered;
		}
	}
}
