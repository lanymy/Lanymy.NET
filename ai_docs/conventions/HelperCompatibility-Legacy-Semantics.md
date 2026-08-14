# Helper Compatibility Legacy Semantics

本文档用于汇总当前已经完成“兼容层 / 严格层”改造的 Helper 模块中，兼容层仍然保留的历史语义。

本文件不讨论“推荐新代码如何调用”，那部分规则见 [HelperResult-Semantics-Convention.md](./HelperResult-Semantics-Convention.md)。
本文件只回答一个问题：

> 老入口今天依然保留了哪些历史行为，后续继续改造时哪些不能被悄悄带偏？

## 1. 总原则

- 兼容层的职责是承接旧调用点，不是追求语义最完美。
- 兼容层若已保留历史行为，后续修改时应优先通过测试锁定，而不是顺手“优化掉”。
- 若某个旧入口的历史语义已经不利于诊断，新代码应优先改用对应 `WithResult(...)` 入口，而不是继续扩展兼容层。

## 2. CompressionHelper

### 2.1 `CompressSourceFileToCompressFile(...)` / `CompressSourceFileToCompressFileAsync(...)`

- 当前兼容语义：
  - 同步入口返回 `void`
  - 异步入口返回 `Task`
  - 源文件不存在时静默 no-op
  - 不因为“源文件不存在”直接抛异常
- 对应严格层：
  - `CompressSourceFileToCompressFileWithResult(...)`
  - `CompressSourceFileToCompressFileWithResultAsync(...)`

### 2.2 `DecompressSourceFileFromCompressFile(...)` / `DecompressSourceFileFromCompressFileAsync(...)`

- 当前兼容语义：
  - 同步入口返回 `void`
  - 异步入口返回 `Task`
  - 压缩文件不存在时静默 no-op
  - 不因为“压缩文件不存在”直接抛异常
- 对应严格层：
  - `DecompressSourceFileFromCompressFileWithResult(...)`
  - `DecompressSourceFileFromCompressFileWithResultAsync(...)`

## 3. FileHelper

### 3.1 `MoveFile(...)`

- 历史兼容语义：
  - 源文件不存在时静默无操作
  - 不抛异常
- 对应严格层：
  - `MoveFileWithResult(...)`
  - 源文件不存在时返回失败结果，并携带 `FileNotFoundException`

### 3.2 `CopyFolderToNewFoler(...)`

- 历史兼容语义：
  - 调用后不对外抛异常
  - 失败信息不会暴露给调用方
- 对应严格层：
  - `CopyFolderToNewFolerWithResult(...)`

### 3.3 `CopyFiles(...)`

- 历史兼容语义：
  - 返回 `void`
  - 输入集合为空时直接抛 `ArgumentNullException`
  - 任一子项失败时沿用单项兼容层语义继续抛异常
  - 首项失败时不会继续执行后续子项
- 对应严格层：
  - `CopyFilesWithResult(...)`
  - 会分别表达批次级异常、请求项数量与逐项结果

### 3.4 `DeleteFolder(...)`

- 历史兼容语义：
  - 返回 `bool`
  - 删除失败时只返回 `false`
- 对应严格层：
  - `DeleteFolderWithResult(...)`

### 3.5 `CreateBinaryFile(...)`

- 历史兼容语义：
  - `bytes` 为空数组时直接 no-op
  - `bytes == null` 时也直接 no-op
  - 不创建空文件
- 对应严格层：
  - `CreateBinaryFileWithResult(...)`
  - `bytes == null` 时返回带异常的失败结果
  - 空数组时视为合法空文件写入

## 4. ProcessHelper

### 4.1 `StartProcess(...)`

- 历史兼容语义：
  - 返回 `bool`
  - 启动失败时只返回 `false`
- 对应严格层：
  - `StartProcessWithResult(...)`
  - 可获取 `ProcessId`、`HasExited`、`ExitCode`、`Exception`

### 4.2 `RunProcess(...)`

- 历史兼容语义：
  - 返回 `bool`
  - 非零退出码按失败处理
- 对应严格层：
  - `RunProcessWithResult(...)`

## 5. EmailHelper

### 5.1 `SendEmail(...)`

