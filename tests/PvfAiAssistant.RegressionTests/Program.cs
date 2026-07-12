using System.Text.Json;
using PvfCode;
using PvfCode.AiAssistant;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;

TestConnectionValidation();
await TestToolLoopAsync();
await TestUnknownToolIsRejectedAsync();
await TestToolFailureIsContainedAsync();
await TestToolCallLimitAsync();
await TestToolResultLimitAsync();
await TestConversationLimitsAsync();
await TestPvfToolCatalogAsync();
await TestKnowledgeSearchAsync();
await TestKnowledgeRoutingBoundaryAsync();
await TestKnowledgeReparseBoundaryAsync();
await TestMigratedKnowledgePackAsync();
await TestHttpGatewayAsync();
await TestHttpGatewayRejectsNonAssistantRoleAsync();
await TestHttpGatewayErrorAsync();
await TestHttpGatewayResponseLimitAsync();
await TestSettingsDoNotPersistKeyAsync();

Console.WriteLine("PVF AI assistant regression tests passed.");

static void TestConnectionValidation()
{
    AiAssistantConnection connection = CreateConnection();
    connection.Endpoint = "https://example.test/v1";
    AssertEqual("https://example.test/v1/", connection.GetBaseUri().AbsoluteUri, "base URI normalization");

    connection.Endpoint = "file:///tmp/openai";
    AssertThrows<InvalidOperationException>(connection.Validate, "non-HTTP endpoint rejection");

    connection = CreateConnection();
    connection.Endpoint = "http://example.test/v1/";
    AssertThrows<InvalidOperationException>(connection.Validate, "remote clear-text endpoint rejection");

    connection.Endpoint = "http://127.0.0.1:1234/v1/";
    connection.Validate();

    connection = CreateConnection();
    connection.Endpoint = "https://user:password@example.test/v1/";
    AssertThrows<InvalidOperationException>(connection.Validate, "endpoint user info rejection");

    connection = CreateConnection();
    connection.Endpoint = "https://example.test/v1/?tenant=one";
    AssertThrows<InvalidOperationException>(connection.Validate, "endpoint query rejection");

    connection = CreateConnection();
    connection.Endpoint = "https://example.test/v1/#fragment";
    AssertThrows<InvalidOperationException>(connection.Validate, "endpoint fragment rejection");

    connection = CreateConnection();
    connection.ApiKey = string.Empty;
    AssertThrows<InvalidOperationException>(connection.Validate, "missing API key rejection");

    connection = CreateConnection();
    connection.ApiKey = "invalid\rkey";
    AssertThrows<InvalidOperationException>(connection.Validate, "API key control character rejection");
}

static async Task TestHttpGatewayAsync()
{
    RecordingHandler handler = new(
        "{\"choices\":[{\"message\":{\"role\":\"assistant\",\"content\":null,\"tool_calls\":[{\"id\":\"call-http\",\"type\":\"function\",\"function\":{\"name\":\"pvf_current_context\",\"arguments\":\"{}\"}}]}}]}");
    using HttpClient client = new(handler);
    OpenAiCompatibleChatGateway gateway = new(client);
    using JsonDocument schema = JsonDocument.Parse("{\"type\":\"object\",\"properties\":{}}");

    AiCompletionMessage response = await gateway.CompleteAsync(
        CreateConnection(),
        [new AiCompletionMessage(AiCompletionRole.User, "status")],
        [new AiToolDefinition("pvf_current_context", "Current PVF context.", schema.RootElement.Clone())],
        CancellationToken.None);

    AssertEqual("https://example.test/v1/chat/completions", handler.RequestUri?.AbsoluteUri, "chat endpoint URI");
    AssertEqual("Bearer", handler.AuthorizationScheme, "authorization scheme");
    AssertEqual("test-key", handler.AuthorizationParameter, "authorization parameter");
    AssertEqual("pvf_current_context", response.ToolCalls?.Single().Name, "tool call response parsing");
    using JsonDocument request = JsonDocument.Parse(handler.RequestBody ?? string.Empty);
    AssertEqual("test-model", request.RootElement.GetProperty("model").GetString(), "request model");
    AssertEqual("auto", request.RootElement.GetProperty("tool_choice").GetString(), "tool choice request");
}

