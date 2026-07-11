using System;
using System.ComponentModel;
using System.Reflection;

namespace ICSharpCode.AvalonEdit;

[Serializable]
public class TextEditorOptions : INotifyPropertyChanged
{
	private bool showSpaces;

	private bool showTabs;

	private bool showEndOfLine;

	private bool showBoxForControlCharacters = true;

	private bool enableHyperlinks = true;

	private bool enableEmailHyperlinks = true;

	private bool requireControlModifierForHyperlinkClick = true;

	private int indentationSize = 4;

	private bool convertTabsToSpaces;

	private bool cutCopyWholeLine = true;

	private bool allowScrollBelowDocument;

	private double wordWrapIndentation;

	private bool inheritWordWrapIndentation = true;

	private bool enableRectangularSelection = true;

	private bool enableTextDragDrop = true;

	private bool enableVirtualSpace;

	private bool enableImeSupport = true;

	private bool showColumnRuler;

	private int columnRulerPosition = 80;

	private bool highlightCurrentLine;

	private bool hideCursorWhileTyping = true;

	private bool allowToggleOverstrikeMode;

	[DefaultValue(false)]
	public virtual bool ShowSpaces
	{
		get
		{
			return showSpaces;
		}
		set
		{
			if (showSpaces != value)
			{
				showSpaces = value;
				OnPropertyChanged("ShowSpaces");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool ShowTabs
	{
		get
		{
			return showTabs;
		}
		set
		{
			if (showTabs != value)
			{
				showTabs = value;
				OnPropertyChanged("ShowTabs");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool ShowEndOfLine
	{
		get
		{
			return showEndOfLine;
		}
		set
		{
			if (showEndOfLine != value)
			{
				showEndOfLine = value;
				OnPropertyChanged("ShowEndOfLine");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool ShowBoxForControlCharacters
	{
		get
		{
			return showBoxForControlCharacters;
		}
		set
		{
			if (showBoxForControlCharacters != value)
			{
				showBoxForControlCharacters = value;
				OnPropertyChanged("ShowBoxForControlCharacters");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool EnableHyperlinks
	{
		get
		{
			return enableHyperlinks;
		}
		set
		{
			if (enableHyperlinks != value)
			{
				enableHyperlinks = value;
				OnPropertyChanged("EnableHyperlinks");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool EnableEmailHyperlinks
	{
		get
		{
			return enableEmailHyperlinks;
		}
		set
		{
			if (enableEmailHyperlinks != value)
			{
				enableEmailHyperlinks = value;
				OnPropertyChanged("EnableEMailHyperlinks");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool RequireControlModifierForHyperlinkClick
	{
		get
		{
			return requireControlModifierForHyperlinkClick;
		}
		set
		{
			if (requireControlModifierForHyperlinkClick != value)
			{
				requireControlModifierForHyperlinkClick = value;
				OnPropertyChanged("RequireControlModifierForHyperlinkClick");
			}
		}
	}

	[DefaultValue(4)]
	public virtual int IndentationSize
	{
		get
		{
			return indentationSize;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("value", value, "value must be positive");
			}
			if (value > 1000)
			{
				throw new ArgumentOutOfRangeException("value", value, "indentation size is too large");
			}
			if (indentationSize != value)
			{
				indentationSize = value;
				OnPropertyChanged("IndentationSize");
				OnPropertyChanged("IndentationString");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool ConvertTabsToSpaces
	{
		get
		{
			return convertTabsToSpaces;
		}
		set
		{
			if (convertTabsToSpaces != value)
			{
				convertTabsToSpaces = value;
				OnPropertyChanged("ConvertTabsToSpaces");
				OnPropertyChanged("IndentationString");
			}
		}
	}

	[Browsable(false)]
	public string IndentationString => GetIndentationString(1);

	[DefaultValue(true)]
	public virtual bool CutCopyWholeLine
	{
		get
		{
			return cutCopyWholeLine;
		}
		set
		{
			if (cutCopyWholeLine != value)
			{
				cutCopyWholeLine = value;
				OnPropertyChanged("CutCopyWholeLine");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool AllowScrollBelowDocument
	{
		get
		{
			return allowScrollBelowDocument;
		}
		set
		{
			if (allowScrollBelowDocument != value)
			{
				allowScrollBelowDocument = value;
				OnPropertyChanged("AllowScrollBelowDocument");
			}
		}
	}

	[DefaultValue(0.0)]
	public virtual double WordWrapIndentation
	{
		get
		{
			return wordWrapIndentation;
		}
		set
		{
			if (double.IsNaN(value) || double.IsInfinity(value))
			{
				throw new ArgumentOutOfRangeException("value", value, "value must not be NaN/infinity");
			}
			if (value != wordWrapIndentation)
			{
				wordWrapIndentation = value;
				OnPropertyChanged("WordWrapIndentation");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool InheritWordWrapIndentation
	{
		get
		{
			return inheritWordWrapIndentation;
		}
		set
		{
			if (value != inheritWordWrapIndentation)
			{
				inheritWordWrapIndentation = value;
				OnPropertyChanged("InheritWordWrapIndentation");
			}
		}
	}

	[DefaultValue(true)]
	public bool EnableRectangularSelection
	{
		get
		{
			return enableRectangularSelection;
		}
		set
		{
			if (enableRectangularSelection != value)
			{
				enableRectangularSelection = value;
				OnPropertyChanged("EnableRectangularSelection");
			}
		}
	}

	[DefaultValue(true)]
	public bool EnableTextDragDrop
	{
		get
		{
			return enableTextDragDrop;
		}
		set
		{
			if (enableTextDragDrop != value)
			{
				enableTextDragDrop = value;
				OnPropertyChanged("EnableTextDragDrop");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool EnableVirtualSpace
	{
		get
		{
			return enableVirtualSpace;
		}
		set
		{
			if (enableVirtualSpace != value)
			{
				enableVirtualSpace = value;
				OnPropertyChanged("EnableVirtualSpace");
			}
		}
	}

	[DefaultValue(true)]
	public virtual bool EnableImeSupport
	{
		get
		{
			return enableImeSupport;
		}
		set
		{
			if (enableImeSupport != value)
			{
				enableImeSupport = value;
				OnPropertyChanged("EnableImeSupport");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool ShowColumnRuler
	{
		get
		{
			return showColumnRuler;
		}
		set
		{
			if (showColumnRuler != value)
			{
				showColumnRuler = value;
				OnPropertyChanged("ShowColumnRuler");
			}
		}
	}

	[DefaultValue(80)]
	public virtual int ColumnRulerPosition
	{
		get
		{
			return columnRulerPosition;
		}
		set
		{
			if (columnRulerPosition != value)
			{
				columnRulerPosition = value;
				OnPropertyChanged("ColumnRulerPosition");
			}
		}
	}

	[DefaultValue(false)]
	public virtual bool HighlightCurrentLine
	{
		get
		{
			return highlightCurrentLine;
		}
		set
		{
			if (highlightCurrentLine != value)
			{
				highlightCurrentLine = value;
				OnPropertyChanged("HighlightCurrentLine");
			}
		}
	}

	[DefaultValue(true)]
	public bool HideCursorWhileTyping
	{
		get
		{
			return hideCursorWhileTyping;
		}
		set
		{
			if (hideCursorWhileTyping != value)
			{
				hideCursorWhileTyping = value;
				OnPropertyChanged("HideCursorWhileTyping");
			}
		}
	}

	[DefaultValue(false)]
	public bool AllowToggleOverstrikeMode
	{
		get
		{
			return allowToggleOverstrikeMode;
		}
		set
		{
			if (allowToggleOverstrikeMode != value)
			{
				allowToggleOverstrikeMode = value;
				OnPropertyChanged("AllowToggleOverstrikeMode");
			}
		}
	}

	[field: NonSerialized]
	public event PropertyChangedEventHandler PropertyChanged;

	public TextEditorOptions()
	{
	}

	public TextEditorOptions(TextEditorOptions options)
	{
		FieldInfo[] fields = typeof(TextEditorOptions).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!fieldInfo.IsNotSerialized)
			{
				fieldInfo.SetValue(this, fieldInfo.GetValue(options));
			}
		}
	}

	protected void OnPropertyChanged(string propertyName)
	{
		OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
	}

	protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, e);
		}
	}

	public virtual string GetIndentationString(int column)
	{
		if (column < 1)
		{
			throw new ArgumentOutOfRangeException("column", column, "Value must be at least 1.");
		}
		int num = IndentationSize;
		if (ConvertTabsToSpaces)
		{
			return new string(' ', num - (column - 1) % num);
		}
		return "\t";
	}
}
