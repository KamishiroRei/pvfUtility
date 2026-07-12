using System;
using System.Linq;

namespace PvfCode.AiAssistant;

public sealed class AiAssistantConnection
{
    public const string DefaultEndpoint = "https://api.openai.com/v1/";

    public const string DefaultModel = "gpt-5.6-sol";

    public string Endpoint { get; set; } = DefaultEndpoint;

    public string Model { get; set; } = DefaultModel;

    public string ApiKey { get; set; } = string.Empty;

    public int MaxOutputTokens { get; set; } = 2048;

    public Uri GetBaseUri()
    {
        if (!Uri.TryCreate(Endpoint?.Trim(), UriKind.Absolute, out Uri? endpoint) ||
            (endpoint.Scheme != Uri.UriSchemeHttps && endpoint.Scheme != Uri.UriSchemeHttp))
        {
            throw new InvalidOperationException("API 地址必须是有效的 HTTP 或 HTTPS 绝对地址。");
        }
        if (endpoint.Scheme == Uri.UriSchemeHttp && !endpoint.IsLoopback)
        {
            throw new InvalidOperationException("远程 API 地址必须使用 HTTPS；HTTP 仅允许本机回环地址。");
        }
        if (!string.IsNullOrEmpty(endpoint.UserInfo) || !string.IsNullOrEmpty(endpoint.Query) || !string.IsNullOrEmpty(endpoint.Fragment))
        {
            throw new InvalidOperationException("API 地址不能包含用户信息、查询参数或片段。");
        }

        string normalized = endpoint.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)
            ? endpoint.AbsoluteUri
            : endpoint.AbsoluteUri + "/";
        return new Uri(normalized, UriKind.Absolute);
    }

    public void Validate()
    {
        _ = GetBaseUri();
        if (string.IsNullOrWhiteSpace(Model))
        {
            throw new InvalidOperationException("模型名称不能为空。");
        }
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            throw new InvalidOperationException("API Key 不能为空。");
        }
        if (ApiKey.Any(char.IsControl))
        {
            throw new InvalidOperationException("API Key 不能包含控制字符。");
        }
        if (MaxOutputTokens is < 1 or > 131072)
        {
            throw new InvalidOperationException("最大输出 Token 必须在 1 到 131072 之间。");
        }
    }
}
