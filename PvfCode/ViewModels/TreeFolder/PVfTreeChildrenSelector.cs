using System;
using System.Collections;
using DevExpress.Xpf.Grid;

namespace PvfCode.ViewModels.TreeFolder;

public class PVfTreeChildrenSelector : IChildNodesSelector
{
	private Func<object, IEnumerable> QKIFkgI9KV;

	public PVfTreeChildrenSelector(Func<object, IEnumerable> selector)
	{
		QKIFkgI9KV = selector;
	}

	IEnumerable IChildNodesSelector.SelectChildren(object item)
	{
		return QKIFkgI9KV(item);
	}
}
