using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.AvalonEdit;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.Localization;
using PvfCode.Views;

namespace PvfCode.Compatibility;

internal static class RecoveredXamlSelfTest
{
	public static int Run(App application)
	{
		string? resultPath = Environment.GetEnvironmentVariable("PVFUTILITY_RECOVERED_XAML_SELF_TEST_RESULT");
		try
		{
			ThemeSwitcher.Instance.SwitchTheme(AppSetting.Instance.NowThemeType);
			LanguageResourceManager.ApplyLanguageAsync(AppSetting.Instance.CurrentLang)
				.GetAwaiter()
				.GetResult();

			AssertEqual(11, application.Resources.MergedDictionaries.Count, "application merged dictionary count");

			ViewScriptEditor window = new();
			try
			{
				AssertBinding(window, Window.TitleProperty, "Title", "window title");
				AssertBinding(window, System.Windows.Controls.Control.FontSizeProperty, "FontSize", "window font size");

				if (window.Content is not Grid grid ||
					grid.Children.Count != 1 ||
					grid.Children[0] is not TextEditorBase editor)
				{
					throw new InvalidOperationException(
						"ViewScriptEditor did not load its expected Grid/TextEditorBase content from BAML.");
				}

				AssertEqual(false, editor.AllowCompletion, "AllowCompletion");
				AssertEqual(false, editor.AllowFolding, "AllowFolding");
				AssertEqual(new Thickness(10, 0, 0, 0), editor.ContentMargin, "ContentMargin");

				Binding documentBinding = AssertBinding(editor, TextEditor.DocumentProperty, "Document", "document");
				AssertEqual(BindingMode.TwoWay, documentBinding.Mode, "document binding mode");
				AssertBinding(editor, TextEditor.IsReadOnlyProperty, "IsReadOnly", "read-only");
			}
			finally
			{
				window.DataContext = null;
			}

			WriteResult(resultPath, "PASS: app.xaml and views/viewscripteditor.xaml loaded with the expected dictionaries, controls, and bindings.");
			return 0;
		}
		catch (Exception exception)
		{
			WriteResult(resultPath, exception.ToString());
			return 1;
		}
	}

	private static Binding AssertBinding(
		DependencyObject target,
		DependencyProperty property,
		string expectedPath,
		string scenario)
	{
		Binding binding = BindingOperations.GetBinding(target, property)
			?? throw new InvalidOperationException($"{scenario}: binding was not present.");
		AssertEqual(expectedPath, binding.Path?.Path, $"{scenario} path");
		return binding;
	}

	private static void AssertEqual<T>(T expected, T actual, string scenario)
	{
		if (!EqualityComparer<T>.Default.Equals(expected, actual))
		{
			throw new InvalidOperationException($"{scenario}: expected '{expected}', got '{actual}'.");
		}
	}

	private static void WriteResult(string? resultPath, string message)
	{
		if (string.IsNullOrWhiteSpace(resultPath))
		{
			return;
		}

		string fullPath = Path.GetFullPath(resultPath);
		string? directory = Path.GetDirectoryName(fullPath);
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}
		File.WriteAllText(fullPath, message);
	}
}
