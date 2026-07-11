using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using DevExpress.Xpf.Core;

namespace PvfCode.Views.SearchPvf;

public class WindowAddNewSearchResult : ThemedWindow, IComponentConnector
{
	internal TextBox input;

	internal Button buttonYes;

	private bool aGoHloiCud;

	public WindowAddNewSearchResult()
	{
		InitializeComponent();
		buttonYes.IsEnabled = false;
	}

	private void Eg3C30emKb(object P_0, TextChangedEventArgs P_1)
	{
		buttonYes.IsEnabled = !string.IsNullOrEmpty(input.Text);
	}

	private void JcWCRpRXHG(object P_0, RoutedEventArgs P_1)
	{
		base.DialogResult = true;
	}

	private void CsdCN20f4o(object P_0, RoutedEventArgs P_1)
	{
		base.DialogResult = false;
	}

	private void SU9CzLCYKG(object P_0, RoutedEventArgs P_1)
	{
		((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
		{
			input.Focus();
		}, Array.Empty<object>());
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!aGoHloiCud)
		{
			aGoHloiCud = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/searchpvf/windowaddnewsearchresult.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((WindowAddNewSearchResult)target).Loaded += SU9CzLCYKG;
			break;
		case 2:
			input = (TextBox)target;
			input.TextChanged += Eg3C30emKb;
			break;
		case 3:
			buttonYes = (Button)target;
			buttonYes.Click += JcWCRpRXHG;
			break;
		case 4:
			((Button)target).Click += CsdCN20f4o;
			break;
		default:
			aGoHloiCud = true;
			break;
		}
	}

	[CompilerGenerated]
	private void L4RHDfSNx7()
	{
		input.Focus();
	}
}
