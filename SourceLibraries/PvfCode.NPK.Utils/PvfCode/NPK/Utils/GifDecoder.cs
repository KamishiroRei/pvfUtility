using System;
using System.Collections;
using System.Drawing;
using System.IO;
using PvfCode.NPK.Utils.Lib;

namespace PvfCode.NPK.Utils;

public class GifDecoder
{
	public class GifFrame
	{
		public int delay;

		public Image image;

		public GifFrame(Image im, int del)
		{
			image = im;
			delay = del;
		}
	}

	public static readonly int STATUS_OK = 0;

	public static readonly int STATUS_FORMAT_ERROR = 1;

	public static readonly int STATUS_OPEN_ERROR = 2;

	protected static readonly int MaxStackSize = 4096;

	protected int[] act;

	protected int bgColor;

	protected int bgIndex;

	protected Bitmap bitmap;

	protected byte[] block = new byte[256];

	protected int blockSize;

	protected int delay;

	protected int dispose;

	protected int frameCount;

	protected ArrayList frames;

	protected int[] gct;

	protected bool gctFlag;

	protected int gctSize;

	protected int height;

	protected Image image;

	protected Stream inStream;

	protected bool interlace;

	protected int ix;

	protected int iy;

	protected int iw;

	protected int ih;

	protected int lastBgColor;

	protected int lastDispose;

	protected Image lastImage;

	protected Rectangle lastRect;

	protected int[] lct;

	protected bool lctFlag;

	protected int lctSize;

	protected int loopCount = 1;

	protected int pixelAspect;

	protected byte[] pixels;

	protected byte[] pixelStack;

	protected short[] prefix;

	protected int status;

	protected byte[] suffix;

	protected int transIndex;

	protected bool transparency;

	protected int width;

	public int GetDelay(int n)
	{
		delay = -1;
		if (n >= 0 && n < frameCount)
		{
			delay = ((GifFrame)frames[n]).delay;
		}
		return delay;
	}

	public int GetFrameCount()
	{
		return frameCount;
	}

	public Image GetImage()
	{
		return GetFrame(0);
	}

	public int GetLoopCount()
	{
		return loopCount;
	}

	private int[] GetPixels(Bitmap bitmap)
	{
		byte[] array = bitmap.ToArray();
		int num = image.Width * image.Height;
		int[] array2 = new int[4 * num];
		for (int i = 0; i < num; i++)
		{
			array2[i * 3] = array[i * 4];
			array2[i * 3 + 1] = array[i * 4 + 1];
			array2[i * 3 + 2] = array[i * 4 + 2];
		}
		return array2;
	}

	private void SetPixels(int[] pixels)
	{
		int num = image.Width * image.Height;
		byte[] array = new byte[num * 4];
		for (int i = 0; i < num; i++)
		{
			int num2 = pixels[i];
			array[i * 4] = (byte)(num2 & 0xFF);
			array[i * 4 + 1] = (byte)((num2 >> 8) & 0xFF);
			array[i * 4 + 2] = (byte)((num2 >> 16) & 0xFF);
			array[i * 4 + 3] = (byte)(num2 >> 24);
		}
		bitmap = array.FromArray(image.Size);
	}

	protected void SetPixels()
	{
		int[] array = GetPixels(bitmap);
		if (lastDispose > 0)
		{
			if (lastDispose == 3)
			{
				int num = frameCount - 2;
				if (num > 0)
				{
					lastImage = GetFrame(num - 1);
				}
				else
				{
					lastImage = null;
				}
			}
			if (lastImage != null)
			{
				Array.Copy(GetPixels(new Bitmap(lastImage)), 0, array, 0, width * height);
				if (lastDispose == 2)
				{
					Graphics graphics = Graphics.FromImage(image);
					_ = Color.Empty;
					Color color = ((!transparency) ? Color.FromArgb(lastBgColor) : Color.FromArgb(0, 0, 0, 0));
					SolidBrush solidBrush = new SolidBrush(color);
					graphics.FillRectangle(solidBrush, lastRect);
					solidBrush.Dispose();
					graphics.Dispose();
				}
			}
		}
		int num2 = 1;
		int num3 = 8;
		int num4 = 0;
		for (int i = 0; i < ih; i++)
		{
			int num5 = i;
			if (interlace)
			{
				if (num4 >= ih)
				{
					num2++;
					switch (num2)
					{
					case 2:
						num4 = 4;
						break;
					case 3:
						num4 = 2;
						num3 = 4;
						break;
					case 4:
						num4 = 1;
						num3 = 2;
						break;
					}
				}
				num5 = num4;
				num4 += num3;
			}
			num5 += iy;
			if (num5 >= height)
			{
				continue;
			}
			int num6 = num5 * width;
			int j = num6 + ix;
			int num7 = j + iw;
			if (num6 + width < num7)
			{
				num7 = num6 + width;
			}
			int num8 = i * iw;
			for (; j < num7; j++)
			{
				int num9 = pixels[num8++] & 0xFF;
				int num10 = act[num9];
				if (num10 != 0)
				{
					array[j] = num10;
				}
			}
		}
		SetPixels(array);
	}

