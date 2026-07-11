using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PvfCode.Dot;

namespace PvfCode.Services.PvfRelease;

public class ServicePvfReleaseToServer : PvfReleaseBase
{
	public ServicePvfReleaseToServer(PvfGroup pvf)
		: base(pvf)
	{
	}

	public override async Task<ResultData> Start()
	{
		ResultData resultData = await LdwlaiQee8();
		if (resultData.IsError)
		{
			return resultData;
		}
		await goflKkf27A(Pvf.GetFiles(PvfFileType.obj).ToArray());
		await goflKkf27A(Pvf.GetFiles(PvfFileType.til).ToArray());
		await goflKkf27A(Pvf.GetFiles(PvfFileType.als).ToArray());
		await goflKkf27A(Pvf.GetFiles(PvfFileType.atk).ToArray());
		await goflKkf27A(Pvf.GetFiles(PvfFileType.act).ToArray());
		await goflKkf27A(Pvf.GetFiles(PvfFileType.ai).ToArray());
		await goflKkf27A(Pvf.GetFiles(PvfFileType.ptl).ToArray());
		await goflKkf27A(Pvf.GetFiles(PvfFileType.ani).ToArray());
		await goflKkf27A(Pvf.GetFiles(PvfFileType.ani).ToArray());
		return new ResultData();
	}

	private Task<ResultData> LdwlaiQee8()
	{
		IEnumerable<string> files = Pvf.GetFiles("sqr");
		if (files != null)
		{
			Pvf.DeleteFiles(files);
		}
		return Task.FromResult(new ResultData());
	}

	private Task goflKkf27A(IEnumerable<string> P_0)
	{
		if (P_0 == null)
		{
			return Task.CompletedTask;
		}
		lock (this)
		{
			foreach (string item in P_0)
			{
				if (Pvf.FileList.TryGetValue(item, out PvfFile _))
				{
					Pvf.FileList.Remove(item);
					Pvf.FileList.Add(item, new PvfFile(item));
				}
			}
		}
		return Task.CompletedTask;
	}

	public override async Task<ResultData> Start(PvfReleaseClientOptions options)
	{
		return await Start();
	}
}
