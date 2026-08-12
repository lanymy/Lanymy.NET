# Socket-Lifecycle

本文档用于专题说明 `Socket` / `Netty` 相关模块的生命周期、关闭链、重连链，以及当前已经沉淀下来的硬约束。

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
- `StartAsync()` 只负责把 host 切到运行态并启动首个连接/监听准备，不承诺“返回时已建立业务连接”
- client 重连链采用“代际编号 + 单活 reconnect task”模型：
  - `BeginReconnectGeneration()` 为每轮启动生成新的 reconnect generation
  - `EnsureReconnectLoopStarted(...)` 保证同一 generation 内最多只有一个 reconnect task 在跑
  - `BaseClientChannelContext.CurrentConnectToServerAction` 必须保持强引用委托，不能退回 `WeakReference<Action<long>>` 这类弱引用模型，否则正常断线后的重连能力会受 GC 时机影响
  - `BaseClientChannelHandler.OnChannelInactive(...)` 只回放自己构造时捕获的 generation，避免旧 channel 在新一轮启动后误触发重连
- `BaseSocketHost` 通过 `_LifecycleSemaphore` 串行化 `StartAsync()` / `StopAsync()` / `DisposeAsync()`，因此停止、释放、重启之间不会并发踩状态

## 4. 当前硬约束

这一节记录的是后续继续改 `Socket.Abstractions` / `Netty` 主线时默认不能退回去的行为契约。

### 4.1 同步桥接约束

- `Start()` / `Close()` / `Send()` 这类同步入口如果内部桥接异步流程，必须统一通过 `TaskHelper.TrySyncWait(...)` 或同等语义 helper 收口。
- 同步桥接失败后不能直接把 `AggregateException` / 桥接异常裸抛给调用方，而要进入各自的错误通道：
  - client 侧走 `OnError` / `OnCloseError` / `ReportError`
  - server 侧走 `OnServerCloseError` / `ReportServerError`
- 启动链上的同步桥接如果失败，必须进入启动回滚，不能把对象留在“`IsRunning=true` 但内部队列 / socket / stream 未完全就绪”的半初始化状态。
- `Dispose()` 可以感知 `CloseAsync()` 失败，但不能依赖公开 `Close()` 的抛异常语义中断后续 fallback 释放。

关键代码：