	public Image GetFrame(int n)
	{
		Image result = null;
		if (n >= 0 && n < frameCount)
		{
			result = ((GifFrame)frames[n]).image;
		}
		return result;
	}

	public Size GetFrameSize()
	{
		return new Size(width, height);
	}

	public int Read(Stream inStream)
	{
		Init();
		if (inStream != null)
		{
			this.inStream = inStream;
			ReadHeader();
			if (!Error())
			{
				ReadContents();
				if (frameCount < 0)
				{
					status = STATUS_FORMAT_ERROR;
				}
			}
			inStream.Close();
		}
		else
		{
			status = STATUS_OPEN_ERROR;
		}
		return status;
	}

	public int Read(string name)
	{
		status = STATUS_OK;
		try
		{
			name = name.Trim().ToLower();
			status = Read(new FileInfo(name).OpenRead());
		}
		catch (IOException)
		{
			status = STATUS_OPEN_ERROR;
		}
		return status;
	}

	protected void DecodeImageData()
	{
		int num = -1;
		int num2 = iw * ih;
		if (pixels == null || pixels.Length < num2)
		{
			pixels = new byte[num2];
		}
		if (prefix == null)
		{
			prefix = new short[MaxStackSize];
		}
		if (suffix == null)
		{
			suffix = new byte[MaxStackSize];
		}
		if (pixelStack == null)
		{
			pixelStack = new byte[MaxStackSize + 1];
		}
		int num3 = Read();
		int num4 = 1 << num3;
		int num5 = num4 + 1;
		int num6 = num4 + 2;
		int num7 = num;
		int num8 = num3 + 1;
		int num9 = (1 << num8) - 1;
		for (int i = 0; i < num4; i++)
		{
			prefix[i] = 0;
			suffix[i] = (byte)i;
		}
		int num11;
		int num12;
		int num13;
		int num14;
		int num15;
		int num16;
		int num10 = (num11 = (num12 = (num13 = (num14 = (num15 = (num16 = 0))))));
		int num17 = 0;
		while (num17 < num2)
		{
			if (num14 == 0)
			{
				if (num11 < num8)
				{
					if (num12 == 0)
					{
						num12 = ReadBlock();
						if (num12 <= 0)
						{
							break;
						}
						num16 = 0;
					}
					num10 += (block[num16] & 0xFF) << num11;
					num11 += 8;
					num16++;
					num12--;
					continue;
				}
				int i = num10 & num9;
				num10 >>= num8;
				num11 -= num8;
				if (i > num6 || i == num5)
				{
					break;
				}
				if (i == num4)
				{
					num8 = num3 + 1;
					num9 = (1 << num8) - 1;
					num6 = num4 + 2;
					num7 = num;
					continue;
				}
				if (num7 == num)
				{
					pixelStack[num14++] = suffix[i];
					num7 = i;
					num13 = i;
					continue;
				}
				int num18 = i;
				if (i == num6)
				{
					pixelStack[num14++] = (byte)num13;
					i = num7;
				}
				while (i > num4)
				{
					pixelStack[num14++] = suffix[i];
					i = prefix[i];
				}
				num13 = suffix[i] & 0xFF;
				if (num6 >= MaxStackSize)
				{
					break;
				}
				pixelStack[num14++] = (byte)num13;
				prefix[num6] = (short)num7;
				suffix[num6] = (byte)num13;
				num6++;
				if ((num6 & num9) == 0 && num6 < MaxStackSize)
				{
					num8++;
					num9 += num6;
				}
				num7 = num18;
			}
			num14--;
			pixels[num15++] = pixelStack[num14];
			num17++;
		}
		for (num17 = num15; num17 < num2; num17++)
		{
			pixels[num17] = 0;
		}
	}

	protected bool Error()
	{
		return status != STATUS_OK;
	}

	protected void Init()
	{
		status = STATUS_OK;
		frameCount = 0;
		frames = new ArrayList();
		gct = null;
		lct = null;
	}

	protected int Read()
	{
		int result = 0;
		try
		{
			result = inStream.ReadByte();
		}
		catch (IOException)
		{
			status = STATUS_FORMAT_ERROR;
		}
		return result;
	}

	protected int ReadBlock()
	{
		blockSize = Read();
		int i = 0;
		if (blockSize > 0)
		{
			try
			{
				int num;
				for (; i < blockSize; i += num)
				{
					num = inStream.Read(block, i, blockSize - i);
					if (num == -1)
					{
						break;
					}
				}
			}
			catch (IOException)
			{
			}
			if (i < blockSize)
			{
				status = STATUS_FORMAT_ERROR;
			}
		}
		return i;
	}

