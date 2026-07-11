using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("guild_files")]
public class guild_files
{
	[SugarColumn(IsPrimaryKey = true)]
	public int gno { get; set; }

	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public byte gf_no { get; set; }

	public string file_server { get; set; }

	public string file_location { get; set; }
}
