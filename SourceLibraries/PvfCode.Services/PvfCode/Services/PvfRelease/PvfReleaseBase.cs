using System.Threading.Tasks;
using PvfCode.Dot;

namespace PvfCode.Services.PvfRelease;

public abstract class PvfReleaseBase
{
	public readonly PvfGroup Pvf;

	public PvfReleaseBase(PvfGroup pvf)
	{
		Pvf = pvf;
	}

	public abstract Task<ResultData> Start();

	public abstract Task<ResultData> Start(PvfReleaseClientOptions options);

	public Task WriteBlankFiles()
	{
		return Task.CompletedTask;
	}
}
