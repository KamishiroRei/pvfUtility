using System;
using System.Runtime.Serialization;

namespace SevenZip;

[Serializable]
public class LzmaException : SevenZipException
{
	public const string DEFAULT_MESSAGE = "Specified stream is not a valid LZMA compressed stream!";

	public LzmaException()
		: base("Specified stream is not a valid LZMA compressed stream!")
	{
	}

	public LzmaException(string message)
		: base("Specified stream is not a valid LZMA compressed stream!", message)
	{
	}

	public LzmaException(string message, Exception inner)
		: base("Specified stream is not a valid LZMA compressed stream!", message, inner)
	{
	}

	protected LzmaException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
