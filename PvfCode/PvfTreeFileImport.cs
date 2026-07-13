using PvfCode.Dot;
using PvfCode.Models.Pvf.ImportModels;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode;

public sealed class PvfTreeFileImport : PvfTreeFileBase
{
	public ImportFileItem? ImportItem { get; set; }

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

	public PvfTreeFileImport(PvfGroup pvf, string fullPath, string fileName, bool isFile, short level, ImportFileItem? importItem = null)
		: base(pvf, fullPath, fileName, isFile, level)
	{
		base.TreeType = TreeViewType.FileList;
		ImportItem = importItem;
		if (importItem != null)
		{
			importItem.TreeFullPath = base.FullPath;
			ImportItem = importItem;
		}
	}
}
