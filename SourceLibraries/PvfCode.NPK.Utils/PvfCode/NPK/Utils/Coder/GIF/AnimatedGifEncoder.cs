using System;
using System.Drawing;
using System.IO;
using PvfCode.NPK.Utils.Lib;

namespace PvfCode.NPK.Utils.Coder.GIF;

public class AnimatedGifEncoder
{
	protected bool closeStream;

	protected int colorDepth;

	protected byte[] colorTab;

	protected int delay;

	protected int dispose = -1;

	protected bool firstFrame = true;

	protected int height;

	protected Image? image;

	protected byte[] indexedPixels;

	protected MemoryStream ms;

	protected int palSize = 7;

	protected byte[] pixels;

	protected int repeat = -1;

	protected int sample = 10;

	protected bool sizeSet;

	protected bool started;

	protected int transIndex;

	protected Color transparent = Color.Empty;

	protected bool[] usedEntry = new bool[256];

	protected int width;

	public void SetDelay(int ms)
	{
		delay = (int)Math.Round((float)ms / 10f);
	}

	public void SetDispose(int code)
	{
		if (code >= 0)
		{
			dispose = code;
		}
	}

	public void SetRepeat(int iter)
	{
		if (iter >= 0)
		{
			repeat = iter;
		}
	}

	public void SetTransparent(Color c)
	{
		transparent = c;
	}

	public bool AddFrame(Image im)
	{
		if (im == null || !started)
		{
			return false;
		}
		bool result = true;
		try
		{
			if (!sizeSet)
			{
				SetSize(im.Width, im.Height);
			}
			image = im;
			GetImagePixels();
			AnalyzePixels();
			if (firstFrame)
			{
				WriteLSD();
				WritePalette();
				if (repeat >= 0)
				{
					WriteNetscapeExt();
				}
			}
			WriteGraphicCtrlExt();
			WriteImageDesc();
			if (!firstFrame)
			{
				WritePalette();
			}
			WritePixels();
			firstFrame = false;
		}
		catch (IOException)
		{
			result = false;
		}
		return result;
	}

	public bool Finish()
	{
		if (!started)
		{
			return false;
		}
		bool result = true;
		started = false;
		try
		{
			ms.WriteByte(59);
			ms.Flush();
			_ = closeStream;
		}
		catch (IOException)
		{
			result = false;
		}
		transIndex = 0;
		image = null;
		pixels = null;
		indexedPixels = null;
		colorTab = null;
		closeStream = false;
		firstFrame = true;
		return result;
	}

	public void SetFrameRate(float fps)
	{
		if (fps != 0f)
		{
			delay = (int)Math.Round(100f / fps);
		}
	}

	public void SetQuality(int quality)
	{
		if (quality < 1)
		{
			quality = 1;
		}
		sample = quality;
	}

	public void SetSize(int w, int h)
	{
		if (!started || firstFrame)
		{
			width = w;
			height = h;
			if (width < 1)
			{
				width = 320;
			}
			if (height < 1)
			{
				height = 240;
			}
			sizeSet = true;
		}
	}

	public bool Start(MemoryStream os)
	{
		if (os == null)
		{
			return false;
		}
		bool flag = true;
		closeStream = false;
		ms = os;
		try
		{
			WriteString("GIF89a");
		}
		catch (IOException)
		{
			flag = false;
		}
		return started = flag;
	}

	public bool Start()
	{
		bool flag;
		try
		{
			flag = Start(new MemoryStream(10240));
			closeStream = true;
		}
		catch (IOException)
		{
			flag = false;
		}
		return started = flag;
	}

	public bool Output(string file)
	{
		try
		{
			FileStream fileStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
			fileStream.Write(ms.ToArray(), 0, (int)ms.Length);
			fileStream.Close();
		}
		catch (IOException)
		{
			return false;
		}
		return true;
	}

	public MemoryStream Output()
	{
		return ms;
	}

