using System;
using System.IO;

namespace SevenZip;

internal class ArchiveEmulationStreamProxy : Stream, IDisposable
{
	public int Offset { get; }

	public Stream Source { get; }

	public override bool CanRead => Source.CanRead;

	public override bool CanSeek => Source.CanSeek;

	public override bool CanWrite => Source.CanWrite;

	public override long Length => Source.Length - Offset;

	public override long Position
	{
		get
		{
			return Source.Position - Offset;
		}
		set
		{
			Source.Position = value;
		}
	}

	public ArchiveEmulationStreamProxy(Stream stream, int offset)
	{
		Source = stream;
		Offset = offset;
		Source.Position = offset;
	}

	public override void Flush()
	{
		Source.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return Source.Read(buffer, offset, count);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return Source.Seek((origin == SeekOrigin.Begin) ? (offset + Offset) : offset, origin) - Offset;
	}

	public override void SetLength(long value)
	{
		Source.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		Source.Write(buffer, offset, count);
	}

	public new void Dispose()
	{
		Source.Dispose();
	}

	public override void Close()
	{
		Source.Close();
	}
}
