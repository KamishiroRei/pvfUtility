using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Docking;

namespace PvfCode.MVVMServices;

public interface IDockLayoutManagerService
{
	void Float(object panelViewModel);

	FloatGroup AddFloatPanel(Control userControl, string caption);

	void SetFloatPanelCenter(FloatGroup floatPanel);

	void SetFloatPanelAutoHeight(FloatGroup floatPanel, SizeToContent sizeToContent);

	void SetFloatPanelAutoHeight(object panelViewModel, SizeToContent sizeToContent);

	void ClosePanel(object panelViewModel);

	void ShowContextMenu(object panelViewModel);

	bool SplitRight(object panelViewModel);
}
