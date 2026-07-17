# NetworkHelper

本文档用于分析 `Lanymy.Common.Helpers.NetworkHelper` 模块。

## 模块定位

- 提供一组偏基础、偏本机环境探测的网络辅助方法。
- 与 `HttpHelper` 不同，这里不处理 HTTP 协议，而是聚焦 IP、Ping、本地地址枚举、MAC/IP 文本转换等能力。
- 当前实现明显偏工具箱风格。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.NetworkHelper/NetworkHelper.cs`

## 主要能力

- 连通性探测
  - `PingIP(string ip)`
  - `PingIP(IPAddress ip)`
- IP 字符串与 `IPAddress` 转换
  - `GetIpAddressByIpString`
- 本地地址获取
  - `GetLocalIpList`
  - `GetLocalIpV4List`
  - `GetLocalIpV6List`
  - `GetLocalIP`
- 地址字节转换
  - `GetMacAddressBytes`
  - `GetIpAddressBytes`

## 实现特征

- 以同步方法为主，没有引入异步网络探测模型。
- `PingIP` 使用 `Ping.Send(..., 5000)`，固定超时 5 秒。
- 本地地址列表通过 `Dns.GetHostEntry(Dns.GetHostName())` 获取，逻辑直接、兼容优先。
- 文件中保留了大量 `#if !NETSTANDARD` 下的注释代码，说明该模块曾经承载过更多局域网扫描和网关相关能力，但当前主体已收缩。

## 维护时需要注意

- `GetLocalIP()` 只取第一个 IPv4 地址，这在多网卡、多 VPN、容器环境下不一定符合真实业务预期。
- `PingIP` 失败时直接返回 `false`，不会暴露具体错误原因。
- `GetIpAddressByIpString` 通过手动拆段构造 `IPAddress`，对异常输入的健壮性不如直接走框架标准解析。
- 历史注释代码很多，后续如果要复活相关能力，建议先做专题整理，不要直接取消注释。

## 风险点

- 本地 IP 获取逻辑在复杂网络环境下容易“能跑但不准”。
- 该模块名字较大，但当前实际能力范围较窄，容易让接手者误判它能处理更复杂网络问题。

## 后续建议

- 可补一份“NetworkHelper 与 Socket 模块的职责边界”专题。
- 若未来要增强网络诊断能力，建议拆成“基础地址工具”和“网络探测工具”两类，而不是继续堆在同一个 Helper 中。
