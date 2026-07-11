using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;

namespace PvfCode.Views.Dialogs;

public class DialogStringListViewModel : ViewModelBase
{
	[CompilerGenerated]
	private string F1svQZdDfQ;

	[CompilerGenerated]
	private string DeSvaadT5o;

	[CompilerGenerated]
	private string ympvgwPORc;

	[CompilerGenerated]
	private string Bd7v6EIFEk;

	public string Message
	{
		[CompilerGenerated]
		get
		{
			return F1svQZdDfQ;
		}
		[CompilerGenerated]
		set
		{
			F1svQZdDfQ = value;
		}
	}

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

	public string YseTitle
	{
		[CompilerGenerated]
		get
		{
			return DeSvaadT5o;
		}
		[CompilerGenerated]
		set
		{
			DeSvaadT5o = value;
		}
	}

	public string NoTitle
	{
		[CompilerGenerated]
		get
		{
			return ympvgwPORc;
		}
		[CompilerGenerated]
		set
		{
			ympvgwPORc = value;
		}
	}

	public string CancelTitle
	{
		[CompilerGenerated]
		get
		{
			return Bd7v6EIFEk;
		}
		[CompilerGenerated]
		set
		{
			Bd7v6EIFEk = value;
		}
	}

	public DialogStringListViewModel(IEnumerable<string> stringList, string message, string yseTitle, string noTitle, string cancelTitle)
	{
		StringList = stringList;
		Message = message;
		YseTitle = yseTitle;
		NoTitle = noTitle;
		CancelTitle = cancelTitle;
	}
}
