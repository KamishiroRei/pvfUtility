using System;

namespace PvfCode.Dot.Desktop.AdvertisingPosition;

public interface IAdvertisingPositionInfo
{
	int Id { get; set; }

	string? Title { get; set; }

	string? ShortTitle { get; set; }

	string? Url { get; set; }

	string? ImageUrl { get; set; }

	DateTime Create { get; set; }

	DateTime OverTime { get; set; }

	DateTime? ReleaseDate { get; set; }

	string? Tooltip { get; set; }

	string? Description { get; set; }

	bool IsTop { get; set; }
}
