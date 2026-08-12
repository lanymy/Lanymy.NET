# WorkTask-Lifecycle

本文档用于专题说明 `WorkTask` 体系的生命周期、停止语义、资源释放链，以及本轮治理后已经沉淀下来的硬约束。

## 1. 适用范围

- `Lanymy.Common.Instruments.WorkTask.Abstractions`
- `Lanymy.Common.Instruments.WorkTaskQueue`

## 2. 当前核心对象

- `BaseWorkTask`
- `BaseChannelWorkTask<TDataModel>`
- `BaseWorkTaskQueue<TDataModel>`
- `BaseSimpleWorkTask`
- `BaseSimpleWorkTaskQueue`
- `BaseWorkTaskTriggerQueue<TDataModel>`

## 3. 当前生命周期主线

### 3.1 启动链

- `BaseWorkTask.StartAsync()` 先把 `IsRunning` 设为 `true`，再调用 `OnStartAsync()`。
- `BaseWorkTaskQueue.OnStartAsync()` 中会：
  - 创建或接管 `Channel`
  - 创建 `CancellationTokenSource`
  - 启动多个后台任务
- `BaseWorkTaskTriggerQueue.OnStartAsync()` 在此基础上还会再启动一个定时触发任务。

### 3.2 停止链

- `BaseWorkTask.StopAsync()` 先把 `IsRunning` 设为 `false`，再调用 `OnStopAsync()`。
- `BaseWorkTaskQueue.OnStopAsync()` 中会：
  - 视情况 `Complete` 写入端
  - `Cancel` 工作任务
  - `Wait` / `Dispose` 当前任务
  - 读取剩余队列
  - 等待 `Reader.Completion`
- `BaseWorkTaskTriggerQueue.OnStopAsync()` 还会额外停止定时触发任务。

### 3.3 释放链

- `BaseWorkTask.Dispose()` 当前先执行 `StopAsync()`，再执行 `OnDisposeAsync()`。
- `StopAsync()` 负责停机与后台任务闭合，`OnDisposeAsync()` 负责子类自己的额外清理。
- `OnDisposeAsync()` 的抽象约束由 `BaseWorkTask` 统一定义，中间抽象层不重复声明同一个成员。
- `BaseSimpleWorkTask`、`BaseSimpleWorkTaskQueue` 不再重复声明 `OnDisposeAsync()`，真正 concrete class 仍需显式实现。

## 4. 当前硬约束

这一节不是“建议做法”，而是后续继续改 `WorkTask` 体系时默认必须保持的行为契约。

### 4.1 同步桥接约束

- 同步入口如果要桥接异步流程，必须统一经过 `TaskHelper.TrySyncWait(...)` 或同等语义的 helper 收口，不能直接把桥接异常裸抛给调用方。
- `Dispose()` 这类同步释放入口允许感知 `StopAsync()` / `OnDisposeAsync()` 的失败，但必须保证：
  - 失败可观测；
  - 后续释放动作不会因为前一步异常被短路。
- 如果同步桥接发生在启动链，桥接失败后必须进入启动回滚，不能把实例留在“状态已启动、资源未启动完”的半初始化状态。

关键代码：

