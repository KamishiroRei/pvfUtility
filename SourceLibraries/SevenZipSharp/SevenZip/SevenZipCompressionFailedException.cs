using System;
using System.Runtime.Serialization;

namespace SevenZip;

[Serializable]
public class SevenZipCompressionFailedException : SevenZipException
{
	public const string DEFAULT_MESSAGE = "The compression has failed for an unknown reason with code ";

	public SevenZipCompressionFailedException()
		: base("The compression has failed for an unknown reason with code ")
	{
	}

	public SevenZipCompressionFailedException(string message)
		: base("The compression has failed for an unknown reason with code ", message)
	{
	}

	public SevenZipCompressionFailedException(string message, Exception inner)
		: base("The compression has failed for an unknown reason with code ", message, inner)
	{
	}

	protected SevenZipCompressionFailedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
