using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using HL.Interfaces;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Models.Enums;
using ServiceLocator;

namespace PvfCode;

public class ThemeSwitcher : ModelBase
{
	public delegate void PvfCodeThemeChanged();

	private static ThemeSwitcher kJxjHTMQ1X;

	private readonly Dictionary<ThemeType, ResourceDictionary> rwwjho0Q0Q;

	[CompilerGenerated]
	private PvfCodeThemeChanged R8xjvb6Tvw;

	public static ThemeSwitcher Instance
	{
		get
		{
			if (kJxjHTMQ1X == null)
			{
				kJxjHTMQ1X = new ThemeSwitcher();
			}
			return kJxjHTMQ1X;
		}
	}

	public ThemeType SettingSelectedTheme
	{
		get
		{
			return AppSetting.Instance.NowThemeType;
		}
		set
		{
			AppSetting.Instance.NowThemeType = value;
			DoNotify("SettingSelectedTheme");
			AppCore.ViewModelBase.BarsVm.OnThemeChanged(value);
		}
	}

	public event PvfCodeThemeChanged PvfCodeThemeChangedEvent
	{
		[CompilerGenerated]
		add
		{
			PvfCodeThemeChanged pvfCodeThemeChanged = R8xjvb6Tvw;
			PvfCodeThemeChanged pvfCodeThemeChanged2;
			do
			{
				pvfCodeThemeChanged2 = pvfCodeThemeChanged;
				PvfCodeThemeChanged value2 = (PvfCodeThemeChanged)Delegate.Combine(pvfCodeThemeChanged2, value);
				pvfCodeThemeChanged = Interlocked.CompareExchange(ref R8xjvb6Tvw, value2, pvfCodeThemeChanged2);
			}
			while ((object)pvfCodeThemeChanged != pvfCodeThemeChanged2);
		}
		[CompilerGenerated]
		remove
		{
			PvfCodeThemeChanged pvfCodeThemeChanged = R8xjvb6Tvw;
			PvfCodeThemeChanged pvfCodeThemeChanged2;
			do
			{
				pvfCodeThemeChanged2 = pvfCodeThemeChanged;
				PvfCodeThemeChanged value2 = (PvfCodeThemeChanged)Delegate.Remove(pvfCodeThemeChanged2, value);
				pvfCodeThemeChanged = Interlocked.CompareExchange(ref R8xjvb6Tvw, value2, pvfCodeThemeChanged2);
			}
			while ((object)pvfCodeThemeChanged != pvfCodeThemeChanged2);
		}
	}

	public ThemeSwitcher()
	{
		rwwjho0Q0Q = new Dictionary<ThemeType, ResourceDictionary>();
		rwwjho0Q0Q.Add(ThemeType.VS2019Blue, GetThemeResourceDictionary(ThemeType.VS2019Blue));
		rwwjho0Q0Q.Add(ThemeType.VS2019Dark, GetThemeResourceDictionary(ThemeType.VS2019Dark));
		rwwjho0Q0Q.Add(ThemeType.VS2019Light, GetThemeResourceDictionary(ThemeType.VS2019Light));
	}

	public void SwitchTheme(ThemeType theme)
	{
		Application.Current.Resources.MergedDictionaries[0] = GetThemeResourceDictionary(theme);
		IThemedHighlightingManager service = GetService<IThemedHighlightingManager>();
		switch (theme)
		{
		case ThemeType.VS2019Blue:
			service.SetCurrentTheme("Light");
			break;
		case ThemeType.VS2019Dark:
			service.SetCurrentTheme("VS2019_Dark");
			break;
		case ThemeType.VS2019Light:
			service.SetCurrentTheme("Light");
			break;
		}
		UpdateTextEditorColorOptions();
		R8xjvb6Tvw?.Invoke();
	}

	public void PvfCodeThemeChangedEventInvoke()
	{
		R8xjvb6Tvw?.Invoke();
	}

