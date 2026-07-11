using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ICSharpCode.AvalonEdit.Rendering;

public class MouseHoverLogic : IDisposable
{
	private UIElement target;

	private DispatcherTimer mouseHoverTimer;

	private Point mouseHoverStartPoint;

	private MouseEventArgs mouseHoverLastEventArgs;

	private bool mouseHovering;

	private bool disposed;

	public event EventHandler<MouseEventArgs> MouseHover;

	public event EventHandler<MouseEventArgs> MouseHoverStopped;

	public MouseHoverLogic(UIElement target)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		this.target = target;
		this.target.MouseLeave += MouseHoverLogicMouseLeave;
		this.target.MouseMove += MouseHoverLogicMouseMove;
		this.target.MouseEnter += MouseHoverLogicMouseEnter;
	}

	private void MouseHoverLogicMouseMove(object sender, MouseEventArgs e)
	{
		Vector vector = mouseHoverStartPoint - e.GetPosition(target);
		if (Math.Abs(vector.X) > SystemParameters.MouseHoverWidth || Math.Abs(vector.Y) > SystemParameters.MouseHoverHeight)
		{
			StartHovering(e);
		}
	}

	private void MouseHoverLogicMouseEnter(object sender, MouseEventArgs e)
	{
		StartHovering(e);
	}

	private void StartHovering(MouseEventArgs e)
	{
		StopHovering();
		mouseHoverStartPoint = e.GetPosition(target);
		mouseHoverLastEventArgs = e;
		mouseHoverTimer = new DispatcherTimer(SystemParameters.MouseHoverTime, DispatcherPriority.Background, OnMouseHoverTimerElapsed, target.Dispatcher);
		mouseHoverTimer.Start();
	}

	private void MouseHoverLogicMouseLeave(object sender, MouseEventArgs e)
	{
		StopHovering();
	}

	private void StopHovering()
	{
		if (mouseHoverTimer != null)
		{
			mouseHoverTimer.Stop();
			mouseHoverTimer = null;
		}
		if (mouseHovering)
		{
			mouseHovering = false;
			OnMouseHoverStopped(mouseHoverLastEventArgs);
		}
	}

	private void OnMouseHoverTimerElapsed(object sender, EventArgs e)
	{
		mouseHoverTimer.Stop();
		mouseHoverTimer = null;
		mouseHovering = true;
		OnMouseHover(mouseHoverLastEventArgs);
	}

	protected virtual void OnMouseHover(MouseEventArgs e)
	{
		if (this.MouseHover != null)
		{
			this.MouseHover(this, e);
		}
	}

	protected virtual void OnMouseHoverStopped(MouseEventArgs e)
	{
		if (this.MouseHoverStopped != null)
		{
			this.MouseHoverStopped(this, e);
		}
	}

	public void Dispose()
	{
		if (!disposed)
		{
			target.MouseLeave -= MouseHoverLogicMouseLeave;
			target.MouseMove -= MouseHoverLogicMouseMove;
			target.MouseEnter -= MouseHoverLogicMouseEnter;
		}
		disposed = true;
	}
}