	protected void AnalyzePixels()
	{
		int num = pixels.Length;
		int num2 = num / 3;
		indexedPixels = new byte[num2];
		NeuQuant neuQuant = new NeuQuant(pixels, num, sample);
		colorTab = neuQuant.Process();
		int num3 = 0;
		for (int i = 0; i < num2; i++)
		{
			int num4 = neuQuant.Map(pixels[num3++] & 0xFF, pixels[num3++] & 0xFF, pixels[num3++] & 0xFF);
			usedEntry[num4] = true;
			indexedPixels[i] = (byte)num4;
		}
		pixels = null;
		colorDepth = 8;
		palSize = 7;
		if (transparent != Color.Empty)
		{
			transIndex = neuQuant.Map(transparent.B, transparent.G, transparent.R);
		}
	}

	protected int FindClosest(Color c)
	{
		if (colorTab == null)
		{
			return -1;
		}
		int r = c.R;
		int g = c.G;
		int b = c.B;
		int result = 0;
		int num = 16777216;
		int num2 = colorTab.Length;
		for (int i = 0; i < num2; i++)
		{
			int num3 = r - (colorTab[i++] & 0xFF);
			int num4 = g - (colorTab[i++] & 0xFF);
			int num5 = b - (colorTab[i] & 0xFF);
			int num6 = num3 * num3 + num4 * num4 + num5 * num5;
			int num7 = i / 3;
			if (usedEntry[num7] && num6 < num)
			{
				num = num6;
				result = num7;
			}
		}
		return result;
	}

	protected void GetImagePixels()
	{
		int num = this.image.Width;
		int num2 = this.image.Height;
		if (num != width || num2 != height)
		{
			Image image = new Bitmap(width, height);
			Graphics graphics = Graphics.FromImage(image);
			graphics.DrawImage(this.image, 0, 0);
			this.image = image;
			graphics.Dispose();
		}
		pixels = new byte[3 * this.image.Width * this.image.Height];
		byte[] array = new Bitmap(this.image).ToArray();
		int num3 = this.image.Width * this.image.Height;
		for (int i = 0; i < num3; i++)
		{
			pixels[i * 3] = array[i * 4 + 2];
			pixels[i * 3 + 1] = array[i * 4 + 1];
			pixels[i * 3 + 2] = array[i * 4];
		}
	}

	protected void WriteGraphicCtrlExt()
	{
		ms.WriteByte(33);
		ms.WriteByte(249);
		ms.WriteByte(4);
		int num;
		int num2;
		if (transparent == Color.Empty)
		{
			num = 0;
			num2 = 0;
		}
		else
		{
			num = 1;
			num2 = 2;
		}
		if (dispose >= 0)
		{
			num2 = dispose & 7;
		}
		num2 <<= 2;
		ms.WriteByte(Convert.ToByte(0 | num2 | 0 | num));
		WriteShort(delay);
		ms.WriteByte(Convert.ToByte(transIndex));
		ms.WriteByte(0);
	}

	protected void WriteImageDesc()
	{
		ms.WriteByte(44);
		WriteShort(0);
		WriteShort(0);
		WriteShort(width);
		WriteShort(height);
		if (firstFrame)
		{
			ms.WriteByte(0);
		}
		else
		{
			ms.WriteByte(Convert.ToByte(0x80 | palSize));
		}
	}

	protected void WriteLSD()
	{
		WriteShort(width);
		WriteShort(height);
		ms.WriteByte(Convert.ToByte(0xF0 | palSize));
		ms.WriteByte(0);
		ms.WriteByte(0);
	}

	protected void WriteNetscapeExt()
	{
		ms.WriteByte(33);
		ms.WriteByte(byte.MaxValue);
		ms.WriteByte(11);
		WriteString("NETSCAPE2.0");
		ms.WriteByte(3);
		ms.WriteByte(1);
		WriteShort(repeat);
		ms.WriteByte(0);
	}

	protected void WritePalette()
	{
		ms.Write(colorTab, 0, colorTab.Length);
		int num = 768 - colorTab.Length;
		for (int i = 0; i < num; i++)
		{
			ms.WriteByte(0);
		}
	}

	protected void WritePixels()
	{
		new LZWEncoder(width, height, indexedPixels, colorDepth).Encode(ms);
	}

	protected void WriteShort(int value)
	{
		ms.WriteByte(Convert.ToByte(value & 0xFF));
		ms.WriteByte(Convert.ToByte((value >> 8) & 0xFF));
	}

	protected void WriteString(string s)
	{
		char[] array = s.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			ms.WriteByte((byte)array[i]);
		}
	}
}
