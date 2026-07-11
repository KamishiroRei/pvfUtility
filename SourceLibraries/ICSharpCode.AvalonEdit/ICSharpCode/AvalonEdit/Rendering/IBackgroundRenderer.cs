using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.Rendering;

public interface IBackgroundRenderer
{
	KnownLayer Layer { get; }

	void Draw(TextView textView, DrawingContext drawingContext);
}
