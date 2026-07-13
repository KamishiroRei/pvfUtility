using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PvfCode.ValidationRules.TreeList;

[ContentProperty("ComparisonNode")]
public class TreeListReNameNewFileNameRequiredValidationRule : ValidationRule
{
	public ComparisonNode ComparisonNode { get; set; }

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
		if (HasDuplicateName(newFullPath, text, value2) && value2.CheckChanged(text))
		{
			string str = string.Format(AppCore.Logger.GetStr("FileExplorer_RenameFile_FileNameIsRepeat"), text);
			return new ValidationResult(isValid: false, str);
		}
		return ValidationResult.ValidResult;
	}

	private bool HasDuplicateName(string newFullPath, string newFileName, PvfTreeFileRename currentFile)
	{
		char[] separator = new char[2] { '\\', '/' };
		string[] pathSegments = newFullPath.Split(separator);
		IDictionary<string, PvfTreeFileBase> dictionary = ComparisonNode.Source;
		for (int i = 0; i < pathSegments.Length - 1; i++)
		{
			if (dictionary.TryGetValue(pathSegments[i], out var value))
			{
				dictionary = value.Children;
				continue;
			}
			return false;
		}
		foreach (PvfTreeFileRename file in dictionary.Values)
		{
			if (currentFile != file && file.NewFileName.ToLower() == newFileName)
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
