using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using PvfCode.Dot;
using PvfCode.Models.Pvf;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.independent_drop.DropList;

public class DropList_independentdrop : DropListBase
{
	private readonly Dictionary<int, KeyValuePair<List<ListItem>, LstItem>> WLG26S15VQ;

	[CompilerGenerated]
	private string Nkb21495Iu;

	public string FilePath
	{
		[CompilerGenerated]
		get
		{
			return Nkb21495Iu;
		}
		[CompilerGenerated]
		set
		{
			Nkb21495Iu = value;
		}
	}

	public int LstId
	{
		get
		{
			return GetProperty(() => LstId);
		}
		set
		{
			SetProperty(() => LstId, value, Y1t2a6vrNZ);
		}
	}

	private void Y1t2a6vrNZ()
	{
		if (WLG26S15VQ != null)
		{
			if (WLG26S15VQ.TryGetValue(LstId, out KeyValuePair<List<ListItem>, LstItem> value))
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
		WLG26S15VQ = dic;
	}

	public override ResultData<string> ToText()
	{
		ResultData<string> resultData = new ResultData<string>();
		ResultData resultData2 = mHv2gGRAFN();
		if (resultData2.IsError)
		{
			resultData.Msg = resultData2.Msg;
		}
		resultData.Data = LstId.ToString();
		if (!WLG26S15VQ.ContainsKey(LstId))
		{
			resultData.Msg = resultData.Msg + "|" + string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_LstIdNotExist"), LstId);
		}
		return resultData;
	}

	private ResultData mHv2gGRAFN()
	{
		ResultData resultData = new ResultData();
		StringBuilder stringBuilder = new StringBuilder("#PVF_File\r\n[list]\r\n");
		foreach (ListItem item in base.Items)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
			handler.AppendFormatted(item.ItemCode);
			handler.AppendLiteral("\t");
			handler.AppendFormatted(item.DropWeight);
			stringBuilder2.AppendLine(ref handler);
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
