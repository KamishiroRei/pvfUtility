using System.Collections.Generic;
using System.IO;
using PvfCode.Dot;

namespace PvfCode.ViewModels.TreeFolder;

public class PvfTreeFileFileListDescription : PvfTreeFileBase
{
	private readonly Dictionary<string, TreelistCommentRes> xicF0DhjTP;

	public override string? Comment
	{
		get
		{
			string text = base.FullPath.GetTreeListComment(xicF0DhjTP)?.Comment;
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
			TreelistCommentRes treeListComment = base.FullPath.GetTreeListComment(xicF0DhjTP);
			if (treeListComment == null || string.IsNullOrEmpty(treeListComment.DetailedComment))
			{
				return base.FullPath;
			}
			return treeListComment.DetailedComment;
		}
	}

	public PvfTreeFileFileListDescription(PvfGroup pvf, string fullPath, string fileName, bool isFile, short level, Dictionary<string, TreelistCommentRes> source)
		: base(pvf, fullPath, fileName, isFile, level)
	{
		base.TreeType = TreeViewType.FileList;
		xicF0DhjTP = source;
	}

	public override bool IsFileMethon()
	{
		bool flag = !string.IsNullOrEmpty(Path.GetExtension(base.FullPath));
		SetIsFile(flag);
		return flag;
	}
}
