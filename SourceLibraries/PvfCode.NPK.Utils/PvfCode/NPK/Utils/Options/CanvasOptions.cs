using System.Windows.Media;
using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode.NPK.Utils.Options;

[JsonObject(MemberSerialization.OptOut)]
public class CanvasOptions : BindableBase
{
	private Color? _BackBrush;

	private Color? _ImageBorderBrush;

	public Color BackBrush
	{
		get
		{
			if (!_BackBrush.HasValue)
			{
				_BackBrush = Colors.Gray;
			}
			return _BackBrush.Value;
		}
		set
		{
			_BackBrush = value;
			RaisePropertyChanged("BackBrush");
		}
	}

	public Color ImageBorderBrush
	{
		get
		{
			if (!_ImageBorderBrush.HasValue)
			{
				_ImageBorderBrush = Colors.Pink;
			}
			return _ImageBorderBrush.Value;
		}
		set
		{
			_ImageBorderBrush = value;
			RaisePropertyChanged("ImageBorderBrush");
		}
	}
}
