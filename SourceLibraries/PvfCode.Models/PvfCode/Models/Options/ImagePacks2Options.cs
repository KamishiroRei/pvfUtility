using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class ImagePacks2Options : ViewModelBase
{
	private string _imagePacks2Path = string.Empty;

	public string ImagePacks2Path
	{
		get => _imagePacks2Path;
		set
		{
			_imagePacks2Path = value;
			RaisePropertyChanged(nameof(ImagePacks2Path));
		}
	}

	[JsonIgnore]
	public bool IsLoaded
	{
		get
		{
			return GetProperty(() => IsLoaded);
		}
		set
		{
			SetProperty(() => IsLoaded, value);
		}
	}

	public int ImgCount
	{
		get
		{
			return GetProperty(() => ImgCount);
		}
		set
		{
			SetProperty(() => ImgCount, value, HKoLzD3Hw4);
		}
	}

	private void HKoLzD3Hw4()
	{
		_ = ImgCount;
	}

	public ImagePacks2Options()
	{
	}
}
