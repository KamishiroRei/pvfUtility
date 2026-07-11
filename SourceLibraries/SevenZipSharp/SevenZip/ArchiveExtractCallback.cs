using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SevenZip;

internal sealed class ArchiveExtractCallback : CallbackBase, IArchiveExtractCallback, ICryptoGetTextPassword, IDisposable
{
	private List<uint> _actualIndexes;

	private IInArchive _archive;

	private long _bytesCount;

	private long _bytesWritten;

	private long _bytesWrittenOld;

	private string _directory;

	private float _doneRate;

	private SevenZipExtractor _extractor;

	private FakeOutStreamWrapper _fakeStream;

	private uint? _fileIndex;

	private int _filesCount;

	private OutStreamWrapper _fileStream;

	private bool _directoryStructure;

	private int _currentIndex;

	private const int MemoryPressure = 67108864;

	public event EventHandler<FileInfoEventArgs> FileExtractionStarted;

	public event EventHandler<FileInfoEventArgs> FileExtractionFinished;

	public event EventHandler<OpenEventArgs> Open;

	public event EventHandler<ProgressEventArgs> Extracting;

	public event EventHandler<FileOverwriteEventArgs> FileExists;

	public ArchiveExtractCallback(IInArchive archive, string directory, int filesCount, bool directoryStructure, List<uint> actualIndexes, SevenZipExtractor extractor)
	{
		Init(archive, directory, filesCount, directoryStructure, actualIndexes, extractor);
	}

	public ArchiveExtractCallback(IInArchive archive, string directory, int filesCount, bool directoryStructure, List<uint> actualIndexes, string password, SevenZipExtractor extractor)
		: base(password)
	{
		Init(archive, directory, filesCount, directoryStructure, actualIndexes, extractor);
	}

	public ArchiveExtractCallback(IInArchive archive, Stream stream, int filesCount, uint fileIndex, SevenZipExtractor extractor)
	{
		Init(archive, stream, filesCount, fileIndex, extractor);
	}

	public ArchiveExtractCallback(IInArchive archive, Stream stream, int filesCount, uint fileIndex, string password, SevenZipExtractor extractor)
		: base(password)
	{
		Init(archive, stream, filesCount, fileIndex, extractor);
	}

