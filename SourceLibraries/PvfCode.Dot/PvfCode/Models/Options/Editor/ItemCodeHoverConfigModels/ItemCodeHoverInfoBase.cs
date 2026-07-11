using System.Collections.Generic;
using System.Xml;

namespace PvfCode.Models.Options.Editor.ItemCodeHoverConfigModels;

public abstract class ItemCodeHoverInfoBase
{
	public string Description { get; set; }

	public string? ParentSectionName { get; set; }

	public List<string> LstFileNames { get; set; }

	public List<int>? IndexList { get; set; }

	public List<int>? IgnoreItemCodeList { get; set; }

	public List<KeyValuePair<string, ValidationSectionData>> ValidationSectionList { get; set; }

	public abstract ItemCodeHoverInfoBase Get(int index, int itemCode);

	public abstract void CreateXmlElement(KeyValuePair<string, ItemCodeHoverInfoBase> item, XmlDocument doc, XmlNode file);

	public abstract XmlNode CreateXml(KeyValuePair<string, ItemCodeHoverInfoBase> item, XmlDocument doc, XmlNode file);

	public bool EqualsData(KeyValuePair<string, ItemCodeHoverInfoBase> item, XmlDocument doc, XmlNode file, KeyValuePair<string, ItemCodeHoverInfoBase> item2)
	{
		XmlNode xmlNode = CreateXml(item, doc, file);
		XmlNode xmlNode2 = CreateXml(item2, doc, file);
		return xmlNode.InnerXml == xmlNode2.InnerXml;
	}
}
