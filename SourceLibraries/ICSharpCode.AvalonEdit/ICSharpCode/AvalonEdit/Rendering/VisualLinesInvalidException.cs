using System;
using System.Runtime.Serialization;

namespace ICSharpCode.AvalonEdit.Rendering;

[Serializable]
public class VisualLinesInvalidException : Exception
{
	public VisualLinesInvalidException()
	{
	}

	public VisualLinesInvalidException(string message)
		: base(message)
	{
	}

	public VisualLinesInvalidException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected VisualLinesInvalidException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
