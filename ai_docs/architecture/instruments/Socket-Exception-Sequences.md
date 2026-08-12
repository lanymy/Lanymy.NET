# Socket-Exception-Sequences

本文档用于补充 `Socket` 传统抽象主线里的异常分支时序。

## 文档目的

- [Socket-Sequences.md](./Socket-Sequences.md) 解决的是主干运行时序。
- [Socket-Lifecycle.md](./Socket-Lifecycle.md) 解决的是生命周期硬约束。
- 本页重点补充“失败时怎么收口”，尤其是最近几轮治理里已经明确收住的异常分支。

## 阅读方式

- 如果先想看正常主干路径，先看 [Socket-Sequences.md](./Socket-Sequences.md)。
- 如果先想看契约，再回来看异常分支为什么这样收口，先看 [Socket-Lifecycle.md](./Socket-Lifecycle.md)。
- 本页适合排查“为什么没有成功启动 / 为什么关闭后还继续清理 / 为什么异常没有直接抛回调用方”这类问题。

## 服务端接入失败分支

### accepted client 初始化失败

```text
BeginAcceptAsync()
  ↓
CreateTcpServerClient(socket)
  ↓
AttachTcpServerClientEventHandlers(client)
  ↓
TryInitializeAcceptedClient(client)
  ├── 成功：继续 StartReceiveAsync()
  └── 失败：CleanupAcceptedClientInitializationFailure(client)
            ↓
          从 _TcpServerClientDic 移除
            ↓
          DetachTcpServerClientEventHandlers(client)
            ↓
          client.Dispose()
```

### child 接收启动失败

```text
await tcpServerClient.StartReceiveAsync()
  ├── 抛异常
  └── 或返回后 IsRunning == false
        ↓
      CleanupAcceptedClientInitializationFailure(client)
        ↓
      从托管字典移除并 Dispose()
        ↓
      本次 accept 结束，不对业务层触发 OnAcceptEvent(...)
```

### 当前收口点

- failed accepted client 不会残留在 `_TcpServerClientDic`。
- 失败 child 不会继续触发后续 `ReceiveData` / `Heart` / `Close` 业务通知。
- 服务端本身不会因为单个 child 初始化失败而退出监听主循环。

## 服务端 child 回调异常分支

### 收包回调异常

```text
BaseTcpServer.OnServerClientReceiveDataEvent(...)
  ↓
TryInvokeManagedClientCallback(...)
  ├── 成功：继续进入 ReceiveDataLoop
  └── 抛异常：ReportManagedClientCallbackError(...)
              ↓
            按托管 child error 通道上报
```

### 拆包 / 业务包处理异常

```text
OnServerClientReceiveDataLoopEvent(...)
  ↓
Filter.CheckPackage / DecodePackage / OnServerReceivePackage(...)
  ├── 包非法：HandleServerManagedClientError(...)
  │           ↓
  │         ReportManagedClientError(...)
  │           ↓
  │         CloseTcpServerClient(...)
  └── 业务处理抛异常：ReportManagedClientCallbackError(...)
                      ↓
                    保留托管链错误可观测性
```

### 心跳回调异常

```text
OnServerClientHeartEvent(...)
  ↓
Handle timeout / SendDataBytes(...)
  ↓
TryInvokeManagedClientCallback(...)
  ├── 成功：完成本轮心跳
  └── 抛异常：ReportManagedClientCallbackError(...)
```

### 当前收口点

- 业务 callback 抛异常时，优先进入 error 通道，不再直接打断核心生命周期动作。
- 如果异常意味着 child 已不再可用，则转入 `HandleServerManagedClientError(...)`，统一走“上报 + 关闭 child”链。
- 已脱管 child 后续 callback 会被托管态校验挡住，不再继续污染业务面。

## 同步桥接失败分支

### client / server / udp 同步关闭入口

```text
Close()
  ↓
TrySyncWait(CloseAsync)
  ├── 成功：完成关闭
  └── 失败：OnCloseError(...) / OnServerCloseError(...)
            ↓
          不把桥接异常直接裸抛给调用方
```

### 同步发送入口

```text
Send(...)
  ↓
TrySyncWait(SendAsync)
  ├── 成功：完成入队 / 发送
  └── 失败：OnError(...) / OnServerClientError(...) / ReportError(...)
```

