using System.ComponentModel.DataAnnotations;

namespace Utools;

public class IsQQAttribute : ValidationAttribute
{
	public override bool IsValid(object value)
	{
		if (value == null)
		{
			base.ErrorMessage = "请输入正确的QQ号码";
			return false;
		}
		int length = value.ToString().Length;
		if (length < 5 || length >= 15)
		{
			base.ErrorMessage = "请输入正确的QQ号码";
			return false;
		}
		return true;
	}

	public IsQQAttribute()
	{
	}
}
