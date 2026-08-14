# ProcessHelper

本文档用于分析 `Lanymy.Common.Helpers.ProcessHelper` 模块。

## 模块定位

- 提供一层轻量的进程启动辅助能力。
- 当前重点不是进程管理框架，而是快速生成 `ProcessStartInfo` 并执行外部进程。
- 在仓库中，它也为 `CMD` 机制提供了基础启动配置能力。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.ProcessHelper/ProcessHelper.cs`

## 主要能力

- 直接启动进程
  - `StartProcess(string applicationFileFullPath, bool createNoWindow, bool useShellExecute = false, params string[] args)`
  - `StartProcess(ProcessStartInfo processStartInfo)`
- 严格结果模式启动/执行
  - `StartProcessWithResult(...)`
  - `RunProcessWithResult(...)`
- 构造进程启动信息
  - `GetProcessStartInfo(...)`
- 获取当前进程位数
  - `GetCurrentProcessBitOperatingSystemType()`

## 实现特征

- `GetProcessStartInfo(...)` 是模块核心，它把“隐藏窗口模式”和“普通窗口模式”分成两种配置路径。
- 当 `createNoWindow = true` 时，会自动：
  - `RedirectStandardInput = true`
  - `RedirectStandardOutput = true`
  - `RedirectStandardError = true`
  - `UseShellExecute = false`
  - `WindowStyle = Hidden`
- 这说明该模块天然支持“后台命令执行 + 标准流重定向”场景。

## 在仓库中的角色

- `BaseCmd` 直接使用 `ProcessHelper.GetProcessStartInfo("cmd", true)` 作为命令执行底座。
- 因此它虽然位于 Helpers，但实际上是 `CMD` 机制的一个基础依赖点。

## 维护时需要注意

- `StartProcess(ProcessStartInfo)` 只返回布尔值，不返回 `Process` 实例，说明这个模块默认不打算让调用方继续做生命周期控制。
- 新增的结果型入口会显式返回：
  - `IsStarted`
  - `ProcessId`
  - `HasExited`
  - `ExitCode`
  - `Exception`
  - `ErrorMessage`
  - `CreateNoWindow`
  - `UseShellExecute`
  - `WaitedForExit`
- 参数拼接使用 `string.Join(" ", args)`，对带空格、引号、转义要求高的参数场景不算特别稳。
- 兼容层 `StartProcess(...)` 仍保留“布尔返回”语义；需要诊断信息时，应优先使用结果型入口。
- 当前与 `BaseCmd` 的真实耦合点仍只有 `GetProcessStartInfo("cmd", true)` 这一层启动配置复用；`BaseCmd` 没有直接切到 `ProcessResultModel`，本轮也未额外扩散内部 adoption。

## 风险点

- 由于不返回进程实例，调用方很难直接等待退出、读取退出码或做更细粒度的进程控制。
- 字符串级参数拼接在复杂命令参数场景下可能产生歧义。
- 当前结果型入口已经能覆盖“启动是否成功”和“退出码是否为 0”，但还没有超时、取消等执行控制。

## 后续建议

- 可继续补一份“ProcessHelper 与 CMD.Abstractions 的关系”专题。
- 若后续要继续增强能力，优先沿结果型入口补超时、取消和更安全的参数拼接，而不是直接破坏现有布尔兼容层。
