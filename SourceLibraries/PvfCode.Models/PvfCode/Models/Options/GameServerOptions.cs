using System;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using SqlSugar;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class GameServerOptions : ViewModelBase
{
	private string ip;

	private string sqlUserName;

	private string sqlPassword;

	private int? sqlPort;

	public string IP
	{
		get
		{
			if (string.IsNullOrEmpty(ip))
			{
				ip = "192.168.200.131";
			}
			return ip;
		}
		set
		{
			ip = value;
			RaisePropertyChanged(nameof(IP));
		}
	}

	public string SqlUserName
	{
		get
		{
			if (string.IsNullOrEmpty(sqlUserName))
			{
				sqlUserName = "game";
			}
			return sqlUserName;
		}
		set
		{
			sqlUserName = value;
			RaisePropertyChanged(nameof(SqlUserName));
		}
	}

	public string SqlPassword
	{
		get
		{
			if (string.IsNullOrEmpty(sqlPassword))
			{
				sqlPassword = "uu5!^%jg";
			}
			return sqlPassword;
		}
		set
		{
			sqlPassword = value;
			RaisePropertyChanged(nameof(SqlPassword));
		}
	}

	public int SqlPort
	{
		get
		{
			if (!sqlPort.HasValue)
			{
				sqlPort = 3306;
			}
			return sqlPort.Value;
		}
		set
		{
			sqlPort = value;
			RaisePropertyChanged(nameof(SqlPort));
		}
	}

	public string PemPrivateKey
	{
		get
		{
			return GetProperty(() => PemPrivateKey);
		}
		set
		{
			SetProperty<string>(() => PemPrivateKey, value);
		}
	}

	public string PemPublicKey { get; set; }

	public ISqlSugarClient GetDb()
	{
		try
		{
			GameServerOptions gameServerOptions = AppSetting.Instance.GameOptions.GameServerOptions;
			string connectionString = $"Server={gameServerOptions.IP};Port=3306;Database=d_taiwan;Uid={gameServerOptions.SqlUserName};Pwd={gameServerOptions.SqlPassword};Charset=utf8;Convert Zero Datetime=True;Allow Zero Datetime=True;SslMode=none;";
			SqlSugarClient sqlSugarClient = new SqlSugarClient(new ConnectionConfig
			{
				DbType = DbType.MySql,
				InitKeyType = InitKeyType.Attribute,
				IsAutoCloseConnection = true,
				ConnectionString = connectionString
			});
			sqlSugarClient.Aop.OnLogExecuting = delegate(string s, SugarParameter[] p)
			{
				Console.WriteLine(s);
				Console.WriteLine(p);
			};
			return sqlSugarClient;
		}
		catch (Exception)
		{
			AppSetting.Instance.GetIlogger()?.ShowMsg("游戏数据库链接失败", isError: true);
		}
		return null;
	}

}
