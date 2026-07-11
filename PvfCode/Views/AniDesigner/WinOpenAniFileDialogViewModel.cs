using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

namespace PvfCode.Views.AniDesigner;

public class WinOpenAniFileDialogViewModel : ViewModelBase
{
	protected virtual IMessageBoxService MessageBoxService => GetService<IMessageBoxService>();

	public string FilePath
	{
		get
		{
			return GetProperty(() => FilePath);
		}
		set
		{
			SetProperty<string>(() => FilePath, value);
		}
	}

	public WinOpenAniFileDialogViewModel()
	{
		FilePath = "monster/characterstatue/fighteranimation/attack1.ani";
	}

	[Command]
	public void OnYes(Window win)
	{
		if (string.IsNullOrEmpty(FilePath))
		{
			MessageBoxService.ShowMessage("请输入ani文件路径", AppSetting.Instance.AppName, MessageButton.OK, MessageIcon.Error);
		}
		else if (!AppCore.ViewModelBase.PVF.FileAny(FilePath))
		{
			MessageBoxService.ShowMessage("文件不存在：" + FilePath, AppSetting.Instance.AppName, MessageButton.OK, MessageIcon.Error);
		}
		else
		{
			win.DialogResult = true;
		}
	}
}
