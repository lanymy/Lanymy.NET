# Socket-Sequences

本文档用于补充 `Socket` 模块的时序说明。

## 文档目的

- [Socket-Relations.md](./Socket-Relations.md) 解决的是结构关系。
- 本页解决的是“运行起来之后，事件按什么顺序发生”。
- 重点覆盖服务端主链路，因为这是当前抽象层里最完整的一条时序。

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
ThreadPool.QueueUserWorkItem(BeginAccept)
  ↓
BeginAccept() 循环 Accept()
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
tcpServerClient.StartReceive()
  ↓
BaseTcpServerClient.BeginReceive()
  ↓
创建 NetworkStream
  ↓
BeginRead(...)
  ↓
BaseTcpServer.OnAccept(tcpServerClient)
  ↓
CreateSessionToken(ip, port)
  ↓
写入 client.CurrentSessionToken
  ↓
放入 _TcpServerClientDic
  ↓
OnAcceptEvent(client)
```

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
client.Send(bytes)
  ↓
BaseTcpServerClient.SendAsync(bytes).Wait()
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
  ├── 超时：OnServerClientErrorEvent(...)
  └── 未超时：SendDataBytes(..., GetHeartBytes(sessionToken))
  ↓
OnServerClientHeartCallBackEvent(tcpServerClient)
```

## 关闭时序

### 单客户端关闭

```text
异常 / 主动关闭
  ↓
BaseTcpServerClient.OnServerClientError(...) 或 Close()
  ↓
BaseTcpServerClient.OnClose()
  ↓
停止 HeartTimerWorkTask
  ↓
停止 SendWorkTaskQueue
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
```

### 服务端整体关闭

```text
BaseTcpServer.Close()
  ↓
OnServerClose()
  ↓
_IsRunning = false
  ↓
Dispose CurrentSocket
  ↓
遍历 _TcpServerClientDic
  ↓
逐个 client.Close()
  ↓
OnServerCloseEvent()
  ↓
_TcpServerClientDic.Clear()
```

## 时序里的几个关键观察点

- `StartReceive()` 先启动 `BeginReceive()`，再触发 `OnStartReceive()`，然后才启动发送队列和心跳任务。
- 收包主线里，业务回调 `OnServerClientReceiveDataCallBackEvent(...)` 发生在真正拆包之前。
- `OnServerClientReceiveDataLoopEvent(...)` 现在已改成循环实现，连续包处理不再额外放大调用深度。
- 发包路径统一经过 `_CurrentSendWorkTaskQueue`，说明发送天然是串行化的。
- 关闭路径设计成“异常优先转关闭”，很多错误最终都会表现成连接被动断开。

## 后续建议

- 这页已经能作为调试入口，下一步可以把“异常分支时序”单独再拆一页。
- 如果后续开始做完整逻辑复验，建议就按本页四条链路逐段核对。
