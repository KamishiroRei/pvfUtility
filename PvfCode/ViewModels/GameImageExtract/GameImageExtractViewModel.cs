using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.NPK.Utils;
using PvfCode.NPK.Utils.Models;
using PvfCode.Views.GameImagesExtract;
using Swordfish.NET.Collections;
using Utools.内存;

namespace PvfCode.ViewModels.GameImageExtract;

public class GameImageExtractViewModel : ViewModelBase, IDisposable
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_0
	{
		public string zBhdIp0CoS;

		public _003C_003Ec__DisplayClass18_0()
		{
		}

		internal bool MHGdbwjVQt(ImagePack it)
		{
			return it.Path == zBhdIp0CoS;
		}
	}

	[CompilerGenerated]
	private ConcurrentObservableDictionary<string, ImageInfo> m3KfWJmMWJ;

	[CompilerGenerated]
	private Dictionary<string, List<ImagePack>> cr3fmSdMsL;

	private HwndSource mpGf2gSvWq;

	private MemoryUtils luRffVS4dP;

	private Process Y0hf5DebUk;

	public ConcurrentObservableDictionary<string, ImageInfo> LogList
	{
		[CompilerGenerated]
		get
		{
			return m3KfWJmMWJ;
		}
		[CompilerGenerated]
		set
		{
			m3KfWJmMWJ = value;
		}
	}

	public KeyValuePair<string, ImageInfo>? CurrentLog
	{
		get
		{
			return GetProperty(() => CurrentLog);
		}
		set
		{
			SetProperty<KeyValuePair<string, ImageInfo>?>(() => CurrentLog, value);
			if (value.HasValue)
			{
				ConcurrentObservableCollection<uint> indexItems = value.Value.Value.IndexItems;
				if (indexItems != null && indexItems.Any())
				{
					CurrentIndex = value.Value.Value.IndexItems.FirstOrDefault();
				}
			}
		}
	}

	public uint? CurrentIndex
	{
		get
		{
			return GetProperty(() => CurrentIndex);
		}
		set
		{
			SetProperty(() => CurrentIndex, value, OHbfD35KxK);
		}
	}

	public ImgFile? CurrentImgFile
	{
		get
		{
			return GetProperty(() => CurrentImgFile);
		}
		set
		{
			SetProperty<ImgFile>(() => CurrentImgFile, value, OHbfD35KxK);
		}
	}

	public bool IsStart
	{
		get
		{
			return GetProperty(() => IsStart);
		}
		set
		{
			SetProperty(() => IsStart, value);
		}
	}

	[SpecialName]
	[CompilerGenerated]
	private Dictionary<string, List<ImagePack>> OawfBrXBwu()
	{
		return cr3fmSdMsL;
	}

	[SpecialName]
	[CompilerGenerated]
	private void c4CfFlInGP(Dictionary<string, List<ImagePack>> P_0)
	{
		cr3fmSdMsL = P_0;
	}

	private void OHbfD35KxK()
	{
		if (!jxkflKYI91())
		{
			CurrentImgFile = null;
		}
	}

	private bool jxkflKYI91()
	{
		_003C_003Ec__DisplayClass18_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass18_0();
		if (!CurrentIndex.HasValue || !CurrentLog.HasValue)
		{
			return false;
		}
		ImagePack2Service instance = ImagePack2Service.Instance;
		string[] array = CurrentLog.Value.Key.Split("----");
		if (array != null && !array.Any())
		{
			return false;
		}
		CS_0024_003C_003E8__locals3.zBhdIp0CoS = "sprite/" + array[0].ToLower().Replace("\\", "/");
		if (!instance.NpkImgDIC.TryGetValue(CS_0024_003C_003E8__locals3.zBhdIp0CoS, out UtImgFile value) || value == null)
		{
			return false;
		}
		string npkFilePath = instance.GetNpkFilePath(value);
		if (!OawfBrXBwu().TryGetValue(npkFilePath, out List<ImagePack> value2))
		{
			value2 = NpkCoder.ReadNpk(npkFilePath);
			OawfBrXBwu().Add(npkFilePath, value2);
		}
		ImagePack imagePack = value2.Where((ImagePack it) => it.Path == CS_0024_003C_003E8__locals3.zBhdIp0CoS).FirstOrDefault();
		if (imagePack == null)
		{
			return false;
		}
		if (CurrentIndex >= imagePack.Count)
		{
			return false;
		}
		CurrentImgFile = imagePack.ImgList[(int)CurrentIndex.Value];
		return true;
	}

	public GameImageExtractViewModel()
	{
		c4CfFlInGP(new Dictionary<string, List<ImagePack>>());
		LogList = new ConcurrentObservableDictionary<string, ImageInfo>();
	}

	[Command]
	public void OnStart(Window win)
	{
		Start(win);
	}

	[Command]
	public void OnStop()
	{
		Stop();
	}

	[Command]
	public void OnClearLog()
	{
		LogList.Clear();
	}

	public void Start(Window win)
	{
		if (mpGf2gSvWq == null)
		{
			mpGf2gSvWq = HwndSource.FromHwnd(new WindowInteropHelper(win).Handle);
			mpGf2gSvWq.AddHook(gJJfjRG3kn);
		}
		Task.Run((Action)inits);
	}

	public void inits()
	{
		Y0hf5DebUk = MemoryUtils.GetProcessByProcessName("dnf");
		if (Y0hf5DebUk == null)
		{
			throw new Exception("没有找到 DNF 进程，请先启动客户端后再使用该功能。");
		}
		luRffVS4dP = new MemoryUtils(Y0hf5DebUk.Id);
		IntPtr intPtr = luRffVS4dP.AllocMemory(1024);
		int value = 9527;
		IntPtr intPtr2 = luRffVS4dP.FindWindow(WindowGameImageExtract.WindowTitle);
		IntPtr intPtr3 = V9DfhnIUEL();
		IntPtr intPtr4 = O4CfvOsCNw(intPtr3, "SendMessageA");
		luRffVS4dP.WriteInt(intPtr + 108, intPtr2.ToInt32());
		luRffVS4dP.WriteInt(intPtr + 100, value);
		byte[] array = new byte[31]
		{
			96, 106, 0, 84, 255, 53, 8, 1, 150, 15,
			255, 53, 0, 1, 150, 15, 232, 211, 142, 33,
			103, 97, 85, 139, 236, 93, 233, 81, 7, 134,
			241
		};
		luRffVS4dP.BuilderValue(array, 6, intPtr + 100);
		luRffVS4dP.BuilderValue(array, 12, intPtr + 108);
		luRffVS4dP.BuilderValue(array, 17, intPtr4 - (intPtr.ToInt32() + 21));
		luRffVS4dP.BuilderValue(array, 27, luRffVS4dP.GetMainModuleByProcessName("dnf") + 14419824 - (intPtr.ToInt32() + 31));
		luRffVS4dP.WriteByteArray(intPtr, array);
		IntPtr memoryAddress = luRffVS4dP.GetMemoryAddress("dnf", 28600396, 0, 16);
		Console.WriteLine("Vtableaddre:{0:x}", memoryAddress);
		Console.WriteLine("allo:{0:x}", intPtr.ToInt32());
		luRffVS4dP.WriteInt(memoryAddress, intPtr.ToInt32());
		IsStart = true;
	}

	public void Stop()
	{
		IsStart = false;
	}

	private IntPtr gJJfjRG3kn(IntPtr P_0, int P_1, IntPtr P_2, IntPtr P_3, ref bool P_4)
	{
		if (P_1 == 9527 && IsStart)
		{
			SelfStruct selfStruct = luRffVS4dP.ReadObject<SelfStruct>(P_2);
			string text = Encoding.Unicode.GetString(luRffVS4dP.ReadToBytes((IntPtr)selfStruct.addre11, 256));
			uint addre = selfStruct.addre12;
			string[] value = text.Split("\u0000");
			string text2 = string.Join("----", value);
			if (addre > 4095)
			{
				text2 += Encoding.Unicode.GetString(luRffVS4dP.ReadToBytes((IntPtr)selfStruct.addre12, 256));
			}
			if (!LogList.TryGetValue(text2, out ImageInfo value2))
			{
				value2 = new ImageInfo
				{
					CurrentValue = IMafCoa2fR(selfStruct),
					AllValue = aJ5fHnXjrj(selfStruct)
				};
				LogList.Add(text2, value2);
			}
			if (!value2.IndexItems.Contains(addre))
			{
				value2.IndexItems.Add(addre);
			}
		}
		return IntPtr.Zero;
	}

	private void pwdfTdRmfw(SelfStruct P_0)
	{
		FieldInfo[] fields = typeof(SelfStruct).GetFields(BindingFlags.Instance | BindingFlags.Public);
		for (int i = 0; i < fields.Length; i++)
		{
			object value = fields[i].GetValue(P_0);
			if (value == null || i <= 0 || !(value is uint num) || num < 0 || num >= 4095)
			{
				continue;
			}
			string[] value2 = Encoding.Unicode.GetString(luRffVS4dP.ReadToBytes((IntPtr)(uint)fields[i - 1].GetValue(P_0), 2560)).Split("\u0000");
			string text = string.Join("----", value2);
			if (text.ToLower().Contains("/") || text.Contains("\\"))
			{
				if (!LogList.TryGetValue(text, out ImageInfo value3))
				{
					value3 = new ImageInfo
					{
						Name = fields[i - 1].Name,
						Value = fields[i - 1].GetValue(P_0),
						Test = IMafCoa2fR(P_0)
					};
					LogList.Add(text, value3);
				}
				if (!value3.IndexItems.Contains(num))
				{
					value3.IndexItems.Add(num);
				}
			}
		}
	}

	private string IMafCoa2fR(SelfStruct P_0)
	{
		List<string> list = new List<string>();
		FieldInfo[] fields = typeof(SelfStruct).GetFields(BindingFlags.Instance | BindingFlags.Public);
		for (int i = 0; i < fields.Length; i++)
		{
			object value = fields[i].GetValue(P_0);
			if (value != null && i > 0 && value is uint num && num >= 0 && num < 4095)
			{
				list.Add(fields[i].Name + "：" + num);
			}
		}
		return "[" + string.Join(",", list) + "]";
	}

	private string aJ5fHnXjrj(SelfStruct P_0)
	{
		List<string> list = new List<string>();
		FieldInfo[] fields = typeof(SelfStruct).GetFields(BindingFlags.Instance | BindingFlags.Public);
		for (int i = 0; i < fields.Length; i++)
		{
			object value = fields[i].GetValue(P_0);
			if (value != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted(fields[i].Name);
				defaultInterpolatedStringHandler.AppendLiteral("：");
				defaultInterpolatedStringHandler.AppendFormatted<object>(value);
				list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
		return "[" + string.Join(",", list) + "]";
	}

	private void AddData(string imgPath, uint imgIndex)
	{
		string[] value = imgPath.Split("\u0000");
		string key = string.Join("----", value);
		if (!LogList.TryGetValue(key, out ImageInfo value2))
		{
			value2 = new ImageInfo();
			LogList.Add(key, value2);
		}
		if (!value2.IndexItems.Contains(imgIndex))
		{
			value2.IndexItems.Add(imgIndex);
		}
	}

	public void Dispose()
	{
		mpGf2gSvWq?.RemoveHook(gJJfjRG3kn);
		mpGf2gSvWq?.Dispose();
	}

	[CompilerGenerated]
	private IntPtr V9DfhnIUEL()
	{
		IntPtr aaa = IntPtr.Zero;
		if (luRffVS4dP.QueryInformationProcess(out aaa) == 0)
		{
			int num = luRffVS4dP.ReadToInt(aaa + 12) + 12;
			for (int num2 = luRffVS4dP.ReadToInt((IntPtr)num); num2 != num; num2 = luRffVS4dP.ReadToInt((IntPtr)num2))
			{
				int num3 = luRffVS4dP.ReadToInt((IntPtr)num2 + 48);
				string text = Encoding.Unicode.GetString(luRffVS4dP.ReadToBytes((IntPtr)num3, 512));
				bool flag = text.Contains("USER32.DLL");
				bool flag2 = text.Contains("user32.dll");
				if (flag || flag2)
				{
					return (IntPtr)luRffVS4dP.ReadToInt((IntPtr)num2 + 24);
				}
			}
		}
		return IntPtr.Zero;
	}

	[CompilerGenerated]
	private IntPtr O4CfvOsCNw(IntPtr P_0, string P_1)
	{
		IMAGE_DOS_HEADER iMAGE_DOS_HEADER = luRffVS4dP.ReadObject<IMAGE_DOS_HEADER>(P_0);
		IMAGE_NT_HEADER32 iMAGE_NT_HEADER = luRffVS4dP.ReadObject<IMAGE_NT_HEADER32>((IntPtr)(P_0.ToInt32() + iMAGE_DOS_HEADER.e_lfanew));
		IMAGE_EXPORT_DIRECTORY iMAGE_EXPORT_DIRECTORY = luRffVS4dP.ReadObject<IMAGE_EXPORT_DIRECTORY>((IntPtr)(P_0.ToInt32() + iMAGE_NT_HEADER.OptionalHeader.DataDirectory[0].VirtualAddress));
		for (int i = 0; i < iMAGE_EXPORT_DIRECTORY.NumberOfFunctions; i++)
		{
			int num = luRffVS4dP.ReadToInt((IntPtr)(P_0.ToInt32() + (iMAGE_EXPORT_DIRECTORY.AddressOfNames + i * 4)));
			if (Encoding.ASCII.GetString(luRffVS4dP.ReadToBytes((IntPtr)(P_0.ToInt32() + num), 256)).Split('\0')[0].Contains(P_1))
			{
				short num2 = luRffVS4dP.ReadToShort((IntPtr)(P_0.ToInt32() + iMAGE_EXPORT_DIRECTORY.AddressOfNameOrdinals + i * 2));
				int num3 = luRffVS4dP.ReadToInt((IntPtr)(P_0.ToInt32() + iMAGE_EXPORT_DIRECTORY.AddressOfFunctions + num2 * 4));
				return P_0 + num3;
			}
		}
		return IntPtr.Zero;
	}
}
