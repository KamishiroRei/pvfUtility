using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Rendering;

public interface IVisualLineTransformer
{
	void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements);
}
