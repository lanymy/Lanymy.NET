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
  └── 失败 / helper 直接抛出：OnCloseError(...) / OnServerCloseError(...)
            ↓
          不把桥接异常直接裸抛给调用方
```

- 同样的规则也适用于同步 `Send(...)` 入口，以及 `OnError(...)` / `OnServerClientError(...)` 内部触发的 `TrySyncWait(CloseAsync)` 收口；这些桥接 helper 即使直接抛异常，也只能继续进入错误通道，不能反向打穿调用方。

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
  ├── 返回 inactive channel
  └── 返回已连接 channel
        ↓
      TryBindConnectedChannel(...)
        ├── 成功：绑定 _CurrentChannelHost，结束本轮 reconnect
        └── 失败：CleanupRejectedChannelAsync(channel) 并退出本轮
```

### client connect 失败后的收口

```text
ConnectChannelAsync() 抛异常 / 返回 null / 返回 inactive channel
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

- 如果 `inactive channel` 分支里的补偿关闭再次失败，只能额外通过 `OnConnectError("NettySocketClient close rejected channel failed.", ex)` 上报清理异常。
- 原始的 `NettySocketClient connect returned inactive channel.` 仍必须继续包进 `connect attempt failed` 主链，不能被 cleanup 失败覆盖。

### client connect 成功后因绑定竞争被拒绝

```text
ConnectChannelAsync()
  ↓
返回 active channel
  ↓
TryBindConnectedChannel(...)
  ├── true：绑定 _CurrentChannelHost，结束本轮 reconnect
  └── false
        ↓
      CleanupRejectedChannelAsync(channel)
        ├── cleanup 成功：直接退出本轮 connect attempt
        └── cleanup 失败：OnConnectError("NettySocketClient close rejected channel failed.", ex)
```

- 这条分支表示 channel 已经建立成功，但在真正绑定到 host 时发现 generation 已经过期，或当前 host 已经有更新/更早成功绑定的 active channel。
- 因此它不是新的 connect failure，不能再额外包一层 `OnConnectError("NettySocketClient connect attempt failed.", ex)`，也不能进入 delay 后的下一轮重连。

### client/server bootstrap initializer 构造失败

```text
CreateBootstrap(...)
  ↓
CreateChannelInitializer()
  ├── 抛异常：start fail -> rollback start state
  └── 返回 null：throw InvalidOperationException("NettySocketClient/Server create channel initializer returned null.")
                ↓
              start fail -> rollback start state
```

- initializer 构造失败属于启动阶段错误，必须在 host 自己这一层就被明确包裹并触发回滚，不能把 `null` initializer 直接继续交给 DotNetty。
- 回滚后必须清空 host 持有的 group / bootstrap / channel 状态，保证后续重试启动仍然是干净起点。
- 如果 `BaseChannelInitializer.InitChannel(...)` 在补偿关闭初始化失败的 channel 时，只遇到 `OperationCanceledException`、`ObjectDisposedException`，或“channel 已关闭 / 尚未注册到 event loop”一类 `InvalidOperationException`，这些异常仍按关闭噪音处理；它们不能覆盖原始的 `init channel failed` 根因，也不应再额外升级成新的 init error。

### server bind 返回空 channel

```text
StartAsync()
  ↓
OnStartAsync()
  ↓
BindServerAsync(...)
  ├── 抛异常：start fail -> rollback start state
  └── 返回 null：throw InvalidOperationException("NettySocketServer bind returned null channel.")
                ↓
              start fail -> rollback boss group / worker group / bootstrap
```

- `BindServerAsync(...)` 返回 `null` 也属于启动失败，不能当作“已启动但暂时没 listener”继续放行。
- 否则 host 会进入 `IsRunning=true`，但 `_CurrentChannelHost` 仍为空的假运行态，后续 stop/restart 与故障排查都会被误导。

### server bind 返回 inactive channel

```text
StartAsync()
  ↓
OnStartAsync()
  ↓
BindServerAsync(...)
  ├── 返回 inactive channel
  │     ↓
  │   CleanupRejectedBoundChannelAsync(channel)
  │     ├── cleanup 成功
  │     └── cleanup 失败：OnStartError("NettySocketServer close rejected bound channel failed.", ex)
  │     ↓
  │   throw InvalidOperationException("NettySocketServer bind returned inactive channel.")
  │     ↓
  │   start fail -> rollback boss group / worker group / bootstrap
  └── 返回 active listener channel
        ↓
      进入正常运行态
