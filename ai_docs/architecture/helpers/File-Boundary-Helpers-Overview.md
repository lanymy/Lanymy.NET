# File Boundary Helpers Overview

本文档用于汇总当前仓库里直接触达文件系统、且已经明确采用“兼容层 + 严格层”治理模式的 helper。

## 文档目的

- 统一说明哪些 helper 属于“文件边界 helper”
- 给出各模块的职责边界与推荐调用方式
- 作为后续继续扫描新入口时的统一对照表

## 适用范围

当前纳入本总览的模块：

- `FileHelper`
- `CompressionHelper`
- `FileSerializeHelper`
- `JsonSerializeHelper`
- `IsolatedStorageHelper`
- `SecurityAesHelper`

说明：

- `SecurityHelper` 主体也包含大量文件相关入口，但大多已经返回 digest model，本轮不再把它归入“弱语义待治理”清单
- `SecurityHelper` / `LanymyCrypto` 的文件型输出入口现在也已和上述 helper 对齐：目标父目录缺失时会自动准备目录，但仍继续沿用既有 digest model，而不再额外补 `WithResult(...)`
- `BinarySerializeHelper`、`DataTableSerializeHelper` 仍是纯内存转换，不纳入本总览

## 模块分工

### 1. `FileHelper`

职责：

- 原始文件与目录操作
- 文件复制、移动、删除、批量复制、二进制文件写入

推荐使用：

- 新调用方优先 `CopyFileWithResult(...)`
- 新调用方优先 `MoveFileWithResult(...)`
- 新调用方优先 `DeleteFolderWithResult(...)`
- 新调用方优先 `CreateBinaryFileWithResult(...)`
- 新调用方优先 `GetBinaryFileBytesWithResult(...)`

适用场景：

- 需要拿到文件系统操作是否成功
- 需要保留异常与源/目标路径诊断信息

### 2. `CompressionHelper`

职责：

- 压缩格式变换
- 文件到文件压缩/解压

推荐使用：

- 纯内存转换继续使用轻量入口
- 文件边界优先：
  - `CompressSourceFileToCompressFileWithResult(...)`
  - `DecompressSourceFileFromCompressFileWithResult(...)`

适用场景：

- 文件压缩/解压失败需要诊断
- 需要区分“缺源文件”与“压缩算法失败”

### 3. `FileSerializeHelper`

职责：

- 对象 <-> 二进制文件 持久化
- 可选压缩

推荐使用：

- `SerializeToBytesFileWithResult(...)`
- `DeserializeFromBytesFileWithResult<T>(...)`

适用场景：

- 对象二进制落盘
- 二进制文件反序列化
- 需要知道文件路径、压缩开关、异常信息

### 4. `JsonSerializeHelper`

职责：

- 对象 <-> JSON 文本
- JSON 文件读写

推荐使用：

- 纯字符串转换继续使用轻量入口
- 文件边界优先：
  - `SerializeToJsonFileWithResult(...)`
  - `SerializeToJsonFileWithResultAsync(...)`
  - `DeserializeFromJsonFileWithResult<T>(...)`
  - `DeserializeFromJsonFileWithResultAsync<T>(...)`

适用场景：

- JSON 文件读写需要稳定诊断
- 需要把“缺失文件”与“JSON 解析失败”区分开

### 5. `SecurityAesHelper`

职责：

- AES 字节、字符串、模型加解密
- 模型文件加密与解密

推荐使用：

- 纯字符串/字节接口继续使用轻量入口
- 文件边界优先：
  - `EncryptModelToFileWithResult(...)`
  - `DecryptModelFromFileWithResult<T>(...)`

适用场景：

- AES 模型文件读写
- 需要把“缺文件”“解密失败”“反序列化失败”区分开
- 需要直接读取 `ModelTypeName` / `ModelTypeFullName` / `ErrorMessage` 这类结果上下文

### 6. `IsolatedStorageHelper`

职责：

- 字符串 / 模型的独立存储持久化
- 托管独立存储与自定义目录模式切换
- 持久化前的加密与压缩载体封装

推荐使用：

- `SaveStringWithResult(...)`
- `GetStringWithResult(...)`
- `SaveModelWithResult<T>(...)`
- `GetModelWithResult<T>(...)`

适用场景：

- 需要区分“文件缺失”“错误密钥”“模型反序列化失败”
- 需要拿到 token、存储文件名、自定义目录路径与默认编码/默认密钥使用情况

## 分层原则

统一建议如下：

1. 纯内存转换
   - 保持轻量 API
   - 失败沿异常通道暴露即可

2. 直接触达文件系统
   - 新调用方优先 `WithResult(...)`
   - 兼容层保留历史语义，不主动改变旧调用行为

3. helper-on-helper 组合封装
   - 若本身也承担文件边界，优先在当前 helper 层补严格结果语义
   - 不要求把所有下层依赖全部改成结果层后才允许新增严格入口

## 兼容层保留策略

当前总原则：

- 旧入口优先保留历史返回类型
- 严格层只为新调用方提供更清晰诊断
- 回归测试继续保留一部分旧入口调用，用于锁定历史语义

## 推荐排查顺序

后续继续扫描文件边界 helper 时，建议按下面顺序判断是否值得补严格层：

1. 是否直接触达文件系统
2. 当前是否只有 `bool / string / void / default(T)` 等弱语义
3. 失败后是否会吞异常、静默 no-op 或丢失诊断信息
4. 是否已有真实调用场景需要拿到失败原因
