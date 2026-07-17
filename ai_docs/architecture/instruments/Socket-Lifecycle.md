# Socket-Lifecycle

本文档用于专题说明 `Socket` / `Netty` 相关模块的生命周期、关闭链、重连链和当前高风险点。

## 1. 适用范围

- `Lanymy.Common.Instruments.Socket.Abstractions`
- `Lanymy.Common.Instruments.Socket.Netty.Abstractions`

## 2. 当前主线分类

### 2.1 传统抽象主线

- `BaseTcpServer`
- `BaseTcpServerClient`
- `BaseTcpClient`
- `BaseUdpClient`

### 2.2 Netty 支线

- `BaseNettySocketServer`
- `BaseNettySocketClient`
- `BaseClientChannelHandler`
- `BaseChannelInitializer`

## 3. 当前生命周期特征

### 3.1 传统抽象主线

- 典型模式是：
  - 启动时创建 `Socket` / `NetworkStream` / 工作队列
  - 运行时通过回调和发送队列协作
  - 关闭时串行停止队列、释放流、关闭 socket、触发事件
- 当前实现更像“事件驱动 + 队列串行化”，不是纯同步也不是纯异步。

### 3.2 Netty 支线

- 启动时创建 `EventLoopGroup` 与 `Bootstrap`
- 连接失败后会按重连逻辑继续尝试连接
- `ChannelInactive` 也可能触发再次重连

## 4. 当前高风险点

### 4.1 关闭链过长且异常被吞

- `BaseUdpClient`、`BaseTcpClient`、`BaseTcpServerClient`、`BaseTcpServer` 的关闭链都包含多个资源释放步骤。
- 当前大量使用分段 `try/catch {}`。
- 这会导致调用方难以判断“对象已关闭”是否等于“资源已完全释放”。

关键代码：

- [BaseUdpClient.cs:L274-L354](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs#L274-L354)
- [BaseTcpClient.cs:L337-L437](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs#L337-L437)
- [BaseTcpServerClient.cs:L415-L558](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServerClient.cs#L415-L558)
- [BaseTcpServer.cs:L448-L514](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServer.cs#L448-L514)

### 4.2 同步阻塞异步

- 当前主线中大量出现 `.Wait()`。
- 这些阻塞点多位于：
  - 队列启动/停止
  - 发送路径
  - 关闭路径
- 这会直接影响吞吐、停机速度和死锁风险。

关键代码：

- [BaseTcpClient.cs:L210-L214](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs#L210-L214)
- [BaseTcpClient.cs:L316-L322](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs#L316-L322)
- [BaseUdpClient.cs:L152-L160](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs#L152-L160)
- [BaseUdpClient.cs:L293-L310](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs#L293-L310)

### 4.3 Netty 客户端重连链无取消边界

- `BaseNettySocketClient.OnStartAsync()` 中使用 `Task.Run(ConnectToServerAsync)`。
- `ConnectToServerAsync()` 失败后通过 `await Task.Delay(...); await ConnectToServerAsync();` 递归重连。
- `BaseClientChannelHandler.OnChannelInactive()` 也会再次调用重连动作。
- 这使得重连链存在多入口、长寿命和停止不彻底的风险。

关键代码：

- [BaseNettySocketClient.cs:L46-L140](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Client/BaseNettySocketClient.cs#L46-L140)
- [BaseClientChannelHandler.cs:L38-L45](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Client/BaseClientChannelHandler.cs#L38-L45)

### 4.4 状态可观测性弱

- 当前很多失败最终都会进入 `OnError -> Close()`。
- 但原始错误与关闭过程中的二次错误常常被吞掉。
- 这会让“业务错误”“网络断开”“关闭失败”混在一起。

## 5. 当前建议

1. 先处理 Netty 客户端重连边界。
2. 再梳理 `Socket.Abstractions` 的关闭链，明确每段资源的所有权和失败行为。
3. 再减少关闭路径中的 `.Wait()` 与空 `catch`。
4. 最后根据需要补更细的：
   - TCP Client 生命周期图
   - TCP ServerClient 生命周期图
   - UDP Client 生命周期图

## 6. 关联文档

- 模块概览见 [Socket.md](./Socket.md)
- 抽象层说明见 [Socket.Abstractions.md](./Socket.Abstractions.md)
- 总体体检见 [../../debugs/review_round_02.md](../../debugs/review_round_02.md)
- 分阶段治理路线见 [../../debugs/remediation_roadmap.md](../../debugs/remediation_roadmap.md)
