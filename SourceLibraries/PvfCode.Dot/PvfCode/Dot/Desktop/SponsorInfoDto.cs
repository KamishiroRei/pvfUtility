using PvfCode.Dot.Desktop.interfaces;

namespace PvfCode.Dot.Desktop;

public class SponsorInfoDto : IContributeInfo
{
	public string NickName { get; set; }

	public string QQ { get; set; }

	public string Photo { get; set; }

	public string Tooltip { get; set; }
}
