using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.OffsetColorizers;

public class MarkSameWord : DocumentColorizingTransformer
{
	private string selectedText;

	public void SetSelectedText(string text)
	{
		selectedText = text;
	}

	protected override void ColorizeLine(DocumentLine line)
	{
		if (string.IsNullOrEmpty(selectedText))
		{
			return;
		}
		SolidColorBrush selectionBrush = (SolidColorBrush)Application.Current.FindResource("EditorSelectionBrush");
		int offset = line.Offset;
		string text = base.CurrentContext.Document.GetText(line);
		int startIndex = 0;
		int num;
		while ((num = text.IndexOf(selectedText, startIndex, StringComparison.Ordinal)) >= 0)
		{
			ChangeLinePart(offset + num, offset + num + selectedText.Length,
				element => element.TextRunProperties.SetBackgroundBrush(selectionBrush));
			startIndex = num + 1;
		}
	}
}
