# CMD-Semantics

本文档用于补充 `CMD` 机制中的“成功 / 失败语义”理解。

## 文档目的

- 这一页不再重复 [CMD.Abstractions.md](./CMD.Abstractions.md) 的执行流程。
- 重点回答一个容易误判的问题：
  - `CmdResultModel.IsSuccess = true` 到底意味着什么？

## 当前结论

- 当前实现里，`IsSuccess = true` 仅表示：
  - `cmd` 进程成功启动
  - 命令字符串成功写入
  - 输出读取流程正常结束
  - 整个执行过程没有抛出异常
  - 退出码为 `0`
- 它**不直接表示**：
  - 命令返回结果符合业务预期
  - 标准错误输出为空

## 证据来源

- `BaseCmd.ExecuteCommand(string cmdString)` 中：
  - 在 `WaitForExit()` 后会读取 `process.ExitCode`
  - 只有在“没有异常 + `ExitCode == 0`”时才会把 `IsSuccess = true`
  - 当前代码不会仅凭 `ErrorDataString` 有内容就反推失败

## 当前成功语义分层

可以把当前返回结果理解成三层：

1. 进程执行层成功
   - 由 `IsSuccess` 表示
2. 进程退出层
   - 由 `ExitCode` 表示
3. 标准流结果层
   - 由 `OutputDataString` / `ErrorDataString` 表示
4. 业务语义层
   - 需要调用方自己解析输出内容后判断

## 典型场景举例

- 场景 1：命令拼写错误，但 `cmd` 正常执行并输出错误文本
  - 可能结果：
    - `IsSuccess = false`
    - `ExitCode != 0`
    - `ErrorDataString` 有内容
- 场景 2：命令执行过程中 `Process.Start()` 抛异常
  - 可能结果：
    - `IsSuccess = false`
    - `Exception` 有值
- 场景 3：命令业务失败，但工具本身没有抛异常
  - 可能结果：
    - `IsSuccess = false`
    - `ExitCode != 0`
    - 输出内容仍然需要调用方自己判定失败细节

## 当前调用建议

- 如果只是把 `CMD` 当成“触发外部命令”的工具，`IsSuccess` 可以当作流程级成功标记。
- 如果把 `CMD` 用于自动化任务、构建、发布、脚本编排，就不能只看 `IsSuccess`。
- 更稳妥的判断方式应至少结合：
  - `IsSuccess`
  - `ExitCode`
  - `ErrorDataString`
  - `OutputDataString`
  - 调用方自己的输出规则解析

## 当前设计带来的好处

- 已经与 shell 习惯对齐，非零退出码不会再被误判成成功。
- 不强制把“stderr 有内容”判成失败，避免误杀一些会把警告写到错误流的命令。

## 当前设计带来的限制

- 自动化场景下仍不能只看 `IsSuccess`，因为很多工具会把详细失败原因写在输出内容里。
- 仍缺少超时、取消等执行控制能力。

## 后续建议

- 若后续继续完善 `CMD`，建议新增：
  - `HasErrorOutput`
  - 可选的“严格成功模式”
- 在使用文档里，建议把 `IsSuccess` 表述为“进程执行成功且退出码为 0”，避免误导为“命令业务成功”。
