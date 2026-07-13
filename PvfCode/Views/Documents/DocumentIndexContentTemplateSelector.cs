using System.Windows;
using System.Windows.Controls;
using PvfCode.ViewModels.DocumentFolder;

namespace PvfCode.Views.Documents;

public class DocumentIndexContentTemplateSelector : DataTemplateSelector
{
	public DataTemplate View2 { get; set; }

	public DataTemplate WebBrowser { get; set; }

	public override DataTemplate SelectTemplate(object item, DependencyObject container)
	{
		if (item is DocumentIndexViewModel documentIndexViewModel)
		{
			if (documentIndexViewModel.InsertView2)
			{
				return View2;
			}
			return WebBrowser;
		}
		return null;
	}

	public DocumentIndexContentTemplateSelector()
	{
	}
}