static async Task TestHttpGatewayRejectsNonAssistantRoleAsync()
{
    RecordingHandler handler = new(
        "{\"choices\":[{\"message\":{\"role\":\"user\",\"content\":\"unexpected\"}}]}");
    using HttpClient client = new(handler);
    OpenAiCompatibleChatGateway gateway = new(client);

    InvalidOperationException exception = await AssertThrowsAsync<InvalidOperationException>(
        () => gateway.CompleteAsync(
            CreateConnection(),
            [new AiCompletionMessage(AiCompletionRole.User, "status")],
            Array.Empty<AiToolDefinition>(),
            CancellationToken.None),
        "non-assistant response role rejection");
    AssertEqual(
        "API 返回了无法识别的 Chat Completions 响应。",
        exception.Message,
        "non-assistant response role message");
}

static async Task TestHttpGatewayErrorAsync()
{
    RecordingHandler handler = new(
        "{\"error\":{\"message\":\"invalid credentials test-key\"}}",
        System.Net.HttpStatusCode.Unauthorized);
    using HttpClient client = new(handler);
    OpenAiCompatibleChatGateway gateway = new(client);
    AiAssistantConnection connection = CreateConnection();

    try
    {
        await gateway.CompleteAsync(
            connection,
            [new AiCompletionMessage(AiCompletionRole.User, "status")],
            Array.Empty<AiToolDefinition>(),
            CancellationToken.None);
    }
    catch (HttpRequestException ex)
    {
        AssertEqual(System.Net.HttpStatusCode.Unauthorized.ToString(), ex.StatusCode?.ToString(), "HTTP error status");
        AssertTrue(ex.Message.Contains("invalid credentials", StringComparison.Ordinal), "HTTP API error message");
        AssertFalse(ex.Message.Contains(connection.ApiKey, StringComparison.Ordinal), "HTTP error key redaction");
        return;
    }
    throw new InvalidOperationException("HTTP error handling failed: expected HttpRequestException.");
}

static async Task TestHttpGatewayResponseLimitAsync()
{
    using HttpClient client = new(new OversizedResponseHandler());
    OpenAiCompatibleChatGateway gateway = new(client);

    InvalidOperationException exception = await AssertThrowsAsync<InvalidOperationException>(
        () => gateway.CompleteAsync(
            CreateConnection(),
            [new AiCompletionMessage(AiCompletionRole.User, "status")],
            Array.Empty<AiToolDefinition>(),
            CancellationToken.None),
        "HTTP response size bound");
    AssertEqual("API 响应超过 2 MiB 安全上限。", exception.Message, "HTTP response limit message");
}

static async Task TestSettingsDoNotPersistKeyAsync()
{
    string root = Path.Combine(Path.GetTempPath(), "pvf-ai-settings-tests-" + Guid.NewGuid().ToString("N"));
    string path = Path.Combine(root, "AiAssistant.json");
    string? originalKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    string? originalBaseUrl = Environment.GetEnvironmentVariable("OPENAI_BASE_URL");
    string? originalModel = Environment.GetEnvironmentVariable("OPENAI_MODEL");
    try
    {
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", null);
        Environment.SetEnvironmentVariable("OPENAI_BASE_URL", null);
        Environment.SetEnvironmentVariable("OPENAI_MODEL", null);
        AiAssistantSettingsStore store = new(path);
        AiAssistantConnection connection = CreateConnection();
        connection.ApiKey = "key-must-never-be-persisted";

        await store.SaveAsync(connection);

        string json = await File.ReadAllTextAsync(path);
        AssertFalse(json.Contains(connection.ApiKey, StringComparison.Ordinal), "API key persistence");
        AiAssistantConnection loaded = store.Load();
        AssertEqual(string.Empty, loaded.ApiKey, "session-only API key");
        AssertEqual(connection.Endpoint, loaded.Endpoint, "persisted endpoint");
        AssertEqual(connection.Model, loaded.Model, "persisted model");
    }
    finally
    {
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", originalKey);
        Environment.SetEnvironmentVariable("OPENAI_BASE_URL", originalBaseUrl);
        Environment.SetEnvironmentVariable("OPENAI_MODEL", originalModel);
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }
}

