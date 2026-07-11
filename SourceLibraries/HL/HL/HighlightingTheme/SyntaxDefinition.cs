using System;
using System.Collections.Generic;
using HL.Xshtd.interfaces;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;

namespace HL.HighlightingTheme;

public class SyntaxDefinition : AbstractFreezable, IFreezable
{
	private string _Name;

	private readonly Dictionary<string, HighlightingColor> _NamedHighlightingColors;

	public string Name
	{
		get
		{
			return _Name;
		}
		set
		{
			if (base.IsFrozen)
			{
				throw new InvalidOperationException();
			}
			_Name = value;
		}
	}

	public IList<string> Extensions { get; private set; }

	public IEnumerable<HighlightingColor> NamedHighlightingColors => _NamedHighlightingColors.Values;

	public SyntaxDefinition(string paramName)
		: this()
	{
		_Name = paramName;
	}

	public SyntaxDefinition()
	{
		Extensions = new NullSafeCollection<string>();
		_NamedHighlightingColors = new Dictionary<string, HighlightingColor>();
	}

	public override string ToString()
	{
		return "[" + GetType().Name + " " + (string.IsNullOrEmpty(Name) ? string.Empty : Name) + "]";
	}

	public HighlightingColor ColorGet(string name)
	{
		if (_NamedHighlightingColors.TryGetValue(name, out var value))
		{
			return value;
		}
		return null;
	}

	public void ColorAdd(HighlightingColor color)
	{
		_NamedHighlightingColors.Add(color.Name, color);
	}

	internal void ColorReplace(string name, HighlightingColor themeColor)
	{
		_NamedHighlightingColors.Remove(name);
		_NamedHighlightingColors.Add(name, themeColor);
	}
}
