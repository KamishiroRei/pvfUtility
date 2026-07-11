using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using SevenZip.Sdk;
using SevenZip.Sdk.Compression.Lzma;

namespace SevenZip;

public sealed class SevenZipCompressor : SevenZipBase
{
	private delegate void CompressFiles1Delegate(string archiveName, string[] fileFullNames);

	private delegate void CompressFiles2Delegate(Stream archiveStream, string[] fileFullNames);

	private delegate void CompressFiles3Delegate(string archiveName, int commonRootLength, string[] fileFullNames);

	private delegate void CompressFiles4Delegate(Stream archiveStream, int commonRootLength, string[] fileFullNames);

	private delegate void CompressFilesEncrypted1Delegate(string archiveName, string password, string[] fileFullNames);

	private delegate void CompressFilesEncrypted2Delegate(Stream archiveStream, string password, string[] fileFullNames);

	private delegate void CompressFilesEncrypted3Delegate(string archiveName, int commonRootLength, string password, string[] fileFullNames);

	private delegate void CompressFilesEncrypted4Delegate(Stream archiveStream, int commonRootLength, string password, string[] fileFullNames);

	private delegate void CompressDirectoryDelegate(string directory, string archiveName, string password, string searchPattern, bool recursion);

	private delegate void CompressDirectory2Delegate(string directory, Stream archiveStream, string password, string searchPattern, bool recursion);

	private delegate void CompressStreamDelegate(Stream inStream, Stream outStream, string password);

	private delegate void ModifyArchiveDelegate(string archiveName, IDictionary<int, string> newFileNames, string password);

	private bool _compressingFilesOnDisk;

	private OutArchiveFormat _archiveFormat;

	private CompressionMethod _compressionMethod = CompressionMethod.Default;

	private long _volumeSize;

	private string _archiveName;

	private bool _directoryCompress;

	private UpdateData _updateData;

	private uint _oldFilesCount;

	private static volatile int _lzmaDictionarySize = 4194304;

	public CompressionLevel CompressionLevel { get; set; }

	public Dictionary<string, string> CustomParameters { get; private set; }

	public bool IncludeEmptyDirectories { get; set; }

	public bool PreserveDirectoryRoot { get; set; }

	public bool DirectoryStructure { get; set; }

	public CompressionMode CompressionMode { get; set; }

	public bool EncryptHeaders { get; set; }

	public bool ScanOnlyWritable { get; set; }

	public ZipEncryptionMethod ZipEncryptionMethod { get; set; }

	public string TempFolderPath { get; set; }

	public string DefaultItemName { get; set; }

	public bool FastCompression { get; set; }

	public OutArchiveFormat ArchiveFormat
	{
		get
		{
			return _archiveFormat;
		}
		set
		{
			_archiveFormat = value;
			if (!MethodIsValid(_compressionMethod))
			{
				_compressionMethod = CompressionMethod.Default;
			}
		}
	}

	public CompressionMethod CompressionMethod
	{
		get
		{
			return _compressionMethod;
		}
		set
		{
			_compressionMethod = ((!MethodIsValid(value)) ? CompressionMethod.Default : value);
		}
	}

	public long VolumeSize
	{
		get
		{
			return _volumeSize;
		}
		set
		{
			_volumeSize = ((value > 0) ? value : 0);
		}
	}

	public static int LzmaDictionarySize
	{
		get
		{
			return _lzmaDictionarySize;
		}
		set
		{
			_lzmaDictionarySize = value;
		}
	}

	public event EventHandler<FileNameEventArgs> FileCompressionStarted;

	public event EventHandler<EventArgs> FileCompressionFinished;

	public event EventHandler<ProgressEventArgs> Compressing;

	public event EventHandler<IntEventArgs> FilesFound;

	public event EventHandler<EventArgs> CompressionFinished;

	private void CommonInit()
	{
		DirectoryStructure = true;
		IncludeEmptyDirectories = true;
		CompressionLevel = CompressionLevel.Normal;
		CompressionMode = CompressionMode.Create;
		ZipEncryptionMethod = ZipEncryptionMethod.ZipCrypto;
		CustomParameters = new Dictionary<string, string>();
		_updateData = default(UpdateData);
		DefaultItemName = "default";
	}

	public SevenZipCompressor()
	{
		try
		{
			TempFolderPath = Path.GetTempPath();
		}
		catch (SecurityException)
		{
			throw new SevenZipCompressionFailedException("Path.GetTempPath() threw a System.Security.SecurityException. You must call SevenZipCompressor constructor overload with your own temporary path.");
		}
		CommonInit();
	}

	public SevenZipCompressor(string temporaryPath)
	{
		TempFolderPath = temporaryPath;
		if (!Directory.Exists(TempFolderPath))
		{
			try
			{
				Directory.CreateDirectory(TempFolderPath);
			}
			catch (Exception)
			{
				throw new SevenZipCompressionFailedException("The specified temporary path is invalid.");
			}
		}
		CommonInit();
	}

	private static void ValidateStream(Stream stream)
	{
		if (!stream.CanWrite || !stream.CanSeek)
		{
			throw new ArgumentException("The specified stream can not seek or is not writable.", "stream");
		}
	}

	private IOutArchive MakeOutArchive(IInStream inArchiveStream)
	{
		IInArchive inArchive = SevenZipLibraryManager.InArchive(Formats.InForOutFormats[_archiveFormat], this);
		using (ArchiveOpenCallback openArchiveCallback = GetArchiveOpenCallback())
		{
			ulong maxCheckStartPosition = 32768uL;
			if (inArchive.Open(inArchiveStream, ref maxCheckStartPosition, openArchiveCallback) != 0 && !ThrowException(null, new SevenZipArchiveException("Can not update the archive: Open() failed.")))
			{
				return null;
			}
			_oldFilesCount = inArchive.GetNumberOfItems();
		}
		return (IOutArchive)inArchive;
	}

