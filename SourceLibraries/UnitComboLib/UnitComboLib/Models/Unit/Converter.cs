namespace UnitComboLib.Models.Unit;

public abstract class Converter
{
	public abstract double Convert(Itemkey inputUnit, double inputValue, Itemkey outputUnit);
}
