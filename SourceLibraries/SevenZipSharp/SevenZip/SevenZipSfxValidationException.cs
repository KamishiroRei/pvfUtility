using System;
using System.Runtime.Serialization;

namespace SevenZip;

[Serializable]
public class SevenZipSfxValidationException : SevenZipException
{
	public static readonly string DefaultMessage = "Sfx settings validation failed.";

	public SevenZipSfxValidationException()
		: base(DefaultMessage)
	{
	}

	public SevenZipSfxValidationException(string message)
		: base(DefaultMessage, message)
	{
	}

	public SevenZipSfxValidationException(string message, Exception inner)
		: base(DefaultMessage, message, inner)
	{
	}

	protected SevenZipSfxValidationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
