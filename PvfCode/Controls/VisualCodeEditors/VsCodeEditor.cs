using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private bool oG9aRb1xif;

	public static readonly DependencyProperty EditorLanguageProperty;

	public static readonly DependencyProperty PvfCodeThemeProperty;

	private bool Oi4aNHn8Sh;

	public bool IsLoadedE
	{
		[CompilerGenerated]
		get
		{
			return oG9aRb1xif;
		}
		[CompilerGenerated]
		set
		{
			oG9aRb1xif = value;
		}
	}

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
		base.CoreWebView2InitializationCompleted += mBWa7mGCxV;
	}

	private void mBWa7mGCxV(object? sender, CoreWebView2InitializationCompletedEventArgs P_1)
	{
		base.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
		base.CoreWebView2.Settings.AreDevToolsEnabled = false;
		base.CoreWebView2.DOMContentLoaded += geRaptH025;
		base.CoreWebView2.NavigationCompleted += sTBaXQGEHZ;
	}

	private async void sTBaXQGEHZ(object? sender, CoreWebView2NavigationCompletedEventArgs P_1)
	{
		await CheckVsCodeEditorThemeType();
		((VsCodeEditorModelBase)base.DataContext)?.Loaded(this);
		base.Visibility = Visibility.Visible;
	}

	private async void geRaptH025(object? sender, CoreWebView2DOMContentLoadedEventArgs P_1)
	{
		IsLoadedE = true;
		await CheckVsCodeEditorThemeType();
	}

	public async Task SetDiffLeftText(string text, LanguageType languageType)
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
		defaultInterpolatedStringHandler.AppendLiteral("\"");
		defaultInterpolatedStringHandler.AppendFormatted(HttpUtility.JavaScriptStringEncode(text));
		defaultInterpolatedStringHandler.AppendLiteral("\",\"");
		defaultInterpolatedStringHandler.AppendFormatted(languageType);
		defaultInterpolatedStringHandler.AppendLiteral("\"");
		string text2 = defaultInterpolatedStringHandler.ToStringAndClear();
		await base.CoreWebView2.ExecuteScriptAsync("SetLeftEditorText(" + text2 + ")");
	}

	public async Task SetDiffRightText(string text, LanguageType languageType)
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
		defaultInterpolatedStringHandler.AppendLiteral("\"");
		defaultInterpolatedStringHandler.AppendFormatted(HttpUtility.JavaScriptStringEncode(text));
		defaultInterpolatedStringHandler.AppendLiteral("\",\"");
		defaultInterpolatedStringHandler.AppendFormatted(languageType);
		defaultInterpolatedStringHandler.AppendLiteral("\"");
		string text2 = defaultInterpolatedStringHandler.ToStringAndClear();
		await base.CoreWebView2.ExecuteScriptAsync("SetRightEditorText(" + text2 + ")");
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

	private static void KhOaVYpuWO(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		_ = (VsCodeEditor)(object)P_0;
	}

	private static async void XgJa3Jl8pe(DependencyObject P_0, DependencyPropertyChangedEventArgs P_1)
	{
		VsCodeEditor vsCodeEditor = (VsCodeEditor)(object)P_0;
		if (vsCodeEditor != null && vsCodeEditor.IsLoadedE)
		{
			await vsCodeEditor.CheckVsCodeEditorThemeType();
		}
	}

	public void Clear()
	{
		if (base.CoreWebView2 != null)
		{
			base.CoreWebView2.DOMContentLoaded -= geRaptH025;
			base.CoreWebView2.NavigationCompleted -= sTBaXQGEHZ;
		}
		base.CoreWebView2InitializationCompleted -= mBWa7mGCxV;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!Oi4aNHn8Sh)
		{
			Oi4aNHn8Sh = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/controls/visualcodeeditors/vscodeeditor.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		Oi4aNHn8Sh = true;
	}

	static VsCodeEditor()
	{
		EditorLanguageProperty = DependencyProperty.Register("EditorLanguage", typeof(LanguageType), typeof(VsCodeEditor), new PropertyMetadata((object)LanguageType.ScriptLanguage, new PropertyChangedCallback(KhOaVYpuWO)));
		PvfCodeThemeProperty = DependencyProperty.Register("PvfCodeTheme", typeof(ThemeType), typeof(VsCodeEditor), new PropertyMetadata((object)ThemeType.VS2019Blue, new PropertyChangedCallback(XgJa3Jl8pe)));
	}
}
