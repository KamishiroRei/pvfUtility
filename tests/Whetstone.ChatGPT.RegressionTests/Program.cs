using System.Net;
using System.Text;
using System.Text.Json;
using Whetstone.ChatGPT;
using Whetstone.ChatGPT.Models;

await RunAsync(
    "configured base address, authorization, and request JSON",
    ConfiguredBaseAddressAndRequestJsonAsync);
await RunAsync(
    "disposing the wrapper preserves an injected HttpClient",
    DisposingWrapperPreservesInjectedHttpClientAsync);
await RunAsync(
    "an already-used injected HttpClient keeps its configured base address",
    UsedInjectedHttpClientKeepsConfiguredBaseAddressAsync);
await RunAsync(
    "tool requests and tool-call responses round-trip",
    ToolRequestsAndResponsesRoundTripAsync);
await RunAsync(
    "a parameterless tool serializes an empty parameter schema",
    ParameterlessToolSerializesEmptySchemaAsync);
await RunAsync(
    "plain-text HTTP failures preserve status without exposing response content",
    PlainTextFailuresPreserveStatusWithoutExposingContentAsync);
await RunAsync(
    "unknown-length responses honor the configured buffer limit",
    UnknownLengthResponsesHonorConfiguredBufferLimitAsync);
await RunAsync(
    "the default base address remains the official v1 endpoint",
    DefaultBaseAddressRemainsOfficialV1Async);
await RunAsync(
    "cancellation reaches an in-flight HTTP request",
    CancellationReachesInFlightRequestAsync);

Console.WriteLine("Whetstone.ChatGPT regression tests passed.");

static async Task ConfiguredBaseAddressAndRequestJsonAsync()
{
    const string responseJson =
        """
        {
          "id": "chatcmpl-test",
          "object": "chat.completion",
          "created": 1710000000,
          "model": "provider-model",
          "choices": [
            {
              "index": 0,
              "message": { "role": "assistant", "content": "ok" },
              "finish_reason": "stop"
            }
          ]
        }
        """;
    var handler = new RecordingHandler((_, _) => Task.FromResult(JsonResponse(responseJson)));
    using var httpClient = new HttpClient(handler, disposeHandler: false);
    using var client = new ChatGPTClient(
        new ChatGPTCredentials("test-secret"),
        httpClient,
        new Uri("https://gateway.example.test/openai/v1"));

    ChatGPTChatCompletionResponse? response = await client.CreateChatCompletionAsync(CreateRequest());

    AssertEqual(
        "https://gateway.example.test/openai/v1/chat/completions",
        handler.RequestUri?.AbsoluteUri,
        "configured base address");
    AssertEqual("Bearer", handler.AuthorizationScheme, "authorization scheme");
    AssertEqual("test-secret", handler.AuthorizationParameter, "authorization parameter");
    AssertEqual("ok", response?.GetCompletionText(), "response text");

    using JsonDocument requestJson = JsonDocument.Parse(handler.RequestBody ?? string.Empty);
    JsonElement root = requestJson.RootElement;
    AssertEqual("provider-model", root.GetProperty("model").GetString(), "request model");
    AssertEqual("user", root.GetProperty("messages")[0].GetProperty("role").GetString(), "message role");
    AssertEqual("hello", root.GetProperty("messages")[0].GetProperty("content").GetString(), "message content");
    AssertFalse(root.TryGetProperty("max_tokens", out _), "default max_tokens must be omitted");
}

static async Task DisposingWrapperPreservesInjectedHttpClientAsync()
{
    var handler = new RecordingHandler((_, _) => Task.FromResult(JsonResponse("{}")));
    using var httpClient = new HttpClient(handler);
    var client = new ChatGPTClient(new ChatGPTCredentials("test-secret"), httpClient);

    client.Dispose();

    using HttpResponseMessage response = await httpClient.GetAsync("https://health.example.test/");
    AssertEqual(HttpStatusCode.OK, response.StatusCode, "injected HttpClient remains usable");
    AssertFalse(handler.IsDisposed, "injected HttpClient handler remains owned by its caller");
}

