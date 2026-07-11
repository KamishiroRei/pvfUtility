using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PvfCode.Services.Npk;

internal class NpkImageArchiveReader
{
	internal struct NpkHeader
	{
		internal string Signature;

		internal uint EntryCount;
	}

	internal struct NpkEntry
	{
		internal int ImageOffset;

		internal uint ImageSize;

		internal string FilePath;

		internal string FileName;

		internal byte[] ArchiveData;

		internal ImgHeader ReadImageHeader()
		{
			int num = ImageOffset;
			ImgHeader result = new ImgHeader
			{
				Signature = Encoding.UTF8.GetString(ArchiveData, num, 16)
			};
			num += 16;
			result.FrameDataLength = BitConverter.ToUInt32(ArchiveData, num);
			num += 4;
			result.Version = BitConverter.ToUInt32(ArchiveData, num);
			num += 4;
			result.UnknownHeaderValue = BitConverter.ToUInt32(ArchiveData, num);
			num += 4;
			result.FrameCount = BitConverter.ToUInt32(ArchiveData, num);
			num += 4;
			result.FrameTableOffset = num;
			if (result.Signature.IndexOf("Neople Img File", StringComparison.Ordinal) != 0)
			{
				throw new Exception("error flag " + result.Signature + " in file " + FilePath);
			}
			result.RawData = ArchiveData;
			return result;
		}
	}

	internal struct ImgHeader
	{
		internal string Signature;

		internal uint FrameDataLength;

		internal uint Version;

		internal uint UnknownHeaderValue;

		internal uint FrameCount;

		internal byte[] RawData;

		internal int FrameTableOffset;

		internal ImgFrame[] ReadFrames()
		{
			int num = FrameTableOffset;
			List<ImgFrame> list = new List<ImgFrame>();
			int num2 = 0;
			for (int i = 0; i < FrameCount; i++)
			{
				ImgFrame item = new ImgFrame
				{
					DataOffset = (int)(FrameTableOffset + FrameDataLength + num2),
					RawData = RawData,
					FrameIndex = i,
					PixelFormatCode = BitConverter.ToUInt32(RawData, num)
				};
				num += 4;
				item.CompressionType = BitConverter.ToUInt32(RawData, num);
				num += 4;
				if (item.PixelFormatCode == 17)
				{
					list.Add(item);
					continue;
				}
				item.Width = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.Height = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.DataLength = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.OffsetX = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.OffsetY = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.CanvasWidth = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.CanvasHeight = BitConverter.ToUInt32(RawData, num);
				num += 4;
				if (item.CompressionType == 5)
				{
					switch (item.PixelFormatCode)
					{
					case 16u:
						num2 += (int)item.DataLength;
						break;
					case 14u:
					case 15u:
						num2 += (int)item.DataLength / 2;
						break;
					}
				}
				else
				{
					num2 += (int)item.DataLength;
				}
				list.Add(item);
			}
			return list.ToArray();
		}

		public void ReadSelectedFrames(HashSet<int> frameIndexes, Dictionary<int, ImageSource> images)
		{
			int num = FrameTableOffset;
			int num2 = 0;
			for (int i = 0; i < FrameCount; i++)
			{
				ImgFrame item = new ImgFrame
				{
					DataOffset = (int)(FrameTableOffset + FrameDataLength + num2),
					RawData = RawData,
					FrameIndex = i,
					PixelFormatCode = BitConverter.ToUInt32(RawData, num)
				};
				num += 4;
				item.CompressionType = BitConverter.ToUInt32(RawData, num);
				num += 4;
				if (item.PixelFormatCode == 17)
				{
					continue;
				}
				item.Width = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.Height = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.DataLength = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.OffsetX = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.OffsetY = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.CanvasWidth = BitConverter.ToUInt32(RawData, num);
				num += 4;
				item.CanvasHeight = BitConverter.ToUInt32(RawData, num);
				num += 4;
				if (item.CompressionType == 5)
				{
					switch (item.PixelFormatCode)
					{
					case 16u:
						num2 += (int)item.DataLength;
						break;
					case 14u:
					case 15u:
						num2 += (int)item.DataLength / 2;
						break;
					}
				}
				else
				{
					num2 += (int)item.DataLength;
				}
				images.Add(i, item.Decode().ToBitmapSource());
			}
		}
	}

	internal struct ImgFrame
	{
		internal uint PixelFormatCode;

		internal uint CompressionType;

		internal uint Width;

		internal uint Height;

		internal uint DataLength;

		internal uint OffsetX;

		internal uint OffsetY;

		internal uint CanvasWidth;

		internal uint CanvasHeight;

		internal int DataOffset;

		internal byte[] RawData;

		internal int FrameIndex;

