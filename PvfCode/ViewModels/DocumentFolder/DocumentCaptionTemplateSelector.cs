using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PvfCode.ViewModels.DocumentFolder.Enums;

namespace PvfCode.ViewModels.DocumentFolder;

public class DocumentCaptionTemplateSelector : DataTemplateSelector
{
	private static readonly DataTemplate FileNameCaption = CreateFileNameCaption();
	public DataTemplate Default { get; set; }

	public DataTemplate PvfFile { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item is DocumentBase documentBase)
		{
			if (documentBase is OfficialAnnotationDocument or PvfTagCommentDocument)
			{
				return FileNameCaption;
			}
			if (documentBase.DocumentType == PvfFileDocumentType.PVF文档)
			{
				return PvfFile;
			}
			return Default;
		}
		return null;
	}

	public DocumentCaptionTemplateSelector()
	{
	}

	private static DataTemplate CreateFileNameCaption()
	{
		FrameworkElementFactory text = new(typeof(TextBlock));
		text.SetValue(FrameworkElement.MarginProperty, new Thickness(4, 0, 0, 0));
		text.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
		text.SetBinding(TextBlock.TextProperty, new Binding(nameof(DocumentBase.FileName)));
		return new DataTemplate { VisualTree = text };
	}
}
