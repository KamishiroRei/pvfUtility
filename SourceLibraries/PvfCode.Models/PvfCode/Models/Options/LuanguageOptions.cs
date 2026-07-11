using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class LuanguageOptions : ViewModelBase
{
	[JsonIgnore]
	public string GoToStoreShareOption_BookMark => AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_ShareSetting") + "/" + AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_Bookmark");

	[JsonIgnore]
	public string GoToStoreShareOption_FileExplorerComment => AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_ShareSetting") + "/" + AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_FileExplorerComment");

	[JsonIgnore]
	public string GoToStoreShareOption_TagTranslation => AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_ShareSetting") + "/" + AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_TagTranslation");

	[JsonIgnore]
	public string GoToStoreShareOption_CodeIntelliSense => AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_ShareSetting") + "/" + AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_CodeIntelliSense");

	[JsonIgnore]
	public string GoToStoreShareOption_ShareSetting => AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_Setting_ShareSetting") ?? "";

	[JsonIgnore]
	public string GoToGameLoginOption_ShareSetting => AppSetting.Instance.GetIlogger()?.GetStr("ViewGlobalOptions_TreeMenu_StartGameConfig") ?? "";

	public LuanguageOptions()
	{
	}
}
