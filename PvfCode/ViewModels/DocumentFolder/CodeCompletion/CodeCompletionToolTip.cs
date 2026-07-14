using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows.Data;
using PvfCode.Controls;
using TextEditLib;

namespace PvfCode.ViewModels.DocumentFolder.CodeCompletion;

public class CodeCompletionToolTip : PopupEx, IComponentConnector
{
	internal TextEdit editor;

	internal TextEdit editorAuto;

	private bool xk1ia37ywO;

	public CodeCompletionToolTip()
	{
		InitializeComponent();
		InstallMarkdownPreview();
	}

	private void InstallMarkdownPreview()
	{
		TabControl tabs = FindDescendant<TabControl>(Child as DependencyObject);
		if (tabs?.Items.Count < 1 || tabs.Items[0] is not TabItem commentTab || commentTab.Content is not Grid commentGrid)
		{
			return;
		}
		MarkdownDocumentViewer preview = new();
		preview.SetBinding(MarkdownDocumentViewer.TitleProperty, new Binding("Comment.Title"));
		preview.SetBinding(MarkdownDocumentViewer.MarkdownProperty, new Binding("Comment.Comment"));
		preview.SetBinding(MarkdownDocumentViewer.OfficialDescriptionProperty, new Binding("Comment.OfficialDescription"));
		Style style = new(typeof(MarkdownDocumentViewer));
		style.Setters.Add(new Setter(VisibilityProperty, Visibility.Visible));
		DataTrigger editing = new() { Binding = new Binding("IsEditorComment"), Value = true };
		editing.Setters.Add(new Setter(VisibilityProperty, Visibility.Collapsed));
		style.Triggers.Add(editing);
		preview.Style = style;
		Grid.SetRow(preview, 0);
		commentGrid.Children.Add(preview);
	}

	private static T FindDescendant<T>(DependencyObject parent) where T : DependencyObject
	{
		if (parent == null)
		{
			return null;
		}
		if (parent is T match)
		{
			return match;
		}
		foreach (object child in LogicalTreeHelper.GetChildren(parent))
		{
			if (child is DependencyObject dependencyObject)
			{
				T result = FindDescendant<T>(dependencyObject);
				if (result != null)
				{
					return result;
				}
			}
		}
		return null;
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!xk1ia37ywO)
		{
			xk1ia37ywO = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/viewmodels/documentfolder/codecompletion/codecompletiontooltip.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			editor = (TextEdit)target;
			break;
		case 2:
			editorAuto = (TextEdit)target;
			break;
		default:
			xk1ia37ywO = true;
			break;
		}
	}
}
