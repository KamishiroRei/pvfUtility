using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PvfCode.Models.Options.Editor.ItemCodeHoverConfigModels;
using Utools;

namespace PvfCode.Models.Options.Editor;

public class ItemCodeConvertItemNameConfiger
{
	public string SavePath { get; set; }

	public Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>> DIC { get; set; }

	public ItemCodeConvertItemNameConfiger()
	{
		SavePath = Path.Combine(AppSetting.AppBasePath, "ItemCodeHoverConfig.xml");
		DIC = new Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>>();
	}

	public List<KeyValuePair<string, ItemCodeHoverInfoBase>> Get(string fileName, string sectionName, int index, int itemCode)
	{
		List<KeyValuePair<string, ItemCodeHoverInfoBase>> list = new List<KeyValuePair<string, ItemCodeHoverInfoBase>>();
		if (DIC == null)
		{
			return null;
		}
		if (Path.GetDirectoryName(fileName).Replace("\\", "/") == "etc/independentdrop")
		{
			if (DIC.TryGetValue("etc/independentdrop", out List<KeyValuePair<string, ItemCodeHoverInfoBase>> value))
			{
				return value;
			}
			return null;
		}
		if (DIC.TryGetValue(fileName, out List<KeyValuePair<string, ItemCodeHoverInfoBase>> value2))
		{
			foreach (KeyValuePair<string, ItemCodeHoverInfoBase> item in value2)
			{
				if (item.Key == sectionName)
				{
					list.Add(item);
				}
			}
			return list;
		}
		foreach (KeyValuePair<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>> item2 in DIC)
		{
			if (!LikeOperator.LikeString(fileName, item2.Key, CompareMethod.Binary))
			{
				continue;
			}
			foreach (KeyValuePair<string, ItemCodeHoverInfoBase> item3 in item2.Value)
			{
				if (item3.Key == sectionName)
				{
					list.Add(item3);
				}
			}
			return list;
		}
		return null;
	}

