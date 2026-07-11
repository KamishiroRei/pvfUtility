using System;

namespace ICSharpCode.AvalonEdit.Rendering;

public abstract class VisualLineElementGenerator
{
	internal int cachedInterest;

	protected ITextRunConstructionContext CurrentContext { get; private set; }

	public virtual void StartGeneration(ITextRunConstructionContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		CurrentContext = context;
	}

	public virtual void FinishGeneration()
	{
		CurrentContext = null;
	}

	public abstract int GetFirstInterestedOffset(int startOffset);

	public abstract VisualLineElement ConstructElement(int offset);
}
