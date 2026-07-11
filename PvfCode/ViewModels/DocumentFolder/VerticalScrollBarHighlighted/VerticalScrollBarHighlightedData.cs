using System.Runtime.CompilerServices;

namespace PvfCode.ViewModels.DocumentFolder.VerticalScrollBarHighlighted;

public class VerticalScrollBarHighlightedData : ModelBase
{
	private double Puv5UCapLC;

	[CompilerGenerated]
	private int dno5cHsa4j;

	[CompilerGenerated]
	private VerticalScrollBarHighlightedType wsZ58NrJZl;

	public double Position
	{
		get
		{
			return Puv5UCapLC;
		}
		set
		{
			Puv5UCapLC = value;
			DoNotify("Position");
		}
	}

	public int Height
	{
		[CompilerGenerated]
		get
		{
			return dno5cHsa4j;
		}
		[CompilerGenerated]
		set
		{
			dno5cHsa4j = value;
		}
	}

	public VerticalScrollBarHighlightedType Type
	{
		[CompilerGenerated]
		get
		{
			return wsZ58NrJZl;
		}
		[CompilerGenerated]
		set
		{
			wsZ58NrJZl = value;
		}
	}

	public VerticalScrollBarHighlightedData(double position, int height, VerticalScrollBarHighlightedType type)
	{
		Position = position;
		Height = height;
		Type = type;
	}
}
