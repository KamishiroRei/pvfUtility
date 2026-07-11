using System;
using SevenZip.Sdk;

namespace SevenZip;

internal sealed class LzmaProgressCallback : ICodeProgress
{
	private readonly long _inSize;

	private float _oldPercentDone;

	public event EventHandler<ProgressEventArgs> Working;

	public LzmaProgressCallback(long inSize, EventHandler<ProgressEventArgs> working)
	{
		_inSize = inSize;
		Working += working;
	}

	public void SetProgress(long inSize, long outSize)
	{
		if (this.Working != null)
		{
			float num = ((float)inSize + 0f) / (float)_inSize;
			float num2 = num - _oldPercentDone;
			if ((double)(num2 * 100f) < 1.0)
			{
				num2 = 0f;
			}
			else
			{
				_oldPercentDone = num;
			}
			this.Working(this, new ProgressEventArgs(PercentDoneEventArgs.ProducePercentDone(num), (byte)((num2 > 0f) ? PercentDoneEventArgs.ProducePercentDone(num2) : 0)));
		}
	}
}
