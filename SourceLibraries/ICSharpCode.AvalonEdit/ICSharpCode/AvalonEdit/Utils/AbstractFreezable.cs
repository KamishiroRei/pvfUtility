using System;

namespace ICSharpCode.AvalonEdit.Utils;

[Serializable]
internal abstract class AbstractFreezable : IFreezable
{
	private bool isFrozen;

	public bool IsFrozen => isFrozen;

	public void Freeze()
	{
		if (!isFrozen)
		{
			FreezeInternal();
			isFrozen = true;
		}
	}

	protected virtual void FreezeInternal()
	{
	}
}
