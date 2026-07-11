using System;

namespace HL.Xshtd.interfaces;

[Serializable]
public abstract class AbstractFreezable : IFreezable
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
