using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.Native;

namespace PvfCode.ViewModels.TreeFolder;

public class CustomTreeListView : TreeListView
{
	protected override SelectionStrategyBase CreateSelectionStrategy()
	{
		return new CustomTreeListSelectionStrategyRow(this);
	}

	internal void sHLFPm5Lc8(bool P_0)
	{
		base.EditorSetInactiveAfterClick = P_0;
	}

	public CustomTreeListView()
	{
	}
}
