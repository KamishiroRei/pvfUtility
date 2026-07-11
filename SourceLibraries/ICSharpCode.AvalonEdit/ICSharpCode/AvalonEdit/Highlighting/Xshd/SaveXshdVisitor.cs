using System;
using System.Linq;
using System.Xml;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

public sealed class SaveXshdVisitor : IXshdVisitor
{
	public const string Namespace = "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008";

	private XmlWriter writer;

	public SaveXshdVisitor(XmlWriter writer)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		this.writer = writer;
	}

	public void WriteDefinition(XshdSyntaxDefinition definition)
	{
		if (definition == null)
		{
			throw new ArgumentNullException("definition");
		}
		writer.WriteStartElement("SyntaxDefinition", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		if (definition.Name != null)
		{
			writer.WriteAttributeString("name", definition.Name);
		}
		if (definition.Extensions != null)
		{
			writer.WriteAttributeString("extensions", string.Join(";", definition.Extensions.ToArray()));
		}
		definition.AcceptElements(this);
		writer.WriteEndElement();
	}

	object IXshdVisitor.VisitRuleSet(XshdRuleSet ruleSet)
	{
		writer.WriteStartElement("RuleSet", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		if (ruleSet.Name != null)
		{
			writer.WriteAttributeString("name", ruleSet.Name);
		}
		WriteBoolAttribute("ignoreCase", ruleSet.IgnoreCase);
		ruleSet.AcceptElements(this);
		writer.WriteEndElement();
		return null;
	}

	private void WriteBoolAttribute(string attributeName, bool? value)
	{
		if (value.HasValue)
		{
			writer.WriteAttributeString(attributeName, value.Value ? "true" : "false");
		}
	}

	private void WriteRuleSetReference(XshdReference<XshdRuleSet> ruleSetReference)
	{
		if (ruleSetReference.ReferencedElement != null)
		{
			if (ruleSetReference.ReferencedDefinition != null)
			{
				writer.WriteAttributeString("ruleSet", ruleSetReference.ReferencedDefinition + "/" + ruleSetReference.ReferencedElement);
			}
			else
			{
				writer.WriteAttributeString("ruleSet", ruleSetReference.ReferencedElement);
			}
		}
	}

	private void WriteColorReference(XshdReference<XshdColor> color)
	{
		if (color.InlineElement != null)
		{
			WriteColorAttributes(color.InlineElement);
		}
		else if (color.ReferencedElement != null)
		{
			if (color.ReferencedDefinition != null)
			{
				writer.WriteAttributeString("color", color.ReferencedDefinition + "/" + color.ReferencedElement);
			}
			else
			{
				writer.WriteAttributeString("color", color.ReferencedElement);
			}
		}
	}

	private void WriteColorAttributes(XshdColor color)
	{
		if (color.Foreground != null)
		{
			writer.WriteAttributeString("foreground", color.Foreground.ToString());
		}
		if (color.Background != null)
		{
			writer.WriteAttributeString("background", color.Background.ToString());
		}
		if (color.FontWeight.HasValue)
		{
			writer.WriteAttributeString("fontWeight", V2Loader.FontWeightConverter.ConvertToInvariantString(color.FontWeight.Value).ToLowerInvariant());
		}
		if (color.FontStyle.HasValue)
		{
			writer.WriteAttributeString("fontStyle", V2Loader.FontStyleConverter.ConvertToInvariantString(color.FontStyle.Value).ToLowerInvariant());
		}
	}

	object IXshdVisitor.VisitColor(XshdColor color)
	{
		writer.WriteStartElement("Color", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		if (color.Name != null)
		{
			writer.WriteAttributeString("name", color.Name);
		}
		WriteColorAttributes(color);
		if (color.ExampleText != null)
		{
			writer.WriteAttributeString("exampleText", color.ExampleText);
		}
		writer.WriteEndElement();
		return null;
	}

	object IXshdVisitor.VisitKeywords(XshdKeywords keywords)
	{
		writer.WriteStartElement("Keywords", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		WriteColorReference(keywords.ColorReference);
		foreach (string word in keywords.Words)
		{
			writer.WriteElementString("Word", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008", word);
		}
		writer.WriteEndElement();
		return null;
	}

	object IXshdVisitor.VisitSpan(XshdSpan span)
	{
		writer.WriteStartElement("Span", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		WriteColorReference(span.SpanColorReference);
		if (span.BeginRegexType == XshdRegexType.Default && span.BeginRegex != null)
		{
			writer.WriteAttributeString("begin", span.BeginRegex);
		}
		if (span.EndRegexType == XshdRegexType.Default && span.EndRegex != null)
		{
			writer.WriteAttributeString("end", span.EndRegex);
		}
		WriteRuleSetReference(span.RuleSetReference);
		if (span.Multiline)
		{
			writer.WriteAttributeString("multiline", "true");
		}
		if (span.BeginRegexType == XshdRegexType.IgnorePatternWhitespace)
		{
			WriteBeginEndElement("Begin", span.BeginRegex, span.BeginColorReference);
		}
		if (span.EndRegexType == XshdRegexType.IgnorePatternWhitespace)
		{
			WriteBeginEndElement("End", span.EndRegex, span.EndColorReference);
		}
		if (span.RuleSetReference.InlineElement != null)
		{
			span.RuleSetReference.InlineElement.AcceptVisitor(this);
		}
		writer.WriteEndElement();
		return null;
	}

	private void WriteBeginEndElement(string elementName, string regex, XshdReference<XshdColor> colorReference)
	{
		if (regex != null)
		{
			writer.WriteStartElement(elementName, "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
			WriteColorReference(colorReference);
			writer.WriteString(regex);
			writer.WriteEndElement();
		}
	}

	object IXshdVisitor.VisitImport(XshdImport import)
	{
		writer.WriteStartElement("Import", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		WriteRuleSetReference(import.RuleSetReference);
		writer.WriteEndElement();
		return null;
	}

	object IXshdVisitor.VisitRule(XshdRule rule)
	{
		writer.WriteStartElement("Rule", "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008");
		WriteColorReference(rule.ColorReference);
		writer.WriteString(rule.Regex);
		writer.WriteEndElement();
		return null;
	}
}
