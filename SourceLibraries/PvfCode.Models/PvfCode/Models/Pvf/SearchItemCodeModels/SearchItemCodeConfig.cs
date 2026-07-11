using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using ICSharpCode.AvalonEdit.Document;

namespace PvfCode.Models.Pvf.SearchItemCodeModels;

public class SearchItemCodeConfig : ViewModelBase
{
	private TextDocument document;

	public TextDocument Document
	{
		get
		{
			if (document == null)
			{
				document = new TextDocument
				{
					Text = AppSetting.Instance.GetIlogger()?.GetStr("mess_SearchCode")
				};
			}
			return document;
		}
		set
		{
			document = value;
			RaisePropertyChanged("_Document");
		}
	}

	public List<int> GetItemCodes()
	{
		string text = Document.Text;
		if (string.IsNullOrEmpty(text))
		{
			return new List<int>();
		}
		string[] separator = new string[3]
		{
			"\t",
			" ",
			"\r\n"
		};
		string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
		if (array == null)
		{
			return new List<int>();
		}
		ConcurrentBag<int> itemCodes = new ConcurrentBag<int>();
		Parallel.ForEach(array, item =>
		{
			if (int.TryParse(item, out var result))
			{
				itemCodes.Add(result);
			}
		});
		return itemCodes.ToHashSet().ToList();
	}
}
