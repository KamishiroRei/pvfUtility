using System;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Utils;

internal sealed class DelayedEvents
{
	private struct EventCall
	{
		private EventHandler handler;

		private object sender;

		private EventArgs e;

		public EventCall(EventHandler handler, object sender, EventArgs e)
		{
			this.handler = handler;
			this.sender = sender;
			this.e = e;
		}

		public void Call()
		{
			handler(sender, e);
		}
	}

	private Queue<EventCall> eventCalls = new Queue<EventCall>();

	public void DelayedRaise(EventHandler handler, object sender, EventArgs e)
	{
		if (handler != null)
		{
			eventCalls.Enqueue(new EventCall(handler, sender, e));
		}
	}

	public void RaiseEvents()
	{
		while (eventCalls.Count > 0)
		{
			eventCalls.Dequeue().Call();
		}
	}
}