static async Task TestToolLoopAsync()
{
    QueueGateway gateway = new(
        new AiCompletionMessage(
            AiCompletionRole.Assistant,
            null,
            [new AiToolCall("call-1", "echo", "{\"value\":\"pvf\"}")]),
        new AiCompletionMessage(AiCompletionRole.Assistant, "完成"));
    AiAssistantAgent agent = new(gateway);
    AiAssistantTool tool = new(
        "echo",
        "Echo a value.",
        "{\"type\":\"object\",\"required\":[\"value\"],\"properties\":{\"value\":{\"type\":\"string\"}}}",
        (arguments, _) => Task.FromResult<object?>(new { value = arguments.GetProperty("value").GetString() }));

    AiAgentReply reply = await agent.ReplyAsync(
        CreateConnection(),
        [new AiCompletionMessage(AiCompletionRole.User, "测试")],
        [tool],
        CancellationToken.None);

    AssertEqual("完成", reply.Content, "final assistant response");
    AssertEqual("echo", reply.ToolTraces.Single().ToolName, "tool trace name");
    AssertTrue(reply.ToolTraces.Single().Succeeded, "tool trace success");
    AiCompletionMessage toolResult = gateway.Requests[1].Single(message => message.Role == AiCompletionRole.Tool);
    AssertEqual("call-1", toolResult.ToolCallId, "tool call correlation");
    AssertTrue(toolResult.Content?.Contains("pvf", StringComparison.Ordinal) == true, "tool result content");
    string systemPrompt = gateway.Requests[0].Single(message => message.Role == AiCompletionRole.System).Content ?? string.Empty;
    AssertTrue(systemPrompt.Contains("pvf-bridge", StringComparison.Ordinal), "unavailable historical tool boundary");
}

static async Task TestUnknownToolIsRejectedAsync()
{
    QueueGateway gateway = new(
        new AiCompletionMessage(
            AiCompletionRole.Assistant,
            null,
            [new AiToolCall("call-2", "run_process", "{}")]),
        new AiCompletionMessage(AiCompletionRole.Assistant, "已拒绝"));
    AiAssistantAgent agent = new(gateway);

    AiAgentReply reply = await agent.ReplyAsync(
        CreateConnection(),
        [new AiCompletionMessage(AiCompletionRole.User, "执行命令")],
        Array.Empty<IAiAssistantTool>(),
        CancellationToken.None);

    AssertEqual("已拒绝", reply.Content, "unknown tool final response");
    AssertFalse(reply.ToolTraces.Single().Succeeded, "unknown tool rejected");
    string result = gateway.Requests[1].Single(message => message.Role == AiCompletionRole.Tool).Content ?? string.Empty;
    using JsonDocument json = JsonDocument.Parse(result);
    AssertTrue(json.RootElement.TryGetProperty("error", out _), "unknown tool error result");
}

static async Task TestToolFailureIsContainedAsync()
{
    QueueGateway gateway = new(
        new AiCompletionMessage(
            AiCompletionRole.Assistant,
            null,
            [new AiToolCall("call-3", "failing_tool", "{}")]),
        new AiCompletionMessage(AiCompletionRole.Assistant, "工具失败已处理"));
    AiAssistantAgent agent = new(gateway);
    AiAssistantTool tool = new(
        "failing_tool",
        "Always fails.",
        "{\"type\":\"object\",\"properties\":{}}",
        (_, _) => throw new NullReferenceException("internal implementation detail"));

    AiAgentReply reply = await agent.ReplyAsync(
        CreateConnection(),
        [new AiCompletionMessage(AiCompletionRole.User, "调用")],
        [tool],
        CancellationToken.None);

    AssertEqual("工具失败已处理", reply.Content, "failed tool continuation");
    AssertFalse(reply.ToolTraces.Single().Succeeded, "failed tool trace");
    string result = gateway.Requests[1].Single(message => message.Role == AiCompletionRole.Tool).Content ?? string.Empty;
    AssertFalse(result.Contains("internal implementation detail", StringComparison.Ordinal), "internal tool error redaction");
}

static async Task TestToolCallLimitAsync()
{
    AiToolCall[] calls = Enumerable.Range(1, 5)
        .Select(index => new AiToolCall($"call-{index}", "unknown", "{}"))
        .ToArray();
    QueueGateway gateway = new(new AiCompletionMessage(AiCompletionRole.Assistant, null, calls));
    AiAssistantAgent agent = new(gateway);

    await AssertThrowsAsync<InvalidOperationException>(
        () => agent.ReplyAsync(
            CreateConnection(),
            [new AiCompletionMessage(AiCompletionRole.User, "调用太多工具")],
            Array.Empty<IAiAssistantTool>(),
            CancellationToken.None),
        "per-round tool call bound");
    AssertIntEqual(1, gateway.Requests.Count, "tool limit stops before a continuation request");
}

static async Task TestToolResultLimitAsync()
{
    QueueGateway gateway = new(new AiCompletionMessage(
        AiCompletionRole.Assistant,
        null,
        [
            new AiToolCall("large-1", "large_result", "{}"),
            new AiToolCall("large-2", "large_result", "{}")
        ]));
    AiAssistantAgent agent = new(gateway);
    AiAssistantTool tool = new(
        "large_result",
        "Return a large bounded test result.",
        "{\"type\":\"object\",\"properties\":{}}",
        (_, _) => Task.FromResult<object?>(new string('x', 89990)));

    await AssertThrowsAsync<InvalidOperationException>(
        () => agent.ReplyAsync(
            CreateConnection(),
            [new AiCompletionMessage(AiCompletionRole.User, "读取大量结果")],
            [tool],
            CancellationToken.None),
        "cumulative tool result character bound");
    AssertIntEqual(1, gateway.Requests.Count, "tool result limit stops before continuation");
}

