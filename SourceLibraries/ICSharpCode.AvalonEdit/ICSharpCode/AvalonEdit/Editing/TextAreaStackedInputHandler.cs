using System;
using System.Windows.Input;

namespace ICSharpCode.AvalonEdit.Editing;

public abstract class TextAreaStackedInputHandler : ITextAreaInputHandler
{
	private readonly TextArea textArea;

	public TextArea TextArea => textArea;

	protected TextAreaStackedInputHandler(TextArea textArea)
	{
		if (textArea == null)
		{
			throw new ArgumentNullException("textArea");
		}
		this.textArea = textArea;
	}

	public virtual void Attach()
	{
	}

	public virtual void Detach()
	{
	}

	public virtual void OnPreviewKeyDown(KeyEventArgs e)
	{
	}

	public virtual void OnPreviewKeyUp(KeyEventArgs e)
	{
	}
}
