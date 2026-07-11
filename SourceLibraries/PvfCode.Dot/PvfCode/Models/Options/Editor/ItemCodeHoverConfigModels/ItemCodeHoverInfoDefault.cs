using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace PvfCode.Models.Options.Editor.ItemCodeHoverConfigModels;

public class ItemCodeHoverInfoDefault : ItemCodeHoverInfoBase
{
	public override XmlNode CreateXml(KeyValuePair<string, ItemCodeHoverInfoBase> item, XmlDocument doc, XmlNode file)
	{
		XmlElement xmlElement = doc.CreateElement("Section");
		xmlElement.SetAttribute("SectionName", item.Key);
		if (item.Value.IndexList != null)
		{
			xmlElement.SetAttribute("Index", string.Join(",", item.Value.IndexList));
		}
		if (item.Value.LstFileNames != null)
		{
			xmlElement.SetAttribute("LstFileName", string.Join(",", item.Value.LstFileNames));
		}
		xmlElement.SetAttribute("Description", item.Value.Description);
		if (item.Value.IgnoreItemCodeList != null && item.Value.IgnoreItemCodeList.Any())
		{
			xmlElement.SetAttribute("IgnoreItemCode", string.Join(",", item.Value.IgnoreItemCodeList));
		}
		if (!string.IsNullOrEmpty(item.Value.ParentSectionName))
		{
			xmlElement.SetAttribute("ParentSectionName", item.Value.ParentSectionName);
		}
		if (item.Value.ValidationSectionList != null && item.Value.ValidationSectionList.Any())
		{
			foreach (KeyValuePair<string, ValidationSectionData> validationSection in item.Value.ValidationSectionList)
			{
				XmlElement xmlElement2 = doc.CreateElement("ValidationSection");
				xmlElement2.SetAttribute("Name", validationSection.Key);
				xmlElement2.SetAttribute("Value", validationSection.Value.Value);
				if (validationSection.Value.Index.HasValue)
				{
					xmlElement2.SetAttribute("Index", validationSection.Value.Index.Value.ToString());
				}
				if (validationSection.Value.CurrentLine.HasValue)
				{
					xmlElement2.SetAttribute("CurrentLine", validationSection.Value.CurrentLine.Value.ToString());
				}
				xmlElement.AppendChild(xmlElement2);
			}
		}
		file.AppendChild(xmlElement);
		return xmlElement;
	}

	public override void CreateXmlElement(KeyValuePair<string, ItemCodeHoverInfoBase> item, XmlDocument doc, XmlNode file)
	{
		CreateXml(item, doc, file);
	}

	public override ItemCodeHoverInfoBase Get(int index, int itemCode)
	{
		if (base.IgnoreItemCodeList != null && base.IgnoreItemCodeList.Contains(itemCode))
		{
			return null;
		}
		if (base.IndexList != null && base.IndexList.Any() && !base.IndexList.Contains(index))
		{
			return null;
		}
		return this;
	}
}
