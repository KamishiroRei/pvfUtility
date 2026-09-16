# pvfUtility

`pvfUtility` 是一个可独立构建和运行的 Windows WPF 工程，主要包含 PVF/NPK 文件浏览、编辑、检索、对比、导入导出等相关代码。

本工程的源码恢复背景、兼容边界和审计记录保留在 `README_RECOVERY.md` 与 `docs/` 中。当前只保留 3 个被原始 BAML 明确引用的混淆 CLR 类型；其他已确认安全的随机类型名、死代码和一批编译器展开结构已经恢复或删除，源码中仍有待分批整理的私有成员和编译警告。

## 当前状态

### PVF 唯一工具与 AI 入口合同

`D:\Game\DNF\pvfUtility` 是 PVF 唯一工具主线。`pvfUtility.exe` 只供用户使用的 GUI，AI/自动化不得启动、点击或读取 GUI。

AI 固定使用 `cli\Pvf110.Cli\`，合同标识为 `pvfUtility-ai-cli-20260831`；标准执行方式是 `dotnet cli\Pvf110.Cli\bin\Release\net10.0\Pvf110.Cli.dll`。根目录孤立的 `Pvf110.Cli.exe` 缺少相邻程序集，不是稳定 AI 入口。

90CN 的 `Script.pvf` 和本机 `国服115.pvf` 都先由 CLI 自动探测格式；只有独立的 Pvf110 归档才提供 `PVF_SKDAT`。
- 主程序目标框架：`.NET 10`，即 `net10.0-windows`。
- 目标平台：Windows x64，运行时标识为 `win-x64`。
- 主解决方案：`pvfUtility.sln`。
- 主解决方案包含 1 个 WPF 主程序、22 个已恢复的源码库项目、Hybrid 资源合并工具，以及资源合并与礼盒预览回归测试项目。
- 默认 `RecoveredSourceLibraryMode=All`，主程序运行时直接使用其中 20 个恢复源码项目的构建产物；`Settings` 和 `SettingsModel` 保持独立可构建，但不是主程序依赖图的一部分。
- 22 个源码库共包含 1,394 个 C# 文件、18 个可编译 XAML 和 16 个 RESX。
- 主程序的 126 个原始 WPF BAML 已保存在 `Resources/pvfUtility.g.resources` 中。
- 默认 `RecoveredWpfResourceMode=Hybrid`：`app.xaml`、`themes/styles/iconsdark.xaml` 和 `views/viewscripteditor.xaml` 由源码编译，其余 123 个 BAML 及 121 个非 BAML 资源从原始容器逐字节补齐。
- `Legacy` 模式仍可完整使用 126 个原始 BAML；`SourceOnly` 在迁移清单达到 126 项前会明确构建失败。
- 原 `Recovered` 汇总目录已经移除；正式工程输入已分别归位到根目录 `Resources/`、`PvfCode/Compatibility/` 和 `SourceLibraries/`。
- 项目不依赖 `pvfUtility-old`、`_analysis_extract`、原始程序目录或工作区外的绝对路径。
- Debug 和 Release 单文件包都会生成 `recovered-source-libraries.txt`；包验证器会核对 `All` 模式对应的 20 个源码项目集合，启动检查会让主窗口保持 15 秒并拒绝任何错误窗口。

## 90CN PVF 输入边界

90CN 的默认编辑输入是客户端目录中的：

```text
D:\Game\DNF\90-CNC\地下城与勇士\Script.pvf
```

`D:\Game\DNF\1031.pvf` 仅是旧版 PVF。`D:\Game\DNF\国服115.pvf` 是已经转换为 90CN/ProtectedNKPI 容器的国服 115 内容参考 PVF，不是需要 `sk.dat` 的原始 115 Pvf110 归档；只有任务明确要求比较或迁移时才打开它们。`D:\Game\DNF\90-CNC\90CN\runtime\data\dnf\Script.pvf` 是历史服务端复制位置，不是当前输入或写回位置。90CN 服务端不维护专门 PVF，而是由控制器读取客户端 `Script.pvf` 并生成服务配置。

使用 GUI 或 CLI 修改 90CN 时，先备份客户端 PVF，再把提取文件放在项目工作目录中编辑，完成格式/条目校验和回读后，只写回上述客户端文件；随后重新执行 90CN 的 BAT 入口，让服务端重新读取。工具的运行选项和注释数据保存在工具自身的 `Options\`，不要把工具目录或运行选项复制进客户端/服务端运行目录。

## 90CN/经典统一解析管线（GUI）

GUI 对 90CN(NKPI/ProtectedNKPI) 与经典 PVF 使用**同一条解析管线**：90CN 不是平行实现，而是经 `Pvf110.Core/ClassicViewAdapter` 适配层继承旧版全部能力——富格式反编译（`ScriptFileParserNew` + CustomSectionFormat）、`[标签]`排版、名称/稀有度扫描、标签注释、IMG 链接、LST 工具与搜索全部按经典格式语义工作。

- 打开 90CN 时用名称池构建虚拟串表（`Stringtable.LoadFromNamePool`），type-1 条目懒加载为经典视图 token 流（`0xD0B0` 魔数 + 经典 ScriptType + 虚拟串表 ID）；标签映射与全量普查证据见 `通用知识区\PVF\PVF操作手册.md` 第 11 节。
- 编辑保存走经典编译器（`ScriptFileCompilerOl`），包保存时反向适配回 110 token；池外新字符串经 `NkpiNamePoolBuilder` 追加，未修改条目字节保真。
- CLI 保持 110 扁平文本合同（`ToText/FromText` 直引名称池偏移），不受 GUI 侧适配影响；`tests/Pvf110.IntegrationTests` 是 90CN 统一管线的 headless 端到端回归（打开→富格式→名称→编辑→保存→重开比对）。
- 已登记差异：`etc/equipmentpartset.etc` 的 90CN 版式与经典套装解析器不一致，套装表为 0（日志报错、不崩溃）；适配需另行逆向 115 版套装表版式。

## AI / CLI 工作版本

`cli/Pvf110.Cli/` 是 pvfUtility 的命令行版本，供脚本与 AI 会话对 PVF 做读取、解编译、搜索、提取、校验与修改重建。它直接复用主解决方案中的 `Pvf110.Core`，自动检测 **Pvf110(sk.dat)**、**Standard NKPI** 与 **Protected NKPI(90CN)** 三种格式，无需单独维护解析逻辑。

> 本节命令是 AI/自动化唯一允许使用的入口；`pvfUtility.exe` 是用户 GUI，AI 不得启动。命令行有两个等价入口：AI 合同固定使用 Release `Pvf110.Cli.dll` + `dotnet`；对外交付使用自包含单文件 `Pvf110.Cli.exe`（见下方"独立单文件发布"）。
> `D:\Game\DNF\国服115.pvf` 已转换为 90CN/ProtectedNKPI 容器，`info` 应显示 `format=ProtectedNKPI`、`requiresSkDat=False`；不要因为文件名是 115 就设置 `PVF_SKDAT`。
> `D:\Game\DNF\115US\Script.pvf` 是**另一份** `Pvf110` 容器（美服 2.38.2.34），配套 `115US\sk.dat` 与 `115US\DFO.exe`；`PVF_SKDAT` / `PVF_CLIENT_EXE` 均可省（缺省探测 PVF 同目录）。两种容器不得互相代换。

```powershell
dotnet build .\cli\Pvf110.Cli\Pvf110.Cli.csproj -c Release --no-restore
$env:PVF_PATH = 'D:\Game\DNF\90-CNC\地下城与勇士\Script.pvf'
dotnet .\cli\Pvf110.Cli\bin\Release\net10.0\Pvf110.Cli.dll version
dotnet .\cli\Pvf110.Cli\bin\Release\net10.0\Pvf110.Cli.dll info
```

```powershell
dotnet build .\cli\Pvf110.Cli\Pvf110.Cli.csproj -c Release
$env:PVF_PATH = 'D:\Game\DNF\90-CNC\地下城与勇士\Script.pvf'   # ProtectedNKPI，无需 PVF_SKDAT
dotnet .\cli\Pvf110.Cli\bin\Release\net10.0\Pvf110.Cli.dll info
dotnet .\cli\Pvf110.Cli\bin\Release\net10.0\Pvf110.Cli.dll list equipment
dotnet .\cli\Pvf110.Cli\bin\Release\net10.0\Pvf110.Cli.dll decompile stackable/10000001/10000039.stk
dotnet .\cli\Pvf110.Cli\bin\Release\net10.0\Pvf110.Cli.dll search "小误会"
dotnet .\cli\Pvf110.Cli\bin\Release\net10.0\Pvf110.Cli.dll extract .\out equipment/character
# 修改单文件内容并重建（Pvf110 输出 Script.pvf + sk.dat；NKPI 输出 Script.pvf）
dotnet .\cli\Pvf110.Cli\bin\Release\net10.0\Pvf110.Cli.dll write stackable/10000001/10000039.stk .\new-content.txt .\out
```

- `PVF_PATH` 必填；Pvf110 的 `sk.dat` 与客户端主程序由工具**自动定位**（PVF 同目录或上溯 3 层），`PVF_SKDAT` / `PVF_CLIENT_EXE` 只在需要显式覆盖时使用；`PVF_TEXT_ENCODING` / `PVF_TEXT_CHARS` 控制非 type-1 文本块（`.str` 等）的解码与输出长度；`PVF_OUTPUT_DIR` 可选输出根目录。
- 子命令：`version / info / list / file / decompile / batch-decompile / batch-decompile-script / batch-write / write / add / search / extract / validate / scan-all / tags / find-empty / trace / compiled-roundtrip / roundtrip / repack / rebuild-test / hash-test / nkpi-incremental-test / pvf110-incremental-test / mainline-epicdiff / tune-monster-base`（完整能力边界由 `version` 输出，Pvf110 的矩阵见 `通用知识区\PVF\PVF操作手册.md` §12）。
- `info` 首先输出格式和 `requiresSkDat`；`ProtectedNKPI` 与 `StandardNKPI` 的值必须为 `false`。文件名含有"115"不改变格式判断。
- 加载使用并行解压+解编译；`NkpiReader` / `Pvf110Reader` 的 group 缓存为线程安全实现。

### 独立单文件发布（对外交付）

`cli/Pvf110.Cli` 只依赖 `Pvf110.Core` 与 .NET 框架，因此可以发布为**单个自包含 exe**（目标机器不需要安装 .NET 运行时）。
`Pvf110.Cli.csproj` 中的 `StripGuiDependencyInjection` 目标会剥离仓库为 GUI 注入的 `lib/` 内容复制与 DevExpress 引用，
使构建输出从 228 个文件 / 385 MB 收敛为 8 个文件，单文件 exe 与 GUI 依赖树无关。

```powershell
dotnet publish .\cli\Pvf110.Cli\Pvf110.Cli.csproj -c Release -r win-x64 -o .\artifacts\publish\cli\Release\win-x64
# 交付：把发布目录中的 Pvf110.Cli.exe 覆盖复制到工具根目录（唯一交付位置，随后清空发布目录即可）
Copy-Item .\artifacts\publish\cli\Release\win-x64\Pvf110.Cli.exe .\Pvf110.Cli.exe -Force
```

发布目录只会产出 `Pvf110.Cli.exe` 一个文件；运行时不依赖同目录的 DLL、`dotnet` 或本仓库。
自包含单文件发布属性只在指定 `RuntimeIdentifier` 时生效，因此普通 `dotnet build` 仍是无 RID 的框架依赖构建。

### AI 入口禁用清单

- 不启动 pvfUtility.exe，不通过 GUI 点击或读取界面状态执行 AI 任务。
- 不调用通用工具区 PVF工具 下的 Node/Python 解析脚本。
- 不调用通用工具区 PVF110 下的旧 CLI、pvf-parser-ts 或其他副本。
- 根目录 `Pvf110.Cli.exe` 是自包含单文件交付件，与 Release dll 是同一份代码的两个入口；AI 合同仍以 Release dll 路径为准，不要按文件名再造第二份解析实现。

## 离线数据与联网边界

账号、云备份、联网商店、共享上传、在线更新、远程起始页和异常遥测已经停用。资源树注释、书签和 PVF 标签注释改为未加密 JSON，运行后可直接维护：

- `Options/AppConfig.json`：应用设置与 `PvfConfig.TreelistCommentDic` 资源树注释。
- `Options/AiAssistant.json`：PVF AI 助手的 Base URL、模型和输出上限；不包含 API Key。
- `Options/Bookmarks.json`：本地书签树。
- `Options/PvfComments/<后缀>.json`：严格按 PVF 文件后缀隔离的标签注释；查询不会回退到其他后缀的同名标签。标签悬浮提示和标签翻译管理器会显示 `Title`、Markdown 格式的 `Comment`，以及 Markdown 格式的 `OfficialDescription`，并提供对应编辑与预览界面。
- `Resources/OfficialAnnotationTranslation/`：官方样例的只读翻译文档。工具菜单中的“官方注释文档”会在主编辑区右侧打开阅读页签；标签注释中的“官方示例：文件名”可直接跳转到对应文档。

新安装的初始数据来自历史 PVF 知识数据工程的离线打包副本，位于 `Resources/OfflineDefaults/Options/`。它只提供 GUI 注释/书签初始数据，不是 PVF 解析器，也不是 AI 的 PVF 操作入口。更新源数据后可重新生成：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Import-PvfParserInitialData.ps1
```

