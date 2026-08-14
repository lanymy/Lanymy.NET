# RegexHelper

本文档用于分析 `Lanymy.Common.Helpers.RegexHelper` 模块。

## 模块定位

- 这是一个轻量正则校验辅助模块。
- 当前主要提供一组历史沿用的静态布尔校验入口。
- 该模块价值在于“模式边界稳定”，而不是复杂失败诊断。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.RegexHelper/RegexHelper.cs`

## 主要能力

- 文本合法性校验
  - `IsPhoneNumber(...)`
  - `IsAllNumeric(...)`
  - `IsPositiveNum(...)`
  - `IsChineseName(...)`
  - `IsChineseOrEnglishName(...)`
  - `IsChineseChar(...)`
  - `IsEmail(...)`
  - `IsUrl(...)`
  - `IsTime(...)`
  - `IsHexString(...)`
- 通配符匹配
  - `CheckWithWildcard(...)`
- 路径文本校验
  - `IsAbsolutePath(...)`
  - `IsRelativePath(...)`

## 实现特征

- 大部分入口都是单行 `Regex.IsMatch(...)` 包装。
- `IsAllNumeric(string.Empty)` 当前会返回 `true`，这是历史正则模式 `^[0-9]*$` 的直接结果。
- `CheckWithWildcard(...)` 通过 `Regex.Escape(...)` 再把 `*` / `?` 映射到正则语义。
- `IsRelativePath(...)` 当前对 `"."`、`"./"`、`"../"`、`"/"` 这一类输入有显式兼容处理，因此它的“相对路径”概念并不是严格的现代 URI 语义，而是历史仓库内部约定。

## 本轮治理结论

- **本轮不补 `WithResult(...)` 严格层。**
- 原因：
  - 该模块本质上是纯模式匹配工具。
  - 当前更重要的是把已有模式边界锁成测试，避免未来在“看起来更合理”的修改里悄悄改掉历史行为。

## 本轮新增回归点

- 手机号、纯数字、正整数、IPv4 的基础匹配语义。
- 中文姓名、中英文姓名、纯中文、十六进制大写文本的边界。
- Email、HTTP/HTTPS URL、时间文本的历史模式。
- 通配符 `*` / `?` 匹配规则。
- 绝对路径与相对路径的当前文本判定规则。

## 维护时需要注意

- 这里的若干模式明显带有历史业务假设，不应在未更新测试和文档前随意“优化”。
- 若未来需要更现代或更严格的验证规则，建议新增明确命名的新 API，而不是直接替换原有正则。
