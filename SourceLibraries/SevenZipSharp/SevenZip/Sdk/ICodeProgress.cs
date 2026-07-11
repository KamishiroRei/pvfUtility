namespace SevenZip.Sdk;

public interface ICodeProgress
{
	void SetProgress(long inSize, long outSize);
}
