using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document;

public static class TextDocumentWeakEventManager
{
	public sealed class UpdateStarted : WeakEventManagerBase<UpdateStarted, TextDocument>
	{
		protected override void StartListening(TextDocument source)
		{
			source.UpdateStarted += base.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.UpdateStarted -= base.DeliverEvent;
		}
	}

	public sealed class UpdateFinished : WeakEventManagerBase<UpdateFinished, TextDocument>
	{
		protected override void StartListening(TextDocument source)
		{
			source.UpdateFinished += base.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.UpdateFinished -= base.DeliverEvent;
		}
	}

	public sealed class Changing : WeakEventManagerBase<Changing, TextDocument>
	{
		protected override void StartListening(TextDocument source)
		{
			source.Changing += base.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.Changing -= base.DeliverEvent;
		}
	}

	public sealed class Changed : WeakEventManagerBase<Changed, TextDocument>
	{
		protected override void StartListening(TextDocument source)
		{
			source.Changed += base.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.Changed -= base.DeliverEvent;
		}
	}

	public sealed class TextChanged : WeakEventManagerBase<TextChanged, TextDocument>
	{
		protected override void StartListening(TextDocument source)
		{
			source.TextChanged += base.DeliverEvent;
		}

		protected override void StopListening(TextDocument source)
		{
			source.TextChanged -= base.DeliverEvent;
		}
	}
}
