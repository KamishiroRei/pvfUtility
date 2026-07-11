using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HL.HighlightingTheme;
using HL.Interfaces;
using HL.Resources;
using HL.Xshtd.interfaces;
using ICSharpCode.AvalonEdit.Highlighting;

namespace HL.Manager;

public class ThemedHighlightingManager : IThemedHighlightingManager, IHighlightingDefinitionReferenceResolver, IHighlightingThemeDefinitionReferenceResolver
{
	public const string HL_GENERIC_NAMESPACE_ROOT = "HL.Resources.Light";

	public const string HL_THEMES_NAMESPACE_ROOT = "HL.Resources.Themes";

	private readonly object lockObj = new object();

	private readonly Dictionary<string, IHLTheme> _ThemedHighlightings;

	public IHLTheme CurrentTheme { get; private set; }

	public static IThemedHighlightingManager Instance => DefaultHighlightingManager.Instance;

	public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
	{
		get
		{
			lock (lockObj)
			{
				if (CurrentTheme != null)
				{
					return CurrentTheme.HighlightingDefinitions;
				}
				return new ReadOnlyCollection<IHighlightingDefinition>(new List<IHighlightingDefinition>());
			}
		}
	}

	public ThemedHighlightingManager()
	{
		_ThemedHighlightings = new Dictionary<string, IHLTheme>();
	}

	IHighlightingDefinition IHighlightingDefinitionReferenceResolver.GetDefinition(string name)
	{
		lock (lockObj)
		{
			if (CurrentTheme != null)
			{
				return CurrentTheme.GetDefinition(name);
			}
			return null;
		}
	}

	public IHighlightingDefinition GetDefinitionByExtension(string extension)
	{
		lock (lockObj)
		{
			if (_ThemedHighlightings.TryGetValue(CurrentTheme.Key, out var value))
			{
				return value.GetDefinitionByExtension(extension);
			}
			return null;
		}
	}

	public void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting)
	{
		if (highlighting == null)
		{
			throw new ArgumentNullException("highlighting");
		}
		lock (lockObj)
		{
			if (CurrentTheme != null)
			{
				CurrentTheme.RegisterHighlighting(name, extensions, highlighting);
			}
		}
	}

	public void RegisterHighlighting(string name, string[] extensions, Func<IHighlightingDefinition> lazyLoadedHighlighting)
	{
		if (lazyLoadedHighlighting == null)
		{
			throw new ArgumentNullException("lazyLoadedHighlighting");
		}
		RegisterHighlighting(name, extensions, new DelayLoadedHighlightingDefinition(name, lazyLoadedHighlighting));
	}

	public void SetCurrentTheme(string themeNameKey)
	{
		SetCurrentThemeInternal(themeNameKey);
		HLResources.RegisterBuiltInHighlightings(DefaultHighlightingManager.Instance, CurrentTheme);
	}

	public void ThemedHighlightingAdd(string key, IHLTheme theme)
	{
		lock (lockObj)
		{
			_ThemedHighlightings.Add(key, theme);
		}
	}

	public void ThemedHighlightingRemove(string removekey)
	{
		lock (lockObj)
		{
			_ThemedHighlightings.Remove(removekey);
		}
	}

	protected void SetCurrentThemeInternal(string themeNameKey)
	{
		CurrentTheme = _ThemedHighlightings[themeNameKey];
	}

	protected virtual string GetPrefix(string themeNameKey)
	{
		lock (lockObj)
		{
			if (_ThemedHighlightings.TryGetValue(themeNameKey, out var value))
			{
				return value.HLBasePrefix;
			}
		}
		return null;
	}

	SyntaxDefinition IHighlightingThemeDefinitionReferenceResolver.GetThemeDefinition(string highlightingName)
	{
		lock (lockObj)
		{
			if (CurrentTheme != null)
			{
				return CurrentTheme.GetThemeDefinition(highlightingName);
			}
			return null;
		}
	}

	SyntaxDefinition IHighlightingThemeDefinitionReferenceResolver.GetThemeDefinition(string hlThemeName, string highlightingName)
	{
		lock (lockObj)
		{
			_ThemedHighlightings.TryGetValue(hlThemeName, out var value);
			return value?.GetThemeDefinition(hlThemeName);
		}
	}
}
