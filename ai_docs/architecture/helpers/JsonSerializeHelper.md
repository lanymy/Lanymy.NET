# JsonSerializeHelper

本文档用于分析 `Lanymy.Common.Helpers.SerializeHelper.Json` 模块的职责与结构。

## 模块定位

- 这是 Helpers 侧的 JSON 序列化统一入口。
- 对上层调用者暴露静态方法，对下层则委托给 `IJsonSerializer` 实现。
- 在仓库内部，它既是独立能力点，也是很多其他模块的基础设施依赖。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.SerializeHelper.Json/JsonSerializeHelper.cs`

## 主要能力

- JSON 字符串序列化与反序列化
  - `SerializeToJson`
  - `DeserializeFromJson`
- 异步 JSON 字符串处理
  - `SerializeToJsonAsync`
  - `DeserializeFromJsonAsync`
- JSON 文件读写
  - `SerializeToJsonFile`
  - `DeserializeFromJsonFile`
- 异步 JSON 文件读写
  - `SerializeToJsonFileAsync`
  - `DeserializeFromJsonFileAsync`

## 默认实现

- 默认序列化器是：
  - `new JsonNetJsonSerializer(JsonNetJsonSerializer.GetDefaultJsonSerializerSettings())`
- 也就是说，当前仓库在 JSON 层默认走的是 Json.NET 实现，而不是 `System.Text.Json`。

## 实现特征

- 典型的“静态 Helper + 可注入实现”双层结构。
- 通过 `GenericityHelper.GetInterface(jsonSerializer, DefaultJsonSerializer)` 允许调用方覆盖默认实现。
- 对空对象或空 JSON 采用快速返回策略：
  - 空对象序列化返回空字符串
  - 空 JSON 反序列化返回 `default(T)`
- 文件级 API 只是向序列化器继续透传，不在 Helper 中重复处理文件细节。

## 在仓库中的角色

- 该模块是多个 Helper / Instrument 的底层依赖。
- `HttpHelper`、`LanymyCrypto`、`LanymyAesCrypto` 等模块都直接或间接依赖它做对象与字符串的桥接。
- 因此它虽然位于 Helpers，但实际上已经具备“基础设施中枢”性质。

## 维护时需要注意

- 由于大量上层模块依赖 JSON 结果格式，修改默认设置时要把兼容性放在首位。
- 这里的异步能力本质上依然是对同步逻辑的包装，不等价于真正流式 JSON 处理。
- 当前约束 `where T : class` 比较广泛，若未来要支持值类型序列化，需要评估整个仓库 API 风格是否一致。

## 风险点

- 作为仓库级基础模块，一旦替换默认序列化器，影响面会非常大。
- Json.NET 默认配置目前只明确忽略循环引用和指定日期格式，未对枚举、Null 处理、大小写策略做更细的统一约束。

## 后续建议

- 可继续补一份“Helpers.JsonSerializeHelper 与 Instruments.JsonNetJsonSerializer 的分层关系”专题。
- 若后续有现代化目标，建议先做全仓库 JSON 兼容面盘点，再考虑 `System.Text.Json` 迁移。
