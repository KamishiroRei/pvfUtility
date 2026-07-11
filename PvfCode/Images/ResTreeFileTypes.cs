using System.Windows;
using System.Windows.Media;

namespace PvfCode.Images;

public class ResTreeFileTypes
{
	private ImageSource CFravqpmtx;

	private ImageSource DrIaBx3RNJ;

	private ImageSource uyHaFTJNfE;

	private ImageSource vSnary2GpW;

	private ImageSource Rp3aW1yj6j;

	private ImageSource Obcamx5atq;

	private ImageSource WOBa2Th4MI;

	public ImageSource FolderClosed
	{
		get
		{
			if (CFravqpmtx == null)
			{
				CFravqpmtx = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/treefiletypes/folderclosed.svg");
				((Freezable)CFravqpmtx).Freeze();
			}
			return CFravqpmtx;
		}
	}

	public ImageSource FolderOpened
	{
		get
		{
			if (DrIaBx3RNJ == null)
			{
				DrIaBx3RNJ = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/treefiletypes/folderopened.svg");
				((Freezable)DrIaBx3RNJ).Freeze();
			}
			return DrIaBx3RNJ;
		}
	}

	public ImageSource Lst
	{
		get
		{
			if (uyHaFTJNfE == null)
			{
				uyHaFTJNfE = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/treefiletypes/lst.svg");
				((Freezable)uyHaFTJNfE).Freeze();
			}
			return uyHaFTJNfE;
		}
	}

	public ImageSource Script_16x
	{
		get
		{
			if (vSnary2GpW == null)
			{
				vSnary2GpW = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/treefiletypes/script_16x.svg");
				((Freezable)vSnary2GpW).Freeze();
			}
			return vSnary2GpW;
		}
	}

	public ImageSource Str
	{
		get
		{
			if (Rp3aW1yj6j == null)
			{
				Rp3aW1yj6j = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/treefiletypes/str.svg");
				((Freezable)Rp3aW1yj6j).Freeze();
			}
			return Rp3aW1yj6j;
		}
	}

	public ImageSource Ani
	{
		get
		{
			if (Obcamx5atq == null)
			{
				Obcamx5atq = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/treefiletypes/ani.svg");
				((Freezable)Obcamx5atq).Freeze();
			}
			return Obcamx5atq;
		}
	}

	public ImageSource Ui
	{
		get
		{
			if (WOBa2Th4MI == null)
			{
				WOBa2Th4MI = Res.Instance.GetSvgImage("pack://application:,,,/pvfUtility;component/images/svgs/treefiletypes/ui.svg");
				((Freezable)WOBa2Th4MI).Freeze();
			}
			return WOBa2Th4MI;
		}
	}

	public ImageSource GetFileIcon(bool isfile, PvfFileType? fileType)
	{
		if (isfile)
		{
			return fileType switch
			{
				PvfFileType.lst => Lst, 
				PvfFileType.str => Str, 
				PvfFileType.ani => Ani, 
				PvfFileType.ui => Ui, 
				_ => Script_16x, 
			};
		}
		return FolderClosed;
	}

	public ResTreeFileTypes()
	{
	}
}
