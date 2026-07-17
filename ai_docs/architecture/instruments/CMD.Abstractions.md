# CMD.Abstractions

本文档用于分析 `Lanymy.Common.Instruments.CMD.Abstractions` 模块。

## 模块定位

- 这是 `CMD` 机制的抽象基座。
- 上一层的 `LanymyCmd` 只是输出回调外观，真正的命令执行生命周期、结果模型与标准流收集逻辑都在这里。

## 当前代码入口

- `src/Commons/Lanymy.Common.Instruments.CMD.Abstractions/BaseCmd.cs`
- `src/Commons/Lanymy.Common.Instruments.CMD.Abstractions/ResultModels/BaseCmdResultModel.cs`
- `src/Commons/Lanymy.Common.Instruments.CMD.Abstractions/ResultModels/CmdResultModel.cs`

## BaseCmd 的执行流程

1. 构造一个固定的 `cmd` 进程启动配置：
   - `ProcessHelper.GetProcessStartInfo("cmd", true)`
2. 每次执行前清空：
   - `_OutputDataReceivedMessage`
   - `_ErrorDataReceivedMessage`
3. 创建 `CmdResultModel`，记录：
   - `CmdID`
   - `ExecuteCommandString`
   - `CmdStartDateTime`
4. 启动 `cmd`
5. 订阅：
   - `OutputDataReceived`
   - `ErrorDataReceived`
6. 开始异步读取标准输出和错误输出
7. 通过 `StandardInput.WriteLine(cmdString)` 把命令写入 `cmd`
8. 关闭标准输入并等待进程退出
9. 回填输出、错误输出、结束时间、异常与成功标记

## 结果模型设计

- `BaseCmdResultModel` 记录：
  - 命令唯一 ID
  - 执行命令字符串
  - 正常输出
  - 错误输出
  - 开始/结束时间
  - 异常对象
  - `IsSuccess`
- `CmdResultModel` 当前只是一个空派生类，说明仓库为未来扩展结果模型留了类型位。

## 实现特征

- `BaseCmd` 同时提供同步与异步入口，但异步版本本质上是 `Task.Run(() => ExecuteCommand(...))` 包装。
- 输出与错误输出会同时：
  - 写入内部 `StringBuilder`
  - 触发派生类回调
- 这解释了为什么 `LanymyCmd` 只需要关心“如何消费事件”，不用自己负责进程启动细节。

## 维护时需要注意

- 这里的 `IsSuccess` 只表示“命令执行流程是否走完且未抛异常”，并不等价于命令本身业务成功。
- 当前没有单独记录进程退出码。
- 通过 `cmd` 标准输入写入命令，而不是直接启动目标可执行文件，这更通用，但也意味着行为受 shell 语义影响。
- `ExecuteCommand(string cmd, params string[] args)` 最终还是拼成单条字符串执行，因此参数转义问题需要调用方自行注意。

## 风险点

- 若命令本身输出错误文本但未抛异常，`IsSuccess` 仍可能为 `true`。
- 结果模型没有退出码字段，后续若接入更严格的自动化场景会受限。
- 当前实现依赖 Windows `cmd` 语义，跨平台适配并不是现成可用状态。

## 与上层的关系

- `LanymyCmd` 负责“输出如何被消费”。
- `BaseCmd` 负责“命令如何被执行、输出如何被采集、结果如何被封装”。

## 后续建议

- 可继续补一份“CMD 成功语义与错误语义”专题。
- 若未来要增强自动化稳定性，建议新增退出码、超时、取消和参数安全拼接支持。
- 当前已补充专题见 [CMD-Semantics.md](./CMD-Semantics.md)。
