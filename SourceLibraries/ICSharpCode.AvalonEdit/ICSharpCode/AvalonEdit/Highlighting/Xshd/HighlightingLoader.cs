using System;
using System.Xml;
using System.Xml.Schema;

namespace ICSharpCode.AvalonEdit.Highlighting.Xshd;

public static class HighlightingLoader
{
	public static XshdSyntaxDefinition LoadXshd(XmlReader reader)
	{
		return LoadXshd(reader, skipValidation: false);
	}

	internal static XshdSyntaxDefinition LoadXshd(XmlReader reader, bool skipValidation)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		try
		{
			reader.MoveToContent();
			if (reader.NamespaceURI == "http://icsharpcode.net/sharpdevelop/syntaxdefinition/2008")
			{
				return V2Loader.LoadDefinition(reader, skipValidation);
			}
			return V1Loader.LoadDefinition(reader, skipValidation);
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

	public static IHighlightingDefinition Load(XshdSyntaxDefinition syntaxDefinition, IHighlightingDefinitionReferenceResolver resolver)
	{
		if (syntaxDefinition == null)
		{
			throw new ArgumentNullException("syntaxDefinition");
		}
		return new XmlHighlightingDefinition(syntaxDefinition, resolver);
	}

	public static IHighlightingDefinition Load(XmlReader reader, IHighlightingDefinitionReferenceResolver resolver)
	{
		return Load(LoadXshd(reader), resolver);
	}
}
