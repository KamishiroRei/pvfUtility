using System;
using System.Collections.Generic;
using Utools;

namespace PvfCode.Dot.Desktop.interfaces;

public interface IBookMarkGroup
{
	int Id { get; set; }

	ObservableConcurrentDictionaryEx<string, BookMarkDto> Trees { get; set; }

	bool IsShare { get; set; }

	DateTime UpdateTime { get; set; }

	DateTime Create { get; set; }

	string Title { get; set; }

	string Instructions { get; set; }

	string DetailedInstructions { get; set; }

	decimal Money { get; set; }

	int DownLoadNumber { get; set; }

	string CheckTitle(IDictionary<string, BookMarkDto> source, string title);

	KeyValuePair<string, BookMarkDto> AddNode(string title, BookMarkDto bookMarkData, IDictionary<string, BookMarkDto>? source);
}
