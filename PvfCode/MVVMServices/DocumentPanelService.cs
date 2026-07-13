using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Docking;
using PvfCode.MVVMServices;

namespace GKcC3iQxbUOdt2roQPY;

internal class Kq4YIkQG1jekqKdxajG : ServiceBase, IDcoumentPanelService
{
	private DocumentPanel Panel => (DocumentPanel)base.AssociatedObject;

	public void Test()
	{
		_ = Panel.IsSelectedItem;
		_ = Panel;
	}

	public Kq4YIkQG1jekqKdxajG()
	{
	}
}
