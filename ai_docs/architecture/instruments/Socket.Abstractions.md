# Socket.Abstractions

本文档用于分析 `Lanymy.Common.Instruments.Socket.Abstractions` 模块。

## 模块定位

- 这是 `Socket` 体系的真正基础层。
- 如果说 [Socket.md](./Socket.md) 说明了模块全貌，那么这里更关注“抽象层本身如何组织”。
- 当前可以明确看到它覆盖了 TCP Server、TCP Client、UDP Client、会话模型、包过滤、事件接口，以及一条尚未完成的 Async 支线。

## 当前代码入口

- 同步主线
  - `BaseTcpServer.cs`
  - `BaseTcpServerClient.cs`
  - `BaseTcpClient.cs`
  - `BaseUdpClient.cs`
  - `BaseFixedHeaderPackageFilter.cs`
  - `BaseSessionToken.cs`
- 异步支线
  - `SocketAsync/BaseAsyncTcpServer.cs`

## 抽象层主结构

- `BaseSessionToken`
  - 维护 `SessionID`、IP、端口、连接时间、最近收发时间、发送序号等会话态信息。
- `BaseFixedHeaderPackageFilter`
  - 负责固定头协议下的半包、粘包拆解，以及包校验、编解码、心跳包生成。
- `BaseTcpServer`
  - 负责监听、接入、会话创建、服务端客户端管理、服务端发送、心跳检查。
- `BaseTcpServerClient`
  - 负责单连接的收发、心跳定时器、发送队列、关闭流程。
- `BaseTcpClient`
  - 负责主动连接型 TCP 客户端逻辑。
- `BaseUdpClient`
  - 负责 UDP 收发与广播，以及收发工作队列。

## 关键机制关系

- `BaseTcpServer` 和 `BaseTcpServerClient` 是配套设计：
  - server 管全局连接和接入
  - server client 管单连接生命周期
- `BaseTcpClient` 复用了相同的包过滤思路，但面向主动连接场景。
- `BaseUdpClient` 则没有连接态，但仍沿用了：
  - 统一包过滤器
  - 发送工作队列
  - 接收工作队列
- `BaseSessionToken` 通过 `SendNum => _SendNum++` 提供递增发送序号，供协议层打包使用。

## 当前实现特征

- 同步主线虽然名字上是“同步”，但内部已经混用了：
  - `BeginRead`
  - `WriteAsync`
  - `WorkTaskQueue`
  - `TimerWorkTask`
  这是一种“事件驱动 + 队列串行化”的实现风格。
- 抽象层大量依赖模板方法：
  - `OnConnectionEvent`
  - `OnReceivePackageEvent`
  - `OnServerReceivePackageEvent`
  - `OnCloseEvent`
  由子类补业务逻辑。

## 当前值得记录的风险点

- 抽象层里吞异常较多，很多错误最后只会触发关闭，不一定能完整保留现场。
- `BaseTcpClient` 明确不支持重启：
  - `_IsFirstStart` 关闭后会禁止再次 `Start()`
- `BaseUdpClient.Close()` 的队列释放条件方向问题已修复，当前关闭路径会在队列非空时执行停止与释放。

## Async 支线现状

- `SocketAsync/BaseAsyncTcpServer.cs` 当前几乎整体处于注释和 `NotImplementedException` 状态。
- 这说明仓库确实尝试过异步抽象支线，但它目前不能被视为成熟能力。
- 因此当前可依赖的主线仍然是同步抽象层这套实现。

## 阅读建议

- 如果要理解完整服务端链路，建议顺序阅读：
  - `BaseSessionToken`
  - `BaseFixedHeaderPackageFilter`
  - `BaseTcpServerClient`
  - `BaseTcpServer`
- 如果要理解主动连接或广播场景，再补读：
  - `BaseTcpClient`
  - `BaseUdpClient`

## 后续建议

- 可继续补一份“Socket 抽象层关系图”可视化文档。
- 若后续进入逻辑复验阶段，`BaseUdpClient.Close()` 和 `BaseTcpClient` 的重启限制都是值得优先确认的点。
- 当前已补充关系型文档见 [Socket-Relations.md](./Socket-Relations.md)。
- 当前已补充生命周期专题见 [Socket-Lifecycle.md](./Socket-Lifecycle.md)。
- 当前第二轮系统性体检记录见 [../../debugs/review_round_02.md](../../debugs/review_round_02.md)。
