using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.LinkFolder;

public delegate void FilePathLinkGoToDocumentPathDelegate(TextSegment textSegment, string? fullPath = null);
