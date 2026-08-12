# Socket-Sequences

本文档用于补充 `Socket` 模块的时序说明。

## 文档目的

- [Socket-Relations.md](./Socket-Relations.md) 解决的是结构关系。
- [Socket.Abstractions.md](./Socket.Abstractions.md) 解决的是接口与抽象职责。
- [Socket-Lifecycle.md](./Socket-Lifecycle.md) 解决的是生命周期硬约束。
- [Socket-Exception-Sequences.md](./Socket-Exception-Sequences.md) 解决的是失败分支如何收口。
- 本页解决的是“运行起来之后，事件按什么顺序发生”。
- 重点覆盖传统 `Socket.Abstractions` 主线，因为这是当前抽象层里最完整、也是最近几轮治理最集中的一条时序。

## 阅读方式

- 如果先想知道“谁调谁、对象如何组合”，先看 [Socket-Relations.md](./Socket-Relations.md)。
- 如果先想知道“同步桥接、回调隔离、关闭补偿有哪些硬约束”，先看 [Socket-Lifecycle.md](./Socket-Lifecycle.md)。
- 如果已经确认主干顺序没问题，想继续排查失败路径，再看 [Socket-Exception-Sequences.md](./Socket-Exception-Sequences.md)。
- 本页更关注“在这些约束下，当前代码实际按什么顺序执行”。

## 服务端接入时序

```text
调用方
  ↓
BaseTcpServer.Start()
  ↓
创建监听 Socket
  ↓
Bind + Listen
  ↓
Task.Factory.StartNew(BeginAcceptAsync)
  ↓
BeginAcceptAsync() 循环 Accept()
  ↓
CreateTcpServerClient(socket)
  ↓
注册 client 事件
  - StartReceiveEvent
  - ServerClientErrorEvent
  - ReceiveDataEvent
  - CloseEvent
  - HeartEvent
  ↓
TryInitializeAcceptedClient(client)
  ↓
CreateSessionToken(ip, port)
  ↓
写入 client.CurrentSessionToken
  ↓
放入 _TcpServerClientDic
  ↓
await tcpServerClient.StartReceiveAsync()
  ↓
BaseTcpServerClient.TryBeginReceive()
  ↓
创建 NetworkStream
  ↓
BeginRead(...)
  ↓
启动 SendWorkTaskQueue
  ↓
启动 HeartTimerWorkTask
  ↓
OnStartReceive() / StartReceiveEvent
  ↓
BaseTcpServer.OnAccept(tcpServerClient)
  ↓
OnAcceptEvent(client)
```

### 接入链当前关键分支

- `TryInitializeAcceptedClient(...)` 失败时，不会进入 `StartReceiveAsync()`，而是走 accepted client 清理链。
- `StartReceiveAsync()` 抛异常或最终没有进入运行态时，服务端会把该 client 从字典移除并 `Dispose()`，不会把半初始化 child 留在托管集合里。
- 只有 child 完成初始化并处于运行态后，`OnAcceptEvent(...)` 才会对业务层可见。

## 服务端收包时序

```text
NetworkStream 收到字节
  ↓
BaseTcpServerClient.OnReceive(...)
  ↓
更新 CurrentSessionToken.LastReceiveDateTime
  ↓
写入 BufferModel.Position
  ↓
OnReceiveData(_CurrentBuffer, _CurrentCache)
  ↓
触发 ReceiveDataEvent
  ↓
BaseTcpServer.OnServerClientReceiveDataEvent(...)
  ↓
OnServerClientReceiveDataCallBackEvent(...)
  ↓
再次校验 child 是否仍处于托管态
  ↓
OnServerClientReceiveDataLoopEvent(...)
  ↓
Filter.GetPackageBytes(buffer, cache)
  ↓
Filter.CheckPackage(packageBytes)
  ↓
Filter.DecodePackage(packageBytes)
  ↓
OnServerReceivePackage(package, sessionToken)
  ↓
OnServerReceivePackageEvent(package, sessionToken)
```

### 收包链当前关键分支

- `ReceiveData` 业务回调发生在真正拆包之前，但回调异常会按 child error 通道上报，不再直接打断后续清理链。
- 进入 `OnServerClientReceiveDataLoopEvent(...)` 前后都会再次校验托管态，避免已脱管 child 在关闭交界期继续推进拆包与业务通知。
- `BaseTcpClient` / `BaseTcpServerClient` 当前都使用“流快照 + 连续性校验 + 关闭噪音忽略”模型，旧 stream 或关闭交界期异常不会再被误判成新的运行态故障。

## 服务端发包时序

```text
调用方 / 业务逻辑
  ↓
BaseTcpServer.SendPackage(sessionID, sendPackage)
  ↓
GetTcpServerClient(sessionID)
  ↓
sendPackage.SendNum = session.SendNum
  ↓
Filter.EncodePackage(sendPackage)
  ↓
SendDataBytes(client, bytes)
  ↓
校验 client 是否仍处于托管态
  ↓
CanSendData(sessionToken)
  ↓
client.Send(bytes)
  ↓
BaseTcpServerClient.Send(bytes)
  ↓
_CurrentSendWorkTaskQueue.AddToQueueAsync(bytes)
  ↓
OnSendWorkTaskQueueAsync(bytes)
  ↓
NetworkStream.WriteAsync(...)
  ↓
FlushAsync()
  ↓
更新 CurrentSessionToken.LastSendDateTime
  ↓
Task.Delay(_SendDataIntervalMilliseconds)
```

### 发包链当前关键分支

