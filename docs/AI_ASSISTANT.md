# PVF AI assistant

## Purpose and host

The assistant migrates the reusable parts of `PVF-Agent-Workbench` into
`pvfUtility`: curated PVF knowledge, deterministic topic routing, and a bounded
read-only tool surface over the PVF already open in the application.

The original BAML toolbar item still binds to
`BarsVm.OnOpenChatGPTDocumentCommand`, but it no longer creates a central
document. `MainWindow` creates a long-lived `ChatGPTDocumentVm` and an AI
`LayoutPanel`, then docks that panel with `DockType.Right` against the root
workspace. The complete conversation remains visible at the far right and spans
the document and output rows.

The migration does not embed the Workbench Node runtime, `pvf-bridge`, native
bridge modules, real PVFs, local profiles, generated runtime output, deployment
skills, or credentials. Commands mentioned by historical knowledge documents
are reference material only and are not available as model tools in this host.

## Request flow

1. The user enters an OpenAI-compatible base URL, model, and session API key.
2. `ChatGPTDocumentVm` builds the bounded conversation and invokes
   `AiAssistantAgent`. Only the most recent 24 messages are eligible; one
   message is limited to 32,000 characters and their combined content to
   128,000 characters before any network request.
3. The gateway maps the assistant domain messages and tools into the recovered
   `Whetstone.ChatGPT.ChatGPTClient`. That original client sends non-streaming
   `POST <base>/chat/completions` requests and parses the provider response.
4. Tool calls are matched against an in-process name allowlist. Unknown names
   receive a fixed error result and are never dynamically resolved.
5. Tool output is appended as a `tool` message and the model is called again.
   The loop is limited to six rounds, four calls per round, twelve calls per
   request, and 90,000 cumulative tool-result characters.
6. The final assistant message is displayed in the AI tab.

The API key is attached only as a Bearer header. It is never serialized by
`AiAssistantSettingsStore`, included in tool output, or written to application
logs. Closing the main window cancels the active request and clears the
in-memory key. Remote HTTP endpoints are rejected; clear-text HTTP is accepted
only for loopback development servers.

## Tool boundary

| Tool | Scope | Main limits |
| --- | --- | --- |
| `knowledge_search` | Curated bundled knowledge | Query at most 1,000 chars; 1-5 documents; at most 30,000 result chars |
| `pvf_current_context` | Open-PVF metadata and active logical document | No host absolute path |
| `pvf_list_files` | PVF logical paths | At most 200 paths; bounded prefix/filter strings |
| `pvf_search` | Logical paths or bounded decompiled text | At most 50 results and 2,000 candidates; content source budget 64 MiB |
| `pvf_read_file` | One exact PVF logical path | Source at most 16 MiB; result at most 30,000 chars |
| `pvf_list_registries` | Known `.lst` paths and loaded groups | At most 200 entries |
| `pvf_resolve_lst_id` | One ID in one exact `.lst` | Read-only resolution and target-exists check |

Only `knowledge_search` is exposed by default. The six PVF tools are created for
the current request only after the user checks **Allow AI to read the current
PVF**. Each request captures a PVF read snapshot and rejects work if the open PVF
is closed or switched while a tool is running.

No tool can open an arbitrary disk path, start a process, save a PVF, replace
text, write client files, publish, deploy, call the legacy cloud transport, or
execute Workbench commands. PVF text, scripts, comments, and tool output are
treated as untrusted data in the system prompt rather than instructions.

## Knowledge migration

The pinned source is `PVF-Agent-Workbench` commit
`41fc9f566bcfc08ed1f93d33fd7eb9470194e85c`.

The package contains the 273 paths listed by that commit's
`knowledge-pack/MANIFEST.json`, plus `LICENSE-KNOWLEDGE-CC0.md`. All 273 copied
manifest files match their declared SHA-256. The final packaged tree contains
274 files and currently occupies 4,339,466 raw bytes.

The line-ending-normalized, ordinal-path canonical tree SHA-256 is:

```text
5973788a44fa7a2a4cb6c6f68b6a56d56a965a891783cf8e7034c1f0a1725ae0
```

Detailed provenance, including the source manifest's byte-summary discrepancy,
is recorded in `Resources/AiAssistant/PROVENANCE.md`.

Files outside the clean allowlist were excluded, including dirty-only
experimental task cards/workflows, `nut攻略`, `.env`, local profiles, real PVFs,
runtime output, Node/native modules, and post-deploy automation. Workbench
program code was not copied.

At build and publish time, `Directory.Build.targets` synchronizes the package to
`AgentKnowledge/`. The destination is removed and recreated from the allowlisted
source items, so deleted knowledge files cannot remain as stale output.
`KnowledgePackService` reads `indexes/knowledge-index.json`, routes to the best
matching compact topics, and fails closed when the packaged index cannot be
loaded. Unindexed fallback is available only when explicitly enabled by an
isolated caller such as a test.

## BAML compatibility

`Resources/pvfUtility.g.resources` remains unchanged. Its ChatGPT toolbar
binding, `ChatGPTMessDocument` pack URI, connector identity, and
`DocumentItemContentTemplateSelector.ChatGPT` property remain runtime contracts.

`ChatGPTMessDocument` first loads the original BAML and then replaces its sparse
content with the recovered conversation UI from code. The readable XAML remains
non-compiled reference material. The selector property is retained because a
deferred style BAML writes it, but ChatGPT is not routed through
`RootDocument.Documents`.

Saved layouts from before the migration do not contain the AI panel. After a
layout restore or reset, `MainWindow` docks the panel at the far right when it
is missing or still uses the earlier shared-tab layout. The toolbar command
restores a hidden panel and focuses the conversation input.

## Configuration

Non-secret settings are written to `Options/AiAssistant.json`:

- `Endpoint`
- `Model`
- `MaxOutputTokens`

Optional environment variables are:

```powershell
$env:OPENAI_API_KEY = "<session key>"
$env:OPENAI_BASE_URL = "https://api.openai.com/v1/"
$env:OPENAI_MODEL = "<available model>"
```

Environment values override JSON settings for the current process. The selected
provider/model must support the Chat Completions message shape, `max_tokens`,
and function tools used by this compatibility adapter.

## Verification

Verify the full source graph, BAML startup, dock-tab interaction, and a
fresh publish directory:

```powershell
dotnet build .\pvfUtility.sln -c Debug --no-restore
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Test-RecoveredStartup.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Test-AiAssistantDocking.ps1

dotnet publish .\pvfUtility.csproj -c Release -r win-x64 --self-contained false `
  -o .\artifacts\publish\ai-assistant
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-RecoveredStartup.ps1 `
  -Configuration Release `
  -OutputDirectory .\artifacts\publish\ai-assistant
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-AiAssistantDocking.ps1 `
  -Configuration Release `
  -OutputDirectory .\artifacts\publish\ai-assistant
```

`RecoveredSourceLibraryMode=Binary` remains a differential-diagnosis mode, not
a release mode. It builds against the original `lib/Whetstone.ChatGPT.dll`, which
predates configurable Base URIs and function tools, so AI requests in that mode
report that `All`, `Core`, or `Leaf` is required. This keeps Binary buildable
without adding a duplicate protocol implementation. The supported AI build and
publish path uses the default `All` source mode.
