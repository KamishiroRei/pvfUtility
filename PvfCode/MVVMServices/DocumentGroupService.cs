using System.Linq;
using System.Runtime.CompilerServices;
using DevExpress.Data.Extensions;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Docking;
using PvfCode.MVVMServices;
using PvfCode.ViewModels.DocumentFolder;

namespace FKRF7IQiMoSJtPdh45P;

internal class NRjpElQyfkexPvgV2wp : ServiceBase, IDocumentGroupService
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		public DocumentBase jNjEU2OyPZ;

		public _003C_003Ec__DisplayClass3_0()
		{
		}

		internal bool YmvEpFqs8E(BaseLayoutItem it)
		{
			return it.DataContext == jNjEU2OyPZ;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass4_0
	{
		public DocumentBase vCDE8DVo2O;

		public _003C_003Ec__DisplayClass4_0()
		{
		}

		internal bool Q5EEcfeXdI(BaseLayoutItem it)
		{
			return it.DataContext == vCDE8DVo2O;
		}
	}

	private DocumentGroup Group => (DocumentGroup)base.AssociatedObject;

	public void ShowContextMenu()
	{
		_ = Group.ContextMenuCustomizations;
	}

	public void NextDocument(DocumentBase P_0)
	{
		_003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass3_0();
		CS_0024_003C_003E8__locals3.jNjEU2OyPZ = P_0;
		BaseLayoutItem[] items = Group.GetItems();
		if (Group.SelectedItem != null)
		{
			CS_0024_003C_003E8__locals3.jNjEU2OyPZ = Group.SelectedItem.DataContext as DocumentBase;
		}
		if (items != null && items.Any())
		{
			int num = items.FindIndex((BaseLayoutItem it) => it.DataContext == CS_0024_003C_003E8__locals3.jNjEU2OyPZ);
			if (num < items.Length - 1 && items[num + 1].DataContext is DocumentBase documentBase)
			{
				documentBase.IsActive = true;
			}
		}
	}

	public void LastDocument(DocumentBase P_0)
	{
		_003C_003Ec__DisplayClass4_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass4_0();
		CS_0024_003C_003E8__locals3.vCDE8DVo2O = P_0;
		BaseLayoutItem[] items = Group.GetItems();
		if (Group.SelectedItem != null)
		{
			CS_0024_003C_003E8__locals3.vCDE8DVo2O = Group.SelectedItem.DataContext as DocumentBase;
		}
		if (items != null && items.Any())
		{
			int num = items.FindIndex((BaseLayoutItem it) => it.DataContext == CS_0024_003C_003E8__locals3.vCDE8DVo2O);
			if (num > 0 && items[num - 1].DataContext is DocumentBase documentBase)
			{
				documentBase.IsActive = true;
			}
		}
	}

	public NRjpElQyfkexPvgV2wp()
	{
	}
}
