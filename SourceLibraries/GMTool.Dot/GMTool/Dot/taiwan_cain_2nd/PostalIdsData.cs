using SqlSugar;

namespace GMTool.Dot.taiwan_cain_2nd;

public class PostalIdsData
{
	public int AvatarId { get; set; }

	public int CreateId { get; set; }

	public int LetterId { get; set; }

	public SqlSugarClient LetterDb { get; set; }
}
