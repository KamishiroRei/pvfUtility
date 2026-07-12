using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors;
using PvfCode.ViewModels.DocumentFolder;
using WpfRangeControls;

namespace PvfCode.Controls.TextEditorFolder;

public class DocumentTextEditor : UserControl, IComponentConnector
{
	[CompilerGenerated]
	private int yYEgWes4XC;

	internal DocumentTextEditor documentEditor;

	internal ButtonEdit pathTextBox;

	internal Grid gridMain;

	internal TextEditorBase editor;

	internal Grid gridScrollMap;

	internal RangeScrollbar rscrollbar;

	private bool WDCgmP7chM;

	public int TopToolBarTextBoxWidth
	{
		[CompilerGenerated]
		get
		{
			return yYEgWes4XC;
		}
		[CompilerGenerated]
		set
		{
			yYEgWes4XC = value;
		}
	}

	public DocumentTextEditor()
	{
		InitializeComponent();
		NormalizeToolbarLabels();
		InstallDocumentActions();
	}

	private void InstallDocumentActions()
	{
		ToolBarControl toolBar = FindVisualChild<ToolBarControl>(this);
		if (toolBar == null)
		{
			return;
		}
		toolBar.Items.Add(new BarItemSeparator());
		toolBar.Items.Add(new BarButtonItem
		{
			Content = "向右拆分编辑器",
			ToolTip = "向右拆分编辑器",
			Glyph = TryFindResource("AddLayoutItem") as ImageSource,
			Command = new DelegateCommand(() => (DataContext as DocumentBase)?.OnSplitRight())
		});
		BarButtonItem previewButton = new()
		{
			Content = "在侧边打开预览",
			ToolTip = "在侧边打开预览",
			Glyph = TryFindResource("PrintPreview_16x") as ImageSource,
			Command = new DelegateCommand(() => (DataContext as PvfFileDocument)?.OnOpenPreview())
		};
		previewButton.SetBinding(BarItem.IsVisibleProperty, new Binding(nameof(PvfFileDocument.SupportsPreview)));
		toolBar.Items.Add(previewButton);
	}

	private void NormalizeToolbarLabels()
	{
		ToolBarControl toolBar = FindVisualChild<ToolBarControl>(this);
		if (toolBar == null)
		{
			return;
		}
		foreach (BarItem item in toolBar.Items.OfType<BarItem>())
		{
			NormalizeBarItem(item);
		}
	}

	private void NormalizeBarItem(BarItem item)
	{
		string content = item.Content?.ToString();
		if (content == "GotoLine")
		{
			item.Content = TryFindResource("DocumentTextEditor_BarToolControl_GoToOffset") ?? "跳转到偏移量";
		}
		else if (content == "DocumentTextEditor_BarToolControl_CommentSelectedLine")
		{
			item.Content = TryFindResource(content) ?? "注释选中行";
		}
		if (item is BarSplitButtonItem splitButton && splitButton.PopupControl is PopupMenu popupMenu)
		{
			foreach (IBarItem popupItem in popupMenu.Items)
			{
				if (popupItem is BarItem childItem)
				{
					NormalizeBarItem(childItem);
				}
			}
		}
	}

	private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
	{
		int childCount = VisualTreeHelper.GetChildrenCount(parent);
		for (int index = 0; index < childCount; index++)
		{
			DependencyObject child = VisualTreeHelper.GetChild(parent, index);
			if (child is T match)
			{
				return match;
			}
			T descendant = FindVisualChild<T>(child);
			if (descendant != null)
			{
				return descendant;
			}
		}
		return null;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!WDCgmP7chM)
		{
			WDCgmP7chM = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/texteditorfolder/documenttexteditor.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			documentEditor = (DocumentTextEditor)target;
			break;
		case 2:
			pathTextBox = (ButtonEdit)target;
			break;
		case 3:
			gridMain = (Grid)target;
			break;
		case 4:
			editor = (TextEditorBase)target;
			break;
		case 5:
			gridScrollMap = (Grid)target;
			break;
		case 6:
			rscrollbar = (RangeScrollbar)target;
			break;
		default:
			WDCgmP7chM = true;
			break;
		}
	}
}
