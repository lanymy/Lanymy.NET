# SerializeHelpers Map

本文档用于梳理 `SerializeHelper.*` 模块族当前的职责边界、依赖关系和结果语义落点。

## 1. 当前模块

当前 `Helpers` 侧的序列化相关模块包括：

- `JsonSerializeHelper`
- `BinarySerializeHelper`
- `FileSerializeHelper`
- `DataTableSerializeHelper`

与其强相关的辅助模块：

- `CompressionHelper`

## 2. 依赖关系

当前依赖链可以概括为：

```text
JsonSerializeHelper
  └─ 依赖 IJsonSerializer / JsonNetJsonSerializer

BinarySerializeHelper
  └─ 依赖 JsonSerializeHelper

FileSerializeHelper
  ├─ 依赖 BinarySerializeHelper
  ├─ 依赖 CompressionHelper
  └─ 依赖 FileHelper

DataTableSerializeHelper
  └─ 依赖 JsonSerializeHelper

CompressionHelper
  └─ 依赖 ICompresser / LanymyCompresser
```

## 3. 各模块职责

### 3.1 `JsonSerializeHelper`

职责：

- 对象 <-> JSON 字符串
- 对象 <-> JSON 文件
- 同步 / 异步 JSON 文件读写桥接

特点：

- 属于整个 `SerializeHelper.*` 体系的基础入口
- 直接承接文件边界，因此适合补严格结果层

当前结果语义：

- 字符串级 API 仍保持轻量返回
- 文件级 API 已补：
  - `SerializeToJsonFileWithResult(...)`
  - `SerializeToJsonFileWithResultAsync(...)`
  - `DeserializeFromJsonFileWithResult(...)`
  - `DeserializeFromJsonFileWithResultAsync(...)`

### 3.2 `BinarySerializeHelper`

职责：

- 对象 <-> JSON 文本字节数组

特点：

- 本质是把 `JsonSerializeHelper` 再包一层字节编码
- 当前没有直接涉及文件系统、网络或进程等外部边界

当前结论：

- 暂时保持纯内存转换 helper 定位
- 本轮不单独补结果层

### 3.3 `FileSerializeHelper`

职责：

- 对象 <-> 二进制文件
- 在 `BinarySerializeHelper` 基础上组合压缩与文件读写

特点：

- 同时触达字节转换、压缩和文件系统
- 是典型的 helper-on-helper 组合封装

当前结果语义：

- 已补：
  - `SerializeToBytesFileWithResult(...)`
  - `DeserializeFromBytesFileWithResult(...)`

### 3.4 `DataTableSerializeHelper`

职责：

- `List<T>` <-> `DataTable`

特点：

- 内部通过 JSON 做中间桥接
- 更偏“结构转换”而不是“外部资源操作”

当前结论：

- 暂时保持纯转换 helper 定位
- 本轮不单独补结果层

### 3.5 `CompressionHelper`

职责：

- 字节数组、字符串、Base64 字符串之间的压缩/解压
- 文件到文件的压缩/解压

特点：

- 既有纯内存转换入口，也有直接触达文件系统的入口
- 是 `FileSerializeHelper` 的直接依赖，但本身不负责对象序列化协议

当前结论：

- 纯内存转换 API 继续保持轻量风格
- 文件级 API 已补：
  - `CompressSourceFileToCompressFileWithResult(...)`
  - `DecompressSourceFileFromCompressFileWithResult(...)`
  - `CompressSourceFileToCompressFileWithResultAsync(...)`
  - `DecompressSourceFileFromCompressFileWithResultAsync(...)`

## 4. 为什么本轮只继续改 `JsonSerializeHelper`

本轮继续推进时，优先级判断如下：

1. `FileSerializeHelper`
   - 已在上一轮补齐结果层

2. `JsonSerializeHelper`
   - 文件读写直接接触文件系统
   - 失败原因有明显诊断价值
   - 对外已存在同步 / 异步文件 API，适合继续沿同一模式补严格层

3. `BinarySerializeHelper`
   - 当前主要是内存中编码转换
   - 失败大多属于参数、编码或 JSON 反序列化异常
   - 尚未形成“必须补结果模型”的外部边界压力

4. `DataTableSerializeHelper`
   - 也是纯内存结构转换
   - 没有直接对外部资源做操作

5. `CompressionHelper`
   - 只有文件级 API 直接接触文件系统
   - 因此只对文件压缩/解压补严格结果层，内存压缩转换保持轻量

## 5. 当前推荐调用方式

- 涉及 JSON 文件落盘/读取：
  - 优先使用 `JsonSerializeHelper` 的 `WithResult(...)` 或 `WithResultAsync(...)`
- 涉及二进制对象文件落盘/读取：
  - 优先使用 `FileSerializeHelper` 的 `WithResult(...)`
- 涉及文件压缩/解压：
  - 优先使用 `CompressionHelper` 的文件级 `WithResult(...)`
- 纯字符串、字节数组、`DataTable` 转换：
  - 当前仍可保持轻量同步 API

## 6. 后续建议

- 若未来 `BinarySerializeHelper` 被更多上层模块拿来承接输入边界，再评估是否补结果层。
- 若未来 `DataTableSerializeHelper` 开始承担文件导入/导出之类职责，应把文件边界下沉到更明确的新模块，而不是继续堆在当前 helper 上。
- 若后续继续现代化 `SerializeHelper.*`，建议优先保持“字符串/字节转换轻量，文件边界补结果层”的分层原则。
