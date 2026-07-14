using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Nito.AsyncEx;
using PvfCode.NPK.Utils.AniModel;
using PvfCode.NPK.Utils.AniModel.Enums;
using PvfCode.ViewModels.DocumentFolder.PreviewControls.Ani;
using PvfCode.Views.AniDesigner;

namespace PvfCode.ViewModels.AniDesigner;

public class WindowAniDesignerViewModel : ViewModelBase
{
	public class FrameProperData : IMetadataProvider<FRAMEModel>
	{
		public void BuildMetadata(MetadataBuilder<FRAMEModel> builder)
		{
			MetadataBuilder<FRAMEModel> metadataBuilder = builder.Property(x => x.Name).DisplayName("帧").Description("帧编号 禁止编辑")
				.EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder2 = metadataBuilder.Property(x => x.Index).Hidden().EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder3 = metadataBuilder2.Property(x => x.Image).DisplayName("贴图设定").EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder4 = metadataBuilder3.Property(x => x.LOOP).DisplayName("开启阴影").EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder5 = metadataBuilder4.Property(x => x.COORD).DisplayName("单色效果").EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder6 = metadataBuilder5.Property(x => x.PRELOAD).DisplayName("预加载").EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder7 = metadataBuilder6.Property(x => x.IMAGE_RATE).DisplayName("缩放效果").Description("这个词条不仅仅可以用来改变 贴图显示的大小还可以进行翻转等操作\r\n[IMAGE RATE]//缩放\r\n0.5 0.5//这样写就是 缩小一倍的意思放大也是类似写法\r\n -1 1.0 前面这个-1的话就是翻转的意思，比如想要一个人物贴图\r\n转向右边就可以使用这个词条！但是NPC的话一般可以在地图调用里用Left词条来控制左右朝向，这里只是举个栗子！ \r\n摘录自台吧 By:@gs13678")
				.EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder8 = metadataBuilder7.Property(x => x.IMAGE_ROTATE).DisplayName("旋转图像").Description("")
				.EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder9 = metadataBuilder8.Property(x => x.RGBA).DisplayName("染色").Description("染色效果 RGBA")
				.EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder10 = metadataBuilder9.Property(x => x.GRAPHIC_EFFECT).DisplayName("特效").Description("特效")
				.EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder11 = metadataBuilder10.Property(x => x.DELAY).DisplayName("延迟").Description("毫秒 1000=1/S")
				.EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder12 = metadataBuilder11.Property(x => x.DAMAGE_TYPE).DisplayName("DAMAGE_TYPE").Description("")
				.EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder13 = metadataBuilder12.Property(x => x.PLAY_SOUND).DisplayName("播放声音").Description("播放声音")
				.EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder14 = metadataBuilder13.Property(x => x.SET_FLAG).DisplayName("SET_FLAG").Description("SET_FLAG")
				.EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder15 = metadataBuilder14.Property(x => x.FLIP_TYPE).DisplayName("翻转").Description("图像翻转\r\nHORIZON=平行\r\nVERTICAL=垂直\r\nALL=全部？")
				.EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder16 = metadataBuilder15.Property(x => x.LOOP_START).DisplayName("循环开始").Description("")
				.EndProperty();
			MetadataBuilder<FRAMEModel> metadataBuilder17 = metadataBuilder16.Property(x => x.LOOP_END).DisplayName("循环结束").Description("")
				.EndProperty();
			metadataBuilder17.Property(x => x.CLIP).DisplayName("CLIP").Description("裁剪？")
				.EndProperty();
		}

		public FrameProperData()
		{
		}
	}

	private CancellationTokenSource PlaybackCancellationTokenSource { get; set; }

	private readonly AsyncLock playbackLock;

	public bool IsLoading
	{
		get
		{
			return GetProperty(() => IsLoading);
		}
		set
		{
			SetProperty(() => IsLoading, value);
		}
	}

	public string Title
	{
		get
		{
			if (IsOpen)
			{
				return "ani设计器  " + AniFileGroup.FilePath;
			}
			return "ani设计器";
		}
	}

	public bool IsOpen
	{
		get
		{
			return GetProperty(() => IsOpen);
		}
		set
		{
			SetProperty(() => IsOpen, value);
		}
	}

