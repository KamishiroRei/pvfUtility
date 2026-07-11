using PvfCode.Dot.Desktop.Enums;

namespace PvfCode.Dot.Desktop.interfaces;

public interface ICodeCompletionData
{
	string Text { get; set; }

	string Description { get; set; }

	double Priority { get; set; }

	bool HaveEndSection { get; set; }

	HighlightingType HighlightingType { get; set; }

	string CompleteText { get; set; }

	CodeCompletScriptType CodeCompletScriptType { get; set; }

	PvfFileType PvfFileType { get; set; }

	string NickNames { get; set; }
}
