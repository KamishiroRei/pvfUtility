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
				// %04d 是时装/皮肤自适配模板（运行时由客户端按当前穿着填充）；
				// GUI 预览无法得知玩家穿着，按约定默认展开为 0000 = 角色默认皮肤。
				return "sprite/" + Img.ToLower().Replace("%04d", "0000").Replace("%02d%02", "0001")
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
