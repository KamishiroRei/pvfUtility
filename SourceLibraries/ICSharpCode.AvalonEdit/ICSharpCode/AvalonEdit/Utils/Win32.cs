using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Utils;

internal static class Win32
{
	[SuppressUnmanagedCodeSecurity]
	private static class SafeNativeMethods
	{
		[DllImport("user32.dll")]
		public static extern int GetCaretBlinkTime();

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetCaretPos(int x, int y);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyCaret();
	}

	public static TimeSpan CaretBlinkTime => TimeSpan.FromMilliseconds(SafeNativeMethods.GetCaretBlinkTime());

	public static bool CreateCaret(Visual owner, Size size)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (PresentationSource.FromVisual(owner) is HwndSource hwndSource)
		{
			Vector vector = owner.PointToScreen(new Point(size.Width, size.Height)) - owner.PointToScreen(new Point(0.0, 0.0));
			return SafeNativeMethods.CreateCaret(hwndSource.Handle, IntPtr.Zero, (int)Math.Ceiling(vector.X), (int)Math.Ceiling(vector.Y));
		}
		return false;
	}

	public static bool SetCaretPosition(Visual owner, Point position)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (PresentationSource.FromVisual(owner) is HwndSource hwndSource)
		{
			Point point = owner.TransformToAncestor(hwndSource.RootVisual).Transform(position).TransformToDevice(hwndSource.RootVisual);
			return SafeNativeMethods.SetCaretPos((int)point.X, (int)point.Y);
		}
		return false;
	}

	public static bool DestroyCaret()
	{
		return SafeNativeMethods.DestroyCaret();
	}
}
