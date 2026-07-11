using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit;

public static class TextEditorWeakEventManager
{
	public sealed class DocumentChanged : WeakEventManagerBase<DocumentChanged, ITextEditorComponent>
	{
		protected override void StartListening(ITextEditorComponent source)
		{
			source.DocumentChanged += base.DeliverEvent;
		}

		protected override void StopListening(ITextEditorComponent source)
		{
			source.DocumentChanged -= base.DeliverEvent;
		}
	}

	public sealed class OptionChanged : WeakEventManagerBase<OptionChanged, ITextEditorComponent>
	{
		protected override void StartListening(ITextEditorComponent source)
		{
			source.OptionChanged += base.DeliverEvent;
		}

		protected override void StopListening(ITextEditorComponent source)
		{
			source.OptionChanged -= base.DeliverEvent;
		}
	}
}
