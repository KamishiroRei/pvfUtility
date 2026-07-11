using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SevenZip;

internal sealed class ArchiveUpdateCallback : CallbackBase, IArchiveUpdateCallback, ICryptoGetTextPassword2, IDisposable
{
	private int _actualFilesCount;

	private long _bytesCount;

	private long _bytesWritten;

	private long _bytesWrittenOld;

	private SevenZipCompressor _compressor;

	private bool _directoryStructure;

	private float _doneRate;

	private string[] _entries;

	private FileInfo[] _files;

	private InStreamWrapper _fileStream;

	private uint _indexInArchive;

	private uint _indexOffset;

	private int _rootLength;

	private Stream[] _streams;

	private UpdateData _updateData;

	private List<InStreamWrapper> _wrappersToDispose;

	private int _memoryPressure;

	public string DefaultItemName { private get; set; }

	public bool FastCompression { private get; set; }

	public float DictionarySize
	{
		set
		{
			_memoryPressure = (int)(value * 1024f * 1024f);
			GC.AddMemoryPressure(_memoryPressure);
		}
	}

	public event EventHandler<FileNameEventArgs> FileCompressionStarted;

	public event EventHandler<ProgressEventArgs> Compressing;

	public event EventHandler FileCompressionFinished;

	public ArchiveUpdateCallback(FileInfo[] files, int rootLength, SevenZipCompressor compressor, UpdateData updateData, bool directoryStructure)
	{
		Init(files, rootLength, compressor, updateData, directoryStructure);
	}

	public ArchiveUpdateCallback(FileInfo[] files, int rootLength, string password, SevenZipCompressor compressor, UpdateData updateData, bool directoryStructure)
		: base(password)
	{
		Init(files, rootLength, compressor, updateData, directoryStructure);
	}

	public ArchiveUpdateCallback(Stream stream, SevenZipCompressor compressor, UpdateData updateData, bool directoryStructure)
	{
		Init(stream, compressor, updateData, directoryStructure);
	}

	public ArchiveUpdateCallback(Stream stream, string password, SevenZipCompressor compressor, UpdateData updateData, bool directoryStructure)
		: base(password)
	{
		Init(stream, compressor, updateData, directoryStructure);
	}

	public ArchiveUpdateCallback(IDictionary<string, Stream> streamDict, SevenZipCompressor compressor, UpdateData updateData, bool directoryStructure)
	{
		Init(streamDict, compressor, updateData, directoryStructure);
	}

	public ArchiveUpdateCallback(IDictionary<string, Stream> streamDict, string password, SevenZipCompressor compressor, UpdateData updateData, bool directoryStructure)
		: base(password)
	{
		Init(streamDict, compressor, updateData, directoryStructure);
	}

	private void CommonInit(SevenZipCompressor compressor, UpdateData updateData, bool directoryStructure)
	{
		_compressor = compressor;
		_indexInArchive = updateData.FilesCount;
		_indexOffset = ((updateData.Mode == InternalCompressionMode.Append) ? _indexInArchive : 0u);
		if (_compressor.ArchiveFormat == OutArchiveFormat.Zip)
		{
			_wrappersToDispose = new List<InStreamWrapper>();
		}
		_updateData = updateData;
		_directoryStructure = directoryStructure;
		DefaultItemName = "default";
	}

