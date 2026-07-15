using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using PvfCode.Dot;
using PvfCode.Models.GameSqlModel;
using SqlSugar;

namespace PvfCode.Services.GMTool;

public class GmtoolService : Repository<Accounts>
{
	public async Task<ResultData<Accounts>> GameLogin(string userName, string password)
	{
		ResultData<Accounts> re = new ResultData<Accounts>();
		try
		{
			Accounts accounts = await FindAsync(item => item.UserName == userName && item.Password == Utools.PwdHelper.GetMD5(password));
			if (accounts == null)
			{
				re.Msg = "账号或密码不正确";
			}
			else
			{
				re.Data = accounts;
			}
		}
		catch (Exception ex)
		{
			re.Msg = "数据库错误：" + ex.Message.Replace("中文提示 :  连接数据库过程中发生错误，检查服务器是否正常连接字符串是否正确，错误信息：", "");
		}
		return re;
	}

	public List<Accounts> Test()
	{
		return base.Db.Queryable<Accounts>().ToList();
	}

	public GmtoolService()
		: base((ISqlSugarClient)null)
	{
	}
}
