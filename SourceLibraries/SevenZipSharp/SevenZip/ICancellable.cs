namespace SevenZip;

public interface ICancellable
{
	bool Cancel { get; set; }

	bool Skip { get; set; }
}
