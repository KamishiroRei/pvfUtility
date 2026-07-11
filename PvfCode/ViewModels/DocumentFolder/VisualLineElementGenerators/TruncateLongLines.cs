using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace PvfCode.ViewModels.DocumentFolder.VisualLineElementGenerators;

public class TruncateLongLines : VisualLineElementGenerator
{
	private readonly string wMS5X0LbNp;

	private int GPu5p65rl0;

	public TruncateLongLines()
	{
		GPu5p65rl0 = 100;
		wMS5X0LbNp = AppSetting.Instance.GetIlogger().GetStr("TruncateLongLines_ellipsis");
	}

	public override int GetFirstInterestedOffset(int startOffset)
	{
		DocumentLine lastDocumentLine = base.CurrentContext.VisualLine.LastDocumentLine;
		if (base.CurrentContext.TextView.GetLineIsCollapsed(lastDocumentLine.LineNumber))
		{
			return -1;
		}
		if (lastDocumentLine.Length > AppSetting.Instance.EditConfig.TruncateLongLineLength)
		{
			int num = lastDocumentLine.Offset + AppSetting.Instance.EditConfig.TruncateLongLineLength - GPu5p65rl0 - wMS5X0LbNp.Length;
			if (startOffset <= num)
			{
				return num;
			}
		}
		return -1;
	}

	public override VisualLineElement ConstructElement(int offset)
	{
		if (base.CurrentContext.TextView.GetLineIsCollapsed(base.CurrentContext.VisualLine.LastDocumentLine.LineNumber))
		{
			return null;
		}
		return new FormattedTextElement(wMS5X0LbNp, base.CurrentContext.VisualLine.LastDocumentLine.EndOffset - offset - GPu5p65rl0);
	}
}
