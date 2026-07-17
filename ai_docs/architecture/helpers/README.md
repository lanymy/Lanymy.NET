# helpers

本目录用于分析 `Lanymy.Common.Helpers.*` 模块族。

## 范围

- 文件、路径、网络、进程
- 序列化、压缩、加密
- 图片、二维码、版本、验证码等

## 结构特征

- 以静态帮助类为主
- 强调开箱即用
- 适合直接调用，但后续现代化改造需要关注可测试性和 API 一致性

## 典型模块

- `HttpHelper`
- `FileHelper`
- `PathHelper`
- `NetworkHelper`
- `ProcessHelper`
- `ImageHelper`
- `QrCodeHelper`
- `SecurityHelper`
- `SerializeHelper.*`

## 专题文档

| 文档 | 用途 |
|------|------|
| [HttpHelper.md](./HttpHelper.md) | HTTP 辅助模块专题 |
| [FileHelper.md](./FileHelper.md) | 文件辅助模块专题 |
| [ImageHelper.md](./ImageHelper.md) | 图片辅助模块专题 |
| [NetworkHelper.md](./NetworkHelper.md) | 网络辅助模块专题 |
| [PathHelper.md](./PathHelper.md) | 路径辅助模块专题 |
| [ProcessHelper.md](./ProcessHelper.md) | 进程辅助模块专题 |
| [SecurityHelper.md](./SecurityHelper.md) | 安全辅助模块专题 |
| [JsonSerializeHelper.md](./JsonSerializeHelper.md) | JSON 序列化辅助模块专题 |
| [QrCodeHelper.md](./QrCodeHelper.md) | 二维码辅助模块专题 |

## 当前维护重点

- 避免 Helpers 之间重复封装相近能力
- 识别哪些模块只是轻量包装，哪些模块已经承载较重逻辑
- 评估历史依赖对跨平台与新框架的约束

## 当前风险点

- 静态帮助类风格虽然易用，但在可测试性、依赖注入和行为配置方面扩展性有限。
- 某些模块仍带有明显历史依赖痕迹，直接现代化替换前需要确认兼容目标。

## 后续建议补充

- 高频 Helpers 一览
- 典型 API 风格总结
- 需要现代化改造的候选模块
