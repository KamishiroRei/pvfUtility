using System;

namespace UnitComboLib.Models.Unit.Screen;

public class ScreenPercent
{
	private double mValue;

	public ScreenPercent(double value)
	{
		mValue = value;
	}

	private ScreenPercent()
	{
	}

	public static double ToUnit(double inputValue, Itemkey targetUnit)
	{
		return new ScreenPercent(inputValue).ToUnit(targetUnit);
	}

	public double ToUnit(Itemkey targetUnit)
	{
		return targetUnit switch
		{
			Itemkey.ScreenPercent => mValue, 
			Itemkey.ScreenFontPoints => mValue * 12.0 / 100.0, 
			_ => throw new NotImplementedException(targetUnit.ToString()), 
		};
	}
}
