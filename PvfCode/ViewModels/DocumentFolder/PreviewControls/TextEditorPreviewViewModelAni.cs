using System;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using Nito.AsyncEx;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.ViewModels.DocumentFolder.PreviewControls.Ani;

namespace PvfCode.ViewModels.DocumentFolder.PreviewControls;

public class TextEditorPreviewViewModelAni : TextEditorPreviewViewModelBase
{
	private AniFileGroup PreviewAniFileGroup { get; set; }

	private CancellationTokenSource PlaybackCancellationTokenSource { get; set; }

	private readonly AsyncLock playbackLock;

	private bool playbackStopped;

	public bool ReplaceSpecialCharacters
	{
		get
		{
			return GetProperty(() => ReplaceSpecialCharacters);
		}
		set
		{
			SetProperty(() => ReplaceSpecialCharacters, value);
		}
	}

	public FRAMEModel Item
	{
		get
		{
			return GetProperty(() => Item);
		}
		set
		{
			SetProperty<FRAMEModel>(() => Item, value);
		}
	}

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

	public TextEditorPreviewViewModelAni(TextEditorBase textEditorBase, PvfFile file)
		: base(textEditorBase, file)
	{
		playbackLock = new AsyncLock();
		playbackStopped = true;
		PlaybackCancellationTokenSource = new CancellationTokenSource();
	}

	public override void LoadData(string fileText)
	{
		try
		{
			if (AppSetting.Instance.EditConfig.ShowAniPreviewPanel)
			{
				LoadAniData(fileText);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorPreviewViewModelAni.LoadData");
		}
	}

	public void LoadDataForPreviewDocument(string fileText)
	{
		try
		{
			LoadAniData(fileText);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorPreviewViewModelAni.LoadDataForPreviewDocument");
		}
	}

	private async void LoadAniData(string fileText)
	{
		base.IsLoading = true;
		PreviewAniFileGroup = new AniFileGroup();
		if (ImagePack2Service.Instance.Count == 0)
		{
			AppCore.Logger.Warning("要预览ani请载入ImagePack2模型补丁 关闭ani预览就不会再看到此提示");
		}
		else if (await Task.Run(() => PreviewAniFileGroup.LoadData(File, fileText)))
		{
			StartPlayback();
			base.IsLoading = false;
		}
	}

	private async void StartPlayback()
	{
		try
		{
			using (await playbackLock.LockAsync())
			{
				while (!playbackStopped)
				{
					if (!PlaybackCancellationTokenSource.IsCancellationRequested)
					{
						PlaybackCancellationTokenSource?.Cancel();
					}
					await Task.Delay(10);
				}
				PlaybackCancellationTokenSource = new CancellationTokenSource();
				await Task.Run(() => PlayFramesAsync(), PlaybackCancellationTokenSource.Token);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorPreviewViewModelAni.LoadToken");
		}
	}

	private async Task PlayFramesAsync()
	{
		playbackStopped = false;
		try
		{
			while (PlaybackCancellationTokenSource != null && !PlaybackCancellationTokenSource.IsCancellationRequested)
			{
				if (PreviewAniFileGroup == null)
				{
					PlaybackCancellationTokenSource?.Cancel();
				}
				else
				{
					if (PreviewAniFileGroup == null || PreviewAniFileGroup.AniFileData == null)
					{
						continue;
					}
					FRAMEModel[] frameSnapshot = PreviewAniFileGroup.AniFileData.Items.ToArray();
					foreach (FRAMEModel frame in frameSnapshot)
					{
						if (PlaybackCancellationTokenSource == null || PlaybackCancellationTokenSource.IsCancellationRequested)
						{
							break;
						}
						Item = frame;
						if (frame.DELAY < 20)
						{
							await Task.Delay(20);
						}
						else if (frame.DELAY > 5000)
						{
							await Task.Delay(5000);
						}
						else
						{
							await Task.Delay(frame.DELAY);
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorPreviewViewModelAni.XL");
		}
		playbackStopped = true;
	}

	[Command]
	public void OnOpenPreviewAniWindow()
	{
	}

	public override void Dispose()
	{
		Item = null;
		PreviewAniFileGroup?.Dispose();
		PreviewAniFileGroup = null;
		PlaybackCancellationTokenSource?.Cancel();
		Editor = null;
		PlaybackCancellationTokenSource = null;
		File = null;
	}

	public override void Refresh(string fileText)
	{
		base.IsLoaded = false;
		LoadData(fileText);
	}

	public override void Uninstall()
	{
		PlaybackCancellationTokenSource?.Cancel();
	}
}
