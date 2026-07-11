using System;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Rendering;

public abstract class ColorizingTransformer : IVisualLineTransformer, ITextViewConnect
{
	protected IList<VisualLineElement> CurrentElements { get; private set; }

	public void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements)
	{
		if (elements == null)
		{
			throw new ArgumentNullException("elements");
		}
		if (CurrentElements != null)
		{
			throw new InvalidOperationException("Recursive Transform() call");
		}
		CurrentElements = elements;
		try
		{
			Colorize(context);
		}
		finally
		{
			CurrentElements = null;
		}
	}

	protected abstract void Colorize(ITextRunConstructionContext context);

	protected void ChangeVisualElements(int visualStartColumn, int visualEndColumn, Action<VisualLineElement> action)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}
		for (int i = 0; i < CurrentElements.Count; i++)
		{
			VisualLineElement visualLineElement = CurrentElements[i];
			if (visualLineElement.VisualColumn > visualEndColumn)
			{
				break;
			}
			if (visualLineElement.VisualColumn < visualStartColumn && visualLineElement.VisualColumn + visualLineElement.VisualLength > visualStartColumn && visualLineElement.CanSplit)
			{
				visualLineElement.Split(visualStartColumn, CurrentElements, i--);
			}
			else
			{
				if (visualLineElement.VisualColumn < visualStartColumn || visualLineElement.VisualColumn >= visualEndColumn)
				{
					continue;
				}
				if (visualLineElement.VisualColumn + visualLineElement.VisualLength > visualEndColumn)
				{
					if (visualLineElement.CanSplit)
					{
						visualLineElement.Split(visualEndColumn, CurrentElements, i--);
					}
				}
				else
				{
					action(visualLineElement);
				}
			}
		}
	}

	protected virtual void OnAddToTextView(TextView textView)
	{
	}

	protected virtual void OnRemoveFromTextView(TextView textView)
	{
	}

	void ITextViewConnect.AddToTextView(TextView textView)
	{
		OnAddToTextView(textView);
	}

	void ITextViewConnect.RemoveFromTextView(TextView textView)
	{
		OnRemoveFromTextView(textView);
	}
}
