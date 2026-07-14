using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using TextEditLib;

namespace PvfCode.ViewModels.DocumentFolder;

public sealed class OfficialAnnotationDocumentView : UserControl
{
	private readonly TextEdit editor;
	private OfficialAnnotationDocument document;

	public OfficialAnnotationDocumentView()
	{
		Grid root = new();
		root.SetResourceReference(Panel.BackgroundProperty, "EditorBackground");
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
		root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

		Grid toolbar = new();
		toolbar.ColumnDefinitions.Add(new ColumnDefinition());
		toolbar.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

		ComboBox filePicker = new()
		{
			Margin = new Thickness(8, 8, 4, 8),
			MinWidth = 220,
			HorizontalAlignment = HorizontalAlignment.Stretch,
			IsEditable = true,
			IsTextSearchEnabled = true,
			StaysOpenOnEdit = true
		};
		AutomationProperties.SetAutomationId(filePicker, "OfficialAnnotationFilePicker");
		filePicker.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(nameof(OfficialAnnotationDocument.AvailableFiles)));
		filePicker.SetBinding(ComboBox.SelectedItemProperty, new Binding(nameof(OfficialAnnotationDocument.SelectedFile))
		{
			Mode = BindingMode.TwoWay,
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
		});
		toolbar.Children.Add(filePicker);

		CheckBox wordWrap = new()
		{
			Content = "自动换行",
			Margin = new Thickness(8, 8, 10, 8),
			VerticalAlignment = VerticalAlignment.Center
		};
		wordWrap.SetResourceReference(Control.ForegroundProperty, "EditorForeground");
		AutomationProperties.SetAutomationId(wordWrap, "OfficialAnnotationWordWrap");
		wordWrap.SetBinding(ToggleButton.IsCheckedProperty, new Binding(nameof(OfficialAnnotationDocument.WordWrap))
		{
			Mode = BindingMode.TwoWay,
			UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
		});
		Grid.SetColumn(wordWrap, 1);
		toolbar.Children.Add(wordWrap);

		Grid.SetRow(toolbar, 0);
		root.Children.Add(toolbar);

		editor = new TextEdit
		{
			IsReadOnly = true,
			ShowLineNumbers = true,
			FontFamily = new FontFamily("Consola"),
			HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
			VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
			EditorType = TextEditorType.Editor
		};
		editor.SetResourceReference(Control.BackgroundProperty, "EditorBackground");
		editor.SetResourceReference(Control.ForegroundProperty, "EditorForeground");
		editor.SetBinding(Control.FontSizeProperty, new Binding("EditConfig.SizeUnitLabel.ScreenPoints")
		{
			Source = AppSetting.Instance,
			Mode = BindingMode.OneWay
		});
		editor.SetBinding(TextEditor.WordWrapProperty, new Binding(nameof(OfficialAnnotationDocument.WordWrap))
		{
			Mode = BindingMode.OneWay
		});
		editor.SetBinding(TextEdit.ShowSpacesProperty, new Binding("EditConfig.ShowSpaces")
		{
			Source = AppSetting.Instance,
			Mode = BindingMode.OneWay
		});
		editor.SetBinding(TextEdit.ShowTabsProperty, new Binding("EditConfig.ShowTabs")
		{
			Source = AppSetting.Instance,
			Mode = BindingMode.OneWay
		});
		editor.SetBinding(TextEdit.ShowEndOfLineProperty, new Binding("EditConfig.ShowEndOfLine")
		{
			Source = AppSetting.Instance,
			Mode = BindingMode.OneWay
		});
		editor.SetBinding(TextEdit.SizeUnitLabelProperty, new Binding("EditConfig.SizeUnitLabel")
		{
			Source = AppSetting.Instance,
			Mode = BindingMode.OneWay
		});
		editor.SetBinding(TextEditor.SyntaxHighlightingProperty, new Binding(nameof(OfficialAnnotationDocument.Highlighting))
		{
			Mode = BindingMode.OneWay
		});
		editor.Options.AllowScrollBelowDocument = false;
		editor.Options.EnableEmailHyperlinks = false;
		editor.Options.HighlightCurrentLine = false;
		editor.Options.ShowBoxForControlCharacters = false;
		AutomationProperties.SetAutomationId(editor, "OfficialAnnotationReader");
		Grid.SetRow(editor, 1);
		root.Children.Add(editor);

		TextBlock status = new()
		{
			Margin = new Thickness(8, 4, 8, 6),
			TextTrimming = TextTrimming.CharacterEllipsis
		};
		status.SetResourceReference(TextBlock.ForegroundProperty, "EditorForeground");
		AutomationProperties.SetAutomationId(status, "OfficialAnnotationStatus");
		status.SetBinding(TextBlock.TextProperty, new Binding(nameof(OfficialAnnotationDocument.Status)));
		Grid.SetRow(status, 2);
		root.Children.Add(status);

		Content = root;
		DataContextChanged += OnDataContextChanged;
	}

	private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		if (document != null)
		{
			document.PropertyChanged -= OnDocumentPropertyChanged;
		}
		document = e.NewValue as OfficialAnnotationDocument;
		if (document != null)
		{
			document.PropertyChanged += OnDocumentPropertyChanged;
			editor.Text = document.Content ?? string.Empty;
			editor.ScrollToHome();
		}
	}

	private void OnDocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(OfficialAnnotationDocument.Content))
		{
			editor.Text = document?.Content ?? string.Empty;
			editor.ScrollToHome();
		}
	}
}
