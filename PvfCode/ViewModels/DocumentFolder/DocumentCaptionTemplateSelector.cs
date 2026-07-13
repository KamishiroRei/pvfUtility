using System.Windows;
using System.Windows.Controls;
using PvfCode.ViewModels.DocumentFolder.Enums;

namespace PvfCode.ViewModels.DocumentFolder;

public class DocumentCaptionTemplateSelector : DataTemplateSelector
{
	public DataTemplate Default { get; set; }

	public DataTemplate PvfFile { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item is DocumentBase documentBase)
		{
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
}
