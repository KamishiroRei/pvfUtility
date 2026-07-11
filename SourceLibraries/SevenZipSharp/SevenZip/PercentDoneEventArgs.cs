using System;

namespace SevenZip;

public class PercentDoneEventArgs : EventArgs
{
	private readonly byte _percentDone;

	public byte PercentDone => _percentDone;

	public PercentDoneEventArgs(byte percentDone)
	{
		if (percentDone > 100 || percentDone < 0)
		{
			throw new ArgumentOutOfRangeException("percentDone", "The percent of finished work must be between 0 and 100.");
		}
		_percentDone = percentDone;
	}

	internal static byte ProducePercentDone(float doneRate)
	{
		return (byte)Math.Round(Math.Min(100f * doneRate, 100f), MidpointRounding.AwayFromZero);
	}
}