### 启动链同步桥接失败

```text
Start()
  ↓
同步桥接启动内部 queue / timer
  ├── 成功：继续后续启动链
  └── 失败：ResetStartState(...)
            ↓
          释放已创建 socket / stream / queue / timer
            ↓
          恢复到可重试或已关闭状态
```

### 当前收口点

- 同步入口允许桥接异步流程，但桥接失败默认不直接把 `AggregateException` 打回调用方。
- 启动链桥接失败时必须回滚，而不是留下半初始化对象。
- `Dispose()` 不依赖公开 `Close()` 的抛异常语义来决定是否继续 fallback 清理。

## 关闭补偿失败分支

### 单个 child 关闭失败

```text
CloseTcpServerClient(sessionToken)
  ↓
TrySyncWait(client.CloseAsync)
  ├── 成功：完成 child 关闭
  └── 失败：ReportChildClientCloseFailure(...)
            ↓
          CleanupClosedTcpServerClientFailure(client)
            ↓
          client.Dispose()
```

### 服务端整体关闭时 child 关闭失败

```text
OnServerCloseAsync()
  ↓
遍历 child clients
  ↓
await client.CloseAsync()
  ├── 成功：继续下一个 child
  └── 失败：ReportChildClientCloseFailure(...)
            ↓
          CleanupClosedTcpServerClientFailure(client)
            ↓
          继续关闭其余 child
```

### client / udp Dispose 期间 close 失败

```text
Dispose()
  ↓
TrySyncWait(CloseAsync)
  ├── 成功：正常结束
  └── 失败：上报 dispose close failed
            ↓
          继续 fallback 释放 queue / timer / stream / socket / udp client
```

### 当前收口点

- 关闭失败不会自动等价成“停止清理”。
- 当前策略是“保留错误可观测性，同时尽量把对象收回到最终已关闭 / 已释放状态”。
- 这也是最近几轮治理里最核心的一条收敛方向。

## 接收链关闭噪音分支

```text
OnReceive(...)
  ↓
EndRead / BeginRead / stream 操作抛异常
  ├── 旧 stream / 已摘除 stream / 关闭交界期异常
  │     ↓
  │   CanIgnoreReceiveException(...) == true
  │     ↓
  │   直接返回，不再当作新的运行态错误放大
  └── 非关闭噪音异常
        ↓
      OnError(...) / OnServerClientError(...)
```

### 当前收口点

- 关闭噪音与真正运行态故障已分流。
- 目标不是少报错，而是避免把本来正常的关闭交界期误判成新的生命周期异常。

## Netty 建链失败分支

### client connect 期间失败

```text
StartAsync()
  ↓
OnStartAsync()
  ↓
BeginReconnectGeneration()
  ↓
EnsureReconnectLoopStarted(generation)
  ↓
ConnectToServerAsync(generation)
  ↓
ConnectChannelAsync()
  ├── 抛异常
  ├── 返回 null channel
  └── 返回已连接 channel
        ↓
      TryBindConnectedChannel(...)
        ├── 成功：绑定 _CurrentChannelHost，结束本轮 reconnect
        └── 失败：CloseChannelAsync(channel) 并退出本轮
```

### client connect 失败后的收口

```text
ConnectChannelAsync() 抛异常 / 返回 null
  ↓
ShouldReportConnectException(...)
  ├── true：OnConnectError("NettySocketClient connect attempt failed.", ex)
  └── false：仅进入后续重试判定
  ↓
CanContinueReconnect(generation, token)
  ├── false：退出 reconnect task
  └── true：DelayBeforeReconnectAsync(token)
            ↓
          下一轮 connect attempt
```

### 当前收口点

- connect 失败和 `null channel` 都不能静默吞掉，否则会把对象留在“`IsRunning=true` 但没有活动 channel、也不再重连”的假运行态。
- 同一 generation 内始终只允许一个活动 reconnect task。
- 触发 reconnect 的入口委托必须保持强引用；如果把它做成弱引用，`ChannelInactive` 可能在 host 仍存活时因为委托已被 GC 回收而静默失去重连。
- 旧 generation 的失败不能拉起新 generation 的 connect 行为。

## Netty handler 回调异常分支

### callback 隔离

