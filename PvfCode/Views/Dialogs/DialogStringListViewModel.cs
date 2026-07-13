using System.Collections.Generic;
using DevExpress.Mvvm;

namespace PvfCode.Views.Dialogs;

public class DialogStringListViewModel : ViewModelBase
{
	public string Message { get; set; }

	public IEnumerable<string> StringList
	{
		get
		{
			return GetProperty(() => StringList);
		}
		set
		{
			SetProperty<IEnumerable<string>>(() => StringList, value);
		}
	}

	public string YseTitle { get; set; }

	public string NoTitle { get; set; }

	public string CancelTitle { get; set; }

	public DialogStringListViewModel(IEnumerable<string> stringList, string message, string yseTitle, string noTitle, string cancelTitle)
	{
		StringList = stringList;
		Message = message;
		YseTitle = yseTitle;
		NoTitle = noTitle;
		CancelTitle = cancelTitle;
	}
}
