using System.Collections.Generic;
namespace PvfCode.Services.PvfFileServices;

public class PvfFileSection : PvfFileSectionBase
{
	public string SectionName { get; set; }

	public bool HasEnding { get; set; }

	public Dictionary<ScriptType, PvfFileSection> Items { get; set; }

	public PvfFileSection()
	{
	}
}
