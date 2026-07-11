using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.Dot;
using PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip.CommentHoverTooltip;
using PvfCode.ViewModels.DocumentFolder.LinkFolder;

namespace PvfCode.ViewModels.DocumentFolder.VisualLineElementGenerators;

public class ScriptCommentLineGenerator : VisualLineElementGenerator, IDisposable
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass15_0
	{
		public ScriptCommentLineGenerator xYCexPaKkh;

		public TextSegment LjVeQHP4bs;

		public _003C_003Ec__DisplayClass15_0()
		{
		}

		internal void a7SeGWwhEb()
		{
			xYCexPaKkh.Editor.Select(LjVeQHP4bs.StartOffset, LjVeQHP4bs.Length);
		}
	}

	private readonly DocumentHighlighter BBX5kF7S7Y;

	private readonly TextEditorBase Editor;

	private readonly PvfFile File;

	private readonly PvfFileType FileType;

	private HashSet<string> Dw350qcChI;

	private readonly FilePathLinkGoToDocumentPathDelegate nBK5722May;

	public ScriptCommentLineGenerator(TextEditorBase editorBase, DocumentHighlighter highlighter, PvfFile file)
	{
		Dw350qcChI = new HashSet<string>
		{
			"Section",
			"SectionEnd",
			"String"
		};
		File = file;
		FileType = file.FileType;
		BBX5kF7S7Y = highlighter;
		Editor = editorBase;
		nBK5722May = ICD59riq29;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		DocumentLine lineByOffset = base.CurrentContext.Document.GetLineByOffset(offset);
		if (base.CurrentContext.TextView.GetLineIsCollapsed(lineByOffset.LineNumber))
		{
			return null;
		}
		HighlightedLine highlightedLine = BBX5kF7S7Y.HighlightLine(lineByOffset.LineNumber);
		if (highlightedLine != null && highlightedLine.Sections != null && highlightedLine.Sections.Count > 0)
		{
			foreach (HighlightedSection section in highlightedLine.Sections)
			{
				if (section.Offset != offset)
				{
					continue;
				}
				string name = section.Color.Name;
				if (!(name == "String"))
				{
					if (!(name == "SectionEnd") && !(name == "Section"))
					{
						continue;
					}
					return new ScriptCommentVisualLine(base.CurrentContext.VisualLine, section.Length);
				}
				if (string.IsNullOrEmpty(Path.GetExtension(base.CurrentContext.Document.GetText(section).Replace("`", string.Empty))))
				{
					return new ScriptCommentVisualLine(base.CurrentContext.VisualLine, section.Length);
				}
				if (AppSetting.Instance.PvfConfig.LstFileUseScriptFile.Contains(File.FileName))
				{
					return null;
				}
				return new FilePathLinkVisualLine(File, nBK5722May, base.CurrentContext.VisualLine, section.Length - 1);
			}
		}
		return null;
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		DocumentLine lineByOffset = base.CurrentContext.Document.GetLineByOffset(startOffset);
		if (base.CurrentContext.TextView.GetLineIsCollapsed(lineByOffset.LineNumber))
		{
			return -1;
		}
		if (lineByOffset.Length > 0)
		{
			HighlightedLine highlightedLine = BBX5kF7S7Y.HighlightLine(lineByOffset.LineNumber);
			if (highlightedLine != null && highlightedLine.Sections != null && highlightedLine.Sections.Count > 0)
			{
				foreach (HighlightedSection section in highlightedLine.Sections)
				{
					if (section.Offset >= startOffset && Dw350qcChI.Contains(section.Color.Name))
					{
						string name = section.Color.Name;
						if (name == "String")
						{
							return section.Offset;
						}
						if (name == "SectionEnd" || name == "Section")
						{
							return section.Offset;
						}
					}
				}
			}
		}
		return -1;
	}

	private void ICD59riq29(TextSegment P_0, string? fullPath = null)
	{
		if (((int)Keyboard.Modifiers & 2) != 2)
		{
			return;
		}
		if (fullPath != null)
		{
			if (fullPath.Contains(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist").Replace("{0}", string.Empty)))
			{
				AppCore.ShowMsg(fullPath, isError: true);
				return;
			}
			LAw5PdHnmj(fullPath);
			Lye5J58Cg0(new TextSegment
			{
				StartOffset = P_0.StartOffset + 1,
				EndOffset = P_0.EndOffset - 1
			});
			return;
		}
		string text = Editor.Document.GetText(P_0).Replace("`", "");
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		if (File != null && File.FileType == PvfFileType.lst)
		{
			if (pVF.FindFilePath(text, out string fullPath2))
			{
				AppCore.ViewModelBase.RootDocument.AddDocument(fullPath2, gotoNode: true);
			}
			else
			{
				AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), text));
			}
		}
		else if (FileType == PvfFileType.nut)
		{
			text = text.Replace("\"", "");
			if (!string.IsNullOrEmpty(Path.GetExtension(text)))
			{
				text = text.ToLower().Replace("\\", "/").Replace("../", string.Empty);
				if (pVF.FileAny(text))
				{
					AppCore.ViewModelBase.RootDocument.AddDocument(text, gotoNode: true);
				}
				else
				{
					string text2 = "sqr/" + text;
					string fullPath3;
					if (pVF.FileAny(text2))
					{
						text = text2;
						AppCore.ViewModelBase.RootDocument.AddDocument(text, gotoNode: true);
					}
					else if (pVF.FindFilePath(text, out fullPath3))
					{
						AppCore.ViewModelBase.RootDocument.AddDocument(fullPath3, gotoNode: true);
					}
					else
					{
						AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), text));
					}
				}
			}
		}
		else
		{
			LAw5PdHnmj(text);
		}
		Lye5J58Cg0(new TextSegment
		{
			StartOffset = P_0.StartOffset + 1,
			EndOffset = P_0.EndOffset - 1
		});
	}

	private void LAw5PdHnmj(string P_0)
	{
		try
		{
			if (File == null)
			{
				return;
			}
			P_0 = P_0.Replace("\\", "/").ToLower();
			PvfGroup pVF = AppCore.ViewModelBase.PVF;
			string text = Path.GetExtension(P_0).ToLower();
			if (text != ".img")
			{
				string filePath = P_0;
				if (pVF.FileAny(filePath))
				{
					AppCore.ViewModelBase.RootDocument.AddDocument(filePath, gotoNode: true);
					return;
				}
				filePath = Path.Combine(Path.GetDirectoryName(File.FileName), P_0).ToLower().Replace("\\", "/");
				if (pVF.FileAny(filePath))
				{
					AppCore.ViewModelBase.RootDocument.AddDocument(filePath, gotoNode: true);
				}
				else if (pVF.FindFilePath(P_0, out filePath))
				{
					AppCore.ViewModelBase.RootDocument.AddDocument(filePath, gotoNode: true);
				}
				else if (P_0.Contains("../"))
				{
					int count = new Regex("\\.\\./", RegexOptions.Compiled).Matches(P_0).Count;
					string directoryName = Path.GetDirectoryName(File.FileName);
					for (int i = 0; i < count; i++)
					{
						if (string.IsNullOrEmpty(directoryName))
						{
							AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), P_0));
							return;
						}
						directoryName = Path.GetDirectoryName(directoryName);
					}
					string text2 = P_0.Replace("../", "");
					string text3 = ((!string.IsNullOrEmpty(directoryName)) ? Path.Combine(directoryName, text2).Replace("\\", "/").ToLower() : text2);
					if (pVF.FileAny(text3))
					{
						AppCore.ViewModelBase.RootDocument.AddDocument(text3, gotoNode: true);
					}
					else
					{
						AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), text3));
					}
				}
				else if (FileType == PvfFileType.twn && text == ".map")
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(P_0);
					string filePath2 = "map/" + P_0.Replace("/" + fileNameWithoutExtension, "/(r)" + fileNameWithoutExtension);
					if (pVF.FileAny(filePath2))
					{
						AppCore.ViewModelBase.RootDocument.AddDocument(filePath2, gotoNode: true);
					}
					else
					{
						AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), P_0));
					}
				}
				else
				{
					AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), P_0));
				}
			}
			else if (text.ToLower() == ".img")
			{
				ihT5ZDkX12(P_0.Replace("`", "").ToLower());
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "HoverHyperLinkGroup.ScriptGoToFile");
		}
	}

	private void ihT5ZDkX12(string P_0)
	{
		ResultData resultData = ImagePack2Service.Instance.ImgGoToNpkFile(P_0.Replace("%04d", "0001").Replace("%02d%02d", "0001"));
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg, isError: true);
		}
	}

	private void Lye5J58Cg0(TextSegment P_0)
	{
		_003C_003Ec__DisplayClass15_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass15_0();
		CS_0024_003C_003E8__locals5.xYCexPaKkh = this;
		CS_0024_003C_003E8__locals5.LjVeQHP4bs = P_0;
		((DispatcherObject)Editor).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			CS_0024_003C_003E8__locals5.xYCexPaKkh.Editor.Select(CS_0024_003C_003E8__locals5.LjVeQHP4bs.StartOffset, CS_0024_003C_003E8__locals5.LjVeQHP4bs.Length);
		}, Array.Empty<object>());
	}

	public void Dispose()
	{
	}
}
