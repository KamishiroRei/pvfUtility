namespace PvfCode.ViewModels.SearchPvf.SearchName;

public interface IItemNameSearchResultBase
{
	int ItemCode { get; }

	string ItemName { get; }

	string FilePath { get; set; }

	int? GetItemCode();

	string GetItemName();
}
