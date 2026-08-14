# DateTimeHelper

本文档用于分析 `Lanymy.Common.Helpers.DateTimeHelper` 模块。

## 模块定位

- 这是一个轻量时间换算辅助模块。
- 当前职责主要是：
  - 若干固定基准时间戳换算
  - 季度 / 周一日期计算
  - 毫秒间隔与 Java 时间戳转换
- 它更接近纯计算 helper，而不是需要补严格结果层的外部边界模块。

## 当前代码入口

- `src/Commons/Lanymy.Common.Helpers.DateTimeHelper/DateTimeHelper.cs`

## 主要能力

- `GetTotalSecondsFrom1970(...)` / `GetDateTimeFrom1970TotalSeconds(...)`
- `GetTotalMillisecondsFrom2000(...)` / `GetDateTimeFrom2000TotalMilliseconds(...)`
- `GetTotalSecondsFromInstantiation(...)` / `GetDateTimeFromInstantiationTotalSeconds(...)`
- `GetQuarterStartDate(...)`
- `GetQuarterLastDate(...)`
- `GetCurrentQuarter(...)`
- `GetIntervalMilliseconds(...)`
- `GetDaysInMonth(...)`
- `GetMondayDateByDate(...)`
- `GetDateTimeFromJavaLongDateTime(...)`

## 实现特征

- 绝大多数入口都是纯计算，没有文件、网络、进程或系统状态写入。
- “实例化时间”基线来自 `DateTimeFormatKeys.START_DATE_TIME_INSTANTIATION`，属于进程加载期捕获的静态时间戳。
- `GetIntervalMilliseconds(...)` 当前仍返回 `int`，说明该模块保留了历史轻量返回风格。
- Java Long 时间转换默认按东八区回填。

## 本轮治理结论

- **本轮不补 `WithResult(...)` 严格层。**
- 原因：
  - 当前模块的价值不在失败诊断，而在时间换算语义是否稳定。
  - 更适合通过定向测试锁定基准点、边界日期和符号语义。

## 本轮新增回归点

- 1970 / 2000 基线下的秒级、毫秒级回环。
- 实例化时间基线下的秒级、毫秒级回环。
- 季度开始日、结束日与季度枚举映射。
- `GetIntervalMilliseconds(...)` 的正负号语义。
- `GetMondayDateByDate(...)` 对工作日和周日都回到同一周周一。
- 闰年二月天数与 Java Long 时间戳转换。

## 维护时需要注意

- 该模块里一部分 API 是秒级，一部分是毫秒级，新增能力时要避免单位混淆。
- “实例化时间”并不是固定纪元，而是进程级动态基线；若未来跨进程持久化该值，需要单独设计协议，不应直接复用这里的轻量入口。
