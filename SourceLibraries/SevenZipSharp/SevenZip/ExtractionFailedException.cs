using System;
using System.Runtime.Serialization;

namespace SevenZip;

[Serializable]
public class ExtractionFailedException : SevenZipException
{
	public const string DEFAULT_MESSAGE = "Could not extract files!";

	public ExtractionFailedException()
		: base("Could not extract files!")
	{
	}

	public ExtractionFailedException(string message)
		: base("Could not extract files!", message)
	{
	}

	public ExtractionFailedException(string message, Exception inner)
		: base("Could not extract files!", message, inner)
	{
	}

	protected ExtractionFailedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
