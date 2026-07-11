using System.Runtime.CompilerServices;
using SqlSugar;

namespace PvfCode.Models.GameSqlModel;

[SugarTable("d_taiwan.accounts")]
public class Accounts
{
	[CompilerGenerated]
	private int vQmZPWxQYN;

	[CompilerGenerated]
	private string gfDZFLZx2H;

	[CompilerGenerated]
	private string l8JZXdT1cZ;

	public int UID
	{
		[CompilerGenerated]
		get
		{
			return vQmZPWxQYN;
		}
		[CompilerGenerated]
		set
		{
			vQmZPWxQYN = value;
		}
	}

	[SugarColumn(ColumnName = "accountname")]
	public string UserName
	{
		[CompilerGenerated]
		get
		{
			return gfDZFLZx2H;
		}
		[CompilerGenerated]
		set
		{
			gfDZFLZx2H = value;
		}
	}

	[SugarColumn(ColumnName = "password")]
	public string Password
	{
		[CompilerGenerated]
		get
		{
			return l8JZXdT1cZ;
		}
		[CompilerGenerated]
		set
		{
			l8JZXdT1cZ = value;
		}
	}

	public Accounts()
	{
	}
}
