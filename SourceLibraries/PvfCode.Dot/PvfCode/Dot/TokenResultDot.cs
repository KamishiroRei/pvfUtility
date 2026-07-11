using System;
using Utools;

namespace PvfCode.Dot;

public class TokenResultDot
{
	public string Token { get; set; }

	public DateTime ExpireTime { get; set; }

	public string GetToken(string pwd)
	{
		if (string.IsNullOrEmpty(Token))
		{
			return string.Empty;
		}
		return Token.TextDecrypt(pwd);
	}
}
