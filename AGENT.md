# pvfUtility Agent Guide

## 作用域

本文件适用于 `pvfUtility-recovered` 整个目录树。它面向继续恢复源码、修复运行问题、迁移依赖或维护构建系统的自动化代理和开发者。

这是反编译恢复工程，不是原始作者仓库。处理优先级依次为：

1. 保持程序可构建、可启动和可独立复制。
2. 保持原发布程序集、资源和运行行为的兼容性。
3. 提高恢复源码的可读性、完整性和可验证性。
4. 在证据充分时再进行结构重构或依赖替换。

不要为了让代码“看起来更正常”而猜测原始实现。对恢复代码的修改必须由 IL、资源、调用点、运行结果或现有文档支持。

## 首先阅读

开始修改前，按任务范围阅读以下文件：

- `README.md`：构建、运行、复制和项目边界。
- `README_RECOVERY.md`：完整恢复状态和来源。
- `pvfUtility.csproj`：主程序入口、目标框架和运行时设置。
- `Directory.Build.targets`：主程序 XAML 排除、BAML 资源、DevExpress 间接依赖和 `lib` 复制规则。
- `docs/BAML_RECOVERY.md`：涉及任何 XAML、资源或 CLR 类型重命名时必读。
- `docs/BINARY_RECOVERY.md`：涉及 DLL、原生库或恢复源码项目时必读。
- `docs/NET10_MIGRATION.md`：涉及目标框架、运行时、依赖清单或 DevExpress 时必读。
- `docs/OBFUSCATED_NAME_MAP.md`：处理混淆类型、语义文件名和名称恢复时必读。
- `SourceLibraries/README.md`：修改恢复库项目时必读。

## 正式目录边界

旧的 `Recovered` 汇总目录已经完成归位并删除。以下位置现在属于正式源码工程：

| 内容 | 正式位置 |
| --- | --- |
| 主程序集原始 BAML 容器 | `Resources/pvfUtility.g.resources` |
| 主程序运行和框架兼容代码 | `PvfCode/Compatibility/` |
| 22 个恢复源码库及其解决方案 | `SourceLibraries/` |
| 源码库生成输出 | `SourceLibraries/.build/` |

不得重新建立 `Recovered/` 来存放正式源码、资源、构建配置或生成产物。`SourceLibraries/.build/` 可以删除并由构建重新生成，其他三个正式位置不能作为临时恢复输出清理。

## 不可破坏的工程约束

### 1. 保持工程独立

项目不得依赖当前目录之外的恢复材料或绝对路径，包括：

- `pvfUtility-old`；
- `_analysis_extract`；
- 原始 `pvfUtility.exe` 所在目录；
- `VVQIVol0ivE5AgW2C1.bt0xYrj1YUPGy0kR1u` 或类似临时提取目录；
- 用户机器上的固定盘符、SDK 安装目录或反编译工具缓存。

新增引用必须使用项目内相对路径、`ProjectReference` 或可还原的 `PackageReference`。提交前检查：

```powershell
rg -n "_analysis_extract|pvfUtility-old|VVQIVol0ivE5AgW2C1" . `
  -g "!bin/**" `
  -g "!obj/**" `
  -g "!artifacts/**" `
  -g "!SourceLibraries/.build/**" `
  -g "!*.md" `
  -g "!*.csv"

rg -n "Recovered[\\/]" . `
  -g "!bin/**" `
  -g "!obj/**" `
  -g "!artifacts/**" `
  -g "!SourceLibraries/.build/**" `
  -g "!*.md" `
  -g "!*.csv"

rg -n "<HintPath>([A-Za-z]:|\\\\)" . `
  -g "*.csproj" `
  -g "Directory.Build.*" `
  -g "!artifacts/**" `
  -g "!SourceLibraries/.build/**"
```

预期三项均无匹配。类名、属性名或脚本名中的 `Recovered` 可以保留；禁止的是重新引用旧目录布局。

### 2. 保持 `.NET 10 / Windows x64` 基线

- 主程序目标必须保持 `net10.0-windows`、`win-x64` 和 `x64`。
- 恢复库保持 `net10.0` 或 `net10.0-windows`，按其 API 使用范围选择。
- 不要重新引入原包中的 `.NET 6` `pvfUtility.deps.json` 覆盖逻辑。
- 不要把 `global.json` 改成机器上的绝对 SDK 路径或只在单台机器存在的版本。
- 不要通过关闭平台分析、nullable 或全部 warning 来掩盖迁移问题。

### 3. 保留主程序原始 BAML

