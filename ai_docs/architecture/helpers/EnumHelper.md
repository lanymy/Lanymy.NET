# EnumHelper

本文档用于分析 `Lanymy.Common.Helpers.EnumHelper` 模块。

## 模块定位

- 这是一个轻量枚举映射辅助模块。
- 当前职责主要是：
  - 枚举项导航
  - 枚举项列表与字典投影
  - Flags 枚举拆解
  - 枚举映射缓存复用
- 它本质上是内存内映射工具，不属于需要补严格结果层的外部边界模块。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.EnumHelper/EnumHelper.cs`

## 主要能力

- `GetEnumItem(...)`
- `GetEnumItemDictionary<TEnum>()`
- `GetEnumItemDictionary(Type enumType)`
- `GetEnumItemList<TEnum>()`
- `GetEnumItemList(Type enumType)`
- `GetEnumItemListRemoveEnumItems<TEnum>(...)`
- `GetEnumFlagsItemDictionary(...)`
- `GetEnumFlagsItemList(...)`
- `HasFlag(...)`

## 实现特征

- 内部依赖 `EnumMapper` 与内存缓存器复用映射结果。
- 当前缓存键使用 `enumType.FullName`。
- `GetEnumItemDictionary(...)` 返回的是底层 mapper 暴露出来的只读字典，因此同一枚举类型的重复读取会复用同一份映射对象。
- Flags 拆解当前依赖 `Enum.ToString()` 的逗号分隔结果，因此它锁定的是 .NET 默认 Flags 文本展开语义。

## 本轮治理结论

- **本轮不补 `WithResult(...)` 严格层。**
- 原因：
  - 模块核心价值是内存映射和枚举导航，不是失败诊断。
  - 当前更适合通过测试锁定缓存复用、过滤语义和 Flags 拆解行为。

## 本轮新增回归点

- 同一枚举类型的字典映射会复用缓存结果。
- `GetEnumItem(...)` 可稳定返回对应枚举项。
- `GetEnumItemListRemoveEnumItems(...)` 仅排除指定项。
- Flags 枚举的字典拆解、列表拆解与 `HasFlag(...)` 行为。

## 维护时需要注意

- 当前实现预期输入必须是合法枚举类型；若后续需要更强的泛型约束或并发初始化策略，应单独设计，不要在现有轻量 API 上做隐式语义切换。
- 缓存机制当前优先考虑复用与低开销；若未来引入更复杂的元数据投影，建议先评估缓存键和线程安全策略。
