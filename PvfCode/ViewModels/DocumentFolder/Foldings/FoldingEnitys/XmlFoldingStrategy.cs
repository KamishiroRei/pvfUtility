using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.ViewModels.DocumentFolder.Foldings.FoldingEnitys;

public class XmlFoldingStrategy
{
	[CompilerGenerated]
	private bool xAlyuJbHgg;

	public bool ShowAttributesWhenFolded
	{
		[CompilerGenerated]
		get
		{
			return xAlyuJbHgg;
		}
		[CompilerGenerated]
		set
		{
			xAlyuJbHgg = value;
		}
	}

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
						XmlElementFolding item = kbmy4H88iY(document, reader);
						stack.Push(item);
					}
					break;
				case XmlNodeType.EndElement:
				{
					XmlElementFolding folding = stack.Pop();
					zn7yYsLQDL(document, list, reader, folding);
					break;
				}
				case XmlNodeType.Comment:
					NBEyAYuGPJ(document, list, reader);
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

	private static int t5FySEShpo(TextDocument P_0, XmlReader P_1)
	{
		if (P_1 is IXmlLineInfo xmlLineInfo && xmlLineInfo.HasLineInfo())
		{
			return P_0.GetOffset(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
		}
		throw new ArgumentException("XmlReader does not have positioning information.");
	}

	private static void NBEyAYuGPJ(TextDocument P_0, List<NewFolding> P_1, XmlReader P_2)
	{
		string value = P_2.Value;
		if (value != null)
		{
			int num = value.IndexOf('\n');
			if (num >= 0)
			{
				int num2 = t5FySEShpo(P_0, P_2) - 4;
				int end = num2 + value.Length + 7;
				string name = "<!--" + value.Substring(0, num).TrimEnd('\r') + "-->";
				P_1.Add(new NewFolding(num2, end)
				{
					Name = name
				});
			}
		}
	}

	private XmlElementFolding kbmy4H88iY(TextDocument P_0, XmlReader P_1)
	{
		XmlElementFolding folding = new XmlElementFolding();
		IXmlLineInfo xmlLineInfo = (IXmlLineInfo)P_1;
		folding.StartLine = xmlLineInfo.LineNumber;
		folding.StartOffset = P_0.GetOffset(folding.StartLine, xmlLineInfo.LinePosition - 1);
		if (ShowAttributesWhenFolded && P_1.HasAttributes)
		{
			folding.Name = "<" + P_1.Name + " " + B9DyyQ2yyi(P_1) + ">";
		}
		else
		{
			folding.Name = "<" + P_1.Name + ">";
		}
		return folding;
	}

	private static void zn7yYsLQDL(TextDocument P_0, List<NewFolding> P_1, XmlReader P_2, XmlElementFolding P_3)
	{
		IXmlLineInfo xmlLineInfo = (IXmlLineInfo)P_2;
		int lineNumber = xmlLineInfo.LineNumber;
		if (lineNumber > P_3.StartLine)
		{
			int column = xmlLineInfo.LinePosition + P_2.Name.Length + 1;
			P_3.EndOffset = P_0.GetOffset(lineNumber, column);
			P_1.Add(P_3);
		}
	}

	private static string B9DyyQ2yyi(XmlReader P_0)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < P_0.AttributeCount; i++)
		{
			P_0.MoveToAttribute(i);
			stringBuilder.Append(P_0.Name);
			stringBuilder.Append("=");
			stringBuilder.Append(P_0.QuoteChar.ToString());
			stringBuilder.Append(IaYyiv8Oww(P_0.Value, P_0.QuoteChar));
			stringBuilder.Append(P_0.QuoteChar.ToString());
			if (i < P_0.AttributeCount - 1)
			{
				stringBuilder.Append(" ");
			}
		}
		return stringBuilder.ToString();
	}

	private static string IaYyiv8Oww(string P_0, char P_1)
	{
		StringBuilder stringBuilder = new StringBuilder(P_0);
		stringBuilder.Replace("&", "&amp;");
		stringBuilder.Replace("<", "&lt;");
		stringBuilder.Replace(">", "&gt;");
		if (P_1 == '"')
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