- 当前兼容语义：
  - 仍保留旧入口名
  - 返回类型保持为 `CommonResultModel`
  - 调用失败不直接抛异常，而是通过结果对象暴露 `IsSuccess` 与 `Exception`
- 对应严格层：
  - `SendEmailWithResult(...)`
  - 可拿到逐收件人结果、成功数、失败数、首个异常，以及本次请求的收件人列表和发送模式

说明：

- `EmailHelper` 的兼容层不是 `bool`，而是历史上就已经存在的轻量结果对象风格。
- 这里保留的是“旧入口名 + 旧返回抽象层级”，不是 `false` 语义。
- 即使是非拆分批量发送失败，严格层也应尽量把异常映射回逐收件人结果，避免再次退回“只有整体失败”的弱诊断。

## 6. FileSerializeHelper

### 6.1 `SerializeToBytesFile(...)`

- 当前兼容语义：
  - 返回 `void`
  - 底层写入失败时直接抛异常
- 对应严格层：
  - `SerializeToBytesFileWithResult(...)`

### 6.2 `DeserializeFromBytesFile(...)`

- 当前兼容语义：
  - 返回反序列化对象
  - 文件缺失、解压失败、反序列化失败时直接抛异常
- 对应严格层：
  - `DeserializeFromBytesFileWithResult(...)`

## 7. JsonSerializeHelper

### 7.1 `SerializeToJsonFile(...)` / `SerializeToJsonFileAsync(...)`

- 当前兼容语义：
  - 同步入口返回 `void`
  - 异步入口返回 `Task`
  - 底层文件写入失败时直接抛异常
- 对应严格层：
  - `SerializeToJsonFileWithResult(...)`
  - `SerializeToJsonFileWithResultAsync(...)`

### 7.2 `DeserializeFromJsonFile(...)` / `DeserializeFromJsonFileAsync(...)`

- 当前兼容语义：
  - 同步入口返回反序列化对象
  - 异步入口返回 `Task<T>`
  - 缺失文件时沿底层文本读取器创建空文件
  - 空文件最终返回 `default(T)`
  - 不会因为“文件不存在”直接抛异常
- 对应严格层：
  - `DeserializeFromJsonFileWithResult(...)`
  - `DeserializeFromJsonFileWithResultAsync(...)`

## 8. NetworkHelper

### 8.1 `PingIP(...)`

- 历史兼容语义：
  - 返回 `bool`
  - 参数非法、Ping 失败、网络异常都统一折叠为 `false`
- 对应严格层：
  - `PingIPWithResult(...)`

### 8.2 `GetIpAddressByIpString(...)`

- 历史兼容语义：
  - 非法输入直接抛异常
- 对应严格层：
  - `GetIpAddressByIpStringWithResult(...)`

### 8.3 `GetLocalIpList()` / `GetLocalIpV4List()` / `GetLocalIpV6List()`

- 历史兼容语义：
  - 查询异常直接抛异常
- 对应严格层：
  - `GetLocalIpListWithResult()`
  - `GetLocalIpV4ListWithResult()`
  - `GetLocalIpV6ListWithResult()`

### 8.4 `GetLocalIP()`

- 历史兼容语义：
  - 查询成功但没有首个 IPv4 地址时返回空字符串
- 对应严格层：
  - `GetLocalIPWithResult()`

## 9. PcInfoHelper

### 9.1 `CreateBootAutoRun(...)` / `CreateDesktopShortcut(...)` / `CreateShortcut(...)`

- 历史兼容语义：
  - 返回 `bool`
  - 失败时只返回 `false`
- 对应严格层：
  - `CreateBootAutoRunWithResult(...)`
  - `CreateDesktopShortcutWithResult(...)`
  - `CreateShortcutWithResult(...)`

### 9.2 `GetHostName()`

- 历史兼容语义：
  - 查询异常直接抛异常
- 对应严格层：
  - `GetHostNameWithResult()`

### 9.3 `GetIPV4()` / `GetIPV6()`

- 历史兼容语义：
  - 查询异常直接抛异常
  - 查询成功但没有对应地址时返回空字符串
- 对应严格层：
  - `GetIPV4WithResult()`
  - `GetIPV6WithResult()`

### 9.4 `GetLocalIpAddress()`

