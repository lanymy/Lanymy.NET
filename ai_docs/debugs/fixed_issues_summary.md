# 已修问题汇总

本文档用于汇总当前这一轮源码复验过程中已经完成修复的问题、对应文件，以及当前可确认的验证状态。

## 当前范围

- 本页只记录已经实际改动源码的问题。
- 不包含“已复验但判定为设计限制”的项。
- 不包含“已排除为误报”的项，误报仍保留在复验记录中方便回溯。

## 已修清单

### 1. `HttpHelper` 泛型 GET 错误转发

- 问题：
  - `HttpGetAsync<TReturnDataModel>` 原先错误调用了 `HttpPostAsync(...)`
- 修复文件：
  - [HttpHelper.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Helpers.HttpHelper/HttpHelper.cs)
- 当前处理：
  - 已改为调用真正的 `HttpGetAsync(url, parameters)`
- 影响说明：
  - 修复后，泛型 GET 与字符串版 GET 的 HTTP 语义重新一致

### 2. `HttpHelper` 泛型 multipart 错误转发

- 问题：
  - `HttpPostMultipartFormDataAsync<TReturnDataModel>` 原先错误转发到普通 `HttpPostAsync(...)`
- 修复文件：
  - [HttpHelper.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Helpers.HttpHelper/HttpHelper.cs)
- 当前处理：
  - 已改为调用真正的 `HttpPostMultipartFormDataAsync(url, attachmentList)`
- 影响说明：
  - 修复后，泛型 multipart 与非泛型 multipart 的提交行为重新一致

### 3. `CustomMemoryCache.SetValue` 不覆盖新值

- 问题：
  - `AddOrUpdate(key, value, (k, v) => v)` 更新分支返回旧值
- 修复文件：
  - [CustomMemoryCache.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Cache.CustomMemoryCache/CustomMemoryCache.cs)
  - [CustomMemoryCacheTests.cs](file:///E:/Code/Git/My/Lanymy.NET/src/UnitTests/Lanymy.Common.AllTests/CustomMemoryCacheTests.cs)
- 当前处理：
  - 已改为返回新值 `value`
- 影响说明：
  - 同 key 二次写入现在会真正覆盖缓存值

### 4. `BaseUdpClient.Close()` 队列释放条件方向错误

- 问题：
  - 原先只有在队列为 `null` 时才进入停止与释放分支
- 修复文件：
  - [BaseUdpClient.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseUdpClient.cs)
- 当前处理：
  - 已改为队列非空时才执行 `StopAsync()`、`Dispose()` 和置空
- 影响说明：
  - UDP 客户端关闭路径现在能够真实回收收发队列

### 5. `BaseTcpServer` 连续拆包递归改循环

- 问题：
  - `OnServerClientReceiveDataLoopEvent(...)` 原先递归处理连续包
- 修复文件：
  - [BaseTcpServer.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Instruments.Socket.Abstractions/BaseTcpServer.cs)
- 当前处理：
  - 已改为等价的 `while` 循环实现
- 影响说明：
  - 连续包处理不再额外放大调用深度

### 6. `ImageHelper.SaveBitmapToImageFile(...)` 隐式释放入参

- 问题：
  - 原先方法内部 `using (var image = encryptImage)` 会直接释放调用方传入对象
- 修复文件：
  - [ImageHelper.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Helpers.ImageHelper/ImageHelper.cs)
- 当前处理：
  - 已改为只执行保存，不再负责 `Bitmap` 生命周期
- 影响说明：
  - `Bitmap` 的所有权重新回到调用方，避免辅助方法偷偷改变对象可用性

### 7. `PathHelper` 路径类型判断过度依赖扩展名

- 问题：
  - 原先主要靠 `Path.HasExtension(...)` 判断文件/目录
- 修复文件：
  - [PathHelper.cs](file:///E:/Code/Git/My/Lanymy.NET/src/Commons/Lanymy.Common.Helpers.PathHelper/PathHelper.cs)
  - [PathHelperTests.cs](file:///E:/Code/Git/My/Lanymy.NET/src/UnitTests/Lanymy.Common.AllTests/PathHelperTests.cs)
- 当前处理：
  - 已增强为“存在性优先 + 目录语法优先 + 扩展名兜底”
- 影响说明：
  - 已存在无扩展名文件、已存在带点号目录的误判风险已明显下降

## 已排除项

### `JsonNetJsonSerializer` 的 `hh / HH` 疑点

- 结论：
  - 这不是实现错误，而是注释误导
- 当前处理：
  - 已修正源码注释
- 参考：
  - [review_round_01.md](./review_round_01.md)

## 暂未改动项

### `BaseTcpClient` 不可重启限制

- 当前判断：
  - 现阶段更像“一次性对象”的设计限制，而不是已经坐实的实现错误
- 当前处理：
  - 暂未修改源码
- 继续观察条件：
  - 若后续发现真实调用方把它当作“可重连客户端”使用，再升级为修复项

## 验证状态

- 已完成：
  - 本轮涉及源码文件都做过 IDE 诊断检查，当前无新增诊断错误
  - `CustomMemoryCache` 与 `PathHelper` 已补最小单测文件，用于锁定回归边界
- 当前阻塞：
  - `dotnet test` 无法在当前环境稳定跑通
  - 已确认阻塞原因是 `NuGet` 锁文件访问受限，不是本轮源码修改直接导致
- 结论：
  - 当前可确认“静态修复已落地”
  - “自动化运行验证”仍需要在可正常还原 NuGet 的环境下补跑

## 建议后续

- 若下一步继续做质量治理，建议从以下方向进入第二轮：
  - 吞异常过多的路径
  - 布尔返回值但无失败详情的工具方法
  - 资源释放链与对象所有权不清晰的模块
- 若下一步需要阶段性交接，可把本页与 [review_round_01.md](./review_round_01.md) 一起作为当前修复阶段的总入口。
