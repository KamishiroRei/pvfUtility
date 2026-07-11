namespace ICSharpCode.AvalonEdit.Editing;

public interface ITextAreaInputHandler
{
	TextArea TextArea { get; }

	void Attach();

	void Detach();
}
