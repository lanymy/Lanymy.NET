# WorkTask

## 1. 模块定位

- 对应项目：
  - `src/Commons/Lanymy.Common.Instruments.WorkTask.Abstractions`
  - `src/Commons/Lanymy.Common.Instruments.WorkTaskQueue`
- 当前模块定位是后台工作任务与队列处理机制。

## 2. 当前核心结构

从当前抽象层看，主要角色包括：

- `BaseWorkTask`
- `BaseChannelWorkTask<TDataModel>`
- `BaseWorkTaskQueue<TDataModel>`
- `BaseSimpleWorkTask`
- `BaseSimpleWorkTaskQueue`
- `BaseTimerWorkTask`
- `BaseWorkTaskDataContext`

## 3. 当前实现特征

- 基于 `System.Threading.Channels`
- 支持内部创建 Channel，也支持外部传入 Channel
- 支持配置：
  - 队列容量
  - 满队列策略
  - 工作线程数
  - 停止时回读剩余队列数据
- `BaseWorkTaskQueue<TDataModel>` 使用长运行任务处理队列数据

## 4. 当前执行模型

- 启动时：
  - 创建或接管 Channel
  - 初始化 `CancellationTokenSource`
  - 启动多个后台任务
- 运行时：
  - 通过 `AddToQueueAsync` 向 Channel 写入数据
  - 后台任务循环读取并执行 `workAction`
- 停止时：
  - 完成写入端
  - 取消后台任务
  - 读取剩余队列数据并回调处理

## 5. 当前维护关注点

- 这是当前仓库里较现代的一类机制模块，因为它已经采用了 `Channel`。
- 适合继续作为任务处理、队列消费能力的基础。
- 但当前实现也混合了较传统的线程 / Task 使用方式，维护时需要谨慎看待生命周期与停止逻辑。

## 6. 当前风险点

- 停止流程涉及 `Cancel`、`Wait`、读取剩余数据和释放资源，边界处理需要特别小心。
- 多工作任务场景下，若后续扩展异常处理、监控或背压控制，需要重新审视当前抽象是否足够。
- 当前公共 API 以委托方式注入工作逻辑，简单直接，但对复杂场景的可观测性支持有限。

## 7. 推荐阅读入口

1. `BaseChannelWorkTask.cs`
2. `BaseWorkTaskQueue.cs`
3. `BaseWorkTask.cs`
4. `BaseTimerWorkTask.cs`
5. `WorkTaskTests.cs`

## 8. 后续建议

- 可补一份“WorkTask 生命周期与停止语义”专题
- 可评估是否需要更明确的异常处理、日志和监控扩展点
- 当前已补充生命周期专题见 [WorkTask-Lifecycle.md](./WorkTask-Lifecycle.md)
- 当前第二轮系统性体检记录见 [../../debugs/review_round_02.md](../../debugs/review_round_02.md)