static async Task TestConversationLimitsAsync()
{
    QueueGateway gateway = new(new AiCompletionMessage(AiCompletionRole.Assistant, "unused"));
    AiAssistantAgent agent = new(gateway);

    await AssertThrowsAsync<InvalidOperationException>(
        () => agent.ReplyAsync(
            CreateConnection(),
            [new AiCompletionMessage(AiCompletionRole.User, new string('x', 32001))],
            Array.Empty<IAiAssistantTool>(),
            CancellationToken.None),
        "single conversation message limit");
    AssertIntEqual(0, gateway.Requests.Count, "single message limit stops before gateway request");

    gateway = new QueueGateway(new AiCompletionMessage(AiCompletionRole.Assistant, "unused"));
    agent = new AiAssistantAgent(gateway);
    AiCompletionMessage[] conversation = Enumerable.Range(0, 5)
        .Select(index => new AiCompletionMessage(
            index % 2 == 0 ? AiCompletionRole.User : AiCompletionRole.Assistant,
            new string((char)('a' + index), 30000)))
        .ToArray();

    await AssertThrowsAsync<InvalidOperationException>(
        () => agent.ReplyAsync(
            CreateConnection(),
            conversation,
            Array.Empty<IAiAssistantTool>(),
            CancellationToken.None),
        "cumulative conversation limit");
    AssertIntEqual(0, gateway.Requests.Count, "conversation limit stops before gateway request");
}

