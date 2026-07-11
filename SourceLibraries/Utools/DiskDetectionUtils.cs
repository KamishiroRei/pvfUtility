using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Utools;

public static class DiskDetectionUtils
{
	private struct stYdBn7SgqeYcECW6oQ
	{
		public readonly uint NqI7D7qU1q;

		public readonly long Q1d71FOBCp;

		public readonly long Qqd7sDM9Rj;
	}

	private struct KvJs5U7id2ymWiVDlZA
	{
		public readonly uint fU872rwL8Z;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		public readonly stYdBn7SgqeYcECW6oQ[] QHj752wi3K;
	}

	private struct n9Vrk97XSVEoLcZHli4
	{
		public uint dvx7WGi9lf;

		public uint v7w7CZEE3T;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
		public readonly byte[] Dku7UmwGPm;
	}

	private struct uN1ryB7LHFDaoCkjuYE
	{
		public readonly uint s9a7BwVcej;

		public readonly uint K847aOtw1N;

		[MarshalAs(UnmanagedType.U1)]
		public readonly bool pZC7m5svfb;
	}

	private struct tHWFqQ7loGE5YkalGhR
	{
		public ushort DBy7v6STbO;

		public ushort XUI7NWVQfE;

		public readonly byte UEI7T8ZfKS;

		public readonly byte xLr7kn0Lmj;

		public readonly byte kOg7cbisg9;

		public readonly byte PDV7RCJOuF;

		public uint q9a7gciR88;

		public uint wlP7xsQmXX;

		public readonly uint awQ731YGBr;