程序只读取和写入上述 JSON 文件，不再包含 `AppConfig.bin`、`PvfTabComments.bin`、加密配置或 SQLite 注释库的兼容迁移路径。

原始 BAML 的类型表和延迟资源仍引用登录、云备份、商店与 ChatGPT 的部分 CLR 类型和模板属性。为保证 BAML 能够反序列化，这些类型和属性继续作为运行时契约保留。账号、云备份、联网商店、共享上传、自动更新、远程起始页和异常遥测仍保持禁用；ChatGPT 兼容入口现在只用于显式配置的 PVF AI 助手，不会重新启用其他在线功能。

## PVF AI 助手

原工具栏中的 `chatGPT` 按钮现显示为“AI 助手”。完整对话界面启动时隐藏，点击工具栏按钮后才会显示；显示时停靠在主工作区最右侧并贯穿工作区高度，不会占用中央文档组，关闭后也可通过同一按钮恢复并聚焦消息输入框。

- API 通过恢复完成的 `Whetstone.ChatGPT.ChatGPTClient` 调用 OpenAI-compatible
  `POST <Base URL>/chat/completions` 与 function tools；主程序不再维护第二套协议实现。
- Base URL、模型和最大输出 Token 保存在 `Options/AiAssistant.json`。
- API Key 只保留在当前进程内，也可由 `OPENAI_API_KEY` 环境变量提供；不会写入 `AppConfig.json` 或 `AiAssistant.json`。
- `OPENAI_BASE_URL` 和 `OPENAI_MODEL` 可覆盖本地配置。远程服务必须使用 HTTPS，HTTP 仅允许本机回环地址。
- “允许 AI 读取当前 PVF”默认关闭。只有用户在当前会话中明确勾选后，模型才能调用 PVF 只读工具并接收对应内容。
- 模型只有知识检索和白名单 PVF 只读工具，没有保存、替换、发布、部署、客户端写入、进程执行或通用文件系统工具。
- 构建输出中的 `Resources/AgentKnowledge/` 固定来自 `PVF-Agent-Workbench` clean commit 的 273 条 manifest 项和一份 CC0 许可，不包含 `.env`、真实 PVF、Node runtime、本地工作区输出或部署脚本。

