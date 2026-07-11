namespace SevenZip;

public sealed class ProgressEventArgs : PercentDoneEventArgs
{
	private readonly byte _delta;

	public byte PercentDelta => _delta;

	public ProgressEventArgs(byte percentDone, byte percentDelta)
		: base(percentDone)
	{
		_delta = percentDelta;
	}
}
