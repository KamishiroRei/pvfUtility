using System.Collections.Generic;

namespace PvfCode.Services.PvfFileModel.etc;

public class TitleCollectionInfo
{
	public TitleCollectionInfoHead Header { get; set; }

	public List<TitleCollectionInfoItem> Items { get; set; }

	public TitleCollectionInfo()
	{
		Items = new List<TitleCollectionInfoItem>();
		Header = new TitleCollectionInfoHead();
	}
}
