# PathHelper

本文档用于分析 `Lanymy.Common.Helpers.PathHelper` 模块。

## 模块定位

- 这是仓库里非常基础的一类路径工具模块。
- 它既承担本地文件系统路径处理，也兼带一部分 HTTP 相对路径计算能力。
- 在整个仓库里，`FileHelper`、配置读写、资源落盘等很多模块都可能间接依赖它。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.PathHelper/PathHelper.cs`

## 主要能力

- 程序域 / 程序集路径获取
  - `GetCallDllFullPath`
  - `GetCallDllFullName`
  - `GetDllFullPath<T>`
  - `GetDllFullName<T>`
  - `GetCallDomainPath`
- 路径初始化与目录处理
  - `InitDirectoryPath`
  - `GetFolderPath`
  - `GetDirectoryParent`
  - `GetDirectoryParentName`
- 路径类型判断
  - `GetPathType`
  - `IsAbsolutePath`
  - `IsRelativePath`
- 相对路径计算
  - `CombineRelativePath`
  - `CombineSystemRelativePath`
  - `CombineHttpRelativePath`
  - `MakeRelativePath`
- 文件列表与通配符匹配
  - `GetFilesFromFolder`
  - `MatchFilesWithWildcard`
  - `CheckFileWithWildcard`
- 文件名处理
  - `GetFileName`
  - `GetFileNameWithFilterInvalidFileNameChars`
- 环境变量路径展开
  - `ExpandEnvironmentVariables`

## 实现特征

- 模块职责面比名字看起来更宽，不只是“拼路径”，还包括目录初始化、文件列表筛选、通配符匹配、环境变量展开。
- `InitDirectoryPath` 的核心策略是先通过 `GetFolderPath` 规整路径，再补目录。
- `GetFolderPath` / `GetPathType` / `GetFileName` 现在会优先相信“已存在的真实文件系统对象”和“以目录分隔符结尾的路径语法”，只有无法确认时才退回到扩展名启发式。
- `CombineRelativePath` 同时兼容系统绝对路径和 HTTP 绝对路径，说明作者希望用统一入口处理两类相对路径推导。

## 与其他模块的关系

- `FileHelper` 明确依赖 `PathHelper.InitDirectoryPath(...)` 做目标目录准备。
- 若后续继续分析配置、日志、资源落盘等模块，大概率也会频繁遇到这里的公共路径规则。

## 维护时需要注意

- 当前“文件路径还是目录路径”的判断仍然带有启发式特征，但已经优先考虑真实存在路径与目录语法，对无扩展名文件、带点号目录的误判风险比之前低。
- `GetFolderPath` 会主动补齐结尾目录分隔符，这种“标准化输出”对调用方便，但也意味着与原始输入文本不完全等价。
- `GetFilesFromFolder` 会按名称排序返回，这是一种隐式约定，调用方可能会依赖这个顺序。
- 模块同时处理本地路径和 HTTP 路径，后续扩展时要避免把 URI 规则和文件系统规则继续混杂得更重。

## 风险点

- 对“路径尚不存在、且既没有目录分隔符结尾也没有稳定上下文”的场景，最终仍需要退回到扩展名启发式判断。
- 若业务未来需要 100% 区分“扩展名目录”和“无扩展名文件”，仍应增加显式 API，而不是继续加猜测规则。

## 后续建议

- 可继续补一份“PathHelper 与 FileHelper 的职责边界”专题。
- 若未来要提升鲁棒性，建议把“路径类型推断”与“路径标准化”拆成更明确的两层能力。
