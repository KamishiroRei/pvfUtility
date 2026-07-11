using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Rendering;

public static class TextViewWeakEventManager
{
	public sealed class DocumentChanged : WeakEventManagerBase<DocumentChanged, TextView>
	{
		protected override void StartListening(TextView source)
		{
			source.DocumentChanged += base.DeliverEvent;
		}

		protected override void StopListening(TextView source)
		{
			source.DocumentChanged -= base.DeliverEvent;
		}
	}

	public sealed class VisualLinesChanged : WeakEventManagerBase<VisualLinesChanged, TextView>
	{
		protected override void StartListening(TextView source)
		{
			source.VisualLinesChanged += base.DeliverEvent;
		}

		protected override void StopListening(TextView source)
		{
			source.VisualLinesChanged -= base.DeliverEvent;
		}
	}

	public sealed class ScrollOffsetChanged : WeakEventManagerBase<ScrollOffsetChanged, TextView>
	{
		protected override void StartListening(TextView source)
		{
			source.ScrollOffsetChanged += base.DeliverEvent;
		}

		protected override void StopListening(TextView source)
		{
			source.ScrollOffsetChanged -= base.DeliverEvent;
		}
	}
}