static async Task UsedInjectedHttpClientKeepsConfiguredBaseAddressAsync()
{
    var handler = new RecordingHandler((_, _) => Task.FromResult(JsonResponse("{}")));
    using var httpClient = new HttpClient(handler, disposeHandler: false)
    {
        BaseAddress = new Uri("https://reused.example.test/openai/v1/")
    };
    using HttpResponseMessage warmupResponse = await httpClient.GetAsync("https://reused.example.test/health");
    using var client = new ChatGPTClient(new ChatGPTCredentials("test-secret"), httpClient);

    await client.CreateChatCompletionAsync(CreateRequest());

    AssertEqual(
        "https://reused.example.test/openai/v1/chat/completions",
        handler.RequestUri?.AbsoluteUri,
        "reused HttpClient base address");
}

static async Task ToolRequestsAndResponsesRoundTripAsync()
{
    const string responseJson =
        """
        {
          "id": "chatcmpl-tool-test",
          "object": "chat.completion",
          "created": 1710000000,
          "model": "provider-model",
          "choices": [
            {
              "index": 0,
              "message": {
                "role": "assistant",
                "content": null,
                "tool_calls": [
                  {
                    "id": "call_response",
                    "type": "function",
                    "function": {
                      "name": "lookup_item",
                      "arguments": "{\"itemId\":42}"
                    }
                  }
                ]
              },
              "finish_reason": "tool_calls"
            }
          ]
        }
        """;
    var handler = new RecordingHandler((_, _) => Task.FromResult(JsonResponse(responseJson)));
    using var httpClient = new HttpClient(handler, disposeHandler: false);
    using var client = new ChatGPTClient(new ChatGPTCredentials("test-secret"), httpClient);
    using JsonDocument parameterDocument = JsonDocument.Parse(
        """
        {
          "type": "object",
          "properties": {
            "itemId": { "type": "integer" }
          },
          "required": ["itemId"]
        }
        """);

    var request = new ChatGPTChatCompletionRequest
    {
        Model = "provider-model",
        ToolChoice = "auto",
        Tools = new List<ChatGPTTool>
        {
            new()
            {
                Function = new ChatGPTFunctionDefinition
                {
                    Name = "lookup_item",
                    Description = "Look up a PVF item.",
                    Parameters = parameterDocument.RootElement.Clone()
                }
            }
        },
        Messages = new List<ChatGPTChatCompletionMessage>
        {
            new()
            {
                Role = MessageRole.User,
                Content = "Find item 42"
            },
            new()
            {
                Role = MessageRole.Assistant,
                ToolCalls = new List<ChatGPTToolCall>
                {
                    new()
                    {
                        Id = "call_request",
                        Function = new ChatGPTFunctionCall
                        {
                            Name = "lookup_item",
                            Arguments = "{\"itemId\":42}"
                        }
                    }
                }
            },
            new()
            {
                Role = MessageRole.Tool,
                ToolCallId = "call_request",
                Content = "{\"name\":\"Example item\"}"
            }
        }
    };

    ChatGPTChatCompletionResponse? response = await client.CreateChatCompletionAsync(request);

    using JsonDocument requestJson = JsonDocument.Parse(handler.RequestBody ?? string.Empty);
    JsonElement root = requestJson.RootElement;
    AssertEqual("auto", root.GetProperty("tool_choice").GetString(), "tool choice");
    JsonElement requestTool = root.GetProperty("tools")[0];
    AssertEqual("function", requestTool.GetProperty("type").GetString(), "tool type");
    AssertEqual("lookup_item", requestTool.GetProperty("function").GetProperty("name").GetString(), "tool name");
    AssertEqual(
        "integer",
        requestTool.GetProperty("function").GetProperty("parameters")
            .GetProperty("properties").GetProperty("itemId").GetProperty("type").GetString(),
        "tool parameter schema");
    JsonElement assistantMessage = root.GetProperty("messages")[1];
    AssertEqual("call_request", assistantMessage.GetProperty("tool_calls")[0].GetProperty("id").GetString(), "assistant tool-call id");
    JsonElement toolMessage = root.GetProperty("messages")[2];
    AssertEqual("tool", toolMessage.GetProperty("role").GetString(), "tool message role");
    AssertEqual("call_request", toolMessage.GetProperty("tool_call_id").GetString(), "tool response id");

    ChatGPTChatCompletionMessage? responseMessage = response?.GetMessage();
    AssertEqual(MessageRole.Assistant, responseMessage?.Role, "response role");
    AssertEqual("call_response", responseMessage?.ToolCalls?[0].Id, "response tool-call id");
    AssertEqual("function", responseMessage?.ToolCalls?[0].Type, "response tool-call type");
    AssertEqual("lookup_item", responseMessage?.ToolCalls?[0].Function?.Name, "response function name");
    AssertEqual("{\"itemId\":42}", responseMessage?.ToolCalls?[0].Function?.Arguments, "response function arguments");
}

