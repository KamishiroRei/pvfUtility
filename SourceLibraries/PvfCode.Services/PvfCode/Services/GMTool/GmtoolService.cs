using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PvfCode.Dot;
using PvfCode.Models.GameSqlModel;
using SqlSugar;

namespace PvfCode.Services.GMTool;

public class GmtoolService : Repository<Accounts>
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_0
	{
		public string zJ4Hn0783p;

		public string qMIHR1l6nq;

		public _003C_003Ec__DisplayClass0_0()
		{
		}
	}

	public async Task<ResultData<Accounts>> GameLogin(string userName, string password)
	{
		_003C_003Ec__DisplayClass0_0 _003C_003Ec__DisplayClass0_1 = new _003C_003Ec__DisplayClass0_0();
		_003C_003Ec__DisplayClass0_1.zJ4Hn0783p = userName;
		_003C_003Ec__DisplayClass0_1.qMIHR1l6nq = password;
		ResultData<Accounts> re = new ResultData<Accounts>();
		try
		{
			GmtoolService gmtoolService = this;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(Accounts), "it");
			Accounts accounts = await gmtoolService.FindAsync(item => item.UserName == _003C_003Ec__DisplayClass0_1.zJ4Hn0783p && item.Password == Utools.PwdHelper.GetMD5(_003C_003Ec__DisplayClass0_1.qMIHR1l6nq));
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
