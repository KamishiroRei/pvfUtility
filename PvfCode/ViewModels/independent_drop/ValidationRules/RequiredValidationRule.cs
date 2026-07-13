using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using PvfCode.Models.Pvf;
using PvfCode.ViewModels.independent_drop.DropList;

namespace PvfCode.ViewModels.independent_drop.ValidationRules;

public class RequiredValidationRule : ValidationRule
{
	public static Dictionary<int, KeyValuePair<List<ListItem>, LstItem>> DIC;

	public string FieldName { get; set; }

	public static string GetErrorMessage(string fieldName, object fieldValue, object nullValue = null)
	{
		string result = string.Empty;
		if (nullValue != null && nullValue.Equals(fieldValue))
		{
			result = string.Format("You cannot leave the {0} field empty.", fieldName);
		}
		if (fieldValue == null || string.IsNullOrEmpty(fieldValue.ToString()))
		{
			result = string.Format("You cannot leave the {0} field empty.", fieldName);
		}
		return result;
	}

	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		string text = "";
		int result;
		if (value == null)
		{
			text = AppSetting.Instance.GetIlogger().GetStr("mess_LstIdCannotBeEmpty");
		}
		else if (!int.TryParse(value.ToString(), out result))
		{
			text = AppSetting.Instance.GetIlogger().GetStr("mess_LstIdMustBeInt");
		}
		else if (!DIC.ContainsKey(result))
		{
			text = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_LstIdNotExistPleaseCheck"), result);
		}
		if (!string.IsNullOrEmpty(text))
		{
			return new ValidationResult(isValid: false, text);
		}
		return ValidationResult.ValidResult;
	}

	public RequiredValidationRule()
	{
	}
}
