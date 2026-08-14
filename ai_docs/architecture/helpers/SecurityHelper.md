# SecurityHelper

本文档用于分析 `Lanymy.Common.Helpers.SecurityHelper` 与 `SecurityAesHelper` 这一组安全辅助模块。

## 模块定位

- 面向调用层提供统一的“加解密 Helper 入口”。
- 本身不直接承载所有细节实现，而是把能力分发到 `LanymyCrypto`、`LanymyAesCrypto` 等 Instruments 实现上。
- 既保留传统字符串/字节/文件加解密，也扩展到了位图、图片文件、模型序列化、证书导入、RSA 密钥生成等历史能力。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.SecurityHelper/SecurityHelper.cs`
- `src/Commons/Lanymy.Common.Helpers.SecurityHelper/SecurityAesHelper.cs`

## 主要能力

- 统一加解密入口
  - `EncryptBytesToBytes`
  - `DecryptBytesFromBytes`
  - `EncryptStringToBase64String`
  - `DecryptStringFromBase64String`
  - `EncryptFileToFile`
  - `DecryptFileFromFile`
  - `EncryptBytesToFile`
  - `DecryptBytesFromFile`
  - `EncryptStringToFile`
  - `DecryptStringFromFile`
- AES 专用入口
  - `EncryptStringToString`
  - `DecryptStringFromString`
  - `EncryptModelToString`
  - `DecryptModelFromString`
  - `EncryptModelToFileWithResult`
  - `DecryptModelFromFileWithResult`
- 位图 / 图片文件加解密
  - `EncryptStringToBitmap`
  - `DecryptStringFromBitmap`
  - `EncryptBytesToImageFile`
  - `DecryptBytesFromImageFile`
  - `EncryptStringToImageFile`
  - `DecryptStringFromImageFile`
  - `EncryptModelToImageFile`
  - `DecryptModelFromImageFile`
- 证书动态导入
  - `LoadCertificate(...)`
  - `LoadCertificateWithResult(...)`
- RSA Blob 密钥生成与加解密
  - `CreateRsaKeyBlobBase64String`
  - `CreateRsaKeyBlobBase64StringWithResult`
  - `CreateRsaKeyBlobBytesWithResult`
  - `RsaEncryptStringToBase64String`
  - `RsaDecryptStringFromBase64String`
  - `RsaEncryptBytesToBytesWithResult`
  - `RsaDecryptBytesFromBytesWithResult`
  - `RsaEncryptStringToBase64StringWithResult`
  - `RsaDecryptStringFromBase64StringWithResult`
- MD5 摘要能力
  - `BytesToMD5`
  - `StringToMD5`

## 实现特征

- `SecurityHelper` 的核心设计是“Helper 负责统一入口，真实算法由 Instruments 承担”，通过 `GenericityHelper.GetInterface(...)` 选择默认实现或外部传入实现。
- 默认实现为 `DefaultLanymyCrypto = new LanymyCrypto()`，说明主流程仍围绕项目自带的自定义加密机制。
- `SecurityAesHelper` 则单独走 `IAesCrypto`，默认实现是 `LanymyAesCrypto`，更像是后期补进来的标准 AES 能力。
- `SecurityAesHelper` 的字符串/字节接口仍保持轻量风格；模型文件读写这组入口已经补了严格结果层，用于承接文件边界诊断。
- 模块把“字符串、字节、文件、位图、模型”都纳入同一套家族接口，使用上统一，但职责面也偏宽。

## 维护时需要注意

- `SecurityHelper` 聚合范围很大，既有摘要、对称加密，也有 RSA、证书导入和图像隐藏式加密，后续扩展时要避免继续把无关安全能力都堆进同一个入口。
- 证书导入逻辑直接写入 `LocalMachine` 的 `Root` 与 `My` 存储，具备明显的环境权限要求，不适合被误用于普通应用流程。
- `LoadCertificateWithResult(...)` 新增了 `storeLocation` 与可选目标 `StoreName` 参数，默认仍写入 `LocalMachine + Root/My`，但严格层与测试可以显式切到 `CurrentUser` 和非受信任根存储做非管理员诊断与回归验证。
- 若排查实际算法问题，不能只看 Helper，必须同步进入 `Lanymy.Common.Instruments.Crypto` 对应实现。
- `SecurityAesHelper` 中存在历史命名拼写 `Bteys`，文档应记录但不擅自修名，以免误导现有调用。

## 典型风险点

- 部分 RSA / 加解密入口在异常时直接吞掉错误并返回空结果，调试成本较高。
- 默认密钥与默认实现存在明显历史兼容属性，安全强度与现代安全基线未必等价。
- 图像加密、Bitmap 加解密这类能力兼容历史场景较强，但跨平台和依赖负担也更重。
- 图片文件入口底层依赖 `ImageHelper.SaveBitmapToImageFile(...)`；当前已明确要求保存失败必须把 digest model 置为失败，而不能继续误报成功。

## 文件 / 图片边界结果语义

`SecurityHelper` 的文件与图片边界入口当前**继续沿用现有 digest model**，本轮未额外补 `WithResult(...)`，原因是：

- 现有返回对象已经提供 `IsSuccess`
- 已包含文件路径、源字符串、模型类型、哈希摘要等上下文
- 再叠一层 `WithResult(...)` 会形成重复抽象

本轮额外收紧的行为：

- `EncryptBytesToImageFile(...)` / `EncryptStringToImageFile(...)` / `EncryptModelToImageFile(...)`
  - 图片保存失败时，必须返回 `IsSuccess == false`
  - 同时通过 `ErrorMessage` 暴露失败信息
- `EncryptBytesToFile(...)` / `EncryptStringToFile(...)`
  - 成功时会回填 `SourceBytes` / `SourceString`
- `EncryptBytesToFile(...)` / `EncryptStringToFile(...)` / `EncryptFileToFile(...)` / `DecryptFileFromFile(...)`
  - 文件型输出目标现在会自动准备父目录，和仓库内其他文件边界入口保持一致
- `DecryptBytesFromFile(...)` / `DecryptStringFromFile(...)`
  - 成功时会回填 `SourceBytes` / `SourceString`
- `DecryptStringFromBitmap(...)`
  - 直接调用且未传 `encoding` 时，会回退到默认编码而不是抛 `NullReferenceException`
- `DecryptModelFromBytes(...)` / `DecryptModelFromBase64String(...)` / `DecryptModelFromFile(...)` / `DecryptModelFromBitmap(...)` / `DecryptModelFromImageFile(...)`
  - 成功时会回填 `ModelTypeName` / `ModelTypeFullName`
- `DecryptModelFromBase64String(...)`
  - 成功时会保留输入的 `EncryptedBase64String`
- `GetEncryptDigestInfoModelFromEncryptedStream(...)`
  - 篡改正文内容时会返回 `IsSuccess == false`，并通过 `ErrorMessage` 暴露校验失败
  - 截断或结构损坏的加密流会返回 `IsSuccess == false` 与结构无效错误，而不是直接抛异常
- `GetEncryptDigestInfoModelFromEncryptedFile(...)`
  - 对文件型 digest model 调用，无论成功或失败，都会保留输入的 `EncryptedFileFullPath`
  - 默认通用 `EncryptDigestInfoModel` 仍保持轻量，不额外承载文件路径属性
- `DencryptStreamFromStream(...)` 及其上层 `Decrypt*` 组合入口
  - 错误密钥或损坏密文时会返回失败 digest，并通过 `ErrorMessage` 暴露“解密失败,密钥或加密内容无效”
  - 对可定位的目标流，失败后会清空已写入内容，避免保留半截解密结果
- `EncryptStreamToStream(...)` / `GetEncryptDigestInfoModelFromEncryptedStream(...)`
  - 成功路径下，运行时摘要字段与 `DencryptHeaderInfoModelJsonString` 中的值保持一致
- `DecryptStringFromBase64String(...)` / `DecryptModelFromBase64String(...)`
  - 非法 Base64 输入会返回失败 digest，并保留原始 `EncryptedBase64String`
- `DecryptModelFromBytes(...)` / `DecryptModelFromFile(...)` / `DecryptModelFromBitmap(...)` / `DecryptModelFromImageFile(...)`
  - 解密成功但模型 JSON 无法反序列化时，会返回失败 digest 与“模型反序列化失败,无法继续解析”
- `EncryptModelToBitmap(...)` / `DecryptModelFromBitmap(...)`
  - 成功路径已通过专项测试锁定 `SourceModel` 与模型类型元数据回填
- `DecryptStringFromBitmap(...)` / `DecryptStringFromImageFile(...)`
  - 非法位图内容、非法图片文件内容会返回失败 digest，而不是直接抛出内容级异常

## AES 文件边界结果语义

`SecurityAesHelper` 已补：

- `EncryptModelToFileWithResult(...)`
- `DecryptModelFromFileWithResult<T>(...)`

对应结果模型：

- `SecurityAesFileOperationResultModel<T>`

当前严格层额外暴露：

- `FilePath`
- `TargetFileExists`
- `Model`
- `ModelTypeName`
- `ModelTypeFullName`
- `ErrorMessage`

兼容层仍保留原语义：

- `EncryptModelToFile(...)` 失败时继续抛异常
- `DecryptModelFromFile<T>(...)` 失败时继续抛异常

## RSA 结果语义

`SecurityHelper` 的 RSA 入口已补：

- `CreateRsaKeyBlobBase64StringWithResult(...)`
- `CreateRsaKeyBlobBytesWithResult(...)`
- `RsaEncryptBytesToBytesWithResult(...)`
- `RsaDecryptBytesFromBytesWithResult(...)`
- `RsaEncryptStringToBase64StringWithResult(...)`
- `RsaDecryptStringFromBase64StringWithResult(...)`

对应结果模型：

- `SecurityRsaKeyBlobBytesResultModel`
- `SecurityRsaKeyBlobStringResultModel`
- `SecurityRsaBytesOperationResultModel`
- `SecurityRsaStringOperationResultModel`

兼容层仍保留原语义：

- `CreateRsaKeyBlobBase64String(...)` / `CreateRsaKeyBlobBytes(...)` 失败时继续直接抛异常
- `RsaEncryptBytesToBytes(...)` / `RsaDecryptBytesFromBytes(...)` 失败时继续返回 `null`
- `RsaEncryptStringToBase64String(...)` / `RsaDecryptStringFromBase64String(...)` 失败时继续返回空字符串

## 证书加载结果语义

`SecurityHelper` 的证书导入入口已补：

- `LoadCertificateWithResult(byte[] rawData, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)`
- `LoadCertificateWithResult(byte[] rawData, string password, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)`
- `LoadCertificateWithResult(string certFileFullPath, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)`
- `LoadCertificateWithResult(string certFileFullPath, string password, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)`
- `LoadCertificateWithResult(X509Certificate2 certificate, StoreLocation storeLocation = StoreLocation.LocalMachine, StoreName trustedStoreName = StoreName.Root, StoreName personalStoreName = StoreName.My)`

对应结果模型：

- `SecurityCertificateLoadResultModel`

兼容层仍保留原语义：

- `LoadCertificate(...)` 默认继续写入 `LocalMachine`
- 缺失文件、密码错误、证书格式错误、权限不足时继续直接抛异常

严格层额外暴露的上下文包括：

- `FilePath`
- `StoreLocation`
- `TrustedStoreName` / `PersonalStoreName`
- `CertificateThumbprint`
- `CertificateSubject`
- `AddedToTrustedStore` / `AddedToPersonalStore`
- `ExistsInTrustedStore` / `ExistsInPersonalStore`

## 后续建议

- 后续可拆出“摘要 / 对称加密 / 非对称加密 / 证书”四类能力边界图。
- 若项目进入现代化治理阶段，优先把 `SecurityHelper` 当成外观层，避免继续直接扩容其内部职责。
