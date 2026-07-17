# architecture

本目录用于沉淀 `Lanymy.NET` 的技术架构分析，按主题分目录维护。

## 子目录导航

| 子目录 | 用途 | 入口 |
|--------|------|------|
| [build_and_packaging](./build_and_packaging/) | 构建母板、版本、打包规则 | [README.md](./build_and_packaging/README.md) |
| [module_map](./module_map/) | 仓库模块地图与依赖分层 | [README.md](./module_map/README.md) |
| [compatibility](./compatibility/) | 多目标框架与兼容性策略 | [README.md](./compatibility/README.md) |
| [helpers](./helpers/) | Helpers 能力面分析 | [README.md](./helpers/README.md) |
| [instruments](./instruments/) | Instruments 能力面分析 | [README.md](./instruments/README.md) |
| [testing](./testing/) | 测试项目与验证现状 | [README.md](./testing/README.md) |

## 阅读建议

- 初次接手，建议先读 `module_map`。
- 涉及构建、版本、打包时，优先进入 `build_and_packaging`。
- 涉及跨框架行为差异时，优先进入 `compatibility`。
- 想看 `Commons` 全量项目清单，进入 [module_map/Commons-Projects.md](./module_map/Commons-Projects.md)。
- 想看构建母板的真实导入顺序，进入 [build_and_packaging/Import-Chain.md](./build_and_packaging/Import-Chain.md)。
- 想看典型 Helper 模块，进入 [helpers/HttpHelper.md](./helpers/HttpHelper.md)、[helpers/FileHelper.md](./helpers/FileHelper.md)、[helpers/ImageHelper.md](./helpers/ImageHelper.md)、[helpers/NetworkHelper.md](./helpers/NetworkHelper.md)、[helpers/PathHelper.md](./helpers/PathHelper.md)、[helpers/ProcessHelper.md](./helpers/ProcessHelper.md)、[helpers/SecurityHelper.md](./helpers/SecurityHelper.md)、[helpers/JsonSerializeHelper.md](./helpers/JsonSerializeHelper.md) 与 [helpers/QrCodeHelper.md](./helpers/QrCodeHelper.md)。
- 想看典型 Instrument 模块，进入 [instruments/PipeLine.md](./instruments/PipeLine.md)、[instruments/WorkTask.md](./instruments/WorkTask.md)、[instruments/Cache.md](./instruments/Cache.md)、[instruments/CMD.md](./instruments/CMD.md)、[instruments/CMD.Abstractions.md](./instruments/CMD.Abstractions.md)、[instruments/CMD-Semantics.md](./instruments/CMD-Semantics.md)、[instruments/Crypto.md](./instruments/Crypto.md)、[instruments/Serializer.md](./instruments/Serializer.md)、[instruments/Socket.md](./instruments/Socket.md)、[instruments/Socket.Abstractions.md](./instruments/Socket.Abstractions.md)、[instruments/Socket-Relations.md](./instruments/Socket-Relations.md) 与 [instruments/Socket-Sequences.md](./instruments/Socket-Sequences.md)。
- 想看当前已识别但尚未正式复验的可疑点，进入 [debugs/review_candidates.md](../debugs/review_candidates.md)。