- 历史兼容语义：
  - 查询异常直接抛异常
  - 找不到“已启用、带网关、非回环”的 IPv4 地址时返回空字符串
- 对应严格层：
  - `GetLocalIpAddressWithResult()`

## 10. SecurityAesHelper

### 10.1 `EncryptModelToFile(...)`

- 当前兼容语义：
  - 返回 `void`
  - 写入失败时直接抛异常
- 对应严格层：
  - `EncryptModelToFileWithResult(...)`

### 10.2 `DecryptModelFromFile<T>(...)`

- 当前兼容语义：
  - 返回解密后的模型
  - 文件不存在、解密失败、反序列化失败时直接抛异常
- 对应严格层：
  - `DecryptModelFromFileWithResult<T>(...)`

## 11. SecurityHelper（RSA）

### 11.1 `CreateRsaKeyBlobBase64String(...)` / `CreateRsaKeyBlobBytes(...)`

- 当前兼容语义：
  - 继续使用 `out` 参数返回公钥 / 私钥
  - RSA Provider 初始化失败时直接抛异常
- 对应严格层：
  - `CreateRsaKeyBlobBase64StringWithResult(...)`
  - `CreateRsaKeyBlobBytesWithResult(...)`

### 11.2 `RsaEncryptBytesToBytes(...)` / `RsaDecryptBytesFromBytes(...)`

- 当前兼容语义：
  - 返回 `byte[]`
  - RSA 参数非法、密钥不匹配、加解密失败时返回 `null`
  - 不直接抛异常
- 对应严格层：
  - `RsaEncryptBytesToBytesWithResult(...)`
  - `RsaDecryptBytesFromBytesWithResult(...)`

### 11.3 `RsaEncryptStringToBase64String(...)` / `RsaDecryptStringFromBase64String(...)`

- 当前兼容语义：
  - 返回 `string`
  - Base64 非法、密钥不匹配、加解密失败时返回空字符串
  - 不直接抛异常
- 对应严格层：
  - `RsaEncryptStringToBase64StringWithResult(...)`
  - `RsaDecryptStringFromBase64StringWithResult(...)`

## 12. SecurityHelper（证书）

### 12.1 `LoadCertificate(...)`

- 当前兼容语义：
  - 返回 `void`
  - 默认继续写入 `LocalMachine` 的 `Root` 与 `My` 存储
  - 缺失文件、密码错误、证书格式错误、权限不足时直接抛异常
- 对应严格层：
  - `LoadCertificateWithResult(...)`
  - 可获取 `FilePath`、`StoreLocation`、目标 `StoreName`、证书主题/指纹，以及目标存储是否已存在或是否新增

## 13. SecurityHelper（文件 / 图片 digest model）

### 13.1 `EncryptBytesToFile(...)` / `DecryptBytesFromFile(...)` / `EncryptStringToFile(...)` / `DecryptStringFromFile(...)`

- 当前兼容语义：
  - 继续直接返回 digest model
  - 参数非法、文件不存在、底层流操作失败时继续直接抛异常
  - 成功路径通过 `IsSuccess`、文件路径和哈希摘要暴露结果
  - 其中字符串/字节组合入口成功后还会回填 `SourceString` / `SourceBytes`
- 对应严格层：
  - 当前**不额外新增 `WithResult(...)`**
  - 原因是现有 digest model 已承担结果层职责

### 13.2 `EncryptBytesToImageFile(...)` / `EncryptStringToImageFile(...)` / `EncryptModelToImageFile(...)`

- 当前兼容语义：
  - 继续直接返回 digest model
  - 图片保存失败时返回 `IsSuccess == false`，并通过 `ErrorMessage` 暴露失败信息
  - 不额外抛异常来替代已有结果对象语义
- 对应严格层：
  - 当前**不额外新增 `WithResult(...)`**
  - 原因是图片入口已直接返回 digest model，继续叠加会形成重复抽象

### 13.3 `DecryptStringFromBitmap(...)`

- 当前兼容语义：
  - 继续直接返回 digest model
  - 未显式传入 `encoding` 时，自动回退默认编码
  - 错误密钥或密文损坏时返回 `IsSuccess == false` 与 `ErrorMessage`
- 对应严格层：
  - 当前**不额外新增 `WithResult(...)`**

