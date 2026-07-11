using System;
using System.Runtime.Serialization;

namespace SevenZip;

[Serializable]
public class SevenZipArchiveException : SevenZipException
{
	public static string DefaultMessage = "Invalid archive: open/read error! Is it encrypted and a wrong password was provided?" + Environment.NewLine + "If your archive is an exotic one, it is possible that SevenZipSharp has no signature for its format and thus decided it is TAR by mistake.";

	public SevenZipArchiveException()
		: base(DefaultMessage)
	{
	}

	public SevenZipArchiveException(string message)
		: base(DefaultMessage, message)
	{
	}

	public SevenZipArchiveException(string message, Exception inner)
		: base(DefaultMessage, message, inner)
	{
	}

	protected SevenZipArchiveException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
