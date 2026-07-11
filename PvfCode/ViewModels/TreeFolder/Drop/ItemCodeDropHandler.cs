using System.Collections.Generic;
using System.Text;
using Collections.Pooled;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.ViewModels.DocumentFolder;

namespace PvfCode.ViewModels.TreeFolder.Drop;

internal class ItemCodeDropHandler : DropDocumentModelBase
{
	public override void Drop(PooledSet<string> fileList, TextEditorBase editor, PvfFileDocument viewModel)
	{
		if (fileList == null || fileList.Count == 0)
		{
			AppCore.Logger.Warning(AppSetting.Instance.GetIlogger()?.GetStr("mess_DeliverFail_NoFile"));
			return;
		}

		List<PvfFile> files = AppCore.ViewModelBase.PVF.GetFiles(fileList);
		if (files == null || files.Count == 0)
		{
			AppCore.Logger.Warning(AppSetting.Instance.GetIlogger()?.GetStr("mess_DeliverFail_NoFile"));
			return;
		}

		PooledList<int> itemCodes = new PooledList<int>();
		foreach (PvfFile file in files)
		{
			if (file.ItemCode.HasValue)
			{
				itemCodes.Add(file.ItemCode.Value);
			}
		}
		if (itemCodes.Count == 0)
		{
			return;
		}

		StringBuilder output = new StringBuilder();
		int codesOnLine = 0;
		foreach (int itemCode in itemCodes)
		{
			codesOnLine++;
			if (codesOnLine == 15)
			{
				output.Append("\r\n");
				codesOnLine = 0;
			}
			output.Append(itemCode + "\t");
		}
		DocumentAppend(output, editor);
	}
}
