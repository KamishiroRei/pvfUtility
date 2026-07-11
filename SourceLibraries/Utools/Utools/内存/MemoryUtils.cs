using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Utools.内存;

public class MemoryUtils
{
	public enum PAGE_EXECUTE_ENUM
	{
		PAGE_EXECUTE_READ = 0x20,
		PAGE_EXECUTE_READWRITE = 0x40
	}

	private IntPtr b5l7ZHCOnp;

	private int fcw7oHkjdX;

	public MemoryUtils(int pid)
	{
		b5l7ZHCOnp = IntPtr.Zero;
		fcw7oHkjdX = pid;
		b5l7ZHCOnp = iI57Qu5iqf(2035711, false, pid);
	}

	~MemoryUtils()
	{
		Qhp7rCX8In(b5l7ZHCOnp);
	}

	public byte[] ReadToBytes(IntPtr address, int size)
	{
		byte[] array = new byte[size];
		ybq7tdiU9E(b5l7ZHCOnp, address, array, size, IntPtr.Zero);
		return array;
	}

	public T ReadObject<T>(IntPtr address) where T : struct
	{
		byte[] array = ReadToBytes(address, Marshal.SizeOf(typeof(T)));
		IntPtr intPtr = Marshal.AllocHGlobal(array.Length);
		Marshal.Copy(array, 0, intPtr, array.Length);
		T result = (T)Marshal.PtrToStructure(intPtr, typeof(T));
		Marshal.FreeHGlobal(intPtr);
		return result;
	}

	public char ReadToChar(IntPtr address)
	{
		return BitConverter.ToChar(ReadToBytes(address, 2), 0);
	}

	public short ReadToShort(IntPtr address)
	{
		return BitConverter.ToInt16(ReadToBytes(address, 2), 0);
	}

	public int ReadToInt(IntPtr address)
	{
		return BitConverter.ToInt32(ReadToBytes(address, 4), 0);
	}

	public long ReadToLong(IntPtr address)
	{
		return BitConverter.ToInt64(ReadToBytes(address, 8), 0);
	}

	public float ReadToFloat(IntPtr address)
	{
		return BitConverter.ToSingle(ReadToBytes(address, 4), 0);
	}

	public double ReadToDouble(IntPtr address)
	{
		return BitConverter.ToDouble(ReadToBytes(address, 8), 0);
	}

	public string ReadToString(IntPtr address, int stringSize)
	{
		return BitConverter.ToString(ReadToBytes(address, stringSize));
	}

	public int QueryInformationProcess(out IntPtr aaa)
	{
		int ReturnLength = 0;
		return NtQueryInformationProcess(b5l7ZHCOnp, 26, out aaa, 8, out ReturnLength);
	}

	public IntPtr AllocMemory(int size)
	{
		return hUb7jlUYYJ(b5l7ZHCOnp, IntPtr.Zero, size, 4096u, PAGE_EXECUTE_ENUM.PAGE_EXECUTE_READWRITE);
	}

	public bool WriteByteArray(IntPtr address, byte[] byteData)
	{
		return HmM7po6LpL(b5l7ZHCOnp, address, byteData, byteData.Length, IntPtr.Zero);
	}

	public bool WriteChar(IntPtr address, char value)
	{
		return WriteByteArray(address, BitConverter.GetBytes(value));
	}

	public bool WriteShort(IntPtr address, short value)
	{
		return WriteByteArray(address, BitConverter.GetBytes(value));
	}

	public bool WriteInt(IntPtr address, int value)
	{
		return WriteByteArray(address, BitConverter.GetBytes(value));
	}

	public bool WriteLong(IntPtr address, long value)
	{
		return WriteByteArray(address, BitConverter.GetBytes(value));
	}

	public bool WriteFloat(IntPtr address, float value)
	{
		return WriteByteArray(address, BitConverter.GetBytes(value));
	}

	public bool WriteDouble(IntPtr address, double value)
	{
		return WriteByteArray(address, BitConverter.GetBytes(value));
	}

	public bool WriteString(IntPtr address, string value)
	{
		return WriteByteArray(address, Encoding.Default.GetBytes(value));
	}

	public static int GetPidByProcessName(string processName)
	{
		return GetProcessByProcessName(processName)?.Id ?? 0;
	}