static async Task TestPvfToolCatalogAsync()
{
    string knowledgeRoot = Path.Combine(Path.GetTempPath(), "pvf-ai-tool-tests-" + Guid.NewGuid().ToString("N"));
    try
    {
        Directory.CreateDirectory(knowledgeRoot);
        PvfFile textFile = new()
        {
            FileName = "etc/sample.txt",
            Data = System.Text.Encoding.UTF8.GetBytes("snapshot-before"),
            DataLen = 15,
            Checksum = 1
        };
        PvfFile lstFile = new()
        {
            FileName = "etc/items.lst",
            FileType = PvfFileType.lst,
            IsScriptFile = true,
            Data = new byte[20],
            DataLen = 20,
            LstEntries = new Dictionary<int, LstItem>
            {
                [100] = new LstItem("stackable", "sample.stk", 100)
            }
        };
        PvfFile oversizedFile = new()
        {
            FileName = "oversized/test.txt",
            IsScriptFile = true,
            DataLen = 16 * 1024 * 1024 + 1
        };
        Dictionary<string, PvfFile> sourceFiles = new(StringComparer.OrdinalIgnoreCase)
        {
            [textFile.FileName] = textFile,
            [lstFile.FileName] = lstFile,
            [oversizedFile.FileName] = oversizedFile
        };
        PvfGroup pvf = new()
        {
            PvfIsOpen = true,
            PvfPackFilePath = "sample.pvf",
            FileList = sourceFiles
        };
        pvf.ListFileTable.LstFilePaths["items"] = lstFile.FileName;
        pvf.ListFileTable.CodeDic["items"] = new Dictionary<int, LstItem>
        {
            [100] = new LstItem("stackable", "sample.stk", 100),
            [101] = new LstItem("stackable", "other.stk", 101)
        };
        pvf.ListFileTable.CodeDic["npc"] = new Dictionary<int, LstItem>
        {
            [1] = new LstItem("npc", "sample.npc", 1)
        };

        KnowledgePackService knowledge = new(knowledgeRoot);
        IReadOnlyList<IAiAssistantTool> denied = PvfAssistantToolCatalog.Create(pvf, knowledge, null, allowPvfRead: false);
        AssertIntEqual(1, denied.Count, "PVF tools require explicit permission");
        AssertEqual("knowledge_search", denied[0].Definition.Name, "knowledge tool remains available");

        IReadOnlyList<IAiAssistantTool> allowed = PvfAssistantToolCatalog.Create(pvf, knowledge, "etc/sample.txt", allowPvfRead: true);
        AssertTrue(allowed.Any(tool => tool.Definition.Name == "pvf_read_file"), "PVF read tool exposure");
        IAiAssistantTool readTool = allowed.Single(tool => tool.Definition.Name == "pvf_read_file");
        IAiAssistantTool registryTool = allowed.Single(tool => tool.Definition.Name == "pvf_list_registries");
        IAiAssistantTool contextTool = allowed.Single(tool => tool.Definition.Name == "pvf_current_context");
        IAiAssistantTool searchTool = allowed.Single(tool => tool.Definition.Name == "pvf_search");

        string readResult = await readTool.ExecuteAsync("{\"path\":\"etc/sample.txt\"}", CancellationToken.None);
        using (JsonDocument json = JsonDocument.Parse(readResult))
        {
            AssertEqual("snapshot-before", json.RootElement.GetProperty("content").GetString(), "request file snapshot");
        }

        string registryResult = await registryTool.ExecuteAsync("{\"limit\":1}", CancellationToken.None);
        using (JsonDocument json = JsonDocument.Parse(registryResult))
        {
            JsonElement group = json.RootElement.GetProperty("groups")[0];
            AssertEqual("items", group.GetProperty("name").GetString(), "registry group ordering");
            AssertIntEqual(2, group.GetProperty("count").GetInt32(), "registry group item count");
            AssertTrue(json.RootElement.GetProperty("truncated").GetBoolean(), "registry group truncation");
        }

        await AssertThrowsAsync<ArgumentException>(
            () => readTool.ExecuteAsync($"{{\"path\":\"{new string('a', 1025)}\"}}", CancellationToken.None),
            "PVF path length bound");
        await AssertThrowsAsync<InvalidOperationException>(
            () => readTool.ExecuteAsync("{\"path\":\"oversized/test.txt\"}", CancellationToken.None),
            "single PVF source size bound");

        string oversizedSearchResult = await searchTool.ExecuteAsync(
            "{\"query\":\"sample\",\"mode\":\"content\",\"prefix\":\"oversized/\"}",
            CancellationToken.None);
        using (JsonDocument json = JsonDocument.Parse(oversizedSearchResult))
        {
            AssertTrue(json.RootElement.GetProperty("truncated").GetBoolean(), "oversized content search is marked truncated");
            AssertIntEqual(1, json.RootElement.GetProperty("oversizedCandidatesSkipped").GetInt32(), "oversized content search skip count");
            AssertIntEqual(0, json.RootElement.GetProperty("scanned").GetInt32(), "oversized content search decompile count");
        }

        textFile.Data = System.Text.Encoding.UTF8.GetBytes("live-after");
        textFile.DataLen = textFile.Data.Length;
        textFile.Checksum++;
        textFile.IsUpdated = true;
        await AssertThrowsAsync<InvalidOperationException>(
            () => readTool.ExecuteAsync("{\"path\":\"etc/sample.txt\"}", CancellationToken.None),
            "changed PVF file rejection");

        IReadOnlyList<IAiAssistantTool> dirtyFileSession = PvfAssistantToolCatalog.Create(pvf, knowledge, null, allowPvfRead: true);
        IAiAssistantTool dirtyFileReadTool = dirtyFileSession.Single(tool => tool.Definition.Name == "pvf_read_file");
        textFile.Data[0] ^= 0x01;
        await AssertThrowsAsync<InvalidOperationException>(
            () => dirtyFileReadTool.ExecuteAsync("{\"path\":\"etc/sample.txt\"}", CancellationToken.None),
            "in-place dirty PVF file rejection");

        IReadOnlyList<IAiAssistantTool> semanticSession = PvfAssistantToolCatalog.Create(pvf, knowledge, null, allowPvfRead: true);
        IAiAssistantTool semanticReadTool = semanticSession.Single(tool => tool.Definition.Name == "pvf_read_file");
        pvf.Strtable.IsStringTableUpdated = true;
        await AssertThrowsAsync<InvalidOperationException>(
            () => semanticReadTool.ExecuteAsync("{\"path\":\"etc/sample.txt\"}", CancellationToken.None),
            "changed string table rejection");

        IReadOnlyList<IAiAssistantTool> dirtyStringTableSession = PvfAssistantToolCatalog.Create(pvf, knowledge, null, allowPvfRead: true);
        IAiAssistantTool dirtyStringTableReadTool = dirtyStringTableSession.Single(tool => tool.Definition.Name == "pvf_read_file");
        await AssertThrowsAsync<InvalidOperationException>(
            () => dirtyStringTableReadTool.ExecuteAsync("{\"path\":\"etc/sample.txt\"}", CancellationToken.None),
            "pre-existing dirty string table rejection");

        IReadOnlyList<IAiAssistantTool> switchSession = PvfAssistantToolCatalog.Create(pvf, knowledge, null, allowPvfRead: true);
        contextTool = switchSession.Single(tool => tool.Definition.Name == "pvf_current_context");
        pvf.FileList = new Dictionary<string, PvfFile>(sourceFiles, StringComparer.OrdinalIgnoreCase);
        await AssertThrowsAsync<InvalidOperationException>(
            () => contextTool.ExecuteAsync("{}", CancellationToken.None),
            "switched PVF session rejection");
    }
    finally
    {
        if (Directory.Exists(knowledgeRoot))
        {
            Directory.Delete(knowledgeRoot, recursive: true);
        }
    }
}

