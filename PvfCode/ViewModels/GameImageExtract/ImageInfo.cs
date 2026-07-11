using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.GameImageExtract;

public class ImageInfo
{
	public object Uni { get; set; }

	public string Name { get; set; }

	public object Value { get; set; }

	public object CurrentValue { get; set; }

	// The original compiled BAML binds this historical property name.
	public object Test
	{
		get => CurrentValue;
		set => CurrentValue = value;
	}

	public object AllValue { get; set; }

	public ConcurrentObservableCollection<uint> IndexItems { get; set; }

	public ImageInfo()
	{
		IndexItems = new ConcurrentObservableCollection<uint>();
	}
}
