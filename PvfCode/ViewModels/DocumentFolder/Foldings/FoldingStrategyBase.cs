using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Rendering;
using Nito.AsyncEx;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.ViewModels.DocumentFolder.BackgroundRenderers;
using PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

namespace PvfCode.ViewModels.DocumentFolder.Foldings;

public class FoldingStrategyBase : IDisposable
{
	private sealed class SectionFoldingState
	{
		public SectionFoldingState(HighlightedSection section)
		{
			Section = section;
		}

		public HighlightedSection Section { get; }

		public int LineCount { get; set; }

		public DocumentLine? LastLine { get; set; }
	}

	public readonly TextEditorBase TextEditor;

	private readonly PvfFileType? fileType;

	private readonly AsyncLock foldingLock;

	public FoldingManager Manager { get; set; }

	public Dictionary<int, string> lineNumberSectionDic { get; set; }

	public Dictionary<int, KeyValuePair<string, string>> HigSectionDic { get; set; }

	public char OpeningBrace { get; set; }

	public char ClosingBrace { get; set; }

	public FoldingStrategyBase(TextEditorBase textEditorBase, PvfFileType? pvfFileType)
	{
		foldingLock = new AsyncLock();
		HigSectionDic = new Dictionary<int, KeyValuePair<string, string>>();
		lineNumberSectionDic = new Dictionary<int, string>();
		OpeningBrace = '{';
		ClosingBrace = '}';
		fileType = pvfFileType;
		TextEditor = textEditorBase;
		InitializeFoldingManager();
	}

	private async void InitializeFoldingManager()
	{
		try
		{
			if (TextEditor == null || TextEditor.Document == null)
			{
				return;
			}

			Manager = new FoldingManager(TextEditor.Document);
			TextEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
			Manager = FoldingManager.Install(TextEditor.TextArea);
			if (AppSetting.Instance.EditConfig.UseFoldingGuideLines)
			{
				TextEditor.TextArea.TextView.BackgroundRenderers.Add(
					new FoldingGuideLines(Manager, TextEditor.GetVisibleOffset));
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "初始化折叠管理器异常");
		}
	}

	public async Task UpdateFoldings()
	{
		try
		{
			using (await foldingLock.LockAsync())
			{
				if (!fileType.HasValue ||
					(fileType.Value == PvfFileType.lst &&
					 !AppSetting.Instance.PvfConfig.LstFileUseScriptFile.Contains(TextEditor.PvfFile.FileName)))
				{
					return;
				}

				List<NewFolding> foldings;
				switch (fileType.Value)
				{
				case PvfFileType.txt:
				case PvfFileType.lst:
				case PvfFileType.str:
				case PvfFileType.bin:
					return;
				case PvfFileType.nut:
					foldings = await CreateBraceFoldingsAsync();
					break;
				case PvfFileType.ani:
					foldings = await CreateAniFrameFoldingsAsync();
					break;
				default:
					foldings = await CreateScriptSectionFoldingsAsync();
					break;
				}

				if (foldings != null && Manager != null)
				{
					Manager.UpdateFoldings(foldings, -1);
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "更新折叠异常.UpdateFoldings");
		}
	}

	private async Task<List<NewFolding>> CreateScriptSectionFoldingsAsync()
	{
		List<NewFolding> foldings = new List<NewFolding>();
		try
		{
			if (TextEditor == null || TextEditor.Document == null || TextEditor.Document.Text == null)
			{
				return foldings;
			}

			lineNumberSectionDic = new Dictionary<int, string>();
			HigSectionDic = new Dictionary<int, KeyValuePair<string, string>>();
			IHighlightingDefinition highlighting = TextEditor.SyntaxHighlighting;
			string documentText = TextEditor.Document.Text;
			if (highlighting == null || string.IsNullOrEmpty(documentText))
			{
				return foldings;
			}

			await Task.Run(() => BuildScriptSectionFoldings(documentText, highlighting, foldings));
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "更新折叠时发生错误");
			return new List<NewFolding>();
		}

		return foldings;
	}

