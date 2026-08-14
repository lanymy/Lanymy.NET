# CompressionHelper

本文档用于分析 `Lanymy.Common.Helpers.CompressionHelper` 模块的职责边界、结果语义和与 `SerializeHelper.*` 的关系。

## 模块定位

- 面向字符串、字节数组、Base64 字符串和文件的 GZip 压缩/解压辅助入口。
- 对上层提供静态 helper 风格 API，对下层委托给 `ICompresser` / `LanymyCompresser`。
- 在仓库内既是独立压缩能力点，也是 `FileSerializeHelper` 等组合 helper 的基础依赖。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.CompressionHelper/CompressionHelper.cs`

## 主要能力

### 1. 纯内存转换

- `byte[] <-> byte[]`
- `byte[] <-> Base64 string`
- `string <-> byte[]`
- `string <-> Base64 string`

这类 API 当前保持轻量风格：

- 成功直接返回转换结果
- 失败仍以异常形式暴露
- 本轮不单独补结果模型

### 2. 文件到文件压缩/解压

- `CompressSourceFileToCompressFile(...)`
- `DecompressSourceFileFromCompressFile(...)`
- `CompressSourceFileToCompressFileAsync(...)`
- `DecompressSourceFileFromCompressFileAsync(...)`

这类 API 直接触达文件系统，是本轮补严格结果层的重点。

## 当前结果语义

文件级 API 已补：

- `CompressSourceFileToCompressFileWithResult(...)`
- `DecompressSourceFileFromCompressFileWithResult(...)`
- `CompressSourceFileToCompressFileWithResultAsync(...)`
- `DecompressSourceFileFromCompressFileWithResultAsync(...)`

结果模型：

- `CompressionFileOperationResultModel`

主要字段：

- `SourcePath`
- `TargetPath`
- `IsCompressOperation`
- `IsSuccess`
- `Exception`
- `TargetFileExists`

## 兼容层历史语义

### 文件压缩

- `CompressSourceFileToCompressFile(...)`
- `CompressSourceFileToCompressFileAsync(...)`

保留旧行为：

- 源文件不存在时静默 no-op
- 不因为“源文件不存在”直接抛异常

### 文件解压

- `DecompressSourceFileFromCompressFile(...)`
- `DecompressSourceFileFromCompressFileAsync(...)`

保留旧行为：

- 压缩文件不存在时静默 no-op
- 不因为“压缩文件不存在”直接抛异常

### 严格层语义

对应 `WithResult(...)` 入口会把“源文件/压缩文件不存在”视为失败，并通过 `FileNotFoundException` 返回诊断信息。

## 与 `SerializeHelper.*` 的关系

- `FileSerializeHelper` 直接依赖 `CompressionHelper` 做字节压缩/解压
- `CompressionHelper` 自身不负责对象序列化协议
- `SerializeHelper.*` 负责“对象表示”与文件持久化边界
- `CompressionHelper` 负责“压缩格式变换”与文件压缩边界

## 当前分层建议

- 纯字符串/字节/Base64 转换继续保持轻量 helper 形态
- 一旦触达文件系统，就优先使用 `WithResult(...)` 获取更稳定的诊断信息
- 若未来出现“压缩流到流”“压缩文件目录树”等新场景，优先新增更明确的边界 API，而不是继续堆叠到现有轻量入口上
