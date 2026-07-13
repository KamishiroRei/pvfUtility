namespace PvfCode.ViewModels.DocumentFolder.VerticalScrollBarHighlighted;

public class VerticalScrollBarHighlightedData : ModelBase
{
	private double position;

	public double Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
			DoNotify("Position");
		}
	}

	public int Height { get; set; }

	public VerticalScrollBarHighlightedType Type { get; set; }

	public VerticalScrollBarHighlightedData(double position, int height, VerticalScrollBarHighlightedType type)
	{
		Position = position;
		Height = height;
		Type = type;
	}
}
