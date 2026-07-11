using System;
using System.IO;
using SevenZip.Sdk.Compression.Lzma;

namespace SevenZip;

public class LzmaEncodeStream : Stream
{
	private const int MAX_BUFFER_CAPACITY = 1073741824;

	private readonly MemoryStream _buffer = new MemoryStream();

	private readonly int _bufferCapacity = 262144;

	private readonly bool _ownOutput;

	private bool _disposed;

	private Encoder _lzmaEncoder;

	private Stream _output;

	public override bool CanRead => false;

	public override bool CanSeek => false;

	public override bool CanWrite
	{
		get
		{
			DisposedCheck();
			return _buffer.CanWrite;
		}
	}

	public override long Length
	{
		get
		{
			DisposedCheck();
			if (_output.CanSeek)
			{
				return _output.Length;
			}
			return _buffer.Position;
		}
	}

	public override long Position
	{
		get
		{
			DisposedCheck();
			if (_output.CanSeek)
			{
				return _output.Position;
			}
			return _buffer.Position;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public LzmaEncodeStream()
	{
		_output = new MemoryStream();
		_ownOutput = true;
		Init();
	}

	public LzmaEncodeStream(int bufferCapacity)
	{
		_output = new MemoryStream();
		_ownOutput = true;
		if (bufferCapacity > 1073741824)
		{
			throw new ArgumentException("Too large capacity.", "bufferCapacity");
		}
		_bufferCapacity = bufferCapacity;
		Init();
	}

	public LzmaEncodeStream(Stream outputStream)
	{
		if (!outputStream.CanWrite)
		{
			throw new ArgumentException("The specified stream can not write.", "outputStream");
		}
		_output = outputStream;
		Init();
	}

	public LzmaEncodeStream(Stream outputStream, int bufferCapacity)
	{
		if (!outputStream.CanWrite)
		{
			throw new ArgumentException("The specified stream can not write.", "outputStream");
		}
		_output = outputStream;
		if (bufferCapacity > 1073741824)
		{
			throw new ArgumentException("Too large capacity.", "bufferCapacity");
		}
		_bufferCapacity = bufferCapacity;
		Init();
	}

	private void Init()
	{
		_buffer.Capacity = _bufferCapacity;
		SevenZipCompressor.LzmaDictionarySize = _bufferCapacity;
		_lzmaEncoder = new Encoder();
		SevenZipCompressor.WriteLzmaProperties(_lzmaEncoder);
	}

	private void DisposedCheck()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("SevenZipExtractor");
		}
	}

	private void WriteChunk()
	{
		_lzmaEncoder.WriteCoderProperties(_output);
		long position = _buffer.Position;
		if (_buffer.Length != _buffer.Position)
		{
			_buffer.SetLength(_buffer.Position);
		}
		_buffer.Position = 0L;
		for (int i = 0; i < 8; i++)
		{
			_output.WriteByte((byte)(position >> 8 * i));
		}
		_lzmaEncoder.Code(_buffer, _output, -1L, -1L, null);
		_buffer.Position = 0L;
	}

	public LzmaDecodeStream ToDecodeStream()
	{
		DisposedCheck();
		Flush();
		return new LzmaDecodeStream(_output);
	}

	public override void Flush()
	{
		DisposedCheck();
		WriteChunk();
	}

	protected override void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}
		if (disposing)
		{
			Flush();
			_buffer.Close();
			if (_ownOutput)
			{
				_output.Dispose();
			}
			_output = null;
		}
		_disposed = true;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		DisposedCheck();
		throw new NotSupportedException();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		DisposedCheck();
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		DisposedCheck();
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		DisposedCheck();
		int num = Math.Min(buffer.Length - offset, count);
		while (_buffer.Position + num >= _bufferCapacity)
		{
			int num2 = _bufferCapacity - (int)_buffer.Position;
			_buffer.Write(buffer, offset, num2);
			offset = num2 + offset;
			num -= num2;
			WriteChunk();
		}
		_buffer.Write(buffer, offset, num);
	}
}
