
namespace PvfCode.ViewModels.independent_drop.DropList;

public class ListItemSelect : ListItem
{
	public override int? ItemCode => GetItemCode();

	public override string ItemName => GetItemName();

	public ListItemSelect()
	{
	}
}
