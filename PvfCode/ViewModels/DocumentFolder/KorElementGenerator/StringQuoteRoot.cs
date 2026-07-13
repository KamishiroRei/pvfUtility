namespace PvfCode.ViewModels.DocumentFolder.KorElementGenerator;

public class StringQuoteRoot
{
	public string FileName { get; private set; }

	public int Count { get; set; }

	public StringQuoteRoot(string fileName, int count)
	{
		FileName = fileName;
		Count = count;
	}
}