	public static Process GetProcessByProcessName(string processName)
	{
		if (processName.Contains(".exe"))
		{
			processName = processName.Replace(".exe", "");
		}
		Process[] processesByName = Process.GetProcessesByName(processName);
		if (processesByName.Length != 0)
		{
			return processesByName[0];
		}
		return null;
	}

	public IntPtr FindWindow(string title)
	{
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			if (process.MainWindowTitle.IndexOf(title) != -1)
			{
				return process.MainWindowHandle;
			}
		}
		return IntPtr.Zero;
	}

	public IntPtr GetModuleBaseAddress(string moduleName)
	{
		Process processById = Process.GetProcessById(fcw7oHkjdX);
		IntPtr result = default(IntPtr);
		for (int i = 0; i < processById.Modules.Count; i++)
		{
			ProcessModule processModule = processById.Modules[i];
			if (processModule.ModuleName == moduleName)
			{
				return processModule.BaseAddress;
			}
		}
		return result;
	}

	public IntPtr GetMemoryAddress(string moduleName, params int[] offsetArray)
	{
		if (offsetArray == null || offsetArray.Length == 0)
		{
			throw new Exception("至少需要一个偏移");
		}
		IntPtr intPtr = IntPtr.Zero;
		IntPtr intPtr2 = GetModuleBaseAddress(moduleName);
		for (int i = 0; i < offsetArray.Length; i++)
		{
			intPtr = intPtr2 + offsetArray[i];
			if (i != offsetArray.Length)
			{
				intPtr2 = (IntPtr)ReadToInt(intPtr);
			}
		}
		return intPtr;
	}

	public static IntPtr GetWindowHwndByProcessName(string processName)
	{
		IntPtr intPtr = GetProcessByProcessName(processName)?.MainWindowHandle ?? IntPtr.Zero;
		if (intPtr == IntPtr.Zero)
		{
			throw new Exception("没有找到[" + processName + "]进程..");
		}
		return intPtr;
	}

	public void BuilderValue(byte[] ShellCode, int Index, IntPtr Value)
	{
		Array.Copy(BitConverter.GetBytes(Value.ToInt32()), 0, ShellCode, Index, 4);
	}

	public void BuilderValue(byte[] ShellCode, int Index, int Value)
	{
		Array.Copy(BitConverter.GetBytes(Value), 0, ShellCode, Index, 4);
	}

	public IntPtr GetMainModuleByProcessName(string processName)
	{
		Process[] processesByName = Process.GetProcessesByName(processName);
		int num = 0;
		if (num < processesByName.Length)
		{
			return processesByName[num].MainModule.BaseAddress;
		}
		return IntPtr.Zero;
	}

	public static int ConvertFrom16To10(string value)
	{
		return Convert.ToInt32(value, 16);
	}

	public static int ConvertFrom16To10(int value)
	{
		return Convert.ToInt32(Convert.ToString(value, 10));
	}

	public static string ConvertFrom10To16(int value)
	{
		return value.ToString("x");
	}

	public static string ConvertFrom10To16(IntPtr value)
	{
		return value.ToString("x");
	}

	public static string ConvertFrom16Or10To2(int value)
	{
		return Convert.ToString(value, 2);
	}

	public static int ConvertFrom2To10(string value)
	{
		return Convert.ToInt32(value, 2);
	}

	[DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
	private static extern bool ybq7tdiU9E(IntPtr P_0, IntPtr P_1, byte[] P_2, int P_3, IntPtr P_4);

	[DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
	private static extern bool HmM7po6LpL(IntPtr P_0, IntPtr P_1, byte[] P_2, int P_3, IntPtr P_4);

	[DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
	private static extern IntPtr iI57Qu5iqf(int P_0, bool P_1, int P_2);

	[DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
	private static extern void Qhp7rCX8In(IntPtr P_0);

	[DllImport("Ntdll.dll")]
	public static extern int NtQueryInformationProcess(IntPtr hProcess, int ProcessInformationClass, out IntPtr ProcessInformation, int ProcessInformationLength, out int ReturnLength);

	[DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
	private static extern IntPtr MPn7qEvksi(IntPtr P_0, string P_1);

	[DllImport("kernel32.dll", EntryPoint = "VirtualAllocEx")]
	private static extern IntPtr hUb7jlUYYJ(IntPtr P_0, IntPtr P_1, int P_2, uint P_3, PAGE_EXECUTE_ENUM P_4);
}
