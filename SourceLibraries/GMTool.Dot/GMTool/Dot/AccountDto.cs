using System;
using GMTool.SqlModel.d_taiwan;

namespace GMTool.Dot;

public class AccountDto : accounts
{
	public DateTime updt_date { get; set; }

	public bool login_status { get; set; }

	public string login_ip { get; set; }
}
