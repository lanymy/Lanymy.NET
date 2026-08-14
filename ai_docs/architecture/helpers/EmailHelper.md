# EmailHelper

本文档用于分析 `Lanymy.Common.Helpers.EmailHelper` 模块。

## 模块定位

- 提供基于 SMTP 的轻量邮件发送能力。
- 当前实现仍以同步发送为主，适合作为工具型发信入口，而不是高吞吐邮件投递框架。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.EmailHelper/EmailHelper.cs`

## 主要能力

- 兼容层发送入口
  - `SendEmail(...)`
- 严格结果模式发送入口
  - `SendEmailWithResult(...)`

## 结果语义

- `SendEmail(...)` 继续返回 `CommonResultModel`，保持原有兼容入口形态。
- `SendEmailWithResult(...)` 返回 `EmailSendResultModel`，显式给出：
  - `SmtpServer`
  - `SenderAddress`
  - `Subject`
  - `SenderDisplayName`
  - `Port`
  - `EnableSsl`
  - `IsUnpackSendMail`
  - `BodyEncodingName`
  - `RequestedRecipientAddresses`
  - `RecipientResults`
  - `SuccessCount`
  - `FailureCount`
- 当 `isUnpackSendMail = true` 时，当前实现会逐个收件人发送，并把每一项结果记录到 `RecipientResults`。
- 当 `isUnpackSendMail = false` 时，当前实现也会先规范化收件人列表；批量 SMTP 调用失败时，会把同一异常映射到每个收件人结果，避免只留下整体失败而没有逐项明细。

## 本轮收口点

- 修正了拆分发送模式下“逐项发送失败但整体仍被标成成功”的历史弱语义。
- 当前严格层会在逐项发送失败时保留首个异常，并同步返回每个收件人的成功/失败明细。
- 当前严格层还会保留本次请求的收件人列表、编码和发送模式；即使是批量一次发送失败，也不会丢掉逐收件人诊断信息。
- 兼容层 `SendEmail(...)` 仍保持旧签名，但内部已复用严格层结果，不再丢失整体失败状态。

## 维护时需要注意

- 该模块仍使用同步 `SmtpClient.Send(...)`，不适合直接承担大批量并发发送。
- 当前结果模型主要解决“诊断信息缺失”问题，还没有引入超时、取消或重试策略。
- 多收件人非拆分模式下，结果仍按单次 SMTP 调用建模；若未来要进一步细化投递状态，需要继续扩展结果模型。

## 后续建议

- 若后续有更复杂邮件场景，优先继续增强严格结果层，而不是继续在兼容层叠加布尔/弱结果语义。
