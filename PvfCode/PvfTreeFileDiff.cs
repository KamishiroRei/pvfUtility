using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PvfCode.Dot;
using PvfCode.Models.Pvf.PvfFileDiffModels.Enums;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode;

public sealed class PvfTreeFileDiff : PvfTreeFileBase
{
	[CompilerGenerated]
	private List<PvfFileDiffType>? nmrjIcTWA7;

	public List<PvfFileDiffType>? Diffs
	{
		[CompilerGenerated]
		get
		{
			return nmrjIcTWA7;
		}
		[CompilerGenerated]
		set
		{
			nmrjIcTWA7 = value;
		}
	}

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

	public PvfTreeFileDiff(PvfGroup pvf, string fullPath, string fileName, bool isFile, short level, List<PvfFileDiffType>? diffs, TreeViewType treeType)
		: base(pvf, fullPath, fileName, isFile, level)
	{
		Diffs = diffs;
		base.TreeType = treeType;
	}
}
