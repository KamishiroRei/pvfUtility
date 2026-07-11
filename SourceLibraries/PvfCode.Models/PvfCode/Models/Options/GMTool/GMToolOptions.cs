using System.Collections.Generic;
using DevExpress.Mvvm;
using GMTool.Dot;
using Newtonsoft.Json;

namespace PvfCode.Models.Options.GMTool;

[JsonObject(MemberSerialization.OptOut)]
public class GMToolOptions : ViewModelBase
{
	private string CD5Ee5jSKq;

	private string JTMEjcl4Gx;

	private TopUpOptions zFYEx9sy7v;

	private List<int> yEEEm5Evpi;

	public string PostalSendTitle
	{
		get
		{
			if (string.IsNullOrEmpty(CD5Ee5jSKq))
			{
				CD5Ee5jSKq = "pvfUtility";
			}
			return CD5Ee5jSKq;
		}
		set
		{
			CD5Ee5jSKq = value;
			RaisePropertyChanged("PostalSendTitle");
		}
	}

	public string PostalSendText
	{
		get
		{
			if (string.IsNullOrEmpty(JTMEjcl4Gx))
			{
				JTMEjcl4Gx = "TestPostal";
			}
			return JTMEjcl4Gx;
		}
		set
		{
			JTMEjcl4Gx = value;
			RaisePropertyChanged("PostalSendText");
		}
	}

	public TopUpOptions TopUpOptions
	{
		get
		{
			if (zFYEx9sy7v == null)
			{
				zFYEx9sy7v = new TopUpOptions();
			}
			return zFYEx9sy7v;
		}
		set
		{
			zFYEx9sy7v = value;
		}
	}

	[JsonIgnore]
	public List<int> UpgradeValueList
	{
		get
		{
			if (yEEEm5Evpi == null)
			{
				yEEEm5Evpi = new List<int>();
				for (int i = 0; i < 32; i++)
				{
					yEEEm5Evpi.Add(i);
				}
			}
			return yEEEm5Evpi;
		}
	}

	public GMToolOptions()
	{
	}
}