	protected int[] ReadColorTable(int ncolors)
	{
		int num = 3 * ncolors;
		int[] array = null;
		byte[] array2 = new byte[num];
		int num2 = 0;
		try
		{
			num2 = inStream.Read(array2, 0, array2.Length);
		}
		catch (IOException)
		{
		}
		if (num2 < num)
		{
			status = STATUS_FORMAT_ERROR;
		}
		else
		{
			array = new int[256];
			int num3 = 0;
			int num4 = 0;
			while (num3 < ncolors)
			{
				int num5 = array2[num4++] & 0xFF;
				int num6 = array2[num4++] & 0xFF;
				int num7 = array2[num4++] & 0xFF;
				array[num3++] = -16777216 | (num5 << 16) | (num6 << 8) | num7;
			}
		}
		return array;
	}

	protected void ReadContents()
	{
		bool flag = false;
		while (!flag && !Error())
		{
			switch (Read())
			{
			case 44:
				ReadImage();
				break;
			case 33:
				switch (Read())
				{
				case 249:
					ReadGraphicControlExt();
					break;
				case 255:
				{
					ReadBlock();
					string text = "";
					for (int i = 0; i < 11; i++)
					{
						string text2 = text;
						char c = (char)block[i];
						text = text2 + c;
					}
					if (text.Equals("NETSCAPE2.0"))
					{
						ReadNetscapeExt();
					}
					else
					{
						Skip();
					}
					break;
				}
				default:
					Skip();
					break;
				}
				break;
			case 59:
				flag = true;
				break;
			default:
				status = STATUS_FORMAT_ERROR;
				break;
			case 0:
				break;
			}
		}
	}

	protected void ReadGraphicControlExt()
	{
		Read();
		int num = Read();
		dispose = (num & 0x1C) >> 2;
		if (dispose == 0)
		{
			dispose = 1;
		}
		transparency = (num & 1) != 0;
		delay = ReadShort() * 10;
		transIndex = Read();
		Read();
	}

	protected void ReadHeader()
	{
		string text = "";
		for (int i = 0; i < 6; i++)
		{
			text += (char)Read();
		}
		if (!text.StartsWith("GIF"))
		{
			status = STATUS_FORMAT_ERROR;
			return;
		}
		ReadLSD();
		if (gctFlag && !Error())
		{
			gct = ReadColorTable(gctSize);
			bgColor = gct[bgIndex];
		}
	}

	protected void ReadImage()
	{
		ix = ReadShort();
		iy = ReadShort();
		iw = ReadShort();
		ih = ReadShort();
		int num = Read();
		lctFlag = (num & 0x80) != 0;
		interlace = (num & 0x40) != 0;
		lctSize = 2 << (num & 7);
		if (lctFlag)
		{
			lct = ReadColorTable(lctSize);
			act = lct;
		}
		else
		{
			act = gct;
			if (bgIndex == transIndex)
			{
				bgColor = 0;
			}
		}
		int num2 = 0;
		if (transparency)
		{
			num2 = act[transIndex];
			act[transIndex] = 0;
		}
		if (act == null)
		{
			status = STATUS_FORMAT_ERROR;
		}
		if (Error())
		{
			return;
		}
		DecodeImageData();
		Skip();
		if (!Error())
		{
			frameCount++;
			bitmap = new Bitmap(width, height);
			image = bitmap;
			SetPixels();
			frames.Add(new GifFrame(bitmap, delay));
			if (transparency)
			{
				act[transIndex] = num2;
			}
			ResetFrame();
		}
	}

	protected void ReadLSD()
	{
		width = ReadShort();
		height = ReadShort();
		int num = Read();
		gctFlag = (num & 0x80) != 0;
		gctSize = 2 << (num & 7);
		bgIndex = Read();
		pixelAspect = Read();
	}

	protected void ReadNetscapeExt()
	{
		do
		{
			ReadBlock();
			if (block[0] == 1)
			{
				int num = block[1] & 0xFF;
				int num2 = block[2] & 0xFF;
				loopCount = (num2 << 8) | num;
			}
		}
		while (blockSize > 0 && !Error());
	}

	protected int ReadShort()
	{
		return Read() | (Read() << 8);
	}

	protected void ResetFrame()
	{
		lastDispose = dispose;
		lastRect = new Rectangle(ix, iy, iw, ih);
		lastImage = image;
		lastBgColor = bgColor;
		lct = null;
	}

	protected void Skip()
	{
		do
		{
			ReadBlock();
		}
		while (blockSize > 0 && !Error());
	}
}
