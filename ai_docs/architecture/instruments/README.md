# instruments

本目录用于分析 `Lanymy.Common.Instruments.*` 模块族。

## 范围

- Cache
- CMD
- Compresser
- Crawler
- Crypto
- PipeLine
- Serializer
- Socket
- WorkTask

## 结构特征

- 比 Helpers 更偏机制型封装
- 常见模式包括抽象层 + 实现层 + 聚合层
- 部分模块带有较明显的历史兼容与反射式装配特征

## 典型模块

- `PipeLine`
- `WorkTask`
- `Cache`
- `Crypto`
- `Crawler`
- `Serializer`
- `Socket`

## 专题文档

| 文档 | 用途 |
|------|------|
| [PipeLine.md](./PipeLine.md) | 管道机制模块专题 |
| [WorkTask.md](./WorkTask.md) | 后台任务与队列模块专题 |
| [Cache.md](./Cache.md) | 缓存机制模块专题 |
| [CMD.md](./CMD.md) | 命令执行机制模块专题 |
| [CMD.Abstractions.md](./CMD.Abstractions.md) | 命令执行抽象层专题 |
| [CMD-Semantics.md](./CMD-Semantics.md) | 命令执行成功语义专题 |
| [Crypto.md](./Crypto.md) | 加解密机制模块专题 |
| [Serializer.md](./Serializer.md) | 序列化机制模块专题 |
| [Socket.md](./Socket.md) | Socket 通信机制模块专题 |
| [Socket.Abstractions.md](./Socket.Abstractions.md) | Socket 抽象层专题 |
| [Socket-Relations.md](./Socket-Relations.md) | Socket 抽象层关系图式说明 |
| [Socket-Sequences.md](./Socket-Sequences.md) | Socket 运行时序说明 |
| [Socket-Lifecycle.md](./Socket-Lifecycle.md) | Socket 生命周期、关闭链与性能风险专题 |
| [WorkTask-Lifecycle.md](./WorkTask-Lifecycle.md) | WorkTask 生命周期、停止语义与资源释放专题 |

## 当前维护重点

- 区分抽象层和实现层的职责
- 控制反射式装配和隐式约定带来的理解成本
- 评估哪些机制型模块仍有真实使用价值
- 把 `WorkTask` / `Socket` 的生命周期和关闭语义从“源码隐含约定”沉淀成显式文档

## 当前风险点

- 一些模块结构相对灵活，但编译期可见性不强，接手成本高。
- 抽象和实现拆分较细，修改时容易只改一层导致行为不一致。

## 后续建议补充

- 各 Instruments 子模块职责表
- 抽象层与实现层关系图
- 典型模块深挖文档
- 第二轮系统性体检记录见 [../../debugs/review_round_02.md](../../debugs/review_round_02.md)
