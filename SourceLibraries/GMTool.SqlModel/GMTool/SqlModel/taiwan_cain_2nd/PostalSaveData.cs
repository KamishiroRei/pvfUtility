using GMTool.SqlModel.Enums;
using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

public class PostalSaveData : Postal
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int Pid { get; set; }

	public bool Best { get; set; }

	public PostalType PostalType { get; set; }
}
