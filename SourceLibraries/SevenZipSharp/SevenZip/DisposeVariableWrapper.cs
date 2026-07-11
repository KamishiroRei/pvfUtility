namespace SevenZip;

internal class DisposeVariableWrapper
{
	public bool DisposeStream { protected get; set; }

	protected DisposeVariableWrapper(bool disposeStream)
	{
		DisposeStream = disposeStream;
	}
}
