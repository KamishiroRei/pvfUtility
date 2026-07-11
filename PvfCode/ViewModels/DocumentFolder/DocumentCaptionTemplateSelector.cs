using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PvfCode.ViewModels.DocumentFolder.Enums;

namespace PvfCode.ViewModels.DocumentFolder;

public class DocumentCaptionTemplateSelector : DataTemplateSelector
{
	[CompilerGenerated]
	private DataTemplate uvNfoJc1T9;

	[CompilerGenerated]
	private DataTemplate kbBfs4Sjkp;

	public DataTemplate Default
	{
		[CompilerGenerated]
		get
		{
			return uvNfoJc1T9;
		}
		[CompilerGenerated]
		set
		{
			uvNfoJc1T9 = value;
		}
	}

	public DataTemplate PvfFile
	{
		[CompilerGenerated]
		get
		{
			return kbBfs4Sjkp;
		}
		[CompilerGenerated]
		set
		{
			kbBfs4Sjkp = value;
		}
	}

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
