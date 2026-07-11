# pvfUtility

`pvfUtility` 是一个可独立构建和运行的 Windows WPF 工程，主要包含 PVF/NPK 文件浏览、编辑、检索、对比、导入导出等相关代码。

本工程的源码恢复背景、兼容边界和审计记录保留在 `README_RECOVERY.md` 与 `docs/` 中。当前只保留 3 个被原始 BAML 明确引用的混淆 CLR 类型；其他已确认安全的随机类型名、死代码和一批编译器展开结构已经恢复或删除，源码中仍有待分批整理的私有成员和编译警告。

## 当前状态

- 主程序目标框架：`.NET 10`，即 `net10.0-windows`。
- 目标平台：Windows x64，运行时标识为 `win-x64`。
- 主解决方案：`pvfUtility.sln`。
- 主解决方案包含 1 个 WPF 主程序和 22 个已恢复的源码库项目。
- 默认 `RecoveredSourceLibraryMode=All`，主程序运行时直接使用其中 20 个恢复源码项目的构建产物；`Settings` 和 `SettingsModel` 保持独立可构建，但不是主程序依赖图的一部分。
- 22 个源码库共包含 1,388 个 C# 文件、18 个可编译 XAML 和 16 个 RESX。
- 主程序的 126 个原始 WPF BAML 已保存在 `Resources/pvfUtility.g.resources` 中。
- 主程序运行时继续依赖该原始 BAML；根目录中的可读 XAML 仅供审阅，不参与主程序 WPF 标记编译。
- 原 `Recovered` 汇总目录已经移除；正式工程输入已分别归位到根目录 `Resources/`、`PvfCode/Compatibility/` 和 `SourceLibraries/`。
- 项目不依赖 `pvfUtility-old`、`_analysis_extract`、原始程序目录或工作区外的绝对路径。
- 目录归位后已重新验证 Debug 和 Release：20 个源码程序集哈希匹配，启动阶段观察到 14 个实际加载，主窗口保持 15 秒且没有错误窗口。
- Debug 和 Release 发布目录都会生成 `recovered-source-libraries.txt`；启动脚本会核对 20 个 DLL 的 SHA-256，防止无意回退到 `lib` 中的旧二进制。

## 离线数据

账号、云备份、联网商店、共享上传、在线更新、远程起始页和异常遥测已经停用。资源树注释、书签和 PVF 标签注释改为未加密 JSON，运行后可直接维护：

- `Options/AppConfig.json`：应用设置与 `PvfConfig.TreelistCommentDic` 资源树注释。
- `Options/Bookmarks.json`：本地书签树。
- `Options/PvfComments/<后缀>.json`：严格按 PVF 文件后缀隔离的标签注释；查询不会回退到其他后缀的同名标签。标签悬浮提示和标签翻译管理器会显示 `Title`、Markdown 格式的 `Comment`，以及 Markdown 格式的 `OfficialDescription`，并提供对应编辑与预览界面。

新安装的初始数据来自相邻 `pvf-parser-ts` 工程，打包副本位于 `Resources/OfflineDefaults/Options/`。更新源数据后可重新生成：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Import-PvfParserInitialData.ps1
```

程序只读取和写入上述 JSON 文件，不再包含 `AppConfig.bin`、`PvfTabComments.bin`、加密配置或 SQLite 注释库的兼容迁移路径。

原始 BAML 的类型表和延迟资源仍引用登录、云备份、商店与 ChatGPT 的部分 CLR 类型和模板属性。为保证 BAML 能够反序列化，这些类型中保留了最小兼容外壳，但不代表联网功能恢复：主窗口会移除对应入口，`ServiceCloud` 的 GET/POST 传输直接返回离线错误，ChatGPT 文档不创建客户端，自动更新与异常遥测程序集也不会进入输出目录。

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

### 还原和编译

在 `pvfUtility-recovered` 目录中执行：

```powershell
dotnet restore .\pvfUtility.sln
dotnet build .\pvfUtility.sln -c Debug --no-restore
```

只编译可运行的主程序：

```powershell
dotnet restore .\pvfUtility.csproj
dotnet build .\pvfUtility.csproj -c Debug --no-restore
```

编译后的程序位于：

```text
bin\Debug\net10.0-windows\win-x64\pvfUtility.exe
```

### 运行

```powershell
.\bin\Debug\net10.0-windows\win-x64\pvfUtility.exe
```

也可以通过 SDK 启动：

```powershell
dotnet run --project .\pvfUtility.csproj -c Debug
```

### 启动验证

仅看到进程启动并不足以证明 WPF/BAML 已成功加载。项目提供了 UI Automation 启动检查：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-RecoveredStartup.ps1
```

