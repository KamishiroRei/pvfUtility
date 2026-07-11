using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SevenZip;

internal class StreamWrapper : DisposeVariableWrapper, IDisposable
{
	private readonly string _fileName;

	private readonly DateTime _fileTime;

	private Stream _baseStream;

	protected Stream BaseStream => _baseStream;

	protected StreamWrapper(Stream baseStream, string fileName, DateTime time, bool disposeStream)
		: base(disposeStream)
	{
		_baseStream = baseStream;
		_fileName = fileName;
		_fileTime = time;
	}

	protected StreamWrapper(Stream baseStream, bool disposeStream)
		: base(disposeStream)
	{
		_baseStream = baseStream;
	}

	public void Dispose()
	{
		if (_baseStream != null && base.DisposeStream)
		{
			try
			{
				_baseStream.Dispose();
			}
			catch (ObjectDisposedException)
			{
			}
			_baseStream = null;
		}
		if (!string.IsNullOrEmpty(_fileName) && File.Exists(_fileName))
		{
			try
			{
				File.SetLastWriteTime(_fileName, _fileTime);
				File.SetLastAccessTime(_fileName, _fileTime);
				File.SetCreationTime(_fileName, _fileTime);
			}
			catch (ArgumentOutOfRangeException)
			{
			}
		}
		GC.SuppressFinalize(this);
	}

	public virtual void Seek(long offset, SeekOrigin seekOrigin, IntPtr newPosition)
	{
		if (BaseStream != null)
		{
			long val = BaseStream.Seek(offset, seekOrigin);
			if (newPosition != IntPtr.Zero)
			{
				Marshal.WriteInt64(newPosition, val);
			}
		}
	}
}
