# PcInfoHelper

本文档用于分析 `Lanymy.Common.Helpers.PcInfoHelper` 模块。

## 模块定位

- 提供本机环境、端口占用、IP 地址以及 Windows 快捷方式相关的辅助能力。
- 模块能力跨度较大，其中快捷方式创建属于明显的外部系统交互入口。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.PcInfoHelper/PcInfoHelper.cs`

## 主要能力

- 主机与 IP 信息
  - `GetHostName`
  - `GetHostNameWithResult`
  - `GetIPV4`
  - `GetIPV4WithResult`
  - `GetIPV6`
  - `GetIPV6WithResult`
  - `GetLocalIpAddress`
  - `GetLocalIpAddressWithResult`
- 端口能力
  - `GetRandomAvaliablePort`
  - `GetRandomAvaliablePorts`
  - `GetLocalInUsedPorts`
  - `IsPortInUsed`
- Windows 快捷方式能力
  - `CreateBootAutoRun`
  - `CreateDesktopShortcut`
  - `CreateShortcut`
  - `CreateBootAutoRunWithResult`
  - `CreateDesktopShortcutWithResult`
  - `CreateShortcutWithResult`

## 本轮收口点

- 为 Windows 快捷方式创建路径补充了严格结果层 `ShortcutOperationResultModel`。
- 为本机网络信息查询补充了严格结果层：
  - `GetHostNameWithResult()`
  - `GetIPV4WithResult()`
  - `GetIPV6WithResult()`
  - `GetLocalIpAddressWithResult()`
- 兼容层 `CreateBootAutoRun(...)` / `CreateDesktopShortcut(...)` / `CreateShortcut(...)` 继续返回 `bool`，保持历史调用方式不变。
- 兼容层 `GetHostName()` / `GetIPV4()` / `GetIPV6()` / `GetLocalIpAddress()` 继续保留原有字符串语义：
  - 查询异常时仍然沿原行为向上抛出
  - 查询成功但没有合适结果时仍返回空字符串
- 需要诊断失败原因时，应优先使用 `Create*WithResult(...)` 系列入口。
- 需要诊断主机名、本机 IPv4 / IPv6、首选本地地址查询失败原因时，应优先使用 `*WithResult(...)` 系列入口。

## 维护时需要注意

- 快捷方式创建依赖 `WScript.Shell` COM 组件，仅适用于 Windows。
- 当前快捷方式严格层会返回：
  - `SourcePath`
  - `TargetPath`
  - `IsSuccess`
  - `Exception`
- 当前网络信息严格层会返回：
  - `HostName`
  - `AddressFamily`
  - `Address`
  - `AddressText`
  - `CandidateAddresses`
  - `IsSuccess`
  - `Exception`
- 兼容层仍然以原有的 `string / bool` 轻量返回为主，因此排障时不要只看旧入口返回值。
- `GetLocalIpAddress()` 只挑选“已启用、带网关、非回环”的首个 IPv4 地址，在多网卡、VPN、容器场景下不一定等于调用方真正想要的地址。

## 后续建议

- 若未来继续增强该模块，建议把“环境信息查询”和“Windows Shell 操作”拆成两个更聚焦的专题看待，避免职责继续发散。
