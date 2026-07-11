using DevExpress.Xpf.Grid;

namespace PvfCode.MVVMServices;

public interface ITreeListService
{
	TreeListNode ContentToNode(object row);

	void ExpandAllNodes();

	void SetFocusableNode(TreeListNode node);

	TreeListNode? FirstOrDefaultNode();

	void UnselectAll();
}