	private bool MethodIsValid(CompressionMethod method)
	{
		if (method == CompressionMethod.Default)
		{
			return true;
		}
		switch (_archiveFormat)
		{
		case OutArchiveFormat.GZip:
			return method == CompressionMethod.Deflate;
		case OutArchiveFormat.BZip2:
			return method == CompressionMethod.BZip2;
		case OutArchiveFormat.SevenZip:
			if (method != CompressionMethod.Deflate)
			{
				return method != CompressionMethod.Deflate64;
			}
			return false;
		case OutArchiveFormat.Tar:
			return method == CompressionMethod.Copy;
		case OutArchiveFormat.Zip:
			return method != CompressionMethod.Lzma2;
		default:
			return true;
		}
	}

	private bool SwitchIsInCustomParameters(string name)
	{
		return CustomParameters.ContainsKey(name);
	}

	private void SetCompressionProperties()
	{
		OutArchiveFormat archiveFormat = _archiveFormat;
		if (archiveFormat == OutArchiveFormat.Tar)
		{
			return;
		}
		ISetProperties setProperties = ((CompressionMode == CompressionMode.Create && _updateData.FileNamesToModify == null) ? ((ISetProperties)SevenZipLibraryManager.OutArchive(_archiveFormat, this)) : ((ISetProperties)SevenZipLibraryManager.InArchive(Formats.InForOutFormats[_archiveFormat], this)));
		if (setProperties == null && !ThrowException(null, new CompressionFailedException("The specified archive format is unsupported.")))
		{
			return;
		}
		if (_volumeSize > 0 && ArchiveFormat != OutArchiveFormat.SevenZip)
		{
			throw new CompressionFailedException("Unfortunately, the creation of multi-volume non-7Zip archives is not implemented.");
		}
		if ((CustomParameters.ContainsKey("x") && !ThrowException(null, new CompressionFailedException("Use the \"CompressionLevel\" property instead of the \"x\" parameter."))) || (CustomParameters.ContainsKey("em") && !ThrowException(null, new CompressionFailedException("Use the \"ZipEncryptionMethod\" property instead of the \"em\" parameter."))) || (CustomParameters.ContainsKey("m") && !ThrowException(null, new CompressionFailedException("Use the \"CompressionMethod\" property instead of the \"m\" parameter."))))
		{
			return;
		}
		List<IntPtr> list = new List<IntPtr>(2 + CustomParameters.Count);
		List<PropVariant> list2 = new List<PropVariant>(2 + CustomParameters.Count);
		list.Add(Marshal.StringToBSTR("x"));
		list2.Add(default(PropVariant));
		if (_compressionMethod != CompressionMethod.Default)
		{
			list.Add((_archiveFormat == OutArchiveFormat.Zip) ? Marshal.StringToBSTR("m") : Marshal.StringToBSTR("0"));
			PropVariant item = new PropVariant
			{
				VarType = VarEnum.VT_BSTR,
				Value = Marshal.StringToBSTR(Formats.MethodNames[_compressionMethod])
			};
			list2.Add(item);
		}
		foreach (KeyValuePair<string, string> customParameter in CustomParameters)
		{
			if (_compressionMethod != CompressionMethod.Ppmd && (customParameter.Key.Equals("mem") || customParameter.Key.Equals("o")))
			{
				ThrowException(null, new CompressionFailedException("Parameter \"" + customParameter.Key + "\" is only valid with the PPMd compression method."));
			}
			list.Add(Marshal.StringToBSTR(customParameter.Key));
			PropVariant item2 = default(PropVariant);
			if (customParameter.Value.All(char.IsDigit))
			{
				item2.VarType = VarEnum.VT_UI4;
				item2.UInt32Value = Convert.ToUInt32(customParameter.Value, CultureInfo.InvariantCulture);
			}
			else
			{
				item2.VarType = VarEnum.VT_BSTR;
				item2.Value = Marshal.StringToBSTR(customParameter.Value);
			}
			list2.Add(item2);
		}
		PropVariant value = list2[0];
		value.VarType = VarEnum.VT_UI4;
		switch (CompressionLevel)
		{
		case CompressionLevel.None:
			value.UInt32Value = 0u;
			break;
		case CompressionLevel.Fast:
			value.UInt32Value = 1u;
			break;
		case CompressionLevel.Low:
			value.UInt32Value = 3u;
			break;
		case CompressionLevel.Normal:
			value.UInt32Value = 5u;
			break;
		case CompressionLevel.High:
			value.UInt32Value = 7u;
			break;
		case CompressionLevel.Ultra:
			value.UInt32Value = 9u;
			break;
		}
		list2[0] = value;
		if (EncryptHeaders && _archiveFormat == OutArchiveFormat.SevenZip && !SwitchIsInCustomParameters("he"))
		{
			list.Add(Marshal.StringToBSTR("he"));
			PropVariant item3 = new PropVariant
			{
				VarType = VarEnum.VT_BSTR,
				Value = Marshal.StringToBSTR("on")
			};
			list2.Add(item3);
		}
		if (_archiveFormat == OutArchiveFormat.Zip && ZipEncryptionMethod != ZipEncryptionMethod.ZipCrypto && !SwitchIsInCustomParameters("em"))
		{
			list.Add(Marshal.StringToBSTR("em"));
			PropVariant item4 = new PropVariant
			{
				VarType = VarEnum.VT_BSTR,
				Value = Marshal.StringToBSTR(Enum.GetName(typeof(ZipEncryptionMethod), ZipEncryptionMethod))
			};
			list2.Add(item4);
		}
		GCHandle gCHandle = GCHandle.Alloc(list.ToArray(), GCHandleType.Pinned);
		GCHandle gCHandle2 = GCHandle.Alloc(list2.ToArray(), GCHandleType.Pinned);
		try
		{
			setProperties?.SetProperties(gCHandle.AddrOfPinnedObject(), gCHandle2.AddrOfPinnedObject(), list.Count);
		}
		finally
		{
			gCHandle.Free();
			gCHandle2.Free();
		}
	}