脚本会观察程序 15 秒，确认：

- 标题为 `pvfUtility` 的主窗口已经出现；
- 主窗口视觉树已完整加载；
- `BarSubItemLinksubFile`、`FilelistLayoutPanel` 和 `DocumentHost` 等关键控件存在；
- 没有出现错误或异常窗口。

### 发布

生成依赖 `.NET 10 Desktop Runtime` 的 x64 发布目录：

```powershell
dotnet publish .\pvfUtility.csproj `
  -c Release `
  -r win-x64 `
  --self-contained false `
  -o .\artifacts\publish\win-x64
```

验证发布结果：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-RecoveredStartup.ps1 `
  -OutputDirectory .\artifacts\publish\win-x64
```

目标机器运行这种发布结果时需要安装 `.NET 10 Desktop Runtime x64`。

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
- `images/`、`styles/`、`themes/`、`iconfont/` 等资源目录。
- `Directory.Build.targets`：负责恢复资源、BAML 间接依赖和本地运行文件的构建规则。
- `global.json`、`pvfUtility.csproj`、`pvfUtility.sln` 和 `SourceLibraries/pvfUtility.SourceLibraries.sln`。

不要只复制 `bin` 中的 `pvfUtility.exe`。该程序不是单文件发布，必须与发布目录中的 DLL、原生库和资源一起运行。

旧的 `Recovered/` 不是独立工程所需目录，不要在复制后重新建立它。`SourceLibraries/.build` 是可再生输出，首次编译源码库时会自动创建。

## 项目结构

| 路径 | 说明 |
| --- | --- |
| `pvfUtility.sln` | 推荐使用的主解决方案，包含主程序和全部 22 个源码库。 |
| `pvfUtility.csproj` | 可运行的 WPF 主程序，目标为 `.NET 10 / Windows x64`。 |
| `PvfCode/` | 主程序的主要恢复源码。 |
| `controls/`、`views/`、`styles/`、`themes/` | 从主程序集恢复出的可读 XAML。 |
| `lib/` | 外部商业/开源托管依赖、原生 DLL、主题、卫星资源、可选运行文件和 `Binary` 回退模式所需的原发布 DLL。默认 `All` 模式不会把已由 20 个源码项目提供的同名 DLL 发布两次。 |
| `Resources/` | 主程序集的原始编译资源容器；其中 `pvfUtility.g.resources` 是正式构建输入。 |
| `PvfCode/Compatibility/` | 程序集解析、DevExpress 试用初始化和 `.NET 10` WPF 兼容代码。 |
| `SourceLibraries/` | 从 22 个托管 DLL 恢复出的独立源码项目、解决方案和目录级构建配置。 |
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

恢复项目之间继续使用 39 条 `ProjectReference`。主程序在 `All` 模式下也通过 `ProjectReference` 接入上述 20 个项目；构建时会从输出内容中排除对应的旧 DLL，发布时同一路径优先保留项目或包解析出的文件。程序集名称、版本和 PublicKeyToken 已与原 DLL 核对，标准启动检查还会验证输出哈希与 `SourceLibraries/.build` 一致。

临时切换模式：

```powershell
dotnet build .\pvfUtility.csproj -c Debug `
  -p:RecoveredSourceLibraryMode=Binary
