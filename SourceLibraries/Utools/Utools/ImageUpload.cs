using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utools;

public static class ImageUpload
{
	private static readonly HashSet<string> qnXxMtUKU;

	public static ValImageResult IsAllowedExtension(this byte[] imgArray, FileExtension[] fileEx = null)
	{
		if (fileEx == null)
		{
			fileEx = new FileExtension[5]
			{
				FileExtension.BMP,
				FileExtension.GIF,
				FileExtension.JPG,
				FileExtension.JPG,
				FileExtension.PNG
			};
		}
		if (imgArray == null || imgArray.Length == 0)
		{
			return ValImageResult.空字节的图片无法上传;
		}
		MemoryStream memoryStream = new MemoryStream(imgArray);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		string text = "";
		try
		{
			text = binaryReader.ReadByte().ToString();
			text += binaryReader.ReadByte();
		}
		catch
		{
		}
		binaryReader.Close();
		memoryStream.Close();
		FileExtension[] array = fileEx;
		foreach (FileExtension fileExtension in array)
		{
			if (int.Parse(text) == (int)fileExtension)
			{
				if (!imgArray.IsSecureUpfilePhoto())
				{
					return ValImageResult.不安全的图片;
				}
				return ValImageResult.OK;
			}
		}
		return ValImageResult.尚不支持的格式;
	}

	public static FileExtension? GetImageType(this byte[] imgArray)
	{
		if (imgArray == null || imgArray.Length == 0)
		{
			return null;
		}
		MemoryStream memoryStream = new MemoryStream(imgArray);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		string text = "";
		try
		{
			text = binaryReader.ReadByte().ToString();
			text += binaryReader.ReadByte();
		}
		catch
		{
		}
		binaryReader.Close();
		memoryStream.Close();
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (int.TryParse(text, out var result))
		{
			try
			{
				return (FileExtension)result;
			}
			catch (Exception)
			{
			}
		}
		return null;
	}

	public static bool IsSecureUpfilePhoto(this byte[] bytes)
	{
		return IsSecureUpfilePhoto(new StreamReader(bytes.BytesToStream(), Encoding.Default));
	}

	public static bool IsSecureUpfilePhoto(this string photoFile)
	{
		bool flag = false;
		string text = "Yes";
		string text2 = Path.GetExtension(photoFile).ToLower();
		string[] array = new string[5]
		{
			".gif",
			".png",
			".jpeg",
			".jpg",
			".bmp"
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (text2 == array[i])
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return true;
		}
		StreamReader streamReader = new StreamReader(photoFile, Encoding.Default);
		string text3 = streamReader.ReadToEnd();
		streamReader.Close();
		string[] array2 = "request|<script|.getfolder|.createfolder|.deletefolder|.createdirectory|.deletedirectory|.saveas|wscript.shell|script.encode|server.|.createobject|execute|activexobject|language=".Split('|');
		foreach (string value in array2)
		{
			if (text3.ToLower().IndexOf(value) != -1)
			{
				File.Delete(photoFile);
				text = "No";
				break;
			}
		}
		return text == "Yes";
	}

	public static bool IsSecureUpfilePhoto(StreamReader sr)
	{
		string text = "Yes";
		string text2 = sr.ReadToEnd();
		sr.Close();
		string[] array = "request|<script|.getfolder|.createfolder|.deletefolder|.createdirectory|.deletedirectory|.saveas|wscript.shell|script.encode|server.|.createobject|execute|activexobject|language=".Split('|');
		foreach (string value in array)
		{
			if (text2.ToLower().IndexOf(value) != -1)
			{
				text = "No";
				break;
			}
		}
		return text == "Yes";
	}

	static ImageUpload()
	{
		qnXxMtUKU = new HashSet<string>
		{
			".gif",
			".png",
			".jpeg",
			".jpg",
			".bmp"
		};
	}
}
