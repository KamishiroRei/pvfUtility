using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode;
using PvfCode.Services;

namespace ViewModels.Tools.ConvertChinaPvfFiles;

public class ViewConvertChinaPvfFilesViewModel : ViewModelBase
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

	public bool Convertequipmentpartset
	{
		get
		{
			return GetProperty(() => Convertequipmentpartset);
		}
		set
		{
			SetProperty(() => Convertequipmentpartset, value);
		}
	}

	public bool ConvertAddSection
	{
		get
		{
			return GetProperty(() => ConvertAddSection);
		}
		set
		{
			SetProperty(() => ConvertAddSection, value);
		}
	}

	[Command]
	public async void OnStart()
	{
		ServiceConvertChinaPlusPvf service = new ServiceConvertChinaPlusPvf(AppCore.ViewModelBase.PVF, Convertequipmentpartset, ConvertAddSection);
		IsLoading = true;
		await Task.Run(() => service.Start());
		IsLoading = false;
		AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_ConvertComplete"));
	}
}
