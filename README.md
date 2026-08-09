# pvfUtility

`pvfUtility` 是一个可独立构建和运行的 Windows WPF 工程，主要包含 PVF/NPK 文件浏览、编辑、检索、对比、导入导出等相关代码。

本工程的源码恢复背景、兼容边界和审计记录保留在 `README_RECOVERY.md` 与 `docs/` 中。当前只保留 3 个被原始 BAML 明确引用的混淆 CLR 类型；其他已确认安全的随机类型名、死代码和一批编译器展开结构已经恢复或删除，源码中仍有待分批整理的私有成员和编译警告。

## 当前状态

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

## 离线数据与联网边界

账号、云备份、联网商店、共享上传、在线更新、远程起始页和异常遥测已经停用。资源树注释、书签和 PVF 标签注释改为未加密 JSON，运行后可直接维护：

- `Options/AppConfig.json`：应用设置与 `PvfConfig.TreelistCommentDic` 资源树注释。
- `Options/AiAssistant.json`：PVF AI 助手的 Base URL、模型和输出上限；不包含 API Key。
- `Options/Bookmarks.json`：本地书签树。
- `Options/PvfComments/<后缀>.json`：严格按 PVF 文件后缀隔离的标签注释；查询不会回退到其他后缀的同名标签。标签悬浮提示和标签翻译管理器会显示 `Title`、Markdown 格式的 `Comment`，以及 Markdown 格式的 `OfficialDescription`，并提供对应编辑与预览界面。
- `Resources/OfficialAnnotationTranslation/`：官方样例的只读翻译文档。工具菜单中的“官方注释文档”会在主编辑区右侧打开阅读页签；标签注释中的“官方示例：文件名”可直接跳转到对应文档。

新安装的初始数据来自相邻 `pvf-parser-ts` 工程，打包副本位于 `Resources/OfflineDefaults/Options/`。更新源数据后可重新生成：

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
