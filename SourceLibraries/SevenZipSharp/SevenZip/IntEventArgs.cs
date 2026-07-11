using System;

namespace SevenZip;

public sealed class IntEventArgs : EventArgs
{
	private readonly int _value;

	public int Value => _value;

	public IntEventArgs(int value)
	{
		_value = value;
	}
}
