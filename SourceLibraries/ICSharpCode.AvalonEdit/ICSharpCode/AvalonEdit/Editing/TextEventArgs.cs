using System;

namespace ICSharpCode.AvalonEdit.Editing;

[Serializable]
public class TextEventArgs : EventArgs
{
	private string text;

	public string Text => text;

	public TextEventArgs(string text)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		this.text = text;
	}
}
