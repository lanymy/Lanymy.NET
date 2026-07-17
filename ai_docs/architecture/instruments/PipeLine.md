# PipeLine

## 1. 模块定位

- 对应项目：
  - `src/Commons/Lanymy.Common.Instruments.PipeLine.Abstractions`
  - `src/Commons/Lanymy.Common.Instruments.PipeLine`
- 当前模块定位是一个轻量的管道处理机制，偏责任链 / 流水线风格。

## 2. 当前核心结构

从当前实现看，主要由以下角色组成：

- `BasePipeLineDataContext<TData>`
- `BasePipeLineEventHandler<TData>`
- `BasePipeLineHandlerContext<TPipeLineDataContext, TData, TPipeLineEventHandler>`
- `PipeLineDataContext<TData>`
- `PipeLineHandlerContext<...>`

## 3. 当前执行模型

- `DataContext` 承载：
  - `Data`
  - `Environment` 字典
- `EventHandler` 承载：
  - `OrderIndex`
  - `ProcessAsync(...)`
  - `OnProcessAsync(...)`
- `HandlerContext` 负责：
  - 构造上下文
  - 自动发现并注册处理器
  - 按顺序执行处理器

## 4. 当前实现特征

- `Build()` 通过反射扫描程序集，自动发现处理器类型
- 处理器按 `OrderIndex` 排序执行
- 通过 `CancelEventArgs` 实现停止后续处理的机制
- `Environment` 采用开放式字典承载上下文扩展数据

## 5. 当前维护关注点

- 这是一个“灵活但隐式”的机制模块。
- 优点是扩展方便，新增处理节点成本低。
- 代价是：
  - 类型发现依赖约定
  - 行为链路不如显式注册直观
  - 新接手时理解成本较高

## 6. 当前风险点

- 反射注册机制让问题排查不如显式依赖关系清晰。
- `Environment` 是字典，灵活但缺少强类型约束。
- 若后续要增强调试性或诊断能力，可能需要补充更明确的注册与跟踪信息。

## 7. 推荐阅读入口

1. `BasePipeLineHandlerContext.cs`
2. `BasePipeLineEventHandler.cs`
3. `BasePipeLineDataContext.cs`
4. `PipeLineHandlerContext.cs`
5. `PipeLineDataContext.cs`

## 8. 后续建议

- 可补一份“PipeLine 使用示例与约定说明”
- 可评估是否需要补充显式注册模式或调试日志能力
