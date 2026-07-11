namespace ICSharpCode.AvalonEdit.Utils;

internal interface IFreezable
{
	bool IsFrozen { get; }

	void Freeze();
}
