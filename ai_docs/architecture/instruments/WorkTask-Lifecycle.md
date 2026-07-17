# WorkTask-Lifecycle

本文档用于专题说明 `WorkTask` 体系的生命周期、停止语义、资源释放链和当前高风险点。

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

## 4. 当前高风险点

### 4.1 `Dispose()` 语义不统一

- 这一项已完成第一步收敛：`BaseWorkTask.Dispose()` 已统一为“先 `StopAsync()` 再 `OnDisposeAsync()`”。
- 当前 `WorkTask` 体系不再依赖具体派生类型自己决定 `Dispose()` 是否顺带停机。
- 后续仍需继续关注停止链内部的 `Wait()`、`async void` 与异常可观测性问题。

关键代码：

- [BaseWorkTask.cs:L95-L105](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTask.cs#L95-L105)
- [BaseSimpleWorkTask.cs:L146-L149](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseSimpleWorkTask.cs#L146-L149)
- [BaseWorkTaskQueue.cs:L218-L229](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTaskQueue.cs#L218-L229)

### 4.2 停止链中存在明确实现缺陷

- `BaseSimpleWorkTask.OnStopAsync()` 的 `CancellationTokenSource` 判空方向写反。
- `BaseWorkTaskTriggerQueue.OnStartAsync()` 把 `_TimeTriggerTask` 写成了局部变量，导致停止阶段访问字段时存在空引用风险。

关键代码：

- [BaseSimpleWorkTask.cs:L118-L149](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseSimpleWorkTask.cs#L118-L149)
- [BaseWorkTaskTriggerQueue.cs:L137-L196](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTaskTriggerQueue.cs#L137-L196)

### 4.3 状态位先改，真实资源后处理

- `BaseWorkTask.StartAsync()` / `StopAsync()` 都是先改 `IsRunning`，再进入真实启动或停止流程。
- 如果真实流程失败，可能出现：
  - 状态已显示运行，但任务并未完整启动
  - 状态已显示停止，但资源并未全部释放

关键代码：

- [BaseWorkTask.cs:L42-L70](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTask.cs#L42-L70)

### 4.4 同步阻塞异步

- 当前 `WorkTask` 体系中存在 `task.Wait()`、`Task.Delay(...).Wait()`、`async void` 等模式。
- 它们会放大：
  - 停止链阻塞
  - 后台异常不可等待
  - 排障复杂度

关键代码：

- [BaseWorkTaskQueue.cs:L65-L82](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTaskQueue.cs#L65-L82)
- [BaseWorkTaskQueue.cs:L139-L149](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTaskQueue.cs#L139-L149)
- [BaseWorkTaskTriggerQueue.cs:L150-L160](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseWorkTaskTriggerQueue.cs#L150-L160)

### 4.5 `Channel` 所有权命名不清

- `BaseChannelWorkTask` 当前用 `_IsInternalChannel` 区分 `Channel` 生命周期。
- 但其命名和实际语义容易误导维护者。
- 该字段需要被重新命名或在文档中明确说明。

关键代码：

- [BaseChannelWorkTask.cs:L29-L75](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions/BaseChannelWorkTask.cs#L29-L75)

## 5. 当前建议

1. 先修明确缺陷：
   - `BaseSimpleWorkTask`
   - `BaseWorkTaskTriggerQueue`
2. 继续验证 `Dispose()` 统一入口下的剩余阻塞点与异常边界。
3. 再减少 `Wait()` 和 `async void` 造成的阻塞与不可观测性。
4. 最后补充 `Channel` 生命周期命名与所有权文档。

## 6. 关联文档

- 总体体检见 [../../debugs/review_round_02.md](../../debugs/review_round_02.md)
- 分阶段治理路线见 [../../debugs/remediation_roadmap.md](../../debugs/remediation_roadmap.md)
- 主模块概览见 [WorkTask.md](./WorkTask.md)
