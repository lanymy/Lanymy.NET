# 2026-08-12 多目标编译矩阵与 Warning 分类

## 本轮范围

- `Lanymy.Common.Instruments.WorkTask.Abstractions`
- `Lanymy.Common.Instruments.Socket.Abstractions`
- `Lanymy.Common.Instruments.Socket.Netty.Abstractions`

目标框架：

- `net48`
- `netstandard2.1`
- `net8.0`

## 编译矩阵结果

### `Lanymy.Common.Instruments.WorkTask.Abstractions`

- `net48`: 通过
- `netstandard2.1`: 通过
- `net8.0`: 通过

本轮新修复：

- `BaseSimpleWorkTaskQueue` 的 `ConcurrentQueue<T>.Clear()` 改为多目标兼容实现：
  - `net48` 使用 `TryDequeue`
  - `netstandard2.1` / `net8.0` 使用 `Clear()`

兼容性复核结论：

- 本项目内未再发现新的 `net48` 硬错误。
- `await foreach` 在当前依赖组合下可正常通过 `net48` 编译，本轮未构成实际兼容问题。
- 在继续完成 `net8.0` 平台/过时 API 治理后，当前结果为：
  - `net48`: `0 warning`
  - `netstandard2.1`: `0 warning`
  - `net8.0`: `0 warning`

### `Lanymy.Common.Instruments.Socket.Abstractions`

- `net48`: 通过
- `netstandard2.1`: 通过
- `net8.0`: 通过

结论：

- 本轮未发现新的项目内编译错误。
- 继续清理后，当前三目标均已达到 `0 warning / 0 error`。

### `Lanymy.Common.Instruments.Socket.Netty.Abstractions`

- `net48`: 通过
- `netstandard2.1`: 通过
- `net8.0`: 通过

本轮新修复：

- `BaseChannelHandler.ScheduleSendBytesAsync(...)` 之前使用：

```csharp
context.Executor.ScheduleAsync(() => WriteBytesAsync(context, bytes), delay)
```

- 这会让调度器内部的异步写入 task 变成悬空任务，造成：
  - 三目标下统一出现 `CS4014`
  - 延迟发送阶段的异步写入失败可能绕开 `OnHandlerError(...)`

- 现已改为：

```csharp
context.Executor.ScheduleAsync(async () => await WriteBytesAsync(context, bytes), delay)
```

修复后结果：

- `Socket.Netty.Abstractions` 三目标均不再出现该项目自身的 `CS4014`
- `SocketNettyTests` 定向回归通过
- 继续清理后，当前三目标均已达到 `0 warning / 0 error`

## 新问题 vs 存量 warning

### 一类：真实兼容性/行为风险

本轮确认的新问题：

1. `BaseSimpleWorkTaskQueue`
   - 问题：`ConcurrentQueue<T>.Clear()` 不兼容 `net48`
   - 状态：已修复

2. `BaseChannelHandler`
   - 问题：延迟发送调度没有串接真实 `WriteBytesAsync(...)` task
   - 表现：`CS4014` + 发送异常可能静默丢失
   - 状态：已修复

本轮新增收口：

- `ImageHelper`
  - 通过 `SupportedOSPlatform("windows")` 明确声明 `System.Drawing` 的平台边界

- `LanymyCrypto`
  - 将 `TripleDESCryptoServiceProvider` 替换为 `TripleDES.Create()`
  - 为 Bitmap / ImageFile 相关能力补齐 Windows 平台约束属性

- `SecurityHelper`
  - 将 `MD5CryptoServiceProvider` 替换为 `MD5.Create()`
  - 为图片加解密 wrapper 补齐 Windows 平台约束属性

- `FileHelper`
  - 将 `HashAlgorithm.Create(string)` 替换为显式算法映射工厂

### 二类：文档 / XML 注释噪音

主要特征：

- `CS1572`
- `CS1573`
- 少量 `CS0168`

高频来源：

- `Lanymy.Common.Instruments.Crypto.Abstractions`
- `Lanymy.Common.Instruments.Crypto`
- `Lanymy.Common.Helpers.CompressionHelper`
- `Lanymy.Common.Helpers.SecurityHelper`
- 个别 `ObjectExtensions` / `PushFileStreamModel` / `Crawler`

结论：

- 这些 warning 在三个目标框架下大多重复出现，属于典型存量问题。
- 它们当前主要影响构建噪音和可读性，不是本轮阻塞编译的原因。

建议后续单独按“接口注释修补”专题处理，不要混在生命周期治理里一起改。

### 三类：`net8.0` 平台分析器 / 过时 API 提示

主要特征：

- `CA1416`
- `SYSLIB0021`
- `SYSLIB0045`

高频来源：

- `ImageHelper`
- `LanymyCrypto`
- `SecurityHelper`
- `FileHelper`
- `PcInfoHelper`

结论：

