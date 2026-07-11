namespace PvfCode.LoggerBase;

public class ErrorItem
{
	public string Description { get; set; }

	public string Input { get; set; }

	public int Line { get; set; }

	public string FilePath { get; set; }

	public ErrorItem(string input, int line, string filePath)
	{
		Input = input;
		Line = line;
		FilePath = filePath;
	}

	public override string ToString()
	{
		return $"{Description}\r\n{Input}\r\n{Line}\r\n{FilePath}";
	}
}
