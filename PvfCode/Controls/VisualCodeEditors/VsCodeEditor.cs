using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using PvfCode.Models.Enums;
using PvfCode.ViewModels.VsCodeEditorViewModels.Enums;

namespace PvfCode.Controls.VisualCodeEditors;

public class VsCodeEditor : WebView2, IComponentConnector
{
	public static readonly DependencyProperty EditorLanguageProperty;

	public static readonly DependencyProperty PvfCodeThemeProperty;

	private bool contentLoaded;

	public bool IsLoadedE { get; set; }

	public LanguageType EditorLanguage
	{
		get
		{
			return (LanguageType)((DependencyObject)this).GetValue(EditorLanguageProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(EditorLanguageProperty, (object)value);
		}
	}

	public ThemeType PvfCodeTheme
	{
		get
		{
			return (ThemeType)((DependencyObject)this).GetValue(PvfCodeThemeProperty);
		}
		set
		{
			((DependencyObject)this).SetValue(PvfCodeThemeProperty, (object)value);
		}
	}

	public VsCodeEditor()
	{
		InitializeComponent();
		base.Visibility = Visibility.Collapsed;
		base.Source = new Uri("about:blank");
		base.CoreWebView2InitializationCompleted += OnCoreWebView2InitializationCompleted;
	}

	private void OnCoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
	{
		base.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
		base.CoreWebView2.Settings.AreDevToolsEnabled = false;
		base.CoreWebView2.DOMContentLoaded += OnDomContentLoaded;
		base.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
	}

	private async void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
	{
		await CheckVsCodeEditorThemeType();
		((VsCodeEditorModelBase)base.DataContext)?.Loaded(this);
		base.Visibility = Visibility.Visible;
	}

	private async void OnDomContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
	{
		IsLoadedE = true;
		await CheckVsCodeEditorThemeType();
	}

	public async Task SetDiffLeftText(string text, LanguageType languageType)
	{
		string arguments = $"\"{HttpUtility.JavaScriptStringEncode(text)}\",\"{languageType}\"";
		await base.CoreWebView2.ExecuteScriptAsync("SetLeftEditorText(" + arguments + ")");
	}

	public async Task SetDiffRightText(string text, LanguageType languageType)
	{
		string arguments = $"\"{HttpUtility.JavaScriptStringEncode(text)}\",\"{languageType}\"";
		await base.CoreWebView2.ExecuteScriptAsync("SetRightEditorText(" + arguments + ")");
	}

	public async Task SetDiffEditorText(string leftText, string rightText, LanguageType languageType)
	{
		await SetDiffLeftText(leftText, languageType);
		await SetDiffRightText(rightText, languageType);
	}

	public async Task GoToNextDiffLine()
	{
		await (base.CoreWebView2?.ExecuteScriptAsync("GoToNextDiffLine()"));
	}

	public async Task<string> GetText()
	{
		return await base.CoreWebView2.ExecuteScriptAsync("window.chrome.webview.postMessage(editor.getValue())");
	}

	public async Task<string> GetLeftDocumentText()
	{
		string text = await base.CoreWebView2.ExecuteScriptAsync("GetLeftDocumentText()");
		if (text != null && text.Length > 0)
		{
			text = text.Substring(1, text.Length - 2).Replace("\\r\\n\\r\\n", "\r\n").Replace("\\r\\n", "\r\n")
				.Replace("\\t", "\t");
		}
		return text;
	}

	public async Task<string> GetRightDocumentText()
	{
		string text = await base.CoreWebView2.ExecuteScriptAsync("GetRightDocumentText()");
		if (text != null && text.Length > 0)
		{
			text = text.Substring(1, text.Length - 2).Replace("\\r\\n\\r\\n", "\r\n").Replace("\\r\\n", "\r\n")
				.Replace("\\t", "\t");
		}
		return text;
	}

	public async Task SetTheme(VsVodeEditorThemeType vsVodeEditorThemeType)
	{
		await base.CoreWebView2.ExecuteScriptAsync("SetTheme(\"" + HttpUtility.JavaScriptStringEncode(vsVodeEditorThemeType.ToString()) + "\")");
	}

	public async Task CheckVsCodeEditorThemeType()
	{
		bool flag = PvfCodeTheme == ThemeType.VS2019Dark;
		VsVodeEditorThemeType? vsVodeEditorThemeType = null;
		switch (EditorLanguage)
		{
		case LanguageType.ScriptLanguage:
			vsVodeEditorThemeType = ((!flag) ? VsVodeEditorThemeType.ThemeScriptLight : VsVodeEditorThemeType.ThemeScriptDark);
			break;
		case LanguageType.Squirrel:
			vsVodeEditorThemeType = (flag ? VsVodeEditorThemeType.ThemeSquirrelDark : VsVodeEditorThemeType.ThemeSquirrelLight);
			break;
		}
		await SetTheme(vsVodeEditorThemeType.Value);
	}

	private static async void OnPvfCodeThemeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		VsCodeEditor vsCodeEditor = (VsCodeEditor)dependencyObject;
		if (vsCodeEditor != null && vsCodeEditor.IsLoadedE)
		{
			await vsCodeEditor.CheckVsCodeEditorThemeType();
		}
	}

	public void Clear()
	{
		if (base.CoreWebView2 != null)
		{
			base.CoreWebView2.DOMContentLoaded -= OnDomContentLoaded;
			base.CoreWebView2.NavigationCompleted -= OnNavigationCompleted;
		}
		base.CoreWebView2InitializationCompleted -= OnCoreWebView2InitializationCompleted;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!contentLoaded)
		{
			contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/visualcodeeditors/vscodeeditor.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		contentLoaded = true;
	}

	static VsCodeEditor()
	{
		EditorLanguageProperty = DependencyProperty.Register("EditorLanguage", typeof(LanguageType), typeof(VsCodeEditor), new PropertyMetadata((object)LanguageType.ScriptLanguage));
		PvfCodeThemeProperty = DependencyProperty.Register("PvfCodeTheme", typeof(ThemeType), typeof(VsCodeEditor), new PropertyMetadata((object)ThemeType.VS2019Blue, new PropertyChangedCallback(OnPvfCodeThemeChanged)));
	}
}
