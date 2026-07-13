using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using Utools;

namespace PvfCode.ViewModels;

public class WindowAboutViewModel : ViewModelBase
{
	public bool IsLoading
	{
		get
		{
			return GetProperty(() => IsLoading);
		}
		set
		{
			SetProperty(() => IsLoading, value);
		}
	}

	public ObservableCollection<ContributeInfoDto> ContributeInfoItems { get; set; }

	public ObservableCollection<ContributeInfoDto> SponsorItems { get; set; }

	public WindowAboutViewModel()
	{
		ContributeInfoItems = new ObservableCollection<ContributeInfoDto>();
		SponsorItems = new ObservableCollection<ContributeInfoDto>();
	}

	[Command]
	public async void Loaded()
	{
		await Task.CompletedTask;
		IsLoading = false;
	}

}
