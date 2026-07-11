namespace HL.Xshtd.interfaces;

internal interface IFreezable
{
	bool IsFrozen { get; }

	void Freeze();
}
