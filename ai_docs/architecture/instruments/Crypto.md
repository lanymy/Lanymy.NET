# Crypto

本文档用于分析 `Lanymy.Common.Instruments.Crypto` 模块的实现特点。

## 模块定位

- 这是仓库内“真实加解密实现”的核心模块之一。
- `SecurityHelper` 更多是调用入口，这里才是主要算法与数据格式规则所在。
- 当前模块同时承载了传统自定义加密、AES 能力，以及与弱密钥兼容相关的扩展。

## 当前代码入口

- `src/Commons/Lanymy.Common.Instruments.Crypto/LanymyCrypto.cs`
- `src/Commons/Lanymy.Common.Instruments.Crypto/LanymyAesCrypto.cs`
- `src/Commons/Lanymy.Common.Instruments.Crypto/DESCryptoExtensions.cs`

## 子模块职责

- `LanymyCrypto`
  - 项目自定义加密实现。
  - 覆盖字节、字符串、Base64、文件、模型、位图、图片文件等多类载体。
- `LanymyAesCrypto`
  - 标准 AES CBC 能力实现。
  - 更聚焦于字节、字符串、模型、文件等常见场景。
- `DESCryptoExtensions`
  - 通过反射调用内部方法，绕过 `TripleDESCryptoServiceProvider` 的弱密钥检查。
  - 这是明显的历史兼容补丁位。

## LanymyCrypto 的关键特征

- 使用 `TripleDESCryptoServiceProvider + GZipStream + 自定义头摘要` 组合。
- 数据格式中包含：
  - 是否随机加密标识
  - 可选随机时间戳标记
  - 头摘要 JSON 压缩数据
  - 正文长度
  - 正文哈希值
  - 加密正文流
- 借助 `JsonSerializeHelper` 把摘要信息模型序列化进头部，说明其协议不是纯算法层，而是带元数据的完整封装格式。
- 同时支持把加密结果映射进 `Bitmap` / 图片文件，这是本仓库很有历史特色的一类能力。

## LanymyAesCrypto 的关键特征

- 使用 `Aes.Create()`，模式为 `CBC`，填充为 `PKCS7`。
- Key 长度固定 32 字节，IV 长度固定 16 字节。
- 加密后会再走一次 `CompressionHelper.CompressBytesToBytes(...)`，解密前则先解压。
- 对调用层暴露的是字符串 / Base64 / 模型 / 文件等便捷接口。

## 维护时需要注意

- `LanymyCrypto` 不只是算法实现，还定义了项目内部的加密数据协议；任何改动都可能导致历史数据无法解密。
- `DESCryptoExtensions` 使用反射访问私有方法，本身就带有运行时兼容风险，升级框架时要重点关注。
- 模块中存在默认密钥、默认 IV、默认行为，这些都属于历史兼容逻辑，不能轻易“顺手优化”。
- 图像加密链路依赖 `System.Drawing` / `Bitmap`，跨平台影响需要单独评估。

## 当前风险点

- 自定义协议复杂度高，阅读成本和接手成本都不低。
- 异常处理并不总是显式抛出，部分路径失败后只返回空值或失败标记。
- 旧式 `TripleDES` 和弱密钥兼容逻辑从现代安全视角看都需要谨慎使用。

## 后续建议

- 后续可以单独补一份“LanymyCrypto 数据格式拆解文档”。
- 如果进入现代化改造阶段，应优先区分“历史数据兼容需求”和“新数据写入策略”。
