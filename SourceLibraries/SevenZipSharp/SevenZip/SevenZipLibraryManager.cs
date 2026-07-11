using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SevenZip;

internal static class SevenZipLibraryManager
{
	private static readonly object _syncRoot = new object();

	private static string _libraryFileName;

	private static IntPtr _modulePtr;

	private static LibraryFeature? _features;

	private static Dictionary<object, Dictionary<InArchiveFormat, IInArchive>> _inArchives;

	private static Dictionary<object, Dictionary<OutArchiveFormat, IOutArchive>> _outArchives;

	private static int _totalUsers;

	private static bool? _modifyCapable;

	private static readonly string Namespace = Assembly.GetExecutingAssembly().GetManifestResourceNames()[0].Split('.')[0];

	public static bool ModifyCapable
	{
		get
		{
			lock (_syncRoot)
			{
				if (!_modifyCapable.HasValue)
				{
					if (_libraryFileName == null)
					{
						_libraryFileName = DetermineLibraryFilePath();
					}
					FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(_libraryFileName);
					_modifyCapable = versionInfo.FileMajorPart >= 9;
				}
				return _modifyCapable.Value;
			}
		}
	}

	public static LibraryFeature CurrentLibraryFeatures
	{
		get
		{
			lock (_syncRoot)
			{
				if (_features.HasValue && _features.HasValue)
				{
					return _features.Value;
				}
				_features = LibraryFeature.None;
				using (MemoryStream outStream = new MemoryStream())
				{
					ExtractionBenchmark("Test.lzma.7z", outStream, ref _features, LibraryFeature.Extract7z);
					ExtractionBenchmark("Test.lzma2.7z", outStream, ref _features, LibraryFeature.Extract7zLZMA2);
					int num = 0;
					if (ExtractionBenchmark("Test.bzip2.7z", outStream, ref _features, _features.Value))
					{
						num++;
					}
					if (ExtractionBenchmark("Test.ppmd.7z", outStream, ref _features, _features.Value))
					{
						num++;
						if (num == 2 && ((uint?)_features & 1u) != 0 && ((uint?)_features & 2u) != 0)
						{
							_features |= LibraryFeature.Extract7zAll;
						}
					}
					ExtractionBenchmark("Test.rar", outStream, ref _features, LibraryFeature.ExtractRar);
					ExtractionBenchmark("Test.tar", outStream, ref _features, LibraryFeature.ExtractTar);
					ExtractionBenchmark("Test.txt.bz2", outStream, ref _features, LibraryFeature.ExtractBzip2);
					ExtractionBenchmark("Test.txt.gz", outStream, ref _features, LibraryFeature.ExtractGzip);
					ExtractionBenchmark("Test.txt.xz", outStream, ref _features, LibraryFeature.ExtractXz);
					ExtractionBenchmark("Test.zip", outStream, ref _features, LibraryFeature.ExtractZip);
				}
				using (MemoryStream memoryStream = new MemoryStream())
				{
					memoryStream.Write(Encoding.UTF8.GetBytes("Test"), 0, 4);
					using MemoryStream outStream2 = new MemoryStream();
					CompressionBenchmark(memoryStream, outStream2, OutArchiveFormat.SevenZip, CompressionMethod.Lzma, ref _features, LibraryFeature.Compress7z);
					CompressionBenchmark(memoryStream, outStream2, OutArchiveFormat.SevenZip, CompressionMethod.Lzma2, ref _features, LibraryFeature.Compress7zLZMA2);
					int num2 = 0;
					if (CompressionBenchmark(memoryStream, outStream2, OutArchiveFormat.SevenZip, CompressionMethod.BZip2, ref _features, _features.Value))
					{
						num2++;
					}
					if (CompressionBenchmark(memoryStream, outStream2, OutArchiveFormat.SevenZip, CompressionMethod.Ppmd, ref _features, _features.Value))
					{
						num2++;
						if (num2 == 2 && ((uint?)_features & 0x200u) != 0 && ((uint?)_features & 0x400u) != 0)
						{
							_features |= LibraryFeature.Compress7zAll;
						}
					}
					CompressionBenchmark(memoryStream, outStream2, OutArchiveFormat.Zip, CompressionMethod.Default, ref _features, LibraryFeature.CompressZip);
					CompressionBenchmark(memoryStream, outStream2, OutArchiveFormat.BZip2, CompressionMethod.Default, ref _features, LibraryFeature.CompressBzip2);
					CompressionBenchmark(memoryStream, outStream2, OutArchiveFormat.GZip, CompressionMethod.Default, ref _features, LibraryFeature.CompressGzip);
					CompressionBenchmark(memoryStream, outStream2, OutArchiveFormat.Tar, CompressionMethod.Default, ref _features, LibraryFeature.CompressTar);
					CompressionBenchmark(memoryStream, outStream2, OutArchiveFormat.XZ, CompressionMethod.Default, ref _features, LibraryFeature.CompressXz);
				}
				if (ModifyCapable && (_features.Value & LibraryFeature.Compress7z) != LibraryFeature.None)
				{
					_features |= LibraryFeature.Modify;
				}
				return _features.Value;
			}
		}
	}

