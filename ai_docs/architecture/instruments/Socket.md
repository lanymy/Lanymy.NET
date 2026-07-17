# Socket

本文档用于分析 `Lanymy.Common.Instruments.Socket.*` 模块族的结构与典型实现。

## 模块定位

- 这是仓库中一支比较完整的通信机制模块。
- 结构上明显分为：
  - 抽象层 `Socket.Abstractions`
  - 演示/协议示例层 `Socket.ImplementDemo`
- 从当前代码看，它承载的是 TCP/UDP 通信框架、会话态、粘包拆包、心跳与发送队列等一整套机制。

## 当前代码入口

- 抽象层代表文件
  - `src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServer.cs`
  - `src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServerClient.cs`
  - `src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseFixedHeaderPackageFilter.cs`
- Demo 层代表文件
  - `src/Commons/Lanymy.Common.Instruments.Socket.ImplementDemo/LanymyTcpServer.cs`
  - `src/Commons/Lanymy.Common.Instruments.Socket.ImplementDemo/LanymyFixedHeaderPackageFilter.cs`

## 结构分层

- `BaseTcpServer`
  - 管理监听、接入、会话创建、心跳检测、客户端字典、发送封包等服务器侧主流程。
- `BaseTcpServerClient`
  - 管理单连接的接收、发送、心跳定时器、发送队列与关闭逻辑。
- `BaseFixedHeaderPackageFilter`
  - 负责固定头协议下的拆包、粘包缓存、编码解码。
- Demo 层
  - 给出一个具体协议实现，演示如何继承这些抽象类完成业务化封包与状态处理。

## 关键设计点

- 这是一个“框架型 Socket 机制”，不是单纯对 `System.Net.Sockets.Socket` 的轻包装。
- `BaseTcpServer` 用 `ConcurrentDictionary<Guid, ITcpServerClient>` 维护会话连接。
- `BaseTcpServerClient` 内部结合：
  - `NetworkStream`
  - `TimerWorkTask`
  - `WorkTaskQueue<byte[]>`
  形成收、发、心跳三条运行链。
- `BaseFixedHeaderPackageFilter` 通过 `BufferModel + CacheModel` 处理半包、粘包与递归拆包。

## Demo 层体现出的使用方式

- `LanymyFixedHeaderPackageFilter`
  - 使用 4 字节头
  - 通过固定规则校验包头和校验和
  - 提供心跳包字节
- `LanymyTcpServer`
  - 通过继承 `BaseTcpServer` 绑定具体的包模型、发送包模型、会话模型
  - 仅在部分事件中写了真实逻辑，仍有多个 `NotImplementedException`
  - 更像协议示例或半成品参考实现，而不是完整可复用成品

## 维护时需要注意

- `Socket` 模块抽象层很厚，修改时不能只改 server 或只改 filter，需要把会话、包过滤、发送队列一起看。
- `BaseTcpServer` 和 `BaseTcpServerClient` 中存在大量吞异常路径，运行期问题可能不容易直接暴露。
- Demo 层里多个回调仍是 `NotImplementedException`，说明它不能被误判成“可直接上线的完整实现”。
- 当前心跳、发送频率、缓冲区大小都通过构造参数控制，后续扩展时应尽量保持这种参数化思路。

## 风险点

- `OnServerClientReceiveDataLoopEvent(...)` 已改成循环实现，当前这条递归深度风险已移除。
- 连接生命周期管理比较复杂，涉及 socket、stream、timer、queue 多对象协同关闭。
- 由于存在 demo 协议实现，接手者容易把“示例代码”和“公共机制”混在一起理解。

## 后续建议

- 下一轮很适合补一份“Socket 抽象层关系图”。
- 也可以单独再拆一篇 “Socket Demo 协议实现说明”，把 `ImplementDemo` 与真正可复用基础层明确分开。
