# AGENTS

## 1. 文档目的

- 本文件服务于 `Lanymy.NET` 项目，面向 AI 编码代理、自动化助手和后续接手开发者。
- 目标不是重复 [README.md](./README.md)，而是沉淀当前项目的工作规则、架构入口、修改边界、验证要求和长期有效的协作约定。
- 默认沟通语言为中文。
- 仓库级 AI 协作文档统一维护在 [./ai_docs/README.md](./ai_docs/README.md)。
- 本文件尽量保持稳定，只保留全局规则和长期有效约束；易变化的专项内容应下沉到 `ai_docs/` 对应子目录。

## 2. 规则优先级

- 当以下信息来源发生冲突时，按优先级从高到低执行：
  - 用户在当前会话中的最新明确指令
  - 当前仓库中的实际代码与配置
  - 本文件 `AGENTS.md`
  - 历史分析结论、旧文档和外部说明
- 若发现本文件内容与当前代码明显不一致，应先回读代码，再决定修正文档还是调整实现。

## 3. 项目级边界

- 项目名：`Lanymy.NET`
- 项目定位：通用 .NET 辅助类库仓库，核心能力集中在 `src/Commons/`
- 组织方式：大量细粒度类库项目 + 若干聚合项目 + 测试项目
- 主要技术方向：
  - 多目标框架类库
  - 公共构建母板（`Build/MsBuildFiles/`）
  - Helpers / Instruments / ExtensionFunctions / Abstractions 分层
- 本仓库不是单体业务应用，分析和修改时应优先从“模块职责、跨框架兼容、打包发布、公共 API 稳定性”视角切入。

## 4. 用户偏好与编码约定

- 默认协作语言：中文
- C# 常量命名：`PascalCase`
- 资源 ID 和文件名：优先使用 `snake_case`
- 偏好在 C# 源码文件头部包含标准块注释
- XML 注释要求完整包含 `<summary>`、`<param>` 和 `<returns>`
- 面对复杂任务，优先先做方案评估，再分段实施
- 偏好通用化方案，不偏好只针对单个实例的特判补丁
- 抽象继承默认遵循“约束源单一”原则：若抽象成员已由最上层基类定义，中间抽象层不重复声明同一成员，也不应用空实现削弱编译期约束；具体类需显式实现自身清理或扩展逻辑

## 5. 进行代码修改时的通用优先级

- 第一步：先确认修改落点属于哪个模块或关注域，再进入对应入口文件阅读。
- 第二步：优先保持现有职责边界，避免把构建逻辑、跨框架兼容逻辑、模块聚合逻辑和具体功能实现混到一起。
- 第三步：尽量局部修复，避免无必要的大范围重写。
- 若改动涉及公共 API、打包行为或目标框架，应先评估影响面再修改。
- 若改动涉及功能行为，默认需要同步在 `src/UnitTests/Lanymy.Common.AllTests` 补齐对应测试；这里的“功能回归”默认同时包含功能正确性、性能验证和稳定性验证；详细规则见 [ai_docs/conventions/Testing-Convention.md](./ai_docs/conventions/Testing-Convention.md)。

## 6. 推荐阅读与专项文档

- 想快速理解仓库级规则，先读本文件。
- AI 协作文档统一入口见 [ai_docs/README.md](./ai_docs/README.md)。
- 若要了解模块地图与入口关系，优先读 [ai_docs/agents/AGENTS-INDEX.md](./ai_docs/agents/AGENTS-INDEX.md)。
- 若要理解当前仓库整体定位与主结构，优先读 [ai_docs/agents/AGENTS-Lanymy.Common.md](./ai_docs/agents/AGENTS-Lanymy.Common.md)。
- 若要继续深挖实现与分层，进入 [ai_docs/architecture/README.md](./ai_docs/architecture/README.md)。
- 若要确认功能修改后的测试补齐约束，读 [ai_docs/conventions/Testing-Convention.md](./ai_docs/conventions/Testing-Convention.md)。
- 若要确认通用 Helper / Instrument 的返回值、结果模型与异常边界，读 [ai_docs/conventions/HelperResult-Semantics-Convention.md](./ai_docs/conventions/HelperResult-Semantics-Convention.md)。
- 若要确认兼容层仍保留的历史语义边界，读 [ai_docs/conventions/HelperCompatibility-Legacy-Semantics.md](./ai_docs/conventions/HelperCompatibility-Legacy-Semantics.md)。

## 7. 通用修改前后检查

- 修改前：
  - 确认改动所在模块、影响范围和主要入口文件
  - 确认是否影响公共 API、聚合包依赖或多目标框架兼容性
- 修改后：
  - 检查相关文件诊断
  - 如涉及构建或打包，至少确认受影响项目的构建状态
  - 如涉及专项域，再按对应专项文档中的检查项补充验证
  - 如涉及功能修改，默认应同步补齐 `Lanymy.Common.AllTests` 对应测试；完整回归由用户手动执行，并据此确认功能、性能和稳定性

## 8. 如果需要继续扩展本文件

- 优先补充“已经验证通过”的全局规则，而不是猜测性的想法。
- 若某项策略只对某个模块成立，优先写入 `ai_docs/` 对应专题文档，不直接堆到主文档。
- 若某项策略是试验性的，应显式标注“试验中”或“待确认”。
