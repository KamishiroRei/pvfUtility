using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PvfCode.AiAssistant;

public sealed class AiAssistantSettingsStore
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		WriteIndented = true
	};

	private readonly string _settingsPath;

	public AiAssistantSettingsStore(string? settingsPath = null)
	{
		_settingsPath = settingsPath ?? Path.Combine(AppSetting.AppBasePath, "AiAssistant.json");
	}

	public AiAssistantConnection Load()
	{
		PersistedSettings settings = new();
		try
		{
			if (File.Exists(_settingsPath))
			{
				settings = JsonSerializer.Deserialize<PersistedSettings>(File.ReadAllText(_settingsPath), JsonOptions) ?? new PersistedSettings();
			}
		}
		catch
		{
			settings = new PersistedSettings();
		}

		return new AiAssistantConnection
		{
			Endpoint = FirstNonEmpty(Environment.GetEnvironmentVariable("OPENAI_BASE_URL"), settings.Endpoint, AiAssistantConnection.DefaultEndpoint),
			Model = FirstNonEmpty(Environment.GetEnvironmentVariable("OPENAI_MODEL"), settings.Model, AiAssistantConnection.DefaultModel),
			ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty,
			MaxOutputTokens = settings.MaxOutputTokens is > 0 and <= 131072 ? settings.MaxOutputTokens : 2048
		};
	}

	public async Task SaveAsync(AiAssistantConnection connection, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(connection);
		_ = connection.GetBaseUri();
		if (string.IsNullOrWhiteSpace(connection.Model))
		{
			throw new InvalidOperationException("模型名称不能为空。");
		}
		if (connection.MaxOutputTokens is < 1 or > 131072)
		{
			throw new InvalidOperationException("最大输出 Token 必须在 1 到 131072 之间。");
		}

		string? directory = Path.GetDirectoryName(_settingsPath);
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}

		PersistedSettings settings = new()
		{
			Endpoint = connection.GetBaseUri().AbsoluteUri,
			Model = connection.Model.Trim(),
			MaxOutputTokens = connection.MaxOutputTokens
		};
		string json = JsonSerializer.Serialize(settings, JsonOptions);
		await File.WriteAllTextAsync(_settingsPath, json, cancellationToken).ConfigureAwait(false);
	}

	private static string FirstNonEmpty(params string?[] values)
	{
		foreach (string? value in values)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				return value.Trim();
			}
		}
		return string.Empty;
	}

	private sealed class PersistedSettings
	{
		public string Endpoint { get; set; } = AiAssistantConnection.DefaultEndpoint;

		public string Model { get; set; } = AiAssistantConnection.DefaultModel;

		public int MaxOutputTokens { get; set; } = 2048;
	}
}