```text
ChannelActive / ChannelInactive / ChannelRead / UserEventTriggered / ExceptionCaught
  ↓
SafeHandleXxx(...)
  ├── 成功：继续主链
  ├── ObjectDisposed / 已关闭 channel 噪音：忽略
  └── 其他异常：OnHandlerError("NettyChannelHandler ... failed.", ex)
```

### reader idle 导致关闭

```text
UserEventTriggered(ReaderIdle)
  ↓
OnReaderTimeOutEventTrigger(...)
  ↓
OnContextClose(context)
  ↓
SafeCloseContextAsync(context)
  ├── 成功：等待 ChannelInactive
  ├── 关闭噪音：忽略
  └── 关闭异常：OnHandlerError("NettyChannelHandler close context failed.", ex)
```

### 当前收口点

- handler 业务回调抛异常时，只上报，不直接打断底层 read/close/reconnect 主链。
- `ChannelActive` 回调失败时，先 `ResetCurrentChannelState()`，再上报并触发 `OnContextClose(...)`，避免半激活状态残留。
- `ChannelInactive` 发生后，当前 handler 的瞬时上下文会先被清空，再进入派生类自己的脱管/重连逻辑。

## Netty channel close 再入分支

```text
OnContextClose(context)
  ↓
SafeCloseContextAsync(context)
  ├── 成功：最终由 ChannelInactive 收尾
  ├── OperationCanceled / ObjectDisposed / 已关闭 channel：忽略
  └── 其他异常：OnHandlerError(...)
```

```text
ChannelInactive(context)
  ↓
ResetCurrentChannelState()
  ↓
BaseClientChannelHandler.OnChannelInactive(...)
  ├── 当前 generation 仍有效：尝试 EnsureReconnectLoopStarted(generation)
  └── generation 已失效 / host 已 stop：请求被忽略
```

### 当前收口点

- `OnContextClose(...)` 可以被 idle timeout、transport exception、外部 stop 等多条路径重复触发，但最终都收敛到 `ChannelInactive`。
- stop 尾声里由 executor / channel close 带出的 `OperationCanceledException` 也按关闭噪音处理，不再额外放大成 handler/init error。
- 关闭再入时，同一 active channel 只允许一个在途 close 请求；如果本次 close 明确失败，后续路径允许再次发起重试。
- 关闭再入时，只允许“有效 generation + host 仍运行”的 client handler 触发重连。

## Netty stop / reconnect 并发分支

```text
StopAsync()
  ↓
BaseSocketHost 持有 _LifecycleSemaphore
  ↓
IsRunning = false
  ↓
OnStopAsync()
  ↓
Cancel reconnect token
  ↓
CloseChannelAsync(currentChannelHost)
  ↓
ShutdownBossGroupAsync(...)
  ↓
AwaitReconnectTaskAsync(currentReconnectTask)
  ↓
Dispose token / ResetStopState(...)
```

```text
StopAsync() 进行中
  ↓
新的 StartAsync() 到来
  ↓
等待 _LifecycleSemaphore
  ↓
只有 stop 尾声完全结束后，新的 StartAsync() 才能进入新 generation
```

### 当前收口点

- stop 与 restart 的主约束不是“谁先抢到线程”，而是 `_LifecycleSemaphore` 保证同一时刻只有一条 host 生命周期主链在跑。
- stop 尾声必须等待当前 reconnect task 收口，再允许新一轮 start 创建新的 boss group / bootstrap / reconnect token。
- server stop 尾声还必须显式清空 `CurrentChannelDictionary`，避免 child `ChannelInactive` 未完全跑完时把旧 handler 映射残留在 context 里。

## 排查建议

1. 如果现象是“单个 child 接不上但服务端还活着”，优先看接入失败分支。
2. 如果现象是“业务回调抛异常但连接没有立刻全断”，优先看回调异常分支。
3. 如果现象是“Close()/Send() 没抛，但对象还是进了错误通道”，优先看同步桥接失败分支。
4. 如果现象是“关闭时有错误日志但资源还是被释放了”，优先看关闭补偿失败分支。
5. 如果现象是“Netty client 看起来还在运行，但既没连接上也不再继续重连”，优先看 Netty 建链失败分支。
6. 如果现象是“Stop 后立刻 Start，结果状态怪异或重连任务残留”，优先看 Netty stop / reconnect 并发分支。
