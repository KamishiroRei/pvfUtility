using System;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using SqlSugar;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class GameServerOptions : ViewModelBase
{
	private string n53LtsHQ4A;

	private string YnwL9HIgVF;

	private string ArtLRru9YM;

	private int? sbOLWbS5ao;

	[CompilerGenerated]
	private string K7bLq4OudN;

	public string IP
	{
		get
		{
			if (string.IsNullOrEmpty(n53LtsHQ4A))
			{
				n53LtsHQ4A = "192.168.200.131";
			}
			return n53LtsHQ4A;
		}
		set
		{
			n53LtsHQ4A = value;
			RaisePropertyChanged("IP");
		}
	}

	public string SqlUserName
	{
		get
		{
			if (string.IsNullOrEmpty(YnwL9HIgVF))
			{
				YnwL9HIgVF = "game";
			}
			return YnwL9HIgVF;
		}
		set
		{
			YnwL9HIgVF = value;
			RaisePropertyChanged("SqlUserName");
		}
	}

	public string SqlPassword
	{
		get
		{
			if (string.IsNullOrEmpty(ArtLRru9YM))
			{
				ArtLRru9YM = "uu5!^%jg";
			}
			return ArtLRru9YM;
		}
		set
		{
			ArtLRru9YM = value;
			RaisePropertyChanged("SqlPassword");
		}
	}

	public int SqlPort
	{
		get
		{
			if (!sbOLWbS5ao.HasValue)
			{
				sbOLWbS5ao = 3306;
			}
			return sbOLWbS5ao.Value;
		}
		set
		{
			sbOLWbS5ao = value;
			RaisePropertyChanged("SqlPort");
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

	public string PemPublicKey
	{
		[CompilerGenerated]
		get
		{
			return K7bLq4OudN;
		}
		[CompilerGenerated]
		set
		{
			K7bLq4OudN = value;
		}
	}

	public ISqlSugarClient GetDb()
	{
		try
		{
			GameServerOptions gameServerOptions = AppSetting.Instance.GameOptions.GameServerOptions;
			string text = "Server=";
			string iP = gameServerOptions.IP;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(117, 2);
			defaultInterpolatedStringHandler.AppendLiteral(";Port=3306;Database=d_taiwan;Uid=");
			defaultInterpolatedStringHandler.AppendFormatted(gameServerOptions.SqlUserName);
			defaultInterpolatedStringHandler.AppendLiteral(";Pwd=");
			defaultInterpolatedStringHandler.AppendFormatted(gameServerOptions.SqlPassword);
			defaultInterpolatedStringHandler.AppendLiteral(";Charset=utf8;Convert Zero Datetime=True;Allow Zero Datetime=True;SslMode=none;");
			string connectionString = text + iP + defaultInterpolatedStringHandler.ToStringAndClear();
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

	public GameServerOptions()
	{
	}
}
