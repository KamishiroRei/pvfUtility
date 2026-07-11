using System.Net;
using System.Net.NetworkInformation;

namespace Utools;

public class PortHelper
{
	public static bool PortInUse(int port)
	{
		bool result = false;
		IPEndPoint[] activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
		for (int i = 0; i < activeTcpListeners.Length; i++)
		{
			if (activeTcpListeners[i].Port == port)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public PortHelper()
	{
	}
}
