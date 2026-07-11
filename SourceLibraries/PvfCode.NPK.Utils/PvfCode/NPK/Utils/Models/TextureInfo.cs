using System.Drawing;

namespace PvfCode.NPK.Utils.Models;

public class TextureInfo
{
	public Texture Texture { get; set; }

	public Point LeftUp { get; set; }

	public Point RightDown { get; set; }

	public Size Size => new Size(RightDown.X - LeftUp.X, RightDown.Y - LeftUp.Y);

	public int Top { get; set; }

	public Rectangle Rectangle => new Rectangle(LeftUp, Size);

	public int Unknown { get; set; }
}
