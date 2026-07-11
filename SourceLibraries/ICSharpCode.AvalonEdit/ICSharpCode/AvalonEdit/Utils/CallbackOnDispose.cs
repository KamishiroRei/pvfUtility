using System;
using System.Threading;

namespace ICSharpCode.AvalonEdit.Utils;

internal sealed class CallbackOnDispose : IDisposable
{
	private Action action;

	public CallbackOnDispose(Action action)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		this.action = action;
	}

	public void Dispose()
	{
		Interlocked.Exchange(ref action, null)?.Invoke();
	}
}