static async Task ParameterlessToolSerializesEmptySchemaAsync()
{
    var handler = new RecordingHandler((_, _) => Task.FromResult(JsonResponse("{}")));
    using var httpClient = new HttpClient(handler, disposeHandler: false);
    using var client = new ChatGPTClient(new ChatGPTCredentials("test-secret"), httpClient);
    ChatGPTChatCompletionRequest request = CreateRequest();
    request.Tools = new List<ChatGPTTool>
    {
        new()
        {
            Function = new ChatGPTFunctionDefinition
            {
                Name = "get_current_context"
            }
        }
    };

    await client.CreateChatCompletionAsync(request);

    using JsonDocument requestJson = JsonDocument.Parse(handler.RequestBody ?? string.Empty);
    JsonElement parameters = requestJson.RootElement.GetProperty("tools")[0]
        .GetProperty("function").GetProperty("parameters");
    AssertEqual(JsonValueKind.Object, parameters.ValueKind, "parameterless tool schema kind");
    AssertFalse(parameters.EnumerateObject().Any(), "parameterless tool schema must be empty");
}

static async Task PlainTextFailuresPreserveStatusWithoutExposingContentAsync()
{
    const string responseBody = "upstream unavailable; test-secret must not escape";
    var handler = new RecordingHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadGateway)
    {
        Content = new StringContent(responseBody, Encoding.UTF8, "text/plain")
    }));
    using var httpClient = new HttpClient(handler, disposeHandler: false);
    using var client = new ChatGPTClient(new ChatGPTCredentials("test-secret"), httpClient);

    ChatGPTException exception = await AssertThrowsAsync<ChatGPTException>(
        () => client.CreateChatCompletionAsync(CreateRequest()),
        "plain-text HTTP error");

    AssertEqual(HttpStatusCode.BadGateway, exception.StatusCode, "HTTP error status");
    AssertFalse(
        exception.Message.Contains("test-secret", StringComparison.Ordinal),
        "HTTP error must not expose response content");
}

static async Task UnknownLengthResponsesHonorConfiguredBufferLimitAsync()
{
    var handler = new RecordingHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StreamContent(new RepeatingReadStream(65))
    }));
    using var httpClient = new HttpClient(handler, disposeHandler: false);
    using var client = new ChatGPTClient(
        new ChatGPTCredentials("test-secret"),
        httpClient,
        new Uri("https://api.openai.com/v1/"),
        maximumResponseBytes: 64);

    await AssertThrowsAsync<ChatGPTResponseTooLargeException>(
        () => client.CreateChatCompletionAsync(CreateRequest()),
        "unknown-length response limit");
}

