using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

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
		Stack<XmlElementFolding> stack = new Stack<XmlElementFolding>();
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
						XmlElementFolding item = CreateElementFoldStart(document, reader);
						stack.Push(item);
					}
					break;
				case XmlNodeType.EndElement:
				{
					XmlElementFolding folding = stack.Pop();
					CreateElementFold(document, list, reader, folding);
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

	private XmlElementFolding CreateElementFoldStart(TextDocument document, XmlReader reader)
	{
		XmlElementFolding folding = new XmlElementFolding();
		IXmlLineInfo xmlLineInfo = (IXmlLineInfo)reader;
		folding.StartLine = xmlLineInfo.LineNumber;
		folding.StartOffset = document.GetOffset(folding.StartLine, xmlLineInfo.LinePosition - 1);
		if (ShowAttributesWhenFolded && reader.HasAttributes)
		{
			folding.Name = "<" + reader.Name + " " + GetAttributeFoldText(reader) + ">";
		}
		else
		{
			folding.Name = "<" + reader.Name + ">";
		}
		return folding;
	}

	private static void CreateElementFold(TextDocument document, List<NewFolding> foldMarkers, XmlReader reader, XmlElementFolding foldStart)
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

	public XmlFoldingStrategy()
	{
	}
}
