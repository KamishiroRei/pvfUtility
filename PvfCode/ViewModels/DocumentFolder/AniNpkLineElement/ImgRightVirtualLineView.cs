using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PvfCode.ViewModels.DocumentFolder.AniNpkLineElement;

public class ImgRightVirtualLineView : UserControl, IComponentConnector
{
	private bool EaIGSm1GAC;

	public ImgRightVirtualLineView(KeyValuePair<string, int> imgInfo)
	{
		base.DataContext = new ImgRightVirtualLineViewModel(imgInfo);
		InitializeComponent();
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!EaIGSm1GAC)
		{
			EaIGSm1GAC = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/viewmodels/documentfolder/aninpklineelement/imgrightvirtuallineview.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		EaIGSm1GAC = true;
	}
}
