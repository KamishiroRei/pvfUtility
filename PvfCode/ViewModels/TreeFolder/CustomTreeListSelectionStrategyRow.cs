using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;

namespace PvfCode.ViewModels.TreeFolder;

public class CustomTreeListSelectionStrategyRow : TreeListSelectionStrategyRow
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass4_0
	{
		public CustomTreeListSelectionStrategyRow eRBL5bp6Fy;

		public TreeListViewHitInfo K16LSZm0D8;

		public _003C_003Ec__DisplayClass4_0()
		{
		}

		internal void BWHLfpAf24()
		{
			eRBL5bp6Fy.InvertRowSelection(K16LSZm0D8.RowHandle);
		}
	}

	private Action C2QFJqbfms;

	private CustomTreeListView View => view as CustomTreeListView;

	public CustomTreeListSelectionStrategyRow(CustomTreeListView view)
		: base(view)
	{
	}

	public override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
	{
		_003C_003Ec__DisplayClass4_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass4_0();
		CS_0024_003C_003E8__locals7.eRBL5bp6Fy = this;
		base.OnMouseLeftButtonUp(e);
		CS_0024_003C_003E8__locals7.K16LSZm0D8 = View.CalcHitInfo(e.GetPosition(View));
		if (base.IsShiftPressed || !base.IsControlPressed || Mouse.RightButton == MouseButtonState.Pressed)
		{
			return;
		}
		SetSelectionAnchorRowHandle(CS_0024_003C_003E8__locals7.K16LSZm0D8.RowHandle);
		View.sHLFPm5Lc8(true);
		if (IsRowSelected(CS_0024_003C_003E8__locals7.K16LSZm0D8.RowHandle) && !base.IsControlPressed && !base.IsShiftPressed)
		{
			C2QFJqbfms = delegate
			{
				CS_0024_003C_003E8__locals7.eRBL5bp6Fy.InvertRowSelection(CS_0024_003C_003E8__locals7.K16LSZm0D8.RowHandle);
			};
		}
		else
		{
			InvertRowSelection(CS_0024_003C_003E8__locals7.K16LSZm0D8.RowHandle);
		}
	}

	protected override void OnAfterMouseLeftButtonDownCore(IDataViewHitInfo hitInfo)
	{
		if (!base.IsShiftPressed && base.IsControlPressed && Mouse.RightButton != MouseButtonState.Pressed)
		{
			View.sHLFPm5Lc8(true);
		}
		else
		{
			base.OnAfterMouseLeftButtonDownCore(hitInfo);
		}
	}
}
