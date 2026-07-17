# module_map

本目录用于分析 `Lanymy.NET` 的模块拆分和依赖组织方式。

## 1. 总体判断

`Lanymy.NET` 的组织方式不是“少量大项目”，而是“多个细粒度类库 + 少量聚合包”。

理解这个仓库时，优先把它看成一组可独立发布、也可整体引用的功能包集合。

## 2. 当前主结构

- `Abstractions / ConstKeys / Enums`：基础层
- `ExtensionFunctions.*`：扩展方法层
- `Helpers.*`：静态辅助能力
- `Instruments.*`：机制型能力
- `*.All`：聚合包

## 3. 典型分层关系

### 3.1 基础层

- `Lanymy.Common.Abstractions`
- `Lanymy.Common.ConstKeys`
- `Lanymy.Common.Enums`

这一层主要承载：
- 公共接口
- 通用模型
- 结果对象
- 常量与枚举

### 3.2 能力层

- `Lanymy.Common.ExtensionFunctions.*`
- `Lanymy.Common.Helpers.*`
- `Lanymy.Common.Instruments.*`

这三层的定位不同：

| 层次 | 特征 |
|------|------|
| ExtensionFunctions | 以扩展方法形式增强现有类型 |
| Helpers | 以静态帮助类提供直接能力 |
| Instruments | 以抽象 + 实现方式提供机制型能力 |

### 3.3 聚合层

- `Lanymy.Common.ExtensionFunctions.All`
- `Lanymy.Common.Helpers.All`
- `Lanymy.Common.Instruments.All`
- `Lanymy.Common.All`

聚合层的作用是为使用方提供更粗粒度的引用入口。

## 4. `Lanymy.Common.All` 的位置

`Lanymy.Common.All` 是当前仓库最重要的总聚合包之一。

它当前直接引用：
- `Lanymy.Common.Abstractions`
- `Lanymy.Common.ConstKeys`
- `Lanymy.Common.Enums`
- `Lanymy.Common.ExtensionFunctions.All`
- `Lanymy.Common.Instruments.All`
- `Lanymy.Common.Helpers.All`

这意味着：
- 如果要理解“仓库对外主暴露面”，优先看 `Lanymy.Common.All`
- 如果要理解“仓库内部细拆结构”，再往各 `*.All` 和各子模块下钻

## 5. 当前维护重点

- 保持分层边界，不把基础层逻辑混进聚合层
- 控制跨模块重复实现
- 评估哪些项目是真正仍在用，哪些更偏历史遗留或示例性质

## 6. 推荐入口

- `src/Commons/`
- `src/Commons/Lanymy.Common.All/`
- `src/Commons/Lanymy.Common.Helpers.All/`
- `src/Commons/Lanymy.Common.Instruments.All/`
- `src/UnitTests/Lanymy.Common.AllTests/`
- [Commons-Projects.md](./Commons-Projects.md)

## 7. 当前风险点

- 模块数量较多，理解和升级成本偏高。
- 聚合包隐藏了真实依赖深度，做框架升级或依赖替换时需要回到底层项目核实。
- 部分模块带有明显历史兼容痕迹，不能只从当前目标框架视角判断设计取舍。

## 8. 后续建议补充

- Commons 全量项目清单
- 按层次整理的依赖图
- 聚合包与子包的关系表
