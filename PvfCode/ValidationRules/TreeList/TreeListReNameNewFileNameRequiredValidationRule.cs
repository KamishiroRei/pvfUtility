using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PvfCode.ValidationRules.TreeList;

[ContentProperty("ComparisonNode")]
public class TreeListReNameNewFileNameRequiredValidationRule : ValidationRule
{
	[CompilerGenerated]
	private ComparisonNode QtnQHRj5Bv;

	public ComparisonNode ComparisonNode
	{
		[CompilerGenerated]
		get
		{
			return QtnQHRj5Bv;
		}
		[CompilerGenerated]
		set
		{
			QtnQHRj5Bv = value;
		}
	}

	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		if (value == null || value.ToString().Length == 0)
		{
			string str = AppCore.Logger.GetStr("FileExplorer_RenameFile_NewFileNameIsNull");
			return new ValidationResult(isValid: false, str);
		}
		string text = value.ToString().ToLower();
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		PvfTreeFileRename value2 = ComparisonNode.Value;
		string newFullPath = value2.GetNewFullPath(text);
		if (pVF.FileAny(newFullPath) && value2.CheckChanged(text))
		{
			string str = string.Format(AppCore.Logger.GetStr("FileExplorer_RenameFile_FileNameIsRepeatInPvfPack"), newFullPath);
			return new ValidationResult(isValid: false, str);
		}
		if (NQ7QCQLSPC(newFullPath, text, value2) && value2.CheckChanged(text))
		{
			string str = string.Format(AppCore.Logger.GetStr("FileExplorer_RenameFile_FileNameIsRepeat"), text);
			return new ValidationResult(isValid: false, str);
		}
		return ValidationResult.ValidResult;
	}

	private bool NQ7QCQLSPC(string P_0, string P_1, PvfTreeFileRename P_2)
	{
		char[] separator = new char[2] { '\\', '/' };
		string[] array = P_0.Split(separator);
		IDictionary<string, PvfTreeFileBase> dictionary = ComparisonNode.Source;
		for (int i = 0; i < array.Length - 1; i++)
		{
			if (dictionary.TryGetValue(array[i], out var value))
			{
				dictionary = value.Children;
				continue;
			}
			return false;
		}
		foreach (PvfTreeFileRename value2 in dictionary.Values)
		{
			if (P_2 != value2 && value2.NewFileName.ToLower() == P_1)
			{
				return true;
			}
		}
		return false;
	}

	public TreeListReNameNewFileNameRequiredValidationRule()
	{
	}
}
