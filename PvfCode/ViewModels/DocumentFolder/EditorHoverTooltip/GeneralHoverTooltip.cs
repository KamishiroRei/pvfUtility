using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.EditorHoverTooltip;

public class GeneralHoverTooltip : Popup, IComponentConnector
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
		internal static extern bool sNYbpd9bhc(IntPtr P_0, out RECT P_1);

		[DllImport("user32", EntryPoint = "SetWindowPos")]
		internal static extern int ENWbUUlCS9(IntPtr P_0, int P_1, int P_2, int P_3, int P_4, int P_5, int P_6);
	}

	internal VisualLineElement hYbiDkDyis;

	public static readonly DependencyProperty IsPositionUpdateProperty;

	public static DependencyProperty TopmostProperty;

	internal GeneralHoverTooltip pop;

	private bool gV5ilsuiaD;

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

	public GeneralHoverTooltip(object vm)
	{
		base.DataContext = vm;
		InitializeComponent();
	}

	private static void Qm2yVaSCiT(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		(P_0 as GeneralHoverTooltip).l61y3UEkMG(P_0 as GeneralHoverTooltip, null);
	}

	private void l61y3UEkMG(object P_0, RoutedEventArgs P_1)
	{
		DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)(object)(P_0 as Popup));
		while (parent != null && !(parent is Window))
		{
			parent = VisualTreeHelper.GetParent(parent);
		}
		if (parent is Window)
		{
			(parent as Window).LocationChanged -= dhByR4J2Nv;
			(parent as Window).SizeChanged -= dhByR4J2Nv;
			if (IsPositionUpdate)
			{
				(parent as Window).LocationChanged += dhByR4J2Nv;
				(parent as Window).SizeChanged += dhByR4J2Nv;
			}
		}
	}

	private void dhByR4J2Nv(object P_0, EventArgs P_1)
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

	private static void GXOyNZ2Uvp(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		(P_0 as GeneralHoverTooltip).mCuyzWsVrU();
	}

	protected override void OnOpened(EventArgs e)
	{
		mCuyzWsVrU();
	}

	private void mCuyzWsVrU()
	{
		NativeMethods.sNYbpd9bhc(((HwndSource)PresentationSource.FromVisual(base.Child)).Handle, out var _);
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!gV5ilsuiaD)
		{
			gV5ilsuiaD = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/viewmodels/documentfolder/editorhovertooltip/generalhovertooltip.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			pop = (GeneralHoverTooltip)target;
			pop.Loaded += l61y3UEkMG;
		}
		else
		{
			gV5ilsuiaD = true;
		}
	}

	static GeneralHoverTooltip()
	{
		IsPositionUpdateProperty = DependencyProperty.Register("IsPositionUpdate", typeof(bool), typeof(GeneralHoverTooltip), new PropertyMetadata((object)true, new PropertyChangedCallback(Qm2yVaSCiT)));
		TopmostProperty = Window.TopmostProperty.AddOwner(typeof(Popup), (PropertyMetadata)(object)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(GXOyNZ2Uvp)));
	}
}
