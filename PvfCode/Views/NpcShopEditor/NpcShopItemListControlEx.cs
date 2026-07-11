using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Grid;

namespace PvfCode.Views.NpcShopEditor;

public class NpcShopItemListControlEx : GridControl, IComponentConnector
{
	public static readonly DependencyProperty TypeProperty;

	public static readonly DependencyProperty ShowSearchPanelModeProperty;

	internal new CardView View;

	private bool KcNH3qUSW6;

	public NpcShopItemListControlType Type
	{
		get
		{
			return (NpcShopItemListControlType)((DependencyObject)this).GetValue(TypeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(TypeProperty, (object)value);
		}
	}

	public ShowSearchPanelMode ShowSearchPanelMode
	{
		get
		{
			return (ShowSearchPanelMode)((DependencyObject)this).GetValue(ShowSearchPanelModeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ShowSearchPanelModeProperty, (object)value);
		}
	}

	public NpcShopItemListControlEx()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!KcNH3qUSW6)
		{
			KcNH3qUSW6 = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/npcshopeditor/npcshopitemlistcontrolex.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		if (connectionId == 1)
		{
			View = (CardView)target;
		}
		else
		{
			KcNH3qUSW6 = true;
		}
	}

	static NpcShopItemListControlEx()
	{
		TypeProperty = DependencyProperty.Register("Type", typeof(NpcShopItemListControlType), typeof(NpcShopItemListControlEx), new PropertyMetadata((PropertyChangedCallback)null));
		ShowSearchPanelModeProperty = DependencyProperty.Register("ShowSearchPanelMode", typeof(ShowSearchPanelMode), typeof(NpcShopItemListControlEx), new PropertyMetadata((object)ShowSearchPanelMode.Always));
	}
}
