using System;

namespace SevenZip.Sdk;

[Serializable]
internal class DataErrorException : ApplicationException
{
	public DataErrorException()
		: base("Data Error")
	{
	}
}
