using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GMTool.Dot;
using GMTool.Dot.QueryModel;
using GMTool.Dot.QueryModel.Enums;
using GMTool.SqlModel.d_taiwan;
using GMTool.SqlModel.taiwan_cain;
using GMTool.SqlModel.taiwan_game_event;
using GMTool.SqlModel.taiwan_login;
using SqlSugar;
using SqlSugar.Extensions;

namespace GMTool.Services;

public class Repository<T> : SimpleClient<T> where T : class, new()
{
	public ISqlSugarClient Db => base.Context;

	public Repository(ISqlSugarClient context)
		: base(context)
	{
	}

	public SqlSugarClient NewDb()
	{
		SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig
		{
			DbType = DbType.MySql,
			InitKeyType = InitKeyType.Attribute,
			IsAutoCloseConnection = true,
			ConnectionString = Db.CurrentConnectionConfig.ConnectionString
		});
		sqlSugarClient.Aop.OnLogExecuting = delegate(string s, SugarParameter[] p)
		{
			Console.WriteLine(s);
			Console.WriteLine(p);
		};
		return sqlSugarClient;
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

	public async Task<List<AccountDto>> FindUserFromUserName(FindUserDto dto)
	{
		Db.Ado.ExecuteCommand("set names latin1;");
		ISugarQueryable<accounts, member_info, login_account_3> sugarQueryable = Db.Queryable<accounts>().LeftJoin((accounts A, member_info B) => B.m_id == A.UID).LeftJoin((accounts A, member_info B, login_account_3 C) => C.m_id == A.UID);
		switch (dto.FindUserType)
		{
		case FindUserType.UserName:
			sugarQueryable = ((!dto.WholeWordMatch) ? ((!dto.IsStartMatch) ? sugarQueryable.Where((accounts A) => A.accountname.Contains(dto.Keyword)) : sugarQueryable.Where((accounts A) => A.accountname.StartsWith(dto.Keyword))) : sugarQueryable.Where((accounts A) => A.accountname == dto.Keyword));
			break;
		case FindUserType.UID:
			sugarQueryable = ((!dto.WholeWordMatch) ? ((!dto.IsStartMatch) ? sugarQueryable.Where((accounts A) => A.UID.ToString().Contains(dto.Keyword)) : sugarQueryable.Where((accounts A) => A.UID.ToString().StartsWith(dto.Keyword))) : sugarQueryable.Where((accounts A) => A.UID == dto.Keyword.ObjToInt()));
			break;
		case FindUserType.IP:
			sugarQueryable = ((!dto.WholeWordMatch) ? ((!dto.IsStartMatch) ? sugarQueryable.Where((accounts A, member_info B, login_account_3 C) => C.login_ip.ToString().Contains(dto.Keyword)) : sugarQueryable.Where((accounts A, member_info B, login_account_3 C) => C.login_ip.ToString().StartsWith(dto.Keyword))) : sugarQueryable.Where((accounts A, member_info B, login_account_3 C) => C.login_ip == dto.Keyword));
			break;
		case FindUserType.RegTime:
			sugarQueryable = sugarQueryable.Where((accounts A, member_info B) => B.updt_date.Day == DateTime.Now.Day);
			break;
		case FindUserType.OnLineUser:
			sugarQueryable = sugarQueryable.Where((accounts A, member_info B, login_account_3 C) => C.login_status == 1).WhereIF(dto.FilterDummies, (accounts A, member_info B, login_account_3 C) => C.login_ip != "10.0.0.1");
			break;
		}
		List<AccountDto> list = await sugarQueryable.Select((accounts A, member_info B, login_account_3 C) => new AccountDto
		{
			login_ip = C.login_ip,
			accountname = A.accountname,
			login_status = (C.login_status == 1),
			password = A.password,
			UID = A.UID,
			updt_date = B.updt_date
		}).ToListAsync();
		if (list == null)
		{
			list = new List<AccountDto>();
		}
		return list;
	}

	public async Task<List<CharacInfoDto>> FindCharac(FindUserDto dto)
	{
		Db.Ado.ExecuteCommand("set names latin1;");
		ISugarQueryable<charac_info, login_account_3, event_1306_account_reward, accounts, charac_stat> sugarQueryable = Db.Queryable<charac_info>().LeftJoin((charac_info A, login_account_3 B) => B.m_id == A.m_id && B.login_status == 1).LeftJoin((charac_info A, login_account_3 B, event_1306_account_reward C) => B.m_id == C.m_id && A.charac_no == C.charac_no)
			.LeftJoin((charac_info A, login_account_3 B, event_1306_account_reward C, accounts D) => D.UID == A.m_id)
			.LeftJoin((charac_info A, login_account_3 B, event_1306_account_reward C, accounts D, charac_stat E) => E.charac_no == A.charac_no);
		switch (dto.FindUserType)
		{
		case FindUserType.CharacName:
			sugarQueryable = ((!dto.WholeWordMatch) ? ((!dto.IsStartMatch) ? sugarQueryable.Where((charac_info A) => A.charac_name.Contains(dto.Keyword)) : sugarQueryable.Where((charac_info A) => A.charac_name.StartsWith(dto.Keyword))) : sugarQueryable.Where((charac_info A) => A.charac_name == dto.Keyword));
			break;
		case FindUserType.CharacId:
			sugarQueryable = ((!dto.WholeWordMatch) ? ((!dto.IsStartMatch) ? sugarQueryable.Where((charac_info A) => A.charac_no.ToString().Contains(dto.Keyword)) : sugarQueryable.Where((charac_info A) => A.charac_no.ToString().StartsWith(dto.Keyword))) : sugarQueryable.Where((charac_info A) => A.charac_no == dto.Keyword.ObjToInt()));
			break;
		case FindUserType.OnLineCharac:
			sugarQueryable = sugarQueryable.Where((charac_info A, login_account_3 B, event_1306_account_reward C) => B.login_status == 1 && C.charac_no == A.charac_no).WhereIF(dto.FilterDummies, (charac_info A, login_account_3 B, event_1306_account_reward C) => B.login_ip != "10.0.0.1");
			break;
		case FindUserType.UidFindCharacs:
			sugarQueryable = sugarQueryable.Where((charac_info A) => A.m_id == dto.Keyword.ObjToInt());
			break;
		}
		return await sugarQueryable.Select((charac_info A, login_account_3 B, event_1306_account_reward C, accounts D, charac_stat E) => new CharacInfoDto
		{
			accountname = D.accountname,
			CharacOnLine = (C.m_id != 0),
			grow_type = A.grow_type,
			charac_name = A.charac_name,
			charac_no = A.charac_no,
			CharName = A.charac_name,
			delete_flag = (A.delete_flag == 1),
			job = A.job,
			last_play_time = E.last_play_time,
			lev = A.lev,
			UID = A.m_id,
			UserOnLine = (B.login_status == 1)
		}).ToListAsync();
	}
}