	private void Init(IInArchive archive, string directory, int filesCount, bool directoryStructure, List<uint> actualIndexes, SevenZipExtractor extractor)
	{
		CommonInit(archive, filesCount, extractor);
		_directory = directory;
		_actualIndexes = actualIndexes;
		_directoryStructure = directoryStructure;
		if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString() ?? "", StringComparison.CurrentCulture))
		{
			_directory += Path.DirectorySeparatorChar;
		}
	}

	private void Init(IInArchive archive, Stream stream, int filesCount, uint fileIndex, SevenZipExtractor extractor)
	{
		CommonInit(archive, filesCount, extractor);
		_fileStream = new OutStreamWrapper(stream, disposeStream: false);
		_fileStream.BytesWritten += IntEventArgsHandler;
		_fileIndex = fileIndex;
	}

	private void CommonInit(IInArchive archive, int filesCount, SevenZipExtractor extractor)
	{
		_archive = archive;
		_filesCount = filesCount;
		_fakeStream = new FakeOutStreamWrapper();
		_fakeStream.BytesWritten += IntEventArgsHandler;
		_extractor = extractor;
		GC.AddMemoryPressure(67108864L);
	}

	private void IntEventArgsHandler(object sender, IntEventArgs e)
	{
		if (_bytesCount == 0L)
		{
			return;
		}
		int num = (int)(_bytesWrittenOld * 100 / _bytesCount);
		_bytesWritten += e.Value;
		int num2 = (int)(_bytesWritten * 100 / _bytesCount);
		if (num2 > num)
		{
			if (num2 > 100)
			{
				num = (num2 = 0);
			}
			_bytesWrittenOld = _bytesWritten;
			this.Extracting?.Invoke(this, new ProgressEventArgs((byte)num2, (byte)(num2 - num)));
		}
	}

	public void SetTotal(ulong total)
	{
		_bytesCount = (long)total;
		this.Open?.Invoke(this, new OpenEventArgs(total));
	}

	public void SetCompleted(ref ulong completeValue)
	{
	}

	public int GetStream(uint index, out ISequentialOutStream outStream, AskMode askExtractMode)
	{
		outStream = null;
		if (base.Canceled)
		{
			return -1;
		}
		_currentIndex = (int)index;
		if (askExtractMode == AskMode.Extract)
		{
			_ = _directory;
			if (!_fileIndex.HasValue)
			{
				if (_actualIndexes == null || _actualIndexes.Contains(index))
				{
					PropVariant value = default(PropVariant);
					_archive.GetProperty(index, ItemPropId.Path, ref value);
					string text = NativeMethods.SafeCast(value, "");
					if (string.IsNullOrEmpty(text))
					{
						if (_filesCount == 1)
						{
							string fileName = Path.GetFileName(_extractor.FileName);
							fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
							if (!fileName.EndsWith(".tar", StringComparison.OrdinalIgnoreCase))
							{
								fileName += ".tar";
							}
							text = fileName;
						}
						else
						{
							text = "[no name] " + index.ToString(CultureInfo.InvariantCulture);
						}
					}
					string text2;
					try
					{
						text2 = Path.Combine(RemoveIllegalCharacters(_directory, isDirectory: true), RemoveIllegalCharacters(_directoryStructure ? text : Path.GetFileName(text)));
						if (string.IsNullOrEmpty(text2))
						{
							throw new SevenZipArchiveException("Some archive name is null or empty.");
						}
					}
					catch (Exception e)
					{
						AddException(e);
						outStream = _fakeStream;
						return 0;
					}
					_archive.GetProperty(index, ItemPropId.IsDirectory, ref value);
					if (!NativeMethods.SafeCast(value, def: false))
					{
						_archive.GetProperty(index, ItemPropId.LastWriteTime, ref value);
						DateTime time = NativeMethods.SafeCast(value, DateTime.MinValue);
						if (File.Exists(text2))
						{
							FileOverwriteEventArgs e2 = new FileOverwriteEventArgs(text2);
							this.FileExists?.Invoke(this, e2);
							if (e2.Cancel)
							{
								base.Canceled = true;
								return -1;
							}
							if (string.IsNullOrEmpty(e2.FileName))
							{
								outStream = _fakeStream;
							}
							else
							{
								text2 = e2.FileName;
							}
						}
						_doneRate += 1f / (float)_filesCount;
						FileInfoEventArgs e3 = new FileInfoEventArgs(_extractor.ArchiveFileData[(int)index], PercentDoneEventArgs.ProducePercentDone(_doneRate));
						this.FileExtractionStarted?.Invoke(this, e3);
						if (e3.Cancel)
						{
							base.Canceled = true;
							return -1;
						}
						if (e3.Skip)
						{
							outStream = _fakeStream;
							return 0;
						}
						CreateDirectory(text2);
						try
						{
							_fileStream = new OutStreamWrapper(File.Create(text2), text2, time, disposeStream: true);
						}
						catch (Exception ex)
						{
							AddException((ex is FileNotFoundException) ? new IOException("The file \"" + text2 + "\" was not extracted due to the File.Create fail.") : ex);
							outStream = _fakeStream;
							return 0;
						}
						_fileStream.BytesWritten += IntEventArgsHandler;
						outStream = _fileStream;
					}
					else
					{
						_doneRate += 1f / (float)_filesCount;
						FileInfoEventArgs e4 = new FileInfoEventArgs(_extractor.ArchiveFileData[(int)index], PercentDoneEventArgs.ProducePercentDone(_doneRate));
						this.FileExtractionStarted?.Invoke(this, e4);
						if (e4.Cancel)
						{
							base.Canceled = true;
							return -1;
						}
						if (e4.Skip)
						{
							outStream = _fakeStream;
							return 0;
						}
						if (!Directory.Exists(text2))
						{
							try
							{
								Directory.CreateDirectory(text2);
							}
							catch (Exception e5)
							{
								AddException(e5);
							}
							outStream = _fakeStream;
						}
					}
				}
				else
				{
					outStream = _fakeStream;
				}
			}
			else if (index == _fileIndex)
			{
				outStream = _fileStream;
				_fileIndex = null;
			}
			else
			{
				outStream = _fakeStream;
			}
		}
		return 0;
	}

	public void PrepareOperation(AskMode askExtractMode)
	{
	}

	public void SetOperationResult(OperationResult operationResult)
	{
		if (operationResult != OperationResult.Ok && base.ReportErrors)
		{
			switch (operationResult)
			{
			case OperationResult.CrcError:
				AddException(new ExtractionFailedException("File is corrupted. Crc check has failed."));
				return;
			case OperationResult.DataError:
				AddException(new ExtractionFailedException("File is corrupted. Data error has occured."));
				return;
			case OperationResult.UnsupportedMethod:
				AddException(new ExtractionFailedException("Unsupported method error has occured."));
				return;
			case OperationResult.Unavailable:
				AddException(new ExtractionFailedException("File is unavailable."));
				return;
			case OperationResult.UnexpectedEnd:
				AddException(new ExtractionFailedException("Unexpected end of file."));
				return;
			case OperationResult.DataAfterEnd:
				AddException(new ExtractionFailedException("Data after end of archive."));
				return;
			case OperationResult.IsNotArc:
				AddException(new ExtractionFailedException("File is not archive."));
				return;
			case OperationResult.HeadersError:
				AddException(new ExtractionFailedException("Archive headers error."));
				return;
			case OperationResult.WrongPassword:
				AddException(new ExtractionFailedException("Wrong password."));
				return;
			}
			AddException(new ExtractionFailedException($"Unexpected operation result: {operationResult}"));
			return;
		}
		if (_fileStream != null && !_fileIndex.HasValue)
		{
			try
			{
				_fileStream.BytesWritten -= IntEventArgsHandler;
				_fileStream.Dispose();
			}
			catch (ObjectDisposedException)
			{
			}
			_fileStream = null;
		}
		FileInfoEventArgs e = new FileInfoEventArgs(_extractor.ArchiveFileData[_currentIndex], PercentDoneEventArgs.ProducePercentDone(_doneRate));
		this.FileExtractionFinished?.Invoke(this, e);
		if (e.Cancel)
		{
			base.Canceled = true;
		}
	}

	public int CryptoGetTextPassword(out string password)
	{
		password = base.Password;
		return 0;
	}

	public void Dispose()
	{
		GC.RemoveMemoryPressure(67108864L);
		if (_fileStream != null)
		{
			try
			{
				_fileStream.Dispose();
			}
			catch (ObjectDisposedException)
			{
			}
			_fileStream = null;
		}
		if (_fakeStream != null)
		{
			try
			{
				_fakeStream.Dispose();
			}
			catch (ObjectDisposedException)
			{
			}
			_fakeStream = null;
		}
	}

	private static void CreateDirectory(string fileName)
	{
		string directoryName = Path.GetDirectoryName(fileName);
		if (!string.IsNullOrEmpty(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
	}

	private static string RemoveIllegalCharacters(string str, bool isDirectory = false)
	{
		List<string> list = new List<string>(str.Split(Path.DirectorySeparatorChar));
		char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
		foreach (char c in invalidFileNameChars)
		{
			for (int j = 0; j < list.Count; j++)
			{
				if ((!isDirectory || c != ':' || j != 0) && !string.IsNullOrEmpty(list[j]))
				{
					while (list[j].IndexOf(c) > -1)
					{
						list[j] = list[j].Replace(c, '_');
					}
				}
			}
		}
		if (str.StartsWith(new string(Path.DirectorySeparatorChar, 2), StringComparison.CurrentCultureIgnoreCase))
		{
			list.RemoveAt(0);
			list.RemoveAt(0);
			list[0] = new string(Path.DirectorySeparatorChar, 2) + list[0];
		}
		return string.Join(new string(Path.DirectorySeparatorChar, 1), list.ToArray());
	}
}
