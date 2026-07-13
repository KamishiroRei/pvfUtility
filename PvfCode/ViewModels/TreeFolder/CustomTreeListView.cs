using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.Native;

namespace PvfCode.ViewModels.TreeFolder;

public class CustomTreeListView : TreeListView
{
	protected override SelectionStrategyBase CreateSelectionStrategy()
	{
		return new CustomTreeListSelectionStrategyRow(this);
	}

	internal void SetEditorInactiveAfterClick(bool value)
	{
		base.EditorSetInactiveAfterClick = value;
	}

	public CustomTreeListView()
	{
	}
}
