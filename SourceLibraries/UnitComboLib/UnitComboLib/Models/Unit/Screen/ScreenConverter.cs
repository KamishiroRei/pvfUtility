using System;

namespace UnitComboLib.Models.Unit.Screen;

public class ScreenConverter : Converter
{
	public const double OneHundretPercentFont = 12.0;

	public const double OneHundretPercent = 100.0;

	public override double Convert(Itemkey inputUnit, double inputValue, Itemkey outputUnit)
	{
		return inputUnit switch
		{
			Itemkey.ScreenFontPoints => ScreenFontPoints.ToUnit(inputValue, outputUnit), 
			Itemkey.ScreenPercent => ScreenPercent.ToUnit(inputValue, outputUnit), 
			_ => throw new NotImplementedException(outputUnit.ToString()), 
		};
	}
}
