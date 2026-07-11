using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using PvfCode.Controls;

namespace PvfCode.Controls.TextEditorFolder;

public class WindowGotoLine : ThemedWindow, IComponentConnector
{
	public static readonly DependencyProperty OffsetProperty;

	private bool Iw26lWBL0A;

	public int Offset
	{
		get
		{
			return (int)((DependencyObject)this).GetValue(OffsetProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(OffsetProperty, (object)value);
		}
	}

	public WindowGotoLine()
	{
		base.DataContext = this;
		InitializeComponent();
		Title = "跳转到偏移量";
		ApplyOffsetLabels();
		Loaded += OnLoaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		Loaded -= OnLoaded;
		ApplyOffsetLabels();
	}

	private void ApplyOffsetLabels()
	{
		EditBox offsetInput = FindVisualChild<EditBox>(this);
		if (offsetInput != null)
		{
			offsetInput.Caption = "偏移量：";
			offsetInput.NullText = "请输入字符偏移量";
		}
	}

	private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
	{
		int childCount = VisualTreeHelper.GetChildrenCount(parent);
		for (int index = 0; index < childCount; index++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(parent, index);
			if (child is T match)
			{
				return match;
			}
			T descendant = FindVisualChild<T>(child);
			if (descendant != null)
			{
				return descendant;
			}
		}
		return null;
	}

	private void Confirm(object sender, RoutedEventArgs e)
	{
		base.DialogResult = true;
	}

	private void Cancel(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!Iw26lWBL0A)
		{
			Iw26lWBL0A = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/texteditorfolder/windowgotoline.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((Button)target).Click += Confirm;
			break;
		case 2:
			((Button)target).Click += Cancel;
			break;
		default:
			Iw26lWBL0A = true;
			break;
		}
	}

	static WindowGotoLine()
	{
		OffsetProperty = DependencyProperty.Register("Offset", typeof(int), typeof(WindowGotoLine), new PropertyMetadata((object)0));
	}
}
