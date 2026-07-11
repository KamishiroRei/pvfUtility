using System.ComponentModel;
using System.Runtime.CompilerServices;
using DevExpress.Utils.About;

namespace PvfCode.Compatibility;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class DevExpressTrialInitializer
{
	[ModuleInitializer]
	internal static void Initialize()
	{
		DXLicenseProvider.SetTrial("T133943904000000000", "DevExpress.Xpf.Docking.DockLayoutManager, DevExpress.Xpf.Docking.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", "DevExpress.Xpf.Editors.TextEdit, DevExpress.Xpf.Core.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", "DevExpress.Xpf.Editors.BaseEdit, DevExpress.Xpf.Core.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", "DevExpress.Xpf.Grid.TreeListControl, DevExpress.Xpf.Grid.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", "DevExpress.Xpf.Editors.ButtonEdit, DevExpress.Xpf.Core.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", "DevExpress.Xpf.Editors.AutoSuggestEdit, DevExpress.Xpf.Core.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", "DevExpress.Xpf.Editors.ColorEdit, DevExpress.Xpf.Core.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", "DevExpress.Xpf.Editors.ComboBoxEdit, DevExpress.Xpf.Core.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", "DevExpress.Xpf.LayoutControl.GroupBox, DevExpress.Xpf.LayoutControl.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", "DevExpress.Xpf.Grid.GridControl, DevExpress.Xpf.Grid.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a", "DevExpress.Xpf.Editors.PopupColorEdit, DevExpress.Xpf.Core.v24.1, Version=24.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a");
	}
}
