using System;
using System.Runtime.CompilerServices;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;

namespace PvfCode.ViewModels.Login;

public class LoginViewModel : ViewModelBase
{
	[CompilerGenerated]
	private Action qYWm7grJoH;

	[CompilerGenerated]
	private Window aiOmXUqXMK;

	[CompilerGenerated]
	private RegIsteredAccountRes oUFmpEPYDq;

	public Action Close
	{
		[CompilerGenerated]
		get
		{
			return qYWm7grJoH;
		}
		[CompilerGenerated]
		set
		{
			qYWm7grJoH = value;
		}
	}

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

	public LoginViewType ViewType
	{
		get
		{
			return GetProperty(() => ViewType);
		}
		set
		{
			SetProperty(() => ViewType, value, wDbmZh1lVP);
		}
	}

	public string Title => ViewType switch
	{
		LoginViewType.Login => AppSetting.Instance.GetIlogger()?.GetStr("LoginWindowTitle"), 
		LoginViewType.Register => AppSetting.Instance.GetIlogger()?.GetStr("RegisterWindowTitle"), 
		LoginViewType.ForgetPassword => AppSetting.Instance.GetIlogger()?.GetStr("FindPasswordWindowTitle"), 
		_ => AppSetting.Instance.GetIlogger()?.GetStr("LoginWindowTitle"), 
	};

	public bool NickNameEditBoxVisibility => ViewType == LoginViewType.Register;

	public LoginAccountRes LoginModel
	{
		get
		{
			return GetProperty(() => LoginModel);
		}
		set
		{
			SetProperty<LoginAccountRes>(() => LoginModel, value);
		}
	}

	public RegIsteredAccountRes RegIsteredModel
	{
		[CompilerGenerated]
		get
		{
			return oUFmpEPYDq;
		}
		[CompilerGenerated]
		set
		{
			oUFmpEPYDq = value;
		}
	}

	public string PasswordCaption
	{
		get
		{
			if (ViewType != LoginViewType.ForgetPassword)
			{
				return AppSetting.Instance.GetIlogger()?.GetStr("PasswordCaption");
			}
			return AppSetting.Instance.GetIlogger()?.GetStr("NewPasswordCaption");
		}
	}

	[SpecialName]
	[CompilerGenerated]
	private Window l3tmJat4iN()
	{
		return aiOmXUqXMK;
	}

	[SpecialName]
	[CompilerGenerated]
	private void EoZmkLlA8v(Window P_0)
	{
		aiOmXUqXMK = P_0;
	}

	public void Loaded(Window win)
	{
		EoZmkLlA8v(win);
	}

	private void wDbmZh1lVP()
	{
		RaisePropertyChanged("Title");
		RaisePropertyChanged("PasswordCaption");
		RaisePropertyChanged("NickNameEditBoxVisibility");
	}

	public LoginViewModel()
	{
		if (AppSetting.Instance.LoginUser != null)
		{
			LoginModel = AppSetting.Instance.LoginUser;
		}
		else
		{
			LoginModel = new LoginAccountRes();
		}
		RegIsteredModel = new RegIsteredAccountRes();
	}

	[Command]
	public void OnChangedViewType(LoginViewType type)
	{
		ViewType = type;
	}

	[Command]
	public void OnLogin()
	{
		IsLoading = true;
		string title = "";
		switch (ViewType)
		{
		case LoginViewType.Login:
			title = AppSetting.Instance.GetIlogger().GetStr("LoginWindow_LoginLoading");
			break;
		case LoginViewType.Register:
			title = AppSetting.Instance.GetIlogger().GetStr("LoginWindow_RegisterLoading");
			break;
		case LoginViewType.ForgetPassword:
			title = AppSetting.Instance.GetIlogger().GetStr("LoginWindow_FindPasswordLoading");
			break;
		}
		WindowLoading windowLoading = AppCore.CreateLoading(title, l3tmJat4iN());
		windowLoading.Show();
		try
		{
			switch (ViewType)
			{
			case LoginViewType.Login:
				Login();
				break;
			case LoginViewType.Register:
				Register();
				break;
			case LoginViewType.ForgetPassword:
				ForgetPassword();
				break;
			}
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg(ex.Message);
			AppCore.Logger.Error(ex.Message);
		}
		windowLoading.Close();
		IsLoading = false;
	}

	private async void Login()
	{
		ResultData<LoginResultDto> resultData = await ServiceCloud.Instance.Login(LoginModel);
		if (resultData.IsError)
		{
			AppCore.Logger.ShowMsg(resultData.Msg, isError: true);
			return;
		}
		AppSetting.Instance.LoginUser = LoginModel;
		await AppSetting.Instance.SaveSetting();
		if (Close != null)
		{
			Close();
		}
	}

	private async void Register()
	{
		ResultData resultData = await ServiceCloud.Instance.Registered(RegIsteredModel);
		if (resultData.IsError)
		{
			AppCore.Logger.ShowMsg(resultData.Msg, isError: true);
			return;
		}
		AppCore.Logger.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("LoginWindow_RegisterSuccess"));
		ViewType = LoginViewType.Login;
		LoginModel.UserName = RegIsteredModel.UserName;
		LoginModel.Password = RegIsteredModel.Password;
	}

	private async void ForgetPassword()
	{
		RegIsteredModel.NickName = "Forget";
		ResultData resultData = await ServiceCloud.Instance.ForgetPassword(RegIsteredModel);
		if (resultData.IsError)
		{
			AppCore.Logger.ShowMsg(resultData.Msg, isError: true);
			return;
		}
		AppCore.Logger.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("LoginWindow_FindPasswordSuccess"));
		ViewType = LoginViewType.Login;
	}

	[Command]
	public async void Logout()
	{
		await ServiceCloud.Instance.Logout();
	}
}
