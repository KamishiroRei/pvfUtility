using System.ComponentModel.DataAnnotations;

namespace Utools;

public class IsEmialAttribute : ValidationAttribute
{
	public override bool IsValid(object value)
	{
		return true;
	}

	public IsEmialAttribute()
	{
	}
}