static async Task TestKnowledgeSearchAsync()
{
    string root = Path.Combine(Path.GetTempPath(), "pvf-ai-tests-" + Guid.NewGuid().ToString("N"));
    try
    {
        Directory.CreateDirectory(Path.Combine(root, "workflows"));
        Directory.CreateDirectory(Path.Combine(root, "dictionaries"));
        await File.WriteAllTextAsync(Path.Combine(root, "workflows", "npc-shop.zh-CN.md"), "NPC 商店修改必须解析 itemshop.lst。");
        await File.WriteAllTextAsync(Path.Combine(root, "dictionaries", "skill.zh-CN.md"), "技能参数词典。");

        KnowledgePackService failClosedService = new(root);
        IReadOnlyList<KnowledgeSearchResult> failClosedResults = await failClosedService.SearchAsync("NPC 商店", cancellationToken: CancellationToken.None);
        AssertIntEqual(0, failClosedResults.Count, "missing knowledge index fails closed");

        KnowledgePackService service = new(root, allowUnindexedFallback: true);
        IReadOnlyList<KnowledgeSearchResult> results = await service.SearchAsync("NPC 商店", cancellationToken: CancellationToken.None);

        AssertEqual("workflows/npc-shop.zh-CN.md", results[0].Path, "knowledge route");
        AssertTrue(results[0].Content.Contains("itemshop.lst", StringComparison.Ordinal), "knowledge content");
    }
    finally
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }
}

static async Task TestKnowledgeRoutingBoundaryAsync()
{
    string root = Path.Combine(Path.GetTempPath(), "pvf-ai-routing-tests-" + Guid.NewGuid().ToString("N"));
    try
    {
        Directory.CreateDirectory(Path.Combine(root, "indexes"));
        Directory.CreateDirectory(Path.Combine(root, "safety"));
        Directory.CreateDirectory(Path.Combine(root, "workflows"));
        Directory.CreateDirectory(Path.Combine(root, "deep"));
        await File.WriteAllTextAsync(
            Path.Combine(root, "indexes", "knowledge-index.json"),
            "{\"topics\":{\"npc-shop\":{\"entries\":[\"workflows/npc-shop.md\"]}}}");
        await File.WriteAllTextAsync(Path.Combine(root, "safety", "README.zh-CN.md"), "默认只读。");
        await File.WriteAllTextAsync(Path.Combine(root, "workflows", "npc-shop.md"), "NPC 商店受控修改流程。");
        await File.WriteAllTextAsync(Path.Combine(root, "deep", "unrouted.md"), "NPC 商店 NPC 商店 NPC 商店 不应直接读取。");

        KnowledgePackService service = new(root);
        IReadOnlyList<KnowledgeSearchResult> results = await service.SearchAsync("NPC 商店", cancellationToken: CancellationToken.None);

        AssertTrue(results.Any(result => result.Path == "workflows/npc-shop.md"), "compact route hit");
        AssertFalse(results.Any(result => result.Path == "deep/unrouted.md"), "unrouted deep document exclusion");

        await File.WriteAllTextAsync(Path.Combine(root, "indexes", "knowledge-index.json"), "{not-json");
        KnowledgePackService invalidIndexService = new(root, allowUnindexedFallback: true);
        IReadOnlyList<KnowledgeSearchResult> invalidIndexResults = await invalidIndexService.SearchAsync("NPC 商店", cancellationToken: CancellationToken.None);
        AssertTrue(invalidIndexResults.Count > 0, "invalid index retains the safety document");
        AssertTrue(invalidIndexResults.All(result => result.Path == "safety/README.zh-CN.md"), "invalid index remains fail closed");
        AssertFalse(invalidIndexResults.Any(result => result.Path == "deep/unrouted.md"), "invalid index blocks unindexed fallback");

        await File.WriteAllTextAsync(Path.Combine(root, "indexes", "knowledge-index.json"), "[]");
        KnowledgePackService wrongShapeIndexService = new(root, allowUnindexedFallback: true);
        IReadOnlyList<KnowledgeSearchResult> wrongShapeResults = await wrongShapeIndexService.SearchAsync("NPC 商店", cancellationToken: CancellationToken.None);
        AssertTrue(wrongShapeResults.Count > 0, "wrong-shape index retains the safety document");
        AssertTrue(wrongShapeResults.All(result => result.Path == "safety/README.zh-CN.md"), "wrong-shape index remains fail closed");
    }
    finally
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
    }
}

