using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using PvfCode.Controls.TextEditorFolder;
using PvfCode.Localization;
using PvfCode.Views;

namespace PvfCode.Compatibility;

internal static class RecoveredXamlSelfTest
{
	private static readonly string[] ExpectedIconsDarkResourceKeys =
	[
		"Action_Delete",
		"AddClassIcon",
		"AddLayoutItem",
		"AddSearchResultPanelIcon",
		"BackwardsIcon",
		"BookMarkIcon",
		"BorderElement_16x",
		"CaseSensitiveXamlIcon",
		"Checkmark_16x",
		"ClearWindowContent",
		"Close",
		"CloseAll_16x",
		"Close_red_16x",
		"CodeIcon",
		"CollapseGroup_16x",
		"CollapseUpSearchPanel",
		"CommentCode",
		"ConfigurationFile",
		"Convert",
		"CopyIcon",
		"CutIcon",
		"DeleteClassIcon2",
		"DiffIcon",
		"DiffLeftIcon",
		"DiffRightIcon",
		"Download_16x",
		"EditComment",
		"EditorShowSerchPanel",
		"ExpandSearchPanel",
		"ExportText",
		"ExtractFilesIcon",
		"FindDown",
		"FindNextIcon",
		"FindPreviousIcon",
		"FindinFiles_16x",
		"FolderClosed_16x",
		"ForwardsIcon",
		"GoToDown",
		"GoToFirstIcon",
		"GoToLastIcon",
		"GoToUp",
		"GroupSelectCursor_16x",
		"HigSectionIcon",
		"HotReload",
		"ImportFilesIcon",
		"ImportText",
		"ItemName",
		"Location",
		"MacroIcon",
		"MacroStore",
		"Memory",
		"MethodSealed",
		"NewFile_16x",
		"NewFolder_16x",
		"PanelDragDropTarget",
		"PasteIcon",
		"PrintPreview_16x",
		"PvfRelease",
		"PvfScriptFile",
		"Question_16x",
		"QuickReplaceIcon",
		"QuickReplaceXamlIcon",
		"RefreshIcon",
		"RegularExpression",
		"RegularExpressionXamlIcon",
		"Remove_color_16x",
		"RenameImage",
		"ReplaceAllXamlIcon",
		"ResizeGripLeftXamlIcon",
		"RunIcon",
		"SaveAs_16x",
		"SaveStatusBar",
		"Save_16x",
		"SearchIcon",
		"SelectAllIcon",
		"SendTestMail_16x",
		"SettingWindowIcon",
		"Space_16x",
		"StatusInvalidOutline_16x",
		"StatusOK_16x",
		"StatusWarning_16x",
		"StopIcon",
		"StringIcon",
		"SynchronousMessage",
		"TreeViewIcon",
		"UncommentCode",
		"UploadIcon",
		"VisualStudioBlendLogo2015Pre",
		"WebApiOpenDecompiling",
		"WholeWordMatchXamlIcon",
		"WholeWordMatch_16x",
		"WordWrap",
		"ZipFileIcon",
		"chatGPTSend",
	];

	public static int Run(App application)
	{
		string? resultPath = Environment.GetEnvironmentVariable("PVFUTILITY_RECOVERED_XAML_SELF_TEST_RESULT");
		try
		{
			string resourceMode = Environment.GetEnvironmentVariable("PVFUTILITY_RECOVERED_XAML_RESOURCE_MODE")
				?? throw new InvalidOperationException("The recovered XAML resource mode was not provided.");
			if (resourceMode is not ("Legacy" or "Hybrid" or "SourceOnly"))
			{
				throw new InvalidOperationException($"Unknown recovered XAML resource mode '{resourceMode}'.");
			}

			ThemeSwitcher.Instance.SwitchTheme(AppSetting.Instance.NowThemeType);
			LanguageResourceManager.ApplyLanguageAsync(AppSetting.Instance.CurrentLang)
				.GetAwaiter()
				.GetResult();

			AssertEqual(11, application.Resources.MergedDictionaries.Count, "application merged dictionary count");

			ResourceDictionary darkIcons = Application.LoadComponent(
				new Uri("/Themes/Styles/IconsDark.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary
				?? throw new InvalidOperationException("IconsDark.xaml did not load as a ResourceDictionary.");
			AssertResourceKeys(darkIcons);
			AssertResource<DrawingImage>(darkIcons, "chatGPTSend");
			AssertResource<DrawingImage>(darkIcons, "Close");
			AssertResource<DrawingImage>(darkIcons, "GoToDown");
			if (resourceMode is "Hybrid" or "SourceOnly")
			{
				AssertResourcePresent(darkIcons, "Action_Delete");
				AssertResourcePresent(darkIcons, "QuickReplaceIcon");
				AssertResourcePresent(darkIcons, "CommentCode");
			}

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

			WriteResult(resultPath, $"PASS: {resourceMode} app.xaml, themes/styles/iconsdark.xaml, and views/viewscripteditor.xaml loaded with the expected resources, controls, and bindings.");
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

	private static void AssertResourceKeys(ResourceDictionary dictionary)
	{
		HashSet<string> actualKeys = new(StringComparer.Ordinal);
		foreach (object key in dictionary.Keys)
		{
			if (key is not string stringKey)
			{
				throw new InvalidOperationException(
					$"IconsDark contained a non-string resource key of type '{key.GetType().FullName}'.");
			}
			actualKeys.Add(stringKey);
		}

		HashSet<string> expectedKeys = new(ExpectedIconsDarkResourceKeys, StringComparer.Ordinal);
		List<string> missingKeys = [];
		foreach (string key in ExpectedIconsDarkResourceKeys)
		{
			if (!actualKeys.Contains(key))
			{
				missingKeys.Add(key);
			}
		}

		List<string> unexpectedKeys = [];
		foreach (string key in actualKeys)
		{
			if (!expectedKeys.Contains(key))
			{
				unexpectedKeys.Add(key);
			}
		}
		unexpectedKeys.Sort(StringComparer.Ordinal);

		if (missingKeys.Count != 0 || unexpectedKeys.Count != 0)
		{
			throw new InvalidOperationException(
				$"IconsDark resource key set differed. Missing: [{string.Join(", ", missingKeys)}]. " +
				$"Unexpected: [{string.Join(", ", unexpectedKeys)}].");
		}
	}

	private static void AssertResourceKey(ResourceDictionary dictionary, string key)
	{
		if (!dictionary.Contains(key))
		{
			throw new InvalidOperationException($"IconsDark resource '{key}' was not present.");
		}
	}

	private static object AssertResourcePresent(ResourceDictionary dictionary, string key)
	{
		AssertResourceKey(dictionary, key);
		return dictionary[key]
			?? throw new InvalidOperationException($"IconsDark resource '{key}' resolved to null.");
	}

	private static void AssertResource<T>(ResourceDictionary dictionary, string key)
	{
		object resource = AssertResourcePresent(dictionary, key);
		if (resource is not T)
		{
			throw new InvalidOperationException(
				$"IconsDark resource '{key}' expected type '{typeof(T).FullName}', got '{resource.GetType().FullName}'.");
		}
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
