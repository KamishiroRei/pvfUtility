using System;
using System.Runtime.Serialization;

namespace SevenZip;

[Serializable]
public class SevenZipInvalidFileNamesException : SevenZipException
{
	public const string DEFAULT_MESSAGE = "Invalid file names have been specified: ";

	public SevenZipInvalidFileNamesException()
		: base("Invalid file names have been specified: ")
	{
	}

	public SevenZipInvalidFileNamesException(string message)
		: base("Invalid file names have been specified: ", message)
	{
	}

	public SevenZipInvalidFileNamesException(string message, Exception inner)
		: base("Invalid file names have been specified: ", message, inner)
	{
	}

	protected SevenZipInvalidFileNamesException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
