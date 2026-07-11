using System.Collections.Generic;

namespace PvfCode.Services.PvfParsingNew;

public class WebApiFileData
{
	private List<WebApiFileData> children;

	public string? SectionName { get; set; }

	public bool IsSection => DataType == ScriptType.Section;

	public bool HasEndSection { get; set; }

	public ScriptType DataType { get; set; }

	public object Value { get; set; }

	public List<WebApiFileData> Children
	{
		get
		{
			if (children == null)
			{
				children = new List<WebApiFileData>();
			}
			return children;
		}
		set
		{
			children = value;
		}
	}

	public WebApiFileData()
	{
	}
}