当前适配器保留传统 Chat Completions 兼容面；不同提供方对模型名称、`max_tokens` 和 function tools 的支持可能不同，模型名称应按实际服务配置。实现结构、工具边界、知识来源和验证方法见 `docs/AI_ASSISTANT.md`。

## 快速开始

### 环境要求

- Windows x64。
- .NET 10 SDK x64。
- 首次执行 `dotnet restore` 时能够访问 NuGet 源。

仓库中的 `global.json` 要求 `.NET 10` SDK，并允许从 `10.0.100` 向后续 `.NET 10` feature band 滚动。当前恢复工程已使用 `10.0.300` 验证。

检查环境：

```powershell
dotnet --version
dotnet --list-sdks
```

### 本地单文件构建与运行

> **唯一打包交付位置（用户锁定，强制）**：打包完成后必须把 `pvfUtility.exe`、`recovered-source-libraries.txt` **直接覆盖复制**到工具根目录 `D:\Game\DNF\pvfUtility\`（用户从根目录运行 GUI）。**不备份、不留旧版本副本**；`artifacts\publish\**` 只是构建中间输出，不是交付位置。详见 `AGENT.md` 的"唯一打包交付位置"章节。

所有本地可运行、可复制和可分发的构建都使用与 CI、GitHub Release 相同的单文件入口：

```powershell
.\scripts\Build-SingleFile.ps1
```

默认生成 `Debug + Hybrid + All + win-x64` 的压缩、自包含单文件包：

```text
artifacts\publish\local\Hybrid\Debug\win-x64\
├── pvfUtility.exe
├── Options\
├── Resources\
├── Defaults\Options\
└── recovered-source-libraries.txt
```

构建脚本会自动还原依赖、发布单文件、验证包中没有独立 DLL/PDB/deps/runtimeconfig、审计 Hybrid WPF 容器，并运行源码 XAML 自检。直接运行：

```powershell
.\artifacts\publish\local\Hybrid\Debug\win-x64\pvfUtility.exe
```

显式生成 Release 或 Legacy 回退包：

```powershell
.\scripts\Build-SingleFile.ps1 `
  -Configuration Release `
  -RecoveredWpfResourceMode Hybrid

