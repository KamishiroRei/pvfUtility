using System;
using System.Runtime.Serialization;

namespace SevenZip;

[Serializable]
public class SevenZipException : Exception
{
	internal const string USER_EXCEPTION_MESSAGE = "The extraction was successful butsome exceptions were thrown in your events. Check UserExceptions for details.";

	public SevenZipException()
		: base("SevenZip unknown exception.")
	{
	}

	public SevenZipException(string defaultMessage)
		: base(defaultMessage)
	{
	}

	public SevenZipException(string defaultMessage, string message)
		: base(defaultMessage + " Message: " + message)
	{
	}

	public SevenZipException(string defaultMessage, string message, Exception inner)
		: base(defaultMessage + (defaultMessage.EndsWith(" ", StringComparison.CurrentCulture) ? "" : " Message: ") + message, inner)
	{
	}

	public SevenZipException(string defaultMessage, Exception inner)
		: base(defaultMessage, inner)
	{
	}

	protected SevenZipException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
