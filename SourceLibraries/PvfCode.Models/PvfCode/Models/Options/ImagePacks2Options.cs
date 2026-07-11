using DevExpress.Mvvm;
using Newtonsoft.Json;

namespace PvfCode.Models.Options;

[JsonObject(MemberSerialization.OptOut)]
public class ImagePacks2Options : ViewModelBase
{
	public string ImagePacks2Path
	{
		get
		{
			return GetProperty(() => ImagePacks2Path);
		}
		set
		{
			SetProperty<string>(() => ImagePacks2Path, value);
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
