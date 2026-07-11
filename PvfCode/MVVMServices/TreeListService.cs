using System;
using System.Linq;
using System.Windows.Threading;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Grid;

namespace PvfCode.MVVMServices;

public class TreeListService : ServiceBase, ITreeListService
{
	private TreeListControl TreeList => (TreeListControl)base.AssociatedObject;

	public TreeListNode ContentToNode(object row)
	{
		return TreeList.View.GetNodeByContent(row);
	}

	public void ExpandAllNodes()
	{
		TreeList.View.ExpandAllNodes();
	}

	public TreeListNode? FirstOrDefaultNode()
	{
		TreeListNode node = null;
		((DispatcherObject)this).Dispatcher.Invoke((Action)delegate
		{
			node = TreeList.View.Nodes?.FirstOrDefault();
		});
		return node;
	}

	public void SetFocusableNode(TreeListNode node)
	{
		if (TreeList.View != null)
		{
			TreeList.UnselectAll();
			TreeList.View.FocusedNode = node;
		}
	}

	public void UnselectAll()
	{
		TreeList.UnselectAll();
	}
}
