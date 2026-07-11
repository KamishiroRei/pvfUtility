using System;
using System.Runtime.Serialization;

namespace SevenZip;

[Serializable]
public class SevenZipLibraryException : SevenZipException
{
	public const string DEFAULT_MESSAGE = "Can not load 7-zip library or internal COM error!";

	public SevenZipLibraryException()
		: base("Can not load 7-zip library or internal COM error!")
	{
	}

	public SevenZipLibraryException(string message)
		: base("Can not load 7-zip library or internal COM error!", message)
	{
	}

	public SevenZipLibraryException(string message, Exception inner)
		: base("Can not load 7-zip library or internal COM error!", message, inner)
	{
	}

	protected SevenZipLibraryException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
