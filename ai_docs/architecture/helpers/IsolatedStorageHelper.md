# IsolatedStorageHelper

本文档用于分析 `Lanymy.Common.Helpers.IsolatedStorageHelper` 及其默认实现 `LanymyIsolatedStorage` 的当前职责与语义边界。

## 模块定位

- 这是一个“文件边界 + 加密载体”的 helper。
- 对外暴露的是轻量静态入口：
  - `SaveString(...)` / `GetString(...)`
  - `SaveModel<T>(...)` / `GetModel<T>(...)`
- 默认实现并不直接把明文写入磁盘，而是走：
  - 字符串/模型序列化
  - `SecurityHelper` 加密
  - `CompressionHelper` 压缩
  - 最终写入托管独立存储或自定义目录

## 当前实现结构

- `IsolatedStorageHelper`
  - 对外静态入口
  - 当前已补：
    - `SaveStringWithResult(...)`
    - `GetStringWithResult(...)`
    - `SaveModelWithResult<T>(...)`
    - `GetModelWithResult<T>(...)`
- `LanymyIsolatedStorage`
  - 默认 `IIsolatedStorage` 实现
  - 负责：
    - token 到真实存储文件名的映射
    - 托管独立存储 / 自定义目录模式切换
    - 加密压缩后的字节读写

## 严格层语义

本轮补充后，严格层结果对象会回填以下上下文：

- `Token`
- `StorageFileName`
- `FilePath`
- `IfUsesCustomIsolatedStorageMode`
- `StorageFileExists`
- `EncodingName`
- `UsedDefaultEncoding`
- `UsedDefaultSecurityKey`
- `StorageImplementationTypeName`
- `ErrorMessage`（用于内容级失败或异常文本回填）

其中模型结果还会补齐：

- `Model`
- `SerializedString`
- `ModelTypeName`
- `ModelTypeFullName`

## 当前锁定的关键边界

### 1. 缺失存储文件

- 兼容层：
  - `GetString(...)` 继续返回 `string.Empty`
  - `GetModel<T>(...)` 继续返回 `default(T)`
- 严格层：
  - 返回 `IsSuccess == false`
  - `Exception` 为 `FileNotFoundException`
  - 保留 token / 存储文件名 / 物理路径上下文

### 2. 错误密钥或损坏内容

- 兼容层：
  - 不额外包装为异常
  - 继续沿底层历史弱语义返回 `null`
- 严格层：
  - 返回 `IsSuccess == false`
  - `Exception == null`
  - `ErrorMessage = "解密失败,密钥或加密内容无效"`

### 3. 模型反序列化失败

- 兼容层：
  - 继续抛出反序列化异常
- 严格层：
  - `Exception` 保留具体反序列化异常
  - `ErrorMessage` 同步回填异常消息文本
  - 同时保留 `SerializedString`、文件存在状态与路径上下文

### 4. 模型序列化失败

- 兼容层：
  - `SaveModel(...)` 继续抛出序列化异常
- 严格层：
  - `SaveModelWithResult(...)` 不再因为结果对象初始化阶段的序列化失败而直接漏抛
  - 会返回：
    - `IsSuccess == false`
    - `Exception`
    - `ErrorMessage`
    - `Model` / `ModelTypeName` / `ModelTypeFullName`

### 5. 自定义独立存储目录

- 严格层会显式标记 `IfUsesCustomIsolatedStorageMode`
- 对使用文件系统落盘的路径，`FilePath` 会回填到结果对象

## 与 FileTextManipulater 的关系

- `IsolatedStorageHelper` 本身不直接依赖 `FileTextReader` / `FileTextWriter`
- 但它和 `JsonSerializeHelper` 一样，都属于“上层对外 helper，底层再落具体文件/流实现”的模式
- 因此本轮治理策略是：
  - `IsolatedStorageHelper` 补严格结果层
  - `FileTextManipulater` 保持轻量基础设施定位，只补边界测试，不新增结果层

## 维护时需要注意

- token 实际会被映射成哈希文件名，调用方不应假设持久化文件名与原 token 相同。
- 自定义目录模式与托管独立存储模式都属于既有能力，不能因为补严格层而删掉任一分支。
- 新代码若需要判断失败原因，优先使用 `WithResult(...)`，不要继续在兼容层追加布尔或空字符串判断。
