using System.Collections.Generic;
using PvfCode.Dot.Desktop;

namespace PvfCode.Dot;

public class PvfCodePublicCloudDataRes
{
	public Dictionary<string, TreelistCommentRes> TreeListComment { get; set; }

	public List<CodeCompletionDataDto> CodeCompletionDataDtos { get; set; }

	public IEnumerable<PvfCommentDto> PvfCommentDtoList { get; set; }

	public PvfCodePublicCloudDataRes()
	{
		TreeListComment = new Dictionary<string, TreelistCommentRes>();
		CodeCompletionDataDtos = new List<CodeCompletionDataDto>();
		PvfCommentDtoList = new List<PvfCommentDto>();
	}

	public void InitTreeListComment(Dictionary<string, TreelistCommentRes> dic)
	{
		if (TreeListComment == null)
		{
			TreeListComment = new Dictionary<string, TreelistCommentRes>();
		}
		foreach (KeyValuePair<string, TreelistCommentRes> item in dic)
		{
			if (!TreeListComment.ContainsKey(item.Key))
			{
				TreeListComment.Add(item.Key, item.Value);
			}
		}
	}
}
