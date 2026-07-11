using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.ViewModels.DocumentFolder.AniNpkLineElement;

public class WindowPrivewNpkImg : ThemedWindow, IComponentConnector
{
	private bool NBvGo1OtEx;

	public WindowPrivewNpkImg(string imgPath, string npkFilePath, int index)
	{
		base.DataContext = new WindowPrivewNpkImgViewModel(imgPath, npkFilePath, index);
		InitializeComponent();
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		Application.Current.MainWindow.Activate();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!NBvGo1OtEx)
		{
			NBvGo1OtEx = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/viewmodels/documentfolder/aninpklineelement/windowprivewnpkimg.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		NBvGo1OtEx = true;
	}
}
