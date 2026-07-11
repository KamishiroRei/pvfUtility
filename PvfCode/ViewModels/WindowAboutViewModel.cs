using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using Utools;

namespace PvfCode.ViewModels;

public class WindowAboutViewModel : ViewModelBase
{
	[CompilerGenerated]
	private ObservableCollection<ContributeInfoDto> PfuFxOhT7P;

	[CompilerGenerated]
	private ObservableCollection<ContributeInfoDto> XduFQBAUFe;

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

	public ObservableCollection<ContributeInfoDto> ContributeInfoItems
	{
		[CompilerGenerated]
		get
		{
			return PfuFxOhT7P;
		}
		[CompilerGenerated]
		set
		{
			PfuFxOhT7P = value;
		}
	}

	public ObservableCollection<ContributeInfoDto> SponsorItems
	{
		[CompilerGenerated]
		get
		{
			return XduFQBAUFe;
		}
		[CompilerGenerated]
		set
		{
			XduFQBAUFe = value;
		}
	}

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
