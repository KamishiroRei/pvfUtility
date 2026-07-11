using System.IO;

namespace SevenZip.Sdk;

internal interface IWriteCoderProperties
{
	void WriteCoderProperties(Stream outStream);
}
