using System;
using System.Xml;
using System.Xml.Schema;
using HL.Xshtd;
using HL.Xshtd.interfaces;
using ICSharpCode.AvalonEdit.Highlighting;

namespace HL.Manager;

internal static class HighlightingThemeLoader
{
	public static XhstdThemeDefinition LoadXshd(XmlReader reader)
	{
		return LoadXshd(reader, skipValidation: false);
	}

	internal static XhstdThemeDefinition LoadXshd(XmlReader reader, bool skipValidation)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		try
		{
			reader.MoveToContent();
			if (reader.NamespaceURI == "http://icsharpcode.net/sharpdevelop/themesyntaxdefinition/2019")
			{
				return XshtdLoader.LoadDefinition(reader, skipValidation);
			}
			throw new ArgumentOutOfRangeException(reader.NamespaceURI);
		}
		catch (XmlSchemaException ex)
		{
			throw WrapException(ex, ex.LineNumber, ex.LinePosition);
		}
		catch (XmlException ex2)
		{
			throw WrapException(ex2, ex2.LineNumber, ex2.LinePosition);
		}
	}

	private static Exception WrapException(Exception ex, int lineNumber, int linePosition)
	{
		return new HighlightingDefinitionInvalidException(FormatExceptionMessage(ex.Message, lineNumber, linePosition), ex);
	}

	internal static string FormatExceptionMessage(string message, int lineNumber, int linePosition)
	{
		if (lineNumber <= 0)
		{
			return message;
		}
		return "Error at position (line " + lineNumber + ", column " + linePosition + "):\n" + message;
	}

	internal static XmlReader GetValidatingReader(XmlReader input, bool ignoreWhitespace, XmlSchemaSet schemaSet)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.CloseInput = true;
		xmlReaderSettings.IgnoreComments = true;
		xmlReaderSettings.IgnoreWhitespace = ignoreWhitespace;
		if (schemaSet != null)
		{
			xmlReaderSettings.Schemas = schemaSet;
			xmlReaderSettings.ValidationType = ValidationType.Schema;
		}
		return XmlReader.Create(input, xmlReaderSettings);
	}

	internal static XmlSchemaSet LoadSchemaSet(XmlReader schemaInput)
	{
		XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
		xmlSchemaSet.Add(null, schemaInput);
		xmlSchemaSet.ValidationEventHandler += delegate(object sender, ValidationEventArgs args)
		{
			throw new HighlightingDefinitionInvalidException(args.Message);
		};
		return xmlSchemaSet;
	}

	public static IHighlightingThemeDefinition Load(XhstdThemeDefinition syntaxDefinition, IHighlightingThemeDefinitionReferenceResolver resolver)
	{
		if (syntaxDefinition == null)
		{
			throw new ArgumentNullException("syntaxDefinition");
		}
		return new XmlHighlightingThemeDefinition(syntaxDefinition, resolver);
	}

	public static IHighlightingThemeDefinition Load(XmlReader reader, IHighlightingThemeDefinitionReferenceResolver resolver)
	{
		return Load(LoadXshd(reader), resolver);
	}
}
