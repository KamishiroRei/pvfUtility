namespace PvfCode.NPK.Utils.AniModel;

public class ImageRate
{
	public float ScaleX { get; set; }

	public float ScaleY { get; set; }

	public ImageRate(float left, float right)
	{
		ScaleX = left;
		ScaleY = right;
	}

	public bool IsNull()
	{
		if (ScaleX == 0f)
		{
			return ScaleY == 0f;
		}
		return false;
	}

	public override string ToString()
	{
		return $"\t[IMAGE RATE]\r\n\t\t{ScaleX}\t{ScaleY}";
	}
}
