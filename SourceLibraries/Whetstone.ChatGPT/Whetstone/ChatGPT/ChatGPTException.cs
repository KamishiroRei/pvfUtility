using System;
using System.Diagnostics;
using System.Net;
using Whetstone.ChatGPT.Models;

namespace Whetstone.ChatGPT;

[DebuggerDisplay("Message = {Message}, StatusCode = {StatusCode}")]
public sealed class ChatGPTException : Exception
{
	public ChatGPTError? ChatGPTError { get; private set; }

	public HttpStatusCode? StatusCode { get; private set; }

	internal ChatGPTException(ChatGPTError? chatGptError, HttpStatusCode statusCode)
		: base(chatGptError?.Message)
	{
		ChatGPTError = chatGptError;
		StatusCode = statusCode;
	}

	internal ChatGPTException(string message, HttpStatusCode statusCode)
		: base(message)
	{
		StatusCode = statusCode;
	}

	internal ChatGPTException(string message, Exception innerEx)
		: base(message, innerEx)
	{
	}

	internal ChatGPTException(string message)
		: base(message)
	{
	}
}
