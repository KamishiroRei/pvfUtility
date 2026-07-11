using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Grid;

namespace PvfCode.Views.Preview.Controls;

public class NpcShopGridControlEx : UserControl, IComponentConnector
{
	internal GridControl Grid;

	internal CardView View;

	private bool JvIHMaWZ6I;

	public NpcShopGridControlEx()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!JvIHMaWZ6I)
		{
			JvIHMaWZ6I = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/preview/controls/npcshopgridcontrolex.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			Grid = (GridControl)target;
			break;
		case 2:
			View = (CardView)target;
			break;
		default:
			JvIHMaWZ6I = true;
			break;
		}
	}
}