	private void Init(FileInfo[] files, int rootLength, SevenZipCompressor compressor, UpdateData updateData, bool directoryStructure)
	{
		_files = files;
		_rootLength = rootLength;
		if (files != null)
		{
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo.Exists)
				{
					_bytesCount += fileInfo.Length;
					if ((fileInfo.Attributes & FileAttributes.Directory) == 0)
					{
						_actualFilesCount++;
					}
				}
			}
		}
		CommonInit(compressor, updateData, directoryStructure);
	}

	private void Init(Stream stream, SevenZipCompressor compressor, UpdateData updateData, bool directoryStructure)
	{
		_fileStream = new InStreamWrapper(stream, disposeStream: false);
		_fileStream.BytesRead += IntEventArgsHandler;
		_actualFilesCount = 1;
		try
		{
			_bytesCount = stream.Length;
		}
		catch (NotSupportedException)
		{
			_bytesCount = -1L;
		}
		try
		{
			stream.Seek(0L, SeekOrigin.Begin);
		}
		catch (NotSupportedException)
		{
			_bytesCount = -1L;
		}
		CommonInit(compressor, updateData, directoryStructure);
	}

	private void Init(IDictionary<string, Stream> streamDict, SevenZipCompressor compressor, UpdateData updateData, bool directoryStructure)
	{
		_streams = new Stream[streamDict.Count];
		streamDict.Values.CopyTo(_streams, 0);
		_entries = new string[streamDict.Count];
		streamDict.Keys.CopyTo(_entries, 0);
		_actualFilesCount = streamDict.Count;
		Stream[] streams = _streams;
		foreach (Stream stream in streams)
		{
			if (stream != null)
			{
				_bytesCount += stream.Length;
			}
		}
		CommonInit(compressor, updateData, directoryStructure);
	}

	private bool EventsForGetStream(uint index)
	{
		if (!FastCompression)
		{
			if (_fileStream != null)
			{
				_fileStream.BytesRead += IntEventArgsHandler;
			}
			_doneRate += 1f / (float)_actualFilesCount;
			FileNameEventArgs e = new FileNameEventArgs((_files != null) ? _files[index].Name : _entries[index], PercentDoneEventArgs.ProducePercentDone(_doneRate));
			OnFileCompression(e);
			if (e.Cancel)
			{
				base.Canceled = true;
				return false;
			}
		}
		return true;
	}

	private void OnFileCompression(FileNameEventArgs e)
	{
		if (this.FileCompressionStarted != null)
		{
			this.FileCompressionStarted(this, e);
		}
	}

	private void OnCompressing(ProgressEventArgs e)
	{
		if (this.Compressing != null)
		{
			this.Compressing(this, e);
		}
	}

	private void OnFileCompressionFinished(EventArgs e)
	{
		if (this.FileCompressionFinished != null)
		{
			this.FileCompressionFinished(this, e);
		}
	}

	public void SetTotal(ulong total)
	{
	}

	public void SetCompleted(ref ulong completeValue)
	{
	}

	public int GetUpdateItemInfo(uint index, ref int newData, ref int newProperties, ref uint indexInArchive)
	{
		switch (_updateData.Mode)
		{
		case InternalCompressionMode.Create:
			newData = 1;
			newProperties = 1;
			indexInArchive = uint.MaxValue;
			break;
		case InternalCompressionMode.Append:
			if (index < _indexInArchive)
			{
				newData = 0;
				newProperties = 0;
				indexInArchive = index;
			}
			else
			{
				newData = 1;
				newProperties = 1;
				indexInArchive = uint.MaxValue;
			}
			break;
		case InternalCompressionMode.Modify:
			newData = 0;
			newProperties = Convert.ToInt32(_updateData.FileNamesToModify.ContainsKey((int)index) && _updateData.FileNamesToModify[(int)index] != null);
			if (_updateData.FileNamesToModify.ContainsKey((int)index) && _updateData.FileNamesToModify[(int)index] == null)
			{
				indexInArchive = (uint)_updateData.ArchiveFileData.Count;
				foreach (KeyValuePair<int, string> item in _updateData.FileNamesToModify)
				{
					if (item.Key <= index && item.Value == null)
					{
						do
						{
							indexInArchive--;
						}
						while (indexInArchive != 0 && _updateData.FileNamesToModify.ContainsKey((int)indexInArchive) && _updateData.FileNamesToModify[(int)indexInArchive] == null);
					}
				}
			}
			else
			{
				indexInArchive = index;
			}
			break;
		}
		return 0;
	}

	public int GetProperty(uint index, ItemPropId propID, ref PropVariant value)
	{
		index -= _indexOffset;
		try
		{
			switch (propID)
			{
			case ItemPropId.IsAnti:
				value.VarType = VarEnum.VT_BOOL;
				value.UInt64Value = 0uL;
				break;
			case ItemPropId.Path:
			{
				value.VarType = VarEnum.VT_BSTR;
				string s = DefaultItemName;
				if (_updateData.Mode != InternalCompressionMode.Modify)
				{
					if (_files != null)
					{
						s = ((!_directoryStructure) ? _files[index].Name : ((_rootLength <= 0) ? (_files[index].FullName[0] + _files[index].FullName.Substring(2)) : _files[index].FullName.Substring(_rootLength)));
					}
					else if (_entries != null)
					{
						s = _entries[index];
					}
				}
				else
				{
					s = _updateData.FileNamesToModify[(int)index];
				}
				value.Value = Marshal.StringToBSTR(s);
				break;
			}
			case ItemPropId.IsDirectory:
				value.VarType = VarEnum.VT_BOOL;
				if (_updateData.Mode != InternalCompressionMode.Modify)
				{
					if (_files == null)
					{
						if (_streams == null)
						{
							value.UInt64Value = 0uL;
						}
						else
						{
							value.UInt64Value = (ulong)((_streams[index] == null) ? 1 : 0);
						}
					}
					else
					{
						value.UInt64Value = (byte)(_files[index].Attributes & FileAttributes.Directory);
					}
				}
				else
				{
					value.UInt64Value = Convert.ToUInt64(_updateData.ArchiveFileData[(int)index].IsDirectory);
				}
				break;
			case ItemPropId.Size:
			{
				value.VarType = VarEnum.VT_UI8;
				ulong uInt64Value = ((_updateData.Mode == InternalCompressionMode.Modify) ? _updateData.ArchiveFileData[(int)index].Size : ((ulong)((_files != null) ? (((_files[index].Attributes & FileAttributes.Directory) == 0) ? _files[index].Length : 0) : ((_streams != null) ? ((_streams[index] == null) ? 0 : _streams[index].Length) : ((_bytesCount > 0) ? _bytesCount : 0)))));
				value.UInt64Value = uInt64Value;
				break;
			}
			case ItemPropId.Attributes:
				value.VarType = VarEnum.VT_UI4;
				if (_updateData.Mode != InternalCompressionMode.Modify)
				{
					if (_files == null)
					{
						if (_streams == null)
						{
							value.UInt32Value = 128u;
						}
						else
						{
							value.UInt32Value = ((_streams[index] == null) ? 16u : 128u);
						}
					}
					else
					{
						value.UInt32Value = (uint)_files[index].Attributes;
					}
				}
				else
				{
					value.UInt32Value = _updateData.ArchiveFileData[(int)index].Attributes;
				}
				break;
			case ItemPropId.CreationTime:
				value.VarType = VarEnum.VT_FILETIME;
				if (_updateData.Mode != InternalCompressionMode.Modify)
				{
					value.Int64Value = ((_files == null) ? DateTime.Now.ToFileTime() : _files[index].CreationTime.ToFileTime());
				}
				else
				{
					value.Int64Value = _updateData.ArchiveFileData[(int)index].CreationTime.ToFileTime();
				}
				break;
			case ItemPropId.LastAccessTime:
				value.VarType = VarEnum.VT_FILETIME;
				if (_updateData.Mode != InternalCompressionMode.Modify)
				{
					value.Int64Value = ((_files == null) ? DateTime.Now.ToFileTime() : _files[index].LastAccessTime.ToFileTime());
				}
				else
				{
					value.Int64Value = _updateData.ArchiveFileData[(int)index].LastAccessTime.ToFileTime();
				}
				break;
			case ItemPropId.LastWriteTime:
				value.VarType = VarEnum.VT_FILETIME;
				if (_updateData.Mode != InternalCompressionMode.Modify)
				{
					value.Int64Value = ((_files == null) ? DateTime.Now.ToFileTime() : _files[index].LastWriteTime.ToFileTime());
				}
				else
				{
					value.Int64Value = _updateData.ArchiveFileData[(int)index].LastWriteTime.ToFileTime();
				}
				break;
			case ItemPropId.Extension:
				value.VarType = VarEnum.VT_BSTR;
				if (_updateData.Mode != InternalCompressionMode.Modify)
				{
					try
					{
						string s = ((_files != null) ? _files[index].Extension.Substring(1) : ((_entries == null) ? "" : Path.GetExtension(_entries[index])));
						value.Value = Marshal.StringToBSTR(s);
					}
					catch (ArgumentException)
					{
						value.Value = Marshal.StringToBSTR("");
					}
				}
				else
				{
					string s = Path.GetExtension(_updateData.ArchiveFileData[(int)index].FileName);
					value.Value = Marshal.StringToBSTR(s);
				}
				break;
			}
		}
		catch (Exception e)
		{
			AddException(e);
		}
		return 0;
	}

	public int GetStream(uint index, out ISequentialInStream inStream)
	{
		index -= _indexOffset;
		if (_files != null)
		{
			_fileStream = null;
			try
			{
				if (File.Exists(_files[index].FullName))
				{
					_fileStream = new InStreamWrapper(new FileStream(_files[index].FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), disposeStream: true);
				}
			}
			catch (Exception e)
			{
				AddException(e);
				inStream = null;
				return -1;
			}
			inStream = _fileStream;
			if (!EventsForGetStream(index))
			{
				return -1;
			}
		}
		else if (_streams == null)
		{
			inStream = _fileStream;
		}
		else
		{
			_fileStream = new InStreamWrapper(_streams[index], disposeStream: true);
			inStream = _fileStream;
			if (!EventsForGetStream(index))
			{
				return -1;
			}
		}
		return 0;
	}

	public long EnumProperties(IntPtr enumerator)
	{
		return 2147500033L;
	}

	public void SetOperationResult(OperationResult operationResult)
	{
		if (operationResult != OperationResult.Ok && base.ReportErrors)
		{
			switch (operationResult)
			{
			case OperationResult.CrcError:
				AddException(new ExtractionFailedException("File is corrupted. Crc check has failed."));
				break;
			case OperationResult.DataError:
				AddException(new ExtractionFailedException("File is corrupted. Data error has occurred."));
				break;
			case OperationResult.UnsupportedMethod:
				AddException(new ExtractionFailedException("Unsupported method error has occurred."));
				break;
			case OperationResult.Unavailable:
				AddException(new ExtractionFailedException("File is unavailable."));
				break;
			case OperationResult.UnexpectedEnd:
				AddException(new ExtractionFailedException("Unexpected end of file."));
				break;
			case OperationResult.DataAfterEnd:
				AddException(new ExtractionFailedException("Data after end of archive."));
				break;
			case OperationResult.IsNotArc:
				AddException(new ExtractionFailedException("File is not archive."));
				break;
			case OperationResult.HeadersError:
				AddException(new ExtractionFailedException("Archive headers error."));
				break;
			case OperationResult.WrongPassword:
				AddException(new ExtractionFailedException("Wrong password."));
				break;
			default:
				AddException(new ExtractionFailedException($"Unexpected operation result: {operationResult}"));
				break;
			}
		}
		if (_fileStream != null)
		{
			_fileStream.BytesRead -= IntEventArgsHandler;
			if (_compressor.ArchiveFormat != OutArchiveFormat.Zip)
			{
				try
				{
					_fileStream.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
			}
			else
			{
				_wrappersToDispose.Add(_fileStream);
			}
			_fileStream = null;
		}
		OnFileCompressionFinished(EventArgs.Empty);
	}

	public int CryptoGetTextPassword2(ref int passwordIsDefined, out string password)
	{
		passwordIsDefined = ((!string.IsNullOrEmpty(base.Password)) ? 1 : 0);
		password = base.Password;
		return 0;
	}

	public void Dispose()
	{
		GC.RemoveMemoryPressure(_memoryPressure);
		if (_fileStream != null)
		{
			try
			{
				_fileStream.Dispose();
			}
			catch (ObjectDisposedException)
			{
			}
		}
		if (_wrappersToDispose != null)
		{
			foreach (InStreamWrapper item in _wrappersToDispose)
			{
				try
				{
					item.Dispose();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}
		GC.SuppressFinalize(this);
	}

	private void IntEventArgsHandler(object sender, IntEventArgs e)
	{
		object obj = ((object)_files) ?? ((object)_streams);
		if (obj == null)
		{
			obj = _fileStream;
		}
		object obj2 = obj;
		lock (obj2)
		{
			byte b = (byte)(_bytesWrittenOld * 100 / _bytesCount);
			_bytesWritten += e.Value;
			byte b2 = (byte)((_bytesCount >= _bytesWritten) ? ((byte)(_bytesWritten * 100 / _bytesCount)) : 100);
			if (b2 > b)
			{
				_bytesWrittenOld = _bytesWritten;
				OnCompressing(new ProgressEventArgs(b2, (byte)(b2 - b)));
			}
		}
	}
}
