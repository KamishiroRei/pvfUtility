using PvfCode.Dot.Desktop.interfaces;

namespace PvfCode.Dot.Desktop;

public class AccountCloudBackupDto : ModelBase, IAccountCloudBackup
{
	private UserCloudBackUpData? _BookMark;

	private UserCloudBackUpData _AppSetting;

	private UserCloudBackUpData _ItemCodeHoverConfig;

	private UserCloudBackUpData _TreeListComment;

	private UserCloudBackUpData _SectionComment;

	public UserCloudBackUpData? BookMark
	{
		get
		{
			return _BookMark;
		}
		set
		{
			_BookMark = value;
			DoNotify("BookMark");
		}
	}

	public UserCloudBackUpData AppSetting
	{
		get
		{
			return _AppSetting;
		}
		set
		{
			_AppSetting = value;
			DoNotify("AppSetting");
		}
	}

	public UserCloudBackUpData ItemCodeHoverConfig
	{
		get
		{
			return _ItemCodeHoverConfig;
		}
		set
		{
			_ItemCodeHoverConfig = value;
			DoNotify("ItemCodeHoverConfig");
		}
	}

	public UserCloudBackUpData TreeListComment
	{
		get
		{
			return _TreeListComment;
		}
		set
		{
			_TreeListComment = value;
			DoNotify("TreeListComment");
		}
	}

	public UserCloudBackUpData SectionComment
	{
		get
		{
			return _SectionComment;
		}
		set
		{
			_SectionComment = value;
			DoNotify("SectionComment");
		}
	}
}
