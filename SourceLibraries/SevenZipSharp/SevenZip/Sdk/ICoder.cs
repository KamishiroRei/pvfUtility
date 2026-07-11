using System.IO;

namespace SevenZip.Sdk;

public interface ICoder
{
	void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress);
}
