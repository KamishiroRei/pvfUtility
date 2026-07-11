using System;

namespace PvfCode.Dot;

public class TreelistCommentRes : ModelBase, ICloneable
{
	private string _Comment;

	private string _DetailedComment;

	private string _NickName;

	public int Id { get; set; }

	public string FilePath { get; set; }

	public string Comment
	{
		get
		{
			return _Comment;
		}
		set
		{
			_Comment = value;
			DoNotify("Comment");
		}
	}

	public string DetailedComment
	{
		get
		{
			return _DetailedComment;
		}
		set
		{
			_DetailedComment = value;
			DoNotify("DetailedComment");
		}
	}

	public string NickName
	{
		get
		{
			return _NickName;
		}
		set
		{
			_NickName = value;
			DoNotify("NickName");
		}
	}

	public string NickNames { get; set; }

	public bool MasterIsShare { get; set; }

	public DateTime Create { get; set; }

	public object Clone()
	{
		return MemberwiseClone();
	}

	public TreelistCommentRes CloneData()
	{
		return (TreelistCommentRes)Clone();
	}
}