	public AniFileGroup AniFileGroup
	{
		get
		{
			return GetProperty(() => AniFileGroup);
		}
		set
		{
			SetProperty<AniFileGroup>(() => AniFileGroup, value);
			IsOpen = value != null;
		}
	}

	public FRAMEModel SelectedFrame
	{
		get
		{
			return GetProperty(() => SelectedFrame);
		}
		set
		{
			SetProperty<FRAMEModel>(() => SelectedFrame, value);
			FrameProper = new MetadataExtendedSource(value, MetadataLocator.Create().AddMetadata(typeof(FRAMEModel), typeof(FrameProperData)));
		}
	}

	public MetadataExtendedSource FrameProper
	{
		get
		{
			return GetProperty(() => FrameProper);
		}
		set
		{
			SetProperty<MetadataExtendedSource>(() => FrameProper, value);
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

	public bool IsPlay
	{
		get
		{
			return GetProperty(() => IsPlay);
		}
		set
		{
			SetProperty(() => IsPlay, value);
		}
	}

	public WindowAniDesignerViewModel()
	{
		playbackLock = new AsyncLock();
	}

	public void LoadAniFileGroup(AniFileGroup aniFileGroup)
	{
		AniFileGroup = aniFileGroup;
	}

	[Command]
	public async void OnOpen(Window win)
	{
		WinOpenAniFileDialog winOpenAniFileDialog = new WinOpenAniFileDialog
		{
			Owner = win,
			WindowStartupLocation = WindowStartupLocation.CenterOwner
		};
		if (winOpenAniFileDialog.ShowDialog().Value)
		{
			AniFileGroup aniFileGroup = new AniFileGroup();
			WinOpenAniFileDialogViewModel dialogViewModel = winOpenAniFileDialog.DataContext as WinOpenAniFileDialogViewModel;
			IsLoading = true;
			WindowLoading loading = AppCore.CreateLoading("载入中...", win);
			loading.Show();
			if (!(await Task.Run(() => aniFileGroup.LoadData(AppCore.ViewModelBase.PVF.GetFile(dialogViewModel.FilePath), isDesigner: true))))
			{
				IsLoading = false;
				loading.Close();
			}
			else
			{
				loading.Close();
				AniFileGroup = aniFileGroup;
				IsLoading = false;
			}
		}
	}

	[Command]
	public void OnSave()
	{
	}

	[Command]
	public void OnClose()
	{
		CancelPlayback();
		AniFileGroup = null;
	}

	[Command]
	public void OnPlay(bool isStop)
	{
		PlayStart(isStop);
	}

	[Command]
	public void ImageDyeingChanged()
	{
	}

	public async void PlayStart(bool isStop)
	{
		if (isStop)
		{
			PlaybackCancellationTokenSource.Cancel();
		}
		else
		{
			if (AniFileGroup == null || AniFileGroup.AniFileData.Items.Count <= 0)
			{
				return;
			}
			using (await playbackLock.LockAsync())
			{
				while (IsPlay)
				{
					if (!PlaybackCancellationTokenSource.IsCancellationRequested)
					{
						PlaybackCancellationTokenSource?.Cancel();
					}
					await Task.Delay(10);
				}
				PlaybackCancellationTokenSource = new CancellationTokenSource();
				await Task.Run((Action)PlayTask);
			}
		}
	}

	public async void PlayTask()
	{
		IsPlay = true;
		try
		{
			while (PlaybackCancellationTokenSource != null && !PlaybackCancellationTokenSource.IsCancellationRequested)
			{
				List<FRAMEModel> frames = AniFileGroup?.AniFileData?.Items;
				if (frames == null)
				{
					PlaybackCancellationTokenSource?.Cancel();
					continue;
				}
				FRAMEModel[] frameSnapshot = frames.ToArray();
				foreach (FRAMEModel frame in frameSnapshot)
				{
					if (PlaybackCancellationTokenSource.IsCancellationRequested)
					{
						break;
					}
					SelectedFrame = frame;
					if (frame.DELAY < 10)
					{
						await Task.Delay(20);
					}
					else if (frame.DELAY > 10000)
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
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorPreviewViewModelAni.XL");
		}
		IsPlay = false;
	}

	private void CancelPlayback()
	{
		PlaybackCancellationTokenSource?.Cancel();
	}
}
