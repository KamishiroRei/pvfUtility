using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SevenZip.Sdk.Compression.Lzma;

namespace SevenZip;

public sealed class SevenZipExtractor : SevenZipBase, IDisposable
{
	private delegate void ExtractArchiveDelegate(string directory);

	private delegate void ExtractFileByFileNameDelegate(string fileName, Stream stream);

	private delegate void ExtractFileByIndexDelegate(int index, Stream stream);

	private delegate void ExtractFiles1Delegate(string directory, int[] indexes);

	private delegate void ExtractFiles2Delegate(string directory, string[] fileNames);

	private delegate void ExtractFiles3Delegate(ExtractFileCallback extractFileCallback);

	private List<ArchiveFileInfo> _archiveFileData;

	private IInArchive _archive;

	private IInStream _archiveStream;

	private int _offset;

	private ArchiveOpenCallback _openCallback;

	private string _fileName;

	private Stream _inStream;

	private long? _packedSize;

	private long? _unpackedSize;

	private uint? _filesCount;

	private bool? _isSolid;

	private bool _opened;

	private bool _disposed;

	private InArchiveFormat _format = (InArchiveFormat)(-1);

	private ReadOnlyCollection<ArchiveFileInfo> _archiveFileInfoCollection;

	private ReadOnlyCollection<ArchiveProperty> _archiveProperties;

	private ReadOnlyCollection<string> _volumeFileNames;

	private bool _asynchronousDisposeLock;

	public string FileName
	{
		get
		{
			DisposedCheck();
			return _fileName;
		}
	}

	public long PackedSize
	{
		get
		{
			DisposedCheck();
			long? packedSize = _packedSize;
			if (!packedSize.HasValue)
			{
				if (_fileName == null)
				{
					return -1L;
				}
				return new FileInfo(_fileName).Length;
			}
			return packedSize.GetValueOrDefault();
		}
	}

	public long UnpackedSize
	{
		get
		{
			DisposedCheck();
			if (!_unpackedSize.HasValue)
			{
				return -1L;
			}
			return _unpackedSize.Value;
		}
	}

	public bool IsSolid
	{
		get
		{
			DisposedCheck();
			if (!_isSolid.HasValue)
			{
				GetArchiveInfo(disposeStream: true);
			}
			return _isSolid.Value;
		}
	}

	[CLSCompliant(false)]
	public uint FilesCount
	{
		get
		{
			DisposedCheck();
			if (!_filesCount.HasValue)
			{
				GetArchiveInfo(disposeStream: true);
			}
			return _filesCount.Value;
		}
	}

	public InArchiveFormat Format
	{
		get
		{
			DisposedCheck();
			return _format;
		}
	}

	public bool PreserveDirectoryStructure { get; set; }

	public ReadOnlyCollection<ArchiveFileInfo> ArchiveFileData
	{
		get
		{
			DisposedCheck();
			InitArchiveFileData(disposeStream: true);
			return _archiveFileInfoCollection;
		}
	}

	public ReadOnlyCollection<ArchiveProperty> ArchiveProperties
	{
		get
		{
			DisposedCheck();
			InitArchiveFileData(disposeStream: true);
			return _archiveProperties;
		}
	}

	public ReadOnlyCollection<string> ArchiveFileNames
	{
		get
		{
			DisposedCheck();
			InitArchiveFileData(disposeStream: true);
			List<string> list = new List<string>(_archiveFileData.Count);
			list.AddRange(_archiveFileData.Select((ArchiveFileInfo afi) => afi.FileName));
			return new ReadOnlyCollection<string>(list);
		}
	}

	public ReadOnlyCollection<string> VolumeFileNames
	{
		get
		{
			DisposedCheck();
			InitArchiveFileData(disposeStream: true);
			return _volumeFileNames;
		}
	}

	public event EventHandler<FileInfoEventArgs> FileExtractionStarted;

	public event EventHandler<FileInfoEventArgs> FileExtractionFinished;

	public event EventHandler<EventArgs> ExtractionFinished;

	public event EventHandler<ProgressEventArgs> Extracting;

	public event EventHandler<FileOverwriteEventArgs> FileExists;