`Resources/pvfUtility.g.resources` 是当前主程序正确加载 UI 的关键输入，其中包含 126 个原始 BAML。它不是可删除的编译产物。

主程序的可读 XAML 被 `Directory.Build.targets` 明确设置为 `None`。除非任务明确要求重建主程序 XAML 编译链，并且已经恢复连接 ID、生成字段、事件绑定和 Pack URI，否则不得：

- 删除或替换 `pvfUtility.g.resources`；
- 把全部主程序 XAML 批量改为 `Page`；
- 把 `app.xaml` 重新设为 `ApplicationDefinition`；
- 重新生成 BAML 后直接覆盖原始资源容器；
- 因为物理文件已改为语义名称，就同步改动 BAML 仍使用的 CLR 类型名。

原始 BAML 的类型表和延迟资源仍引用账号、云备份、商店、ChatGPT 等遗留功能的部分 CLR 类型、枚举和模板属性。这些成员首先是反序列化兼容契约；只有 ChatGPT 工具栏绑定被明确路由为新的 PVF AI 助手，不能据此推断其他遗留联网功能也应恢复。不得仅因 C# 中没有直接调用就删除；缺失成员可能直到延迟资源实例化时才以 `WpfXamlLoader.TransformNodes` 空引用的形式失败。

联网边界必须在兼容类型之外保持：云 GET/POST 不得创建或调用外网客户端，更新和遥测程序集不得进入输出，账号/云/商店/共享等遗留控件必须从最终界面移除。ChatGPT 是唯一允许的显式联网入口：不得在源码或应用 JSON 中保存 API Key；只允许 OpenAI-compatible 聊天请求和白名单 PVF 只读工具；当前 PVF 内容必须经会话内用户授权后才能发送；模型不得获得保存、替换、发布、客户端写入、进程执行或通用文件系统工具。AI 对话必须与 `FindView` 共用右侧停靠位置的页签，不得重新路由为中央文档。修改这些兼容类型或入口后必须运行 `scripts/Test-RecoveredStartup.ps1`，仅通过编译不算完成验证。

依赖库 `SourceLibraries` 中的 18 个 XAML 已恢复为正常可编译资源，可以按各项目现有规则维护。

### 4. 理解恢复源码运行模式

`Directory.Build.targets` 通过 `RecoveredSourceLibraryMode` 控制主程序依赖图：

- `All` 是默认值，主程序实际使用 20 个恢复源码项目；
- `Core` 使用 17 个恢复项目；
- `Leaf` 使用 7 个叶子项目；
- `Binary` 使用 `lib/` 中的原发布 DLL，只用于差分诊断和兼容回退。

22 项恢复库内部共有 39 条 `ProjectReference`。`Settings` 和 `SettingsModel` 可独立构建，但不是主程序的运行依赖。默认 `All` 模式下，修改 `PvfCode.Services`、`PvfCode.Models` 等恢复源码会直接改变主程序输出。

不要移除以下验证边界：

- `AssemblyName`、Version、Culture 和 PublicKeyToken 必须与替代的 DLL 兼容；
- 对应 `lib` DLL 不能与项目输出以同一路径重复发布；
- `recovered-source-libraries.txt` 必须列出实际接入的项目；
- `scripts/Test-RecoveredStartup.ps1` 必须通过 20 个 SHA-256 来源比较、已加载模块检查和完整主窗口检查；
- `Binary` 模式必须保持可用，便于区分源码恢复问题和原发布依赖问题。

### 5. 保留运行依赖和构建规则

以下内容是源码工程的一部分，不是普通生成文件：

- `lib/` 中的外部托管 DLL、原生 DLL、主题、卫星资源、可选运行数据和 `Binary` 回退 DLL；
- `Resources/pvfUtility.g.resources`；
- `PvfCode/Compatibility/DevExpressTrialInitializer.cs`；
- `Directory.Build.targets` 中的 BAML 间接依赖和复制规则；
- `PvfCode/Compatibility/RecoveredAssemblyResolver.cs`；
- `PvfCode/Compatibility/DevExpressNet10Compatibility.cs`。

`RecoveredAssemblyResolver` 只能在默认加载失败后探测应用输出目录，并验证程序集身份。不要把它扩展成递归搜索磁盘、父目录或不可信路径的通用加载器。

`DevExpressTrialInitializer` 使用 `[ModuleInitializer]`，必须继续位于主程序编译输入中。不要把它移回临时运行目录，也不要仅因没有显式调用点而判定为死代码。

`DevExpressNet10Compatibility` 是针对 DevExpress WPF 24.1 与 `.NET 10` WPF 私有字段结构的窄范围适配。只有在升级到原生支持该结构的 DevExpress 版本并完成 UI 验证后，才能移除。

