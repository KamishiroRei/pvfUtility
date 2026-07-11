using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

namespace PvfCode.ViewModels.DocumentFolder.Foldings;

public class BraceFoldingStrategy
{
	public char OpeningBrace { get; set; }

	public char ClosingBrace { get; set; }

	public BraceFoldingStrategy()
	{
		OpeningBrace = '{';
		ClosingBrace = '}';
	}

	public void UpdateFoldings(FoldingManager manager, TextDocument document)
	{
		IEnumerable<NewFolding> foldings = CreateNewFoldings(document, out int firstErrorOffset);
		((DispatcherObject)Application.Current).Dispatcher.Invoke(
			() => manager.UpdateFoldings(foldings, firstErrorOffset));
	}

	public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
	{
		firstErrorOffset = -1;
		return CreateNewFoldings(document);
	}

	public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
	{
		List<NewFolding> list = new List<NewFolding>();
		Stack<int> stack = new Stack<int>();
		int num = 0;
		char openingBrace = OpeningBrace;
		char closingBrace = ClosingBrace;
		for (int i = 0; i < document.TextLength; i++)
		{
			char charAt = document.GetCharAt(i);
			if (charAt == openingBrace)
			{
				stack.Push(i);
			}
			else if (charAt == closingBrace && stack.Count > 0)
			{
				int num2 = stack.Pop();
				if (num2 < num)
				{
					list.Add(new NewFolding(num2, i + 1)
					{
						DefaultClosed = AppSetting.Instance.EditConfig.UseAutoCodeFolding
					});
				}
			}
			else if (charAt == '\n' || charAt == '\r')
			{
				num = i + 1;
			}
		}
		list.Sort((NewFolding a, NewFolding b) => a.StartOffset.CompareTo(b.StartOffset));
		return list;
	}
}
