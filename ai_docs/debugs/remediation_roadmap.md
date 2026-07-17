# 第二轮分阶段治理路线

本文档用于把第二轮系统性体检结果转换成可逐步推进的治理路线。

## 目标

- 不做一次性大重构。
- 先修明确实现缺陷，再收敛生命周期语义，最后做性能与可观测性治理。
- 每一阶段都应可独立落地、独立验证、独立归档。

## 分阶段路线

### 阶段 1. 明确缺陷修复

目标：

- 先修已经能够从源码直接确认的实现错误。

范围：

- `BaseSimpleWorkTask`
- `BaseWorkTaskTriggerQueue`
- `BaseNettySocketClient`

本阶段建议项：

1. 修正 `CancellationTokenSource` 判空方向错误。
2. 修正 `_TimeTriggerTask` 局部变量遮蔽字段的问题。
3. 给 Netty 重连链增加可取消边界，避免 `StopAsync()` 后继续重连。

完成判据：

- 关键停止链不再依赖错误判空。
- 关键后台任务能被正确取消。
- 关键重连流程能被显式停止。

### 阶段 2. 生命周期语义统一

目标：

- 统一 `Dispose()` / `StopAsync()` / `Close()` 的语义，避免调用方必须记忆不同模块的顺序。

范围：

- `WorkTask` 体系
- `Socket.Abstractions`
- `Socket.Netty.Abstractions`

本阶段建议项：

1. 明确 `Dispose()` 是否等价于“停止并释放”。
2. 梳理 `Channel` / `Task` / `CancellationTokenSource` / `Socket` / `NetworkStream` 的所有权归属。
3. 对外部传入对象与内部创建对象使用不同命名或文档说明。

完成判据：

- 调用方可以只靠 API 语义判断释放责任。
- 关闭链不再出现“状态已改但资源未必释放”的明显歧义。

### 阶段 3. 可观测性治理

目标：

- 降低“失败被吞掉”的比例，让关闭失败、停止失败、发送失败具备最小可诊断性。

范围：

- `Socket.Abstractions`
- `WorkTask`
- `Crawler`
- `Helpers` 中吞异常返回 `false` 的热点模块

本阶段建议项：

1. 逐步减少核心关闭链中的 `catch {}`。
2. 不能直接抛出的地方，至少保留状态、日志或错误回调。
3. 区分“兼容性吞异常”与“基础设施级吞异常”。

完成判据：

- 核心基础设施链路的失败原因不再完全丢失。
- 关闭失败与业务失败能被区分。

### 阶段 4. 性能收敛

目标：

- 先处理高价值性能问题，不做无意义微优化。

范围：

- `.Wait()` 密集路径
- `Task.Run(...)` 包同步实现
- 队列整队复制
- 高频反射与重复分配点

本阶段建议项：

1. 优先替换关闭链和队列链上的 `.Wait()`。
2. 把“伪异步”包装收敛成真正异步或明确同步 API。
3. 评估 `ConcurrentQueue.ToList()` 的批量复制是否需要替换为更稳定的批处理方式。

完成判据：

- 核心路径阻塞点明显下降。
- 后台任务与网络关闭路径不再大量同步阻塞异步。

## 推荐执行顺序

1. `WorkTask`
2. `Socket.Abstractions`
3. `Socket.Netty.Abstractions`
4. `Crawler`
5. 其他 Helpers / Toolkits

## 文档联动要求

- 每完成一个阶段，更新：
  - [review_round_02.md](./review_round_02.md)
  - 对应 `architecture/` 专题文档
  - 当前阶段的修复汇总页
- 若新增稳定专题，必须同步更新：
  - [../README.md](../README.md)
  - [README.md](./README.md)

## 当前入口

- 总体体检结果见 [review_round_02.md](./review_round_02.md)。
- `WorkTask` 生命周期专题见 [../architecture/instruments/WorkTask-Lifecycle.md](../architecture/instruments/WorkTask-Lifecycle.md)。
- `Socket` 生命周期专题见 [../architecture/instruments/Socket-Lifecycle.md](../architecture/instruments/Socket-Lifecycle.md)。
