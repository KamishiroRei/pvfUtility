using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using PvfCode.Models.Enums;

namespace PvfCode.Models.Options;

/// <summary>
/// 单个后缀的“自动打开预览”开关，供设置界面按行绑定。
/// 取值直接读写宿主 <see cref="PvfFilePreviewOptions"/>，保证保存时与 JSON 落到同一份数据。
/// </summary>
public sealed class PvfPreviewExtensionToggle : ViewModelBase
{
	private readonly PvfFilePreviewOptions owner;

	public string Extension { get; }

	public string DisplayName { get; }

	public string DisplayText => $"{DisplayName}（*.{Extension}）";

	public bool IsEnabled
	{
		get
		{
			return owner.IsAutoOpenEnabled(Extension);
		}
		set
		{
			if (owner.IsAutoOpenEnabled(Extension) == value)
			{
				return;
			}
			owner.SetAutoOpenEnabled(Extension, value);
			RaisePropertyChanged("IsEnabled");
		}
	}

	public PvfPreviewExtensionToggle(PvfFilePreviewOptions owner, string extension, string displayName)
	{
		this.owner = owner;
		Extension = extension;
		DisplayName = displayName;
	}
}

[JsonObject(MemberSerialization.OptOut)]
public class PvfFilePreviewOptions : ViewModelBase
{
	private Dictionary<ThemeType, PvfFilePreviewColorOptions> LK2Ek5VtJq;

	private double? rV8ELcIB2t;

	private Dictionary<string, bool> autoOpenExtensions;

	private ObservableCollection<PvfPreviewExtensionToggle> autoOpenExtensionSettings;

	/// <summary>
	/// 具备可视化预览能力、因而可以单独配置自动打开行为的后缀。
	/// 与运行时能力判定 PvfPreviewDocument.Supports 对应的后缀集合一致；此处只描述“可配置项”，不承担能力判定。
	/// </summary>
	public static readonly string[] PreviewableExtensions = { "xui", "ani", "equ", "stk", "shp", "qst", "skl", "als", "co", "etc" };

	private static readonly Dictionary<string, string> PreviewableExtensionNames = new(StringComparer.OrdinalIgnoreCase)
	{
		["xui"] = "界面布局",
		["ani"] = "动画帧",
		["equ"] = "装备",
		["stk"] = "道具/堆叠",
		["shp"] = "商店",
		["qst"] = "任务",
		["skl"] = "技能",
		["als"] = "动画分层",
		["co"] = "技能树/脚本",
		["etc"] = "技能树/其它脚本"
	};

	public Dictionary<ThemeType, PvfFilePreviewColorOptions> ColorConfigs
	{
		get
		{
			if (LK2Ek5VtJq == null)
			{
				LK2Ek5VtJq = new Dictionary<ThemeType, PvfFilePreviewColorOptions>();
			}
			foreach (object value in Enum.GetValues(typeof(ThemeType)))
			{
				ThemeType themeType = (ThemeType)Enum.Parse(typeof(ThemeType), value.ToString());
				if (!LK2Ek5VtJq.ContainsKey(themeType))
				{
					LK2Ek5VtJq.Add(themeType, new PvfFilePreviewColorOptions(themeType));
				}
			}
			return LK2Ek5VtJq;
		}
	}

	[JsonIgnore]
	public PvfFilePreviewColorOptions CurrentColorConfing
	{
		get
		{
			if (ColorConfigs.TryGetValue(AppSetting.Instance.NowThemeType, out PvfFilePreviewColorOptions value))
			{
				return value;
			}
			return null;
		}
	}

	public double? FontSize
	{
		get
		{
			if (!rV8ELcIB2t.HasValue)
			{
				rV8ELcIB2t = 12.0;
			}
			return rV8ELcIB2t;
		}
		set
		{
			rV8ELcIB2t = value;
			RaisePropertyChanged("FontSize");
		}
	}

	public bool ForbidShowToolTip
	{
		get
		{
			return GetProperty(() => ForbidShowToolTip);
		}
		set
		{
			SetProperty(() => ForbidShowToolTip, value);
		}
	}

	/// <summary>
	/// 按后缀记录的“不自动打开预览”标记：键为不含点的后缀（小写），值为 false 表示已由用户关闭。
	/// 只为 true 或缺失时才自动打开，因此旧配置（无此节点）行为不变。
	/// </summary>
	public Dictionary<string, bool> AutoOpenExtensions
	{
		get
		{
			if (autoOpenExtensions == null)
			{
				autoOpenExtensions = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
			}
			return autoOpenExtensions;
		}
		set
		{
			autoOpenExtensions = value;
		}
	}

	/// <summary>设置界面使用的开关项集合，逐项读写 <see cref="AutoOpenExtensions"/>，不参与序列化。</summary>
	[JsonIgnore]
	public ObservableCollection<PvfPreviewExtensionToggle> AutoOpenExtensionSettings
	{
		get
		{
			if (autoOpenExtensionSettings == null)
			{
				autoOpenExtensionSettings = new ObservableCollection<PvfPreviewExtensionToggle>();
				foreach (string extension in PreviewableExtensions)
				{
					autoOpenExtensionSettings.Add(new PvfPreviewExtensionToggle(this, extension, PreviewExtensionDisplayName(extension)));
				}
			}
			return autoOpenExtensionSettings;
		}
	}

	public void ChangedCurrentColorConfing()
	{
		RaisePropertiesChanged("CurrentColorConfing");
	}

	/// <summary>把后缀或带点的扩展名统一成不含点的小写键；空值返回空串。</summary>
	public static string NormalizePreviewExtension(string extension)
	{
		if (string.IsNullOrWhiteSpace(extension))
		{
			return string.Empty;
		}
		return extension.Trim().TrimStart('.').ToLowerInvariant();
	}

	public static string PreviewExtensionDisplayName(string extension)
	{
		string key = NormalizePreviewExtension(extension);
		if (PreviewableExtensionNames.TryGetValue(key, out string name))
		{
			return name;
		}
		return key;
	}

	/// <summary>该后缀未被用户关闭时返回 true；未知后缀与缺失键一律视为开启。</summary>
	public bool IsAutoOpenEnabled(string extension)
	{
		string key = NormalizePreviewExtension(extension);
		if (key.Length == 0)
		{
			return true;
		}
		return !AutoOpenExtensions.TryGetValue(key, out bool enabled) || enabled;
	}

	public void SetAutoOpenEnabled(string extension, bool enabled)
	{
		string key = NormalizePreviewExtension(extension);
		if (key.Length == 0)
		{
			return;
		}
		// 只持久化被关闭的后缀，保持配置文件精简，同时让“缺省即开启”的语义可读。
		if (enabled)
		{
			AutoOpenExtensions.Remove(key);
		}
		else
		{
			AutoOpenExtensions[key] = false;
		}
		RaisePropertyChanged("AutoOpenExtensions");
	}

	/// <summary>文档激活时是否应当自动在侧边打开预览页。</summary>
	public bool ShouldAutoOpen(PvfFile file)
	{
		if (file == null)
		{
			return false;
		}
		return IsAutoOpenEnabled(file.Extension);
	}

	public PvfFilePreviewOptions()
	{
	}
}
