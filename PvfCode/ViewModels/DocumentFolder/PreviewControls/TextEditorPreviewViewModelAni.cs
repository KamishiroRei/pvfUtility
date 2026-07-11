using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm.DataAnnotations;
using Nito.AsyncEx;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.ViewModels.DocumentFolder.PreviewControls.Ani;

namespace PvfCode.ViewModels.DocumentFolder.PreviewControls;

public class TextEditorPreviewViewModelAni : TextEditorPreviewViewModelBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass19_0
	{
		public TextEditorPreviewViewModelAni xDeeZouEUd;

		public string pyqeJ0sSZO;

		public _003C_003Ec__DisplayClass19_0()
		{
		}

		internal Task<bool>? jTxePNhlMt()
		{
			return xDeeZouEUd.g2E4T5M3of().LoadData(xDeeZouEUd.File, pyqeJ0sSZO);
		}
	}

	[CompilerGenerated]
	private AniFileGroup quW4FjPV8S;

	[CompilerGenerated]
	private CancellationTokenSource Uau4r6Rxv8;

	private readonly AsyncLock fCH4WrJSOB;

	private bool Nux4mnGGmh;

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

	[SpecialName]
	[CompilerGenerated]
	private AniFileGroup g2E4T5M3of()
	{
		return quW4FjPV8S;
	}

	[SpecialName]
	[CompilerGenerated]
	private void ghl4CKd20n(AniFileGroup P_0)
	{
		quW4FjPV8S = P_0;
	}

	[SpecialName]
	[CompilerGenerated]
	private CancellationTokenSource jVE4hmShHR()
	{
		return Uau4r6Rxv8;
	}

	[SpecialName]
	[CompilerGenerated]
	private void xg54vXIiNW(CancellationTokenSource P_0)
	{
		Uau4r6Rxv8 = P_0;
	}

	public TextEditorPreviewViewModelAni(TextEditorBase textEditorBase, PvfFile file)
		: base(textEditorBase, file)
	{
		fCH4WrJSOB = new AsyncLock();
		Nux4mnGGmh = true;
		xg54vXIiNW(new CancellationTokenSource());
	}

	public override void LoadData(string fileText)
	{
		try
		{
			if (AppSetting.Instance.EditConfig.ShowAniPreviewPanel)
			{
				LFKAzTqTFU(fileText);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorPreviewViewModelAni.LoadData");
		}
	}

	private async void LFKAzTqTFU(string P_0)
	{
		_003C_003Ec__DisplayClass19_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass19_0();
		CS_0024_003C_003E8__locals5.xDeeZouEUd = this;
		CS_0024_003C_003E8__locals5.pyqeJ0sSZO = P_0;
		base.IsLoading = true;
		ghl4CKd20n(new AniFileGroup());
		if (ImagePack2Service.Instance.Count == 0)
		{
			AppCore.Logger.Warning("要预览ani请载入ImagePack2模型补丁 关闭ani预览就不会再看到此提示");
		}
		else if (await Task.Run(() => CS_0024_003C_003E8__locals5.xDeeZouEUd.g2E4T5M3of().LoadData(CS_0024_003C_003E8__locals5.xDeeZouEUd.File, CS_0024_003C_003E8__locals5.pyqeJ0sSZO)))
		{
			vhB4DuAlrE();
			base.IsLoading = false;
		}
	}

	private async void vhB4DuAlrE()
	{
		try
		{
			using (await fCH4WrJSOB.LockAsync())
			{
				while (!Nux4mnGGmh)
				{
					if (!jVE4hmShHR().IsCancellationRequested)
					{
						jVE4hmShHR()?.Cancel();
					}
					await Task.Delay(10);
				}
				xg54vXIiNW(new CancellationTokenSource());
				await Task.Run(() => NLq4lqHdiK(), jVE4hmShHR().Token);
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorPreviewViewModelAni.LoadToken");
		}
	}

	private async Task NLq4lqHdiK()
	{
		Nux4mnGGmh = false;
		try
		{
			while (jVE4hmShHR() != null && !jVE4hmShHR().IsCancellationRequested)
			{
				if (g2E4T5M3of() == null)
				{
					jVE4hmShHR()?.Cancel();
				}
				else
				{
					if (g2E4T5M3of() == null || g2E4T5M3of().AniFileData == null)
					{
						continue;
					}
					FRAMEModel[] array = g2E4T5M3of().AniFileData.Items.ToArray();
					foreach (FRAMEModel fRAMEModel in array)
					{
						if (jVE4hmShHR() == null || jVE4hmShHR().IsCancellationRequested)
						{
							break;
						}
						Item = fRAMEModel;
						if (fRAMEModel.DELAY < 20)
						{
							await Task.Delay(20);
						}
						else if (fRAMEModel.DELAY > 5000)
						{
							await Task.Delay(5000);
						}
						else
						{
							await Task.Delay(fRAMEModel.DELAY);
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "TextEditorPreviewViewModelAni.XL");
		}
		Nux4mnGGmh = true;
	}

	[Command]
	public void OnOpenPreviewAniWindow()
	{
	}

	public override void Dispose()
	{
		Item = null;
		g2E4T5M3of()?.Dispose();
		ghl4CKd20n(null);
		jVE4hmShHR()?.Cancel();
		Editor = null;
		xg54vXIiNW(null);
		File = null;
	}

	public override void Refresh(string fileText)
	{
		base.IsLoaded = false;
		LoadData(fileText);
	}

	public override void Uninstall()
	{
		jVE4hmShHR()?.Cancel();
	}

	[CompilerGenerated]
	private Task? Xrl4jk8ngq()
	{
		return NLq4lqHdiK();
	}
}
