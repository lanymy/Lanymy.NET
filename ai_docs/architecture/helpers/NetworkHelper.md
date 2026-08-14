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
  - `PingIPWithResult(string ip)`
  - `PingIPWithResult(IPAddress ip)`
- IP 字符串与 `IPAddress` 转换
  - `GetIpAddressByIpString`
  - `GetIpAddressByIpStringWithResult`
- 本地地址获取
  - `GetLocalIpList`
  - `GetLocalIpListWithResult`
  - `GetLocalIpV4List`
  - `GetLocalIpV4ListWithResult`
  - `GetLocalIpV6List`
  - `GetLocalIpV6ListWithResult`
  - `GetLocalIP`
  - `GetLocalIPWithResult`
- 地址字节转换
  - `GetMacAddressBytes`
  - `GetIpAddressBytes`

## 实现特征

- 以同步方法为主，没有引入异步网络探测模型。
- `PingIP` 使用 `Ping.Send(..., 5000)`，固定超时 5 秒。
- 本地地址列表通过 `Dns.GetHostEntry(Dns.GetHostName())` 获取，逻辑直接、兼容优先。
- `GetIpAddressByIpString` 历史上只覆盖 IPv4 点分十进制输入；当前严格层也沿用了这一约束，没有把能力悄悄扩到 IPv6。
- 文件中保留了大量 `#if !NETSTANDARD` 下的注释代码，说明该模块曾经承载过更多局域网扫描和网关相关能力，但当前主体已收缩。

## 维护时需要注意

- `GetLocalIP()` 只取第一个 IPv4 地址，这在多网卡、多 VPN、容器环境下不一定符合真实业务预期。
- 兼容层 `PingIP(...)` 失败时仍直接返回 `false`，保持历史语义。
- 需要失败原因、`IPStatus` 和耗时信息时，应优先使用 `PingIPWithResult(...)`。
- `GetLocalIP()` 兼容层继续保留“查询失败返回空字符串”的历史语义；需要诊断时应走严格层。
- `GetIpAddressByIpString` 兼容层仍保留非法输入直接抛异常的历史语义；需要诊断时应优先使用严格层。
- 历史注释代码很多，后续如果要复活相关能力，建议先做专题整理，不要直接取消注释。

## 本轮收口点

- 为 `PingIP(...)` 增加了严格结果层 `PingIPWithResult(...)`。
- 新结果模型会显式返回：
  - `AddressText`
  - `Address`
  - `IsSuccess`
  - `Status`
  - `RoundtripTime`
  - `Exception`
- 这使得网络探测场景不再只有“成功/失败”布尔语义，而能区分“地址格式错误”和“Ping 未成功”。
- 为 IP 解析与本机地址查询补齐了严格结果层：
  - `GetIpAddressByIpStringWithResult(...)`
  - `GetLocalIpListWithResult()`
  - `GetLocalIpV4ListWithResult()`
  - `GetLocalIpV6ListWithResult()`
  - `GetLocalIPWithResult()`
- 新结果模型现在能显式区分：
  - 参数格式错误
  - DNS/本机地址枚举异常
  - 查询成功但没有拿到首个 IPv4 地址

## 风险点

- 本地 IP 获取逻辑在复杂网络环境下容易“能跑但不准”。
- 该模块名字较大，但当前实际能力范围较窄，容易让接手者误判它能处理更复杂网络问题。
- 严格层已经补齐到“连通性探测 + 地址解析 + 本机地址查询”，但当前仍是同步 API，后续如果引入更重的网络探测逻辑，需要重新评估超时和调用阻塞成本。

## 后续建议

- 可补一份“NetworkHelper 与 Socket 模块的职责边界”专题。
- 若未来要增强网络诊断能力，建议拆成“基础地址工具”和“网络探测工具”两类，而不是继续堆在同一个 Helper 中。
