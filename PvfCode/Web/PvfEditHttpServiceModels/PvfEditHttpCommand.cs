using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PvfCode.Web.PvfEditHttpServiceModels;

public class PvfEditHttpCommand
{
	[CompilerGenerated]
	private string YRET8Ec51o;

	public Command Cmd;

	public string Value;

	public int UseDecompile;

	public IEnumerable<string> FilePaths;

	public int ItemCode;

	public string ItemName;

	public string FileText;

	public ExtractSetting ExtractSet;

	public int Error => (!string.IsNullOrEmpty(ErrorStr)) ? 1 : 0;

	public string ErrorStr
	{
		[CompilerGenerated]
		get
		{
			return YRET8Ec51o;
		}
		[CompilerGenerated]
		set
		{
			YRET8Ec51o = value;
		}
	}

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
