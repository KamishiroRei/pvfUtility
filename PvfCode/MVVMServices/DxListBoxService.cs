using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Editors;

namespace PvfCode.MVVMServices;

public class DxListBoxService : ServiceBase, IDxListBoxService
{
	private ListBoxEdit ListBox => (ListBoxEdit)base.AssociatedObject;

	public void SetSelectedIndex(int index)
	{
		ListBox.SelectedIndex = index;
	}

	public DxListBoxService()
	{
	}
}