.\scripts\Build-SingleFile.ps1 `
  -Configuration Debug `
  -RecoveredWpfResourceMode Legacy
```

`SourceOnly` 是最终全源码门槛；当前只有 3/126 项完成，因此该模式会明确构建失败。

### 编译器与 IDE 检查（非交付物）

普通 `dotnet build` 仅用于编译器、IDE 和解决方案级回归检查：

```powershell
dotnet restore .\pvfUtility.sln
dotnet build .\pvfUtility.sln -c Debug --no-restore
```

它产生的普通 `bin` 输出不是可复制或可分发的构建结果，也不是本项目支持的本地运行入口。

### 启动验证

仅看到进程启动并不足以证明 WPF/BAML 已成功加载。项目提供了 UI Automation 启动检查：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-RecoveredStartup.ps1 `
  -Configuration Debug `
  -OutputDirectory .\artifacts\publish\local\Hybrid\Debug\win-x64 `
  -SingleFile
```

脚本会观察程序 15 秒，确认：

- 标题为 `pvfUtility` 的主窗口已经出现；
- 主窗口视觉树已完整加载；
- `BarSubItemLinksubFile`、`FilelistLayoutPanel` 和 `DocumentHost` 等关键控件存在；
- 没有出现错误或异常窗口。

### GitHub 单文件 Release

仓库中的 `.github/workflows/release.yml` 会生成 Windows x64 自包含单文件版本。该版本把 .NET 10 Desktop Runtime、托管依赖和发布时原生依赖压缩进 `pvfUtility.exe`，不需要目标机器预先安装 .NET。为避免破坏 WPF BAML、DevExpress 和反射加载，Release 发布明确关闭裁剪和 ReadyToRun。

推送以 `v` 开头的标签会自动创建或更新 GitHub Release：

```powershell
git tag v1.0.0
git push origin v1.0.0
```

也可以在 GitHub 的 **Actions → Build and publish Windows release → Run workflow** 中手动输入 `release_tag`。如果标签尚不存在，工作流会在所选提交上创建标签；如果标签或 Release 已存在，只有在标签仍指向本次构建提交时才覆盖同名发布资产。带连字符后缀的标签（如 `v1.0.0-beta.1`）会创建为预发布版本。

每个 Release 包含 ZIP 和对应的 `.sha256` 文件。ZIP 结构如下：

```text
pvfUtility-v1.0.0-win-x64\
├── pvfUtility.exe
├── Options\
├── Resources\
├── Defaults\Options\
└── recovered-source-libraries.txt
```

- `Options/` 从仓库维护的 `Defaults/Options/` 生成，可以直接修改；开发机上被忽略的运行时 `/Options` 永远不会进入发布包。
- `Resources/` 只包含程序按外部路径读取的 AI 知识库和官方注释资源，可以在解压后定制。WPF BAML、图标和其他编译资源仍位于 EXE 内。
- `Defaults/Options/` 是缺少配置文件时使用的原始模板。删除 `Options` 中的对应文件后，程序可在后续启动时重新复制默认值。
- 首次运行可能在 EXE 旁生成 `7z64.dll` 和 `pvfUtility.exe.WebView2/` 等运行时文件；这些文件不属于原始 Release 包。

本地复现单文件发布使用与 CI、GitHub Release 相同的构建脚本和发布配置：

```powershell
.\scripts\Build-SingleFile.ps1 `
  -Configuration Release `
  -RecoveredWpfResourceMode Hybrid `
  -RecoveredSourceLibraryMode All `
  -OutputDirectory .\artifacts\publish\local\Hybrid\Release\win-x64

powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-SingleFilePackage.ps1 `
  -OutputDirectory .\artifacts\publish\local\Hybrid\Release\win-x64 `
  -RecoveredSourceLibraryMode All

powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-RecoveredStartup.ps1 `
  -Configuration Release `
  -OutputDirectory .\artifacts\publish\local\Hybrid\Release\win-x64 `
  -SingleFile
```

当输入位于 `artifacts/publish` 时，运行时 UI 检查会先复制到 `artifacts/smoke/local-tests`，避免启动时释放的 `7z64.dll`、WebView 数据或配置写回污染已验证的发布目录。

`Properties/PublishProfiles/SingleFile.pubxml` 固定自包含、压缩、单文件、关闭裁剪与 ReadyToRun 等公共参数。`Test-GitHubReleasePackage.ps1` 现在是 `Test-SingleFilePackage.ps1` 的兼容包装。正式工作流会验证发布目录、独立冒烟副本和 ZIP 解压目录，并对单文件副本执行启动测试。

`master`/PR 构建也会从干净产物复制独立冒烟目录再启动程序，随后只上传未经启动污染的原始目录；运行时释放的 `7z64.dll` 或 WebView2 数据不会进入 Actions artifact。

## 独立复制

完整复制 `pvfUtility-recovered` 目录后，可以在其他路径中直接还原、编译和运行。以下生成目录可以不复制：

```text
bin\
obj\
artifacts\
SourceLibraries\.build\
```

以下内容不能遗漏：

- `SourceLibraries/`：22 个恢复源码项目、独立解决方案和源码库构建规则。
- `lib/`：原发布包中的托管 DLL、原生 DLL、卫星资源和其他运行文件。
- `Resources/pvfUtility.g.resources`：主程序原始的已编译 WPF 资源。
- `Resources/OfficialAnnotationTranslation/`：随构建和发布输出复制的官方注释只读文档。
- `images/`、`styles/`、`themes/`、`iconfont/` 等资源目录。
- `tools/PvfResourceMerger/`：Hybrid 构建必需的确定性 WPF 资源合并与审计工具。
- `tests/PvfResourceMerger.RegressionTests/`：资源合并器的控制台回归测试项目。
- `tests/GiftBoxPreview.RegressionTests/`：礼盒物品数量、概率权重和默认物品语义的控制台回归测试项目。
- `Directory.Build.targets`：负责恢复资源、BAML 间接依赖和本地运行文件的构建规则。
- `global.json`、`pvfUtility.csproj`、`pvfUtility.sln` 和 `SourceLibraries/pvfUtility.SourceLibraries.sln`。

