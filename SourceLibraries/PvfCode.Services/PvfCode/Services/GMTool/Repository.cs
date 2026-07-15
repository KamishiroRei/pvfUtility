using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PvfCode.Models.GameSqlModel;
using PvfCode.Models.Options;
using SqlSugar;

namespace PvfCode.Services.GMTool;

public class Repository<T> : SimpleClient<T> where T : class, new()
{
	public ISqlSugarClient Db => base.Context;

	public Repository(ISqlSugarClient context = null)
		: base(context)
	{
		if (context != null)
		{
			return;
		}
		try
		{
			GameServerOptions gameServerOptions = AppSetting.Instance.GameOptions.GameServerOptions;
			string connectionString = $"Server={gameServerOptions.IP};Port=3306;Database=d_taiwan;Uid={gameServerOptions.SqlUserName};Pwd={gameServerOptions.SqlPassword};Charset=utf8;Convert Zero Datetime=True;Allow Zero Datetime=True;SslMode=none;";
			base.Context = new SqlSugarClient(new ConnectionConfig
			{
				DbType = DbType.MySql,
				InitKeyType = InitKeyType.Attribute,
				IsAutoCloseConnection = true,
				ConnectionString = connectionString
			});
			base.Context.Aop.OnLogExecuting = delegate(string s, SugarParameter[] p)
			{
				Console.WriteLine(s);
				Console.WriteLine(p);
			};
		}
		catch (Exception)
		{
			AppSetting.Instance.GetIlogger()?.ShowMsg("游戏数据库链接失败", isError: true);
		}
	}

	public string CreateTables()
	{
		try
		{
			_ = Db;
			return "ok";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public async Task<bool> Adds(List<T> list)
	{
		return await base.Context.Insertable(list).ExecuteCommandAsync() > 0;
	}

	public async Task<bool> Add(T dto)
	{
		return await base.Context.Insertable(dto).ExecuteCommandIdentityIntoEntityAsync();
	}

	public async Task<int> AddsReturnNumber(List<T> list)
	{
		return await base.Context.Insertable(list).ExecuteCommandAsync();
	}

	public async Task<List<T>> GetListNotNull()
	{
		return (await base.GetListAsync()) ?? new List<T>();
	}

	public async Task<List<T>> GetListNotNull(Expression<Func<T, bool>> func)
	{
		return (await base.Context.Queryable<T>().Where(func).ToListAsync()) ?? new List<T>();
	}

	public T AddReturnEntity(T t)
	{
		return base.Context.Insertable(t).ExecuteReturnEntity();
	}

	public async Task<T> AddReturnEntityAsync(T t)
	{
		return await base.Context.Insertable(t).ExecuteReturnEntityAsync();
	}

	public async Task<T> FindAsync(Expression<Func<T, bool>> func)
	{
		return await base.Context.Queryable<T>().Where(func).FirstAsync();
	}

	public T Find(Expression<Func<T, bool>> func)
	{
		return base.Context.Queryable<T>().Where(func).First();
	}

	public override async Task<bool> DeleteAsync(Expression<Func<T, bool>> func)
	{
		return await base.Context.Deleteable(func).ExecuteCommandAsync() > 0;
	}

	public override async Task<bool> DeleteAsync(T t)
	{
		return await base.Context.Deleteable(t).ExecuteCommandAsync() == 0;
	}

	public async Task<bool> DeletesAsync(List<T> ts)
	{
		return await base.Context.Deleteable(ts).ExecuteCommandHasChangeAsync();
	}

	public async Task<List<T>> QueryAsnyc(Expression<Func<T, bool>> func)
	{
		return (await base.GetListAsync(func)) ?? new List<T>();
	}

	public async Task<List<T>> ToListAsync()
	{
		return await base.Context.Queryable<T>().ToListAsync();
	}

	public async Task<List<T>> QueryPageAsync(Expression<Func<T, bool>> func, int pageIndex, int pageSize, RefAsync<int> total)
	{
		List<T> list = await base.Context.Queryable<T>().Where(func).ToPageListAsync(pageIndex, pageSize, total);
		if (list == null)
		{
			list = new List<T>();
		}
		return list;
	}

	public async Task<bool> DeleteAsync(Expression<Func<T, bool>> func, List<T> list)
	{
		return await base.Context.Deleteable(list).Where(func).ExecuteCommandAsync() > 0;
	}

	public async Task<bool> UpdateAsync(Expression<Func<T, bool>> func)
	{
		return await base.Context.Updateable(func).ExecuteCommandAsync() > 0;
	}

	public async Task<bool> AnyAsync(Expression<Func<T, bool>> func)
	{
		return await base.Context.Queryable<T>().Where(func).AnyAsync();
	}

	public async Task<Accounts> UserIdToUser(int uid)
	{
		return await base.Context.Queryable<Accounts>().Where(item => item.UID == uid).FirstAsync();
	}
}
