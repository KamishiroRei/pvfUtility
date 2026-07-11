using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PvfCode.Dot.Desktop.interfaces;
using Utools;

namespace PvfCode.Dot.Desktop;

[JsonObject(MemberSerialization.OptOut)]
public class BookMarkGroupDto : ModelBase, IBookMarkGroup
{
	private ObservableConcurrentDictionaryEx<string, BookMarkDto> _Trees;

	private bool _IsShare;

	private string _Title;

	private string _Instructions;

	private string _DetailedInstructions;

	private decimal _Money;

	public ObservableConcurrentDictionaryEx<string, BookMarkDto> Trees
	{
		get
		{
			if (_Trees == null)
			{
				_Trees = new ObservableConcurrentDictionaryEx<string, BookMarkDto>();
				_Trees.Add("我的书签", new BookMarkDto());
				Create = DateTime.Now;
				UpdateTime = DateTime.Now;
			}
			return _Trees;
		}
		set
		{
			_Trees = value;
			DoNotify("Trees");
		}
	}

	public bool IsShare
	{
		get
		{
			return _IsShare;
		}
		set
		{
			_IsShare = value;
			DoNotify("IsShare");
		}
	}

	public DateTime UpdateTime { get; set; }

	public DateTime Create { get; set; }

	public string Title
	{
		get
		{
			return _Title;
		}
		set
		{
			_Title = value;
			DoNotify("Title");
		}
	}

	public string Instructions
	{
		get
		{
			return _Instructions;
		}
		set
		{
			_Instructions = value;
			DoNotify("Instructions");
		}
	}

	public string DetailedInstructions
	{
		get
		{
			if (string.IsNullOrEmpty(_DetailedInstructions))
			{
				_DetailedInstructions = "//您的书签详细说明\r\n//为鼓励作者共享 该处允许适当投放广告";
			}
			return _DetailedInstructions;
		}
		set
		{
			_DetailedInstructions = value;
			DoNotify("DetailedInstructions");
		}
	}

	public decimal Money
	{
		get
		{
			return _Money;
		}
		set
		{
			_Money = value;
			DoNotify("Money");
		}
	}

	public int Id { get; set; }

	public int DownLoadNumber { get; set; }

	public string NickName { get; set; }

	public string Avatar { get; set; }

	public string CheckTitle(IDictionary<string, BookMarkDto> source, string title)
	{
		string text = title;
		int num = 0;
		while (source.ContainsKey(text))
		{
			text = $"{title}({num})";
			num++;
		}
		return text;
	}

	public KeyValuePair<string, BookMarkDto> AddNode(string title, BookMarkDto bookMarkData, IDictionary<string, BookMarkDto>? source)
	{
		title = CheckTitle(source, title);
		source.TryAdd(title, bookMarkData);
		UpdateTime = DateTime.Now;
		return source.First<KeyValuePair<string, BookMarkDto>>((KeyValuePair<string, BookMarkDto> it) => it.Key == title);
	}

	public void DownSaveBookMark(ObservableConcurrentDictionaryEx<string, BookMarkDto> source, bool isMerge)
	{
		if (isMerge)
		{
			source = source["我的书签"].Children;
			ObservableConcurrentDictionaryEx<string, BookMarkDto> children = Trees["我的书签"].Children;
			MergeBookMark(source, children);
		}
		else
		{
			Trees = source;
		}
	}

	private void MergeBookMark(IDictionary<string, BookMarkDto> source, ObservableConcurrentDictionaryEx<string, BookMarkDto> target)
	{
		foreach (KeyValuePair<string, BookMarkDto> item in source)
		{
			if (item.Value.IsFile)
			{
				if (!target.ContainsKey(item.Key))
				{
					item.Value.Sort = GetSortNum(target.Values);
					target.Add(item);
				}
			}
			else if (target.ContainsKey(item.Key))
			{
				if (!target[item.Key].HaveChildren())
				{
					if (!item.Value.HaveChildren())
					{
						continue;
					}
					int num = 0;
					foreach (KeyValuePair<string, BookMarkDto> item2 in (IEnumerable<KeyValuePair<string, BookMarkDto>>)item.Value.Children)
					{
						item2.Value.Sort = num;
						num++;
					}
					((ICollection<KeyValuePair<string, BookMarkDto>>)target[item.Key].Children).AddRange((IEnumerable<KeyValuePair<string, BookMarkDto>>)item.Value.Children);
				}
				else
				{
					MergeBookMark(item.Value.Children, target[item.Key].Children);
				}
			}
			else
			{
				item.Value.Sort = GetSortNum(target.Values);
				target.Add(item);
			}
		}
	}

	private int GetSortNum(IEnumerable<BookMarkDto> list)
	{
		if (!list.Any())
		{
			return 0;
		}
		return list.Max((BookMarkDto it) => it.Sort) + 1;
	}
}
