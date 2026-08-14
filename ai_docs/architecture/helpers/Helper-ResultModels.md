# Helper Result Models

本文档用于梳理当前 `Helpers` 体系下已经引入的结果模型，并对字段命名、职责边界和适用场景做一次轻量归类。

## 1. 当前范围

本轮已落地结果模型的模块包括：

- `CompressionHelper`
- `FileHelper`
- `FileSerializeHelper`
- `JsonSerializeHelper`
- `ProcessHelper`
- `EmailHelper`
- `NetworkHelper`
- `IsolatedStorageHelper`
- `PcInfoHelper`
- `SecurityAesHelper`
- `SecurityHelper`

对应源码目录集中在：

- `src/Commons/Lanymy.Common.Helpers.CompressionHelper/ResultModels/`
- `src/Commons/Lanymy.Common.Helpers.FileHelper/ResultModels/`
- `src/Commons/Lanymy.Common.Helpers.SerializeHelper.File/ResultModels/`
- `src/Commons/Lanymy.Common.Helpers.SerializeHelper.Json/ResultModels/`
- `src/Commons/Lanymy.Common.Helpers.ProcessHelper/ResultModels/`
- `src/Commons/Lanymy.Common.Helpers.EmailHelper/ResultModels/`
- `src/Commons/Lanymy.Common.Helpers.NetworkHelper/ResultModels/`
- `src/Commons/Lanymy.Common.Helpers.IsolatedStorageHelper/ResultModels/`
- `src/Commons/Lanymy.Common.Helpers.PcInfoHelper/ResultModels/`
- `src/Commons/Lanymy.Common.Helpers.SecurityHelper/ResultModels/`

## 2. 归类方式

### 2.1 简单操作结果模型

适用于“单次外部操作，对应一个明确成功/失败结果”的场景。

代表模型：

- `CompressionFileOperationResultModel`
- `FileOperationResultModel`
- `BinaryFileSerializationResultModel<T>`
- `JsonFileSerializationResultModel<T>`
- `SecurityAesFileOperationResultModel<T>`
- `IsolatedStorageStringOperationResultModel`
- `IsolatedStorageModelOperationResultModel<T>`
- `SecurityCertificateLoadResultModel`
- `SecurityRsaKeyBlobBytesResultModel`
- `SecurityRsaKeyBlobStringResultModel`
- `SecurityRsaBytesOperationResultModel`
- `SecurityRsaStringOperationResultModel`
- `ShortcutOperationResultModel`
- `HostNameQueryResultModel`
- `EmailRecipientSendResultModel`

共同特征：

- 都直接暴露 `IsSuccess`
- 都直接暴露 `Exception`
- 额外再补少量最关键的上下文字段
- 若结果对象本身承载排障职责，优先同步补 `ErrorMessage`，避免调用方只能从 `Exception` 提取文本

适用场景：

- 文件到文件压缩/解压
- 文件复制/移动/创建
- 对象与二进制文件之间的序列化/反序列化
- 对象与 JSON 文件之间的序列化/反序列化
- AES 模型文件加密/解密
- 独立存储字符串与模型读写
- 证书导入
- RSA 密钥创建
- RSA 二进制与字符串加解密
- 快捷方式创建
- 单个收件人发送
- 单值主机名查询

### 2.2 查询型结果模型

适用于“查询成功并不等于一定找到目标值”的场景。

代表模型：

- `PingResultModel`
- `IpAddressParseResultModel`
- `LocalIpListResultModel`
- `LocalIpAddressResultModel`
- `LocalNetworkAddressResultModel`

共同特征：

- `IsSuccess` 表示“查询或解析已命中有效结果”
- `Exception` 表示“执行过程出错”
- 当 `IsSuccess == false` 且 `Exception == null` 时，通常表示“执行完成，但没有找到合适结果”

推荐字段：

- `Address`
- `AddressText`
- `Addresses` / `CandidateAddresses`
- `Status`
- `AddressFamily`

### 2.3 进程/状态机型结果模型

适用于“成功语义由多个状态字段共同决定”的场景。

代表模型：

- `ProcessResultModel`

共同特征：

- 不只看 `Exception`
- `IsSuccess` 由 `IsStarted / HasExited / ExitCode` 联合决定
- 更强调“过程状态”和“最终状态”的同时表达
- 同时建议回填关键请求上下文，例如：
  - `CreateNoWindow`
  - `UseShellExecute`
  - `WaitedForExit`
  - `ErrorMessage`

### 2.4 聚合型结果模型

适用于“一次操作包含多项子操作，且可能部分成功、部分失败”的场景。

代表模型：

- `BatchFileOperationResultModel`
- `EmailSendResultModel`

共同特征：

- 内部包含子项结果集合
- 对外提供 `SuccessCount / FailureCount`
- `IsSuccess` 表示“是否整体成功”
- 允许同时携带“请求级上下文”和“逐子项诊断”，例如：
  - `RequestedRecipientAddresses`
  - `IsUnpackSendMail`
  - `BodyEncodingName`
