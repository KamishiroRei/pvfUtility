using System;
using System.Threading.Tasks;
using Utools;

namespace PvfCode;

public class CloudOptionsService
{
	private static CloudOptionsService RI81ecc0g;

	private static CloudOptions qvqybQAAJ;

	public static CloudOptionsService Instance
	{
		get
		{
			if (RI81ecc0g == null)
			{
				RI81ecc0g = new CloudOptionsService();
			}
			return RI81ecc0g;
		}
	}

	public static CloudOptions Options
	{
		get
		{
			if (qvqybQAAJ == null)
			{
				qvqybQAAJ = new CloudOptions();
				qvqybQAAJ.InitDefault();
			}
			return qvqybQAAJ;
		}
	}

	public async Task<bool> Init()
	{
		try
		{
			qvqybQAAJ = new CloudOptions();
			qvqybQAAJ.InitDefault();
			return true;
		}
		catch (Exception e)
		{
			AppSetting.Instance.GetIlogger()?.ErrorUploadDialog(e, "获取云端配置项失败 该异常不影响程序正常使用 可忽略");
		}
		return false;
	}

	public string GetNewTxt()
	{
		CloudOptions cloudOptions = new CloudOptions();
		cloudOptions.InitDefault();
		return cloudOptions.ToJson().TextEncrypt("sgdf1g1sdf4353466()__(*)");
	}

	public CloudOptionsService()
	{
	}
}
