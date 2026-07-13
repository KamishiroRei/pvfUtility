using System.Collections.Generic;
using System.Text;
using PvfCode.Dot;
using PvfCode.Models.Pvf;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.independent_drop.DropList;

public class DropList_independentdrop : DropListBase
{
	private readonly Dictionary<int, KeyValuePair<List<ListItem>, LstItem>> listFiles;

	public string FilePath { get; set; }

	public int LstId
	{
		get
		{
			return GetProperty(() => LstId);
		}
		set
		{
			SetProperty(() => LstId, value, RefreshListItems);
		}
	}

	private void RefreshListItems()
	{
		if (listFiles != null)
		{
			if (listFiles.TryGetValue(LstId, out KeyValuePair<List<ListItem>, LstItem> value))
			{
				base.Items = new ConcurrentObservableCollection<ListItem>();
				base.Items.AddRange(value.Key);
				FilePath = value.Value.FullPath;
			}
			else
			{
				base.Items = null;
			}
			RaisePropertyChanged("Items");
		}
	}

	public DropList_independentdrop(int lstId, IList<ListItem> listItems, string filePath, Dictionary<int, KeyValuePair<List<ListItem>, LstItem>> dic)
	{
		base.Items = new ConcurrentObservableCollection<ListItem>();
		base.Items.AddRange(listItems);
		FilePath = filePath;
		LstId = lstId;
		listFiles = dic;
	}

	public override ResultData<string> ToText()
	{
		ResultData<string> resultData = new ResultData<string>();
		ResultData resultData2 = SaveDropList();
		if (resultData2.IsError)
		{
			resultData.Msg = resultData2.Msg;
		}
		resultData.Data = LstId.ToString();
		if (!listFiles.ContainsKey(LstId))
		{
			resultData.Msg = resultData.Msg + "|" + string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_LstIdNotExist"), LstId);
		}
		return resultData;
	}

	private ResultData SaveDropList()
	{
		ResultData resultData = new ResultData();
		StringBuilder stringBuilder = new StringBuilder("#PVF_File\r\n[list]\r\n");
		foreach (ListItem item in base.Items)
		{
			stringBuilder.AppendLine($"{item.ItemCode}\t{item.DropWeight}");
		}
		stringBuilder.AppendLine("[/list]");
		if (AppCore.ViewModelBase.PVF.FileAny(FilePath))
		{
			AppCore.ViewModelBase.PVF.SaveFileText(FilePath, stringBuilder.ToString());
		}
		else
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_DropFileNotExist"), FilePath);
		}
		return resultData;
	}
}