static async Task TestKnowledgeReparseBoundaryAsync()
{
    string root = Path.Combine(Path.GetTempPath(), "pvf-ai-reparse-tests-" + Guid.NewGuid().ToString("N"));
    string outside = Path.Combine(Path.GetTempPath(), "pvf-ai-reparse-outside-" + Guid.NewGuid().ToString("N"));
    try
    {
        Directory.CreateDirectory(Path.Combine(root, "indexes"));
        Directory.CreateDirectory(Path.Combine(root, "safety"));
        Directory.CreateDirectory(outside);
        await File.WriteAllTextAsync(
            Path.Combine(root, "indexes", "knowledge-index.json"),
            "{\"topics\":{\"npc-shop\":{\"entries\":[\"linked/untrusted.md\"]}}}");
        await File.WriteAllTextAsync(Path.Combine(root, "safety", "README.zh-CN.md"), "默认只读。");
        await File.WriteAllTextAsync(Path.Combine(outside, "untrusted.md"), "NPC 商店 REPARSE_MARKER");
        try
        {
            Directory.CreateSymbolicLink(Path.Combine(root, "linked"), outside);
        }
        catch (Exception exception) when (exception is UnauthorizedAccessException or PlatformNotSupportedException or IOException)
        {
            return;
        }

        KnowledgePackService service = new(root);
        IReadOnlyList<KnowledgeSearchResult> results = await service.SearchAsync("NPC 商店", cancellationToken: CancellationToken.None);
        AssertFalse(results.Any(result => result.Content.Contains("REPARSE_MARKER", StringComparison.Ordinal)), "knowledge reparse point exclusion");
        AssertFalse(results.Any(result => result.Path == "linked/untrusted.md"), "knowledge reparse path exclusion");
    }
    finally
    {
        if (Directory.Exists(root))
        {
            Directory.Delete(root, recursive: true);
        }
        if (Directory.Exists(outside))
        {
            Directory.Delete(outside, recursive: true);
        }
    }
}

static async Task TestMigratedKnowledgePackAsync()
{
    DirectoryInfo? directory = new(AppContext.BaseDirectory);
    while (directory != null && !File.Exists(Path.Combine(directory.FullName, "pvfUtility.csproj")))
    {
        directory = directory.Parent;
    }
    if (directory == null)
    {
        throw new InvalidOperationException("Could not locate the repository root for migrated knowledge tests.");
    }

    string knowledgeRoot = Path.Combine(directory.FullName, "Resources", "AiAssistant", "KnowledgePack");
    AssertIntEqual(274, Directory.EnumerateFiles(knowledgeRoot, "*", SearchOption.AllDirectories).Count(), "migrated knowledge file count");
    AssertEqual(
        "5973788a44fa7a2a4cb6c6f68b6a56d56a965a891783cf8e7034c1f0a1725ae0",
        ComputeCanonicalTreeHash(knowledgeRoot),
        "migrated knowledge canonical tree hash");
    AssertFalse(File.Exists(Path.Combine(knowledgeRoot, "task-cards", "nut-community-source-position-triage.zh-CN.md")), "dirty-only task card exclusion");
    AssertFalse(File.Exists(Path.Combine(knowledgeRoot, "workflows", "nut-skill-buff-po-controlled-planner.zh-CN.md")), "dirty-only workflow exclusion");
    KnowledgePackService service = new(knowledgeRoot);
    IReadOnlyList<KnowledgeSearchResult> results = await service.SearchAsync("NPC 商店", cancellationToken: CancellationToken.None);

    AssertTrue(results.Count > 0, "migrated knowledge route result");
    AssertTrue(results.Any(result => result.Path.Contains("npc-shop", StringComparison.OrdinalIgnoreCase)), "migrated NPC shop route");
    AssertFalse(results.Any(result => result.Path.Contains("nut攻略", StringComparison.OrdinalIgnoreCase)), "excluded knowledge material");
}

static AiAssistantConnection CreateConnection()
{
    return new AiAssistantConnection
    {
        Endpoint = "https://example.test/v1/",
        Model = "test-model",
        ApiKey = "test-key",
        MaxOutputTokens = 512
    };
}

