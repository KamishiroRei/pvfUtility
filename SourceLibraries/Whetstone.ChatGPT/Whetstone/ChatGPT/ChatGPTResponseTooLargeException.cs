using System;

namespace Whetstone.ChatGPT;

public sealed class ChatGPTResponseTooLargeException : InvalidOperationException
{
	public long MaximumResponseBytes { get; }

	internal ChatGPTResponseTooLargeException(long maximumResponseBytes)
		: base($"ChatGPT response exceeds the configured {maximumResponseBytes} byte limit.")
	{
		MaximumResponseBytes = maximumResponseBytes;
	}
}
