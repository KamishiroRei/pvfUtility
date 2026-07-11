using System;

namespace PvfCode.Dot.Desktop.AdvertisingPosition;

public class AdvertisingPositionInfoDto : IAdvertisingPositionInfo
{
	public string? Title { get; set; }

	public string? ShortTitle { get; set; }

	public string? Url { get; set; }

	public string? ImageUrl { get; set; }

	public int Id { get; set; }

	public DateTime Create { get; set; }

	public DateTime OverTime { get; set; }

	public DateTime? ReleaseDate { get; set; }

	public string? Tooltip { get; set; }

	public string? Description { get; set; }

	public bool IsTop { get; set; }
}
