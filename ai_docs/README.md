# AI 协作文档

> `Lanymy.NET` 项目面向 AI 编码代理和协作开发者的统一文档入口。  
> 全局工作规则见仓库根目录 [AGENTS.md](../AGENTS.md)；本目录按“目的域”分层组织专项文档。

## 目录导航

| 子目录 | 用途 | 入口 |
|--------|------|------|
| [agents](./agents/) | AI 协作认知：项目索引与仓库概览 | [README.md](./agents/README.md) |
| [architecture](./architecture/) | 架构分析：构建、模块、兼容、测试 | [README.md](./architecture/README.md) |
| [conventions](./conventions/) | 编码规范与文档约定 | [README.md](./conventions/README.md) |
| [debugs](./debugs/) | 调试文档与问题归档 | [README.md](./debugs/README.md) |
| [design](./design/) | 设计与资源说明 | [README.md](./design/README.md) |
| [skills](./skills/) | Skills 设计与预留目录 | [README.md](./skills/README.md) |

## 阅读引导

- **新人 / 新 AI 会话**：先读仓库根目录 [AGENTS.md](../AGENTS.md) 获取全局规则，再按需进入本目录对应子目录。
- **快速理解仓库结构**：读 [agents/AGENTS-INDEX.md](./agents/AGENTS-INDEX.md)。
- **快速理解项目定位**：读 [agents/AGENTS-Lanymy.Common.md](./agents/AGENTS-Lanymy.Common.md)。
- **理解构建与打包母板**：读 [architecture/build_and_packaging/README.md](./architecture/build_and_packaging/README.md)。
- **理解模块拆分**：读 [architecture/module_map/README.md](./architecture/module_map/README.md)。
- **理解跨框架兼容**：读 [architecture/compatibility/README.md](./architecture/compatibility/README.md)。
- **理解 Helpers / Instruments 两大能力面**：读 [architecture/helpers/README.md](./architecture/helpers/README.md) 与 [architecture/instruments/README.md](./architecture/instruments/README.md)。
- **了解测试现状**：读 [architecture/testing/README.md](./architecture/testing/README.md)。
- **确认功能修改后的测试补齐规则**：读 [conventions/Testing-Convention.md](./conventions/Testing-Convention.md)。
- **查看问题复验与修复过程**：读 [debugs/review_round_01.md](./debugs/review_round_01.md) 与 [debugs/fixed_issues_summary.md](./debugs/fixed_issues_summary.md)。
- **查看第二轮系统性体检**：读 [debugs/review_round_02.md](./debugs/review_round_02.md) 与 [debugs/remediation_roadmap.md](./debugs/remediation_roadmap.md)。

## 目录结构

```text
ai_docs/
├── README.md
├── agents/
│   ├── README.md
│   ├── AGENTS-INDEX.md
│   └── AGENTS-Lanymy.Common.md
├── architecture/
│   ├── README.md
│   ├── build_and_packaging/
│   │   ├── README.md
│   │   └── Import-Chain.md
│   ├── module_map/
│   │   ├── README.md
│   │   └── Commons-Projects.md
│   ├── compatibility/
│   │   └── README.md
│   ├── helpers/
│   │   ├── README.md
│   │   ├── FileHelper.md
│   │   ├── HttpHelper.md
│   │   ├── ImageHelper.md
│   │   ├── JsonSerializeHelper.md
│   │   ├── NetworkHelper.md
│   │   ├── PathHelper.md
│   │   ├── ProcessHelper.md
│   │   ├── QrCodeHelper.md
│   │   └── SecurityHelper.md
│   ├── instruments/
│   │   ├── README.md
│   │   ├── Cache.md
│   │   ├── CMD.md
│   │   ├── CMD.Abstractions.md
│   │   ├── CMD-Semantics.md
│   │   ├── Crypto.md
│   │   ├── PipeLine.md
│   │   ├── Serializer.md
│   │   ├── Socket.md
│   │   ├── Socket.Abstractions.md
│   │   ├── Socket-Relations.md
│   │   ├── Socket-Sequences.md
│   │   ├── Socket-Lifecycle.md
│   │   ├── WorkTask.md
│   │   └── WorkTask-Lifecycle.md
│   └── testing/
│       └── README.md
├── conventions/
│   ├── README.md
│   ├── FileHeader-Convention.md
│   ├── Naming-Convention.md
│   ├── MultiTargeting-Convention.md
│   └── Testing-Convention.md
├── debugs/
│   ├── README.md
│   ├── review_candidates.md
│   ├── review_round_01.md
│   ├── fixed_issues_summary.md
│   ├── review_round_02.md
│   └── remediation_roadmap.md
├── design/
│   └── README.md
└── skills/
    └── README.md
```

## 文档治理约定

- 根 [AGENTS.md](../AGENTS.md) 只保留全局规则和稳定约束，不堆具体实现细节。
- `agents/` 用于沉淀仓库级认知与入口索引。
- `architecture/` 用于沉淀按主题组织的技术分析。
- `conventions/` 用于沉淀可长期复用的编码和协作约定。
- `debugs/` 用于归档调试分析；如后续出现专题问题，可在其下增建子目录。
- 当 `architecture/` 或 `debugs/` 下新增稳定专题文档时，应同步更新本页导航和目录结构，避免“文档已存在但入口失效”。
- `design/` 与 `skills/` 当前以预留为主，后续按需扩展。
- 各子目录必须以 `README.md` 作为本层索引入口。
