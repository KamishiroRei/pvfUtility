using System.Collections.Generic;
using System.Windows.Media;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class FileItemIconPreviewViewModel
{
	public readonly PvfFile File;

	private readonly PvfGroup pvf;

	public ImageSource? ImageSource
	{
		get
		{
			ImagePack2Service.Instance.TreeGetIcon(pvf, File, out ImageSource imageSource);
			return imageSource;
		}
	}

	public int ItemCount { get; set; }

	public FilePreviewDataBase PreviewBase => FilePreviewDataBase.Create(pvf, File, ImageSource);

	public FileItemIconPreviewViewModel(PvfGroup pvf, PvfFile file, int itemCount)
	{
		this.pvf = pvf;
		File = file;
		ItemCount = itemCount;
	}

	public static List<FileItemIconPreviewViewModel> Create(List<KeyValuePair<int, PvfFile>> files, PvfGroup pvf)
	{
		List<FileItemIconPreviewViewModel> previews = new List<FileItemIconPreviewViewModel>();
		if (files == null)
		{
			return previews;
		}
		files.ForEach(file =>
		{
			previews.Add(new FileItemIconPreviewViewModel(pvf, file.Value, file.Key));
		});
		return previews;
	}
}
