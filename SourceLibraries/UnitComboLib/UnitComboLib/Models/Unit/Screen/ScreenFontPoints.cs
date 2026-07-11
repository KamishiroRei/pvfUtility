using System;

namespace UnitComboLib.Models.Unit.Screen;

public class ScreenFontPoints
{
	private double mValue;

	public ScreenFontPoints(double value)
	{
		mValue = value;
	}

	private ScreenFontPoints()
	{
	}

	public static double ToUnit(double inputValue, Itemkey targetUnit)
	{
		return new ScreenFontPoints(inputValue).ToUnit(targetUnit);
	}

	public double ToUnit(Itemkey targetUnit)
	{
		return targetUnit switch
		{
			Itemkey.ScreenPercent => mValue * 8.333333333333334, 
			Itemkey.ScreenFontPoints => mValue, 
			_ => throw new NotImplementedException(targetUnit.ToString()), 
		};
	}
}