	private void Init(string archiveFullName)
	{
		_fileName = archiveFullName;
		bool isExecutable = false;
		if (_format == (InArchiveFormat)(-1))
		{
			_format = FileChecker.CheckSignature(archiveFullName, out _offset, out isExecutable);
		}
		PreserveDirectoryStructure = true;
		SevenZipLibraryManager.LoadLibrary(this, _format);
		try
		{
			_archive = SevenZipLibraryManager.InArchive(_format, this);
		}
		catch (SevenZipLibraryException)
		{
			SevenZipLibraryManager.FreeLibrary(this, _format);
			throw;
		}
		if (isExecutable && _format != InArchiveFormat.PE && !Check())
		{
			CommonDispose();
			_format = InArchiveFormat.PE;
			SevenZipLibraryManager.LoadLibrary(this, _format);
			try
			{
				_archive = SevenZipLibraryManager.InArchive(_format, this);
			}
			catch (SevenZipLibraryException)
			{
				SevenZipLibraryManager.FreeLibrary(this, _format);
				throw;
			}
		}
	}

	private void Init(Stream stream)
	{
		ValidateStream(stream);
		bool isExecutable = false;
		if (_format == (InArchiveFormat)(-1))
		{
			_format = FileChecker.CheckSignature(stream, out _offset, out isExecutable);
		}
		PreserveDirectoryStructure = true;
		SevenZipLibraryManager.LoadLibrary(this, _format);
		try
		{
			_inStream = new ArchiveEmulationStreamProxy(stream, _offset);
			_packedSize = stream.Length;
			_archive = SevenZipLibraryManager.InArchive(_format, this);
		}
		catch (SevenZipLibraryException)
		{
			SevenZipLibraryManager.FreeLibrary(this, _format);
			throw;
		}
		if (isExecutable && _format != InArchiveFormat.PE && !Check())
		{
			CommonDispose();
			_format = InArchiveFormat.PE;
			try
			{
				_inStream = new ArchiveEmulationStreamProxy(stream, _offset);
				_packedSize = stream.Length;
				_archive = SevenZipLibraryManager.InArchive(_format, this);
			}
			catch (SevenZipLibraryException)
			{
				SevenZipLibraryManager.FreeLibrary(this, _format);
				throw;
			}
		}
	}

	public SevenZipExtractor(Stream archiveStream)
	{
		Init(archiveStream);
	}

	public SevenZipExtractor(Stream archiveStream, InArchiveFormat format)
	{
		_format = format;
		Init(archiveStream);
	}

	public SevenZipExtractor(string archiveFullName)
	{
		Init(archiveFullName);
	}

	public SevenZipExtractor(string archiveFullName, InArchiveFormat format)
	{
		_format = format;
		Init(archiveFullName);
	}

	public SevenZipExtractor(string archiveFullName, string password)
		: base(password)
	{
		Init(archiveFullName);
	}

	public SevenZipExtractor(string archiveFullName, string password, InArchiveFormat format)
		: base(password)
	{
		_format = format;
		Init(archiveFullName);
	}

	public SevenZipExtractor(Stream archiveStream, string password)
		: base(password)
	{
		Init(archiveStream);
	}

	public SevenZipExtractor(Stream archiveStream, string password, InArchiveFormat format)
		: base(password)
	{
		_format = format;
		Init(archiveStream);
	}

