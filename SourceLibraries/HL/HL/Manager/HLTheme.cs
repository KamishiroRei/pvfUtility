using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using HL.HighlightingTheme;
using HL.Resources;
using HL.Xshtd;
using HL.Xshtd.interfaces;
using ICSharpCode.AvalonEdit.Highlighting;

namespace HL.Manager;

internal class HLTheme : IHLTheme
{
	private readonly object lockObj = new object();

	private Dictionary<string, IHighlightingDefinition> highlightingsByName = new Dictionary<string, IHighlightingDefinition>();

	private Dictionary<string, IHighlightingDefinition> highlightingsByExtension = new Dictionary<string, IHighlightingDefinition>(StringComparer.OrdinalIgnoreCase);

	private List<IHighlightingDefinition> allHighlightings = new List<IHighlightingDefinition>();

	private bool _HLThemeIsInitialized;

	private XhstdThemeDefinition _xshtd;

	private XmlHighlightingThemeDefinition _hlTheme;

	private readonly IHighlightingThemeDefinitionReferenceResolver _hLThemeResolver;

	public string Key { get; }

	public string HLBasePrefix { get; }

	public string HLBaseKey { get; }

	public string HLThemePrefix { get; }

	public string HLThemeFileName { get; }

	public string DisplayName { get; }

	public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
	{
		get
		{
			lock (lockObj)
			{
				return Array.AsReadOnly(allHighlightings.OrderBy((IHighlightingDefinition x) => x.Name).ToArray());
			}
		}
	}

	public IHighlightingThemeDefinition HlTheme
	{
		get
		{
			ResolveHighLightingTheme();
			return _hlTheme;
		}
	}

	public bool IsBuiltInThemesRegistered { get; set; }

	public HLTheme(string paramKey, string paramHLBasePrefix, string paramDisplayName)
		: this()
	{
		Key = paramKey;
		HLBasePrefix = paramHLBasePrefix;
		HLBaseKey = paramKey;
		DisplayName = paramDisplayName;
	}

	public HLTheme(string paramKey, string paramHLBaseKey, string paramDisplayName, string paramHLThemePrefix, string paramHLThemeName, IHighlightingThemeDefinitionReferenceResolver themeResolver)
		: this()
	{
		Key = paramKey;
		HLBaseKey = paramHLBaseKey;
		HLThemePrefix = paramHLThemePrefix;
		HLThemeFileName = paramHLThemeName;
		_hLThemeResolver = themeResolver;
		DisplayName = paramDisplayName;
	}

	protected HLTheme()
	{
	}

	public IHighlightingDefinition GetDefinition(string name)
	{
		lock (lockObj)
		{
			ResolveHighLightingTheme();
			if (highlightingsByName.TryGetValue(name, out var value))
			{
				return value;
			}
			return null;
		}
	}

	public SyntaxDefinition GetThemeDefinition(string highlightingName)
	{
		lock (lockObj)
		{
			ResolveHighLightingTheme();
			return _hlTheme.GetNamedSyntaxDefinition(highlightingName);
		}
	}

	public IHighlightingDefinition GetDefinitionByExtension(string extension)
	{
		lock (lockObj)
		{
			ResolveHighLightingTheme();
			if (highlightingsByExtension.TryGetValue(extension, out var value))
			{
				return value;
			}
			return null;
		}
	}

	public void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting)
	{
		lock (lockObj)
		{
			IHighlightingDefinition highlightingDefinition = allHighlightings.FirstOrDefault((IHighlightingDefinition i) => name == i.Name);
			if (highlightingDefinition != null)
			{
				allHighlightings.Remove(highlightingDefinition);
			}
			allHighlightings.Add(highlighting);
			if (name != null)
			{
				highlightingsByName[name] = highlighting;
			}
			if (extensions != null)
			{
				foreach (string key in extensions)
				{
					highlightingsByExtension[key] = highlighting;
				}
			}
		}
	}

	protected virtual void ResolveHighLightingTheme()
	{
		if (_hlTheme == null && !_HLThemeIsInitialized)
		{
			_HLThemeIsInitialized = true;
			_xshtd = ResolveHighLightingTheme(HLThemePrefix, HLThemeFileName);
			if (_hLThemeResolver != null && _xshtd != null)
			{
				_hlTheme = new XmlHighlightingThemeDefinition(_xshtd, _hLThemeResolver);
			}
		}
	}

	public XhstdThemeDefinition ResolveHighLightingTheme(string hLPrefix, string hLThemeName)
	{
		if (string.IsNullOrEmpty(hLPrefix) || string.IsNullOrEmpty(hLThemeName))
		{
			return null;
		}
		using Stream input = HLResources.OpenStream(hLPrefix, hLThemeName);
		using XmlTextReader reader = new XmlTextReader(input);
		return HighlightingThemeLoader.LoadXshd(reader, skipValidation: false);
	}
}