	public ResourceDictionary GetThemeResourceDictionary(ThemeType theme)
	{
		Uri resourceLocator = null;
		switch (theme)
		{
		case ThemeType.VS2019Blue:
			resourceLocator = new Uri("/Themes/Vs2019Blue.xaml", UriKind.RelativeOrAbsolute);
			break;
		case ThemeType.VS2019Dark:
			resourceLocator = new Uri("/Themes/Vs2019Dark.xaml", UriKind.RelativeOrAbsolute);
			break;
		case ThemeType.VS2019Light:
			resourceLocator = new Uri("/Themes/Vs2019Light.xaml", UriKind.RelativeOrAbsolute);
			break;
		}
		return Application.LoadComponent(resourceLocator) as ResourceDictionary;
	}

	public TServiceContract GetService<TServiceContract>() where TServiceContract : class
	{
		return ServiceContainer.Instance.GetService<TServiceContract>();
	}

	public IHighlightingDefinition GetHighlightingDefinition(PvfFileType fileType)
	{
		IThemedHighlightingManager service = GetService<IThemedHighlightingManager>();
		return fileType switch
		{
			PvfFileType.nut => service.GetDefinition("Squirrel"), 
			PvfFileType.str => service.GetDefinition("Kor"), 
			PvfFileType.lst => service.GetDefinition("Lst"), 
			PvfFileType.bin => service.GetDefinition("StringTable"), 
			_ => service.GetDefinition("Script"), 
		};
	}

	public IHighlightingDefinition GetHighlightingDefinition(string name)
	{
		return GetService<IThemedHighlightingManager>().GetDefinition(name);
	}

	public void UpdateTextEditorColorOptions()
	{
		Action<IHighlightingDefinition> action = delegate(IHighlightingDefinition hig)
		{
			foreach (HighlightingColor namedHighlightingColor in hig.NamedHighlightingColors)
			{
				string name = namedHighlightingColor.Name;
				if (name != null)
				{
					switch (name.Length)
					{
					case 6:
						switch (name[0])
						{
						case 'H':
							if (name == "Header")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.Header);
							}
							break;
						case 'D':
							if (name == "Digits")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.Digits);
							}
							break;
						case 'S':
							if (name == "String")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.String);
							}
							break;
						}
						break;
					case 7:
						switch (name[0])
						{
						case 'S':
							if (name == "Section")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.Section);
							}
							break;
						case 'C':
							if (name == "Comment")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.Comment);
							}
							break;
						case 'K':
							if (name == "KorName")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.KorName);
							}
							break;
						}
						break;
					case 10:
						switch (name[0])
						{
						case 'S':
							if (name == "SectionEnd")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.Section);
							}
							break;
						case 'G':
							if (name == "GGenObject")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.GGenObject);
							}
							break;
						case 'M':
							if (name == "MethodCall")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.MethodCall);
							}
							break;
						}
						break;
					case 8:
						switch (name[1])
						{
						case 'e':
							if (name == "Keywords")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.Keywords);
							}
							break;
						case 'i':
							if (name == "FilePath")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.FilePath);
							}
							break;
						case 'o':
							if (name == "KorColon")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.KorColon);
							}
							break;
						}
						break;
					case 11:
						switch (name[6])
						{
						case 'r':
							if (name == "Curlybraces")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.Curlybraces);
							}
							break;
						case 'a':
							if (name == "Punctuation")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.Punctuation);
							}
							break;
						case 'I':
							if (name == "KorStrIndex")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.KorStrIndex);
							}
							break;
						case 'V':
							if (name == "KorStrValue")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.KorStrValue);
							}
							break;
						}
						break;
					case 19:
						switch (name[6])
						{
						case 'i':
							if (name == "KorStringMarkSymbol")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.KorStringMarkSymbol);
							}
							break;
						case 'A':
							if (name == "KorStrAngleBrackets")
							{
								namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.KorStrAngleBrackets);
							}
							break;
						}
						break;
					case 9:
						if (name == "Keywords2")
						{
							namedHighlightingColor.Foreground = new SimpleHighlightingBrush(AppSetting.Instance.EditConfig.NowEditorHighlightingColorOption.Keywords2);
						}
						break;
					}
				}
			}
		};
		IThemedHighlightingManager service = GetService<IThemedHighlightingManager>();
		IHighlightingDefinition definition = service.GetDefinition("Script");
		action(definition);
		definition = service.GetDefinition("Squirrel");
		action(definition);
		definition = service.GetDefinition("Kor");
		action(definition);
		definition = service.GetDefinition("Lst");
		action(definition);
	}
}