- 这类 warning 主要反映：
  - Windows-only API 缺少平台隔离或注解
  - 部分加密 API 已被 `net8.0` 标记为过时

它们不是本轮新增 warning，但属于后续值得分专题治理的真实工程债。

当前状态：

- 本清单中优先处理的 `ImageHelper` / `LanymyCrypto` / `SecurityHelper` / `FileHelper` 已全部完成
- `PcInfoHelper` 的 `Type.GetTypeFromProgID(...)` 平台 warning 也已收口，并补齐了快捷方式相关 wrapper 的 Windows 平台约束
- `PcInfoHelperTests` 已补齐调用侧 `SupportedOSPlatform("windows")` 约束，测试项目不再残留该条 `CA1416`
- `Lanymy.Common.Instruments.Crawler` 已清理 `LanymyDownloadCrawler` 中未使用的 `catch` 变量，不再残留该条 `CS0168`
- `PipeLineHandlerContext` 已补齐缺失的泛型 `typeparam` XML 注释，`PipeLine` 项目不再残留该组 `CS1712`
- 公共打包配置已统一补齐 `PackageReadmeFile` 与 `README.md` 打包项，用于收口全仓库 NuGet 缺少 readme 的打包 warning
- 公共构建链已补齐多目标外层 `GetTargetPath` 兼容 shim，用于兼容旧式 Xamarin 项目对 SDK 多目标项目引用的输出路径解析
- 相关项目在 `net48` / `netstandard2.1` / `net8.0` 下均已回编通过
- `WorkTask.Abstractions` 聚合编译在 `net8.0` 下也已达到 `0 warning / 0 error`
- 当前再次执行 `dotnet build src/Lanymy.NET.sln --no-restore -v minimal` 时，已收敛为 `0 warning`
- 当前再次执行 `dotnet build src/Lanymy.NET.sln --no-restore -v minimal` 时，已达到 `0 warning / 0 error`

## 建议的后续执行顺序

1. 先清一轮 XML 注释 warning
   - 收益高
   - 风险低
   - 可以明显降低构建噪音

2. 若继续扩展 warning 治理，下一组建议转向其他带平台依赖但当前未报 warning 的模块，重点复核调用边界是否需要显式平台声明

3. 再评估是否需要对部分平台限定 API 增加运行时显式保护（如主动抛 `PlatformNotSupportedException`），而不只是编译期属性提示

## 2026-08-13 补充收口

### `BaseUdpClient` 生命周期拖尾问题

在 `warning / build` 收口完成后，`Lanymy.Common.AllTests` 的 `net8.0` 全量回归一度出现 testhost 退出拖尾。

排查结论：

- 之前 `BaseUdpClient.ReciveCallBack(...)` 在关闭窗口内会过早返回，旧的 `BeginReceive(...)` 回调没有先 `EndReceive(...)` 收尾。
- `CloseAsync()` 之前又是“先停 receive/send queue，最后才关 `UdpClient`”的顺序。
- 两者叠加后，在 `CloseAsync_WhenQueueStopFails_ShouldReplaceFailedQueuesBeforeRestart` 这类“关闭失败后替换队列并重启”的路径里，旧异步接收可能残留到当前生命周期之外，最终把 testhost 退出拖住。

本轮修复：

- `BeginReceive(...)` 改为把当前 `UdpClient` 通过 `asyncState` 传入回调。
- `ReciveCallBack(...)` 改为即使关闭窗口里也先对旧实例执行 `EndReceive(...)`，再判断是否继续分发或续接收。
- `CloseAsync()` 改为：
  - 先摘除 `_CurrentUdpClient`
  - 先关闭 / 释放旧 `UdpClient`
  - 再停止 receive / send queue

新增回归：

- `BaseUdpClient_StaleReceiveCallbackAfterClose_ShouldEndReceiveWithoutRedispatching`
  - 锁定“关闭后旧回调仍需先收尾，但不能把旧数据重新分发进当前生命周期”

验证结果：

- `UdpLifecycleTests`: `27/27 passed`
- `BaseUdpClient_CloseAsync_WhenQueueStopFails_ShouldReplaceFailedQueuesBeforeRestart`: 定向通过

### `PcInfoHelperTests` 环境适配

之前全量回归里另一个干扰项，不是功能失败，而是测试直接调用 `CreateDesktopShortcut(...)`，向真实桌面写入 `ShortcutDemo.lnk`，在当前沙箱环境下被拦截。

本轮调整：

- 测试改为直接验证 `CreateShortcut(...)`
- 目标路径改为 `Path.GetTempPath()` 下的临时目录
- 测试结束后自动删除临时目录与 `.lnk`

验证结果：

- `PcInfoHelperTests`: `1/1 passed`

### 最新全量结果

- `dotnet test src/UnitTests/Lanymy.Common.AllTests/Lanymy.Common.AllTests.csproj --framework net8.0 --no-restore --blame-hang-timeout 2m -v minimal`
- 结果：`204/204 passed`
