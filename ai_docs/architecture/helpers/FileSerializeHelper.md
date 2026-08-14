# FileSerializeHelper

本文档用于分析 `Lanymy.Common.Helpers.SerializeHelper.File` 模块的职责、实现风格与维护关注点。

## 模块定位

- 面向“对象 <-> 二进制文件”的轻量静态辅助模块。
- 本质上是 `BinarySerializeHelper + CompressionHelper + FileHelper` 的组合封装。
- 适用于把对象以 JSON 文本字节的形式落盘，再按需做压缩与回读。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.SerializeHelper.File/FileSerializeHelper.cs`

## 主要能力

- 对象序列化到二进制文件
  - `SerializeToBytesFile`
  - `SerializeToBytesFileWithResult`
- 从二进制文件反序列化对象
  - `DeserializeFromBytesFile`
  - `DeserializeFromBytesFileWithResult`

## 实现特征

- 写入流程：
  - 先使用 `BinarySerializeHelper.SerializeToBytes(...)` 把对象转成 JSON 文本字节
  - 再按 `ifCompressBytes` 决定是否经过 `CompressionHelper.CompressBytesToBytes(...)`
  - 最后通过 `FileHelper.CreateBinaryFileWithResult(...)` 落盘
- 读取流程：
  - 先通过 `FileHelper.GetBinaryFileBytesWithResult(...)` 读取文件
  - 再按 `ifDecompressBytes` 决定是否解压
  - 最后通过 `BinarySerializeHelper.DeserializeFromBytes<T>(...)` 反序列化成对象
- 当前已经补充严格结果层，结果模型为：
  - `BinaryFileSerializationResultModel<T>`

## 维护时需要注意

- 该模块不是独立序列化协议，只是对现有 JSON 字节序列化 + 压缩 + 文件写入的组合包装。
- 文件缺失、压缩解压失败、JSON 反序列化失败等异常，当前严格层都会保留在 `Exception` 中。
- 当前读取侧已经切到 `FileHelper.GetBinaryFileBytesWithResult(...)`，文件路径和字节长度诊断由下层读取结果统一回填，不再在当前 helper 层重复猜测文件读取失败原因。
- 兼容层 `DeserializeFromBytesFile(...)` 仍然保持历史抛异常语义。
- 兼容层 `SerializeToBytesFile(...)` 仍然保持“失败直接抛异常”的调用方式。
- 若新调用方需要诊断文件路径、字节长度、压缩标志和异常边界，优先使用 `WithResult(...)` 系列入口。

## 当前结果模型语义

`BinaryFileSerializationResultModel<T>` 当前主要包含：

- `FilePath`
- `IsCompressed`
- `BytesLength`
- `Model`
- `IsSuccess`
- `Exception`

其中：

- 写入场景下，`Model` 表示待写入对象
- 读取场景下，`Model` 表示反序列化得到的对象

## 适合的使用场景

- 简单配置对象落盘
- 本地缓存对象文件读写
- 需要复用仓库现有 JSON/压缩/文件能力的轻量序列化场景

## 不适合直接扩张的方向

- 大对象或超大文件流式序列化
- 强协议约束的跨语言二进制格式
- 需要版本演进策略的长期持久化格式

## 后续建议

- 若未来 `SerializeHelper.*` 体系继续补结果层，可优先沿用当前“兼容层保旧入口、严格层补详细结果”的模式。
- 若后续出现更多“对象文件持久化”场景，可再评估是否需要抽出更通用的文件序列化结果模型层。
