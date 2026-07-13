using System.Collections.Generic;

namespace PvfCode.Web.PvfEditHttpServiceModels;

public class PvfEditHttpCommand
{
	public Command Cmd;

	public string Value;

	public int UseDecompile;

	public IEnumerable<string> FilePaths;

	public int ItemCode;

	public string ItemName;

	public string FileText;

	public ExtractSetting ExtractSet;

	public int Error => (!string.IsNullOrEmpty(ErrorStr)) ? 1 : 0;

	public string ErrorStr { get; set; }

	public bool? GetUseDecompile()
	{
		if (UseDecompile == -1)
		{
			return null;
		}
		return UseDecompile == 1;
	}

	public PvfEditHttpCommand()
	{
	}
}