- [TaskHelper.cs:L10-L35](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/TaskHelper.cs#L10-L35)
- [BaseWorkTask.cs:L119-L154](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTask.cs#L119-L154)

### 4.2 `StopAsync()` 尾声清理约束

- `StopAsync()` 的实现要先抓当前资源快照，再执行停止动作，避免关闭过程中字段被并发改写后造成清理对象漂移。
- stop 尾声中的各个步骤要按“尽量继续清理、最后统一上报”的模式组织：
  - `Cancel`
  - `Wait`
  - `Dispose`
  - 读残留数据
  - 等待 completion
- 即使后台 task fault、读残留回调抛异常、channel completion fault，也不能跳过 task / `CancellationTokenSource` / channel / 缓存的最终清理。
- finally 块里负责恢复最终状态，异常是否上抛放到状态收尾之后处理。

关键代码：

- [BaseWorkTaskQueue.cs:L139-L236](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTaskQueue.cs#L139-L236)
- [BaseWorkTaskTriggerQueue.cs:L213-L289](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTaskTriggerQueue.cs#L213-L289)
- [WorkTaskTriggerQueueContext.cs:L98-L174](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTaskQueue/WorkTaskTriggerQueueContext.cs#L98-L174)
- [WorkTaskTriggerQueueContext.cs:L217-L245](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTaskQueue/WorkTaskTriggerQueueContext.cs#L217-L245)

### 4.3 队列单次写入契约

- `BaseChannelWorkTask.AddToQueueAsync()` 必须遵循“快照判定后单次写入”约束：
  - 先抓取当前 channel 快照；
  - 基于该快照等待可写；
  - 写入前再次校验 `IsRunning` 与字段一致性；
  - 整个调用链只允许一次真正的 `WriteAsync(...)`。
- 这个约束的目标不是只防止异常，而是防止启停交界期把一条数据放大成多条，避免重复入队、重复触发、重复 flush。
- stop 交界期如果 channel 已完成，可以忽略关闭噪音，但不能通过重试或重复写入把问题放大。

关键代码：

- [BaseChannelWorkTask.cs:L104-L147](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseChannelWorkTask.cs#L104-L147)
- [WorkTaskTests.cs:L849-L870](file:///E:/Code/Git/My/Lanymy.NET/src/UnitTests/Lanymy.Common.AllTests/WorkTaskTests.cs#L849-L870)
- [WorkTaskTests.cs:L928-L1059](file:///E:/Code/Git/My/Lanymy.NET/src/UnitTests/Lanymy.Common.AllTests/WorkTaskTests.cs#L928-L1059)

### 4.4 组合层异常聚合约束

- `WorkTaskTriggerQueueContext` 这类组合层对象在启动 / 停止时，不能只顾自己状态位，还要负责把 child、channel 和自身状态一起收口。
- child 启动失败时，必须回滚已启动 child、关闭 channel，并恢复 `StateType=Stop`。
- child `StopAsync()` / `Dispose()` 失败时，必须继续清理剩余 child 和 channel，最后统一聚合异常上抛。

关键代码：

- [WorkTaskTriggerQueueContext.cs:L176-L245](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTaskQueue/WorkTaskTriggerQueueContext.cs#L176-L245)
- [WorkTaskTests.cs:L1062-L1117](file:///E:/Code/Git/My/Lanymy.NET/src/UnitTests/Lanymy.Common.AllTests/WorkTaskTests.cs#L1062-L1117)

## 5. 当前高风险点

### 5.1 `Dispose()` 统一入口已经收敛，但仍要继续盯剩余阻塞点

- `BaseWorkTask.Dispose()` 已统一为“先 `StopAsync()` 再 `OnDisposeAsync()`”。
- 当前更大的残留风险不再是“语义不统一”，而是同步桥接链上仍可能残留新的阻塞点或不可观测异常。

### 5.2 `Channel` 所有权命名仍有歧义

- `BaseChannelWorkTask` 当前用 `_IsInternalChannel` 区分 `Channel` 生命周期。
- 实际语义更接近“当前实例是否接管 channel 的创建/关闭责任”，名字本身仍然容易误导维护者。

### 5.3 旧的阻塞模式仍需继续减少

- 当前 `WorkTask` 体系虽然已经把最脆弱的 stop / dispose 路径收住，但历史上的 `.Wait()`、`async void` 思路仍是后续维护风险来源。

## 6. 当前建议

1. 继续验证 `Dispose()` 统一入口下的剩余阻塞点与异常边界。
2. 继续减少 `Wait()` 和 `async void` 造成的阻塞与不可观测性。
3. 补充 `Channel` 生命周期命名与所有权文档，降低 `_IsInternalChannel` 的语义歧义。
4. 后续新增 `AddToQueueAsync()` 相关改动时，默认要补覆盖“启停竞态 + 单次入队”回归用例。

## 7. 关联文档

- 总体体检见 [../../debugs/review_round_02.md](../../debugs/review_round_02.md)
- 分阶段治理路线见 [../../debugs/remediation_roadmap.md](../../debugs/remediation_roadmap.md)
- 主模块概览见 [WorkTask.md](./WorkTask.md)
