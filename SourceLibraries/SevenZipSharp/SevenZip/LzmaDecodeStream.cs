using System;
using System.IO;
using SevenZip.Sdk.Compression.Lzma;

namespace SevenZip;

public class LzmaDecodeStream : Stream
{
	private readonly MemoryStream _buffer = new MemoryStream();

	private readonly Decoder _decoder = new Decoder();

	private readonly Stream _input;

	private byte[] _commonProperties;

	private bool _error;

	private bool _firstChunkRead;

	public int ChunkSize => (int)_buffer.Length;

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => false;

	public override long Length
	{
		get
		{
			if (_input.CanSeek)
			{
				return _input.Length;
			}
			return _buffer.Length;
		}
	}

	public override long Position
	{
		get
		{
			if (_input.CanSeek)
			{
				return _input.Position;
			}
			return _buffer.Position;
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public LzmaDecodeStream(Stream encodedStream)
	{
		if (!encodedStream.CanRead)
		{
			throw new ArgumentException("The specified stream can not read.", "encodedStream");
		}
		_input = encodedStream;
	}

	private void ReadChunk()
	{
		byte[] lzmaProperties;
		long outSize;
		try
		{
			lzmaProperties = SevenZipExtractor.GetLzmaProperties(_input, out outSize);
		}
		catch (LzmaException)
		{
			_error = true;
			return;
		}
		if (!_firstChunkRead)
		{
			_commonProperties = lzmaProperties;
		}
		if (_commonProperties[0] != lzmaProperties[0] || _commonProperties[1] != lzmaProperties[1] || _commonProperties[2] != lzmaProperties[2] || _commonProperties[3] != lzmaProperties[3] || _commonProperties[4] != lzmaProperties[4])
		{
			_error = true;
			return;
		}
		if (_buffer.Capacity < (int)outSize)
		{
			_buffer.Capacity = (int)outSize;
		}
		_buffer.SetLength(outSize);
		_decoder.SetDecoderProperties(lzmaProperties);
		_buffer.Position = 0L;
		_decoder.Code(_input, _buffer, 0L, outSize, null);
		_buffer.Position = 0L;
	}

	public override void Flush()
	{
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (_error)
		{
			return 0;
		}
		if (!_firstChunkRead)
		{
			ReadChunk();
			_firstChunkRead = true;
		}
		int num = 0;
		while (count > _buffer.Length - _buffer.Position && !_error)
		{
			byte[] array = new byte[_buffer.Length - _buffer.Position];
			_buffer.Read(array, 0, array.Length);
			array.CopyTo(buffer, offset);
			offset += array.Length;
			count -= array.Length;
			num += array.Length;
			ReadChunk();
		}
		if (!_error)
		{
			_buffer.Read(buffer, offset, count);
			num += count;
		}
		return num;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}
}
