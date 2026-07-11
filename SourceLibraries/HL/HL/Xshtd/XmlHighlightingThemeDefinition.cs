using System;
using System.Collections.Generic;
using HL.HighlightingTheme;
using HL.Xshtd.interfaces;
using ICSharpCode.AvalonEdit.Highlighting;

namespace HL.Xshtd;

internal class XmlHighlightingThemeDefinition : IHighlightingThemeDefinition
{
	private sealed class RegisterNamedElementsVisitor : IXshtdVisitor
	{
		private readonly XmlHighlightingThemeDefinition def;

		public RegisterNamedElementsVisitor(XmlHighlightingThemeDefinition def)
			: this()
		{
			this.def = def;
		}

		private RegisterNamedElementsVisitor()
		{
		}

		public object VisitSyntaxDefinition(XshtdSyntaxDefinition syntax)
		{
			if (syntax.Name != null)
			{
				if (syntax.Name.Length == 0)
				{
					throw Error(syntax, "Name must not be the empty string");
				}
				if (def.syntaxDefDict.ContainsKey(syntax.Name))
				{
					throw Error(syntax, "Duplicate syntax definition name '" + syntax.Name + "'.");
				}
				def.syntaxDefDict.Add(syntax.Name, new SyntaxDefinition());
			}
			syntax.AcceptElements(this);
			return null;
		}

		public object VisitColor(XshtdSyntaxDefinition syntax, XshtdColor color)
		{
			if (color.Name != null)
			{
				if (color.Name.Length == 0)
				{
					throw Error(color, "Name must not be the empty string");
				}
				if (syntax == null)
				{
					throw Error(syntax, "Syntax Definition for theme must not be null");
				}
				if (!def.syntaxDefDict.TryGetValue(syntax.Name, out var value))
				{
					throw Error(syntax, "Themed Syntax Definition does not exist '" + syntax.Name + "'.");
				}
				if (value.ColorGet(color.Name) != null)
				{
					throw Error(color, "Duplicate color name '" + color.Name + "'.");
				}
				value.ColorAdd(new HighlightingColor
				{
					Name = color.Name
				});
			}
			return null;
		}

		public object VisitGlobalStyles(XshtdGlobalStyles globStyles)
		{
			globStyles.AcceptElements(this);
			return null;
		}

		public object VisitGlobalStyle(XshtdGlobalStyles globStyles, XshtdGlobalStyle style)
		{
			if (style.TypeName != null)
			{
				if (style.TypeName.Length == 0)
				{
					throw Error(style, "Name must not be the empty string");
				}
				if (globStyles == null)
				{
					throw Error(globStyles, "GlobalStyles parameter must not be null");
				}
				if (def._GlobalStyles.TryGetValue(style.TypeName, out var value))
				{
					throw Error(style, "GlobalStyle definition '" + style.TypeName + "' has duplicates.");
				}
				value = new GlobalStyle(style.TypeName);
				value.backgroundcolor = style.background;
				value.foregroundcolor = style.foreground;
				value.bordercolor = style.bordercolor;
				value.Freeze();
				def._GlobalStyles.Add(style.TypeName, new GlobalStyle(style.TypeName));
			}
			return null;
		}
	}

	private sealed class TranslateElementVisitor : IXshtdVisitor
	{
		private readonly XmlHighlightingThemeDefinition def;

		public TranslateElementVisitor(XmlHighlightingThemeDefinition def, IHighlightingThemeDefinitionReferenceResolver resolver)
		{
			this.def = def;
		}

		public object VisitSyntaxDefinition(XshtdSyntaxDefinition syntax)
		{
			SyntaxDefinition syntaxDefinition;
			if (syntax.Name != null)
			{
				syntaxDefinition = def.syntaxDefDict[syntax.Name];
			}
			else
			{
				if (syntax.Extensions == null)
				{
					return null;
				}
				syntaxDefinition = new SyntaxDefinition(syntax.Name);
			}
			foreach (string extension in syntax.Extensions)
			{
				syntaxDefinition.Extensions.Add(extension);
			}
			syntax.AcceptElements(this);
			return syntaxDefinition;
		}

