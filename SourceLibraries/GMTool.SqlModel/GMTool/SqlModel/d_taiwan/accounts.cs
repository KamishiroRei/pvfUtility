using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("d_taiwan.accounts")]
public class accounts
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public virtual int UID { get; set; }

	public virtual string accountname { get; set; }

	public virtual string password { get; set; }
}