### 6. 谨慎处理混淆名称

物理文件名和 CLR 名称都可以恢复，但 CLR 名称可能被以下内容引用：

- 原始 BAML；
- `Type.GetType`、反射或依赖注入；
- JSON/XML/二进制序列化；
- 配置文件或资源 URI；
- 其他发布 DLL。

重命名前必须使用 `rg` 搜索源码和可读资源、检查 `Resources/pvfUtility.g.resources` 的 BAML 字节，并核对原程序集 IL 与 `docs/OBFUSCATED_NAME_MAP.md`。当前只有 `DocumentGroupService`、`DocumentPanelService` 和 `TreeFileClipboardManager` 的三个混淆 CLR 身份因 BAML 引用而保留；不要在未重写 BAML 类型记录的情况下修改它们。

### 7. 区分源文件和生成文件

可以删除并重新生成：

```text
bin\
obj\
artifacts\
SourceLibraries\.build\
```

不要手工修改这些目录中的 C#、XAML、deps.json 或 DLL，然后把修改误认为源码修复。

根目录 `Recovered/` 既不是正式源码目录，也不是允许的生成目录，应保持不存在。不要把 `SourceLibraries/.build`、反编译器输出或运行日志移动到该名称下。

`docs/` 根目录中的 10 个 Markdown/CSV 文件是保留的恢复审计记录，不属于生成文件。它们记录了 BAML 映射、连接 ID、字符串恢复、混淆名称、二进制分类和 `.NET 10` 迁移依据，不应在普通清理中删除。

旧的 ILSpy 临时参考树已经删除。以后如需重新反编译程序集，应输出到 `artifacts/decompiler/<AssemblyName>/`，完成核对后只把有效源码和资源移入正式项目路径；不要让临时反编译树重新参与 MSBuild。

## 标准工作流程

### 1. 建立基线

确认当前目录和 SDK：

```powershell
Get-Location
dotnet --version
dotnet --info
dotnet sln .\pvfUtility.sln list
Test-Path .\Recovered
```

最后一项预期为 `False`。主解决方案中的源码库路径应全部位于 `SourceLibraries\...`。

如果目录由 Git 管理，先检查现有改动。不要覆盖、回退或清理用户已有修改。当前目录也可能不是 Git 仓库，因此工作流不得依赖 Git 才能完成。

### 2. 定位实际编译输入

优先使用 `rg` 和 MSBuild 项目文件确认代码是否真的进入构建：

```powershell
rg -n "目标类型或成员" . `
  -g "*.cs" `
  -g "*.xaml" `
  -g "!bin/**" `
  -g "!obj/**"

dotnet msbuild .\pvfUtility.csproj -getProperty:TargetFramework
```

不要只修改 `bin`、`obj`、`artifacts/decompiler` 或其他反编译器临时输出。

### 3. 小范围修改

- 优先遵循现有目录、namespace 和项目引用模式。
- 保留与二进制兼容相关的 public/internal 签名，除非任务明确要求改变。
- 对反编译器产生的异常结构做重构前，先确认控制流和异常行为。
- 对推断出的含义使用简短注释，不要把推测写成确定事实。
- 不进行与任务无关的大范围格式化、nullable 清理或重命名。
- 不修改第三方商业 DLL 本体来绕过兼容问题。
- 不在源码、配置或文档中加入真实密钥、令牌、服务器凭据或个人路径。

### 4. 按风险验证

最低构建命令：

```powershell
dotnet restore .\pvfUtility.sln
dotnet build .\pvfUtility.sln -c Debug --no-restore
```

仅恢复库任务也应运行：

```powershell
dotnet build .\SourceLibraries\pvfUtility.SourceLibraries.sln `
  -c Debug `
  --no-restore
```

主程序运行、资源、依赖或框架相关修改必须运行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-RecoveredStartup.ps1
```

发布逻辑变化还必须发布并测试发布目录：

```powershell
dotnet publish .\pvfUtility.csproj `
  -c Release `
  -r win-x64 `
  --self-contained false `
  -o .\artifacts\publish\win-x64

powershell -NoProfile -ExecutionPolicy Bypass `
  -File .\scripts\Test-RecoveredStartup.ps1 `
  -OutputDirectory .\artifacts\publish\win-x64
