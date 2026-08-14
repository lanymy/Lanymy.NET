# Tasks

本目录用于维护 `Lanymy.NET` 后续要处理的任务清单，并为每个任务提供统一的回归验证落点。

与 `architecture/`、`conventions/`、`debugs/` 的职责区别如下：

- `architecture/` 负责解释“系统是什么、风险点在哪里”。
- `conventions/` 负责说明“后续修改应遵循什么规则”。
- `debugs/` 负责归档“问题是如何被发现、定位和修复的”。
- `tasks/` 负责维护“接下来做什么、做到什么程度、如何回归验证”。

## 1. 当前入口

- 活跃任务清单见 [active-regression-backlog.md](./active-regression-backlog.md)
- 结果型 API 调用面盘点见 [helper-result-api-adoption-audit.md](./helper-result-api-adoption-audit.md)
- 已完成任务归档入口见 [archive/README.md](./archive/README.md)

## 2. 使用方式

- 新任务先登记到活跃任务清单，并明确优先级。
- 每个任务都应显式记录：
  - 目标模块
  - 影响代码
  - 对应测试文件
  - 功能验证点
  - 性能验证点
  - 稳定性验证点
  - 用户完整回归状态
- 任务完成后，再移入 `archive/` 归档，避免主清单持续膨胀。

## 3. 维护约定

- 任务文档优先服务“执行与回归验证”，不重复抄录大段架构分析。
- 若任务来自某份专题分析文档，应在任务条目中直接挂回对应专题链接。
- 若任务对应功能修改，默认仍需遵循 [../conventions/Testing-Convention.md](../conventions/Testing-Convention.md) 中的测试补齐约束。