```

- `BindServerAsync(...)` 返回“非空但未激活”的 listener channel，本质上和返回 `null` 一样，都是启动失败。
- 这类 channel 不能继续挂到 `_CurrentChannelHost` 上，否则会把服务端留在“对象显示运行中，但实际上没有可用监听 socket”的假运行态。
- 如果 rejected listener channel 的补偿关闭再次失败，清理异常只能额外记入 `OnStartError(...)`；`NettySocketServer bind returned inactive channel.` 仍必须保持为启动失败主根因。
- 如果 rejected listener channel 的补偿关闭、以及随后的 rollback start / stop close，只遇到 `OperationCanceledException`、`ObjectDisposedException`，或 `"channel not registered to an event loop"` 这类 `InvalidOperationException`，这些异常都按关闭噪音处理，不再额外记新的 `StartError` / `StopError`。

### 当前收口点

- connect 失败、`null channel`、以及“非空但 inactive 的 channel”都不能静默吞掉，否则会把对象留在“`IsRunning=true` 但没有活动 channel、也不再重连”的假运行态。
- server bind 返回 `null channel` 或 inactive listener channel 同样不能静默吞掉，否则会把服务端留在“`IsRunning=true` 但没有监听 channel”的假运行态。
- rejected channel / listener channel 的补偿关闭，如果只是命中了 `OperationCanceledException`、`ObjectDisposedException`，或“尚未注册到 event loop”这类 DotNetty 关闭噪音，不能再额外放大成新的 cleanup error；主根因仍应保持在 `inactive channel` / `inactive listener channel` 这一层。
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
  ├── ObjectDisposed / 已关闭 channel / 未注册 event loop / removed context 噪音：忽略
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
  ├── 关闭噪音：释放 close request state 后忽略
  └── 关闭异常：OnHandlerError("NettyChannelHandler close context failed.", ex)
```

### 当前收口点

- handler 业务回调抛异常时，只上报，不直接打断底层 read/close/reconnect 主链。
- `ChannelActive` 回调失败时，先 `ResetCurrentChannelState()`，再上报并触发 `OnContextClose(...)`，避免半激活状态残留。
- `ChannelInactive` 发生后，当前 handler 的瞬时上下文会先被清空，再进入派生类自己的脱管/重连逻辑。
- `BaseChannelHandler` 的 callback / flush / write / close 四条收口链都要把 DotNetty `"channel not registered to an event loop"`、`"handler not added to pipeline yet"` 这一类 `InvalidOperationException` 识别成关闭噪音，而不是新的运行态错误。
- `SafeCloseContextAsync(...)` / `SafeScheduleCloseContextAsync(...)` 在识别到关闭噪音时，虽然不会额外走 `OnHandlerError(...)`，但仍必须把 `_CurrentCloseRequestState` 释放回 `None`，避免后续真实 close 请求被前一次“已忽略”的关闭噪音卡住。

## Netty channel close 再入分支

```text
OnContextClose(context)
  ↓
SafeCloseContextAsync(context)
  ├── 成功：最终由 ChannelInactive 收尾
  ├── OperationCanceled / ObjectDisposed / 已关闭 channel / 未注册 event loop / removed context：释放 close request state 后忽略
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
- 关闭再入时，同一 active channel 只允许一个在途 close 请求；如果本次 close 明确失败，后续路径允许再次发起重试；这里的“失败”也包括被识别并吞掉的关闭噪音，因为这类噪音同样可能把状态位留在旧值。
- 关闭再入时，只允许“有效 generation + host 仍运行”的 client handler 触发重连。

## Netty 延迟发送失败分支

```text
SendBytes(...)
  ↓
SafeScheduleSendBytesAsync(...)
  ↓
ScheduleSendBytesAsync(...)
  ↓
executor.ScheduleAsync(async () => await WriteBytesAsync(...))
  ├── 调度失败：OnHandlerError("NettyChannelHandler schedule send failed.", ex)
  ├── 写入失败且属于关闭噪音：忽略
  └── 写入失败且属于真实运行态异常：OnHandlerError("NettyChannelHandler write send failed.", ex)
```

### 当前收口点

- 延迟发送链不能只把“调度动作已提交”当成成功，必须把真实 `WriteBytesAsync(...)` 的完成态继续回传到调度 task。
- 否则 executor 内部异步写入失败会变成悬空 fault task，既绕开 `SafeScheduleSendBytesAsync(...)`，也绕开 `OnHandlerError(...)`，最终留下“发送失败但错误通道无信号”的静默漂移。
- 关闭交界期的 `OperationCanceledException` / `ObjectDisposedException` / “未注册到 event loop” / “handler not added to pipeline yet” 类 `InvalidOperationException` 仍按关闭噪音处理，不因为修复调度链而放大成新的运行态错误。

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