- [TaskHelper.cs:L10-L35](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/TaskHelper.cs#L10-L35)
- [BaseTcpClient.cs:L233-L246](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs#L233-L246)
- [BaseTcpClient.cs:L423-L430](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs#L423-L430)
- [BaseTcpServerClient.cs:L265-L278](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServerClient.cs#L265-L278)
- [BaseTcpServer.cs:L905-L918](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServer.cs#L905-L918)
- [BaseUdpClient.cs:L147-L160](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs#L147-L160)

### 4.2 业务回调隔离约束

- 所有业务 callback 都必须和 socket 生命周期动作解耦，至少包括：
  - `ReceiveData`
  - `ReceivePackage`
  - `Heart`
  - `StartReceive`
  - `OnCloseEvent`
- callback 抛异常时，应走 error 通道上报，而不能直接打断核心收包、发送、心跳或关闭链。
- server 管理 child client 时，回调前后都要校验“是否仍处于托管态”，避免已脱管 child 在关闭交界期继续触发业务通知。
- 事件通知与事件句柄清理要拆开，保证外部回调抛异常时不会阻断最终解绑。

关键代码：

- [BaseTcpClient.cs:L94-L157](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs#L94-L157)
- [BaseTcpServerClient.cs:L145-L211](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServerClient.cs#L145-L211)
- [BaseTcpServerClient.cs:L709-L745](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServerClient.cs#L709-L745)
- [BaseTcpServer.cs:L231-L248](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServer.cs#L231-L248)
- [BaseTcpServer.cs:L662-L699](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServer.cs#L662-L699)
- [BaseTcpServer.cs:L731-L810](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServer.cs#L731-L810)
- [BaseUdpClient.cs:L72-L145](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs#L72-L145)

### 4.3 `CloseAsync()` / `Dispose()` 尾声清理约束

- 关闭链必须先抓资源快照，再分段执行 stop / dispose / socket 关闭 / 最终事件通知，避免并发交错时清理对象漂移。
- send queue、heart timer、receive queue、socket / stream 这些资源要拆开清理：
  - `StopAsync()` 失败不能阻断 `Dispose()`
  - 某一个资源释放失败不能阻断其余资源最终释放
- 状态位和字段清空要放在尾声收口处统一处理；异常是否上抛或上报，放在清理动作之后决定。
- server 关闭 child client 时，即使单个 child `CloseAsync()` 失败，也要继续清理其余 child，并对失败 child 做补偿 `Dispose()`。

关键代码：

- [BaseTcpClient.cs:L524-L765](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs#L524-L765)
- [BaseTcpServerClient.cs:L602-L897](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServerClient.cs#L602-L897)
- [BaseTcpServer.cs:L366-L397](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServer.cs#L366-L397)
- [BaseTcpServer.cs:L921-L1000](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServer.cs#L921-L1000)
- [BaseUdpClient.cs:L591-L840](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs#L591-L840)

### 4.4 接收链连续性约束

- `BaseTcpClient` / `BaseTcpServerClient` 的接收回调采用“流快照 + 连续性校验 + 关闭期异常忽略”模型。
- 旧 `NetworkStream`、已摘除 stream、关闭交界期的 `ObjectDisposedException` / `IOException` / 中止类 `SocketException`，都应按关闭噪音处理，而不是重新当作运行期故障放大。
- 这一约束的目标是把真正的协议 / 网络错误与关闭噪音分开，避免误打空引用或重复关闭。

关键代码：

- [BaseTcpClient.cs:L315-L374](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs#L315-L374)
- [BaseTcpServerClient.cs:L466-L536](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServerClient.cs#L466-L536)
- [TcpReceiveGuardHelper.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/TcpReceiveGuardHelper.cs)

### 4.5 Netty host / reconnect 约束

- `BaseSocketHost.StartAsync()` / `StopAsync()` / `DisposeAsync()` 必须继续保持信号量串行化语义：
  - `StopAsync()` 执行期间，新的 `StartAsync()` 必须等待 stop 尾声完成
  - `DisposeAsync()` 与 `StopAsync()` 并发时，`DisposeAsync()` 只能在 stop 完成后继续 dispose，且不能重复 stop
- `BaseNettySocketClient` 的 reconnect 链必须满足：
  - 同一 generation 内只能有一个活动 reconnect task
  - reconnect 入口委托必须是稳定可用的强引用，不能因为 GC 回收掉委托对象而静默失去重连能力
  - `StopAsync()` 必须先把 `IsRunning` 置为 `false`，再关闭 channel、取消 reconnect token、等待 reconnect task 尾声
  - 旧 generation 的 `ChannelInactive` 不能在新 generation 已启动后重新拉起旧重连链
  - `ConnectChannelAsync()` 返回异常或返回 `null channel` 时，都必须进入统一的“上报 + 延迟 + 是否继续重连”分支，不能静默丢失重连
- `BaseChannelHandler` / `BaseClientChannelHandler` / `BaseServerChannelHandler` 的 callback 必须满足：
  - handler 业务回调抛异常时，只能进入 `OnHandlerError(...)`，不能直接打断底层 close / reconnect / read complete 主链
  - `ChannelInactive` 清理当前状态后，server 侧只能移除“当前 handler 仍持有”的 session 映射，不能误删已被新 handler 接管的会话槽位
  - `BaseNettySocketServer.OnStopAsync()` 尾声必须清空 `CurrentChannelDictionary`，不能把已停止实例的旧 handler 映射留在 server context 里
  - `BaseChannelHandler.OnContextClose(...)` 必须对同一 active channel 做单次关闭请求防抖，避免 idle timeout、transport exception、主动 stop 叠加时重复调度 close；若单次 close 请求明确失败，则允许后续再次触发重试

关键代码：

- [BaseSocketHost.cs:L70-L199](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Common/BaseSocketHost.cs#L70-L199)
- [BaseNettySocketClient.cs:L51-L355](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Client/BaseNettySocketClient.cs#L51-L355)
- [BaseClientChannelHandler.cs:L16-L45](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Client/BaseClientChannelHandler.cs#L16-L45)
- [BaseChannelHandler.cs:L51-L582](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Common/BaseChannelHandler.cs#L51-L582)
- [BaseServerChannelHandler.cs:L24-L52](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Server/BaseServerChannelHandler.cs#L24-L52)

## 5. 当前高风险点

### 5.1 Netty 客户端重连链仍是当前最高优先级风险面

- 当前实现已经从早期“多入口递归重连”收敛到了 generation-gated 单活 reconnect task。
- 但 Netty client 仍然是当前最容易出现长寿命竞态的位置，因为它同时承载：
  - 启动期建链
  - 运行期断线重连
  - stop/dispose 尾声取消与等待
- 后续继续审查时，优先关注：
  - connect 中失败是否都能进入统一上报/重试分支
  - stop 与 reconnect 并发时，是否还有残留 task / token / channel host
  - 旧 channel 的 `ChannelInactive` 是否还能影响新 generation

关键代码：

- [BaseNettySocketClient.cs:L51-L355](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Client/BaseNettySocketClient.cs#L51-L355)
- [BaseClientChannelHandler.cs:L38-L45](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Client/BaseClientChannelHandler.cs#L38-L45)

### 5.2 传统主线仍有历史阻塞模式残留

- 当前主线虽然已经把最脆弱的同步桥接和关闭尾声收住，但历史上的 `.Wait()`、同步阻塞异步思路仍会继续放大吞吐、停机速度和排障复杂度问题。

### 5.3 状态可观测性仍可继续增强

- 当前很多失败最终仍会汇入 `OnError -> Close()`。
- 现阶段已经能把关闭尾声中的二次错误继续上报，但“原始业务错误”和“关闭补偿错误”仍然可以在后续文档和日志模型中进一步分层。

## 6. 当前建议

1. 传统 `Socket.Abstractions` 主线当前可暂时从“生命周期竞态治理”切到“重复模式收敛与文档沉淀”。
2. 优先继续处理 Netty 客户端重连边界，因为它仍是当前最明显的长寿命生命周期风险点。
3. 再减少关闭路径中的 `.Wait()` 与空 `catch`。
4. 最后根据需要补更细的：
   - TCP Client 生命周期图
   - TCP ServerClient 生命周期图
   - UDP Client 生命周期图

## 7. 关联文档

- 模块概览见 [Socket.md](./Socket.md)
- 抽象层说明见 [Socket.Abstractions.md](./Socket.Abstractions.md)
- 主干时序见 [Socket-Sequences.md](./Socket-Sequences.md)
- 异常分支时序见 [Socket-Exception-Sequences.md](./Socket-Exception-Sequences.md)
- 总体体检见 [../../debugs/review_round_02.md](../../debugs/review_round_02.md)
- 分阶段治理路线见 [../../debugs/remediation_roadmap.md](../../debugs/remediation_roadmap.md)