不要复制或运行普通 `bin` 输出；它只用于编译器和 IDE 检查，不是支持的本地运行或分发入口。通过 `scripts/Build-SingleFile.ps1` 与 `SingleFile.pubxml` 生成的单文件 EXE 仍需保留发布包中的外置 `Options`、`Resources` 和 `Defaults` 目录。

旧的 `Recovered/` 不是独立工程所需目录，不要在复制后重新建立它。`SourceLibraries/.build` 是可再生输出，首次编译源码库时会自动创建。

## 项目结构

| 路径 | 说明 |
| --- | --- |
| `pvfUtility.sln` | 推荐使用的主解决方案，包含主程序、全部 22 个源码库、资源合并工具和两个回归测试项目。 |
| `pvfUtility.csproj` | 可运行的 WPF 主程序，目标为 `.NET 10 / Windows x64`。 |
| `PvfCode/` | 主程序的主要恢复源码。 |
| `controls/`、`views/`、`styles/`、`themes/` | 从主程序集恢复出的可读 XAML。 |
| `lib/` | 外部商业/开源托管依赖、原生 DLL、主题、卫星资源、可选运行文件和 `Binary` 回退模式所需的原发布 DLL。默认 `All` 模式不会把已由 20 个源码项目提供的同名 DLL 发布两次。 |
| `Resources/` | 主程序集的原始编译资源容器、官方注释只读文档和 AI 知识包；其中 `pvfUtility.g.resources` 是正式构建输入。 |
| `PvfCode/Compatibility/` | 程序集解析、DevExpress 试用初始化和 `.NET 10` WPF 兼容代码。 |
| `SourceLibraries/` | 从 22 个托管 DLL 恢复出的独立源码项目、解决方案和目录级构建配置。 |
| `tools/PvfResourceMerger/` | Hybrid 构建使用的无第三方依赖资源合并与程序集容器审计工具。 |
| `tests/PvfResourceMerger.RegressionTests/` | 覆盖资源替换、失败边界、确定性和审计契约的控制台回归测试。 |
| `tests/GiftBoxPreview.RegressionTests/` | 覆盖礼盒行末数量、可选概率权重和默认物品的控制台回归测试。 |
| `scripts/` | 启动验证和字符串恢复工具。 |
| `docs/` | BAML、连接 ID、二进制、混淆名称、字符串和 `.NET 10` 恢复审计记录。 |

### 已归位的恢复内容

| 旧临时位置 | 当前正式位置 | 处理方式 |
| --- | --- | --- |
| `Recovered/Resources/` | `Resources/` | 保留原始 `pvfUtility.g.resources`，由主项目直接嵌入。 |
| `Recovered/Runtime/` | `PvfCode/Compatibility/` | 初始化代码并入主程序兼容层，namespace 使用 `PvfCode.Compatibility`。 |
| `Recovered/SourceLibraries/` | `SourceLibraries/` | 22 个源码项目、解决方案和构建配置整体归位。 |
| `Recovered/SourceLibraries/.build/` | `SourceLibraries/.build/` | 不迁移旧产物；由当前源码重新生成。 |

构建配置、解决方案、脚本和审计文档均使用当前正式路径。源码或项目文件中不应重新出现指向旧 `Recovered/...` 布局的依赖。

旧的 ILSpy 临时参考树已经在正式源码归位后删除。以后如需重新反编译程序集，应把临时输出放在 `artifacts/decompiler/` 下，只将核对完成的源码和资源移入正式项目路径。

## 主程序与恢复库的关系

`Directory.Build.targets` 为主程序提供四种恢复源码接入模式：

| 模式 | 行为 |
| --- | --- |
| `All` | 默认值；主程序使用 20 个已验证的恢复源码程序集。 |
| `Core` | 使用 17 个核心恢复项目，其他本地依赖回退到 `lib`。 |
| `Leaf` | 只使用 7 个叶子恢复项目。 |
| `Binary` | 完全使用 `lib` 中的原发布 DLL，供差分诊断使用。 |

恢复项目之间继续使用 39 条 `ProjectReference`。主程序在 `All` 模式下也通过 `ProjectReference` 接入上述 20 个项目；构建时会从输出内容中排除对应的旧 DLL，发布时同一路径优先保留项目或包解析出的文件。程序集名称、版本和 PublicKeyToken 已与原 DLL 核对；单文件包验证器会核对恢复源码清单的项目集合，启动检查负责验证完整主窗口和错误窗口边界。

