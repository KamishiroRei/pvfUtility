using System;
using System.Runtime.Serialization;

namespace SevenZip;

[Serializable]
public class SevenZipExtractionFailedException : SevenZipException
{
	public const string DEFAULT_MESSAGE = "The extraction has failed for an unknown reason with code ";

	public SevenZipExtractionFailedException()
		: base("The extraction has failed for an unknown reason with code ")
	{
	}

	public SevenZipExtractionFailedException(string message)
		: base("The extraction has failed for an unknown reason with code ", message)
	{
	}

	public SevenZipExtractionFailedException(string message, Exception inner)
		: base("The extraction has failed for an unknown reason with code ", message, inner)
	{
	}

	protected SevenZipExtractionFailedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
