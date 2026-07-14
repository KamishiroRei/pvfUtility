using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Nito.AsyncEx;
using PvfCode.Services.PvfParsingNew.EditorPrivew;

namespace PvfCode.ViewModels.DocumentFolder.AniNpkLineElement;

public class WindowPreviewAniViewModel : ViewModelBase, IDisposable
{
	private CancellationTokenSource cancellationTokenSource;

	private readonly AsyncLock playLock;

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

	public List<PrivewAniData> Items { get; set; }

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

	public WindowPreviewAniViewModel(IList<PrivewAniData> items)
	{
		playLock = new AsyncLock();
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
			cancellationTokenSource.Cancel();
		}
		else
		{
			if (Items == null || Items.Count <= 0)
			{
				return;
			}
			using (await playLock.LockAsync())
			{
				while (IsStart)
				{
					if (!cancellationTokenSource.IsCancellationRequested)
					{
						cancellationTokenSource?.Cancel();
					}
					await Task.Delay(10);
				}
				cancellationTokenSource = new CancellationTokenSource();
				await Task.Run((Action)PlayAnimation);
			}
		}
	}

	private async void PlayAnimation()
	{
		IsStart = true;
		try
		{
			while (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
			{
				if (Items == null)
				{
					cancellationTokenSource.Cancel();
				}
				else
				{
					if (Items == null)
					{
						continue;
					}
					foreach (PrivewAniData item in Items)
					{
						PrivewAniData currentItem = (Item = item);
						if (currentItem.Delay < 10)
						{
							await Task.Delay(20);
						}
						else
						{
							await Task.Delay(currentItem.Delay);
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
