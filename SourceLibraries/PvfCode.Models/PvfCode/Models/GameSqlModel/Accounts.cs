using SqlSugar;

namespace PvfCode.Models.GameSqlModel;

[SugarTable("d_taiwan.accounts")]
public class Accounts
{
	public int UID { get; set; }

	[SugarColumn(ColumnName = "accountname")]
	public string UserName { get; set; }

	[SugarColumn(ColumnName = "password")]
	public string Password { get; set; }
}
