using Collections.Pooled;
using PvfCode.Services.SearchModel;

namespace PvfCode.ViewModels.SearchPvf;

public class SearchResultGroup
{
	private PooledList<string> _fileList;

	public SearchConfig Config { get; set; }

	public PooledList<string> FileList
	{
		get
		{
			if (_fileList == null)
			{
				_fileList = new PooledList<string>();
			}
			return _fileList;
		}
		set
		{
			_fileList = value;
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
