using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PvfCode.ViewModels.DocumentFolder;

namespace PvfCode.Views.Documents;

public class DocumentIndexContentTemplateSelector : DataTemplateSelector
{
	[CompilerGenerated]
	private DataTemplate SiJv2qYUdM;

	[CompilerGenerated]
	private DataTemplate mjxvfTcpkM;

	public DataTemplate View2
	{
		[CompilerGenerated]
		get
		{
			return SiJv2qYUdM;
		}
		[CompilerGenerated]
		set
		{
			SiJv2qYUdM = value;
		}
	}

	public DataTemplate WebBrowser
	{
		[CompilerGenerated]
		get
		{
			return mjxvfTcpkM;
		}
		[CompilerGenerated]
		set
		{
			mjxvfTcpkM = value;
		}
	}

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
