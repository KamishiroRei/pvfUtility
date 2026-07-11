using PvfCode.NPK.Utils.Models;

namespace PvfCode;

public class AniImage
{
	public string Img { get; set; }

	public int Index { get; set; }

	public string ImgFullPath
	{
		get
		{
			if (!string.IsNullOrEmpty(Img))
			{
				return "sprite/" + Img.ToLower().Replace("%04d", "0001").Replace("%02d%02", "0001")
					.Replace("%02d%02d", "0001");
			}
			return string.Empty;
		}
	}

	public ImgFile? ImgFile { get; set; }

	public AniImage(string img, ushort index)
	{
		Img = img;
		Index = index;
	}

	public override string ToString()
	{
		return $"{Index} {Img}";
	}

	public string GetStringData()
	{
		return $"\t[IMAGE]\r\n\t\t{Img}\r\n\t\t{Index}";
	}
}