		internal DecodedFrame Decode()
		{
			byte[] array = new byte[3145728];
			if (PixelFormatCode == 17)
			{
				return default(DecodedFrame);
			}
			uint num = DataLength;
			if (CompressionType == 5)
			{
				switch (PixelFormatCode)
				{
				case 16u:
					num = DataLength;
					break;
				case 14u:
				case 15u:
					num = DataLength / 2;
					break;
				}
			}
			switch (CompressionType)
			{
			case 6u:
			{
				MemoryStream memoryStream2 = new MemoryStream(RawData, DataOffset + 2, (int)(num - 2));
				DeflateStream deflateStream = new DeflateStream(memoryStream2, CompressionMode.Decompress);
				try
				{
					deflateStream.Read(array, 0, array.Length);
				}
				catch (Exception ex)
				{
					throw new Exception("compress error!" + ex.Message);
				}
				memoryStream2.Close();
				deflateStream.Close();
				break;
			}
			case 5u:
			{
				MemoryStream memoryStream = new MemoryStream(RawData, DataOffset + 2, (int)num);
				memoryStream.Read(array, 0, array.Length);
				memoryStream.Close();
				break;
			}
			default:
				throw new Exception("error unknown compress type: " + CompressionType + " in file ");
			}
			return new DecodedFrame((int)Width, (int)Height, PixelFormatCode, array, FrameIndex);
		}

		public override string ToString()
		{
			string text;
			switch (PixelFormatCode)
			{
			case 14u:
				text = "ARGB_1555";
				break;
			case 15u:
				text = "ARGB_4444";
				break;
			case 16u:
				text = "ARGB_8888";
				break;
			case 17u:
				return "Link file. offset=>" + DataOffset;
			default:
				text = "nonononononono";
				break;
			}
			return "{ " + FrameIndex + ": key=>(" + OffsetX + "," + OffsetY + "), wh=>(" + Width + ", " + Height + "), max=>(" + CanvasWidth + ", " + CanvasHeight + "), type=>" + text + " offset=>" + DataOffset + "}";
		}
	}

	internal struct DecodedFrame
	{
		internal int Width;

		internal int Height;

		internal uint PixelFormatCode;

		internal byte[] PixelData;

		internal int FrameIndex;

		internal DecodedFrame(int width, int height, uint pixelFormatCode, byte[] pixelData, int frameIndex)
		{
			Width = width;
			Height = height;
			PixelFormatCode = pixelFormatCode;
			PixelData = pixelData;
			FrameIndex = frameIndex;
		}

		public override string ToString()
		{
			string text = PixelFormatCode switch
			{
				14u => "ARGB_1555", 
				15u => "ARGB_4444", 
				16u => "ARGB_8888", 
				17u => "ARGB_NONE", 
				_ => "nonononononono", 
			};
			return "{ " + FrameIndex + ": wh=>(" + Width + "," + Height + "), type=>" + text + " }";
		}

