using System;
using System.Runtime.Serialization;

namespace SevenZip;

[Serializable]
public class CompressionFailedException : SevenZipException
{
	public const string DEFAULT_MESSAGE = "Could not pack files!";

	public CompressionFailedException()
		: base("Could not pack files!")
	{
	}

	public CompressionFailedException(string message)
		: base("Could not pack files!", message)
	{
	}

	public CompressionFailedException(string message, Exception inner)
		: base("Could not pack files!", message, inner)
	{
	}

	protected CompressionFailedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
