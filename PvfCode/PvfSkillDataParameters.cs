using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace PvfCode;

internal sealed class PvfSkillDataParameterLabels
{
	public Dictionary<int, List<string>> LevelInfo { get; } = new();

	public Dictionary<int, List<string>> StaticData { get; } = new();
}

internal sealed class PvfSkillDataParameterSkill
{
	public string Key { get; init; }

	public string Name { get; init; }

	public Dictionary<string, PvfSkillDataParameterLabels> Scenes { get; } = new(StringComparer.OrdinalIgnoreCase);

	public PvfSkillDataParameterLabels GetLabels(string sceneKey)
	{
		Scenes.TryGetValue("default", out PvfSkillDataParameterLabels defaults);
		Scenes.TryGetValue(sceneKey ?? "default", out PvfSkillDataParameterLabels scene);
		if (defaults == null)
		{
			return scene;
		}
		if (scene == null || ReferenceEquals(defaults, scene))
		{
			return defaults;
		}
		PvfSkillDataParameterLabels merged = new();
		CopyLabels(defaults.LevelInfo, merged.LevelInfo);
		CopyLabels(defaults.StaticData, merged.StaticData);
		CopyLabels(scene.LevelInfo, merged.LevelInfo);
		CopyLabels(scene.StaticData, merged.StaticData);
		return merged;
	}

	private static void CopyLabels(Dictionary<int, List<string>> source, Dictionary<int, List<string>> destination)
	{
		foreach ((int index, List<string> labels) in source)
		{
			destination[index] = new List<string>(labels);
		}
	}
}

internal static class PvfSkillDataParameters
{
	private sealed class Catalog
	{
		public Dictionary<string, PvfSkillDataParameterSkill> ByPath { get; } = new(StringComparer.OrdinalIgnoreCase);

		public Dictionary<int, List<PvfSkillDataParameterSkill>> ByCode { get; } = new();

		public Dictionary<string, string> EquipmentSetAliases { get; } = new(StringComparer.OrdinalIgnoreCase);
	}

	private const string ResourceName = "PvfCode.SkillDataParameters.json";
	private static readonly Lazy<Catalog> Data = new(Load, true);

	public static PvfSkillDataParameterSkill Find(PvfFile file)
	{
		if (file == null)
		{
			return null;
		}
		Catalog catalog = Data.Value;
		string path = NormalizeKey(file.FileName);
		string shortPath = RemoveSkillPrefix(path);
		if (catalog.ByPath.TryGetValue(path, out PvfSkillDataParameterSkill exact) ||
			catalog.ByPath.TryGetValue(shortPath, out exact))
		{
			return exact;
		}
		if (!file.ItemCode.HasValue || !catalog.ByCode.TryGetValue(file.ItemCode.Value, out List<PvfSkillDataParameterSkill> matches))
		{
			return null;
		}
		string baseName = Path.GetFileNameWithoutExtension(path);
		return matches.FirstOrDefault(skill => Path.GetFileNameWithoutExtension(skill.Key).Equals(baseName, StringComparison.OrdinalIgnoreCase)) ??
			(matches.Count == 1 ? matches[0] : null);
	}

	public static PvfSkillDataParameterSkill FindForEquipmentSet(PvfFile file)
	{
		if (file == null)
		{
			return null;
		}
		Catalog catalog = Data.Value;
		string source = RemoveSkillPrefix(NormalizeKey(file.FileName));
		if (catalog.EquipmentSetAliases.TryGetValue(source, out string target) &&
			catalog.ByPath.TryGetValue(RemoveSkillPrefix(NormalizeKey(target)), out PvfSkillDataParameterSkill alias))
		{
			return alias;
		}
		return Find(file);
	}

