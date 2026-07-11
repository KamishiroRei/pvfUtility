using System;
using System.Windows.Media;
using HL.Xshtd.interfaces;

namespace HL.HighlightingTheme;

public class GlobalStyle : AbstractFreezable, IFreezable
{
	private string _TypeName;

	private Color? _Foregroundcolor;

	private Color? _Backgroundcolor;

	private Color? _Bordercolor;

	public string TypeName
	{
		get
		{
			return _TypeName;
		}
		set
		{
			if (base.IsFrozen)
			{
				throw new InvalidOperationException("Property is already frozen.");
			}
			_TypeName = value;
		}
	}

	public Color? foregroundcolor
	{
		get
		{
			return _Foregroundcolor;
		}
		set
		{
			if (base.IsFrozen)
			{
				throw new InvalidOperationException("Property is already frozen.");
			}
			_Foregroundcolor = value;
		}
	}

	public Color? backgroundcolor
	{
		get
		{
			return _Backgroundcolor;
		}
		set
		{
			if (base.IsFrozen)
			{
				throw new InvalidOperationException("Property is already frozen.");
			}
			_Backgroundcolor = value;
		}
	}

	public Color? bordercolor
	{
		get
		{
			return _Bordercolor;
		}
		set
		{
			if (base.IsFrozen)
			{
				throw new InvalidOperationException("Property is already frozen.");
			}
			_Bordercolor = value;
		}
	}

	public GlobalStyle(string typeName)
		: this()
	{
		TypeName = typeName;
	}

	protected GlobalStyle()
	{
		TypeName = string.Empty;
		_Foregroundcolor = null;
		_Backgroundcolor = null;
		_Bordercolor = null;
	}

	public override string ToString()
	{
		return "[" + (string.IsNullOrEmpty(TypeName) ? string.Empty : TypeName) + "]";
	}
}
