using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Folding;

public class XmlFoldingStrategy
{
	public bool ShowAttributesWhenFolded { get; set; }

	public void UpdateFoldings(FoldingManager manager, TextDocument document)
	{
		int firstErrorOffset;
		IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out firstErrorOffset);
		manager.UpdateFoldings(newFoldings, firstErrorOffset);
	}

	public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
	{
		try
		{
			XmlTextReader xmlTextReader = new XmlTextReader(document.CreateReader());
			xmlTextReader.XmlResolver = null;
			return CreateNewFoldings(document, xmlTextReader, out firstErrorOffset);
		}
		catch (XmlException)
		{
			firstErrorOffset = 0;
			return Enumerable.Empty<NewFolding>();
		}
	}

	public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, XmlReader reader, out int firstErrorOffset)
	{
		Stack<XmlFoldStart> stack = new Stack<XmlFoldStart>();
		List<NewFolding> list = new List<NewFolding>();
		try
		{
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
				case XmlNodeType.Element:
					if (!reader.IsEmptyElement)
					{
						XmlFoldStart item = CreateElementFoldStart(document, reader);
						stack.Push(item);
					}
					break;
				case XmlNodeType.EndElement:
				{
					XmlFoldStart foldStart = stack.Pop();
					CreateElementFold(document, list, reader, foldStart);
					break;
				}
				case XmlNodeType.Comment:
					CreateCommentFold(document, list, reader);
					break;
				}
			}
			firstErrorOffset = -1;
		}
		catch (XmlException ex)
		{
			if (ex.LineNumber >= 1 && ex.LineNumber <= document.LineCount)
			{
				firstErrorOffset = document.GetOffset(ex.LineNumber, ex.LinePosition);
			}
			else
			{
				firstErrorOffset = 0;
			}
		}
		list.Sort((NewFolding a, NewFolding b) => a.StartOffset.CompareTo(b.StartOffset));
		return list;
	}

	private static int GetOffset(TextDocument document, XmlReader reader)
	{
		if (reader is IXmlLineInfo xmlLineInfo && xmlLineInfo.HasLineInfo())
		{
			return document.GetOffset(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
		}
		throw new ArgumentException("XmlReader does not have positioning information.");
	}

	private static void CreateCommentFold(TextDocument document, List<NewFolding> foldMarkers, XmlReader reader)
	{
		string value = reader.Value;
		if (value != null)
		{
			int num = value.IndexOf('\n');
			if (num >= 0)
			{
				int num2 = GetOffset(document, reader) - 4;
				int end = num2 + value.Length + 7;
				string name = "<!--" + value.Substring(0, num).TrimEnd('\r') + "-->";
				foldMarkers.Add(new NewFolding(num2, end)
				{
					Name = name
				});
			}
		}
	}

	private XmlFoldStart CreateElementFoldStart(TextDocument document, XmlReader reader)
	{
		XmlFoldStart xmlFoldStart = new XmlFoldStart();
		IXmlLineInfo xmlLineInfo = (IXmlLineInfo)reader;
		xmlFoldStart.StartLine = xmlLineInfo.LineNumber;
		xmlFoldStart.StartOffset = document.GetOffset(xmlFoldStart.StartLine, xmlLineInfo.LinePosition - 1);
		if (ShowAttributesWhenFolded && reader.HasAttributes)
		{
			xmlFoldStart.Name = "<" + reader.Name + " " + GetAttributeFoldText(reader) + ">";
		}
		else
		{
			xmlFoldStart.Name = "<" + reader.Name + ">";
		}
		return xmlFoldStart;
	}

	private static void CreateElementFold(TextDocument document, List<NewFolding> foldMarkers, XmlReader reader, XmlFoldStart foldStart)
	{
		IXmlLineInfo xmlLineInfo = (IXmlLineInfo)reader;
		int lineNumber = xmlLineInfo.LineNumber;
		if (lineNumber > foldStart.StartLine)
		{
			int column = xmlLineInfo.LinePosition + reader.Name.Length + 1;
			foldStart.EndOffset = document.GetOffset(lineNumber, column);
			foldMarkers.Add(foldStart);
		}
	}

	private static string GetAttributeFoldText(XmlReader reader)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < reader.AttributeCount; i++)
		{
			reader.MoveToAttribute(i);
			stringBuilder.Append(reader.Name);
			stringBuilder.Append("=");
			stringBuilder.Append(reader.QuoteChar.ToString());
			stringBuilder.Append(XmlEncodeAttributeValue(reader.Value, reader.QuoteChar));
			stringBuilder.Append(reader.QuoteChar.ToString());
			if (i < reader.AttributeCount - 1)
			{
				stringBuilder.Append(" ");
			}
		}
		return stringBuilder.ToString();
	}

	private static string XmlEncodeAttributeValue(string attributeValue, char quoteChar)
	{
		StringBuilder stringBuilder = new StringBuilder(attributeValue);
		stringBuilder.Replace("&", "&amp;");
		stringBuilder.Replace("<", "&lt;");
		stringBuilder.Replace(">", "&gt;");
		if (quoteChar == '"')
		{
			stringBuilder.Replace("\"", "&quot;");
		}
		else
		{
			stringBuilder.Replace("'", "&apos;");
		}
		return stringBuilder.ToString();
	}
}