```

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

主程序目录中的 126 个可读 `.xaml` 文件用于阅读、检索和继续恢复源码。它们被明确标记为 `None`，不会重新编译成 WPF Page。

运行时仍加载 `Resources/pvfUtility.g.resources` 中的原始 BAML。原因是从 BAML 反编译出的 XAML 可能缺失连接 ID、生成字段和事件绑定关系，直接重编译可能导致界面能编译但运行时控件或事件失效。

ILSpy 曾在 74 个主程序 XAML 中留下 270 条 `Unknown connection ID` 诊断注释。这些注释不是可执行标记，现已从可读 XAML 移出，并按原文件、原行号、连接 ID 和相邻上下文保存在 `docs/BAML_CONNECTION_ID_AUDIT.csv`。外置诊断不表示连接关系已经恢复，也不改变继续使用原始 BAML 的兼容边界。

因此：

- 不要删除 `pvfUtility.g.resources`；
- 不要在未完成连接关系验证前把主程序 XAML 改回 `Page` 或 `ApplicationDefinition`；
- 不要仅凭物理文件名重命名混淆后的 CLR 类型，原始 BAML 可能仍引用其完整类型名。
- 不要删除看似未被 C# 调用、但由 BAML 类型表或延迟资源引用的兼容属性；修改后必须运行 UI 启动检查。

恢复原始 BAML 后，主窗口可能重新实例化 BAML 中遗留的账号、商店和 ChatGPT 控件。当前实现以两层方式保持离线：底层传输和客户端逻辑被禁用，界面层再按本地化资源键过滤遗留入口。仅隐藏菜单而保留可用网络传输不符合本工程的离线边界。

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

先运行 `scripts/Test-RecoveredStartup.ps1`。脚本会收集错误窗口标题和可读取的异常文本，比只观察进程退出码更容易定位 BAML 或依赖问题。

### 出现缺少 DLL、主题或 Pack URI 资源错误

确认复制的是完整工程，特别是 `lib/`、`Resources/pvfUtility.g.resources` 和 `Directory.Build.targets`。不要从不完整的单独 `bin` 目录拼装运行环境。

### 修改恢复库后主程序没有变化

先确认没有显式传入 `-p:RecoveredSourceLibraryMode=Binary`，并检查输出目录中的 `recovered-source-libraries.txt`。默认 `All` 模式下，清单应有 20 行；`scripts/Test-RecoveredStartup.ps1` 会进一步比较输出 DLL 与源码构建 DLL 的 SHA-256，并报告启动期间实际加载的恢复程序集数量。

## 恢复文档

- `README_RECOVERY.md`：完整英文恢复说明和恢复来源。
- `docs/BINARY_RECOVERY.md`：托管/原生二进制分类和源码恢复清单。
- `docs/BAML_RECOVERY.md`：BAML 到 XAML 的恢复方法与运行策略。
- `docs/NET10_MIGRATION.md`：`.NET 10` 迁移和 DevExpress 兼容处理。
- `docs/OBFUSCATED_NAME_MAP.md`：混淆 CLR 类型与语义文件名的映射。
- `docs/BAML_XAML_MAP.csv`：126 个 BAML/XAML 的一对一映射。
- `docs/BAML_CONNECTION_ID_AUDIT.csv`：从 74 个可读 XAML 外置的 270 条连接 ID 反编译诊断。
- `docs/RECOVERED_*_STRING_MAP.csv`：已内联混淆字符串的审计记录。
- `AGENT.md`：后续自动化代理和维护者的工程规则。

## 来源与许可说明

本目录保存的是对已发布二进制的恢复结果，不代表能够证明或恢复原始源码版权、许可证和开发历史。目录中还包含 DevExpress 等第三方商业组件以及其他第三方库的二进制或恢复源码。

本工程目前没有统一的顶层许可证文件。使用、修改或再分发前，应分别确认原程序和所有第三方组件的授权条件；源码恢复本身不会产生额外的再分发权利。