	private static string DetermineLibraryFilePath()
	{
		if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["7zLocation"]))
		{
			return ConfigurationManager.AppSettings["7zLocation"];
		}
		if (string.IsNullOrEmpty(Assembly.GetExecutingAssembly().Location))
		{
			return null;
		}
		return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Environment.Is64BitProcess ? "7z64.dll" : "7z.dll");
	}

	private static void InitUserInFormat(object user, InArchiveFormat format)
	{
		if (!_inArchives.ContainsKey(user))
		{
			_inArchives.Add(user, new Dictionary<InArchiveFormat, IInArchive>());
		}
		if (!_inArchives[user].ContainsKey(format))
		{
			_inArchives[user].Add(format, null);
			_totalUsers++;
		}
	}

	private static void InitUserOutFormat(object user, OutArchiveFormat format)
	{
		if (!_outArchives.ContainsKey(user))
		{
			_outArchives.Add(user, new Dictionary<OutArchiveFormat, IOutArchive>());
		}
		if (!_outArchives[user].ContainsKey(format))
		{
			_outArchives[user].Add(format, null);
			_totalUsers++;
		}
	}

	private static void Init()
	{
		_inArchives = new Dictionary<object, Dictionary<InArchiveFormat, IInArchive>>();
		_outArchives = new Dictionary<object, Dictionary<OutArchiveFormat, IOutArchive>>();
	}

	public static void LoadLibrary(object user, Enum format)
	{
		lock (_syncRoot)
		{
			if (_inArchives == null || _outArchives == null)
			{
				Init();
			}
			if (_modulePtr == IntPtr.Zero)
			{
				if (_libraryFileName == null)
				{
					_libraryFileName = DetermineLibraryFilePath();
				}
				if (!File.Exists(_libraryFileName))
				{
					throw new SevenZipLibraryException("DLL file does not exist.");
				}
				if ((_modulePtr = NativeMethods.LoadLibrary(_libraryFileName)) == IntPtr.Zero)
				{
					throw new SevenZipLibraryException("failed to load library from \"" + _libraryFileName + "\".");
				}
				if (NativeMethods.GetProcAddress(_modulePtr, "GetHandlerProperty") == IntPtr.Zero)
				{
					NativeMethods.FreeLibrary(_modulePtr);
					throw new SevenZipLibraryException("library is invalid.");
				}
			}
			if (format is InArchiveFormat format2)
			{
				InitUserInFormat(user, format2);
				return;
			}
			if (format is OutArchiveFormat format3)
			{
				InitUserOutFormat(user, format3);
				return;
			}
			throw new ArgumentException($"Enum {format} is not a valid archive format attribute!");
		}
	}

	private static string GetResourceString(string str)
	{
		return Namespace + ".arch." + str;
	}

	private static bool ExtractionBenchmark(string archiveFileName, Stream outStream, ref LibraryFeature? features, LibraryFeature testedFeature)
	{
		Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetResourceString(archiveFileName));
		try
		{
			using SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(manifestResourceStream);
			sevenZipExtractor.ExtractFile(0, outStream);
		}
		catch (Exception)
		{
			return false;
		}
		features |= testedFeature;
		return true;
	}

	private static bool CompressionBenchmark(Stream inStream, Stream outStream, OutArchiveFormat format, CompressionMethod method, ref LibraryFeature? features, LibraryFeature testedFeature)
	{
		try
		{
			SevenZipCompressor sevenZipCompressor = new SevenZipCompressor
			{
				ArchiveFormat = format,
				CompressionMethod = method
			};
			sevenZipCompressor.CompressStream(inStream, outStream);
		}
		catch (Exception)
		{
			return false;
		}
		features |= testedFeature;
		return true;
	}

	public static void FreeLibrary(object user, Enum format)
	{
		lock (_syncRoot)
		{
			if (!(_modulePtr != IntPtr.Zero))
			{
				return;
			}
			if (format is InArchiveFormat key && _inArchives != null && _inArchives.ContainsKey(user) && _inArchives[user].ContainsKey(key) && _inArchives[user][key] != null)
			{
				try
				{
					Marshal.ReleaseComObject(_inArchives[user][key]);
				}
				catch (InvalidComObjectException)
				{
				}
				_inArchives[user].Remove(key);
				_totalUsers--;
				if (_inArchives[user].Count == 0)
				{
					_inArchives.Remove(user);
				}
			}
			if (format is OutArchiveFormat key2 && _outArchives != null && _outArchives.ContainsKey(user) && _outArchives[user].ContainsKey(key2) && _outArchives[user][key2] != null)
			{
				try
				{
					Marshal.ReleaseComObject(_outArchives[user][key2]);
				}
				catch (InvalidComObjectException)
				{
				}
				_outArchives[user].Remove(key2);
				_totalUsers--;
				if (_outArchives[user].Count == 0)
				{
					_outArchives.Remove(user);
				}
			}
			if ((_inArchives == null || _inArchives.Count == 0) && (_outArchives == null || _outArchives.Count == 0))
			{
				_inArchives = null;
				_outArchives = null;
				if (_totalUsers == 0)
				{
					NativeMethods.FreeLibrary(_modulePtr);
					_modulePtr = IntPtr.Zero;
				}
			}
		}
	}

	public static IInArchive InArchive(InArchiveFormat format, object user)
	{
		lock (_syncRoot)
		{
			if (_inArchives[user][format] == null)
			{
				if (_modulePtr == IntPtr.Zero)
				{
					LoadLibrary(user, format);
					if (_modulePtr == IntPtr.Zero)
					{
						throw new SevenZipLibraryException();
					}
				}
				NativeMethods.CreateObjectDelegate createObjectDelegate = (NativeMethods.CreateObjectDelegate)Marshal.GetDelegateForFunctionPointer(NativeMethods.GetProcAddress(_modulePtr, "CreateObject"), typeof(NativeMethods.CreateObjectDelegate));
				if (createObjectDelegate == null)
				{
					throw new SevenZipLibraryException();
				}
				Guid interfaceID = typeof(IInArchive).GUID;
				Guid classID = Formats.InFormatGuids[format];
				object outObject;
				try
				{
					createObjectDelegate(ref classID, ref interfaceID, out outObject);
				}
				catch (Exception)
				{
					throw new SevenZipLibraryException("Your 7-zip library does not support this archive type.");
				}
				InitUserInFormat(user, format);
				_inArchives[user][format] = outObject as IInArchive;
			}
			return _inArchives[user][format];
		}
	}

	public static IOutArchive OutArchive(OutArchiveFormat format, object user)
	{
		lock (_syncRoot)
		{
			if (_outArchives[user][format] == null)
			{
				if (_modulePtr == IntPtr.Zero)
				{
					throw new SevenZipLibraryException();
				}
				NativeMethods.CreateObjectDelegate createObjectDelegate = (NativeMethods.CreateObjectDelegate)Marshal.GetDelegateForFunctionPointer(NativeMethods.GetProcAddress(_modulePtr, "CreateObject"), typeof(NativeMethods.CreateObjectDelegate));
				Guid interfaceID = typeof(IOutArchive).GUID;
				try
				{
					Guid classID = Formats.OutFormatGuids[format];
					createObjectDelegate(ref classID, ref interfaceID, out var outObject);
					InitUserOutFormat(user, format);
					_outArchives[user][format] = outObject as IOutArchive;
				}
				catch (Exception)
				{
					throw new SevenZipLibraryException("Your 7-zip library does not support this archive type.");
				}
			}
			return _outArchives[user][format];
		}
	}

	public static void SetLibraryPath(string libraryPath)
	{
		if (_modulePtr != IntPtr.Zero && !Path.GetFullPath(libraryPath).Equals(Path.GetFullPath(_libraryFileName), StringComparison.OrdinalIgnoreCase))
		{
			throw new SevenZipLibraryException("can not change the library path while the library \"" + _libraryFileName + "\" is being used.");
		}
		if (!File.Exists(libraryPath))
		{
			throw new SevenZipLibraryException("can not change the library path because the file \"" + libraryPath + "\" does not exist.");
		}
		_libraryFileName = libraryPath;
		_features = null;
	}
}
