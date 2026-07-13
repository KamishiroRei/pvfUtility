using System.Windows.Input;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;

namespace PvfCode.ViewModels.TreeFolder;

public class CustomTreeListSelectionStrategyRow : TreeListSelectionStrategyRow
{
	private CustomTreeListView View => view as CustomTreeListView;

	public CustomTreeListSelectionStrategyRow(CustomTreeListView view)
		: base(view)
	{
	}

	public override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
	{
		base.OnMouseLeftButtonUp(e);
		TreeListViewHitInfo hitInfo = View.CalcHitInfo(e.GetPosition(View));
		if (base.IsShiftPressed || !base.IsControlPressed || Mouse.RightButton == MouseButtonState.Pressed)
		{
			return;
		}
		SetSelectionAnchorRowHandle(hitInfo.RowHandle);
		View.SetEditorInactiveAfterClick(true);
		InvertRowSelection(hitInfo.RowHandle);
	}

	protected override void OnAfterMouseLeftButtonDownCore(IDataViewHitInfo hitInfo)
	{
		if (!base.IsShiftPressed && base.IsControlPressed && Mouse.RightButton != MouseButtonState.Pressed)
		{
			View.SetEditorInactiveAfterClick(true);
		}
		else
		{
			base.OnAfterMouseLeftButtonDownCore(hitInfo);
		}
	}
}
