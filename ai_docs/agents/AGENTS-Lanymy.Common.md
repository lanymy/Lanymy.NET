# AGENTS-Lanymy.Common

## 1. 项目定位

- `Lanymy.NET` 是一个通用 .NET 辅助类库仓库。
- 组织方式以“细粒度可独立引用的小包 + 聚合包”为主，而不是单体应用。
- 主要关注点不是单一业务流程，而是：
  - 公共 API 稳定性
  - 多目标框架兼容
  - 构建与打包一致性
  - 模块职责边界

## 2. 当前结构特征

- `src/Commons/` 下包含大量细拆项目。
- `Helpers` 更偏静态工具类能力。
- `Instruments` 更偏机制型、可组合能力。
- `ExtensionFunctions` 提供扩展方法层。
- `Abstractions / ConstKeys / Enums` 提供公共基础层。
- `Lanymy.Common.All` 作为总聚合包暴露整体能力。
- 构建与打包通过 `Build/MsBuildFiles/` 下的自定义 props / targets 母板统一管理。

## 3. 当前维护重点

- 维护公共构建母板的一致性
- 处理多目标框架带来的兼容成本
- 评估历史依赖和现代化迁移机会
- 提升测试覆盖与有效性

## 4. 不建议的做法

- 不要把本仓库当成单一业务应用理解。
- 不要轻易在多个模块里复制相近实现。
- 不要未经评估就修改公共打包配置、目标框架或聚合引用。
- 不要只从单个子项目视角判断仓库结构，应同时考虑聚合包和跨框架影响。

## 5. 相关入口

- 构建与打包： [../architecture/build_and_packaging/README.md](../architecture/build_and_packaging/README.md)
- 模块地图： [../architecture/module_map/README.md](../architecture/module_map/README.md)
- Commons 全量项目清单： [../architecture/module_map/Commons-Projects.md](../architecture/module_map/Commons-Projects.md)
- 兼容性： [../architecture/compatibility/README.md](../architecture/compatibility/README.md)
- Helpers 能力面： [../architecture/helpers/README.md](../architecture/helpers/README.md)
- Instruments 能力面： [../architecture/instruments/README.md](../architecture/instruments/README.md)
- 测试现状： [../architecture/testing/README.md](../architecture/testing/README.md)
