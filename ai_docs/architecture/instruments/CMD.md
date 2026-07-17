# CMD

本文档用于分析 `Lanymy.Common.Instruments.CMD` 模块。

## 模块定位

- 这是仓库内对命令行执行能力的一层外观封装。
- 当前看到的具体实现 `LanymyCmd` 比较薄，主要负责把输出和错误输出转成回调。
- 模块真正的主体能力还依赖其抽象基类 `BaseCmd`。

## 当前代码入口

- `src/Commons/Lanymy.Common.Instruments.CMD/LanymyCmd.cs`

## 当前实现特征

- 构造函数允许注入两类回调：
  - `outputDataReceivedAction`
  - `errorDataReceivedAction`
- `OnOutputDataReceivedEvent(...)`
  - 把标准输出逐条转交给外部回调
- `OnErrorDataReceivedEvent(...)`
  - 把错误输出逐条转交给外部回调

## 设计意图

- 从当前类的形态看，模块目标不是把命令执行做成复杂编排系统，而是做一个“可订阅输出流”的命令执行器。
- 这很适合：
  - 工具型命令调用
  - 日志回显
  - 把 CLI 输出桥接到 UI / 调试日志

## 维护时需要注意

- 仅阅读 `LanymyCmd` 还不够理解整个模块，后续若继续深挖，需要同步补读 `CMD.Abstractions` 中的基类。
- 当前实现并不在此类中自行做输出缓存或错误聚合，而是直接回调分发。
- 如果后续要增强可靠性，关注点应该放在基类中的进程生命周期管理，而不是只改这个外观类。

## 当前认知边界

- 目前可以确认：
  - `LanymyCmd` 是对输出事件的轻量包装
  - 模块偏回调式、偏工具式
- 目前还不能仅凭该文件确认：
  - 命令启动参数策略
  - 退出码处理策略
  - 进程资源释放策略

## 后续建议

- 下一轮可以补一份 `CMD.Abstractions` 专题，把 `BaseCmd` 的完整生命周期整理出来。
- 在总文档里，当前先把 `CMD` 归类为“机制入口已见、生命周期细节待补”的模块更稳妥。
