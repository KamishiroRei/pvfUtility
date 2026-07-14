using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
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
	private readonly DocumentHighlighter highlighter;

	private readonly TextEditorBase editor;

	private readonly PvfFile file;

	private readonly PvfFileType fileType;

	private HashSet<string> interestedHighlightingNames;

	private readonly FilePathLinkGoToDocumentPathDelegate goToDocumentPath;

	public ScriptCommentLineGenerator(TextEditorBase editorBase, DocumentHighlighter highlighter, PvfFile file)
	{
		interestedHighlightingNames = new HashSet<string>
		{
			"Section",
			"SectionEnd",
			"String"
		};
		this.file = file;
		fileType = file.FileType;
		this.highlighter = highlighter;
		editor = editorBase;
		goToDocumentPath = GoToDocumentPath;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		DocumentLine line = base.CurrentContext.Document.GetLineByOffset(offset);
		if (base.CurrentContext.TextView.GetLineIsCollapsed(line.LineNumber))
		{
			return null;
		}
		HighlightedLine highlightedLine = highlighter.HighlightLine(line.LineNumber);
		if (highlightedLine != null && highlightedLine.Sections != null && highlightedLine.Sections.Count > 0)
		{
			foreach (HighlightedSection section in highlightedLine.Sections)
			{
				if (section.Offset != offset)
				{
					continue;
				}
				string highlightingName = section.Color.Name;
				if (highlightingName != "String")
				{
					if (highlightingName != "SectionEnd" && highlightingName != "Section")
					{
						continue;
					}
					return new ScriptCommentVisualLine(base.CurrentContext.VisualLine, section.Length);
				}
				if (string.IsNullOrEmpty(Path.GetExtension(base.CurrentContext.Document.GetText(section).Replace("`", string.Empty))))
				{
					return new ScriptCommentVisualLine(base.CurrentContext.VisualLine, section.Length);
				}
				if (AppSetting.Instance.PvfConfig.LstFileUseScriptFile.Contains(file.FileName))
				{
					return null;
				}
				return new FilePathLinkVisualLine(file, goToDocumentPath, base.CurrentContext.VisualLine, section.Length - 1);
			}
		}
		return null;
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		DocumentLine line = base.CurrentContext.Document.GetLineByOffset(startOffset);
		if (base.CurrentContext.TextView.GetLineIsCollapsed(line.LineNumber))
		{
			return -1;
		}
		if (line.Length > 0)
		{
			HighlightedLine highlightedLine = highlighter.HighlightLine(line.LineNumber);
			if (highlightedLine != null && highlightedLine.Sections != null && highlightedLine.Sections.Count > 0)
			{
				foreach (HighlightedSection section in highlightedLine.Sections)
				{
					if (section.Offset >= startOffset && interestedHighlightingNames.Contains(section.Color.Name))
					{
						string highlightingName = section.Color.Name;
						if (highlightingName == "String")
						{
							return section.Offset;
						}
						if (highlightingName == "SectionEnd" || highlightingName == "Section")
						{
							return section.Offset;
						}
					}
				}
			}
		}
		return -1;
	}

	private void GoToDocumentPath(TextSegment segment, string? fullPath = null)
	{
		if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
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
			ScriptGoToFile(fullPath);
			SelectSegment(new TextSegment
			{
				StartOffset = segment.StartOffset + 1,
				EndOffset = segment.EndOffset - 1
			});
			return;
		}
		string referencedPath = editor.Document.GetText(segment).Replace("`", "");
		if (string.IsNullOrEmpty(referencedPath))
		{
			return;
		}
		PvfGroup pvf = AppCore.ViewModelBase.PVF;
		if (file != null && file.FileType == PvfFileType.lst)
		{
			if (pvf.FindFilePath(referencedPath, out string matchedPath))
			{
				AppCore.ViewModelBase.RootDocument.AddDocument(matchedPath, gotoNode: true);
			}
			else
			{
				AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), referencedPath));
			}
		}
		else if (fileType == PvfFileType.nut)
		{
			referencedPath = referencedPath.Replace("\"", "");
			if (!string.IsNullOrEmpty(Path.GetExtension(referencedPath)))
			{
				referencedPath = referencedPath.ToLower().Replace("\\", "/").Replace("../", string.Empty);
				if (pvf.FileAny(referencedPath))
				{
					AppCore.ViewModelBase.RootDocument.AddDocument(referencedPath, gotoNode: true);
				}
				else
				{
					string sqrPath = "sqr/" + referencedPath;
					string matchedPath;
					if (pvf.FileAny(sqrPath))
					{
						referencedPath = sqrPath;
						AppCore.ViewModelBase.RootDocument.AddDocument(referencedPath, gotoNode: true);
					}
					else if (pvf.FindFilePath(referencedPath, out matchedPath))
					{
						AppCore.ViewModelBase.RootDocument.AddDocument(matchedPath, gotoNode: true);
					}
					else
					{
						AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), referencedPath));
					}
				}
			}
		}
		else
		{
			ScriptGoToFile(referencedPath);
		}
		SelectSegment(new TextSegment
		{
			StartOffset = segment.StartOffset + 1,
			EndOffset = segment.EndOffset - 1
		});
	}

	private void ScriptGoToFile(string filePath)
	{
		try
		{
			if (file == null)
			{
				return;
			}
			filePath = filePath.Replace("\\", "/").ToLower();
			PvfGroup pvf = AppCore.ViewModelBase.PVF;
			string extension = Path.GetExtension(filePath).ToLower();
			if (extension != ".img")
			{
				string candidatePath = filePath;
				if (pvf.FileAny(candidatePath))
				{
					AppCore.ViewModelBase.RootDocument.AddDocument(candidatePath, gotoNode: true);
					return;
				}
				candidatePath = Path.Combine(Path.GetDirectoryName(file.FileName), filePath).ToLower().Replace("\\", "/");
				if (pvf.FileAny(candidatePath))
				{
					AppCore.ViewModelBase.RootDocument.AddDocument(candidatePath, gotoNode: true);
				}
				else if (pvf.FindFilePath(filePath, out candidatePath))
				{
					AppCore.ViewModelBase.RootDocument.AddDocument(candidatePath, gotoNode: true);
				}
				else if (filePath.Contains("../"))
				{
					int parentDirectoryCount = new Regex("\\.\\./", RegexOptions.Compiled).Matches(filePath).Count;
					string directoryName = Path.GetDirectoryName(file.FileName);
					for (int i = 0; i < parentDirectoryCount; i++)
					{
						if (string.IsNullOrEmpty(directoryName))
						{
							AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), filePath));
							return;
						}
						directoryName = Path.GetDirectoryName(directoryName);
					}
					string relativePath = filePath.Replace("../", "");
					string resolvedPath = ((!string.IsNullOrEmpty(directoryName)) ? Path.Combine(directoryName, relativePath).Replace("\\", "/").ToLower() : relativePath);
					if (pvf.FileAny(resolvedPath))
					{
						AppCore.ViewModelBase.RootDocument.AddDocument(resolvedPath, gotoNode: true);
					}
					else
					{
						AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), resolvedPath));
					}
				}
				else if (fileType == PvfFileType.twn && extension == ".map")
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
					string mapPath = "map/" + filePath.Replace("/" + fileNameWithoutExtension, "/(r)" + fileNameWithoutExtension);
					if (pvf.FileAny(mapPath))
					{
						AppCore.ViewModelBase.RootDocument.AddDocument(mapPath, gotoNode: true);
					}
					else
					{
						AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), filePath));
					}
				}
				else
				{
					AppCore.Logger.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), filePath));
				}
			}
			else if (extension.ToLower() == ".img")
			{
				OpenImagePackFile(filePath.Replace("`", "").ToLower());
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "HoverHyperLinkGroup.ScriptGoToFile");
		}
	}

	private void OpenImagePackFile(string filePath)
	{
		ResultData result = ImagePack2Service.Instance.ImgGoToNpkFile(filePath.Replace("%04d", "0001").Replace("%02d%02d", "0001"));
		if (result.IsError)
		{
			AppCore.ShowMsg(result.Msg, isError: true);
		}
	}

	private void SelectSegment(TextSegment segment)
	{
		editor.Dispatcher.BeginInvoke((Action)delegate
		{
			editor.Select(segment.StartOffset, segment.Length);
		});
	}

	public void Dispose()
	{
	}
}
