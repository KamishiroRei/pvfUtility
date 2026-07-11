using PvfCode.Dot.Desktop.Enums;
using PvfCode.Dot.Desktop.interfaces;

namespace PvfCode.Dot.Desktop;

public class CodeCompletionDataDto : ICodeCompletionData
{
	public string Text { get; set; }

	public string Description { get; set; }

	public double Priority { get; set; }

	public bool HaveEndSection { get; set; }

	public HighlightingType HighlightingType { get; set; }

	public string CompleteText { get; set; }

	public CodeCompletScriptType CodeCompletScriptType { get; set; }

	public PvfFileType PvfFileType { get; set; }

	public string NickNames { get; set; }
}
