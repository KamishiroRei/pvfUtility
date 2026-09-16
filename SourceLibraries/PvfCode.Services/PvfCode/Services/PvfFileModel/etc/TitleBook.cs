using System;
using System.Collections.Generic;
using System.Linq;
using PvfCode.Services.PvfParsingNew;

namespace PvfCode.Services.PvfFileModel.etc;

public class TitleBook
{
	private readonly PvfFile file;

	private readonly PvfGroup pvf;

	private readonly List<TitleCollectionInfo> collections;

	public TitleBook(PvfFile file, PvfGroup pvf)
	{
		collections = new List<TitleCollectionInfo>();
		this.file = file;
		this.pvf = pvf;
		ParseCollections();
	}

	private void ParseCollections()
	{
		ScriptFileParserNew parser = new ScriptFileParserNew(file, pvf);
		parser.PraseStructureMain();
		foreach (SectionBase section in parser.Sections)
		{
			if (section.GetSectionName() == "[title collection info]")
			{
				ParseCollection(section);
			}
		}
	}

	private void ParseCollection(SectionBase section)
	{
		int valueCount = section.Children.Count;
		if (section.HasEndSection())
		{
			valueCount--;
		}
		int valueIndex = 0;
		bool headerParsed = false;
		TitleCollectionInfoItem currentItem = new TitleCollectionInfoItem(pvf);
		TitleCollectionInfo collection = new TitleCollectionInfo();
		for (int i = 1; i < valueCount; i++)
		{
			SectionBase valueSection = section.Children[i];
			ScriptItem nextItem = ScriptLinkText.TryGetLinkedLiteral(section.Children, i, section.Children.Count);
			string itemText = valueSection.Item.GetItemText(pvf, nextItem);
			valueIndex++;
			if (!headerParsed)
			{
				switch (valueIndex)
				{
				case 1:
					collection.Header = new TitleCollectionInfoHead
					{
						Str = itemText
					};
					break;
				case 2:
					collection.Header.Value = int.Parse(itemText);
					headerParsed = true;
					valueIndex = 0;
					break;
				}
				continue;
			}
			switch (valueIndex)
			{
			case 1:
				currentItem.Index = int.Parse(itemText);
				break;
			case 2:
				currentItem.Open = int.Parse(itemText) != -1;
				break;
			case 3:
				currentItem.QstCode = int.Parse(itemText);
				break;
			case 4:
				currentItem.Value3 = int.Parse(itemText);
				break;
			case 5:
				currentItem.TitleEquCode = int.Parse(itemText);
				break;
			default:
				throw new Exception("称号簿模型解析失败 [title collection info]中不应出现第6个数据");
			}
			if (int.TryParse(itemText, out var result) && result == -1 && valueIndex == 2)
			{
				valueIndex = 0;
				collection.Items.Add(currentItem);
				currentItem = new TitleCollectionInfoItem(pvf);
			}
			else if (valueIndex == 5)
			{
				valueIndex = 0;
				collection.Items.Add(currentItem);
				currentItem = new TitleCollectionInfoItem(pvf);
			}
		}
		collections.Add(collection);
	}

	public List<int> ExtractQstCodeList()
	{
		return (from row in collections.SelectMany((TitleCollectionInfo item) => item.Items)
			where row.Open
			select row.QstCode).ToList();
	}

	public List<int> ExtractTitleEquCodeList()
	{
		return (from row in collections.SelectMany((TitleCollectionInfo item) => item.Items)
			where row.Open
			select row.TitleEquCode).ToList();
	}
}
