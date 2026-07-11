using Newtonsoft.Json;

namespace PvfCode;

[JsonObject(MemberSerialization.OptOut)]
public class ClientApiOptions : ModelBase
{
	private int? TZ3tFwkhw;

	private bool VfI9E7n0U;

	private bool glvRLjvuo;

	[JsonIgnore]
	public int Port
	{
		get
		{
			if (!TZ3tFwkhw.HasValue)
			{
				TZ3tFwkhw = 27000;
			}
			return TZ3tFwkhw.Value;
		}
		set
		{
			TZ3tFwkhw = value;
			DoNotify("Port");
		}
	}

	[JsonIgnore]
	public bool StartLoading
	{
		get
		{
			return VfI9E7n0U;
		}
		set
		{
			VfI9E7n0U = value;
			DoNotify("StartLoading");
		}
	}

	public bool UseCompatibleDecompiler
	{
		get
		{
			return glvRLjvuo;
		}
		set
		{
			glvRLjvuo = value;
			DoNotify("UseCompatibleDecompiler");
		}
	}

	public ClientApiOptions()
	{
	}
}