临时切换模式：

```powershell
dotnet build .\pvfUtility.csproj -c Debug `
  -p:RecoveredSourceLibraryMode=Binary
```

`Binary` 只用于确认旧 DLL 与恢复源码之间的差异，不是发布模式。原始
`lib/PvfCode.Dot.dll`、`lib/PvfCode.Services.dll` 和 `lib/Whetstone.ChatGPT.dll`
缺少恢复源码新增的注释字段、自选礼盒预览和 function tools API。Binary 构建通过
窄范围条件编译保持可用：缺失注释字段留空，自选礼盒使用通用预览，AI 请求会提示
改用源码模式；不会在 Binary 分支重新实现一套聊天协议。`Leaf` 和 `Core` 选中同一
旧 DLL 时也只启用对应的窄范围降级。默认构建、测试和发布路径仍是 `All`。

只构建恢复库源码图：

```powershell
dotnet restore .\SourceLibraries\pvfUtility.SourceLibraries.sln
dotnet build .\SourceLibraries\pvfUtility.SourceLibraries.sln `
  -c Debug `
  --no-restore
```

这些项目的输出集中在：

```text
SourceLibraries\.build\<ProjectName>\
```

## XAML 与 BAML

主程序目录中的 126 个可读 `.xaml` 文件用于阅读、检索和继续恢复源码。`Directory.Build.targets` 默认先把它们标记为 `None`，再只将显式 `RecoveredSourceXaml` 清单中的文件加入 WPF 编译。

当前 Hybrid 清单包含 `app.xaml`、`themes/styles/iconsdark.xaml` 和 `views/viewscripteditor.xaml`。WPF 先生成这三个 BAML，`PvfResourceMerger` 再以原始 `Resources/pvfUtility.g.resources` 为不可变基线，只替换 `app.baml`、`themes/styles/iconsdark.baml` 和 `views/viewscripteditor.baml`。验证后的最终容器仍有 247 个资源和 126 个 BAML，另外 244 项的原始类型与数据哈希不变。`IconsDark.xaml` 是无代码资源字典，源码版本保留 94 个唯一资源键。

`scripts/Test-RecoveredWpfResourceContainer.ps1` 将上述数量、差异键以及最终程序集内唯一资源容器的字节一致性固化为可重复审计；GitHub 的 `master`/PR 构建和标签 Release 都会执行该检查。

`Legacy` 模式不编译主程序恢复 XAML，直接嵌入原始容器；它是运行差分和紧急回退路径。`SourceOnly` 不允许原始 BAML 补齐，当前会因迁移清单只有 3 项而失败。

ILSpy 曾在 74 个主程序 XAML 中留下 270 条 `Unknown connection ID` 诊断注释。这些注释不是可执行标记，现已从可读 XAML 移出，并按原文件、原行号、连接 ID 和相邻上下文保存在 `docs/BAML_CONNECTION_ID_AUDIT.csv`。外置诊断不表示连接关系已经恢复，因此后续 XAML 仍必须逐项恢复和验证，不能批量改为 `Page`。

因此：

- 不要删除 `pvfUtility.g.resources`；
- 不要在未完成连接关系验证前把主程序 XAML 加入 `RecoveredSourceXaml`；
- 不要原地覆盖原始资源容器，Hybrid 合并输出只能写入 `obj`；
- 不要仅凭物理文件名重命名混淆后的 CLR 类型，原始 BAML 可能仍引用其完整类型名。
- 不要删除看似未被 C# 调用、但由 BAML 类型表或延迟资源引用的兼容属性；修改后必须运行 UI 启动检查。

恢复原始 BAML 后，主窗口可能重新实例化 BAML 中遗留的账号、商店和 ChatGPT 控件。账号、云、商店、更新和遥测仍通过禁用传输与过滤入口两层保持离线。ChatGPT 是唯一按用户配置恢复的联网入口：它使用会话内密钥、白名单只读工具和显式 PVF 读取授权，并作为查找面板旁的页签存在；不得借此恢复其他遗留网络传输。

依赖库中的 18 个 BAML 已单独恢复为可编译 XAML，不受上述主程序限制。详细记录见 `docs/BAML_RECOVERY.md`。

## `.NET 10` 兼容处理

项目不再把原发布包中的 `.NET 6` `pvfUtility.deps.json` 覆盖到构建结果中，当前依赖清单由 `.NET 10 SDK` 正常生成。

三个兼容组件在 UI 初始化前运行：

- `PvfCode/Compatibility/DevExpressTrialInitializer.cs`：通过模块初始化器恢复原发布程序使用的 DevExpress 试用初始化，文件已经从临时运行目录并入正式兼容层。
- `PvfCode/Compatibility/RecoveredAssemblyResolver.cs`：仅在正常程序集解析失败后，从程序输出目录按程序集身份补充解析可选托管依赖。
- `PvfCode/Compatibility/DevExpressNet10Compatibility.cs`：适配 DevExpress WPF 24.1 对旧版 WPF `Popup._popupRoot` 私有结构的反射访问。

此外，`Directory.Build.targets` 显式声明了只由原始 BAML 引用的 DevExpress Controls、PropertyGrid、TypedStyles 和主题程序集。

## 编译警告

恢复源码当前以“零编译错误”为基线，但仍存在数量较多的警告，主要包括：

- 原程序集没有保留 nullable 上下文；
- 反编译后未使用的字段或局部结构；
- 过时 API 和 Windows 平台分析；
- NuGet `NU1510` 和旧 DevExpress 引用引起的 `WindowsBase` 版本解析警告；
- 无法从 IL 判断原始意图的未等待异步调用。

这些警告不应被整体禁用。修复时应逐项确认行为，避免为了消除警告而改变反编译代码语义。

## VS Code

`.vscode/settings.json` 已把默认解决方案设置为：

```text
pvfUtility.sln
```

状态栏应显示 `Solution: pvfUtility.sln`。如果 C# Dev Kit 仍打开工作区缓存中自动生成的 `pvfUT.sln`，可能出现不存在项目导致的 NuGet 还原错误。此时执行：

1. `Ctrl+Shift+P`。
2. 运行 `Developer: Reload Window`。
3. 若仍未切换，运行 `C# Dev Kit: Open Solution`。
4. 选择当前目录中的 `pvfUtility.sln`。

