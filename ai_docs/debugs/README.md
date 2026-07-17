# debugs

本目录用于归档 `Lanymy.NET` 的调试分析文档和问题复盘。

## 当前状态

- 当前已补充一页统一排查入口：[review_candidates.md](./review_candidates.md)
- 当前已补充第一轮静态复验记录：[review_round_01.md](./review_round_01.md)
- 当前已补充已修问题汇总：[fixed_issues_summary.md](./fixed_issues_summary.md)
- 当前已补充第二轮系统性体检记录：[review_round_02.md](./review_round_02.md)
- 当前已补充分阶段治理路线：[remediation_roadmap.md](./remediation_roadmap.md)

## 阅读顺序

1. 先看 [review_candidates.md](./review_candidates.md) 了解最初候选问题池。
2. 再看 [review_round_01.md](./review_round_01.md) 与 [fixed_issues_summary.md](./fixed_issues_summary.md) 了解第一轮已落地修复。
3. 最后看 [review_round_02.md](./review_round_02.md) 与 [remediation_roadmap.md](./remediation_roadmap.md) 进入第二轮系统性治理。

## 当前归档主题

- 第一轮：聚焦已能从静态阅读直接确认的功能缺陷。
- 第二轮：聚焦 `WorkTask`、`Socket`、`Netty` 相关的生命周期、性能、吞异常和资源所有权问题。

## 建议结构

```text
debugs/
├── README.md
├── review_candidates.md
├── review_round_01.md
├── fixed_issues_summary.md
├── review_round_02.md
├── remediation_roadmap.md
└── {topic}/
    ├── README.md
    └── logs/
```
