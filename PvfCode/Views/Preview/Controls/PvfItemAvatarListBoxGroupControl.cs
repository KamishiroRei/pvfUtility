using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Grid;
using PvfCode.Services.PreviewPvfFileFolder;
using Swordfish.NET.Collections;

namespace PvfCode.Views.Preview.Controls;

public class PvfItemAvatarListBoxGroupControl : UserControl, IComponentConnector
{
	public static readonly DependencyProperty ItemsProperty;

	internal GridControl Grid;

	internal GridColumn columnName;

	internal CardView View;

	private bool NLIHVPspDW;

	public ConcurrentObservableCollection<FileItemIconPreviewViewModel> Items
	{
		get
		{
			return (ConcurrentObservableCollection<FileItemIconPreviewViewModel>)((DependencyObject)this).GetValue(ItemsProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(ItemsProperty, (object)value);
		}
	}

	public PvfItemAvatarListBoxGroupControl()
	{
		InitializeComponent();
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!NLIHVPspDW)
		{
			NLIHVPspDW = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/preview/controls/pvfitemavatarlistboxgroupcontrol.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			Grid = (GridControl)target;
			break;
		case 2:
			columnName = (GridColumn)target;
			break;
		case 3:
			View = (CardView)target;
			break;
		default:
			NLIHVPspDW = true;
			break;
		}
	}

	static PvfItemAvatarListBoxGroupControl()
	{
		ItemsProperty = DependencyProperty.Register("Items", typeof(ConcurrentObservableCollection<FileItemIconPreviewViewModel>), typeof(PvfItemAvatarListBoxGroupControl), new PropertyMetadata((PropertyChangedCallback)null));
	}
}
