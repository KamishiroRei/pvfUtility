using SqlSugar;

namespace GMTool.SqlModel.taiwan_cain_2nd;

[SugarTable("dblab_avatar_socket")]
public class dblab_avatar_socket
{
	[SugarColumn(IsPrimaryKey = true)]
	public int it_id { get; set; }

	public string jewel_socket { get; set; }
}
