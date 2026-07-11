namespace PvfCode.NPK.Utils.Coder;

public class DdsTexture
{
	public int Length { get; set; }

	public int Flags { get; set; }

	public int Width { get; set; } = 4;

	public int Height { get; set; } = 4;

	public int Count { get; set; } = 1;

	public DdsFormat Format { get; set; }

	public bool IsCubemap { get; internal set; }

	public DdsMipmap[] DdsMipmaps { get; internal set; }

	public int Pitch { get; internal set; }

	public int Depth { get; internal set; }

	public byte[] Reverse { get; set; } = new byte[11];

	public int PixelFormatSize { get; internal set; }
}
