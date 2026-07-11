using System.Linq;
using Newtonsoft.Json;
using Utools;

namespace PvfCode.Dot.Desktop;

[JsonObject(MemberSerialization.OptOut)]
public class BookMarkDto : ModelBase
{
	private int _Sort;

	private bool? _PasteStatus;

	private ObservableConcurrentDictionaryEx<string, BookMarkDto> _Children;

	public string? FilePath { get; set; }

	public bool IsFile { get; set; }

	public int Sort
	{
		get
		{
			return _Sort;
		}
		set
		{
			_Sort = value;
		}
	}

	public bool? CutStatus
	{
		get
		{
			return _PasteStatus;
		}
		set
		{
			_PasteStatus = value;
			DoNotify("CutStatus");
		}
	}

	public ObservableConcurrentDictionaryEx<string, BookMarkDto> Children
	{
		get
		{
			if (_Children == null)
			{
				_Children = new ObservableConcurrentDictionaryEx<string, BookMarkDto>();
			}
			return _Children;
		}
		set
		{
			_Children = value;
		}
	}

	public BookMarkDto()
	{
		IsFile = false;
	}

	public BookMarkDto(string filePath)
	{
		FilePath = filePath;
		IsFile = true;
	}

	public void ChangedSort(int sort)
	{
		Sort = sort;
		DoNotify("Sort");
	}

	public int ChildrenCount()
	{
		if (_Children != null)
		{
			return _Children.Count();
		}
		return 0;
	}

	public bool HaveChildren()
	{
		if (_Children != null)
		{
			return _Children.Any();
		}
		return false;
	}
}