命令行 `dotnet restore .\pvfUtility.sln` 是判断项目自身 NuGet 状态的可靠方式。

## 常见问题

### 找不到合适的 SDK

执行 `dotnet --list-sdks`，确认存在 `10.0.100` 或更高的 `.NET 10` SDK。仅安装 Desktop Runtime 不能编译项目。

### 编译成功但启动时出现错误窗口

先运行 `scripts/Build-SingleFile.ps1`，再运行 `scripts/Test-RecoveredStartup.ps1`。脚本默认检查本地 Debug/Hybrid 单文件包，并会收集错误窗口标题和可读取的异常文本，比只观察进程退出码更容易定位 BAML 或依赖问题。

### 出现缺少 DLL、主题或 Pack URI 资源错误

确认复制的是完整工程，特别是 `lib/`、`Resources/pvfUtility.g.resources` 和 `Directory.Build.targets`。不要从不完整的单独 `bin` 目录拼装运行环境。

### 修改恢复库后主程序没有变化

先确认没有显式传入 `-RecoveredSourceLibraryMode Binary`，并检查单文件包中的 `recovered-source-libraries.txt`。默认 `All` 模式下，清单应有 20 行；`scripts/Test-SingleFilePackage.ps1` 会核对全部项目名称和项目文件名，`scripts/Test-RecoveredStartup.ps1` 会确认对应源码构建输出存在并验证完整主窗口。

## 恢复文档

- `README_RECOVERY.md`：完整英文恢复说明和恢复来源。
- `docs/BINARY_RECOVERY.md`：托管/原生二进制分类和源码恢复清单。
- `docs/BAML_RECOVERY.md`：BAML 到 XAML 的恢复方法与运行策略。
- `docs/SOURCE_XAML_MIGRATION.md`：主程序渐进源码 XAML、资源合并、单文件发布与回退规则。
- `docs/NET10_MIGRATION.md`：`.NET 10` 迁移和 DevExpress 兼容处理。
- `docs/AI_ASSISTANT.md`：AI 助手宿主、接口、安全边界、知识来源与验证方法。
- `docs/OBFUSCATED_NAME_MAP.md`：混淆 CLR 类型与语义文件名的映射。
- `docs/BAML_XAML_MAP.csv`：126 个 BAML/XAML 的一对一映射。
- `docs/BAML_CONNECTION_ID_AUDIT.csv`：从 74 个可读 XAML 外置的 270 条连接 ID 反编译诊断。
- `docs/RECOVERED_*_STRING_MAP.csv`：已内联混淆字符串的审计记录。
- `AGENT.md`：后续自动化代理和维护者的工程规则。

## 来源与许可说明

本目录保存的是对已发布二进制的恢复结果，不代表能够证明或恢复原始源码版权、许可证和开发历史。目录中还包含 DevExpress 等第三方商业组件以及其他第三方库的二进制或恢复源码。

本工程目前没有统一的顶层许可证文件。使用、修改或再分发前，应分别确认原程序和所有第三方组件的授权条件；源码恢复本身不会产生额外的再分发权利。
