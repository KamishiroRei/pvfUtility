using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
	private HwndSource hwndSource;

	private MemoryUtils memoryUtils;

	private Process process;

	private Dictionary<string, List<ImagePack>> ImagePacks { get; set; }

	public ConcurrentObservableDictionary<string, ImageInfo> LogList { get; set; }

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
			SetProperty(() => CurrentIndex, value, UpdateCurrentImage);
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
			SetProperty<ImgFile>(() => CurrentImgFile, value, UpdateCurrentImage);
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

	private void UpdateCurrentImage()
	{
		if (!TryLoadCurrentImage())
		{
			CurrentImgFile = null;
		}
	}

	private bool TryLoadCurrentImage()
	{
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
		string imagePath = "sprite/" + array[0].ToLower().Replace("\\", "/");
		if (!instance.NpkImgDIC.TryGetValue(imagePath, out UtImgFile value) || value == null)
		{
			return false;
		}
		string npkFilePath = instance.GetNpkFilePath(value);
		if (!ImagePacks.TryGetValue(npkFilePath, out List<ImagePack> value2))
		{
			value2 = NpkCoder.ReadNpk(npkFilePath);
			ImagePacks.Add(npkFilePath, value2);
		}
		ImagePack imagePack = value2.FirstOrDefault(it => it.Path == imagePath);
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
		ImagePacks = new Dictionary<string, List<ImagePack>>();
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
		if (hwndSource == null)
		{
			hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(win).Handle);
			hwndSource.AddHook(WindowMessageHook);
		}
		Task.Run((Action)InitializeDnfProcessMemory);
	}

	public void inits()
	{
		InitializeDnfProcessMemory();
	}

	private void InitializeDnfProcessMemory()
	{
		process = MemoryUtils.GetProcessByProcessName("dnf");
		if (process == null)
		{
			throw new Exception("没有找到 DNF 进程，请先启动客户端后再使用该功能。");
		}
		memoryUtils = new MemoryUtils(process.Id);
		IntPtr intPtr = memoryUtils.AllocMemory(1024);
		int value = 9527;
		IntPtr intPtr2 = memoryUtils.FindWindow(WindowGameImageExtract.WindowTitle);
		IntPtr intPtr3 = FindUser32ModuleAddress();
		IntPtr intPtr4 = GetExportAddress(intPtr3, "SendMessageA");
		memoryUtils.WriteInt(intPtr + 108, intPtr2.ToInt32());
		memoryUtils.WriteInt(intPtr + 100, value);
		byte[] array = new byte[31]
		{
			96, 106, 0, 84, 255, 53, 8, 1, 150, 15,
			255, 53, 0, 1, 150, 15, 232, 211, 142, 33,
			103, 97, 85, 139, 236, 93, 233, 81, 7, 134,
			241
		};
		memoryUtils.BuilderValue(array, 6, intPtr + 100);
		memoryUtils.BuilderValue(array, 12, intPtr + 108);
		memoryUtils.BuilderValue(array, 17, intPtr4 - (intPtr.ToInt32() + 21));
		memoryUtils.BuilderValue(array, 27, memoryUtils.GetMainModuleByProcessName("dnf") + 14419824 - (intPtr.ToInt32() + 31));
		memoryUtils.WriteByteArray(intPtr, array);
		IntPtr memoryAddress = memoryUtils.GetMemoryAddress("dnf", 28600396, 0, 16);
		Console.WriteLine("Vtableaddre:{0:x}", memoryAddress);
		Console.WriteLine("allo:{0:x}", intPtr.ToInt32());
		memoryUtils.WriteInt(memoryAddress, intPtr.ToInt32());
		IsStart = true;
	}

	public void Stop()
	{
		IsStart = false;
	}

	private IntPtr WindowMessageHook(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (message == 9527 && IsStart)
		{
			SelfStruct selfStruct = memoryUtils.ReadObject<SelfStruct>(wParam);
			string text = Encoding.Unicode.GetString(memoryUtils.ReadToBytes((IntPtr)selfStruct.addre11, 256));
			uint addre = selfStruct.addre12;
			string[] value = text.Split("\u0000");
			string text2 = string.Join("----", value);
			if (addre > 4095)
			{
				text2 += Encoding.Unicode.GetString(memoryUtils.ReadToBytes((IntPtr)selfStruct.addre12, 256));
			}
			if (!LogList.TryGetValue(text2, out ImageInfo value2))
			{
				value2 = new ImageInfo
				{
					CurrentValue = GetCurrentValues(selfStruct),
					AllValue = GetAllValues(selfStruct)
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

	private void ExtractImagePaths(SelfStruct data)
	{
		FieldInfo[] fields = typeof(SelfStruct).GetFields(BindingFlags.Instance | BindingFlags.Public);
		for (int i = 0; i < fields.Length; i++)
		{
			object value = fields[i].GetValue(data);
			if (value == null || i <= 0 || !(value is uint num) || num < 0 || num >= 4095)
			{
				continue;
			}
			string[] value2 = Encoding.Unicode.GetString(memoryUtils.ReadToBytes((IntPtr)(uint)fields[i - 1].GetValue(data), 2560)).Split("\u0000");
			string text = string.Join("----", value2);
			if (text.ToLower().Contains("/") || text.Contains("\\"))
			{
				if (!LogList.TryGetValue(text, out ImageInfo value3))
				{
					value3 = new ImageInfo
					{
						Name = fields[i - 1].Name,
						Value = fields[i - 1].GetValue(data),
						Test = GetCurrentValues(data)
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

	private string GetCurrentValues(SelfStruct data)
	{
		List<string> list = new List<string>();
		FieldInfo[] fields = typeof(SelfStruct).GetFields(BindingFlags.Instance | BindingFlags.Public);
		for (int i = 0; i < fields.Length; i++)
		{
			object value = fields[i].GetValue(data);
			if (value != null && i > 0 && value is uint num && num >= 0 && num < 4095)
			{
				list.Add(fields[i].Name + "：" + num);
			}
		}
		return "[" + string.Join(",", list) + "]";
	}

	private string GetAllValues(SelfStruct data)
	{
		List<string> list = new List<string>();
		FieldInfo[] fields = typeof(SelfStruct).GetFields(BindingFlags.Instance | BindingFlags.Public);
		for (int i = 0; i < fields.Length; i++)
		{
			object value = fields[i].GetValue(data);
			if (value != null)
			{
				list.Add($"{fields[i].Name}：{value}");
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
		hwndSource?.RemoveHook(WindowMessageHook);
		hwndSource?.Dispose();
	}

	private IntPtr FindUser32ModuleAddress()
	{
		IntPtr aaa = IntPtr.Zero;
		if (memoryUtils.QueryInformationProcess(out aaa) == 0)
		{
			int num = memoryUtils.ReadToInt(aaa + 12) + 12;
			for (int num2 = memoryUtils.ReadToInt((IntPtr)num); num2 != num; num2 = memoryUtils.ReadToInt((IntPtr)num2))
			{
				int num3 = memoryUtils.ReadToInt((IntPtr)num2 + 48);
				string text = Encoding.Unicode.GetString(memoryUtils.ReadToBytes((IntPtr)num3, 512));
				bool flag = text.Contains("USER32.DLL");
				bool flag2 = text.Contains("user32.dll");
				if (flag || flag2)
				{
					return (IntPtr)memoryUtils.ReadToInt((IntPtr)num2 + 24);
				}
			}
		}
		return IntPtr.Zero;
	}

	private IntPtr GetExportAddress(IntPtr moduleAddress, string functionName)
	{
		IMAGE_DOS_HEADER iMAGE_DOS_HEADER = memoryUtils.ReadObject<IMAGE_DOS_HEADER>(moduleAddress);
		IMAGE_NT_HEADER32 iMAGE_NT_HEADER = memoryUtils.ReadObject<IMAGE_NT_HEADER32>((IntPtr)(moduleAddress.ToInt32() + iMAGE_DOS_HEADER.e_lfanew));
		IMAGE_EXPORT_DIRECTORY iMAGE_EXPORT_DIRECTORY = memoryUtils.ReadObject<IMAGE_EXPORT_DIRECTORY>((IntPtr)(moduleAddress.ToInt32() + iMAGE_NT_HEADER.OptionalHeader.DataDirectory[0].VirtualAddress));
		for (int i = 0; i < iMAGE_EXPORT_DIRECTORY.NumberOfFunctions; i++)
		{
			int num = memoryUtils.ReadToInt((IntPtr)(moduleAddress.ToInt32() + (iMAGE_EXPORT_DIRECTORY.AddressOfNames + i * 4)));
			if (Encoding.ASCII.GetString(memoryUtils.ReadToBytes((IntPtr)(moduleAddress.ToInt32() + num), 256)).Split('\0')[0].Contains(functionName))
			{
				short num2 = memoryUtils.ReadToShort((IntPtr)(moduleAddress.ToInt32() + iMAGE_EXPORT_DIRECTORY.AddressOfNameOrdinals + i * 2));
				int num3 = memoryUtils.ReadToInt((IntPtr)(moduleAddress.ToInt32() + iMAGE_EXPORT_DIRECTORY.AddressOfFunctions + num2 * 4));
				return moduleAddress + num3;
			}
		}
		return IntPtr.Zero;
	}
}
