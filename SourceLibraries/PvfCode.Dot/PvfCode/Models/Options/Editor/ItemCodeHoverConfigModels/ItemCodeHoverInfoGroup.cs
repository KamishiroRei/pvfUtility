using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace PvfCode.Models.Options.Editor.ItemCodeHoverConfigModels;

public class ItemCodeHoverInfoGroup : ItemCodeHoverInfoBase
{
	public Dictionary<int, ItemCodeHoverInfoBase> ChildNodes { get; set; }

	public override ItemCodeHoverInfoBase Get(int index, int itemCode)
	{
		if (ChildNodes == null || !ChildNodes.Any())
		{
			return null;
		}
		if (base.IgnoreItemCodeList != null && base.IgnoreItemCodeList.Contains(itemCode))
		{
			return null;
		}
		if (ChildNodes.TryGetValue(index, out ItemCodeHoverInfoBase value))
		{
			if (value.IgnoreItemCodeList != null && value.IgnoreItemCodeList.Contains(itemCode))
			{
				return null;
			}
			return value;
		}
		return null;
	}

	public override void CreateXmlElement(KeyValuePair<string, ItemCodeHoverInfoBase> item, XmlDocument doc, XmlNode file)
	{
		CreateXml(item, doc, file);
	}

	public override XmlNode CreateXml(KeyValuePair<string, ItemCodeHoverInfoBase> item, XmlDocument doc, XmlNode file)
	{
		XmlElement xmlElement = doc.CreateElement("SectionGroup");
		xmlElement.SetAttribute("SectionName", item.Key);
		foreach (KeyValuePair<int, ItemCodeHoverInfoBase> childNode in ChildNodes)
		{
			XmlElement xmlElement2 = doc.CreateElement("Index");
			if (childNode.Value.IndexList != null && childNode.Value.IndexList.Count > 0)
			{
				xmlElement2.SetAttribute("Value", childNode.Value.IndexList[0].ToString());
			}
			if (childNode.Value.LstFileNames != null)
			{
				xmlElement2.SetAttribute("LstFileName", string.Join(",", childNode.Value.LstFileNames));
			}
			xmlElement2.SetAttribute("Description", childNode.Value.Description);
			if (childNode.Value.IgnoreItemCodeList != null && childNode.Value.IgnoreItemCodeList.Any())
			{
				xmlElement2.SetAttribute("IgnoreItemCode", string.Join(",", childNode.Value.IgnoreItemCodeList));
			}
			xmlElement.AppendChild(xmlElement2);
		}
		if (!string.IsNullOrEmpty(item.Value.ParentSectionName))
		{
			xmlElement.SetAttribute("ParentSectionName", item.Value.ParentSectionName);
		}
		if (item.Value.ValidationSectionList != null && item.Value.ValidationSectionList.Any())
		{
			foreach (KeyValuePair<string, ValidationSectionData> validationSection in item.Value.ValidationSectionList)
			{
				XmlElement xmlElement3 = doc.CreateElement("ValidationSection");
				xmlElement3.SetAttribute("Name", validationSection.Key);
				xmlElement3.SetAttribute("Value", validationSection.Value.Value);
				if (validationSection.Value.Index.HasValue)
				{
					xmlElement3.SetAttribute("Index", validationSection.Value.Index.Value.ToString());
				}
				if (validationSection.Value.CurrentLine.HasValue)
				{
					xmlElement3.SetAttribute("CurrentLine", validationSection.Value.CurrentLine.Value.ToString());
				}
				xmlElement.AppendChild(xmlElement3);
			}
		}
		file.AppendChild(xmlElement);
		return xmlElement;
	}
}
