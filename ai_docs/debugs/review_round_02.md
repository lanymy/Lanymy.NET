# 第二轮系统性体检记录

本文档记录基于当前源码静态扫描得到的第二轮系统性问题复验结果。

## 复验目标

- 不再只看单点功能错误。
- 重点转向基础设施层的系统性问题：
  - 生命周期与关闭链
  - 性能与线程阻塞
  - 资源所有权与潜在泄漏
  - 吞异常与状态不透明

## 复验范围

- `Lanymy.Common.Instruments.WorkTask.*`
- `Lanymy.Common.Instruments.Socket.Abstractions`
- `Lanymy.Common.Instruments.Socket.Netty.Abstractions`
- `Lanymy.Common.Instruments.Crawler.*`
- 与对象所有权相关的 `ImageHelper` / `LanymyCrypto`

## 扫描信号

- `.Wait()`：`35` 处
- `catch`：`100` 处
- `Task.Run(...)`：`15` 处
- `async void`：`12` 处

> 上述统计来自 `src/Commons`，用于判断是否属于系统性模式，不等同于运行时 profiler 结论。

## 总体结论

- 当前项目第二轮的主要问题，不是“业务逻辑错一行”，而是基础设施层存在重复出现的同类模式。
- 最需要优先治理的主线是：
  - `WorkTask` 生命周期
  - `Socket` / `Netty` 关闭链
  - 同步阻塞异步
- 当前尚未静态确认“长期持续增长的大托管对象缓存泄漏”，但已确认多处存在：
  - 后台任务残留风险
  - 重连任务残留风险
  - `Timer` 保活风险
  - `Bitmap` / 句柄对象所有权不清

## 严重度分级

### P1. 生命周期闭合失败

- `BaseWorkTask.Dispose()` 不保证执行完整的 `Stop + Release` 语义。
- `BaseWorkTaskQueue` 体系在“调用方只 Dispose 不 Stop”的情况下，后台任务与队列可能继续存活。
- `BaseSimpleWorkTask.OnStopAsync()` 的 `CancellationTokenSource` 判空方向写反，属于明确实现缺陷。
- `BaseWorkTaskTriggerQueue` 启动阶段把 `_TimeTriggerTask` 写成局部变量，停止阶段访问字段，属于明确实现缺陷。

关键代码：

- [BaseWorkTask.cs:L42-L105](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTask.cs#L42-L105)
- [BaseSimpleWorkTask.cs:L118-L149](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseSimpleWorkTask.cs#L118-L149)
- [BaseWorkTaskTriggerQueue.cs:L137-L196](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTaskTriggerQueue.cs#L137-L196)

### P1. Netty 客户端重连无取消边界

- `BaseNettySocketClient.OnStartAsync()` 中使用 `Task.Run(ConnectToServerAsync)` 放飞重连任务。
- `ConnectToServerAsync()` 在失败时递归调用自身，没有显式退出条件。
- `OnChannelInactive()` 会再次触发重连动作，形成第二条重连入口。
- `StopAsync()` 后仍可能残留重连流程，属于高风险生命周期问题。

关键代码：

- [BaseNettySocketClient.cs:L46-L140](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Client/BaseNettySocketClient.cs#L46-L140)
- [BaseClientChannelHandler.cs:L38-L45](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Netty.Abstractions/Client/BaseClientChannelHandler.cs#L38-L45)

### P1. Socket 关闭链状态不透明

- `BaseUdpClient`、`BaseTcpClient`、`BaseTcpServerClient`、`BaseTcpServer` 的关闭链都较长。
- 多段 `try/catch {}` 会吞掉真实关闭失败原因。
- 外部调用者很难判断“对象已关闭”是否等于“资源已全部释放”。

关键代码：

- [BaseUdpClient.cs:L274-L354](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs#L274-L354)
- [BaseTcpClient.cs:L337-L437](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs#L337-L437)
- [BaseTcpServerClient.cs:L415-L558](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServerClient.cs#L415-L558)

### P2. 性能主问题是同步阻塞异步

- 当前仓库最显著的性能问题不是算法复杂度，而是 `.Wait()` 大量出现在队列、Socket、Crawler 和关闭路径中。
- 这些点会直接带来：
  - 线程阻塞
  - 吞吐下降
  - 停机变慢
  - 死锁风险增大

代表代码：

- [BaseWorkTaskQueue.cs:L139-L149](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTaskQueue.cs#L139-L149)
- [BaseWorkTaskTriggerQueue.cs:L150-L160](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTaskTriggerQueue.cs#L150-L160)
- [BaseTcpClient.cs:L354-L360](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpClient.cs#L354-L360)
- [BaseUdpClient.cs:L293-L310](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs#L293-L310)

### P2. 所有权边界不清

- `Bitmap` 的所有权在 `LanymyCrypto` 中会随调用路径变化。
- `ImageHelper.GetBitmapFromImageFile(...)` 返回的 `Bitmap` 需要调用方显式释放，但 API 层没有突出这个约束。
- `BaseChannelWorkTask` 的 `_IsInternalChannel` 命名与实际含义不一致，容易误导维护者对 `Channel` 生命周期的理解。

关键代码：

- [LanymyCrypto.cs:L1171-L1184](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Crypto/LanymyCrypto.cs#L1171-L1184)
- [LanymyCrypto.cs:L1332-L1342](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Crypto/LanymyCrypto.cs#L1332-L1342)
- [LanymyCrypto.cs:L1360-L1380](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Crypto/LanymyCrypto.cs#L1360-L1380)
- [ImageHelper.cs:L39-L42](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Helpers.ImageHelper/ImageHelper.cs#L39-L42)
- [BaseChannelWorkTask.cs:L29-L75](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseChannelWorkTask.cs#L29-L75)

### P3. 分配与调度型问题

- `Task.Run(...)` 包同步实现的“伪异步”模式较多。
- `BaseWorkTaskTriggerQueue` 使用 `ConcurrentQueue.ToList()` 进行整队快照，批量大时会产生突发分配。
- 一些字符串与正则路径存在可优化空间，但优先级低于生命周期和阻塞问题。

## 当前文档归档关系

- 第一轮功能缺陷修复汇总见 [fixed_issues_summary.md](./fixed_issues_summary.md)。
- 第二轮系统性治理路线见 [remediation_roadmap.md](./remediation_roadmap.md)。
- `WorkTask` 生命周期专题见 [../architecture/instruments/WorkTask-Lifecycle.md](../architecture/instruments/WorkTask-Lifecycle.md)。
- `Socket` 生命周期专题见 [../architecture/instruments/Socket-Lifecycle.md](../architecture/instruments/Socket-Lifecycle.md)。

## 当前建议

1. 先修明确实现缺陷：
   - `BaseSimpleWorkTask`
   - `BaseWorkTaskTriggerQueue`
   - `BaseNettySocketClient`
2. 再统一 `Dispose` 与 `Stop` 语义。
3. 再收敛 Socket 关闭链的异常可观测性。
4. 最后再做性能层面的 `.Wait()` 与 `Task.Run` 清理。