	private static Catalog Load()
	{
		Catalog catalog = new();
		try
		{
			using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
			if (stream == null)
			{
				return catalog;
			}
			using JsonDocument document = JsonDocument.Parse(stream);
			if (!document.RootElement.TryGetProperty("skills", out JsonElement skills) || skills.ValueKind != JsonValueKind.Object)
			{
				return catalog;
			}
			foreach (JsonProperty property in skills.EnumerateObject())
			{
				PvfSkillDataParameterSkill skill = ParseSkill(property.Name, property.Value);
				if (skill == null)
				{
					continue;
				}
				catalog.ByPath[skill.Key] = skill;
				catalog.ByPath[$"skill/{skill.Key}"] = skill;
				if (property.Value.TryGetProperty("codes", out JsonElement codes) && codes.ValueKind == JsonValueKind.Array)
				{
					foreach (JsonElement code in codes.EnumerateArray())
					{
						if (code.TryGetInt32(out int value))
						{
							AddCode(catalog, value, skill);
						}
					}
				}
			}
			if (document.RootElement.TryGetProperty("equipmentSetSkillAliases", out JsonElement aliases) && aliases.ValueKind == JsonValueKind.Object)
			{
				foreach (JsonProperty property in aliases.EnumerateObject())
				{
					if (property.Value.ValueKind == JsonValueKind.Object &&
						property.Value.TryGetProperty("target", out JsonElement target) && target.ValueKind == JsonValueKind.String)
					{
						catalog.EquipmentSetAliases[RemoveSkillPrefix(NormalizeKey(property.Name))] =
							RemoveSkillPrefix(NormalizeKey(target.GetString()));
					}
				}
			}
			if (document.RootElement.TryGetProperty("byCode", out JsonElement byCode) && byCode.ValueKind == JsonValueKind.Object)
			{
				foreach (JsonProperty property in byCode.EnumerateObject())
				{
					if (!int.TryParse(property.Name, out int code))
					{
						continue;
					}
					foreach (string key in ReadStrings(property.Value))
					{
						if (catalog.ByPath.TryGetValue(NormalizeKey(key), out PvfSkillDataParameterSkill skill) ||
							catalog.ByPath.TryGetValue(RemoveSkillPrefix(NormalizeKey(key)), out skill))
						{
							AddCode(catalog, code, skill);
						}
					}
				}
			}
		}
		catch (JsonException)
		{
			return new Catalog();
		}
		catch (IOException)
		{
			return new Catalog();
		}
		return catalog;
	}

	private static PvfSkillDataParameterSkill ParseSkill(string rawKey, JsonElement element)
	{
		if (element.ValueKind != JsonValueKind.Object)
		{
			return null;
		}
		string key = RemoveSkillPrefix(NormalizeKey(rawKey));
		if (key.Length == 0)
		{
			return null;
		}
		PvfSkillDataParameterSkill skill = new()
		{
			Key = key,
			Name = element.TryGetProperty("name", out JsonElement name) && name.ValueKind == JsonValueKind.String ? name.GetString() : string.Empty
		};
		if (!element.TryGetProperty("scenes", out JsonElement scenes) || scenes.ValueKind != JsonValueKind.Object)
		{
			return skill;
		}
		foreach (JsonProperty scene in scenes.EnumerateObject())
		{
			if (scene.Value.ValueKind != JsonValueKind.Object)
			{
				continue;
			}
			PvfSkillDataParameterLabels labels = new();
			ReadLabelMap(scene.Value, "levelInfo", labels.LevelInfo);
			ReadLabelMap(scene.Value, "staticData", labels.StaticData);
			skill.Scenes[scene.Name] = labels;
		}
		return skill;
	}

	private static void ReadLabelMap(JsonElement scene, string propertyName, Dictionary<int, List<string>> target)
	{
		if (!scene.TryGetProperty(propertyName, out JsonElement map) || map.ValueKind != JsonValueKind.Object)
		{
			return;
		}
		foreach (JsonProperty property in map.EnumerateObject())
		{
			if (!int.TryParse(property.Name, out int index) || index < 0)
			{
				continue;
			}
			List<string> labels = ReadStrings(property.Value).Select(value => value.Trim()).Where(value => value.Length > 0).Distinct().ToList();
			if (labels.Count > 0)
			{
				target[index] = labels;
			}
		}
	}

	private static IEnumerable<string> ReadStrings(JsonElement value)
	{
		if (value.ValueKind == JsonValueKind.String)
		{
			yield return value.GetString();
			yield break;
		}
		if (value.ValueKind != JsonValueKind.Array)
		{
			yield break;
		}
		foreach (JsonElement item in value.EnumerateArray())
		{
			if (item.ValueKind == JsonValueKind.String)
			{
				yield return item.GetString();
			}
		}
	}

	private static void AddCode(Catalog catalog, int code, PvfSkillDataParameterSkill skill)
	{
		if (!catalog.ByCode.TryGetValue(code, out List<PvfSkillDataParameterSkill> skills))
		{
			skills = new List<PvfSkillDataParameterSkill>();
			catalog.ByCode[code] = skills;
		}
		if (!skills.Contains(skill))
		{
			skills.Add(skill);
		}
	}

	private static string NormalizeKey(string value)
	{
		return (value ?? string.Empty).Replace('\\', '/').Trim('/').ToLowerInvariant();
	}

	private static string RemoveSkillPrefix(string value)
	{
		return value.StartsWith("skill/", StringComparison.OrdinalIgnoreCase) ? value.Substring("skill/".Length) : value;
	}
}
