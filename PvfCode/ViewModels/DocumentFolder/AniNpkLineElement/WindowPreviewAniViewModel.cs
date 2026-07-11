using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Nito.AsyncEx;
using PvfCode.Services.PvfParsingNew.EditorPrivew;

namespace PvfCode.ViewModels.DocumentFolder.AniNpkLineElement;

public class WindowPreviewAniViewModel : ViewModelBase, IDisposable
{
	[CompilerGenerated]
	private List<PrivewAniData> H7iG6mqOQP;

	[CompilerGenerated]
	private CancellationTokenSource zTaG1kA2NW;

	private readonly AsyncLock zK8Gww6kyS;

	public bool RealLocation
	{
		get
		{
			return GetProperty(() => RealLocation);
		}
		set
		{
			SetProperty(() => RealLocation, value);
		}
	}

	public List<PrivewAniData> Items
	{
		[CompilerGenerated]
		get
		{
			return H7iG6mqOQP;
		}
		[CompilerGenerated]
		set
		{
			H7iG6mqOQP = value;
		}
	}

	public bool IsStart
	{
		get
		{
			return GetProperty(() => IsStart);
		}
		set
		{
			SetProperty(() => IsStart, value);
		}
	}

	public PrivewAniData Item
	{
		get
		{
			return GetProperty(() => Item);
		}
		set
		{
			SetProperty<PrivewAniData>(() => Item, value);
		}
	}

	[SpecialName]
	[CompilerGenerated]
	private CancellationTokenSource B3tGQcxvBV()
	{
		return zTaG1kA2NW;
	}

	[SpecialName]
	[CompilerGenerated]
	private void G2GGaIIop3(CancellationTokenSource P_0)
	{
		zTaG1kA2NW = P_0;
	}

	public WindowPreviewAniViewModel(IList<PrivewAniData> items)
	{
		zK8Gww6kyS = new AsyncLock();
		Items = new List<PrivewAniData>(items);
		RealLocation = true;
	}

	[Command]
	public void OnLoaded()
	{
		OnPlay(isStop: false);
	}

	[Command]
	public async void OnPlay(bool isStop)
	{
		if (isStop)
		{
			B3tGQcxvBV().Cancel();
		}
		else
		{
			if (Items == null || Items.Count <= 0)
			{
				return;
			}
			using (await zK8Gww6kyS.LockAsync())
			{
				while (IsStart)
				{
					if (!B3tGQcxvBV().IsCancellationRequested)
					{
						B3tGQcxvBV()?.Cancel();
					}
					await Task.Delay(10);
				}
				G2GGaIIop3(new CancellationTokenSource());
				await Task.Run((Action)MJGGxKWHlr);
			}
		}
	}

	private async void MJGGxKWHlr()
	{
		IsStart = true;
		try
		{
			while (B3tGQcxvBV() != null && !B3tGQcxvBV().IsCancellationRequested)
			{
				if (Items == null)
				{
					B3tGQcxvBV().Cancel();
				}
				else
				{
					if (Items == null)
					{
						continue;
					}
					foreach (PrivewAniData item in Items)
					{
						PrivewAniData privewAniData = (Item = item);
						if (privewAniData.Delay < 10)
						{
							await Task.Delay(20);
						}
						else
						{
							await Task.Delay(privewAniData.Delay);
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorPreviewViewModelAni.XL");
		}
		IsStart = false;
	}

	public void Dispose()
	{
	}
}
