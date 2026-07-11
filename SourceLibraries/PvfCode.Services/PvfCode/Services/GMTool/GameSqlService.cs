using GMTool.SqlModel.d_taiwan;
using SqlSugar;

namespace PvfCode.Services.GMTool;

public class GameSqlService : Repository<accounts>
{
	public GameSqlService()
		: base((ISqlSugarClient)null)
	{
	}
}