	private static int CommonRoot(ICollection<string> files)
	{
		List<string[]> list = new List<string[]>(files.Count);
		list.AddRange(files.Select((string fn) => fn.Split(Path.DirectorySeparatorChar)));
		int num = list[0].Length - 1;
		if (files.Count > 1)
		{
			for (int num2 = 1; num2 < files.Count; num2++)
			{
				if (num > list[num2].Length)
				{
					num = list[num2].Length;
				}
			}
		}
		string text = "";
		for (int num3 = 0; num3 < num; num3++)
		{
			bool flag = true;
			for (int num4 = 1; num4 < files.Count; num4++)
			{
				if (!(flag &= list[num4 - 1][num3] == list[num4][num3]))
				{
					break;
				}
			}
			if (!flag)
			{
				break;
			}
			text = text + list[0][num3] + Path.DirectorySeparatorChar;
		}
		return text.Length;
	}

	private static void CheckCommonRoot(IReadOnlyList<string> files, ref int commonRootLength)
	{
		string commonRoot;
		try
		{
			commonRoot = files[0].Substring(0, commonRootLength);
		}
		catch (ArgumentOutOfRangeException)
		{
			throw new SevenZipInvalidFileNamesException("invalid common root.");
		}
		if (commonRoot.EndsWith(new string(Path.DirectorySeparatorChar, 1), StringComparison.CurrentCulture))
		{
			commonRoot = commonRoot.Substring(0, commonRootLength - 1);
			commonRootLength--;
		}
		if (files.Any((string fn) => !fn.StartsWith(commonRoot, StringComparison.CurrentCulture)))
		{
			throw new SevenZipInvalidFileNamesException("invalid common root.");
		}
	}