	private void BuildScriptSectionFoldings(
		string documentText,
		IHighlightingDefinition highlighting,
		List<NewFolding> foldings)
	{
		int errorId = 0;
		try
		{
			errorId = 1;
			TextDocument document = new TextDocument { Text = documentText };
			errorId = 2;
			DocumentHighlighter highlighter = new DocumentHighlighter(document, highlighting);
			errorId = 3;
			if (highlighter == null)
			{
				return;
			}

			const string sectionColor = "Section";
			const string sectionEndColor = "SectionEnd";
			Dictionary<string, SectionFoldingState> sectionStates =
				new Dictionary<string, SectionFoldingState>();
			string currentSection = null;

			foreach (DocumentLine line in document.Lines)
			{
				if (line.Length == 0)
				{
					continue;
				}

				HighlightedLine highlightedLine = highlighter.HighlightLine(line.LineNumber);
				if (highlightedLine == null || highlightedLine.Sections == null)
				{
					continue;
				}

				IList<HighlightedSection> sections = highlightedLine.Sections;
				if (sections.Count == 0)
				{
					continue;
				}

				HighlightedSection firstSection = sections[0];
				foreach (HighlightedSection section in sections)
				{
					errorId = 4;
					string colorName = section.Color.Name;
					errorId = 5;
					if (colorName == sectionColor)
					{
						string sectionText = document.GetText(section);
						errorId = 6;
						if (string.IsNullOrEmpty(sectionText))
						{
							continue;
						}

						if (sectionStates.TryGetValue(sectionText, out SectionFoldingState previousState))
						{
							if (previousState.LineCount > 2)
							{
								foldings.Add(new NewFolding
								{
									StartOffset = previousState.Section.Offset + previousState.Section.Length,
									EndOffset = previousState.LastLine.Offset + previousState.LastLine.Length,
									DefaultClosed = AppSetting.Instance.EditConfig.UseAutoCodeFolding
								});
							}
							sectionStates.Remove(sectionText);
						}

						errorId = 7;
						sectionStates.Add(sectionText, new SectionFoldingState(section));
						if (!lineNumberSectionDic.ContainsKey(line.LineNumber))
						{
							lineNumberSectionDic.Add(line.LineNumber, sectionText);
						}
						errorId = 8;
						currentSection = sectionText;
					}
					else if (colorName == sectionEndColor)
					{
						errorId = 9;
						string sectionEndText = document.GetText(section);
						errorId = 10;
						if (string.IsNullOrEmpty(sectionEndText))
						{
							continue;
						}

						string sectionKey = sectionEndText.Remove(1, 1);
						errorId = 11;
						if (sectionStates.TryGetValue(sectionKey, out SectionFoldingState state))
						{
							errorId = 12;
							foldings.Add(new NewFolding
							{
								StartOffset = state.Section.Offset + state.Section.Length,
								EndOffset = section.Offset + section.Length,
								DefaultClosed = AppSetting.Instance.EditConfig.UseAutoCodeFolding
							});
							sectionStates.Remove(sectionKey);
						}
						currentSection = null;
					}
				}

				errorId = 13;
				if (firstSection.Color.Name != sectionColor &&
					firstSection.Color.Name != sectionEndColor &&
					currentSection != null)
				{
					errorId = 14;
					if (sectionStates.TryGetValue(currentSection, out SectionFoldingState state))
					{
						errorId = 15;
						state.LineCount++;
						state.LastLine = line;
						errorId = 16;
					}
				}

				errorId = 17;
				if (!HigSectionDic.ContainsKey(line.LineNumber))
				{
					errorId = 18;
					string firstColorName = firstSection.Color.Name;
					errorId = 19;
					if (firstColorName == sectionColor || firstColorName == "String")
					{
						errorId = 20;
						string firstSectionText = document.GetText(firstSection);
						if (!string.IsNullOrEmpty(firstSectionText))
						{
							HigSectionDic.Add(
								line.LineNumber,
								new KeyValuePair<string, string>(firstColorName, firstSectionText));
						}
					}
				}
			}

			errorId = 21;
			foreach (SectionFoldingState state in sectionStates.Values.Where(state => state.LineCount > 2))
			{
				foldings.Add(new NewFolding
				{
					StartOffset = state.Section.Offset + state.Section.Length,
					EndOffset = state.LastLine.Offset + state.LastLine.Length,
					DefaultClosed = AppSetting.Instance.EditConfig.UseAutoCodeFolding
				});
			}

			errorId = 22;
			foldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
			errorId = 23;
			if (lineNumberSectionDic != null && lineNumberSectionDic.Count > 0)
			{
				lineNumberSectionDic = lineNumberSectionDic
					.OrderByDescending(pair => pair.Key)
					.ToDictionary(pair => pair.Key, pair => pair.Value);
			}
			if (HigSectionDic != null && HigSectionDic.Count > 0)
			{
				HigSectionDic = HigSectionDic
					.OrderByDescending(pair => pair.Key)
					.ToDictionary(pair => pair.Key, pair => pair.Value);
			}
			errorId = 24;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "更新折叠内部任务发生错误 ErrId:" + errorId);
		}
	}

	private async Task<List<NewFolding>> CreateAniFrameFoldingsAsync()
	{
		try
		{
			List<NewFolding> foldings = new List<NewFolding>();
			IHighlightingDefinition highlighting = TextEditor.SyntaxHighlighting;
			string documentText = TextEditor.Text;
			await Task.Run(() => BuildAniFrameFoldings(documentText, highlighting, foldings));
			return foldings;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "创建ani折叠");
			return new List<NewFolding>();
		}
	}

	private static void BuildAniFrameFoldings(
		string documentText,
		IHighlightingDefinition highlighting,
		List<NewFolding> foldings)
	{
		try
		{
			HighlightedSection previousFrameSection = null;
			TextDocument document = new TextDocument { Text = documentText };
			DocumentHighlighter highlighter = new DocumentHighlighter(document, highlighting);

			foreach (DocumentLine line in document.Lines)
			{
				HighlightedLine highlightedLine = highlighter.HighlightLine(line.LineNumber);
				if (highlightedLine != null && highlightedLine.Sections != null)
				{
					foreach (HighlightedSection section in highlightedLine.Sections)
					{
						if (section.Color.Name != "Section")
						{
							continue;
						}

						string sectionText = document.GetText(section);
						if (sectionText.Contains("FRAME") && sectionText.Length > 7 && !sectionText.Contains(" "))
						{
							if (previousFrameSection != null)
							{
								foldings.Add(new NewFolding
								{
									StartOffset = previousFrameSection.Offset + previousFrameSection.Length,
									EndOffset = line.PreviousLine.EndOffset,
									DefaultClosed = AppSetting.Instance.EditConfig.UseAutoCodeFolding
								});
							}
							previousFrameSection = section;
						}
					}
				}

				if (previousFrameSection != null && line.LineNumber == document.Lines.Count)
				{
					foldings.Add(new NewFolding
					{
						StartOffset = previousFrameSection.Offset + previousFrameSection.Length,
						EndOffset = line.EndOffset,
						DefaultClosed = AppSetting.Instance.EditConfig.UseAutoCodeFolding
					});
				}
			}

			foldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "创建ani折叠内部异常");
		}
	}

	private async Task<List<NewFolding>> CreateBraceFoldingsAsync()
	{
		try
		{
			List<NewFolding> foldings = new List<NewFolding>();
			string documentText = TextEditor.Document.Text;
			await Task.Run(() => BuildBraceFoldings(documentText, foldings));
			return foldings;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "创建花括号折叠");
			return new List<NewFolding>();
		}
	}

	private void BuildBraceFoldings(string documentText, List<NewFolding> foldings)
	{
		try
		{
			TextDocument document = new TextDocument { Text = documentText };
			Stack<int> openingOffsets = new Stack<int>();
			int currentLineOffset = 0;
			char openingBrace = OpeningBrace;
			char closingBrace = ClosingBrace;

			for (int offset = 0; offset < document.TextLength; offset++)
			{
				char value = document.GetCharAt(offset);
				if (value == openingBrace)
				{
					openingOffsets.Push(offset);
				}
				else if (value == closingBrace && openingOffsets.Count > 0)
				{
					int openingOffset = openingOffsets.Pop();
					if (openingOffset < currentLineOffset)
					{
						foldings.Add(new NewFolding(openingOffset, offset + 1)
						{
							DefaultClosed = AppSetting.Instance.EditConfig.UseAutoCodeFolding
						});
					}
				}
				else if (value == '\n' || value == '\r')
				{
					currentLineOffset = offset + 1;
				}
			}

			foldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "创建花括号折叠内部异常");
		}
	}

	public void Clear()
	{
		try
		{
			HigSectionDic?.Clear();
			lineNumberSectionDic?.Clear();
			Manager?.Clear();
			if (Manager == null)
			{
				return;
			}

			FoldingManager.Uninstall(Manager);
			RemoveFoldingGuideLines();
			Manager = null;
			InitializeFoldingManager();
		}
		catch (Exception)
		{
		}
	}

	public void Dispose()
	{
		HigSectionDic = null;
		lineNumberSectionDic = null;
		Manager?.Clear();
		if (Manager == null)
		{
			return;
		}

		FoldingManager.Uninstall(Manager);
		try
		{
			RemoveFoldingGuideLines();
		}
		catch (Exception)
		{
		}
		Manager = null;
	}

	private void RemoveFoldingGuideLines()
	{
		if (TextEditor == null)
		{
			return;
		}

		IBackgroundRenderer[] renderers = TextEditor.TextArea.TextView.BackgroundRenderers.ToArray();
		foreach (IBackgroundRenderer renderer in renderers)
		{
			if (renderer is FoldingGuideLines)
			{
				TextEditor.TextArea.TextView.BackgroundRenderers.Remove(renderer);
			}
		}
	}
}
