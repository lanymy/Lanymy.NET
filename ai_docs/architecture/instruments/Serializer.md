# Serializer

本文档用于分析 `Lanymy.Common.Instruments.Serializer.*` 模块的代表实现。

## 模块定位

- 该模块族承载“具体序列化器实现”。
- 与 Helpers 侧 `JsonSerializeHelper` 的关系是：
  - Helpers 提供统一静态入口
  - Instruments 提供实际序列化器实现
- 当前最明确的代表实现是 Json.NET 版本序列化器。

## 当前代码入口

- `src/Commons/Lanymy.Common.Instruments.Serializer.JsonNetJsonSerializer/JsonNetJsonSerializer.cs`

## 核心职责

- 提供 `IJsonSerializer` 的具体实现。
- 负责对象 <-> JSON 字符串、对象 <-> JSON 文件的双向转换。
- 负责维护默认 `JsonSerializerSettings` 约定。

## 当前默认策略

- `ReferenceLoopHandling = Ignore`
- `DateFormatString = yyyy-MM-dd hh:mm:ss.fff`

这说明当前默认策略重点解决的是：

- 循环引用对象图的可序列化问题
- 日期字符串格式统一问题

## 实现特征

- 构造函数允许外部传入 `JsonSerializerSettings`，未传时则使用 Json.NET 默认行为。
- 提供同步与异步两套 API，但异步实现主要通过 `GenericityHelper.DoTaskWorkAsync(...)` 包装同步逻辑。
- 文件读写依赖 `FileTextWriter` 与 `FileTextReader`，说明该模块并不直接操作低层 `Stream`。

## 与 Helpers 的协作关系

- `JsonSerializeHelper` 默认就是实例化当前类作为仓库级默认 JSON 序列化器。
- 因此这里虽然只是一个 Instruments 实现，但实际上对整个仓库的对象序列化格式有基础影响。

## 维护时需要注意

- 默认日期格式用了小写 `hh`，属于 12 小时制格式；如果业务依赖严格时间文本比较，需要谨慎复验其预期。
- 若调整 `JsonSerializerSettings`，需要回看所有调用 `JsonSerializeHelper` 的上游模块，尤其是加密头信息、HTTP 返回体反序列化、文件持久化等路径。
- 当前模块没有覆盖多实现并存的治理文档，若未来再接入 `System.Text.Json`，应明确默认实现切换规则。

## 后续建议

- 可继续补“Serializer 抽象接口与实现清单”。
- 若后续做格式兼容评估，这一模块应作为基础排查入口之一。