- 对文件类批量操作，还适合补：
  - `RequestedItemCount`
  - `Exception`
  - `FirstException`
  用来区分“批次参数本身无效”和“批次中的某个子项失败”
- 若兼容层仍保留“首项失败立即中断”的历史行为，严格层文档和测试应明确哪些子项已经执行、哪些没有执行，避免把兼容层中断语义误读成“全部项都已尝试”。

## 3. 当前字段命名约定

### 3.1 通用字段

以下字段已基本形成事实标准：

- `IsSuccess`
- `Exception`

新增结果模型时，若场景允许，优先保留这两个字段名，不再发散成：

- `Success`
- `HasError`
- `Error`
- `ErrorMessage`

### 3.2 地址相关字段

网络与本机信息场景统一优先使用：

- `Address`
- `AddressText`
- `Addresses`
- `CandidateAddresses`
- `AddressFamily`

约定说明：

- `Address` 表示最终选中的单值地址对象
- `AddressText` 表示该地址的字符串表达
- `Addresses` 表示通用地址集合
- `CandidateAddresses` 表示“筛选过程中的候选集”，强调不是所有原始输入

### 3.3 路径相关字段

文件与快捷方式场景统一优先使用：

- `FilePath`
- `SourcePath`
- `TargetPath`

### 3.4 聚合统计字段

批量场景统一优先使用：

- `Results` / `RecipientResults`
- `SuccessCount`
- `FailureCount`

### 3.5 安全上下文字段

证书与安全文件场景优先补充：

- `StoreLocation`
- `TrustedStoreName`
- `PersonalStoreName`
- `CertificateThumbprint`
- `CertificateSubject`
- `AddedToTrustedStore` / `AddedToPersonalStore`
- `ExistsInTrustedStore` / `ExistsInPersonalStore`

### 3.6 独立存储上下文字段

独立存储场景优先补充：

- `Token`
- `StorageFileName`
- `FilePath`
- `IfUsesCustomIsolatedStorageMode`
- `StorageFileExists`
- `EncodingName`
- `UsedDefaultEncoding`
- `UsedDefaultSecurityKey`
- `StorageImplementationTypeName`

## 4. 当前一致性结论

这轮复核后，当前结果模型体系已经基本满足以下一致性：

- 外部操作型模型普遍使用 `IsSuccess + Exception`
- 网络查询型模型已统一补齐 `Address` / `AddressText` 一类上下文字段
- 批量场景已统一采用聚合结果，而不是再退回单一布尔值
- `SecurityHelper` 中历史存在的 digest model 继续作为结果对象使用；对于文件 / 图片边界，优先补成功判定与测试覆盖，而不是机械再加一层 `WithResult(...)`
- 组合入口若沿用 digest model，则成功后必须回填该模型语义上承诺的关键字段，例如 `SourceBytes`、`SourceString`、文件路径和模型类型
- 对于带输入载体语义的结果模型，还应保留调用侧传入的重要上下文，例如 `EncryptedBase64String`
- 对于 digest model 覆盖的解密失败路径，优先通过 `IsSuccess + ErrorMessage` 暴露失败，而不是把结构损坏、密钥错误这类内容级异常直接抛出
- 即使解密失败，也应尽量保留原始输入载体字段，例如 `EncryptedBase64String`、`EncryptedFileFullPath`
- 对文件型 digest model，优先把文件路径在成功和失败两侧都回填，避免文件级诊断信息只存在于成功路径
- 通用 `EncryptDigestInfoModel` 保持摘要职责，不额外挂载文件路径等载体特有字段；这类上下文应放在更具体的派生模型中

当前保留的“有意差异”主要有两类：

1. `ProcessResultModel`
   - `IsSuccess` 为计算属性，因为它依赖 `ExitCode` 和生命周期状态

2. `LocalIpListResultModel` 与 `LocalNetworkAddressResultModel`
   - 前者强调“查询结果全集”
   - 后者强调“筛选后的候选与最终选中值”

3. `IsolatedStorageHelper` 结果模型
   - 同时承载“文件边界失败”和“内容级失败”
   - 因此会同时出现：
     - `Exception` 用于缺文件、反序列化异常等系统级失败
     - `ErrorMessage` 用于错误密钥、密文损坏等内容级失败

这两类差异都属于场景差异，不属于命名漂移。

## 5. 后续新增结果模型时的推荐顺序

新增一个 `WithResult(...)` 入口时，建议按下面顺序决定模型形态：

1. 先判断它是单次操作、查询、进程状态，还是批量聚合
2. 优先复用现有字段命名
3. 如果成功语义由多个状态共同决定，再考虑计算型 `IsSuccess`
4. 如果存在部分成功，优先设计聚合结果模型
5. 除非场景特别强，否则不要引入单独错误码字段替代异常

## 6. 配套文档

- 结果语义规则见 [../conventions/HelperResult-Semantics-Convention.md](../../conventions/HelperResult-Semantics-Convention.md)
- 兼容层历史语义见 [../../conventions/HelperCompatibility-Legacy-Semantics.md](../../conventions/HelperCompatibility-Legacy-Semantics.md)
