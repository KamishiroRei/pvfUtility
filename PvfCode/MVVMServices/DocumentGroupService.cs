using System.Linq;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Docking;
using PvfCode.MVVMServices;
using PvfCode.ViewModels.DocumentFolder;

namespace FKRF7IQiMoSJtPdh45P;

internal class NRjpElQyfkexPvgV2wp : ServiceBase, IDocumentGroupService
{
	private DocumentGroup Group => (DocumentGroup)base.AssociatedObject;

	public void ShowContextMenu()
	{
		_ = Group.ContextMenuCustomizations;
	}

	public void NextDocument(DocumentBase document)
	{
		DocumentBase selectedDocument = document;
		BaseLayoutItem[] items = Group.GetItems();
		if (Group.SelectedItem != null)
		{
			selectedDocument = Group.SelectedItem.DataContext as DocumentBase;
		}
		if (items != null && items.Any())
		{
			int selectedIndex = items.FindIndex((BaseLayoutItem item) => item.DataContext == selectedDocument);
			if (selectedIndex < items.Length - 1 && items[selectedIndex + 1].DataContext is DocumentBase nextDocument)
			{
				nextDocument.IsActive = true;
			}
		}
	}

	public void LastDocument(DocumentBase document)
	{
		DocumentBase selectedDocument = document;
		BaseLayoutItem[] items = Group.GetItems();
		if (Group.SelectedItem != null)
		{
			selectedDocument = Group.SelectedItem.DataContext as DocumentBase;
		}
		if (items != null && items.Any())
		{
			int selectedIndex = items.FindIndex((BaseLayoutItem item) => item.DataContext == selectedDocument);
			if (selectedIndex > 0 && items[selectedIndex - 1].DataContext is DocumentBase previousDocument)
			{
				previousDocument.IsActive = true;
			}
		}
	}

	public NRjpElQyfkexPvgV2wp()
	{
	}
}
