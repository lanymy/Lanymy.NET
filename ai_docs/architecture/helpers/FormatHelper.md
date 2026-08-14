# FormatHelper

本文档用于分析 `Lanymy.Common.Helpers.FormatHelper` 模块。

## 模块定位

- 这是一个轻量格式化辅助模块。
- 当前职责集中在：
  - XML 缩进格式化
  - Base64 字符串与“可作为文件名/目录名”的文本互转
- 这是典型的纯转换 helper，不属于需要新增严格结果层的模块。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.FormatHelper/FormatHelper.cs`

## 主要能力

- `FormatXml(string xmlStr)`
- `FormatXml(XmlDocument xmlDocument)`
- `FormatBase64StringToFileNameBase64String(...)`
- `FormatBase64StringFromFileNameBase64String(...)`
- `FormatBase64StringToDirectoryNameBase64String(...)`
- `FormatBase64StringFromDirectoryNameBase64String(...)`

## 实现特征

- `FormatXml(...)` 对空输入直接返回 `string.Empty`。
- 非空 XML 字符串会先装载到 `XmlDocument`，再统一走缩进输出。
- Base64 名称格式化当前只做最小字符替换：
  - 原始 `/` 替换为 `@`
  - 反向再把 `@` 还原为 `/`
- 文件名和目录名格式化当前共用同一套规则。

## 本轮治理结论

- **本轮不补 `WithResult(...)` 严格层。**
- 原因：
  - 当前模块没有外部边界写入，只是纯文本和对象格式转换。
  - 更适合通过测试锁定空输入、格式化输出和可逆性，而不是新增结果模型。

## 本轮新增回归点

- `FormatXml(string)` 会输出带缩进的 XML 文本。
- 空字符串与空 `XmlDocument` 输入继续返回 `string.Empty`。
- Base64 文件名/目录名转换保持可逆。

## 维护时需要注意

- 当前 Base64 名称规则只处理 `/`，并不等同于完整的文件系统安全编码协议。
- 若未来要支持更多平台或 URI 语义，建议新增更明确的编码 API，而不是在当前方法上悄悄扩大替换范围。
