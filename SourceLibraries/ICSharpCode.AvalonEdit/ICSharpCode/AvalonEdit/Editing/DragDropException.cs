using System;
using System.Runtime.Serialization;

namespace ICSharpCode.AvalonEdit.Editing;

[Serializable]
public class DragDropException : Exception
{
	public DragDropException()
	{
	}

	public DragDropException(string message)
		: base(message)
	{
	}

	public DragDropException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected DragDropException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
