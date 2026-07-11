using System;
using PvfCode.Dot.Desktop.Enums;
using SqlSugar;

namespace PvfCode.Dot.Desktop;

[SugarTable("pvf_comment")]
public class PvfCommentDto : ModelBase, ICloneable
{
	public delegate void FileTypeChangedDelegate(PvfFileType? fileType);

	private PvfFileType? _FileType;

	private string _Section;

	private string _Comment;

	private string _Title;

	private string _OfficialDescription;

	private DateTime? _UpdateTime;

	[SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "Id")]
	public int Id { get; set; }

	[SugarColumn(IsNullable = true)]
	public PvfCommentType PvfCommentType { get; set; }

	[SugarColumn(IsNullable = true)]
	public PvfFileType? FileType
	{
		get
		{
			return _FileType;
		}
		set
		{
			_FileType = value;
			DoNotify("FileType");
			this.OnFileTypeChanged?.Invoke(value);
		}
	}

	public string Section
	{
		get
		{
			return _Section;
		}
		set
		{
			_Section = value;
			DoNotify("Section");
		}
	}

	[SugarColumn(IsIgnore = true)]
	public string Title
	{
		get => _Title;
		set
		{
			_Title = value;
			DoNotify("Title");
		}
	}

	[SugarColumn(ColumnDataType = "text", IsNullable = true)]
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

	[SugarColumn(IsIgnore = true)]
	public string OfficialDescription
	{
		get => _OfficialDescription;
		set
		{
			_OfficialDescription = value;
			DoNotify("OfficialDescription");
		}
	}

	[SugarColumn(IsIgnore = true)]
	public bool Closing { get; set; }

	[SugarColumn(IsIgnore = true)]
	public int UserId { get; set; }

	[SugarColumn(IsNullable = true)]
	public string Authors { get; set; }

	public DateTime Create { get; set; }

	public DateTime? UpdateTime
	{
		get
		{
			if (!_UpdateTime.HasValue)
			{
				_UpdateTime = Create;
			}
			return _UpdateTime;
		}
		set
		{
			_UpdateTime = value;
		}
	}

	public event FileTypeChangedDelegate OnFileTypeChanged;

	public object Clone()
	{
		return MemberwiseClone();
	}

	public PvfCommentDto CloneData()
	{
		return (PvfCommentDto)Clone();
	}
}