		public IntPtr Xbx7EulQcE;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] bhi7w5XvE4;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] Cxx7eP7KcW;
	}

	private struct lTCE4R7HojYoNy59XTZ
	{
		public tHWFqQ7loGE5YkalGhR zKR7fQBwSW;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		public ushort[] sTG7ICVM0i;
	}

	public static bool IsAdministrator()
	{
		WindowsIdentity current = WindowsIdentity.GetCurrent();
		if (current == null)
		{
			return false;
		}
		return new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator);
	}

	public static DriveInfoExtended DetectFixedDrive(string driveName, QueryType queryType = QueryType.SeekPenalty, bool useFallbackQuery = true)
	{
		DriveInfoExtended result = new DriveInfoExtended();
		DriveInfo driveInfo = new DriveInfo(driveName);
		if (driveInfo.DriveType == DriveType.Fixed && driveInfo.IsReady)
		{
			DriveInfoExtended driveInfoExtended = new DriveInfoExtended
			{
				DriveFormat = driveInfo.DriveFormat,
				VolumeLabel = driveInfo.VolumeLabel,
				Name = driveInfo.Name,
				UncPath = NetworkDrivePathResolver.ToUncPath(driveInfo.Name),
				DriveType = driveInfo.DriveType,
				AvailableFreeSpace = driveInfo.AvailableFreeSpace,
				TotalSize = driveInfo.TotalSize,
				TotalFreeSpace = driveInfo.TotalFreeSpace,
				RootDirectory = driveInfo.RootDirectory,
				DriveLetter = driveInfo.Name.Substring(0, 1).ToCharArray()[0]
			};
			int num = G6xo7CI6s(driveInfoExtended.DriveLetter);
			if (num != -1)
			{
				driveInfoExtended.Id = num;
				if (queryType == QueryType.SeekPenalty)
				{
					driveInfoExtended.HardwareType = DetectHardwareTypeBySeekPenalty(num);
				}
				else if (IsAdministrator())
				{
					driveInfoExtended.HardwareType = DetectHardwareTypeByRotationRate(num);
				}
				else
				{
					if (!useFallbackQuery)
					{
						throw new SecurityException("DetectHardwareTypeBySeekPenalty needs administrative access.");
					}
					driveInfoExtended.HardwareType = DetectHardwareTypeBySeekPenalty(num);
				}
				if (driveInfoExtended.HardwareType != HardwareType.Unknown)
				{
					result = driveInfoExtended;
				}
			}
		}
		return result;
	}

	public static List<DriveInfoExtended> DetectFixedDrives(QueryType queryType = QueryType.SeekPenalty, bool useFallbackQuery = true)
	{
		List<DriveInfoExtended> list = new List<DriveInfoExtended>();
		DriveInfo[] drives = DriveInfo.GetDrives();
		foreach (DriveInfo driveInfo in drives)
		{
			if (driveInfo.DriveType != DriveType.Fixed || !driveInfo.IsReady)
			{
				continue;
			}
			DriveInfoExtended driveInfoExtended = new DriveInfoExtended
			{
				DriveFormat = driveInfo.DriveFormat,
				VolumeLabel = driveInfo.VolumeLabel,
				Name = driveInfo.Name,
				UncPath = NetworkDrivePathResolver.ToUncPath(driveInfo.Name),
				DriveType = driveInfo.DriveType,
				AvailableFreeSpace = driveInfo.AvailableFreeSpace,
				TotalSize = driveInfo.TotalSize,
				TotalFreeSpace = driveInfo.TotalFreeSpace,
				RootDirectory = driveInfo.RootDirectory,
				DriveLetter = driveInfo.Name.Substring(0, 1).ToCharArray()[0]
			};
			int num = G6xo7CI6s(driveInfoExtended.DriveLetter);
			if (num == -1)
			{
				continue;
			}
			driveInfoExtended.Id = num;
			if (queryType == QueryType.SeekPenalty)
			{
				driveInfoExtended.HardwareType = DetectHardwareTypeBySeekPenalty(num);
			}
			else if (IsAdministrator())
			{
				driveInfoExtended.HardwareType = DetectHardwareTypeByRotationRate(num);
			}
			else
			{
				if (!useFallbackQuery)
				{
					throw new SecurityException("DetectHardwareTypeBySeekPenalty needs administrative access.");
				}
				driveInfoExtended.HardwareType = DetectHardwareTypeBySeekPenalty(num);
			}
			if (driveInfoExtended.HardwareType != HardwareType.Unknown)
			{
				list.Add(driveInfoExtended);
			}
		}
		return list;
	}

	public static DriveInfoExtended DetectDrive(string driveName, QueryType queryType = QueryType.SeekPenalty, bool useFallbackQuery = true)
	{
		DriveInfoExtended result = new DriveInfoExtended();
		DriveInfo driveInfo = new DriveInfo(driveName);
		if (driveInfo.DriveType == DriveType.Fixed)
		{
			if (driveInfo.IsReady)
			{
				DriveInfoExtended driveInfoExtended = new DriveInfoExtended
				{
					DriveFormat = driveInfo.DriveFormat,
					VolumeLabel = driveInfo.VolumeLabel,
					Name = driveInfo.Name,
					UncPath = NetworkDrivePathResolver.ToUncPath(driveInfo.Name),
					DriveType = driveInfo.DriveType,
					AvailableFreeSpace = driveInfo.AvailableFreeSpace,
					TotalSize = driveInfo.TotalSize,
					TotalFreeSpace = driveInfo.TotalFreeSpace,
					RootDirectory = driveInfo.RootDirectory,
					DriveLetter = driveInfo.Name.Substring(0, 1).ToCharArray()[0]
				};
				int num = G6xo7CI6s(driveInfoExtended.DriveLetter);
				if (num != -1)
				{
					driveInfoExtended.Id = num;
					if (queryType == QueryType.SeekPenalty)
					{
						driveInfoExtended.HardwareType = DetectHardwareTypeBySeekPenalty(num);
					}
					else if (IsAdministrator())
					{
						driveInfoExtended.HardwareType = DetectHardwareTypeByRotationRate(num);
					}
					else
					{
						if (!useFallbackQuery)
						{
							throw new SecurityException("DetectHardwareTypeBySeekPenalty needs administrative access.");
						}
						driveInfoExtended.HardwareType = DetectHardwareTypeBySeekPenalty(num);
					}
					result = driveInfoExtended;
				}
			}
		}
		else if (driveInfo.IsReady)
		{
			result = new DriveInfoExtended
			{
				DriveFormat = driveInfo.DriveFormat,
				VolumeLabel = driveInfo.VolumeLabel,
				Name = driveInfo.Name,
				UncPath = NetworkDrivePathResolver.ToUncPath(driveInfo.Name),
				DriveType = driveInfo.DriveType,
				AvailableFreeSpace = driveInfo.AvailableFreeSpace,
				TotalSize = driveInfo.TotalSize,
				TotalFreeSpace = driveInfo.TotalFreeSpace,
				RootDirectory = driveInfo.RootDirectory,
				DriveLetter = driveInfo.Name.Substring(0, 1).ToCharArray()[0],
				HardwareType = HardwareType.Unknown,
				Id = -1
			};
		}
		return result;
	}

	public static List<DriveInfoExtended> DetectDrives(QueryType queryType = QueryType.SeekPenalty, bool useFallbackQuery = true)
	{
		List<DriveInfoExtended> list = new List<DriveInfoExtended>();
		DriveInfo[] drives = DriveInfo.GetDrives();
		foreach (DriveInfo driveInfo in drives)
		{
			if (driveInfo.DriveType == DriveType.Fixed)
			{
				if (!driveInfo.IsReady)
				{
					continue;
				}
				DriveInfoExtended driveInfoExtended = new DriveInfoExtended
				{
					DriveFormat = driveInfo.DriveFormat,
					VolumeLabel = driveInfo.VolumeLabel,
					Name = driveInfo.Name,
					UncPath = NetworkDrivePathResolver.ToUncPath(driveInfo.Name),
					DriveType = driveInfo.DriveType,
					AvailableFreeSpace = driveInfo.AvailableFreeSpace,
					TotalSize = driveInfo.TotalSize,
					TotalFreeSpace = driveInfo.TotalFreeSpace,
					RootDirectory = driveInfo.RootDirectory,
					DriveLetter = driveInfo.Name.Substring(0, 1).ToCharArray()[0]
				};
				int num = G6xo7CI6s(driveInfoExtended.DriveLetter);
				if (num == -1)
				{
					continue;
				}
				driveInfoExtended.Id = num;
				if (queryType == QueryType.SeekPenalty)
				{
					driveInfoExtended.HardwareType = DetectHardwareTypeBySeekPenalty(num);
				}
				else if (IsAdministrator())
				{
					driveInfoExtended.HardwareType = DetectHardwareTypeByRotationRate(num);
				}
				else
				{
					if (!useFallbackQuery)
					{
						throw new SecurityException("DetectHardwareTypeBySeekPenalty needs administrative access.");
					}
					driveInfoExtended.HardwareType = DetectHardwareTypeBySeekPenalty(num);
				}
				list.Add(driveInfoExtended);
			}
			else if (driveInfo.IsReady)
			{
				DriveInfoExtended item = new DriveInfoExtended
				{
					DriveFormat = driveInfo.DriveFormat,
					VolumeLabel = driveInfo.VolumeLabel,
					Name = driveInfo.Name,
					UncPath = NetworkDrivePathResolver.ToUncPath(driveInfo.Name),
					DriveType = driveInfo.DriveType,
					AvailableFreeSpace = driveInfo.AvailableFreeSpace,
					TotalSize = driveInfo.TotalSize,
					TotalFreeSpace = driveInfo.TotalFreeSpace,
					RootDirectory = driveInfo.RootDirectory,
					DriveLetter = driveInfo.Name.Substring(0, 1).ToCharArray()[0],
					HardwareType = HardwareType.Unknown,
					Id = -1
				};
				list.Add(item);
			}
		}
		return list;
	}

	[DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool c2CZ6wc0x(SafeFileHandle P_0, uint P_1, IntPtr P_2, uint P_3, ref KvJs5U7id2ymWiVDlZA P_4, uint P_5, out uint P_6, IntPtr P_7);

	private static int G6xo7CI6s(char P_0)
	{
		if (new DriveInfo(P_0.ToString()).DriveType != DriveType.Fixed)
		{
			throw new DetectionFailedException(string.Format("This drive is not fixed drive: {0}", P_0));
		}
		SafeFileHandle safeFileHandle = d5ODKU7Mc("\\\\.\\" + P_0 + ":", 0u, 3u, IntPtr.Zero, 3u, 128u, IntPtr.Zero);
		if (safeFileHandle == null || safeFileHandle.IsInvalid)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			throw new DetectionFailedException(string.Format("Could not detect Disk Id of {0}", P_0), new Win32Exception(lastWin32Error));
		}
		uint num = Ub01kOBy7(86u, 0u, 0u, 0u);
		KvJs5U7id2ymWiVDlZA structure = default(KvJs5U7id2ymWiVDlZA);
		uint num3;
		bool num2 = c2CZ6wc0x(safeFileHandle, num, IntPtr.Zero, 0u, ref structure, (uint)Marshal.SizeOf(structure), out num3, IntPtr.Zero);
		safeFileHandle.Close();
		if (!num2)
		{
			int lastWin32Error2 = Marshal.GetLastWin32Error();
			if (lastWin32Error2 != 234 || structure.QHj752wi3K.Length < 1)
			{
				throw new DetectionFailedException(string.Format("Could not detect Disk Id of {0}", P_0), new Win32Exception(lastWin32Error2));
			}
		}
		return (int)structure.QHj752wi3K[0].NqI7D7qU1q;
	}

	public static HardwareType DetectHardwareTypeBySeekPenalty(char driveLetter)
	{
		try
		{
			return DetectHardwareTypeBySeekPenalty(G6xo7CI6s(driveLetter));
		}
		catch (DetectionFailedException)
		{
			return HardwareType.Unknown;
		}
	}

	public static HardwareType DetectHardwareTypeBySeekPenalty(int driveId)
	{
		string text = "\\\\.\\PhysicalDrive" + driveId;
		try
		{
			return a2P2OGDGr(text) ? HardwareType.Hdd : HardwareType.Ssd;
		}
		catch (DetectionFailedException)
		{
			return HardwareType.Unknown;
		}
	}

	public static HardwareType DetectHardwareTypeByRotationRate(char driveLetter)
	{
		try
		{
			return DetectHardwareTypeByRotationRate(G6xo7CI6s(driveLetter));
		}
		catch (DetectionFailedException)
		{
			return HardwareType.Unknown;
		}
	}

	public static HardwareType DetectHardwareTypeByRotationRate(int driveId)
	{
		string text = "\\\\.\\PhysicalDrive" + driveId;
		try
		{
			return GiG59qUUW(text) ? HardwareType.Hdd : HardwareType.Ssd;
		}
		catch (DetectionFailedException)
		{
			return HardwareType.Unknown;
		}
	}

	[DllImport("mpr.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern int WNetGetConnection([MarshalAs(UnmanagedType.LPTStr)] string localName, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName, ref int length);

	private static object ig3SRTgsT(string P_0)
	{
		int length = 200;
		StringBuilder remoteName = new StringBuilder(length);
		WNetGetConnection("Z:", remoteName, ref length);
		int length2 = 255;
		StringBuilder stringBuilder = new StringBuilder(length2);
		WNetGetConnection("Z:", stringBuilder, ref length2);
		return stringBuilder.ToString();
	}

	[DllImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true)]
	private static extern SafeFileHandle d5ODKU7Mc([MarshalAs(UnmanagedType.LPWStr)] string P_0, uint P_1, uint P_2, IntPtr P_3, uint P_4, uint P_5, IntPtr P_6);

	private static uint Ub01kOBy7(uint P_0, uint P_1, uint P_2, uint P_3)
	{
		return (P_0 << 16) | (P_3 << 14) | (P_1 << 2) | P_2;
	}

	[DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool ucbsZDots(SafeFileHandle P_0, uint P_1, ref n9Vrk97XSVEoLcZHli4 P_2, uint P_3, ref uN1ryB7LHFDaoCkjuYE P_4, uint P_5, out uint P_6, IntPtr P_7);

	[DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool A22iabIMc(SafeFileHandle P_0, uint P_1, ref lTCE4R7HojYoNy59XTZ P_2, uint P_3, ref lTCE4R7HojYoNy59XTZ P_4, uint P_5, out uint P_6, IntPtr P_7);

	private static bool a2P2OGDGr(string P_0)
	{
		SafeFileHandle safeFileHandle = d5ODKU7Mc(P_0, 0u, 3u, IntPtr.Zero, 3u, 128u, IntPtr.Zero);
		if (safeFileHandle == null || safeFileHandle.IsInvalid)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			throw new DetectionFailedException(string.Format("Could not detect SeekPenalty of {0}", P_0), new Win32Exception(lastWin32Error));
		}
		uint num = Ub01kOBy7(45u, 1280u, 0u, 0u);
		n9Vrk97XSVEoLcZHli4 structure = new n9Vrk97XSVEoLcZHli4
		{
			dvx7WGi9lf = 7u,
			v7w7CZEE3T = 0u
		};
		uN1ryB7LHFDaoCkjuYE structure2 = default(uN1ryB7LHFDaoCkjuYE);
		uint num3;
		bool num2 = ucbsZDots(safeFileHandle, num, ref structure, (uint)Marshal.SizeOf(structure), ref structure2, (uint)Marshal.SizeOf(structure2), out num3, IntPtr.Zero);
		safeFileHandle.Close();
		if (!num2)
		{
			int lastWin32Error2 = Marshal.GetLastWin32Error();
			throw new DetectionFailedException(string.Format("Could not detect SeekPenalty of {0}", P_0), new Win32Exception(lastWin32Error2));
		}
		if (!structure2.pZC7m5svfb)
		{
			return false;
		}
		return true;
	}

	private static bool GiG59qUUW(string P_0)
	{
		SafeFileHandle safeFileHandle = d5ODKU7Mc(P_0, 3221225472u, 3u, IntPtr.Zero, 3u, 128u, IntPtr.Zero);
		if (safeFileHandle == null || safeFileHandle.IsInvalid)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			throw new DetectionFailedException(string.Format("Could not detect NominalMediaRotationRate of {0}", P_0), new Win32Exception(lastWin32Error));
		}
		uint num = Ub01kOBy7(4u, 1035u, 0u, 3u);
		lTCE4R7HojYoNy59XTZ structure = new lTCE4R7HojYoNy59XTZ
		{
			sTG7ICVM0i = new ushort[256]
		};
		structure.zKR7fQBwSW.DBy7v6STbO = (ushort)Marshal.SizeOf(structure.zKR7fQBwSW);
		structure.zKR7fQBwSW.XUI7NWVQfE = 2;
		structure.zKR7fQBwSW.q9a7gciR88 = (uint)(structure.sTG7ICVM0i.Length * 2);
		structure.zKR7fQBwSW.wlP7xsQmXX = 3u;
		structure.zKR7fQBwSW.Xbx7EulQcE = Marshal.OffsetOf(typeof(lTCE4R7HojYoNy59XTZ), "data");
		structure.zKR7fQBwSW.bhi7w5XvE4 = new byte[8];
		structure.zKR7fQBwSW.Cxx7eP7KcW = new byte[8];
		structure.zKR7fQBwSW.Cxx7eP7KcW[6] = 236;
		uint num3;
		bool num2 = A22iabIMc(safeFileHandle, num, ref structure, (uint)Marshal.SizeOf(structure), ref structure, (uint)Marshal.SizeOf(structure), out num3, IntPtr.Zero);
		safeFileHandle.Close();
		if (!num2)
		{
			int lastWin32Error2 = Marshal.GetLastWin32Error();
			throw new DetectionFailedException(string.Format("Could not detect NominalMediaRotationRate of {0}", P_0), new Win32Exception(lastWin32Error2));
		}
		if (structure.sTG7ICVM0i[217] == 1)
		{
			return false;
		}
		return true;
	}
}