### 13.4 `DecryptModelFromBytes(...)` / `DecryptModelFromBase64String(...)` / `DecryptModelFromFile(...)` / `DecryptModelFromBitmap(...)` / `DecryptModelFromImageFile(...)`

- 当前兼容语义：
  - 继续直接返回 digest model
  - 成功时回填 `SourceModel`、`ModelTypeName`、`ModelTypeFullName`
  - `DecryptModelFromBase64String(...)` 额外保留输入 `EncryptedBase64String`
  - 内容损坏或解密失败时返回 `IsSuccess == false` 与 `ErrorMessage`
- 对应严格层：
  - 当前**不额外新增 `WithResult(...)`**

### 13.5 `GetEncryptDigestInfoModelFromEncryptedStream(...)` / `GetEncryptDigestInfoModelFromEncryptedFile(...)`

- 当前兼容语义：
  - 继续直接返回 digest model
  - 正文校验失败时返回 `IsSuccess == false` 和明确 `ErrorMessage`
  - 结构截断或内容损坏时同样返回失败 digest，而不是直接抛异常
  - 文件型 digest model 会在成功与失败两侧都保留 `EncryptedFileFullPath`
  - 默认通用 digest 仍不暴露文件路径属性
- 对应严格层：
  - 当前**不额外新增 `WithResult(...)`**

### 13.6 `DecryptStringFromBase64String(...)` / `DecryptModelFromBase64String(...)`

- 当前兼容语义：
  - 继续直接返回 digest model
  - 非法 Base64 内容返回 `IsSuccess == false` 与 `ErrorMessage`
  - 失败时仍保留原始 `EncryptedBase64String`
- 对应严格层：
  - 当前**不额外新增 `WithResult(...)`**

### 13.7 `DecryptModelFromBytes(...)` / `DecryptModelFromFile(...)` / `DecryptModelFromBitmap(...)` / `DecryptModelFromImageFile(...)`

- 当前兼容语义：
  - 继续直接返回 digest model
  - 模型 JSON 反序列化失败时返回 `IsSuccess == false` 与 `ErrorMessage`
  - 成功时继续回填 `SourceModel` 与模型类型元数据
- 对应严格层：
  - 当前**不额外新增 `WithResult(...)`**

### 13.8 `DencryptStreamFromStream(...)`

- 当前兼容语义：
  - 继续直接返回 digest model
  - 内容级失败返回 `IsSuccess == false` 与 `ErrorMessage`
  - 当目标流支持定位时，失败后会清空已写入内容
  - 成功路径下，运行时摘要字段与头部 JSON 摘要保持一致
- 对应严格层：
  - 当前**不额外新增 `WithResult(...)`**

## 14. IsolatedStorageHelper

### 14.1 `GetString(...)`

- 当前兼容语义：
  - 存储文件缺失时返回 `string.Empty`
  - 错误密钥或内容损坏时继续沿底层历史弱语义返回 `null`
  - 不把“缺文件”统一提升为异常
- 对应严格层：
  - `GetStringWithResult(...)`
  - 缺文件时返回 `FileNotFoundException`
  - 内容级失败通过 `IsSuccess == false` 与 `ErrorMessage` 暴露

### 14.2 `GetModel<T>(...)`

- 当前兼容语义：
  - 存储文件缺失时返回 `default(T)`
  - 模型反序列化失败时继续抛异常
- 对应严格层：
  - `GetModelWithResult<T>(...)`
  - 可补齐 token、存储文件名、文件路径、模型 JSON 文本与异常

### 14.3 `SaveString(...)` / `SaveModel<T>(...)`

- 当前兼容语义：
  - 返回 `void`
  - 写入失败时继续直接抛异常
- 对应严格层：
  - `SaveStringWithResult(...)`
  - `SaveModelWithResult<T>(...)`

## 15. 维护建议

- 新增测试时，兼容层断言应直接对照本文件，不要凭印象写“更合理”的新行为。
- 若后续决定主动废弃某个兼容层历史语义，应先更新本文件，再调整实现和测试，避免文档与代码脱节。
- 后续继续扩展 `WithResult(...)` 模式时，若发现新的兼容层历史行为，应及时补入本文件。