		public object VisitColor(XshtdSyntaxDefinition syntax, XshtdColor color)
		{
			if (color.Name == null)
			{
				throw Error(color, "Name must not be null");
			}
			if (color.Name.Length == 0)
			{
				throw Error(color, "Name must not be the empty string");
			}
			if (syntax == null)
			{
				throw Error(syntax, "Syntax Definition for theme must not be null");
			}
			if (!def.syntaxDefDict.TryGetValue(syntax.Name, out var value))
			{
				throw Error(syntax, "Themed Syntax Definition does not exist '" + syntax.Name + "'.");
			}
			HighlightingColor highlightingColor = value.ColorGet(color.Name);
			if (highlightingColor == null)
			{
				highlightingColor = new HighlightingColor
				{
					Name = color.Name
				};
				value.ColorAdd(highlightingColor);
			}
			highlightingColor.Foreground = color.Foreground;
			highlightingColor.Background = color.Background;
			highlightingColor.Underline = color.Underline;
			highlightingColor.FontStyle = color.FontStyle;
			highlightingColor.FontWeight = color.FontWeight;
			return highlightingColor;
		}

		public object VisitGlobalStyles(XshtdGlobalStyles globStyles)
		{
			globStyles.AcceptElements(this);
			return null;
		}

		public object VisitGlobalStyle(XshtdGlobalStyles globStyles, XshtdGlobalStyle style)
		{
			if (style.TypeName == null)
			{
				throw Error(style, "Name must not be null");
			}
			if (style.TypeName.Length == 0)
			{
				throw Error(style, "Name must not be the empty string");
			}
			if (globStyles == null)
			{
				throw Error(globStyles, "GlobalStyles parameter must not be null");
			}
			if (!def._GlobalStyles.TryGetValue(style.TypeName, out var value))
			{
				throw Error(style, "Style definition '" + style.TypeName + "' does not exist in collection of GlobalStyles.");
			}
			value.backgroundcolor = style.background;
			value.foregroundcolor = style.foreground;
			value.bordercolor = style.bordercolor;
			value.Freeze();
			return value;
		}
	}

	private Dictionary<string, SyntaxDefinition> syntaxDefDict;

	private Dictionary<string, GlobalStyle> _GlobalStyles;

	private readonly XhstdThemeDefinition _xshtd;

	public string Name => _xshtd.Name;

	public IEnumerable<GlobalStyle> GlobalStyles => _GlobalStyles.Values;

	public XmlHighlightingThemeDefinition(XhstdThemeDefinition xshtd, IHighlightingThemeDefinitionReferenceResolver resolver)
		: this()
	{
		xshtd.AcceptVisitor(new RegisterNamedElementsVisitor(this));
		xshtd.AcceptVisitor(new TranslateElementVisitor(this, resolver));
		_xshtd = xshtd;
	}

	protected XmlHighlightingThemeDefinition()
	{
		syntaxDefDict = new Dictionary<string, SyntaxDefinition>();
		_GlobalStyles = new Dictionary<string, GlobalStyle>();
	}

	public SyntaxDefinition GetNamedSyntaxDefinition(string name)
	{
		SyntaxDefinition value = null;
		syntaxDefDict.TryGetValue(name, out value);
		return value;
	}

	public HighlightingColor GetNamedColor(string synDefName, string colorName)
	{
		return GetNamedSyntaxDefinition(synDefName)?.ColorGet(colorName);
	}

	public IEnumerable<HighlightingColor> NamedHighlightingColors(string synDefName)
	{
		SyntaxDefinition namedSyntaxDefinition = GetNamedSyntaxDefinition(synDefName);
		if (namedSyntaxDefinition == null)
		{
			return new List<HighlightingColor>();
		}
		return namedSyntaxDefinition.NamedHighlightingColors;
	}

	private static Exception Error(XshtdElement element, string message)
	{
		if (element.LineNumber > 0)
		{
			return new HighlightingDefinitionInvalidException("Error at line " + element.LineNumber + ":\n" + message);
		}
		return new HighlightingDefinitionInvalidException(message);
	}
}
