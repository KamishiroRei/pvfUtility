using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

internal sealed class V1Loader
{
	private static XmlSchemaSet schemaSet;

	private char ruleSetEscapeCharacter;

	private static XmlSchemaSet SchemaSet
	{
		get
		{
			if (schemaSet == null)
			{
				schemaSet = HighlightingLoader.LoadSchemaSet(new XmlTextReader(Resources.OpenStream("ModeV1.xsd")));
			}
			return schemaSet;
		}
	}

	public static XshdSyntaxDefinition LoadDefinition(XmlReader reader, bool skipValidation)
	{
		reader = HighlightingLoader.GetValidatingReader(reader, ignoreWhitespace: false, skipValidation ? null : SchemaSet);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(reader);
		return new V1Loader().ParseDefinition(xmlDocument.DocumentElement);
	}

	private XshdSyntaxDefinition ParseDefinition(XmlElement syntaxDefinition)
	{
		XshdSyntaxDefinition xshdSyntaxDefinition = new XshdSyntaxDefinition();
		xshdSyntaxDefinition.Name = syntaxDefinition.GetAttributeOrNull("name");
		if (syntaxDefinition.HasAttribute("extensions"))
		{
			xshdSyntaxDefinition.Extensions.AddRange(syntaxDefinition.GetAttribute("extensions").Split(';', '|'));
		}
		XshdRuleSet xshdRuleSet = null;
		foreach (XmlElement item in syntaxDefinition.GetElementsByTagName("RuleSet"))
		{
			XshdRuleSet xshdRuleSet2 = ImportRuleSet(item);
			xshdSyntaxDefinition.Elements.Add(xshdRuleSet2);
			if (xshdRuleSet2.Name == null)
			{
				xshdRuleSet = xshdRuleSet2;
			}
			if (syntaxDefinition["Digits"] != null)
			{
				xshdRuleSet2.Elements.Add(new XshdRule
				{
					ColorReference = GetColorReference(syntaxDefinition["Digits"]),
					RegexType = XshdRegexType.IgnorePatternWhitespace,
					Regex = "\\b0[xX][0-9a-fA-F]+|(\\b\\d+(\\.[0-9]+)?|\\.[0-9]+)([eE][+-]?[0-9]+)?"
				});
			}
		}
		if (syntaxDefinition.HasAttribute("extends"))
		{
			xshdRuleSet?.Elements.Add(new XshdImport
			{
				RuleSetReference = new XshdReference<XshdRuleSet>(syntaxDefinition.GetAttribute("extends"), string.Empty)
			});
		}
		return xshdSyntaxDefinition;
	}

	private static XshdColor GetColorFromElement(XmlElement element)
	{
		if (!element.HasAttribute("bold") && !element.HasAttribute("italic") && !element.HasAttribute("color") && !element.HasAttribute("bgcolor"))
		{
			return null;
		}
		XshdColor xshdColor = new XshdColor();
		if (element.HasAttribute("bold"))
		{
			xshdColor.FontWeight = (XmlConvert.ToBoolean(element.GetAttribute("bold")) ? FontWeights.Bold : FontWeights.Normal);
		}
		if (element.HasAttribute("italic"))
		{
			xshdColor.FontStyle = (XmlConvert.ToBoolean(element.GetAttribute("italic")) ? FontStyles.Italic : FontStyles.Normal);
		}
		if (element.HasAttribute("color"))
		{
			xshdColor.Foreground = ParseColor(element.GetAttribute("color"));
		}
		if (element.HasAttribute("bgcolor"))
		{
			xshdColor.Background = ParseColor(element.GetAttribute("bgcolor"));
		}
		return xshdColor;
	}

	private static XshdReference<XshdColor> GetColorReference(XmlElement element)
	{
		XshdColor colorFromElement = GetColorFromElement(element);
		if (colorFromElement != null)
		{
			return new XshdReference<XshdColor>(colorFromElement);
		}
		return default(XshdReference<XshdColor>);
	}

