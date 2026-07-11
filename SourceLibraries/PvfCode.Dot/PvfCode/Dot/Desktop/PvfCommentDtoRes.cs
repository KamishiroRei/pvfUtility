using PvfCode.Dot.Desktop.Enums;

namespace PvfCode.Dot.Desktop;

public class PvfCommentDtoRes
{
	private string _Section;

	public PvfFileType? FileType { get; set; }

	public PvfCommentType PvfCommentType { get; set; }

	public string Section
	{
		get
		{
			if (_Section != null)
			{
				_Section = _Section.Replace("[", "").Replace("[/", "").Replace("]", "")
					.Replace("`", "");
			}
			return _Section;
		}
		set
		{
			_Section = value;
		}
	}
}