```

## 验证矩阵

| 改动类型 | 最低验证要求 |
| --- | --- |
| 仅 Markdown/CSV 文档 | 检查相对链接、路径和命令；无需伪称运行了应用测试。 |
| 主程序普通 C# | 构建主解决方案；影响启动或服务注册时运行 UI 启动检查。 |
| 主程序 XAML、BAML、资源或类型名 | 构建主解决方案并运行 UI 启动检查；核对 BAML/XAML 映射。 |
| `Directory.Build.targets`、`.csproj`、依赖或 `lib` | `restore`、完整构建、UI 启动检查；涉及发布时再验证发布目录。 |
| 单个已接入恢复库内部实现 | 构建该项目、恢复库解决方案和主程序；默认 `All` 模式下还要按影响范围运行 UI 启动检查。 |
| 恢复库公共 API、程序集身份或项目引用 | 构建全部 22 个源码库并核对依赖图；必要时做 API/强名称比较。 |
| `.NET`、WPF 或 DevExpress 迁移 | 完整还原、构建、发布和两次连续 UI 启动检查。 |
| 启动崩溃或错误窗口 | 使用 `Test-RecoveredStartup.ps1` 获取窗口文本；不能只报告进程退出码。 |

现有大量反编译警告不是测试失败。完成标准是没有新增编译错误，并且任务相关的运行验证通过。若警告数量或类型发生明显变化，应定位原因而不是全局禁用。

## 继续恢复 DLL 时

新增 DLL 源码恢复应遵循以下顺序：

1. 先判断 DLL 是托管程序集、原生库还是纯资源卫星程序集。
2. 对托管程序集记录 AssemblyName、版本、目标框架、PublicKeyToken、资源和公共 API。
3. 用反编译器输出项目后，删除绝对 `HintPath`，恢复项目内相对依赖。
4. 已恢复项目之间必须使用 `ProjectReference`，避免再次引用对应的 `lib/*.dll`。
5. BAML 应恢复为 XAML；RESX、ICO、XSHD、压缩载荷等数据资源应按原逻辑保留。
6. 编译后核对程序集身份、资源名和公共 API；强名称程序集还要核对 token。
7. 更新 `SourceLibraries/pvfUtility.SourceLibraries.sln`、主解决方案和 `docs/BINARY_RECOVERY.md`。
8. 接入主程序时更新 `RecoveredSourceProject` 清单，并完成程序集身份、发布去重、来源哈希和 UI 启动验证；不能只因为源码项目可编译就宣告替换成功。

原生 DLL 不能“恢复成 C#”。应保留二进制、记录用途和架构，并确认复制到输出目录的位置正确。

## NuGet 与 IDE

- 正式解决方案是根目录的 `pvfUtility.sln`。
- `.vscode/settings.json` 已设置 `dotnet.defaultSolution`。
- 不要把工作区级 C# Dev Kit 自动生成的临时 `pvfUT.sln` 当成项目文件修改。
- IDE 报 NuGet 错误时，先执行命令行 `dotnet restore .\pvfUtility.sln` 区分项目错误和 IDE 缓存错误。
- 若命令行成功而 IDE 仍失败，重载窗口并通过 `C# Dev Kit: Open Solution` 重新选择正式解决方案。
- 不要通过加入父目录中的旧工程或临时 `*.wpftmp.csproj` 来消除 IDE 提示。

## 文档同步

以下变化必须同步文档：

- 新恢复或替换 DLL：更新 `docs/BINARY_RECOVERY.md` 和源码库 README。
- BAML/XAML 数量或映射变化：更新 `docs/BAML_RECOVERY.md` 和 `docs/BAML_XAML_MAP.csv`。
- CLR/物理文件语义名称变化：更新 `docs/OBFUSCATED_NAME_MAP.md`。
- 目标框架、运行时、DevExpress 或 deps.json 变化：更新 `docs/NET10_MIGRATION.md`。
- 独立复制要求、构建命令或目录结构变化：更新根 `README.md` 和本文件中的正式目录边界。

不要改写审计 CSV 来隐藏未恢复项。映射文件应保留可追踪性。

## 完成标准

任务完成前确认：

- 修改发生在正式源码或配置中，而不是生成/参考目录；
- 根目录不存在 `Recovered/`，源码、项目和脚本中也没有旧 `Recovered/...` 路径依赖；
- 没有新增项目外路径、绝对 `HintPath` 或旧恢复目录依赖；
- 没有误删 `lib`、`pvfUtility.g.resources` 或 BAML 间接依赖；
- 已执行验证矩阵要求的命令，并记录成功或失败；
- 启动相关改动已经确认主窗口完整出现且无错误窗口；
- 恢复事实、推断和仍未知内容在说明中被明确区分；
- 最终报告列出修改文件、构建结果、启动结果以及仍存在的风险。
