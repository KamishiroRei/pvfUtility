using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Editing;

public static class CaretWeakEventManager
{
	public sealed class PositionChanged : WeakEventManagerBase<PositionChanged, Caret>
	{
		protected override void StartListening(Caret source)
		{
			source.PositionChanged += base.DeliverEvent;
		}

		protected override void StopListening(Caret source)
		{
			source.PositionChanged -= base.DeliverEvent;
		}
	}
}
