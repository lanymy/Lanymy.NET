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
- AES 专用入口
  - `EncryptStringToString`
  - `DecryptStringFromString`
  - `EncryptModelToString`
  - `DecryptModelFromString`
- 位图 / 图片文件加解密
  - `EncryptStringToBitmap`
  - `DecryptStringFromBitmap`
  - `EncryptModelToImageFile`
  - `DecryptModelFromImageFile`
- 证书动态导入
  - `LoadCertificate(...)`
- RSA Blob 密钥生成与加解密
  - `CreateRsaKeyBlobBase64String`
  - `RsaEncryptStringToBase64String`
  - `RsaDecryptStringFromBase64String`
- MD5 摘要能力
  - `BytesToMD5`
  - `StringToMD5`

## 实现特征

- `SecurityHelper` 的核心设计是“Helper 负责统一入口，真实算法由 Instruments 承担”，通过 `GenericityHelper.GetInterface(...)` 选择默认实现或外部传入实现。
- 默认实现为 `DefaultLanymyCrypto = new LanymyCrypto()`，说明主流程仍围绕项目自带的自定义加密机制。
- `SecurityAesHelper` 则单独走 `IAesCrypto`，默认实现是 `LanymyAesCrypto`，更像是后期补进来的标准 AES 能力。
- 模块把“字符串、字节、文件、位图、模型”都纳入同一套家族接口，使用上统一，但职责面也偏宽。

## 维护时需要注意

- `SecurityHelper` 聚合范围很大，既有摘要、对称加密，也有 RSA、证书导入和图像隐藏式加密，后续扩展时要避免继续把无关安全能力都堆进同一个入口。
- 证书导入逻辑直接写入 `LocalMachine` 的 `Root` 与 `My` 存储，具备明显的环境权限要求，不适合被误用于普通应用流程。
- 若排查实际算法问题，不能只看 Helper，必须同步进入 `Lanymy.Common.Instruments.Crypto` 对应实现。
- `SecurityAesHelper` 中存在历史命名拼写 `Bteys`，文档应记录但不擅自修名，以免误导现有调用。

## 典型风险点

- 部分 RSA / 加解密入口在异常时直接吞掉错误并返回空结果，调试成本较高。
- 默认密钥与默认实现存在明显历史兼容属性，安全强度与现代安全基线未必等价。
- 图像加密、Bitmap 加解密这类能力兼容历史场景较强，但跨平台和依赖负担也更重。

## 后续建议

- 后续可拆出“摘要 / 对称加密 / 非对称加密 / 证书”四类能力边界图。
- 若项目进入现代化治理阶段，优先把 `SecurityHelper` 当成外观层，避免继续直接扩容其内部职责。
