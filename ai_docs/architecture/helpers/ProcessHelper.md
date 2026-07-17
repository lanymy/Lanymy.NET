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
- 参数拼接使用 `string.Join(" ", args)`，对带空格、引号、转义要求高的参数场景不算特别稳。
- 当前异常统一吞掉并返回 `false`，适合轻调用，但不利于定位失败原因。

## 风险点

- 由于不返回进程实例，调用方很难直接等待退出、读取退出码或做更细粒度的进程控制。
- 字符串级参数拼接在复杂命令参数场景下可能产生歧义。

## 后续建议

- 可继续补一份“ProcessHelper 与 CMD.Abstractions 的关系”专题。
- 若后续要增强能力，适合新增“返回 `Process` / 退出码 / 详细异常”的严格模式接口，而不是直接改现有布尔返回语义。
