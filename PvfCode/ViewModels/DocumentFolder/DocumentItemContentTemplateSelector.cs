using System.Windows;
using System.Windows.Controls;
using PvfCode.ViewModels.DocumentFolder.Enums;

namespace PvfCode.ViewModels.DocumentFolder;

public class DocumentItemContentTemplateSelector : DataTemplateSelector
{
	private static readonly DataTemplate PreviewTemplate = CreatePreviewTemplate();
	private static readonly DataTemplate OfficialAnnotationTemplate = CreateTemplate(typeof(OfficialAnnotationDocumentView));
	private static readonly DataTemplate PvfTagCommentTemplate = CreateTemplate(typeof(PvfTagCommentDocumentView));

	public DataTemplate PvfFileDocumentDataTemplate { get; set; }

	// These properties are populated by the original compiled BAML resource.
	// They remain compatibility-only; offline document routing does not select them.
	public DataTemplate ViewMacroStoreDataTemplate { get; set; }

	public DataTemplate PvfDiffToolDataTemplate { get; set; }

	public DataTemplate BookMarkStoreTemplate { get; set; }

	public DataTemplate DocumentIndex { get; set; }

	public DataTemplate ViewStoreList { get; set; }

	public DataTemplate PvfRelease { get; set; }

	public DataTemplate Import { get; set; }

	public DataTemplate ChatGPT { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item is PvfPreviewDocument)
		{
			return PreviewTemplate;
		}
		if (item is OfficialAnnotationDocument)
		{
			return OfficialAnnotationTemplate;
		}
		if (item is PvfTagCommentDocument)
		{
			return PvfTagCommentTemplate;
		}
		return ((DocumentBase)item).DocumentType switch
		{
			PvfFileDocumentType.PVF文档 => PvfFileDocumentDataTemplate, 
			PvfFileDocumentType.起始页 => DocumentIndex, 
			PvfFileDocumentType.PVF差异比较器 => PvfDiffToolDataTemplate, 
			PvfFileDocumentType.发布 => PvfRelease, 
			PvfFileDocumentType.导入文件 => Import, 
			_ => base.SelectTemplate(item, container), 
		};
	}

	private DataTemplate FindDataTemplate(DependencyObject container, object resourceKey)
	{
		if (container is FrameworkContentElement contentElement)
		{
			return contentElement.TryFindResource(resourceKey) as DataTemplate;
		}
		if (container is FrameworkElement element)
		{
			return element.TryFindResource(resourceKey) as DataTemplate;
		}
		return null;
	}

	private static DataTemplate CreatePreviewTemplate()
	{
		return CreateTemplate(typeof(PvfPreviewDocumentView));
	}

	private static DataTemplate CreateTemplate(System.Type viewType)
	{
		return new DataTemplate { VisualTree = new FrameworkElementFactory(viewType) };
	}
}
