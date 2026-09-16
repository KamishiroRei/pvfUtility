using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm;
using PvfCode.NPK.Utils;
using PvfCode.NPK.Utils.Models;
using PvfCode.Services;
using PvfCode.Services.PreviewPvfFileFolder.Xui;

namespace PvfCode.ViewModels.DocumentFolder.PreviewControls.Xui;

/// <summary>
/// XUI 布局预览的状态容器：持有解析结果、渲染开关和按 NPK 分组加载的帧位图缓存。
/// 取图沿 AniFileGroup 的既有模式：按 NPK 分组各 ReadNpk 一次，帧 ImageSource 在 UI 线程解码（ImgFile 内部冻结）。
/// </summary>
public sealed class XuiPreviewViewModel : ViewModelBase
{
	private readonly Dictionary<(string packKey, int index), ImageSource> frameCache = new();
	private readonly Dictionary<(string packKey, int index), Size> frameSizeCache = new();
	private int loadGeneration;

	public XuiLayoutDocument Layout
	{
		get { return GetProperty(() => Layout); }
		private set { SetProperty(() => Layout, value); }
	}

	/// <summary>画布缩放倍率（设计画布 800×600）。</summary>
	public double Zoom
	{
		get { return GetProperty(() => Zoom); }
		set { SetProperty(() => Zoom, value); }
	}

	public bool ShowHidden
	{
		get { return GetProperty(() => ShowHidden); }
		set { SetProperty(() => ShowHidden, value); }
	}

	public bool ShowNoPos
	{
		get { return GetProperty(() => ShowNoPos); }
		set { SetProperty(() => ShowNoPos, value); }
	}

	public bool ShowPlaceholders
	{
		get { return GetProperty(() => ShowPlaceholders); }
		set { SetProperty(() => ShowPlaceholders, value); }
	}

	public string Status
	{
		get { return GetProperty(() => Status); }
		private set { SetProperty(() => Status, value); }
	}

	/// <summary>帧数据或开关变化后递增，视图据此重建画布。</summary>
	public int CanvasRevision
	{
		get { return GetProperty(() => CanvasRevision); }
		private set { SetProperty(() => CanvasRevision, value); }
	}

	public XuiPreviewViewModel()
	{
		Zoom = 1.0;
		ShowPlaceholders = true;
	}

	public bool TryGetFrame(string imagePack, int index, out ImageSource source, out Size frameSize)
	{
		source = null;
		frameSize = default;
		if (string.IsNullOrEmpty(imagePack) || index < 0)
		{
			return false;
		}
		string packKey = "sprite/" + imagePack.ToLowerInvariant();
		if (!frameCache.TryGetValue((packKey, index), out source))
		{
			return false;
		}
		frameSizeCache.TryGetValue((packKey, index), out frameSize);
		return source != null;
	}

	/// <summary>刷新解析结果并重新加载帧数据。已解码帧缓存按文档版本整体重建。</summary>
	public void UpdateLayout(XuiLayoutDocument document)
	{
		Layout = document;
		frameCache.Clear();
		frameSizeCache.Clear();
		CanvasRevision++;
		BeginLoadFrames(document);
	}

	private void BeginLoadFrames(XuiLayoutDocument document)
	{
		if (document == null || document.AllControls.Count == 0)
		{
			return;
		}
		if (ImagePack2Service.Instance.Count == 0)
		{
			Status = "未载入 ImagePacks2（NPK 资源），当前显示线框布局；载入后重新打开可显示真实帧。";
			return;
		}
		List<(string Pack, int Index)> needs = document.AllControls
			.Where(control => control.ImagePack != null && control.ImageIndex.HasValue && control.ImageIndex.Value >= 0)
			.Select(control => (control.ImagePack, control.ImageIndex.Value))
			.Distinct()
			.ToList();
		if (needs.Count == 0)
		{
			Status = null;
			return;
		}
		int generation = ++loadGeneration;
		Status = $"正在加载 {needs.Count} 个图集帧引用...";
		Task.Run(() =>
		{
			Dictionary<string, List<(string PackKey, int Index)>> byNpk = new(StringComparer.OrdinalIgnoreCase);
			int missingReference = 0;
			foreach ((string pack, int index) in needs)
			{
				string packKey = "sprite/" + pack.ToLowerInvariant();
				if (!ImagePack2Service.Instance.NpkImgDIC.TryGetValue(packKey, out UtImgFile imgFile) || index >= imgFile.Count)
				{
					missingReference++;
					continue;
				}
				string npkFilePath = ImagePack2Service.Instance.GetNpkFilePath(imgFile);
				if (!byNpk.TryGetValue(npkFilePath, out List<(string, int)> list))
				{
					byNpk[npkFilePath] = list = new List<(string, int)>();
				}
				list.Add((packKey, index));
			}
			List<(string PackKey, int Index, ImgFile Img)> found = new();
			foreach (KeyValuePair<string, List<(string PackKey, int Index)>> pair in byNpk)
			{
				List<ImagePack> imagePacks;
				try
				{
					imagePacks = File.Exists(pair.Key) ? NpkCoder.ReadNpk(pair.Key) : new List<ImagePack>();
				}
				catch
				{
					missingReference += pair.Value.Count;
					continue;
				}
				foreach ((string packKey, int index) in pair.Value)
				{
					ImagePack imagePack = imagePacks.FirstOrDefault(item => string.Equals(item.Path, packKey, StringComparison.OrdinalIgnoreCase));
					if (imagePack?.ImgList != null && index < imagePack.ImgList.Count)
					{
						found.Add((packKey, index, imagePack.ImgList[index]));
					}
					else
					{
						missingReference++;
					}
				}
			}
			return (Found: found, Missing: missingReference);
		}).ContinueWith(task =>
		{
			if (generation != loadGeneration)
			{
				return;
			}
			if (task.IsFaulted || task.Result.Found.Count == 0)
			{
				Status = task.IsFaulted ? "NPK 帧加载失败，当前显示线框布局。" : "图集帧引用均未命中已载入的 NPK，当前显示线框布局。";
				return;
			}
			foreach ((string packKey, int index, ImgFile img) in task.Result.Found)
			{
				try
				{
					ImageSource source = img.ImageSource;
					frameCache[(packKey, index)] = source;
					frameSizeCache[(packKey, index)] = new Size(img.Size.Width, img.Size.Height);
				}
				catch
				{
				}
			}
			Status = $"已加载 {task.Result.Found.Count} 帧；{task.Result.Missing} 个引用缺失或未命中。";
			CanvasRevision++;
		}, TaskScheduler.FromCurrentSynchronizationContext());
	}
}