static async Task DefaultBaseAddressRemainsOfficialV1Async()
{
    var handler = new RecordingHandler((_, _) => Task.FromResult(JsonResponse("{}")));
    using var httpClient = new HttpClient(handler, disposeHandler: false);
    using var client = new ChatGPTClient(new ChatGPTCredentials("test-secret"), httpClient);

    await client.CreateChatCompletionAsync(CreateRequest());

    AssertEqual(
        "https://api.openai.com/v1/chat/completions",
        handler.RequestUri?.AbsoluteUri,
        "default base address");
}

static async Task CancellationReachesInFlightRequestAsync()
{
    var requestStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    var handler = new RecordingHandler(async (_, cancellationToken) =>
    {
        requestStarted.TrySetResult(true);
        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        return JsonResponse("{}");
    });
    using var httpClient = new HttpClient(handler, disposeHandler: false);
    using var client = new ChatGPTClient(new ChatGPTCredentials("test-secret"), httpClient);
    using var cancellationSource = new CancellationTokenSource();

    Task<ChatGPTChatCompletionResponse?> requestTask =
        client.CreateChatCompletionAsync(CreateRequest(), cancellationSource.Token);
    await requestStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
    cancellationSource.Cancel();

    await AssertThrowsAsync<OperationCanceledException>(
        () => requestTask,
        "in-flight cancellation");
}

static ChatGPTChatCompletionRequest CreateRequest()
{
    return new ChatGPTChatCompletionRequest
    {
        Model = "provider-model",
        Messages = new List<ChatGPTChatCompletionMessage>
        {
            new()
            {
                Role = MessageRole.User,
                Content = "hello"
            }
        }
    };
}

static HttpResponseMessage JsonResponse(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
{
    return new HttpResponseMessage(statusCode)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
    };
}

static async Task RunAsync(string scenario, Func<Task> test)
{
    try
    {
        await test();
        Console.WriteLine($"PASS: {scenario}");
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"FAIL: {scenario}", ex);
    }
}

static void AssertEqual<T>(T expected, T actual, string scenario)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new InvalidOperationException(
            $"{scenario} failed. Expected: {expected}; Actual: {actual}");
    }
}

static void AssertFalse(bool actual, string scenario)
{
    if (actual)
    {
        throw new InvalidOperationException($"{scenario} failed. Expected false; Actual: true");
    }
}

static async Task<TException> AssertThrowsAsync<TException>(Func<Task> action, string scenario)
    where TException : Exception
{
    try
    {
        await action();
    }
    catch (TException exception)
    {
        return exception;
    }
    catch (Exception exception)
    {
        throw new InvalidOperationException(
            $"{scenario} failed. Expected {typeof(TException).Name}; Actual: {exception.GetType().Name}",
            exception);
    }

    throw new InvalidOperationException(
        $"{scenario} failed. Expected {typeof(TException).Name}; no exception was thrown.");
}

sealed class RecordingHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _respond;

    public RecordingHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond)
    {
        _respond = respond;
    }

    public Uri? RequestUri { get; private set; }

    public string? AuthorizationScheme { get; private set; }

    public string? AuthorizationParameter { get; private set; }

    public string? RequestBody { get; private set; }

    public bool IsDisposed { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        RequestUri = request.RequestUri;
        AuthorizationScheme = request.Headers.Authorization?.Scheme;
        AuthorizationParameter = request.Headers.Authorization?.Parameter;
        RequestBody = request.Content == null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);
        return await _respond(request, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            IsDisposed = true;
        }
        base.Dispose(disposing);
    }
}

sealed class RepeatingReadStream : Stream
{
    private long _remaining;

    public RepeatingReadStream(long length)
    {
        _remaining = length;
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int read = (int)Math.Min(count, _remaining);
        Array.Clear(buffer, offset, read);
        _remaining -= read;
        return read;
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        int read = (int)Math.Min(buffer.Length, _remaining);
        buffer.Span[..read].Clear();
        _remaining -= read;
        return ValueTask.FromResult(read);
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