static string ComputeCanonicalTreeHash(string root)
{
    using System.Security.Cryptography.IncrementalHash hash = System.Security.Cryptography.IncrementalHash.CreateHash(
        System.Security.Cryptography.HashAlgorithmName.SHA256);
    byte[] separator = [0];
    foreach (string path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
        .OrderBy(path => Path.GetRelativePath(root, path).Replace('\\', '/'), StringComparer.Ordinal))
    {
        string relativePath = Path.GetRelativePath(root, path).Replace('\\', '/');
        hash.AppendData(System.Text.Encoding.UTF8.GetBytes(relativePath));
        hash.AppendData(separator);
        hash.AppendData(NormalizeLineEndings(File.ReadAllBytes(path)));
        hash.AppendData(separator);
    }
    return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
}

static byte[] NormalizeLineEndings(byte[] source)
{
    byte[] normalized = new byte[source.Length];
    int targetIndex = 0;
    for (int sourceIndex = 0; sourceIndex < source.Length; sourceIndex++)
    {
        byte value = source[sourceIndex];
        if (value == (byte)'\r')
        {
            if (sourceIndex + 1 < source.Length && source[sourceIndex + 1] == (byte)'\n')
            {
                sourceIndex++;
            }
            normalized[targetIndex++] = (byte)'\n';
        }
        else
        {
            normalized[targetIndex++] = value;
        }
    }
    return normalized.AsSpan(0, targetIndex).ToArray();
}

static void AssertEqual(string? expected, string? actual, string scenario)
{
    if (!string.Equals(expected, actual, StringComparison.Ordinal))
    {
        throw new InvalidOperationException($"{scenario} failed: expected '{expected}', got '{actual}'.");
    }
}

static void AssertIntEqual(int expected, int actual, string scenario)
{
    if (expected != actual)
    {
        throw new InvalidOperationException($"{scenario} failed: expected '{expected}', got '{actual}'.");
    }
}

static void AssertTrue(bool actual, string scenario)
{
    if (!actual)
    {
        throw new InvalidOperationException($"{scenario} failed: expected true, got false.");
    }
}

static void AssertFalse(bool actual, string scenario)
{
    if (actual)
    {
        throw new InvalidOperationException($"{scenario} failed: expected false, got true.");
    }
}

static void AssertThrows<TException>(Action action, string scenario) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }
    throw new InvalidOperationException($"{scenario} failed: expected {typeof(TException).Name}.");
}

static async Task<TException> AssertThrowsAsync<TException>(Func<Task> action, string scenario) where TException : Exception
{
    try
    {
        await action();
    }
    catch (TException exception)
    {
        return exception;
    }
    throw new InvalidOperationException($"{scenario} failed: expected {typeof(TException).Name}.");
}

sealed class RecordingHandler : HttpMessageHandler
{
    private readonly string _responseJson;

    private readonly System.Net.HttpStatusCode _statusCode;

    public Uri? RequestUri { get; private set; }

    public string? AuthorizationScheme { get; private set; }

    public string? AuthorizationParameter { get; private set; }

    public string? RequestBody { get; private set; }

    public RecordingHandler(string responseJson, System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.OK)
    {
        _responseJson = responseJson;
        _statusCode = statusCode;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestUri = request.RequestUri;
        AuthorizationScheme = request.Headers.Authorization?.Scheme;
        AuthorizationParameter = request.Headers.Authorization?.Parameter;
        RequestBody = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        return new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_responseJson, System.Text.Encoding.UTF8, "application/json")
        };
    }
}

sealed class OversizedResponseHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StreamContent(new RepeatingReadStream(2 * 1024 * 1024 + 1))
        });
    }
}

sealed class RepeatingReadStream : Stream
{
    private long _remaining;

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public RepeatingReadStream(long length)
    {
        _remaining = length;
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

sealed class QueueGateway : IAiChatCompletionGateway
{
    private readonly Queue<AiCompletionMessage> _responses;

    public List<IReadOnlyList<AiCompletionMessage>> Requests { get; } = [];

    public QueueGateway(params AiCompletionMessage[] responses)
    {
        _responses = new Queue<AiCompletionMessage>(responses);
    }

    public Task<AiCompletionMessage> CompleteAsync(
        AiAssistantConnection connection,
        IReadOnlyList<AiCompletionMessage> messages,
        IReadOnlyList<AiToolDefinition> tools,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Requests.Add(messages.ToArray());
        return Task.FromResult(_responses.Dequeue());
    }
}