- `SendDataBytes(...)` 在真正发送前会先检查托管态，并对 `CanSendData(...)` 的业务异常做错误上报，不再直接把异常冒回调用方。
- `client.Send(...)` 仍然是同步入口，但当前实现会把桥接异常转入错误通道，而不是直接裸抛桥接异常。
- 发送最终统一经过发送队列串行化，所以业务侧看到的是同步调用面，数据面仍是队列化异步落地。

## 心跳时序

```text
TimerWorkTask 定时触发
  ↓
BaseTcpServerClient.OnHeartTimerWorkTask()
  ↓
HeartEvent(this)
  ↓
BaseTcpServer.OnServerClientHeartEvent(tcpServerClient)
  ↓
根据 LastReceiveDateTimeTotalMillisecondsFromInstantiation 判断是否超时
  ├── 超时：HandleServerManagedClientError(...)
  │         ↓
  │       ReportManagedClientError(...)
  │         ↓
  │       CloseTcpServerClient(...)
  └── 未超时：SendDataBytes(..., GetHeartBytes(sessionToken))
  ↓
OnServerClientHeartCallBackEvent(tcpServerClient)
```

### 心跳链当前关键分支

- 如果心跳检查阶段已经触发关闭或发送失败，后续不会再继续执行 `OnServerClientHeartCallBackEvent(...)`。
- 心跳业务通知现在是“托管态仍有效时才触发”的附加动作，不再反向主导关闭链。

## 关闭时序

### 单客户端关闭

```text
异常 / 主动关闭
  ↓
BaseTcpServerClient.OnServerClientError(...) 或 Close()
  ↓
BaseTcpServerClient.OnCloseAsync()
  ↓
停止 HeartTimerWorkTask
  ↓
停止 SendWorkTaskQueue
  ↓
从字段中摘除 timer / queue / stream 引用
  ↓
释放 NetworkStream
  ↓
Shutdown / Dispose Socket
  ↓
OnCloseEvent()
  ↓
触发 CloseEvent(this)
  ↓
BaseTcpServer.OnServerClientCloseEvent(...)
  ↓
CloseTcpServerClient(sessionToken)
  ↓
从 _TcpServerClientDic 移除
  ↓
清理事件句柄
```

### 服务端整体关闭

```text
BaseTcpServer.Close()
  ↓
OnServerClose()
  ↓
TrySyncWait(CloseAsync)
  ↓
OnServerCloseAsync()
  ↓
_IsRunning = false
  ↓
Dispose CurrentSocket
  ↓
遍历 _TcpServerClientDic
  ↓
逐个 DetachTcpServerClientEventHandlers(client)
  ↓
逐个 await client.CloseAsync()
  ↓
OnServerCloseEvent()
  ↓
_TcpServerClientDic.Clear()
```

### 关闭链当前关键分支

- child `StopAsync()` / `Dispose()` / socket 关闭中的任一步失败，都不应阻断后续资源释放。
- 服务端主动关闭 child 前会先解绑事件，避免 child `CloseEvent` 反向触发重复关闭。
- 单个 child `CloseAsync()` 失败时，服务端会继续关闭其余 child，并对失败 child 做补偿 `Dispose()`。
- `Dispose()` 不依赖公开 `Close()` 的抛异常语义；即使关闭阶段报错，只要实例已退出运行态，fallback 释放仍会继续。

## 客户端与 UDP 补充时序

### TCP 客户端启动 / 关闭摘要

```text
BaseTcpClient.Start()
  ↓
Connect(server)
  ↓
创建 NetworkStream
  ↓
同步桥接启动 SendWorkTaskQueue
  ↓
OnConnection()
  ↓
BeginRead(...)

异常 / Close()
  ↓
CloseAsync()
  ↓
停止并释放 SendWorkTaskQueue
  ↓
摘除并释放 NetworkStream / Socket
  ↓
OnCloseEvent()
```

### UDP 启动 / 关闭摘要

```text
BaseUdpClient.Start()
  ↓
同步桥接启动 ReceiveWorkTaskQueue
  ↓
创建并绑定 UdpClient
  ↓
BeginReceive(...)
  ↓
同步桥接启动 SendWorkTaskQueue
  ↓
OnStart()

CloseAsync()
  ↓
停止 ReceiveWorkTaskQueue
  ↓
停止 SendWorkTaskQueue
  ↓
摘除失败队列并按需重建
  ↓
释放 UdpClient
  ↓
OnCloseEvent()
```

## 时序里的几个关键观察点

- 服务端 child 当前是“先完成托管初始化，再启动接收，再对业务暴露 accept”。
- 收包主线里，业务回调 `OnServerClientReceiveDataCallBackEvent(...)` 仍发生在真正拆包之前，但托管态校验已经把关闭交界期的误通知收住。
- 发包路径统一经过发送队列，业务面虽然保留了同步 `Send()` 入口，但数据面仍是串行化异步落地。
- 关闭路径当前强调“先抓快照，再尽量继续清理，最后统一上报”，而不是某一步失败就停止收尾。
- 当前比早期实现更重要的变化，不是时序表面顺序，而是每个阶段都增加了托管态校验、桥接异常收口和关闭补偿。

## 后续建议

- 这页已经能作为主干调试入口，异常分支可继续参考 [Socket-Exception-Sequences.md](./Socket-Exception-Sequences.md)。
- 如果后续开始做完整逻辑复验，建议优先按“接入 -> 收包 -> 发包 -> 心跳 -> 关闭”这五条链路逐段核对。
