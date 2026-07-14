using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.OfficialAnnotations;
using PvfCode.ViewModels.DocumentFolder.Enums;

namespace PvfCode.ViewModels.DocumentFolder;

public sealed class OfficialAnnotationDocument : DocumentBase
{
	public const string OfficialAnnotationDocumentPath = "pvf-official-annotation://current";

	private readonly OfficialAnnotationCatalog catalog;

	public override string FileName => "官方注释文档";

	public IReadOnlyList<string> AvailableFiles => catalog.Files;

	public IHighlightingDefinition Highlighting
	{
		get => GetProperty(() => Highlighting);
		private set => SetProperty(() => Highlighting, value);
	}

	public string SelectedFile
	{
		get => GetProperty(() => SelectedFile);
		set
		{
			if (string.Equals(SelectedFile, value, StringComparison.Ordinal))
			{
				return;
			}
			SetProperty(() => SelectedFile, value);
			SelectFile(value);
		}
	}

	public string Content
	{
		get => GetProperty(() => Content);
		private set => SetProperty(() => Content, value);
	}

	public string Status
	{
		get => GetProperty(() => Status);
		private set => SetProperty(() => Status, value);
	}

	public OfficialAnnotationDocument(OfficialAnnotationCatalog catalog = null)
		: base(OfficialAnnotationDocumentPath)
	{
		this.catalog = catalog ?? new OfficialAnnotationCatalog();
		DocumentType = PvfFileDocumentType.官方注释文档;
		Content = string.Empty;
		ThemeSwitcher.Instance.PvfCodeThemeChangedEvent += OnThemeChanged;

		if (AvailableFiles.Count == 0)
		{
			Status = "未找到 Resources\\OfficialAnnotationTranslation 中的文档。";
			return;
		}

		SelectFile(AvailableFiles[0]);
	}

	public bool SelectFile(string requestedFile)
	{
		if (!catalog.TryRead(requestedFile, out string resolvedPath, out string content))
		{
			Status = string.IsNullOrWhiteSpace(requestedFile)
				? "请选择官方注释文档。"
				: $"未找到官方注释文档：{requestedFile}";
			return false;
		}

		if (!string.Equals(SelectedFile, resolvedPath, StringComparison.Ordinal))
		{
			SetProperty(() => SelectedFile, resolvedPath);
		}
		Content = content;
		Status = resolvedPath;
		UpdateHighlighting();
		return true;
	}

	public override void Dispose()
	{
		ThemeSwitcher.Instance.PvfCodeThemeChangedEvent -= OnThemeChanged;
	}

	private void OnThemeChanged()
	{
		UpdateHighlighting();
	}

	private void UpdateHighlighting()
	{
		string logicalPath = SelectedFile ?? string.Empty;
		if (logicalPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
		{
			logicalPath = logicalPath[..^4];
		}
		string extension = Path.GetExtension(logicalPath).TrimStart('.');
		Highlighting = Enum.TryParse(extension, ignoreCase: true, out PvfFileType fileType)
			? ThemeSwitcher.Instance.GetHighlightingDefinition(fileType)
			: ThemeSwitcher.Instance.GetHighlightingDefinition("Script");
		RaisePropertyChanged(nameof(Highlighting));
	}
}
