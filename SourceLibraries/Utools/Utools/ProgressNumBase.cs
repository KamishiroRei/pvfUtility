
namespace Utools;

public class ProgressNumBase
{
	public static int GetProgressNum(int nValue, int nMaxValue)
	{
		return nValue * 100 / nMaxValue;
	}

	public ProgressNumBase()
	{
	}
}
