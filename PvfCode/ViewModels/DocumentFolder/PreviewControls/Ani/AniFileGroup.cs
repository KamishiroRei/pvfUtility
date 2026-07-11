using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm;
using PvfCode.NPK.Utils;
using PvfCode.NPK.Utils.Models;
using PvfCode.Services;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.DocumentFolder.PreviewControls.Ani;

public class AniFileGroup : ViewModelBase, IDisposable
{
	public string FilePath
	{
		get
		{
			return GetProperty(() => FilePath);
		}
		set
		{
			SetProperty<string>(() => FilePath, value);
		}
	}

	public AniFile AniFileData { get; set; }

	public bool ImgLoadSuccess { get; set; }

	public Task<bool> LoadData(PvfFile file, string fileText, bool isDesigner = false)
	{
		if (file == null)
		{
			AppCore.Logger.Error("加载ani数据 文件不能为空！");
			return Task.FromResult(result: false);
		}
		FilePath = file.FileName;
		if (!BinaryAniCompiler.FileTextConvertAniFile(fileText, file.FileName, out AniFile anifile))
		{
			AppCore.Logger.Error("ani模型数据加载失败 详情查看错误窗口");
			return Task.FromResult(result: false);
		}
		AniFileData = anifile;
		LoadImagePack(isDesigner);
		return Task.FromResult(result: true);
	}

	public Task<bool> LoadData(PvfFile file, bool isDesigner = false)
	{
		if (!BinaryAniCompiler.DecompileAniToModel(file, out AniFile anifile))
		{
			AppCore.Logger.Error("ani模型数据加载失败 详情查看错误窗口");
			return Task.FromResult(result: false);
		}
		AniFileData = anifile;
		LoadImagePack(isDesigner);
		return Task.FromResult(result: true);
	}

	public Task LoadImagePack(bool isDesigner)
	{
		if (AniFileData == null)
		{
			return Task.CompletedTask;
		}
		if (ImagePack2Service.Instance.Count == 0)
		{
			return Task.CompletedTask;
		}
		Dictionary<string, List<ImagePack>> dictionary = new Dictionary<string, List<ImagePack>>();
		foreach (FRAMEModel frame in AniFileData.Items)
		{
			if (frame.Image == null || !ImagePack2Service.Instance.NpkImgDIC.TryGetValue(frame.Image.ImgFullPath, out UtImgFile value) || frame.Image.Index >= value.Count)
			{
				continue;
			}
			string npkFilePath = ImagePack2Service.Instance.GetNpkFilePath(value);
			if (!dictionary.TryGetValue(npkFilePath, out List<ImagePack> imagePacks))
			{
				if (File.Exists(npkFilePath))
				{
					imagePacks = NpkCoder.ReadNpk(npkFilePath);
				}
				else
				{
					imagePacks = new List<ImagePack>();
				}
				dictionary.Add(npkFilePath, imagePacks);
			}
			ConcurrentObservableCollection<ImgFile> images = imagePacks
				.FirstOrDefault(imagePack => imagePack.Path == frame.Image.ImgFullPath)?.ImgList;
			if (images != null && frame.Image.Index < images.Count)
			{
				if (!ImgLoadSuccess)
				{
					ImgLoadSuccess = true;
				}
				frame.Image.ImgFile = images[frame.Image.Index];
			}
		}
		foreach (FRAMEModel frame in AniFileData.Items)
		{
			((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)frame.SetImageSourceEffect);
		}
		return Task.CompletedTask;
	}

	public void Dispose()
	{
		AniFileData = null;
	}
}
