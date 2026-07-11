using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using HL.Resources;
using HL.Xshtd;
using ICSharpCode.AvalonEdit.Highlighting;

namespace HL.Manager;

internal static class XshtdLoader
{
	public const string Namespace = "http://icsharpcode.net/sharpdevelop/themesyntaxdefinition/2019";

	private static XmlSchemaSet schemaSet;

	internal static readonly ColorConverter ColorConverter = new ColorConverter();

	internal static readonly FontWeightConverter FontWeightConverter = new FontWeightConverter();

	internal static readonly FontStyleConverter FontStyleConverter = new FontStyleConverter();

	private static XmlSchemaSet SchemaSet
	{
		get
		{
			if (schemaSet == null)
			{
				schemaSet = HighlightingLoader.LoadSchemaSet(new XmlTextReader(HLResources.OpenStream("HL.Modes", "ModeV2_htd.xsd")));
			}
			return schemaSet;
		}
	}

	public static XhstdThemeDefinition LoadDefinition(XmlReader reader, bool skipValidation)
	{
		reader = HighlightingLoader.GetValidatingReader(reader, ignoreWhitespace: true, skipValidation ? null : SchemaSet);
		reader.Read();
		return ParseDefinition(reader);
	}

	private static XhstdThemeDefinition ParseDefinition(XmlReader reader)
	{
		XhstdThemeDefinition xhstdThemeDefinition = new XhstdThemeDefinition();
		xhstdThemeDefinition.Name = reader.GetAttribute("name");
		Stack<XshtdElement> stack = new Stack<XshtdElement>();
		stack.Push(xhstdThemeDefinition);
		ParseElements(xhstdThemeDefinition.Elements, reader, stack);
		stack.Pop();
		return xhstdThemeDefinition;
	}

	private static void ParseElements(ICollection<XshtdElement> c, XmlReader reader, Stack<XshtdElement> xmlPath)
	{
		if (reader.IsEmptyElement)
		{
			return;
		}
		while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
		{
			if (reader.NamespaceURI != "http://icsharpcode.net/sharpdevelop/themesyntaxdefinition/2019")
			{
				if (!reader.IsEmptyElement)
				{
					reader.Skip();
				}
				continue;
			}
			string name = reader.Name;
			if (name != null)
			{
				int length = name.Length;
				if (length <= 9)
				{
					if (length != 5)
					{
						if (length == 9)
						{
							char c2 = name[0];
							if (c2 != 'H')
							{
								if (c2 == 'S' && name == "Selection")
								{
									goto IL_01b5;
								}
							}
							else if (name == "Hyperlink")
							{
								goto IL_01b5;
							}
						}
					}
					else if (name == "Color")
					{
						if (!(xmlPath.Peek() is XshtdSyntaxDefinition syntax))
						{
							throw new Exception("Syntax Error: Color cannot occurr outside of SyntaxDefinition");
						}
						c.Add(ParseNamedColor(reader, syntax));
						continue;
					}
				}
				else if (length != 12)
				{
					if (length != 16)
					{
						if (length == 21)
						{
							char c2 = name[0];
							if (c2 != 'C')
							{
								if (c2 != 'L')
								{
									if (c2 == 'N' && name == "NonPrintableCharacter")
									{
										goto IL_01b5;
									}
								}
								else if (name == "LineNumbersForeground")
								{
									goto IL_01b5;
								}
							}
							else if (name == "CurrentLineBackground")
							{
								goto IL_01b5;
							}
						}
					}
					else if (name == "SyntaxDefinition")
					{
						c.Add(ParseSyntaxDefinition(reader, xmlPath));
						continue;
					}
				}
				else
				{
					char c2 = name[0];
					if (c2 != 'D')
					{
						if (c2 == 'G' && name == "GlobalStyles")
						{
							ParseGlobalStyles(reader, xmlPath);
							continue;
						}
					}
					else if (name == "DefaultStyle")
					{
						goto IL_01b5;
					}
				}
			}
			throw new NotSupportedException("Unknown element " + reader.Name);
			IL_01b5:
			ParseGlobalStyle(reader, xmlPath);
		}
	}

	private static XshtdSyntaxDefinition ParseSyntaxDefinition(XmlReader reader, Stack<XshtdElement> xmlPath)
	{
		XshtdSyntaxDefinition xshtdSyntaxDefinition = new XshtdSyntaxDefinition();
		xshtdSyntaxDefinition.Name = reader.GetAttribute("name");
		string attribute = reader.GetAttribute("extensions");
		if (attribute != null)
		{
			xshtdSyntaxDefinition.Extensions.AddRange(attribute.Split(';'));
		}
		xmlPath.Push(xshtdSyntaxDefinition);
		ParseElements(xshtdSyntaxDefinition.Elements, reader, xmlPath);
		return xmlPath.Pop() as XshtdSyntaxDefinition;
	}

	private static XshtdElement ParseGlobalStyles(XmlReader reader, Stack<XshtdElement> xmlPath)
	{
		XhstdThemeDefinition xhstdThemeDefinition = xmlPath.Peek() as XhstdThemeDefinition;
		xmlPath.Push(xhstdThemeDefinition.GlobalStyleElements);
		ParseElements(null, reader, xmlPath);
		xmlPath.Pop();
		return null;
	}

