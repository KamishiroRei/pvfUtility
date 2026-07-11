using Newtonsoft.Json;

namespace PvfCode.NPK.Utils.Models;

[JsonObject(MemberSerialization.OptOut)]
public class UtImgFileTemp : UtImgFile
{
	[JsonIgnore]
	public int? Offset { get; }

	public UtImgFileTemp(int offset, int fileNameId)
		: base(fileNameId)
	{
		Offset = offset;
	}
}
