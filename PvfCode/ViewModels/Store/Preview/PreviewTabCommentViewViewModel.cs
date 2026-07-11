using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;
using PvfCode.Dot.Desktop;

namespace PvfCode.ViewModels.Store.Preview;

public class PreviewTabCommentViewViewModel : ViewModelBase
{
	[CompilerGenerated]
	private List<PvfCommentDto> cNgWAvTmSR;

	public string Count
	{
		get
		{
			return GetProperty(() => Count);
		}
		set
		{
			SetProperty<string>(() => Count, value);
		}
	}

	public List<PvfCommentDto> Items
	{
		[CompilerGenerated]
		get
		{
			return cNgWAvTmSR;
		}
		[CompilerGenerated]
		set
		{
			cNgWAvTmSR = value;
		}
	}

	public PvfCommentDto SelectedItem
	{
		get
		{
			return GetProperty(() => SelectedItem);
		}
		set
		{
			SetProperty<PvfCommentDto>(() => SelectedItem, value);
		}
	}

	public PreviewTabCommentViewViewModel(List<PvfCommentDto> items)
	{
		Items = items;
		Count = string.Format(AppSetting.Instance.GetIlogger()?.GetStrNoReplace("mess_Total"), items.Count);
	}
}
