using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Views.Store;

public class ViewStoreList : UserControl, IComponentConnector
{
	internal ListBox listBox;

	private bool adPG8gtyK;

	public ViewStoreList()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!adPG8gtyK)
		{
			adPG8gtyK = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/store/viewstorelist.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			listBox = (ListBox)target;
		}
		else
		{
			adPG8gtyK = true;
		}
	}
}