	private static XshtdElement ParseGlobalStyle(XmlReader reader, Stack<XshtdElement> xmlPath)
	{
		XshtdGlobalStyles obj = xmlPath.Peek() as XshtdGlobalStyles;
		XshtdGlobalStyle xshtdGlobalStyle = new XshtdGlobalStyle(obj)
		{
			TypeName = reader.Name
		};
		string attribute = reader.GetAttribute("background");
		if (!string.IsNullOrEmpty(attribute))
		{
			xshtdGlobalStyle.background = (Color?)ColorConverter.ConvertFromInvariantString(attribute);
		}
		attribute = reader.GetAttribute("foreground");
		if (!string.IsNullOrEmpty(attribute))
		{
			xshtdGlobalStyle.foreground = (Color?)ColorConverter.ConvertFromInvariantString(attribute);
		}
		attribute = reader.GetAttribute("bordercolor");
		if (!string.IsNullOrEmpty(attribute))
		{
			xshtdGlobalStyle.bordercolor = (Color?)ColorConverter.ConvertFromInvariantString(attribute);
		}
		obj.Elements.Add(xshtdGlobalStyle);
		return obj;
	}

	private static Exception Error(XmlReader reader, string message)
	{
		return Error(reader as IXmlLineInfo, message);
	}

	private static Exception Error(IXmlLineInfo lineInfo, string message)
	{
		if (lineInfo != null)
		{
			return new HighlightingDefinitionInvalidException(HighlightingLoader.FormatExceptionMessage(message, lineInfo.LineNumber, lineInfo.LinePosition));
		}
		return new HighlightingDefinitionInvalidException(message);
	}

	private static void SetPosition(XshtdElement element, XmlReader reader)
	{
		if (reader is IXmlLineInfo xmlLineInfo)
		{
			element.LineNumber = xmlLineInfo.LineNumber;
			element.ColumnNumber = xmlLineInfo.LinePosition;
		}
	}

	private static void CheckElementName(XmlReader reader, string name)
	{
		if (name != null)
		{
			if (name.Length == 0)
			{
				throw Error(reader, "The empty string is not a valid name.");
			}
			if (name.IndexOf('/') >= 0)
			{
				throw Error(reader, "Element names must not contain a slash.");
			}
		}
	}

	private static XshtdColor ParseNamedColor(XmlReader reader, XshtdSyntaxDefinition syntax)
	{
		XshtdColor xshtdColor = ParseColorAttributes(reader, syntax);
		xshtdColor.Name = reader.GetAttribute("name");
		CheckElementName(reader, xshtdColor.Name);
		xshtdColor.ExampleText = reader.GetAttribute("exampleText");
		return xshtdColor;
	}

	private static XshtdColor ParseColorAttributes(XmlReader reader, XshtdSyntaxDefinition syntax)
	{
		XshtdColor xshtdColor = new XshtdColor(syntax);
		SetPosition(xshtdColor, reader);
		IXmlLineInfo lineInfo = reader as IXmlLineInfo;
		xshtdColor.Foreground = ParseColor(lineInfo, reader.GetAttribute("foreground"));
		xshtdColor.Background = ParseColor(lineInfo, reader.GetAttribute("background"));
		xshtdColor.FontWeight = ParseFontWeight(reader.GetAttribute("fontWeight"));
		xshtdColor.FontStyle = ParseFontStyle(reader.GetAttribute("fontStyle"));
		xshtdColor.Underline = reader.GetBoolAttribute("underline");
		return xshtdColor;
	}

	private static HighlightingBrush ParseColor(IXmlLineInfo lineInfo, string color)
	{
		if (string.IsNullOrEmpty(color))
		{
			return null;
		}
		if (color.StartsWith("SystemColors.", StringComparison.Ordinal))
		{
			return GetSystemColorBrush(lineInfo, color);
		}
		return FixedColorHighlightingBrush((Color?)ColorConverter.ConvertFromInvariantString(color));
	}

	internal static SystemColorHighlightingBrush GetSystemColorBrush(IXmlLineInfo lineInfo, string name)
	{
		string text = name.Substring(13);
		PropertyInfo property = typeof(SystemColors).GetProperty(text + "Brush");
		if (property == null)
		{
			throw Error(lineInfo, "Cannot find '" + name + "'.");
		}
		return new SystemColorHighlightingBrush(property);
	}

	private static HighlightingBrush FixedColorHighlightingBrush(Color? color)
	{
		if (!color.HasValue)
		{
			return null;
		}
		return new SimpleHighlightingBrush(color.Value);
	}

	private static FontWeight? ParseFontWeight(string fontWeight)
	{
		if (string.IsNullOrEmpty(fontWeight))
		{
			return null;
		}
		return (FontWeight?)FontWeightConverter.ConvertFromInvariantString(fontWeight);
	}

	private static FontStyle? ParseFontStyle(string fontStyle)
	{
		if (string.IsNullOrEmpty(fontStyle))
		{
			return null;
		}
		return (FontStyle?)FontStyleConverter.ConvertFromInvariantString(fontStyle);
	}
}