	public string ModelToXml(Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>> source)
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
		xmlDocument.AppendChild(newChild);
		XmlElement xmlElement = xmlDocument.CreateElement("", "root", "");
		xmlDocument.AppendChild(xmlElement);
		foreach (KeyValuePair<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>> item in source)
		{
			XmlElement xmlElement2 = xmlDocument.CreateElement("File");
			xmlElement2.SetAttribute("FileName", item.Key);
			foreach (KeyValuePair<string, ItemCodeHoverInfoBase> item2 in item.Value)
			{
				item2.Value.CreateXmlElement(item2, xmlDocument, xmlElement2);
			}
			xmlElement.AppendChild(xmlElement2);
		}
		return UXMLFormat.ConvertXmlDocumentTostring(xmlDocument);
	}

	public bool XmlToModel(bool showErrDialog = false)
	{
		try
		{
			if (!File.Exists(SavePath))
			{
				File.WriteAllText(SavePath, Resource1.ItemCodeHoverConfig);
			}
			if (!File.Exists(SavePath))
			{
				AppSetting.Instance.GetIlogger().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_LoadCodeIntelliSenseErrorFileNotExist"), SavePath));
				return false;
			}
			Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>> dictionary = new Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>>();
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = true;
			XmlDocument xmlDocument = new XmlDocument();
			XmlReader reader = XmlReader.Create(SavePath, xmlReaderSettings);
			xmlDocument.Load(reader);
			if (xmlDocument.DocumentElement == null)
			{
				return false;
			}
			XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectSingleNode("/root")?.ChildNodes;
			if (xmlNodeList == null)
			{
				return false;
			}
			foreach (XmlNode item in xmlNodeList)
			{
				string attribute = ((XmlElement)item).GetAttribute("FileName");
				if (string.IsNullOrEmpty(attribute))
				{
					continue;
				}
				if (!dictionary.TryGetValue(attribute, out var value))
				{
					dictionary.Add(attribute, new List<KeyValuePair<string, ItemCodeHoverInfoBase>>());
					value = dictionary[attribute];
				}
				foreach (XmlElement childNode in item.ChildNodes)
				{
					string attribute2 = childNode.GetAttribute("SectionName");
					string attribute3 = childNode.GetAttribute("LstFileName");
					string attribute4 = childNode.GetAttribute("Description");
					string attribute5 = childNode.GetAttribute("ParentSectionName");
					ItemCodeHoverInfoBase itemCodeHoverInfoBase = null;
					if (childNode.Name == "Section")
					{
						string attribute6 = childNode.GetAttribute("Index");
						string attribute7 = childNode.GetAttribute("IgnoreItemCode");
						itemCodeHoverInfoBase = new ItemCodeHoverInfoDefault
						{
							Description = attribute4,
							IgnoreItemCodeList = ParseIntegerList(attribute7),
							ParentSectionName = attribute5,
							IndexList = ParseIntegerList(attribute6),
							LstFileNames = ParseFileNames(attribute3)
						};
					}
					else if (childNode.Name == "SectionGroup")
					{
						ItemCodeHoverInfoGroup itemCodeHoverInfoGroup = new ItemCodeHoverInfoGroup
						{
							ChildNodes = new Dictionary<int, ItemCodeHoverInfoBase>(),
							ParentSectionName = attribute5
						};
						foreach (XmlNode childNode2 in childNode.ChildNodes)
						{
							if (childNode2.Name == "Index")
							{
								attribute3 = ((XmlElement)childNode2).GetAttribute("LstFileName");
								attribute4 = ((XmlElement)childNode2).GetAttribute("Description");
								string attribute8 = ((XmlElement)childNode2).GetAttribute("IgnoreItemCode");
								string attribute9 = ((XmlElement)childNode2).GetAttribute("Value");
								if (int.TryParse(attribute9, out var result))
								{
									itemCodeHoverInfoGroup.ChildNodes.Add(result, new ItemCodeHoverInfoDefault
									{
										Description = attribute4,
										IndexList = ParseIntegerList(attribute9),
										LstFileNames = ParseFileNames(attribute3),
										IgnoreItemCodeList = ParseIntegerList(attribute8)
									});
								}
							}
						}
						itemCodeHoverInfoBase = itemCodeHoverInfoGroup;
					}
					else if (childNode.Name == "SectionRange")
					{
						string attribute10 = childNode.GetAttribute("IgnoreItemCode");
						int result2;
						int startIndex = (int.TryParse(childNode.GetAttribute("StartIndex"), out result2) ? result2 : (-1));
						itemCodeHoverInfoBase = new ItemCodeHoverInfoRange
						{
							Description = attribute4,
							IgnoreItemCodeList = ParseIntegerList(attribute10),
							ParentSectionName = attribute5,
							StartIndex = startIndex,
							LstFileNames = ParseFileNames(attribute3)
						};
					}
					if (itemCodeHoverInfoBase != null)
					{
						LoadValidationSections(itemCodeHoverInfoBase, childNode.ChildNodes);
						value.Add(new KeyValuePair<string, ItemCodeHoverInfoBase>(attribute2, itemCodeHoverInfoBase));
					}
				}
			}
			DIC = dictionary;
			return true;
		}
		catch (Exception ex)
		{
			string msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_LoadCodeIntelliSenseError"), ex.Message);
			if (showErrDialog)
			{
				AppSetting.Instance.GetIlogger().ShowMsg(msg, isError: true);
			}
			AppSetting.Instance.GetIlogger().Error(msg);
			return false;
		}
	}

	private static void LoadValidationSections(ItemCodeHoverInfoBase hoverInfo, XmlNodeList childNodes)
	{
		if (childNodes == null)
		{
			return;
		}
		foreach (XmlElement childNode in childNodes)
		{
			if (childNode.Name == "ValidationSection")
			{
				if (hoverInfo.ValidationSectionList == null)
				{
					hoverInfo.ValidationSectionList = new List<KeyValuePair<string, ValidationSectionData>>();
				}
				string attribute = childNode.GetAttribute("Index");
				int? index = null;
				if (!string.IsNullOrEmpty(attribute))
				{
					index = (int.TryParse(attribute, out var result) ? new int?(result) : ((int?)null));
				}
				string attribute2 = childNode.GetAttribute("CurrentLine");
				bool? currentLine = null;
				if (bool.TryParse(attribute2, out var result2))
				{
					currentLine = result2;
				}
				KeyValuePair<string, ValidationSectionData> item = new KeyValuePair<string, ValidationSectionData>(childNode.GetAttribute("Name"), new ValidationSectionData
				{
					Value = childNode.GetAttribute("Value"),
					Index = index,
					CurrentLine = currentLine
				});
				hoverInfo.ValidationSectionList.Add(item);
			}
		}
	}

	private static List<string> ParseFileNames(string value)
	{
		return value.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
	}

	private static List<int>? ParseIntegerList(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		HashSet<int> hashSet = new HashSet<int>();
		string[] array = value.Split(",", StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			if (int.TryParse(array[i], out var result))
			{
				hashSet.Add(result);
			}
		}
		if (hashSet.Count != 0)
		{
			return hashSet.ToList();
		}
		return null;
	}
}
