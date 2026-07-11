using PvfCode.Dot.Desktop.Enums;

namespace PvfCode.Dot.Desktop;

public class GetStoreListQueryData : ModelBase
{
	public delegate void StoreTypeChangedDelegate(StoreType storeType);

	private string _Keyword;

	private StoreType _Type;

	public string Keyword
	{
		get
		{
			return _Keyword;
		}
		set
		{
			_Keyword = value;
			DoNotify("Keyword");
		}
	}

	public StoreType Type
	{
		get
		{
			return _Type;
		}
		set
		{
			_Type = value;
			DoNotify("Type");
			this.StoreTypeChanged?.Invoke(value);
		}
	}

	public event StoreTypeChangedDelegate StoreTypeChanged;
}
