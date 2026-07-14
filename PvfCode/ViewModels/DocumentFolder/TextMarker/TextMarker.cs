using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.TextMarker;

public sealed class TextMarker : TextSegment, ITextMarker
{
	private readonly TextMarkerService service;

	private Color? backgroundColor;

	private Color? foregroundColor;

	private FontWeight? fontWeight;

	private FontStyle? fontStyle;

	private TextMarkerTypes markerTypes;

	private Color markerColor;

	public bool IsDeleted => !base.IsConnectedToCollection;

	public Color? BackgroundColor
	{
		get
		{
			return backgroundColor;
		}
		set
		{
			if (backgroundColor != value)
			{
				backgroundColor = value;
				Redraw();
			}
		}
	}

	public Color? ForegroundColor
	{
		get
		{
			return foregroundColor;
		}
		set
		{
			if (foregroundColor != value)
			{
				foregroundColor = value;
				Redraw();
			}
		}
	}

	public FontWeight? FontWeight
	{
		get
		{
			return fontWeight;
		}
		set
		{
			if (fontWeight != value)
			{
				fontWeight = value;
				Redraw();
			}
		}
	}

	public FontStyle? FontStyle
	{
		get
		{
			return fontStyle;
		}
		set
		{
			if (fontStyle != value)
			{
				fontStyle = value;
				Redraw();
			}
		}
	}

	public object Tag { get; set; }

	public TextMarkerTypes MarkerTypes
	{
		get
		{
			return markerTypes;
		}
		set
		{
			if (markerTypes != value)
			{
				markerTypes = value;
				Redraw();
			}
		}
	}

	public Color MarkerColor
	{
		get
		{
			return markerColor;
		}
		set
		{
			if (markerColor != value)
			{
				markerColor = value;
				Redraw();
			}
		}
	}

	public object ToolTip { get; set; }

	public event EventHandler Deleted;

	public TextMarker(TextMarkerService service, int startOffset, int length)
	{
		if (service == null)
		{
			throw new ArgumentNullException("service");
		}
		this.service = service;
		base.StartOffset = startOffset;
		base.Length = length;
		markerTypes = TextMarkerTypes.None;
	}

	public void Delete()
	{
		service.Remove(this);
	}

	internal void OnDeleted()
	{
		Deleted?.Invoke(this, EventArgs.Empty);
	}

	private void Redraw()
	{
		service.Redraw(this);
	}

	int ITextMarker.StartOffset => base.StartOffset;
}
