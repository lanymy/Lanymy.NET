# Cache

本文档用于分析 `Lanymy.Common.Instruments.Cache.*` 模块族的结构与职责。

## 模块定位

- 提供仓库内的内存缓存抽象与两类典型实现。
- 设计上分为抽象层、原生缓存实现层、自定义字典缓存实现层。
- 相比 `Helpers`，这里更偏机制型封装，强调统一 Key 约定和多实现兼容。

## 当前代码入口

- 抽象层
  - `src/Commons/Lanymy.Common.Instruments.Cache.Abstractions/BaseMemoryCache.cs`
  - `src/Commons/Lanymy.Common.Instruments.Cache.Abstractions/Interfaces/ICustomMemoryCache.cs`
- 实现层
  - `src/Commons/Lanymy.Common.Instruments.Cache.CoreMemoryCache/CoreMemoryCache.cs`
  - `src/Commons/Lanymy.Common.Instruments.Cache.CustomMemoryCache/CustomMemoryCache.cs`

## 结构分层

- `BaseCache`
  - 统一定义默认 Key 规则、枚举 Key 规则、通用 Set/Get/Remove/Clear 约定。
- `ICustomMemoryCache`
  - 通过组合多个 Key-Value 接口定义缓存能力面。
- `CoreMemoryCache`
  - 基于 `Microsoft.Extensions.Caching.Memory.IMemoryCache`。
  - 支持 `MemoryCacheEntryOptions`，可带过期与优先级信息。
- `CustomMemoryCache`
  - 基于 `ConcurrentDictionary<string, object>`。
  - 实现简单、直接、不支持过期控制。

## 关键设计点

- 默认 Key 以 `typeof(T).FullName` 为基础，这意味着缓存天然带类型命名空间语义。
- 枚举 Key 会格式化成 `{枚举类型全名}_{枚举值}`，用于区分同类型下的多项缓存值。
- 抽象层把“按类型读写”和“按字符串 Key 读写”统一在同一套基类中，降低了调用门槛。

## 两类实现的差异

- `CoreMemoryCache`
  - 优点：能直接使用微软原生缓存能力，支持过期策略和优先级。
  - 特点：默认 `SetValue(string, object)` 走 `NeverRemove`，说明仓库作者把“显式不过期”作为默认行为。
  - 注意：`Clear()` 调用 `MemoryCache.Compact(1)`，注释中也说明无法清理 `NeverRemove` 项。
- `CustomMemoryCache`
  - 优点：实现轻、依赖少、行为容易理解。
  - 特点：全部行为围绕字典展开，没有额外过期语义。
  - 适合：轻量进程内共享状态与测试场景。

## 维护时需要注意

- `BaseCache` 虽然提供了很多泛型包装，但真正的缓存一致性仍取决于调用方是否统一使用默认 Key 规则。
- `CoreMemoryCache` 的默认 `NeverRemove` 策略很强势，若调用方误以为 `Clear()` 能完全清空，会产生认知偏差。
- `CustomMemoryCache` 中 `SetValue` 的 `AddOrUpdate(key, value, (k, v) => v)` 会保留旧值，按当前代码看并不会真正覆盖为新值，这一点值得后续单独复验。

## 典型适用场景

- 进程内缓存
- 类型默认缓存值托管
- 需要兼容“简单字典缓存”和“微软原生缓存”的统一调用面

## 后续建议

- 可继续补“Cache 模块接口关系图”。
- 若后续进行逻辑复验，`CustomMemoryCache.SetValue` 的更新行为建议优先核实。
