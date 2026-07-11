namespace PvfCode.Services.PvfParsingNew;

public class WebApiFileRootSectionData
{
	public string? SectionName { get; set; }

	public string? EndSectionName
	{
		get
		{
			if (!HasEndSection || string.IsNullOrEmpty(SectionName))
			{
				return null;
			}
			return SectionName.Insert(1, "/");
		}
	}

	public ScriptType DataType { get; set; }

	public bool IsSection => DataType == ScriptType.Section;

	public bool HasEndSection { get; set; }

	public WebApiFileRootSectionData()
	{
	}
}
