namespace PvfCode.Services.PvfFileModel.etc;

public class TitleCollectionInfoItem
{
	private PvfGroup pvf;

	public int Index { get; set; }

	public bool Open { get; set; }

	public int QstCode { get; set; }

	public int Value3 { get; set; }

	public int TitleEquCode { get; set; }

	public int QstMinLevel
	{
		get
		{
			if (!Open)
			{
				return -1;
			}
			string qstFilePath = QstFilePath;
			if (qstFilePath == null)
			{
				return -1;
			}
			if (pvf == null || !pvf.GetFile(qstFilePath).GetSectionIntValue("[level]", pvf, out var val))
			{
				return -1;
			}
			return val;
		}
	}

	public string? QstFilePath
	{
		get
		{
			if (!Open)
			{
				return null;
			}
			return pvf?.ListFileTable.ItemCodeConvertFilePath("quest", QstCode);
		}
	}

	public TitleCollectionInfoItem()
	{
	}

	public TitleCollectionInfoItem(PvfGroup pvf)
	{
		this.pvf = pvf;
	}
}
