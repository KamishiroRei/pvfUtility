using System;

namespace SevenZip.Sdk;

[Serializable]
internal class InvalidParamException : ApplicationException
{
	public InvalidParamException()
		: base("Invalid Parameter")
	{
	}
}
