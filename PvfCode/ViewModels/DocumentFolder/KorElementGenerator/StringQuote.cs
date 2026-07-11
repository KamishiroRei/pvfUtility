using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Editors.Flyout;

namespace PvfCode.ViewModels.DocumentFolder.KorElementGenerator;

public class StringQuote : UserControl, IComponentConnector
{
	internal Button TestBlock;

	internal FlyoutControl flyout;

	private bool ix14k6txEe;

	public StringQuote(string key, int index, SourceType sourceType)
	{
		base.DataContext = new StringQuoteViewModel(index, key, sourceType);
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!ix14k6txEe)
		{
			ix14k6txEe = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/viewmodels/documentfolder/korelementgenerator/stringquote.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			TestBlock = (Button)target;
			break;
		case 2:
			flyout = (FlyoutControl)target;
			break;
		default:
			ix14k6txEe = true;
			break;
		}
	}
}
