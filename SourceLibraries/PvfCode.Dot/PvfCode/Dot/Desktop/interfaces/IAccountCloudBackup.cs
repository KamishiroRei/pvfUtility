namespace PvfCode.Dot.Desktop.interfaces;

public interface IAccountCloudBackup
{
	UserCloudBackUpData? BookMark { get; set; }

	UserCloudBackUpData? AppSetting { get; set; }

	UserCloudBackUpData? ItemCodeHoverConfig { get; set; }

	UserCloudBackUpData? TreeListComment { get; set; }

	UserCloudBackUpData? SectionComment { get; set; }
}
