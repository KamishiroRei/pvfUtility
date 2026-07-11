using System.Runtime.CompilerServices;

namespace PvfCode.ViewModels.DocumentFolder.KorElementGenerator;

public class StringQuoteRoot
{
	[CompilerGenerated]
	private string TLt4MtP34x;

	[CompilerGenerated]
	private int nPd4VStgXm;

	public string FileName
	{
		[CompilerGenerated]
		get
		{
			return TLt4MtP34x;
		}
		[CompilerGenerated]
		private set
		{
			TLt4MtP34x = value;
		}
	}

	public int Count
	{
		[CompilerGenerated]
		get
		{
			return nPd4VStgXm;
		}
		[CompilerGenerated]
		set
		{
			nPd4VStgXm = value;
		}
	}

	public StringQuoteRoot(string fileName, int count)
	{
		FileName = fileName;
		Count = count;
	}
}
