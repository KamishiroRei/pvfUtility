using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Dot;
using PvfCode.Models.GameSqlModel;
using PvfCode.Models.Options;
using PvfCode.Services.GMTool;

namespace PvfCode.ViewModels.Game;

public class GameLoginViewModel : ViewModelBase
{
	private string oQw2NFfDm1;

	private string UeP2zwODLU;

	public bool GameIsStart
	{
		get
		{
			return GetProperty(() => GameIsStart);
		}
		set
		{
			SetProperty(() => GameIsStart, value);
		}
	}

	public bool IsSavePvfFile
	{
		get
		{
			return GetProperty(() => IsSavePvfFile);
		}
		set
		{
			SetProperty(() => IsSavePvfFile, value);
		}
	}

	public GameLoginViewModel()
	{
		oQw2NFfDm1 = "1FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00";
		UeP2zwODLU = "010101010101010101010101010101010101010101010101010101010101010155914510010403030101";
		IsSavePvfFile = true;
	}

	[Command]
	public async void OnStart(bool isStop)
	{
		if (isStop)
		{
			FOF28efm4C();
			return;
		}
		if (string.IsNullOrEmpty(AppSetting.Instance.GameOptions.GameClientPath))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSetClientDirInLoginSetting"));
			return;
		}
		if (IsSavePvfFile)
		{
			if (!AppCore.ViewModelBase.PVF.PvfIsOpen)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseLoadPvfPackWhenStartUp"), isError: true);
				return;
			}
			if (!Directory.Exists(Path.Combine(AppSetting.Instance.GameOptions.GameClientPath)))
			{
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_PathNotExist"), Path.Combine(AppSetting.Instance.GameOptions.GameClientPath)));
				return;
			}
			await Task.Run(async delegate
			{
				await AppCore.ViewModelBase.PVF.SavePvfPack(Path.Combine(AppSetting.Instance.GameOptions.GameClientPath, "script.pvf"), isFastMode: false, AppCore.ViewModelBase.MainProgress);
			});
		}
		FGC2c4yfgb();
		WindowLoading loading = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_LoginLoading"), Application.Current.MainWindow);
		loading.Show();
		int processId = await Task.Run(() => Login(AppSetting.Instance.GameOptions.GameUserA));
		AppSetting.Instance.GameOptions.GameUserA.ProcessId = processId;
		loading.Close();
	}

	private void FGC2c4yfgb()
	{
		List<Process> list = new List<Process>();
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			if (process.ProcessName.ToLower() == "dnf")
			{
				if (AppSetting.Instance.GameOptions.GameUserA.ProcessId == process.Id)
				{
					process.Kill();
					return;
				}
				list.Add(process);
			}
		}
		foreach (Process item in list)
		{
			item.Kill();
		}
	}

	[Command]
	public async void OnStart2()
	{
		WindowLoading loading = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_LoginLoading"), Application.Current.MainWindow);
		loading.Show();
		int processId = await Task.Run(() => Login(AppSetting.Instance.GameOptions.GameUserB));
		AppSetting.Instance.GameOptions.GameUserB.ProcessId = processId;
		loading.Close();
	}

	private void FOF28efm4C()
	{
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			if (process.ProcessName.ToLower() == "dnf")
			{
				process.Kill();
			}
		}
	}

	private async Task<int> Login(GameLoginAccountInfo loginUser)
	{
		ResultData<string> resultData;
		if (loginUser.UseUIDLogin)
		{
			if (string.IsNullOrEmpty(loginUser.UID))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_LoginUIDNotSet"), isError: true);
				return -1;
			}
			resultData = Jps2MoBOeQ(loginUser.UID);
		}
		else
		{
			ResultData<Accounts> resultData2 = await new GmtoolService().GameLogin(loginUser.UserName, loginUser.Password);
			if (resultData2.IsError)
			{
				AppCore.Logger.Error(resultData2.Msg);
				AppCore.ShowMsg(resultData2.Msg, isError: true);
				return -1;
			}
			resultData = Jps2MoBOeQ(resultData2.Data.UID.ToString());
		}
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg, isError: true);
			return -1;
		}
		if (string.IsNullOrEmpty(AppSetting.Instance.GameOptions.GameClientPath))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSetClientDirInLoginSetting"));
			return -1;
		}
		if (!File.Exists(Path.Combine(AppSetting.Instance.GameOptions.GameClientPath, "dnf.exe")))
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_NotFoundDNFExe"), AppSetting.Instance.GameOptions.GameClientPath), isError: true);
			return -1;
		}
		string text = (string.IsNullOrEmpty(loginUser.ClientPath) ? AppSetting.Instance.GameOptions.GameClientPath : loginUser.ClientPath);
		return Process.Start(new ProcessStartInfo
		{
			FileName = Path.Combine(text, "dnf.exe"),
			WorkingDirectory = text,
			Arguments = resultData.Data
		})?.Id ?? (-1);
	}

	private ResultData<string> Jps2MoBOeQ(string P_0)
	{
		ResultData<string> resultData = new ResultData<string>();
		try
		{
			string s = P_0;
			if (string.IsNullOrEmpty(AppSetting.Instance.GameOptions.GameServerOptions.PemPrivateKey))
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_PleaseSetPrivateKeyInLoginSetting");
				return resultData;
			}
			RSAParameters rSAParameters = ConvertFromPemPrivateKey(AppSetting.Instance.GameOptions.GameServerOptions.PemPrivateKey);
			BigInteger bigInteger = new BigInteger(rSAParameters.Modulus);
			BigInteger bigInteger2 = new BigInteger(rSAParameters.D);
			s = int.Parse(s).ToString("X8");
			BigInteger bigInteger3 = new BigInteger(e3o2Vgjd8m(oQw2NFfDm1 + s + UeP2zwODLU, bigInteger2, bigInteger), 16);
			resultData.Data = Convert.ToBase64String(bigInteger3.getBytes());
		}
		catch (Exception ex)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_GenerateLoginParamError"), ex.Message);
		}
		return resultData;
	}

	public RSAParameters ConvertFromPemPrivateKey(string pemFileConent)
	{
		if (string.IsNullOrEmpty(pemFileConent))
		{
			throw new ArgumentNullException("pemFileConent", "This arg cann't be empty.");
		}
		pemFileConent = pemFileConent.Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "").Replace("\n", "")
			.Replace("\r", "");
		byte[] array = Convert.FromBase64String(pemFileConent);
		bool flag;
		int num = ((flag = array.Length == 609 || array.Length == 610) ? 11 : 12);
		byte[] array2 = (flag ? new byte[128] : new byte[256]);
		Array.Copy(array, num, array2, 0, array2.Length);
		num += array2.Length;
		num += 2;
		byte[] array3 = new byte[3];
		Array.Copy(array, num, array3, 0, 3);
		num += 3;
		num += 4;
		if (array[num] == 0)
		{
			num++;
		}
		byte[] array4 = (flag ? new byte[128] : new byte[256]);
		Array.Copy(array, num, array4, 0, array4.Length);
		num += array4.Length;
		num += ((!flag) ? ((array[num + 2] == 128) ? 3 : 4) : ((array[num + 1] == 64) ? 2 : 3));
		byte[] array5 = (flag ? new byte[64] : new byte[128]);
		Array.Copy(array, num, array5, 0, array5.Length);
		num += array5.Length;
		num += ((!flag) ? ((array[num + 2] == 128) ? 3 : 4) : ((array[num + 1] == 64) ? 2 : 3));
		byte[] array6 = (flag ? new byte[64] : new byte[128]);
		Array.Copy(array, num, array6, 0, array6.Length);
		num += array6.Length;
		num += ((!flag) ? ((array[num + 2] == 128) ? 3 : 4) : ((array[num + 1] == 64) ? 2 : 3));
		byte[] array7 = (flag ? new byte[64] : new byte[128]);
		Array.Copy(array, num, array7, 0, array7.Length);
		num += array7.Length;
		num += ((!flag) ? ((array[num + 2] == 128) ? 3 : 4) : ((array[num + 1] == 64) ? 2 : 3));
		byte[] array8 = (flag ? new byte[64] : new byte[128]);
		Array.Copy(array, num, array8, 0, array8.Length);
		num += array8.Length;
		num += ((!flag) ? ((array[num + 2] == 128) ? 3 : 4) : ((array[num + 1] == 64) ? 2 : 3));
		byte[] array9 = (flag ? new byte[64] : new byte[128]);
		Array.Copy(array, num, array9, 0, array9.Length);
		return new RSAParameters
		{
			Modulus = array2,
			Exponent = array3,
			D = array4,
			P = array5,
			Q = array6,
			DP = array7,
			DQ = array8,
			InverseQ = array9
		};
	}

	private static string e3o2Vgjd8m(string P_0, BigInteger P_1, BigInteger P_2)
	{
		return new BigInteger(P_0, 16).modPow(P_1, P_2).ToHexString();
	}

	[CompilerGenerated]
	private Task<int>? VCu234ZI6j()
	{
		return Login(AppSetting.Instance.GameOptions.GameUserA);
	}

	[CompilerGenerated]
	private Task<int>? QT82Rj4Z3O()
	{
		return Login(AppSetting.Instance.GameOptions.GameUserB);
	}
}
