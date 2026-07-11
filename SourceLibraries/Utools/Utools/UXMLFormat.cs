using System.IO;
using System.Xml;

namespace Utools;

public class UXMLFormat
{
	public static string FormatXML(string XMLstring)
	{
		if (!XMLstring.Contains("<?xml version"))
		{
			return XMLstring;
		}
		return ConvertXmlDocumentTostring(GetXmlDocument(XMLstring));
	}

	public static string ConvertXmlDocumentTostring(XmlDocument xmlDocument)
	{
		MemoryStream memoryStream = new MemoryStream();
		XmlTextWriter w = new XmlTextWriter(memoryStream, null)
		{
			Formatting = Formatting.Indented,
			IndentChar = '\t'
		};
		xmlDocument.Save(w);
		StreamReader streamReader = new StreamReader(memoryStream);
		memoryStream.Position = 0L;
		string result = streamReader.ReadToEnd();
		streamReader.Close();
		memoryStream.Close();
		return result;
	}

	public static XmlDocument GetXmlDocument(string xmlString)
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xmlString);
		return xmlDocument;
	}

	public UXMLFormat()
	{
	}
}
