using PvfCode.Dot;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode;

public class PvfTreeFile : PvfTreeFileBase
{
	public override string? Comment
	{
		get
		{
			string text = base.FullPath.GetTreeListComment()?.Comment;
			if (text == null)
			{
				return null;
			}
			return "(" + text + ")";
		}
	}

	public override string? DetailedComment
	{
		get
		{
			TreelistCommentRes treeListComment = base.FullPath.GetTreeListComment();
			if (treeListComment == null || string.IsNullOrEmpty(treeListComment.DetailedComment))
			{
				return base.FullPath;
			}
			return treeListComment.DetailedComment;
		}
	}

	public PvfTreeFile(PvfGroup pvf, string fullPath, string fileName, bool isFile, short level)
		: base(pvf, fullPath, fileName, isFile, level)
	{
		base.TreeType = TreeViewType.FileList;
	}
}