	private void DisposedCheck()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("SevenZipExtractor");
		}
		RecreateInstanceIfNeeded();
	}

	private ArchiveOpenCallback GetArchiveOpenCallback()
	{
		return _openCallback ?? (_openCallback = (string.IsNullOrEmpty(base.Password) ? new ArchiveOpenCallback(_fileName) : new ArchiveOpenCallback(_fileName, base.Password)));
	}

	private IInStream GetArchiveStream(bool dispose)
	{
		if (_archiveStream != null)
		{
			if (_archiveStream is DisposeVariableWrapper)
			{
				(_archiveStream as DisposeVariableWrapper).DisposeStream = dispose;
			}
			return _archiveStream;
		}
		if (_inStream != null)
		{
			_inStream.Seek(0L, SeekOrigin.Begin);
			_archiveStream = new InStreamWrapper(_inStream, disposeStream: false);
		}
		else if (!_fileName.EndsWith(".001", StringComparison.OrdinalIgnoreCase))
		{
			_archiveStream = new InStreamWrapper(new ArchiveEmulationStreamProxy(new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), _offset), dispose);
		}
		else
		{
			_archiveStream = new InMultiStreamWrapper(_fileName, dispose);
			_packedSize = (_archiveStream as InMultiStreamWrapper).Length;
		}
		return _archiveStream;
	}

	private OperationResult OpenArchiveInner(IInStream archiveStream, IArchiveOpenCallback openCallback)
	{
		ulong maxCheckStartPosition = 32768uL;
		return (OperationResult)_archive.Open(archiveStream, ref maxCheckStartPosition, openCallback);
	}

	private bool OpenArchive(IInStream archiveStream, ArchiveOpenCallback openCallback)
	{
		if (!_opened)
		{
			if (OpenArchiveInner(archiveStream, openCallback) != OperationResult.Ok && !ThrowException(null, new SevenZipArchiveException()))
			{
				return false;
			}
			_volumeFileNames = new ReadOnlyCollection<string>(openCallback.VolumeFileNames);
			_opened = true;
		}
		return true;
	}

	private void GetArchiveInfo(bool disposeStream)
	{
		if (_archive == null)
		{
			ThrowException(null, new SevenZipArchiveException());
			return;
		}
		IInStream archiveStream;
		using ((archiveStream = GetArchiveStream(disposeStream)) as IDisposable)
		{
			ArchiveOpenCallback archiveOpenCallback = GetArchiveOpenCallback();
			if (!_opened)
			{
				if (!OpenArchive(archiveStream, archiveOpenCallback))
				{
					return;
				}
				_opened = !disposeStream;
			}
			_filesCount = _archive.GetNumberOfItems();
			_archiveFileData = new List<ArchiveFileInfo>((int)_filesCount.Value);
			if (_filesCount != 0)
			{
				PropVariant value = default(PropVariant);
				try
				{
					for (uint num = 0u; num < _filesCount; num++)
					{
						try
						{
							ArchiveFileInfo item = new ArchiveFileInfo
							{
								Index = (int)num
							};
							_archive.GetProperty(num, ItemPropId.Path, ref value);
							item.FileName = NativeMethods.SafeCast(value, "[no name]");
							_archive.GetProperty(num, ItemPropId.LastWriteTime, ref value);
							item.LastWriteTime = NativeMethods.SafeCast(value, DateTime.Now);
							_archive.GetProperty(num, ItemPropId.CreationTime, ref value);
							item.CreationTime = NativeMethods.SafeCast(value, DateTime.Now);
							_archive.GetProperty(num, ItemPropId.LastAccessTime, ref value);
							item.LastAccessTime = NativeMethods.SafeCast(value, DateTime.Now);
							_archive.GetProperty(num, ItemPropId.Size, ref value);
							item.Size = NativeMethods.SafeCast(value, 0uL);
							if (item.Size == 0L)
							{
								item.Size = NativeMethods.SafeCast(value, 0u);
							}
							_archive.GetProperty(num, ItemPropId.Attributes, ref value);
							item.Attributes = NativeMethods.SafeCast(value, 0u);
							_archive.GetProperty(num, ItemPropId.IsDirectory, ref value);
							item.IsDirectory = NativeMethods.SafeCast(value, def: false);
							_archive.GetProperty(num, ItemPropId.Encrypted, ref value);
							item.Encrypted = NativeMethods.SafeCast(value, def: false);
							_archive.GetProperty(num, ItemPropId.Crc, ref value);
							item.Crc = NativeMethods.SafeCast(value, 0u);
							_archive.GetProperty(num, ItemPropId.Comment, ref value);
							item.Comment = NativeMethods.SafeCast(value, "");
							_archive.GetProperty(num, ItemPropId.Method, ref value);
							item.Method = NativeMethods.SafeCast(value, "");
							_archiveFileData.Add(item);
						}
						catch (InvalidCastException)
						{
							ThrowException(null, new SevenZipArchiveException("probably archive is corrupted."));
						}
					}
					uint numberOfArchiveProperties = _archive.GetNumberOfArchiveProperties();
					List<ArchiveProperty> list = new List<ArchiveProperty>((int)numberOfArchiveProperties);
					for (uint num2 = 0u; num2 < numberOfArchiveProperties; num2++)
					{
						_archive.GetArchivePropertyInfo(num2, out var _, out var propId, out var _);
						_archive.GetArchiveProperty(propId, ref value);
						if (propId == ItemPropId.Solid)
						{
							_isSolid = NativeMethods.SafeCast(value, def: true);
						}
						if (PropIdToName.PropIdNames.ContainsKey(propId))
						{
							list.Add(new ArchiveProperty
							{
								Name = PropIdToName.PropIdNames[propId],
								Value = value.Object
							});
						}
					}
					_archiveProperties = new ReadOnlyCollection<ArchiveProperty>(list);
					if (!_isSolid.HasValue && _format == InArchiveFormat.Zip)
					{
						_isSolid = false;
					}
					if (!_isSolid.HasValue)
					{
						_isSolid = true;
					}
				}
				catch (Exception)
				{
					if (archiveOpenCallback.ThrowException())
					{
						throw;
					}
				}
			}
		}
		if (disposeStream)
		{
			_archive.Close();
			_archiveStream = null;
		}
		_archiveFileInfoCollection = new ReadOnlyCollection<ArchiveFileInfo>(_archiveFileData);
	}

	private void InitArchiveFileData(bool disposeStream)
	{
		if (_archiveFileData == null)
		{
			GetArchiveInfo(disposeStream);
		}
	}

	private static uint[] SolidIndexes(uint[] indexes)
	{
		int num = indexes.Aggregate(0, (int current, uint i) => Math.Max(current, (int)i));
		if (num > 0)
		{
			num++;
			uint[] array = new uint[num];
			for (int num2 = 0; num2 < num; num2++)
			{
				array[num2] = (uint)num2;
			}
			return array;
		}
		return indexes;
	}

	private static bool CheckIndexes(params int[] indexes)
	{
		return indexes.All((int i) => i >= 0);
	}

	private void ArchiveExtractCallbackCommonInit(ArchiveExtractCallback aec)
	{
		aec.Open += delegate(object? s, OpenEventArgs e)
		{
			_unpackedSize = (long)e.TotalSize;
		};
		aec.FileExtractionStarted += FileExtractionStartedEventProxy;
		aec.FileExtractionFinished += FileExtractionFinishedEventProxy;
		aec.Extracting += ExtractingEventProxy;
		aec.FileExists += FileExistsEventProxy;
	}

	private ArchiveExtractCallback GetArchiveExtractCallback(string directory, int filesCount, List<uint> actualIndexes)
	{
		ArchiveExtractCallback archiveExtractCallback = (string.IsNullOrEmpty(base.Password) ? new ArchiveExtractCallback(_archive, directory, filesCount, PreserveDirectoryStructure, actualIndexes, this) : new ArchiveExtractCallback(_archive, directory, filesCount, PreserveDirectoryStructure, actualIndexes, base.Password, this));
		ArchiveExtractCallbackCommonInit(archiveExtractCallback);
		return archiveExtractCallback;
	}

	private ArchiveExtractCallback GetArchiveExtractCallback(Stream stream, uint index, int filesCount)
	{
		ArchiveExtractCallback archiveExtractCallback = (string.IsNullOrEmpty(base.Password) ? new ArchiveExtractCallback(_archive, stream, filesCount, index, this) : new ArchiveExtractCallback(_archive, stream, filesCount, index, base.Password, this));
		ArchiveExtractCallbackCommonInit(archiveExtractCallback);
		return archiveExtractCallback;
	}

	private void FreeArchiveExtractCallback(ArchiveExtractCallback callback)
	{
		callback.Open -= delegate(object? s, OpenEventArgs e)
		{
			_unpackedSize = (long)e.TotalSize;
		};
		callback.FileExtractionStarted -= FileExtractionStartedEventProxy;
		callback.FileExtractionFinished -= FileExtractionFinishedEventProxy;
		callback.Extracting -= ExtractingEventProxy;
		callback.FileExists -= FileExistsEventProxy;
	}

	private static void ValidateStream(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (!stream.CanSeek || !stream.CanRead)
		{
			throw new ArgumentException("The specified stream can not seek or read.", "stream");
		}
		if (stream.Length == 0L)
		{
			throw new ArgumentException("The specified stream has zero length.", "stream");
		}
	}

	private void CommonDispose()
	{
		if (_opened)
		{
			try
			{
				_archive?.Close();
			}
			catch (Exception)
			{
			}
		}
		_archive = null;
		_archiveFileData = null;
		_archiveProperties = null;
		_archiveFileInfoCollection = null;
		if (_inStream != null)
		{
			_inStream.Dispose();
			_inStream = null;
		}
		if (_openCallback != null)
		{
			try
			{
				_openCallback.Dispose();
			}
			catch (ObjectDisposedException)
			{
			}
			_openCallback = null;
		}
		if (_archiveStream != null && _archiveStream is IDisposable)
		{
			try
			{
				if (_archiveStream is DisposeVariableWrapper)
				{
					(_archiveStream as DisposeVariableWrapper).DisposeStream = true;
				}
				(_archiveStream as IDisposable).Dispose();
			}
			catch (ObjectDisposedException)
			{
			}
			_archiveStream = null;
		}
		SevenZipLibraryManager.FreeLibrary(this, _format);
	}

	public void Dispose()
	{
		if (_asynchronousDisposeLock)
		{
			throw new InvalidOperationException("SevenZipExtractor instance must not be disposed while making an asynchronous method call.");
		}
		if (!_disposed)
		{
			CommonDispose();
		}
		_disposed = true;
		GC.SuppressFinalize(this);
	}

	private void FileExtractionStartedEventProxy(object sender, FileInfoEventArgs e)
	{
		OnEvent(this.FileExtractionStarted, e, synchronous: true);
	}

	private void FileExtractionFinishedEventProxy(object sender, FileInfoEventArgs e)
	{
		OnEvent(this.FileExtractionFinished, e, synchronous: true);
	}

	private void ExtractingEventProxy(object sender, ProgressEventArgs e)
	{
		OnEvent(this.Extracting, e, synchronous: false);
	}

	private void FileExistsEventProxy(object sender, FileOverwriteEventArgs e)
	{
		OnEvent(this.FileExists, e, synchronous: true);
	}

	public bool Check()
	{
		DisposedCheck();
		try
		{
			InitArchiveFileData(disposeStream: false);
			IInStream archiveStream = GetArchiveStream(dispose: true);
			ArchiveOpenCallback archiveOpenCallback = GetArchiveOpenCallback();
			if (!OpenArchive(archiveStream, archiveOpenCallback))
			{
				return false;
			}
			using ArchiveExtractCallback archiveExtractCallback = GetArchiveExtractCallback("", (int)_filesCount.Value, null);
			try
			{
				CheckedExecute(_archive.Extract(null, uint.MaxValue, 1, archiveExtractCallback), "The extraction has failed for an unknown reason with code ", archiveExtractCallback);
			}
			finally
			{
				FreeArchiveExtractCallback(archiveExtractCallback);
			}
		}
		catch (Exception)
		{
			return false;
		}
		finally
		{
			_archive?.Close();
			if (_archiveStream is IDisposable)
			{
				((IDisposable)_archiveStream).Dispose();
			}
			_archiveStream = null;
			_opened = false;
		}
		return true;
	}

	public void ExtractFile(string fileName, Stream stream)
	{
		DisposedCheck();
		InitArchiveFileData(disposeStream: false);
		int num = -1;
		foreach (ArchiveFileInfo archiveFileDatum in _archiveFileData)
		{
			if (archiveFileDatum.FileName == fileName && !archiveFileDatum.IsDirectory)
			{
				num = archiveFileDatum.Index;
				break;
			}
		}
		if (num == -1)
		{
			ThrowException(null, new ArgumentOutOfRangeException("fileName", "The specified file name was not found in the archive file table."));
		}
		else
		{
			ExtractFile(num, stream);
		}
	}

	public void ExtractFile(int index, Stream stream)
	{
		DisposedCheck();
		ClearExceptions();
		if ((!CheckIndexes(index) && !ThrowException(null, new ArgumentException("The index must be more or equal to zero.", "index"))) || (!stream.CanWrite && !ThrowException(null, new ArgumentException("The specified stream can not be written.", "stream"))))
		{
			return;
		}
		InitArchiveFileData(disposeStream: false);
		if (index > _filesCount - 1 && !ThrowException(null, new ArgumentOutOfRangeException("index", "The specified index is greater than the archive files count.")))
		{
			return;
		}
		IInStream archiveStream = GetArchiveStream(dispose: false);
		ArchiveOpenCallback archiveOpenCallback = GetArchiveOpenCallback();
		if (!OpenArchive(archiveStream, archiveOpenCallback))
		{
			return;
		}
		try
		{
			uint[] array = new uint[1] { (uint)index };
			ArchiveFileInfo archiveFileInfo = _archiveFileData[index];
			if (_isSolid.Value && !archiveFileInfo.Method.Equals("Copy", StringComparison.InvariantCultureIgnoreCase))
			{
				array = SolidIndexes(array);
			}
			using ArchiveExtractCallback archiveExtractCallback = GetArchiveExtractCallback(stream, (uint)index, array.Length);
			try
			{
				CheckedExecute(_archive.Extract(array, (uint)array.Length, 0, archiveExtractCallback), "The extraction has failed for an unknown reason with code ", archiveExtractCallback);
			}
			finally
			{
				FreeArchiveExtractCallback(archiveExtractCallback);
			}
		}
		catch (Exception)
		{
			if (archiveOpenCallback.ThrowException())
			{
				throw;
			}
		}
		OnEvent(this.ExtractionFinished, EventArgs.Empty, synchronous: false);
		ThrowUserException();
	}

	public void ExtractFiles(string directory, params int[] indexes)
	{
		DisposedCheck();
		ClearExceptions();
		if (!CheckIndexes(indexes) && !ThrowException(null, new ArgumentException("The indexes must be more or equal to zero.", "indexes")))
		{
			return;
		}
		InitArchiveFileData(disposeStream: false);
		uint[] array = new uint[indexes.Length];
		for (int i = 0; i < indexes.Length; i++)
		{
			array[i] = (uint)indexes[i];
		}
		if (array.Where((uint num) => num >= _filesCount).Any((uint num) => !ThrowException(null, new ArgumentOutOfRangeException("indexes", "Index must be less than " + _filesCount.Value.ToString(CultureInfo.InvariantCulture) + "!"))))
		{
			return;
		}
		List<uint> list = new List<uint>(array);
		list.Sort();
		array = list.ToArray();
		if (_isSolid.Value)
		{
			array = SolidIndexes(array);
		}
		try
		{
			IInStream archiveStream;
			using ((archiveStream = GetArchiveStream(list.Count != 1)) as IDisposable)
			{
				ArchiveOpenCallback archiveOpenCallback = GetArchiveOpenCallback();
				if (!OpenArchive(archiveStream, archiveOpenCallback))
				{
					return;
				}
				try
				{
					using ArchiveExtractCallback archiveExtractCallback = GetArchiveExtractCallback(directory, (int)_filesCount.Value, list);
					try
					{
						CheckedExecute(_archive.Extract(array, (uint)array.Length, 0, archiveExtractCallback), "The extraction has failed for an unknown reason with code ", archiveExtractCallback);
					}
					finally
					{
						FreeArchiveExtractCallback(archiveExtractCallback);
					}
				}
				catch (Exception)
				{
					if (archiveOpenCallback.ThrowException())
					{
						throw;
					}
				}
			}
			OnEvent(this.ExtractionFinished, EventArgs.Empty, synchronous: false);
		}
		finally
		{
			if (list.Count > 1)
			{
				_archive?.Close();
				_archiveStream = null;
				_opened = false;
			}
		}
		ThrowUserException();
	}

	public void ExtractFiles(string directory, params string[] fileNames)
	{
		DisposedCheck();
		InitArchiveFileData(disposeStream: false);
		List<int> list = new List<int>(fileNames.Length);
		List<string> list2 = new List<string>(ArchiveFileNames);
		foreach (string text in fileNames)
		{
			if (!list2.Contains(text))
			{
				if (!ThrowException(null, new ArgumentOutOfRangeException("fileNames", "File \"" + text + "\" was not found in the archive file table.")))
				{
					return;
				}
				continue;
			}
			foreach (ArchiveFileInfo archiveFileDatum in _archiveFileData)
			{
				if (archiveFileDatum.FileName == text && !archiveFileDatum.IsDirectory)
				{
					list.Add(archiveFileDatum.Index);
					break;
				}
			}
		}
		ExtractFiles(directory, list.ToArray());
	}

	public void ExtractFiles(ExtractFileCallback extractFileCallback)
	{
		DisposedCheck();
		InitArchiveFileData(disposeStream: false);
		if (IsSolid)
		{
			throw new SevenZipExtractionFailedException("Solid archives are not supported.");
		}
		foreach (ArchiveFileInfo archiveFileDatum in ArchiveFileData)
		{
			ExtractFileCallbackArgs extractFileCallbackArgs = new ExtractFileCallbackArgs(archiveFileDatum);
			extractFileCallback(extractFileCallbackArgs);
			if (extractFileCallbackArgs.CancelExtraction)
			{
				break;
			}
			if (extractFileCallbackArgs.ExtractToStream == null && extractFileCallbackArgs.ExtractToFile == null)
			{
				continue;
			}
			bool flag = false;
			try
			{
				if (extractFileCallbackArgs.ExtractToStream != null)
				{
					ExtractFile(archiveFileDatum.Index, extractFileCallbackArgs.ExtractToStream);
				}
				else
				{
					using FileStream stream = new FileStream(extractFileCallbackArgs.ExtractToFile, FileMode.CreateNew, FileAccess.Write, FileShare.None, 8192);
					ExtractFile(archiveFileDatum.Index, stream);
				}
				flag = true;
			}
			catch (Exception ex)
			{
				Exception ex2 = (extractFileCallbackArgs.Exception = ex);
				extractFileCallbackArgs.Reason = ExtractFileCallbackReason.Failure;
				extractFileCallback(extractFileCallbackArgs);
				if (!ThrowException(null, ex2))
				{
					break;
				}
			}
			if (flag)
			{
				extractFileCallbackArgs.Reason = ExtractFileCallbackReason.Done;
				extractFileCallback(extractFileCallbackArgs);
			}
		}
	}

	public void ExtractArchive(string directory)
	{
		DisposedCheck();
		ClearExceptions();
		InitArchiveFileData(disposeStream: false);
		try
		{
			IInStream archiveStream;
			using ((archiveStream = GetArchiveStream(dispose: true)) as IDisposable)
			{
				ArchiveOpenCallback archiveOpenCallback = GetArchiveOpenCallback();
				if (!OpenArchive(archiveStream, archiveOpenCallback))
				{
					return;
				}
				try
				{
					using ArchiveExtractCallback archiveExtractCallback = GetArchiveExtractCallback(directory, (int)_filesCount.Value, null);
					try
					{
						CheckedExecute(_archive.Extract(null, uint.MaxValue, 0, archiveExtractCallback), "The extraction has failed for an unknown reason with code ", archiveExtractCallback);
						OnEvent(this.ExtractionFinished, EventArgs.Empty, synchronous: false);
					}
					finally
					{
						FreeArchiveExtractCallback(archiveExtractCallback);
					}
				}
				catch (Exception)
				{
					if (archiveOpenCallback.ThrowException())
					{
						throw;
					}
				}
			}
		}
		finally
		{
			_archive?.Close();
			_archiveStream = null;
			_opened = false;
		}
		ThrowUserException();
	}

	internal static byte[] GetLzmaProperties(Stream inStream, out long outSize)
	{
		byte[] array = new byte[5];
		if (inStream.Read(array, 0, 5) != 5)
		{
			throw new LzmaException();
		}
		outSize = 0L;
		for (int i = 0; i < 8; i++)
		{
			int num = inStream.ReadByte();
			if (num < 0)
			{
				throw new LzmaException();
			}
			outSize |= (long)((ulong)(byte)num << (i << 3));
		}
		return array;
	}

	public static void DecompressStream(Stream inStream, Stream outStream, int? inLength, EventHandler<ProgressEventArgs> codeProgressEvent)
	{
		if (!inStream.CanRead || !outStream.CanWrite)
		{
			throw new ArgumentException("The specified streams are invalid.");
		}
		Decoder decoder = new Decoder();
		long inSize = (((long?)inLength) ?? inStream.Length) - inStream.Position;
		decoder.SetDecoderProperties(GetLzmaProperties(inStream, out var outSize));
		decoder.Code(inStream, outStream, inSize, outSize, new LzmaProgressCallback(inSize, codeProgressEvent));
	}

	public static byte[] ExtractBytes(byte[] data)
	{
		using MemoryStream memoryStream = new MemoryStream(data);
		Decoder decoder = new Decoder();
		memoryStream.Seek(0L, SeekOrigin.Begin);
		using MemoryStream memoryStream2 = new MemoryStream();
		decoder.SetDecoderProperties(GetLzmaProperties(memoryStream, out var outSize));
		decoder.Code(memoryStream, memoryStream2, memoryStream.Length - memoryStream.Position, outSize, null);
		return memoryStream2.ToArray();
	}

	private void RecreateInstanceIfNeeded()
	{
		if (NeedsToBeRecreated)
		{
			NeedsToBeRecreated = false;
			Stream stream = null;
			string archiveFullName = null;
			if (string.IsNullOrEmpty(_fileName))
			{
				stream = _inStream;
			}
			else
			{
				archiveFullName = _fileName;
			}
			CommonDispose();
			if (stream == null)
			{
				Init(archiveFullName);
			}
			else
			{
				Init(stream);
			}
		}
	}

	internal override void SaveContext()
	{
		DisposedCheck();
		_asynchronousDisposeLock = true;
		base.SaveContext();
	}

	internal override void ReleaseContext()
	{
		base.ReleaseContext();
		_asynchronousDisposeLock = false;
	}

	public void BeginExtractArchive(string directory)
	{
		SaveContext();
		Task.Run(delegate
		{
			new ExtractArchiveDelegate(ExtractArchive)(directory);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public async Task ExtractArchiveAsync(string directory)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new ExtractArchiveDelegate(ExtractArchive)(directory);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public void BeginExtractFile(string fileName, Stream stream)
	{
		SaveContext();
		Task.Run(delegate
		{
			new ExtractFileByFileNameDelegate(ExtractFile)(fileName, stream);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public async Task ExtractFileAsync(string fileName, Stream stream)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new ExtractFileByFileNameDelegate(ExtractFile)(fileName, stream);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public void BeginExtractFile(int index, Stream stream)
	{
		SaveContext();
		Task.Run(delegate
		{
			new ExtractFileByIndexDelegate(ExtractFile)(index, stream);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public async Task ExtractFileAsync(int index, Stream stream)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new ExtractFileByIndexDelegate(ExtractFile)(index, stream);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public void BeginExtractFiles(string directory, params int[] indexes)
	{
		SaveContext();
		Task.Run(delegate
		{
			new ExtractFiles1Delegate(ExtractFiles)(directory, indexes);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public async Task ExtractFilesAsync(string directory, params int[] indexes)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new ExtractFiles1Delegate(ExtractFiles)(directory, indexes);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public void BeginExtractFiles(string directory, params string[] fileNames)
	{
		SaveContext();
		Task.Run(delegate
		{
			new ExtractFiles2Delegate(ExtractFiles)(directory, fileNames);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public async Task ExtractFilesAsync(string directory, params string[] fileNames)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new ExtractFiles2Delegate(ExtractFiles)(directory, fileNames);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}

	public void BeginExtractFiles(ExtractFileCallback extractFileCallback)
	{
		SaveContext();
		Task.Run(delegate
		{
			new ExtractFiles3Delegate(ExtractFiles)(extractFileCallback);
		}).ContinueWith(delegate
		{
			ReleaseContext();
		});
	}

	public async Task ExtractFilesAsync(ExtractFileCallback extractFileCallback)
	{
		try
		{
			SaveContext();
			await Task.Run(delegate
			{
				new ExtractFiles3Delegate(ExtractFiles)(extractFileCallback);
			});
		}
		finally
		{
			ReleaseContext();
		}
	}
}
