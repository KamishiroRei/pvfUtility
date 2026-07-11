using System;
using System.IO;

namespace Whetstone.ChatGPT.Models;

public class ChatGPTFileContent
{
	public byte[]? Content { get; set; }

	public string? FileName { get; set; }

	public ChatGPTFileContent()
	{
	}

	public ChatGPTFileContent(byte[] content, string fileName)
	{
		if (content == null || content.Length == 0)
		{
			throw new ArgumentException("content is required");
		}
		if (string.IsNullOrWhiteSpace(fileName))
		{
			throw new ArgumentException("fileName is required");
		}
		Content = content;
		FileName = fileName;
	}

	public static ChatGPTFileContent Load(string file)
	{
		byte[] content = File.ReadAllBytes(file);
		string fileName = Path.GetFileName(file);
		return new ChatGPTFileContent(content, fileName);
	}
}
