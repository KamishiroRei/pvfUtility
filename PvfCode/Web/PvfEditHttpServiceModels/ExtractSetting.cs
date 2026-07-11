
namespace PvfCode.Web.PvfEditHttpServiceModels;

public class ExtractSetting
{
	public bool IsEncryptScript;

	public bool IsEncryptAni;

	public bool IsConvertChinese;

	public ExtractSetting()
	{
		IsEncryptScript = true;
		IsEncryptAni = true;
		IsConvertChinese = false;
	}
}
