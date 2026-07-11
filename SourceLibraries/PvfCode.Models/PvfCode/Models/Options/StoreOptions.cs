using PvfCode.Dot.Desktop;
using PvfCode.Dot.Desktop.Enums;

namespace PvfCode.Models.Options;

public class StoreOptions : ModelBase
{
	private StoreListRes leoEchC0YK;

	private bool VhEEghpen1;

	private bool k4uEKWLQJ2;

	private StoreListRes zBMEYG2vhg;

	private bool yn4EJEZSS2;

	private StoreListRes hC9EdcUfCJ;

	public StoreListRes FileListComment
	{
		get
		{
			if (leoEchC0YK == null)
			{
				leoEchC0YK = new StoreListRes
				{
					Type = StoreType.文件资源管理器注释
				};
			}
			return leoEchC0YK;
		}
		set
		{
			leoEchC0YK = value;
		}
	}

	public bool UploadFileListComment
	{
		get
		{
			return VhEEghpen1;
		}
		set
		{
			VhEEghpen1 = value;
			DoNotify("UploadFileListComment");
		}
	}

	public bool UploadTabComment
	{
		get
		{
			return k4uEKWLQJ2;
		}
		set
		{
			k4uEKWLQJ2 = value;
			DoNotify("UploadTabComment");
		}
	}

	public StoreListRes TabComment
	{
		get
		{
			if (zBMEYG2vhg == null)
			{
				zBMEYG2vhg = new StoreListRes
				{
					Type = StoreType.脚本文件标签翻译
				};
			}
			return zBMEYG2vhg;
		}
		set
		{
			zBMEYG2vhg = value;
		}
	}

	public bool UploadItemCodeHoverConfig
	{
		get
		{
			return yn4EJEZSS2;
		}
		set
		{
			yn4EJEZSS2 = value;
			DoNotify("UploadItemCodeHoverConfig");
		}
	}

	public StoreListRes ItemCodeHoverConfig
	{
		get
		{
			if (hC9EdcUfCJ == null)
			{
				hC9EdcUfCJ = new StoreListRes
				{
					Type = StoreType.代码智能提示
				};
			}
			return hC9EdcUfCJ;
		}
		set
		{
			hC9EdcUfCJ = value;
		}
	}

	public StoreOptions()
	{
	}
}
