using System;
using System.Runtime.Serialization;

namespace ICSharpCode.AvalonEdit.Search;

public class SearchPatternException : Exception, ISerializable
{
	public SearchPatternException()
	{
	}

	public SearchPatternException(string message)
		: base(message)
	{
	}

	public SearchPatternException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected SearchPatternException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