	private static bool RecursiveDirectoryEmptyCheck(string directory)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(directory);
		if (directoryInfo.GetFiles().Length != 0)
		{
			return false;
		}
		bool flag = true;
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		foreach (DirectoryInfo directoryInfo2 in directories)
		{
			flag &= RecursiveDirectoryEmptyCheck(directoryInfo2.FullName);
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	private static FileInfo[] ProduceFileInfoArray(IReadOnlyList<string> files, int commonRootLength, bool directoryCompress, bool directoryStructure)
	{
		List<FileInfo> list = new List<FileInfo>(files.Count);
		string text = files[0].Substring(0, commonRootLength);
		if (directoryCompress)
		{
			list.AddRange(files.Select((string fn) => new FileInfo(fn)));
		}
		else if (!directoryStructure)
		{
			list.AddRange(from fn in files
				where !Directory.Exists(fn)
				select new FileInfo(fn));
		}
		else
		{
			List<string> list2 = new List<string>(files.Count);
			CheckCommonRoot(files, ref commonRootLength);
			if (commonRootLength > 0)
			{
				commonRootLength++;
				foreach (string file in files)
				{
					string[] array = file.Substring(commonRootLength).Split(Path.DirectorySeparatorChar);
					string text2 = text;
					string[] array2 = array;
					foreach (string text3 in array2)
					{
						text2 = text2 + Path.DirectorySeparatorChar + text3;
						if (!list2.Contains(text2))
						{
							list.Add(new FileInfo(text2));
							list2.Add(text2);
						}
					}
				}
			}
			else
			{
				foreach (string file2 in files)
				{
					string[] array3 = file2.Substring(commonRootLength).Split(Path.DirectorySeparatorChar);
					string text4 = array3[0];
					for (int num2 = 1; num2 < array3.Length; num2++)
					{
						text4 = text4 + Path.DirectorySeparatorChar + array3[num2];
						if (!list2.Contains(text4))
						{
							list.Add(new FileInfo(text4));
							list2.Add(text4);
						}
					}
				}
			}
		}
		return list.ToArray();
	}

	private void AddFilesFromDirectory(string directory, ICollection<string> files, string searchPattern)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(directory);
		FileInfo[] files2 = directoryInfo.GetFiles(searchPattern);
		foreach (FileInfo fileInfo in files2)
		{
			if (!ScanOnlyWritable)
			{
				files.Add(fileInfo.FullName);
				continue;
			}
			try
			{
				using (fileInfo.OpenWrite())
				{
				}
				files.Add(fileInfo.FullName);
			}
			catch (IOException)
			{
			}
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		foreach (DirectoryInfo directoryInfo2 in directories)
		{
			if (IncludeEmptyDirectories)
			{
				files.Add(directoryInfo2.FullName);
			}
			AddFilesFromDirectory(directoryInfo2.FullName, files, searchPattern);
		}
	}

	private void CommonUpdateCallbackInit(ArchiveUpdateCallback auc)
	{
		auc.FileCompressionStarted += FileCompressionStartedEventProxy;
		auc.Compressing += CompressingEventProxy;
		auc.FileCompressionFinished += FileCompressionFinishedEventProxy;
		auc.DefaultItemName = DefaultItemName;
		auc.FastCompression = FastCompression;
	}

	private float GetDictionarySize()
	{
		float result = 0.001f;
		switch (_compressionMethod)
		{
		case CompressionMethod.Lzma:
		case CompressionMethod.Lzma2:
		case CompressionMethod.Default:
			switch (CompressionLevel)
			{
			case CompressionLevel.None:
				result = 0.001f;
				break;
			case CompressionLevel.Fast:
				result = 4.46875f;
				break;
			case CompressionLevel.Low:
				result = 90.25f;
				break;
			case CompressionLevel.Normal:
				result = 188f;
				break;
			case CompressionLevel.High:
				result = 372f;
				break;
			case CompressionLevel.Ultra:
				result = 740f;
				break;
			}
			break;
		case CompressionMethod.BZip2:
			switch (CompressionLevel)
			{
			case CompressionLevel.None:
				result = 0f;
				break;
			case CompressionLevel.Fast:
				result = 0.095f;
				break;
			case CompressionLevel.Low:
				result = 0.477f;
				break;
			case CompressionLevel.Normal:
			case CompressionLevel.High:
			case CompressionLevel.Ultra:
				result = 0.858f;
				break;
			}
			break;
		case CompressionMethod.Deflate:
		case CompressionMethod.Deflate64:
			result = 32f;
			break;
		case CompressionMethod.Ppmd:
			result = 16f;
			break;
		}
		return result;
	}

	private ArchiveUpdateCallback GetArchiveUpdateCallback(FileInfo[] files, int rootLength, string password)
	{
		SetCompressionProperties();
		ArchiveUpdateCallback archiveUpdateCallback = (string.IsNullOrEmpty(password) ? new ArchiveUpdateCallback(files, rootLength, this, GetUpdateData(), DirectoryStructure)
		{
			DictionarySize = GetDictionarySize()
		} : new ArchiveUpdateCallback(files, rootLength, password, this, GetUpdateData(), DirectoryStructure)
		{
			DictionarySize = GetDictionarySize()
		});
		CommonUpdateCallbackInit(archiveUpdateCallback);
		return archiveUpdateCallback;
	}

	private ArchiveUpdateCallback GetArchiveUpdateCallback(Stream inStream, string password)
	{
		SetCompressionProperties();
		ArchiveUpdateCallback archiveUpdateCallback = (string.IsNullOrEmpty(password) ? new ArchiveUpdateCallback(inStream, this, GetUpdateData(), DirectoryStructure)
		{
			DictionarySize = GetDictionarySize()
		} : new ArchiveUpdateCallback(inStream, password, this, GetUpdateData(), DirectoryStructure)
		{
			DictionarySize = GetDictionarySize()
		});
		CommonUpdateCallbackInit(archiveUpdateCallback);
		return archiveUpdateCallback;
	}

	private ArchiveUpdateCallback GetArchiveUpdateCallback(IDictionary<string, Stream> streamDict, string password)
	{
		SetCompressionProperties();
		ArchiveUpdateCallback archiveUpdateCallback = (string.IsNullOrEmpty(password) ? new ArchiveUpdateCallback(streamDict, this, GetUpdateData(), DirectoryStructure)
		{
			DictionarySize = GetDictionarySize()
		} : new ArchiveUpdateCallback(streamDict, password, this, GetUpdateData(), DirectoryStructure)
		{
			DictionarySize = GetDictionarySize()
		});
		CommonUpdateCallbackInit(archiveUpdateCallback);
		return archiveUpdateCallback;
	}

	private void FreeCompressionCallback(ArchiveUpdateCallback callback)
	{
		callback.FileCompressionStarted -= FileCompressionStartedEventProxy;
		callback.Compressing -= CompressingEventProxy;
		callback.FileCompressionFinished -= FileCompressionFinishedEventProxy;
	}

	private string GetTempArchiveFileName(string archiveName)
	{
		return Path.Combine(TempFolderPath, Path.GetFileName(archiveName) + ".~");
	}

	private FileStream GetArchiveFileStream(string archiveName)
	{
		if ((CompressionMode != CompressionMode.Create || _updateData.FileNamesToModify != null) && !File.Exists(archiveName) && !ThrowException(null, new CompressionFailedException("file \"" + archiveName + "\" does not exist.")))
		{
			return null;
		}
		if (_volumeSize != 0L)
		{
			return null;
		}
		if (CompressionMode != CompressionMode.Create || _updateData.FileNamesToModify != null)
		{
			return File.Create(GetTempArchiveFileName(archiveName));
		}
		return File.Create(archiveName);
	}

	private void FinalizeUpdate()
	{
		if (_volumeSize == 0L && (CompressionMode != CompressionMode.Create || _updateData.FileNamesToModify != null))
		{
			File.Move(GetTempArchiveFileName(_archiveName), _archiveName);
		}
	}

	private UpdateData GetUpdateData()
	{
		if (_updateData.FileNamesToModify == null)
		{
			UpdateData result = new UpdateData
			{
				Mode = (InternalCompressionMode)CompressionMode
			};
			switch (CompressionMode)
			{
			case CompressionMode.Create:
				result.FilesCount = uint.MaxValue;
				break;
			case CompressionMode.Append:
				result.FilesCount = _oldFilesCount;
				break;
			}
			return result;
		}
		return _updateData;
	}

	private ISequentialOutStream GetOutStream(Stream outStream)
	{
		if (!_compressingFilesOnDisk)
		{
			return new OutStreamWrapper(outStream, disposeStream: false);
		}
		if (_volumeSize == 0L || CompressionMode != CompressionMode.Create || _updateData.FileNamesToModify != null)
		{
			return new OutStreamWrapper(outStream, disposeStream: true);
		}
		return new OutMultiStreamWrapper(_archiveName, _volumeSize);
	}

	private IInStream GetInStream()
	{
		if (!File.Exists(_archiveName) || ((CompressionMode == CompressionMode.Create || !_compressingFilesOnDisk) && _updateData.FileNamesToModify == null))
		{
			return null;
		}
		return new InStreamWrapper(new FileStream(_archiveName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), disposeStream: true);
	}

	private ArchiveOpenCallback GetArchiveOpenCallback()
	{
		if (!string.IsNullOrEmpty(base.Password))
		{
			return new ArchiveOpenCallback(_archiveName, base.Password);
		}
		return new ArchiveOpenCallback(_archiveName);
	}

	private void FileCompressionStartedEventProxy(object sender, FileNameEventArgs e)
	{
		OnEvent(this.FileCompressionStarted, e, synchronous: false);
	}

	private void FileCompressionFinishedEventProxy(object sender, EventArgs e)
	{
		OnEvent(this.FileCompressionFinished, e, synchronous: false);
	}

	private void CompressingEventProxy(object sender, ProgressEventArgs e)
	{
		OnEvent(this.Compressing, e, synchronous: false);
	}

	private void FilesFoundEventProxy(object sender, IntEventArgs e)
	{
		OnEvent(this.FilesFound, e, synchronous: false);
	}

	public void CompressFiles(string archiveName, params string[] fileFullNames)
	{
		CompressFilesEncrypted(archiveName, string.Empty, fileFullNames);
	}

	public void CompressFiles(Stream archiveStream, params string[] fileFullNames)
	{
		CompressFilesEncrypted(archiveStream, string.Empty, fileFullNames);
	}

	public void CompressFiles(string archiveName, int commonRootLength, params string[] fileFullNames)
	{
		CompressFilesEncrypted(archiveName, commonRootLength, string.Empty, fileFullNames);
	}

	public void CompressFiles(Stream archiveStream, int commonRootLength, params string[] fileFullNames)
	{
		fileFullNames = GetFullFilePaths(fileFullNames);
		CompressFilesEncrypted(archiveStream, commonRootLength, string.Empty, fileFullNames);
	}

	public void CompressFilesEncrypted(string archiveName, string password, params string[] fileFullNames)
	{
		fileFullNames = GetFullFilePaths(fileFullNames);
		CompressFilesEncrypted(archiveName, CommonRoot(fileFullNames), password, fileFullNames);
	}

	public void CompressFilesEncrypted(Stream archiveStream, string password, params string[] fileFullNames)
	{
		fileFullNames = GetFullFilePaths(fileFullNames);
		CompressFilesEncrypted(archiveStream, CommonRoot(fileFullNames), password, fileFullNames);
	}

	public void CompressFilesEncrypted(string archiveName, int commonRootLength, string password, params string[] fileFullNames)
	{
		_compressingFilesOnDisk = true;
		_archiveName = archiveName;
		using (FileStream fileStream = GetArchiveFileStream(archiveName))
		{
			if (fileStream == null && _volumeSize == 0L)
			{
				return;
			}
			CompressFilesEncrypted(fileStream, commonRootLength, password, fileFullNames);
		}
		FinalizeUpdate();
	}

	public void CompressFilesEncrypted(Stream archiveStream, int commonRootLength, string password, params string[] fileFullNames)
	{
		ClearExceptions();
		if (fileFullNames.Length > 1 && (_archiveFormat == OutArchiveFormat.BZip2 || _archiveFormat == OutArchiveFormat.GZip || _archiveFormat == OutArchiveFormat.XZ) && !ThrowException(null, new CompressionFailedException("Can not compress more than one file in this format.")))
		{
			return;
		}
		if (_volumeSize == 0L || !_compressingFilesOnDisk)
		{
			ValidateStream(archiveStream);
		}
		FileInfo[] array = null;
		try
		{
			array = ProduceFileInfoArray(fileFullNames, commonRootLength, _directoryCompress, DirectoryStructure);
		}
		catch (Exception ex)
		{
			if (!ThrowException(null, ex))
			{
				return;
			}
		}
		_directoryCompress = false;
		this.FilesFound?.Invoke(this, new IntEventArgs(fileFullNames.Length));
		try
		{
			ISequentialOutStream outStream;
			using ((outStream = GetOutStream(archiveStream)) as IDisposable)
			{
				IInStream inStream;
				using ((inStream = GetInStream()) as IDisposable)
				{
					IOutArchive outArchive;
					if (CompressionMode == CompressionMode.Create || !_compressingFilesOnDisk)
					{
						SevenZipLibraryManager.LoadLibrary(this, _archiveFormat);
						outArchive = SevenZipLibraryManager.OutArchive(_archiveFormat, this);
					}
					else
					{
						SevenZipLibraryManager.LoadLibrary(this, Formats.InForOutFormats[_archiveFormat]);
						if ((outArchive = MakeOutArchive(inStream)) == null)
						{
							return;
						}
					}
					using ArchiveUpdateCallback archiveUpdateCallback = GetArchiveUpdateCallback(array, commonRootLength, password);
					try
					{
						if (array != null)
						{
							CheckedExecute(outArchive.UpdateItems(outStream, (uint)array.Length + _oldFilesCount, archiveUpdateCallback), "The compression has failed for an unknown reason with code ", archiveUpdateCallback);
						}
					}
					finally
					{
						FreeCompressionCallback(archiveUpdateCallback);
					}
				}
			}
		}
		finally
		{
			if (CompressionMode == CompressionMode.Create || !_compressingFilesOnDisk)
			{
				SevenZipLibraryManager.FreeLibrary(this, _archiveFormat);
			}
			else
			{
				SevenZipLibraryManager.FreeLibrary(this, Formats.InForOutFormats[_archiveFormat]);
				File.Delete(_archiveName);
			}
			_compressingFilesOnDisk = false;
			OnEvent(this.CompressionFinished, EventArgs.Empty, synchronous: false);
		}
		ThrowUserException();
	}

	public void CompressDirectory(string directory, string archiveName, string password = "", string searchPattern = "*", bool recursion = true)
	{
		_compressingFilesOnDisk = true;
		_archiveName = archiveName;
		using (FileStream fileStream = GetArchiveFileStream(archiveName))
		{
			if (fileStream == null && _volumeSize == 0L)
			{
				return;
			}
			CompressDirectory(directory, fileStream, password, searchPattern, recursion);
		}
		FinalizeUpdate();
	}

	public void CompressDirectory(string directory, Stream archiveStream, string password = "", string searchPattern = "*", bool recursion = true)
	{
		List<string> list = new List<string>();
		if (!Directory.Exists(directory))
		{
			throw new ArgumentException("Directory \"" + directory + "\" does not exist!");
		}
		directory = Path.GetFullPath(directory);
		if (RecursiveDirectoryEmptyCheck(directory))
		{
			throw new SevenZipInvalidFileNamesException("the specified directory is empty!");
		}
		if (recursion)
		{
			AddFilesFromDirectory(directory, list, searchPattern);
		}
		else
		{
			list.AddRange(from fi in new DirectoryInfo(directory).GetFiles(searchPattern)
				select fi.FullName);
		}
		int num = directory.Length;
		if (directory.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
		{
			directory = directory.Substring(0, directory.Length - 1);
		}
		else
		{
			num++;
		}
		if (PreserveDirectoryRoot)
		{
			string directoryName = Path.GetDirectoryName(directory);
			num = directoryName.Length + ((!directoryName.EndsWith("\\", StringComparison.OrdinalIgnoreCase)) ? 1 : 0);
		}
		_directoryCompress = true;
		CompressFilesEncrypted(archiveStream, num, password, list.ToArray());
	}

	public void CompressFileDictionary(IDictionary<string, string> fileDictionary, string archiveName, string password = "")
	{
		_compressingFilesOnDisk = true;
		_archiveName = archiveName;
		using (FileStream fileStream = GetArchiveFileStream(archiveName))
		{
			if (fileStream == null && _volumeSize == 0L)
			{
				return;
			}
			CompressFileDictionary(fileDictionary, fileStream, password);
		}
		FinalizeUpdate();
	}

	public void CompressFileDictionary(IDictionary<string, string> fileDictionary, Stream archiveStream, string password = "")
	{
		Dictionary<string, Stream> dictionary = new Dictionary<string, Stream>(fileDictionary.Count);
		foreach (KeyValuePair<string, string> item in fileDictionary)
		{
			if (item.Value == null)
			{
				dictionary.Add(item.Key, null);
				continue;
			}
			if (!File.Exists(item.Value))
			{
				throw new CompressionFailedException("The file corresponding to the archive entry \"" + item.Key + "\" does not exist.");
			}
			dictionary.Add(item.Key, new FileStream(item.Value, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
		}
		CompressStreamDictionary(dictionary, archiveStream, password);
	}

	public void CompressStreamDictionary(IDictionary<string, Stream> streamDictionary, string archiveName, string password = "")
	{
		_compressingFilesOnDisk = true;
		_archiveName = archiveName;
		using (FileStream fileStream = GetArchiveFileStream(archiveName))
		{
			if (fileStream == null && _volumeSize == 0L)
			{
				return;
			}
			CompressStreamDictionary(streamDictionary, fileStream, password);
		}
		FinalizeUpdate();
	}

	public void CompressStreamDictionary(IDictionary<string, Stream> streamDictionary, Stream archiveStream, string password = "")
	{
		ClearExceptions();
		if (streamDictionary.Count > 1 && (_archiveFormat == OutArchiveFormat.BZip2 || _archiveFormat == OutArchiveFormat.GZip || _archiveFormat == OutArchiveFormat.XZ) && !ThrowException(null, new CompressionFailedException("Can not compress more than one file/stream in this format.")))
		{
			return;
		}
		if (_volumeSize == 0L || !_compressingFilesOnDisk)
		{
			ValidateStream(archiveStream);
		}
		if (streamDictionary.Where((KeyValuePair<string, Stream> pair) => pair.Value != null && (!pair.Value.CanSeek || !pair.Value.CanRead)).Any((KeyValuePair<string, Stream> pair) => !ThrowException(null, new ArgumentException("The specified stream dictionary contains an invalid stream corresponding to the archive entry \"" + pair.Key + "\".", "streamDictionary"))))
		{
			return;
		}
		try
		{
			ISequentialOutStream outStream;
			using ((outStream = GetOutStream(archiveStream)) as IDisposable)
			{
				IInStream inStream;
				using ((inStream = GetInStream()) as IDisposable)
				{
					IOutArchive outArchive;
					if (CompressionMode == CompressionMode.Create || !_compressingFilesOnDisk)
					{
						SevenZipLibraryManager.LoadLibrary(this, _archiveFormat);
						outArchive = SevenZipLibraryManager.OutArchive(_archiveFormat, this);
					}
					else
					{
						SevenZipLibraryManager.LoadLibrary(this, Formats.InForOutFormats[_archiveFormat]);
						if ((outArchive = MakeOutArchive(inStream)) == null)
						{
							return;
						}
					}
					using ArchiveUpdateCallback archiveUpdateCallback = GetArchiveUpdateCallback(streamDictionary, password);
					try
					{
						CheckedExecute(outArchive.UpdateItems(outStream, (uint)streamDictionary.Count + _oldFilesCount, archiveUpdateCallback), "The compression has failed for an unknown reason with code ", archiveUpdateCallback);
					}
					finally
					{
						FreeCompressionCallback(archiveUpdateCallback);
					}
				}
			}
		}
		finally
		{
			if (CompressionMode == CompressionMode.Create || !_compressingFilesOnDisk)
			{
				SevenZipLibraryManager.FreeLibrary(this, _archiveFormat);
			}
			else
			{
				SevenZipLibraryManager.FreeLibrary(this, Formats.InForOutFormats[_archiveFormat]);
				File.Delete(_archiveName);
			}
			_compressingFilesOnDisk = false;
			OnEvent(this.CompressionFinished, EventArgs.Empty, synchronous: false);
		}
		ThrowUserException();
	}

	public void CompressStream(Stream inStream, Stream outStream, string password = "")
	{
		ClearExceptions();
		if ((!inStream.CanSeek || !inStream.CanRead || !outStream.CanWrite) && !ThrowException(null, new ArgumentException("The specified streams are invalid.")))
		{
			return;
		}
		try
		{
			SevenZipLibraryManager.LoadLibrary(this, _archiveFormat);
			ISequentialOutStream outStream2;
			using ((outStream2 = GetOutStream(outStream)) as IDisposable)
			{
				using ArchiveUpdateCallback archiveUpdateCallback = GetArchiveUpdateCallback(inStream, password);
				try
				{
					CheckedExecute(SevenZipLibraryManager.OutArchive(_archiveFormat, this).UpdateItems(outStream2, 1u, archiveUpdateCallback), "The compression has failed for an unknown reason with code ", archiveUpdateCallback);
				}
				finally
				{
					FreeCompressionCallback(archiveUpdateCallback);
				}
			}
		}
		finally
		{
			SevenZipLibraryManager.FreeLibrary(this, _archiveFormat);
			OnEvent(this.CompressionFinished, EventArgs.Empty, synchronous: false);
		}
		ThrowUserException();
	}

	public void ModifyArchive(string archiveName, IDictionary<int, string> newFileNames, string password = "")
	{
		ClearExceptions();
		if (!SevenZipLibraryManager.ModifyCapable)
		{
			throw new SevenZipLibraryException("The specified 7zip native library does not support this method.");
		}
		if ((!File.Exists(archiveName) && !ThrowException(null, new ArgumentException("The specified archive does not exist.", "archiveName"))) || ((newFileNames == null || newFileNames.Count == 0) && !ThrowException(null, new ArgumentException("Invalid new file names.", "newFileNames"))))
		{
			return;
		}
		if (!string.IsNullOrEmpty(password) && string.IsNullOrEmpty(base.Password))
		{
			base.Password = password;
		}
		try
		{
			using (SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(archiveName, password))
			{
				_updateData = default(UpdateData);
				ArchiveFileInfo[] array = new ArchiveFileInfo[sevenZipExtractor.ArchiveFileData.Count];
				sevenZipExtractor.ArchiveFileData.CopyTo(array, 0);
				_updateData.ArchiveFileData = new List<ArchiveFileInfo>(array);
			}
			_updateData.FileNamesToModify = newFileNames;
			_updateData.Mode = InternalCompressionMode.Modify;
		}
		catch (SevenZipException ex)
		{
			if (!ThrowException(null, ex))
			{
				return;
			}
		}
		try
		{
			_compressingFilesOnDisk = true;
			ISequentialOutStream outStream;
			using ((outStream = GetOutStream(GetArchiveFileStream(archiveName))) as IDisposable)
			{
				_archiveName = archiveName;
				IInStream inStream;
				using ((inStream = GetInStream()) as IDisposable)
				{
					SevenZipLibraryManager.LoadLibrary(this, Formats.InForOutFormats[_archiveFormat]);
					IOutArchive outArchive;
					if ((outArchive = MakeOutArchive(inStream)) == null)
					{
						return;
					}
					using ArchiveUpdateCallback archiveUpdateCallback = GetArchiveUpdateCallback(null, 0, password);
					uint num = 0u;
					if (_updateData.FileNamesToModify != null)
					{
						num = (uint)_updateData.FileNamesToModify.Sum((KeyValuePair<int, string> pairDeleted) => (pairDeleted.Value == null) ? 1 : 0);
					}
					try
					{
						CheckedExecute(outArchive.UpdateItems(outStream, _oldFilesCount - num, archiveUpdateCallback), "The compression has failed for an unknown reason with code ", archiveUpdateCallback);
					}
					finally
					{
						FreeCompressionCallback(archiveUpdateCallback);
					}
				}
			}
		}
		finally
		{
			SevenZipLibraryManager.FreeLibrary(this, Formats.InForOutFormats[_archiveFormat]);
			File.Delete(archiveName);
			FinalizeUpdate();
			_compressingFilesOnDisk = false;
			_updateData.FileNamesToModify = null;
			_updateData.ArchiveFileData = null;
			OnEvent(this.CompressionFinished, EventArgs.Empty, synchronous: false);
		}
		ThrowUserException();
	}

	internal static void WriteLzmaProperties(Encoder encoder)
	{
		CoderPropId[] propIDs = new CoderPropId[8]
		{
			CoderPropId.DictionarySize,
			CoderPropId.PosStateBits,
			CoderPropId.LitContextBits,
			CoderPropId.LitPosBits,
			CoderPropId.Algorithm,
			CoderPropId.NumFastBytes,
			CoderPropId.MatchFinder,
			CoderPropId.EndMarker
		};
		object[] properties = new object[8] { _lzmaDictionarySize, 2, 3, 0, 2, 256, "bt4", false };
		encoder.SetCoderProperties(propIDs, properties);
	}

	public static void CompressStream(Stream inStream, Stream outStream, int? inLength, EventHandler<ProgressEventArgs> codeProgressEvent)
	{
		if (!inStream.CanRead || !outStream.CanWrite)
		{
			throw new ArgumentException("The specified streams are invalid.");
		}
		Encoder encoder = new Encoder();
		WriteLzmaProperties(encoder);
		encoder.WriteCoderProperties(outStream);
		long num = ((long?)inLength) ?? inStream.Length;
		for (int i = 0; i < 8; i++)
		{
			outStream.WriteByte((byte)(num >> 8 * i));
		}
		encoder.Code(inStream, outStream, -1L, -1L, new LzmaProgressCallback(num, codeProgressEvent));
	}

	public static byte[] CompressBytes(byte[] data)
	{
		using MemoryStream memoryStream = new MemoryStream(data);
		using MemoryStream memoryStream2 = new MemoryStream();
		Encoder encoder = new Encoder();
		WriteLzmaProperties(encoder);
		encoder.WriteCoderProperties(memoryStream2);
		long length = memoryStream.Length;
		for (int i = 0; i < 8; i++)
		{
			memoryStream2.WriteByte((byte)(length >> 8 * i));
		}
		encoder.Code(memoryStream, memoryStream2, -1L, -1L, null);
		return memoryStream2.ToArray();
	}

	private static string[] GetFullFilePaths(IEnumerable<string> fileFullNames)
	{
		return fileFullNames.Select(Path.GetFullPath).ToArray();
	}

	public void BeginCompressFiles(string archiveName, params string[] fileFullNames)
	{
		SaveContext();
		Task.Run(delegate
		{
			new CompressFiles1Delegate(CompressFiles)(archiveName, fileFullNames);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public void BeginCompressFiles(Stream archiveStream, params string[] fileFullNames)
	{
		SaveContext();
		Task.Run(delegate
		{
			new CompressFiles2Delegate(CompressFiles)(archiveStream, fileFullNames);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public void BeginCompressFiles(string archiveName, int commonRootLength, params string[] fileFullNames)
	{
		SaveContext();
		Task.Run(delegate
		{
			new CompressFiles3Delegate(CompressFiles)(archiveName, commonRootLength, fileFullNames);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public void BeginCompressFiles(Stream archiveStream, int commonRootLength, params string[] fileFullNames)
	{
		SaveContext();
		Task.Run(delegate
		{
			new CompressFiles4Delegate(CompressFiles)(archiveStream, commonRootLength, fileFullNames);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public void BeginCompressFilesEncrypted(string archiveName, string password, params string[] fileFullNames)
	{
		SaveContext();
		Task.Run(delegate
		{
			new CompressFilesEncrypted1Delegate(CompressFilesEncrypted)(archiveName, password, fileFullNames);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public void BeginCompressFilesEncrypted(Stream archiveStream, string password, params string[] fileFullNames)
	{
		SaveContext();
		Task.Run(delegate
		{
			new CompressFilesEncrypted2Delegate(CompressFilesEncrypted)(archiveStream, password, fileFullNames);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public void BeginCompressFilesEncrypted(string archiveName, int commonRootLength, string password, params string[] fileFullNames)
	{
		SaveContext();
		Task.Run(delegate
		{
			new CompressFilesEncrypted3Delegate(CompressFilesEncrypted)(archiveName, commonRootLength, password, fileFullNames);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public void BeginCompressFilesEncrypted(Stream archiveStream, int commonRootLength, string password, params string[] fileFullNames)
	{
		SaveContext();
		Task.Run(delegate
		{
			new CompressFilesEncrypted4Delegate(CompressFilesEncrypted)(archiveStream, commonRootLength, password, fileFullNames);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public async Task CompressFilesAsync(string archiveName, params string[] fileFullNames)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new CompressFiles1Delegate(CompressFiles)(archiveName, fileFullNames);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public async Task CompressFilesAsync(Stream archiveStream, params string[] fileFullNames)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new CompressFiles2Delegate(CompressFiles)(archiveStream, fileFullNames);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public async Task CompressFilesAsync(string archiveName, int commonRootLength, params string[] fileFullNames)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new CompressFiles3Delegate(CompressFiles)(archiveName, commonRootLength, fileFullNames);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public async Task CompressFilesAsync(Stream archiveStream, int commonRootLength, params string[] fileFullNames)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new CompressFiles4Delegate(CompressFiles)(archiveStream, commonRootLength, fileFullNames);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public async Task CompressFilesEncryptedAsync(string archiveName, string password, params string[] fileFullNames)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new CompressFilesEncrypted1Delegate(CompressFilesEncrypted)(archiveName, password, fileFullNames);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public async Task CompressFilesEncryptedAsync(Stream archiveStream, string password, params string[] fileFullNames)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new CompressFilesEncrypted2Delegate(CompressFilesEncrypted)(archiveStream, password, fileFullNames);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public async Task CompressFilesEncryptedAsync(string archiveName, int commonRootLength, string password, params string[] fileFullNames)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new CompressFilesEncrypted3Delegate(CompressFilesEncrypted)(archiveName, commonRootLength, password, fileFullNames);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public async Task CompressFilesEncryptedAsync(Stream archiveStream, int commonRootLength, string password, params string[] fileFullNames)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new CompressFilesEncrypted4Delegate(CompressFilesEncrypted)(archiveStream, commonRootLength, password, fileFullNames);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public void BeginCompressDirectory(string directory, string archiveName, string password = "", string searchPattern = "*", bool recursion = true)
	{
		SaveContext();
		Task.Run(delegate
		{
			new CompressDirectoryDelegate(CompressDirectory)(directory, archiveName, password, searchPattern, recursion);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public void BeginCompressDirectory(string directory, Stream archiveStream, string password, string searchPattern = "*", bool recursion = true)
	{
		SaveContext();
		Task.Run(delegate
		{
			new CompressDirectory2Delegate(CompressDirectory)(directory, archiveStream, password, searchPattern, recursion);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public async Task CompressDirectoryAsync(string directory, string archiveName, string password = "", string searchPattern = "*", bool recursion = true)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new CompressDirectoryDelegate(CompressDirectory)(directory, archiveName, password, searchPattern, recursion);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public async Task CompressDirectoryAsync(string directory, Stream archiveStream, string password, string searchPattern = "*", bool recursion = true)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new CompressDirectory2Delegate(CompressDirectory)(directory, archiveStream, password, searchPattern, recursion);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public void BeginCompressStream(Stream inStream, Stream outStream, string password = "")
	{
		SaveContext();
		Task.Run(delegate
		{
			new CompressStreamDelegate(CompressStream)(inStream, outStream, password);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public async Task CompressStreamAsync(Stream inStream, Stream outStream, string password = "")
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new CompressStreamDelegate(CompressStream)(inStream, outStream, password);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public void BeginModifyArchive(string archiveName, IDictionary<int, string> newFileNames, string password = "")
	{
		SaveContext();
		Task.Run(delegate
		{
			new ModifyArchiveDelegate(ModifyArchive)(archiveName, newFileNames, password);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public async Task ModifyArchiveAsync(string archiveName, IDictionary<int, string> newFileNames, string password = "")
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new ModifyArchiveDelegate(ModifyArchive)(archiveName, newFileNames, password);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}
}
