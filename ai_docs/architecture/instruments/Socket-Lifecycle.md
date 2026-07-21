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
- 当前已开始第一步治理：`BaseTcpClient`、`BaseTcpServerClient`、`BaseTcpServer` 的关闭阶段异常已改为按步骤上报，而不是纯吞掉。

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

### 4.5 TCP 主线已收敛约束

- `BaseTcpServer` 当前已补齐启动失败回滚、`CloseAsync()` 后可重启、启动中 `CloseAsync()` 可取消本轮启动这三类基础契约。
- `BaseTcpServer` 对子连接事件已增加“托管态”过滤：
  - 已脱管客户端的 `Error` / `Close` / `StartReceive` / `ReceiveData` / `Heart` 回调会被直接忽略。
  - 服务端主动关闭子连接前会先解绑事件，避免子连接 `CloseEvent` 反向触发重复关闭。
- `BaseTcpServer` 的收包循环与公开发送入口已增加托管态保护：
  - 未托管客户端不会再继续进入 `ReceiveDataLoop` 的包处理阶段。
  - 外部保留下来的旧客户端实例不能再通过 `SendDataBytes(ITcpServerClient, ...)` 继续发送。
- `BaseTcpServerClient` 与 `BaseTcpClient` 的接收回调已统一采用“流快照 + 连续性校验 + 关闭期异常忽略”模型：
  - 旧 `NetworkStream` / 已清空 `NetworkStream` 的回调不会再误打空引用。
  - 关闭交错期间的 `ObjectDisposedException`、`IOException` 和中止类 `SocketException` 会被视为关闭噪音而不是运行期错误。
- `BaseTcpServerClient` 的关闭尾声已拆分为：
  - 内部 `OnCloseEvent()`
  - 外部 `CloseEvent`
  - 事件句柄清理
- 这保证了：
  - 内部关闭回调异常不会阻断外部关闭通知。
  - 外部关闭事件异常不会阻断事件句柄最终清零。

关键代码：

- [BaseTcpServer.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServer.cs)
- [BaseTcpServerClient.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServerClient.cs)
- [BaseTcpClient.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs)
- [TcpReceiveGuardHelper.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/TcpReceiveGuardHelper.cs)

## 5. 当前建议

1. 传统 `Socket.Abstractions` 主线当前可暂时从“生命周期竞态治理”切到“重复模式收敛与文档沉淀”。
2. 优先继续处理 Netty 客户端重连边界，因为它仍是当前最明显的长寿命生命周期风险点。
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
