using System.Runtime.CompilerServices;
using Collections.Pooled;
using PvfCode.Services.SearchModel;

namespace PvfCode.ViewModels.SearchPvf;

public class SearchResultGroup
{
	[CompilerGenerated]
	private SearchConfig DWqWu0rTLN;

	private PooledList<string> RDAWGbQ7iE;

	public SearchConfig Config
	{
		[CompilerGenerated]
		get
		{
			return DWqWu0rTLN;
		}
		[CompilerGenerated]
		set
		{
			DWqWu0rTLN = value;
		}
	}

	public PooledList<string> FileList
	{
		get
		{
			if (RDAWGbQ7iE == null)
			{
				RDAWGbQ7iE = new PooledList<string>();
			}
			return RDAWGbQ7iE;
		}
		set
		{
			RDAWGbQ7iE = value;
		}
	}

	public void Clear()
	{
		FileList?.Dispose();
		FileList = null;
	}

	public SearchResultGroup()
	{
	}
}
