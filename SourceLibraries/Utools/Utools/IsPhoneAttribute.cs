using System.ComponentModel.DataAnnotations;

namespace Utools;

public class IsPhoneAttribute : ValidationAttribute
{
	public override bool IsValid(object value)
	{
		return true;
	}

	public IsPhoneAttribute()
	{
	}
}
