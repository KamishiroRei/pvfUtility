using System.Text;
using Collections.Pooled;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.ViewModels.DocumentFolder;

namespace PvfCode.ViewModels.TreeFolder.Drop;

public abstract class DropDocumentModelBase
{
	public abstract void Drop(PooledSet<string> fileList, TextEditorBase editor, PvfFileDocument vm);

	public virtual void DocumentAppend(StringBuilder bui, TextEditorBase editor)
	{
		if (editor.Document.GetLineByOffset(editor.CaretOffset).Length > 0)
		{
			bui.Insert(0, "\r\n");
		}
		bui.Append("\r\n");
		editor.Document.Insert(editor.CaretOffset, bui.ToString());
	}

	protected DropDocumentModelBase()
	{
	}
}
