# VersionHelper

本文档用于分析 `Lanymy.Common.Helpers.VersionHelper` 模块。

## 模块定位

- 这是一个轻量版本信息辅助模块。
- 当前职责聚焦在程序集版本号与文件版本号读取。
- 它更像对 `Assembly` / `FileVersionInfo` 的便捷包装，而不是一个需要单独结果层的重型边界模块。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.VersionHelper/VersionHelper.cs`

## 主要能力

- `GetAssemblyVersion(...)`
  - 获取指定程序集的 `AssemblyName.Version`
- `GetCallDomainAssemblyVersion()`
  - 获取当前应用程序域入口程序集版本
- `GetCallDomainAssemblyFileVersion()`
  - 获取当前应用程序域入口程序集对应文件版本
- `GetCallingAssemblyVersion()`
  - 获取调用方程序集版本
- `GetFileVersionString(...)`
- `GetFileVersion(...)`

## 实现特征

- 读取程序集版本时直接依赖 `Assembly.GetName().Version`。
- 读取文件版本时通过 `CustomFileInfo` 间接包装 `FileVersionInfo`。
- `GetCallDomainAssembly*` 现在会优先使用 `Assembly.GetEntryAssembly()`，若宿主环境拿不到入口程序集，则回退到当前 helper 所在程序集，避免在测试或特殊宿主下出现空引用。
- `GetCallDomainAssemblyFileVersion()` 现在使用 `AssemblyName.Name` 计算文件名，不再依赖 `Assembly.ToString()` 文本切割。

## 本轮治理结论

- **本轮不补 `WithResult(...)` 严格层。**
- 原因：
  - 该模块主要是本地元数据读取，失败语义不复杂。
  - 当前高价值工作在于锁定“存在文件 / 缺失文件 / 调用域回退”的行为边界，而不是再引入结果模型。

## 本轮新增回归点

- `GetAssemblyVersion(...)` 返回程序集自身版本。
- `GetCallingAssemblyVersion()` 返回调用侧测试程序集版本。
- `GetFileVersionString(...)` / `GetFileVersion(...)` 对存在文件返回稳定版本号。
- 文件不存在时：
  - `GetFileVersionString(...)` 返回 `string.Empty`
  - `GetFileVersion(...)` 返回 `null`
- `GetCallDomainAssemblyVersion()` / `GetCallDomainAssemblyFileVersion()` 在入口程序集存在与回退路径下都可稳定返回版本信息。

## 维护时需要注意

- 这个模块当前偏同步、轻包装风格，不建议为了形式统一再叠一层结果模型。
- 若后续出现更复杂的“程序集探测 / 多目录扫描 / 版本比较”需求，建议新增明确能力，而不是继续往这里堆隐式推断。
