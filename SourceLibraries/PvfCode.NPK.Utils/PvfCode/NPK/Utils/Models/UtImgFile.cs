namespace PvfCode.NPK.Utils.Models;

public class UtImgFile
{
	public int Count { get; set; }

	public int FileNameId { get; }

	public UtImgFile(int fileNameId)
	{
		FileNameId = fileNameId;
	}
}
