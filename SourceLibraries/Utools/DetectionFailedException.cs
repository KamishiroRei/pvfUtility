using System;

public class DetectionFailedException : Exception
{
	public DetectionFailedException()
	{
	}

	public DetectionFailedException(string message)
		: base(message)
	{
	}

	public DetectionFailedException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
