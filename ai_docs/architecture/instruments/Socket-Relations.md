# Socket-Relations

本文档用于补充 `Socket` 抽象层的关系图式说明。

## 文档目的

- [Socket.md](./Socket.md) 偏总体概览。
- [Socket.Abstractions.md](./Socket.Abstractions.md) 偏抽象层职责说明。
- [Socket-Lifecycle.md](./Socket-Lifecycle.md) 偏生命周期和关闭链约束。
- 本页重点把这些类之间“谁依赖谁、数据怎么流动”串起来。

## 配套阅读关系

- 如果想先理解“接口对外承诺了什么”，先看 [Socket.Abstractions.md](./Socket.Abstractions.md)。
- 如果想确认“这些链路在启动、关闭、异常时怎么收口”，再看 [Socket-Lifecycle.md](./Socket-Lifecycle.md)。
- 本页更适合作为看源码前的结构索引和排查入口。

## 核心关系图

```text
固定头协议实现类
    ↓
BaseFixedHeaderPackageFilter<TPackage, TSendPackage, TSessionToken>
    ↓
────────────────────────────────────────────
服务端链路
BaseTcpServer<TTcpServerClient, TSessionToken, TFilter, TPackage, TSendPackage>
    ├── 管理 CurrentSocket / 监听 / 接入
    ├── 创建 TSessionToken
    ├── 持有 ConcurrentDictionary<Guid, ITcpServerClient>
    └── 驱动 TTcpServerClient
            ↓
      BaseTcpServerClient
            ├── NetworkStream
            ├── BufferModel
            ├── CacheModel
            ├── WorkTaskQueue<byte[]>
            └── TimerWorkTask
                    ↓
              ReceiveDataEvent / HeartEvent / CloseEvent
                    ↓
      BaseFixedHeaderPackageFilter.GetPackageBytes(...)
                    ↓
              DecodePackage(...)
                    ↓
      BaseTcpServer.OnServerReceivePackage(...)

────────────────────────────────────────────
客户端链路
BaseTcpClient<TPackage, TSendPackage, TSessionToken, TFilter>
    ├── 主动 Connect
    ├── NetworkStream
    ├── BufferModel
    ├── CacheModel
    └── WorkTaskQueue<byte[]>
            ↓
      Filter.GetPackageBytes(...)
            ↓
      DecodePackage(...)
            ↓
      OnReceivePackage(...)

────────────────────────────────────────────
UDP 链路
BaseUdpClient<TSessionToken, TFilter, TPackage, TSendPackage>
    ├── UdpClient
    ├── WorkTaskQueue<UdpSourceDataModel>
    ├── WorkTaskQueue<SendUdpDataModel>
    └── Filter.CheckPackage / DecodePackage / EncodePackage
```

## 三个基础数据对象

- `BufferModel`
  - 表示当前读取缓冲区
  - 关心：
    - `BufferData`
    - `Position`
    - `CursorIndex`
- `CacheModel`
  - 表示跨次读取遗留的半包缓存
  - 关心：
    - `Data`
    - `Position`
- `BaseSessionToken`
  - 表示连接/会话的运行时状态
  - 关心：
    - `SessionID`
    - `IP`
    - `Port`
    - `LastReceiveDateTime`
    - `LastSendDateTime`
    - `SendNum`

## 服务端数据流

1. `BaseTcpServer.Start()` 建立监听 socket
2. `Accept()` 到新连接
3. 创建 `TTcpServerClient`
4. 创建 `TSessionToken`
5. `TTcpServerClient.StartReceive()`
6. `BaseTcpServerClient` 通过 `NetworkStream.BeginRead(...)` 收数据
7. 数据进入 `BufferModel`
8. 半包残留进入 `CacheModel`
9. `BaseFixedHeaderPackageFilter.GetPackageBytes(...)` 做拆包
10. `CheckPackage(...)` 校验
11. `DecodePackage(...)` 转成业务包
12. `BaseTcpServer.OnServerReceivePackage(...)` 把包交给子类

## 客户端数据流

1. `BaseTcpClient.Start()` 主动连接服务端
2. 创建 `NetworkStream`
3. `BeginRead(...)` 收数据
4. `BufferModel + CacheModel` 参与拆包
5. `Filter.DecodePackage(...)` 得到业务包
6. `OnReceivePackageEvent(...)` 交给子类处理

## UDP 数据流

- UDP 与 TCP 最大区别是没有连接态循环，但模块仍保留了两层串行化：
  - 接收进入 `_ReceiveWorkTaskQueue`
  - 发送进入 `_SendWorkTaskQueue`
- 收到的原始字节会先封装成 `UdpSourceDataModel`，再进入过滤器做校验和解码。

## 心跳与发送链路

- `BaseTcpServerClient` 内部的 `TimerWorkTask` 会定时触发 `HeartEvent`
- `BaseTcpServer` 收到心跳事件后，会：
  - 检查超时
  - 或发送 `_CurrentFixedHeaderPackageFilter.GetHeartBytes(sessionToken)`
- 普通发送则统一经过 `WorkTaskQueue<byte[]>`
- 这意味着“发送节流”和“心跳节拍”都不是散落在业务代码里，而是集中在抽象层里

## 当前实现风格总结

- 核心思想不是“直接 read / write socket”，而是：
  - 用过滤器统一协议
  - 用缓冲对象处理分包粘包
  - 用会话对象保存状态
  - 用工作队列串行化发送
  - 用模板方法把业务处理下沉给子类

## 当前边界判断

- 已成熟主线：
  - `BaseTcpServer`
  - `BaseTcpServerClient`
  - `BaseTcpClient`
  - `BaseUdpClient`
  - `BaseFixedHeaderPackageFilter`
- 未成熟支线：
  - `SocketAsync/*`

## 后续建议

- 下一步很适合补一页“Socket 时序图文档”，把接入、收包、拆包、心跳、关闭画成阶段图。
- 如果后续开始做源码复验，这一页可以直接当作排查入口索引使用。
- 当前已补充时序文档见 [Socket-Sequences.md](./Socket-Sequences.md)。
