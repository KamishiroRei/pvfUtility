using System.Windows.Media;
using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode.Models.Options.AniDesigner;

[JsonObject(MemberSerialization.OptOut)]
public class AniDesignerConfig : ViewModelBase
{
	private Color? HQiZetAFvn;

	private Color? RKeZjSXuDx;

	public Color PreviewBackBrush
	{
		get
		{
			if (!HQiZetAFvn.HasValue)
			{
				HQiZetAFvn = Colors.Gray;
			}
			return HQiZetAFvn.Value;
		}
		set
		{
			HQiZetAFvn = value;
			RaisePropertyChanged("PreviewBackBrush");
		}
	}

	public Color ImageBorderBrush
	{
		get
		{
			if (!RKeZjSXuDx.HasValue)
			{
				RKeZjSXuDx = Colors.Pink;
			}
			return RKeZjSXuDx.Value;
		}
		set
		{
			RKeZjSXuDx = value;
			RaisePropertyChanged("ImageBorderBrush");
		}
	}

	public AniDesignerConfig()
	{
	}
}
