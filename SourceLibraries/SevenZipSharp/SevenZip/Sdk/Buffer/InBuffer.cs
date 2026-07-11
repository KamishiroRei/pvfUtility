using System.IO;

namespace SevenZip.Sdk.Buffer;

internal class InBuffer
{
	private readonly byte[] m_Buffer;

	private readonly uint m_BufferSize;

	private uint m_Limit;

	private uint m_Pos;

	private ulong m_ProcessedSize;

	private Stream m_Stream;

	private bool m_StreamWasExhausted;

	private InBuffer(uint bufferSize)
	{
		m_Buffer = new byte[bufferSize];
		m_BufferSize = bufferSize;
	}

	private void Init(Stream stream)
	{
		m_Stream = stream;
		m_ProcessedSize = 0uL;
		m_Limit = 0u;
		m_Pos = 0u;
		m_StreamWasExhausted = false;
	}

	private bool ReadBlock()
	{
		if (m_StreamWasExhausted)
		{
			return false;
		}
		m_ProcessedSize += m_Pos;
		int num = m_Stream.Read(m_Buffer, 0, (int)m_BufferSize);
		m_Pos = 0u;
		m_Limit = (uint)num;
		m_StreamWasExhausted = num == 0;
		return !m_StreamWasExhausted;
	}

	private void ReleaseStream()
	{
		m_Stream = null;
	}

	private bool ReadByte(out byte b)
	{
		b = 0;
		if (m_Pos >= m_Limit && !ReadBlock())
		{
			return false;
		}
		b = m_Buffer[m_Pos++];
		return true;
	}

	private byte ReadByte()
	{
		if (m_Pos >= m_Limit && !ReadBlock())
		{
			return byte.MaxValue;
		}
		return m_Buffer[m_Pos++];
	}

	private ulong GetProcessedSize()
	{
		return m_ProcessedSize + m_Pos;
	}
}