		public Bitmap ToBitmap()
		{
			Bitmap bitmap = new Bitmap(Width, Height);
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					switch (PixelFormatCode)
					{
					case 14u:
					{
						int red3 = ((PixelData[i * Width * 2 + j * 2 + 1] & 0x7F) >> 2 << 3) % 256;
						int green3 = ((((PixelData[i * Width * 2 + j * 2 + 1] & 3) << 3) | ((PixelData[i * Width * 2 + j * 2] >> 5) & 7)) << 3) % 256;
						int blue3 = ((PixelData[i * Width * 2 + j * 2] & 0x3F) << 3) % 256;
						int alpha3 = (((PixelData[i * Width * 2 + j * 2 + 1] >> 7) % 256 != 0) ? 255 : 0);
						bitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(alpha3, red3, green3, blue3));
						break;
					}
					case 15u:
					{
						int red2 = ((PixelData[i * Width * 2 + j * 2 + 1] & 0xF) << 4) % 256;
						int green2 = ((PixelData[i * Width * 2 + j * 2] & 0xF0) >> 4 << 4) % 256;
						int blue2 = ((PixelData[i * Width * 2 + j * 2] & 0xF) << 4) % 256;
						int alpha2 = (PixelData[i * Width * 2 + j * 2 + 1] & 0xF0) >> 4 << 4;
						bitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(alpha2, red2, green2, blue2));
						break;
					}
					case 16u:
					{
						int red = PixelData[i * Width * 4 + j * 4 + 2];
						int green = PixelData[i * Width * 4 + j * 4 + 1];
						int blue = PixelData[i * Width * 4 + j * 4];
						int alpha = PixelData[i * Width * 4 + j * 4 + 3];
						bitmap.SetPixel(j, i, System.Drawing.Color.FromArgb(alpha, red, green, blue));
						break;
					}
					default:
						Console.WriteLine("error known type:" + PixelFormatCode);
						break;
					case 17u:
						break;
					}
				}
			}
			return bitmap;
		}

		public BitmapSource ToBitmapSource()
		{
			PixelFormat bgra = PixelFormats.Bgra32;
			int stride = Width * bgra.BitsPerPixel / 8;
			return BitmapSource.Create(Width, Height, 0.0, 0.0, bgra, null, PixelData, stride);
		}
	}

	private static readonly char[] FileNameDecodeKey = "puchikon@neople dungeon and fighter DNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNFDNF\u0000".ToCharArray();

	internal List<NpkEntry> Entries;

	public NpkImageArchiveReader()
	{
	}

	internal NpkImageArchiveReader(string archivePath)
	{
		int num = 0;
		using FileStream fileStream = new FileStream(archivePath, FileMode.Open, FileAccess.Read);
		byte[] array = new byte[fileStream.Length];
		fileStream.Read(array, 0, (int)fileStream.Length);
		NpkHeader header = new NpkHeader
		{
			Signature = Encoding.UTF8.GetString(array, num, 16)
		};
		if (header.Signature != "NeoplePack_Bill\u0000")
		{
			return;
		}
		num += 16;
		header.EntryCount = BitConverter.ToUInt32(array, num);
		num += 4;
		Entries = new List<NpkEntry>();
		for (int i = 0; i < header.EntryCount; i++)
		{
			NpkEntry item = new NpkEntry
			{
				ImageOffset = (int)BitConverter.ToUInt32(array, num)
			};
			if (item.ImageOffset == array.Length)
			{
				continue;
			}
			if (Encoding.UTF8.GetString(array, item.ImageOffset, 16) != "Neople Img File\u0000")
			{
				num += 264;
				continue;
			}
			num += 4;
			item.ImageSize = BitConverter.ToUInt32(array, num);
			num += 4;
			char[] array2 = new char[256];
			for (int j = 0; j < 256; j++)
			{
				array2[j] = (char)(array[num++] ^ FileNameDecodeKey[j]);
			}
			item.FilePath = new string(array2);
			item.FilePath = item.FilePath.Replace("\u0000", "");
			item.FileName = Regex.Replace(item.FilePath, "^.*/", "");
			item.ArchiveData = array;
			item.ReadImageHeader().ReadFrames();
			Entries.Add(item);
		}
	}

	internal void LoadSelectedImages(Dictionary<string, Dictionary<string, HashSet<int>>> requests)
	{
		Dictionary<string, Dictionary<int, ImageSource>> imagesByFile = new Dictionary<string, Dictionary<int, ImageSource>>();
		foreach (KeyValuePair<string, Dictionary<string, HashSet<int>>> item in requests)
		{
			LoadSelectedImagesFromArchive(item.Key, item.Value, imagesByFile);
		}
	}

	private void LoadSelectedImagesFromArchive(string archivePath, Dictionary<string, HashSet<int>> requestedFrames, Dictionary<string, Dictionary<int, ImageSource>> imagesByFile)
	{
		if (!File.Exists(archivePath))
		{
			return;
		}
		int num = 0;
		using FileStream fileStream = new FileStream(archivePath, FileMode.Open, FileAccess.Read);
		byte[] array = new byte[fileStream.Length];
		fileStream.Read(array, 0, (int)fileStream.Length);
		NpkHeader header = new NpkHeader
		{
			Signature = Encoding.UTF8.GetString(array, num, 16)
		};
		if (header.Signature != "NeoplePack_Bill\u0000")
		{
			return;
		}
		num += 16;
		header.EntryCount = BitConverter.ToUInt32(array, num);
		num += 4;
		for (int i = 0; i < header.EntryCount; i++)
		{
			NpkEntry entry = new NpkEntry
			{
				ImageOffset = (int)BitConverter.ToUInt32(array, num)
			};
			if (entry.ImageOffset == array.Length)
			{
				continue;
			}
			if (Encoding.UTF8.GetString(array, entry.ImageOffset, 16) != "Neople Img File\u0000")
			{
				num += 264;
				continue;
			}
			num += 4;
			entry.ImageSize = BitConverter.ToUInt32(array, num);
			num += 4;
			char[] array2 = new char[256];
			for (int j = 0; j < 256; j++)
			{
				array2[j] = (char)(array[num++] ^ FileNameDecodeKey[j]);
			}
			entry.FilePath = new string(array2).Replace("\u0000", string.Empty);
			if (!requestedFrames.TryGetValue(entry.FilePath, out HashSet<int> value) || imagesByFile.ContainsKey(entry.FilePath))
			{
				continue;
			}
			entry.ArchiveData = array;
			Dictionary<int, ImageSource> dictionary = new Dictionary<int, ImageSource>();
			try
			{
				entry.ReadImageHeader().ReadSelectedFrames(value, dictionary);
				if (dictionary.Count > 0)
				{
					imagesByFile.Add(entry.FilePath, dictionary);
				}
			}
			catch (Exception)
			{
			}
		}
	}

}
