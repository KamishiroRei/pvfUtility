using System.ComponentModel.DataAnnotations;

namespace Utools;

public class IsIdCardAttribute : ValidationAttribute
{
	public override bool IsValid(object value)
	{
		return true;
	}

	public IsIdCardAttribute()
	{
	}
}
