using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.TextMarker;

public sealed class TextMarker : TextSegment, ITextMarker
{
	private readonly TextMarkerService h9IShoVmy0;

	[CompilerGenerated]
	private EventHandler zndSvcv41q;

	private Color? rZaSB682II;

	private Color? t4CSFR77C1;

	private FontWeight? AQjSrYHj7m;

	private FontStyle? pejSWpfNBg;

	[CompilerGenerated]
	private object VRbSm8OkqN;

	private TextMarkerTypes ENRS2lyOWY;

	private Color eqoSfOj9KM;

	[CompilerGenerated]
	private object QGXS5Ypdee;

	public bool IsDeleted => !base.IsConnectedToCollection;

	public Color? BackgroundColor
	{
		get
		{
			return rZaSB682II;
		}
		set
		{
			if (rZaSB682II != value)
			{
				rZaSB682II = value;
				w7pSHMeWEV();
			}
		}
	}

	public Color? ForegroundColor
	{
		get
		{
			return t4CSFR77C1;
		}
		set
		{
			if (t4CSFR77C1 != value)
			{
				t4CSFR77C1 = value;
				w7pSHMeWEV();
			}
		}
	}

	public FontWeight? FontWeight
	{
		get
		{
			return AQjSrYHj7m;
		}
		set
		{
			if (AQjSrYHj7m != value)
			{
				AQjSrYHj7m = value;
				w7pSHMeWEV();
			}
		}
	}

	public FontStyle? FontStyle
	{
		get
		{
			return pejSWpfNBg;
		}
		set
		{
			if (pejSWpfNBg != value)
			{
				pejSWpfNBg = value;
				w7pSHMeWEV();
			}
		}
	}

	public object Tag
	{
		[CompilerGenerated]
		get
		{
			return VRbSm8OkqN;
		}
		[CompilerGenerated]
		set
		{
			VRbSm8OkqN = value;
		}
	}

	public TextMarkerTypes MarkerTypes
	{
		get
		{
			return ENRS2lyOWY;
		}
		set
		{
			if (ENRS2lyOWY != value)
			{
				ENRS2lyOWY = value;
				w7pSHMeWEV();
			}
		}
	}

	public Color MarkerColor
	{
		get
		{
			return eqoSfOj9KM;
		}
		set
		{
			if (eqoSfOj9KM != value)
			{
				eqoSfOj9KM = value;
				w7pSHMeWEV();
			}
		}
	}

	public object ToolTip
	{
		[CompilerGenerated]
		get
		{
			return QGXS5Ypdee;
		}
		[CompilerGenerated]
		set
		{
			QGXS5Ypdee = value;
		}
	}

	public event EventHandler Deleted
	{
		[CompilerGenerated]
		add
		{
			EventHandler eventHandler = zndSvcv41q;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref zndSvcv41q, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
		[CompilerGenerated]
		remove
		{
			EventHandler eventHandler = zndSvcv41q;
			EventHandler eventHandler2;
			do
			{
				eventHandler2 = eventHandler;
				EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref zndSvcv41q, value2, eventHandler2);
			}
			while ((object)eventHandler != eventHandler2);
		}
	}

	public TextMarker(TextMarkerService service, int startOffset, int length)
	{
		if (service == null)
		{
			throw new ArgumentNullException("service");
		}
		h9IShoVmy0 = service;
		base.StartOffset = startOffset;
		base.Length = length;
		ENRS2lyOWY = TextMarkerTypes.None;
	}

	public void Delete()
	{
		h9IShoVmy0.Remove(this);
	}

	internal void sXFSCtUS7y()
	{
		if (zndSvcv41q != null)
		{
			zndSvcv41q(this, EventArgs.Empty);
		}
	}

	private void w7pSHMeWEV()
	{
		h9IShoVmy0.Ymo5NGv8tI(this);
	}

	int ITextMarker.StartOffset => base.StartOffset;
}
