using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace PvfCode.Controls;

public class PopupEx : Popup
{
	public struct RECT
	{
		public int Left;

		public int Top;

		public int Right;

		public int Bottom;
	}

	public static class NativeMethods
	{
		[DllImport("user32.dll", EntryPoint = "GetWindowRect")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool gUrODGAYCo(IntPtr P_0, out RECT P_1);

		[DllImport("user32", EntryPoint = "SetWindowPos")]
		internal static extern int pcOOlvIC6H(IntPtr P_0, int P_1, int P_2, int P_3, int P_4, int P_5, int P_6);
	}

	public static readonly DependencyProperty IsPositionUpdateProperty;

	public static DependencyProperty TopmostProperty;

	public bool IsPositionUpdate
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(IsPositionUpdateProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(IsPositionUpdateProperty, (object)value);
		}
	}

	public bool Topmost
	{
		get
		{
			return (bool)((DependencyObject)this).GetValue(TopmostProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TopmostProperty, (object)value);
		}
	}

	private static void TJraLoUKNv(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		(P_0 as PopupEx).DTKanMEMxv(P_0 as PopupEx, null);
	}

	public PopupEx()
	{
		base.Loaded += DTKanMEMxv;
	}

	private void DTKanMEMxv(object P_0, RoutedEventArgs P_1)
	{
		DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)(P_0 as Popup));
		while (parent != null && !(parent is Window))
		{
			parent = VisualTreeHelper.GetParent(parent);
		}
		if (parent is Window)
		{
			(parent as Window).LocationChanged -= DxJaq6qZsm;
			(parent as Window).SizeChanged -= DxJaq6qZsm;
			if (IsPositionUpdate)
			{
				(parent as Window).LocationChanged += DxJaq6qZsm;
				(parent as Window).SizeChanged += DxJaq6qZsm;
			}
		}
	}

	private void DxJaq6qZsm(object? sender, EventArgs P_1)
	{
		try
		{
			MethodInfo method = typeof(Popup).GetMethod("UpdatePosition", BindingFlags.Instance | BindingFlags.NonPublic);
			if (base.IsOpen)
			{
				method.Invoke(this, null);
			}
		}
		catch
		{
		}
	}

	private static void KTUadsm4GX(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		(P_0 as PopupEx).If1aeLs34N();
	}

	protected override void OnOpened(EventArgs e)
	{
		If1aeLs34N();
	}

	private void If1aeLs34N()
	{
		NativeMethods.gUrODGAYCo(((HwndSource)PresentationSource.FromVisual(base.Child)).Handle, out var _);
	}

	static PopupEx()
	{
		IsPositionUpdateProperty = DependencyProperty.Register("IsPositionUpdate", typeof(bool), typeof(PopupEx), new PropertyMetadata((object)true, new PropertyChangedCallback(TJraLoUKNv)));
		TopmostProperty = Window.TopmostProperty.AddOwner(typeof(Popup), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(KTUadsm4GX)));
	}
}
