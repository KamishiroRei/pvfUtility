using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PvfCode.Views.ChatGPT;

public class ChatGPTMessDocument : UserControl, IComponentConnector
{
	private bool GG6vqnyuOV;

	public ChatGPTMessDocument()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!GG6vqnyuOV)
		{
			GG6vqnyuOV = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/chatgpt/chatgptmessdocument.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		GG6vqnyuOV = true;
	}
}
