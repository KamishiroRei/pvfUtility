using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.PvfTreeFolder;

public class CreateShopItem : ThemedWindow, IComponentConnector
{
	internal CreateShopItem win;

	private bool iIIHhVoE48;

	public CreateShopItem(List<string> files)
	{
		base.DataContext = new CreateShopItemViewModel(files);
		InitializeComponent();
	}

	protected override void OnClosed(EventArgs e)
	{
		base.OnClosed(e);
		Application.Current.MainWindow?.Activate();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!iIIHhVoE48)
		{
			iIIHhVoE48 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/pvftreefolder/createshopitem.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			win = (CreateShopItem)target;
		}
		else
		{
			iIIHhVoE48 = true;
		}
	}
}