	private static HighlightingBrush ParseColor(string c)
	{
		if (c.StartsWith("#", StringComparison.Ordinal))
		{
			int num = 255;
			int num2 = 0;
			if (c.Length > 7)
			{
				num2 = 2;
				num = int.Parse(c.Substring(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}
			int num3 = int.Parse(c.Substring(1 + num2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			int num4 = int.Parse(c.Substring(3 + num2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			int num5 = int.Parse(c.Substring(5 + num2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			return new SimpleHighlightingBrush(Color.FromArgb((byte)num, (byte)num3, (byte)num4, (byte)num5));
		}
		if (c.StartsWith("SystemColors.", StringComparison.Ordinal))
		{
			return V2Loader.GetSystemColorBrush(null, c);
		}
		return new SimpleHighlightingBrush((Color)V2Loader.ColorConverter.ConvertFromInvariantString(c));
	}

	private XshdRuleSet ImportRuleSet(XmlElement element)
	{
		XshdRuleSet xshdRuleSet = new XshdRuleSet();
		xshdRuleSet.Name = element.GetAttributeOrNull("name");
		if (element.HasAttribute("escapecharacter"))
		{
			ruleSetEscapeCharacter = element.GetAttribute("escapecharacter")[0];
		}
		else
		{
			ruleSetEscapeCharacter = '\0';
		}
		if (element.HasAttribute("reference"))
		{
			xshdRuleSet.Elements.Add(new XshdImport
			{
				RuleSetReference = new XshdReference<XshdRuleSet>(element.GetAttribute("reference"), string.Empty)
			});
		}
		xshdRuleSet.IgnoreCase = element.GetBoolAttribute("ignorecase");
		foreach (XmlElement item in element.GetElementsByTagName("KeyWords"))
		{
			XshdKeywords xshdKeywords = new XshdKeywords();
			xshdKeywords.ColorReference = GetColorReference(item);
			foreach (XmlElement item2 in item.GetElementsByTagName("Key"))
			{
				string attribute = item2.GetAttribute("word");
				if (!string.IsNullOrEmpty(attribute))
				{
					xshdKeywords.Words.Add(attribute);
				}
			}
			if (xshdKeywords.Words.Count > 0)
			{
				xshdRuleSet.Elements.Add(xshdKeywords);
			}
		}
		foreach (XmlElement item3 in element.GetElementsByTagName("Span"))
		{
			xshdRuleSet.Elements.Add(ImportSpan(item3));
		}
		foreach (XmlElement item4 in element.GetElementsByTagName("MarkPrevious"))
		{
			xshdRuleSet.Elements.Add(ImportMarkPrevNext(item4, markFollowing: false));
		}
		foreach (XmlElement item5 in element.GetElementsByTagName("MarkFollowing"))
		{
			xshdRuleSet.Elements.Add(ImportMarkPrevNext(item5, markFollowing: true));
		}
		return xshdRuleSet;
	}

	private static XshdRule ImportMarkPrevNext(XmlElement el, bool markFollowing)
	{
		bool valueOrDefault = el.GetBoolAttribute("markmarker") == true;
		string text = Regex.Escape(el.InnerText);
		string regex = (markFollowing ? ((!valueOrDefault) ? ("(?<=(" + text + "\\s*))[\\d\\w_]+") : (text + "\\s*[\\d\\w_]+")) : ((!valueOrDefault) ? ("[\\d\\w_]+(?=(\\s*" + text + "))") : ("[\\d\\w_]+\\s*" + text)));
		return new XshdRule
		{
			ColorReference = GetColorReference(el),
			Regex = regex,
			RegexType = XshdRegexType.IgnorePatternWhitespace
		};
	}

	private XshdSpan ImportSpan(XmlElement element)
	{
		XshdSpan xshdSpan = new XshdSpan();
		if (element.HasAttribute("rule"))
		{
			xshdSpan.RuleSetReference = new XshdReference<XshdRuleSet>(null, element.GetAttribute("rule"));
		}
		char c = ruleSetEscapeCharacter;
		if (element.HasAttribute("escapecharacter"))
		{
			c = element.GetAttribute("escapecharacter")[0];
		}
		xshdSpan.Multiline = element.GetBoolAttribute("stopateol") != true;
		xshdSpan.SpanColorReference = GetColorReference(element);
		xshdSpan.BeginRegexType = XshdRegexType.IgnorePatternWhitespace;
		xshdSpan.BeginRegex = ImportRegex(element["Begin"].InnerText, element["Begin"].GetBoolAttribute("singleword") == true, element["Begin"].GetBoolAttribute("startofline"));
		xshdSpan.BeginColorReference = GetColorReference(element["Begin"]);
		string text = string.Empty;
		if (element["End"] != null)
		{
			xshdSpan.EndRegexType = XshdRegexType.IgnorePatternWhitespace;
			text = element["End"].InnerText;
			xshdSpan.EndRegex = ImportRegex(text, element["End"].GetBoolAttribute("singleword") == true, null);
			xshdSpan.EndColorReference = GetColorReference(element["End"]);
		}
		if (c != 0)
		{
			XshdRuleSet xshdRuleSet = new XshdRuleSet();
			if (text.Length == 1 && text[0] == c)
			{
				xshdRuleSet.Elements.Add(new XshdSpan
				{
					BeginRegex = Regex.Escape(text + text),
					EndRegex = ""
				});
			}
			else
			{
				xshdRuleSet.Elements.Add(new XshdSpan
				{
					BeginRegex = Regex.Escape(c.ToString()),
					EndRegex = "."
				});
			}
			if (xshdSpan.RuleSetReference.ReferencedElement != null)
			{
				xshdRuleSet.Elements.Add(new XshdImport
				{
					RuleSetReference = xshdSpan.RuleSetReference
				});
			}
			xshdSpan.RuleSetReference = new XshdReference<XshdRuleSet>(xshdRuleSet);
		}
		return xshdSpan;
	}

	private static string ImportRegex(string expr, bool singleWord, bool? startOfLine)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (startOfLine.HasValue)
		{
			if (startOfLine.Value)
			{
				stringBuilder.Append("(?<=(^\\s*))");
			}
			else
			{
				stringBuilder.Append("(?<!(^\\s*))");
			}
		}
		else if (singleWord)
		{
			stringBuilder.Append("\\b");
		}
		for (int i = 0; i < expr.Length; i++)
		{
			char c = expr[i];
			if (c == '@')
			{
				i++;
				if (i == expr.Length)
				{
					throw new HighlightingDefinitionInvalidException("Unexpected end of @ sequence, use @@ to look for a single @.");
				}
				switch (expr[i])
				{
				case 'C':
					stringBuilder.Append("[^\\w\\d_]");
					break;
				case '!':
				{
					StringBuilder stringBuilder3 = new StringBuilder();
					i++;
					while (i < expr.Length && expr[i] != '@')
					{
						stringBuilder3.Append(expr[i++]);
					}
					stringBuilder.Append("(?!(");
					stringBuilder.Append(Regex.Escape(stringBuilder3.ToString()));
					stringBuilder.Append("))");
					break;
				}
				case '-':
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					i++;
					while (i < expr.Length && expr[i] != '@')
					{
						stringBuilder2.Append(expr[i++]);
					}
					stringBuilder.Append("(?<!(");
					stringBuilder.Append(Regex.Escape(stringBuilder2.ToString()));
					stringBuilder.Append("))");
					break;
				}
				case '@':
					stringBuilder.Append("@");
					break;
				default:
					throw new HighlightingDefinitionInvalidException("Unknown character in @ sequence.");
				}
			}
			else
			{
				stringBuilder.Append(Regex.Escape(c.ToString()));
			}
		}
		if (singleWord)
		{
			stringBuilder.Append("\\b");
		}
		return stringBuilder.ToString();
	}
}
